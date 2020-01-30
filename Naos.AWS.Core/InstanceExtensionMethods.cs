// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Domain;

    using Newtonsoft.Json;

    using Instance = Naos.AWS.Domain.Instance;
    using InstanceState = Naos.AWS.Domain.InstanceState;
    using InstanceStatus = Naos.AWS.Domain.InstanceStatus;
    using KeyPair = Naos.AWS.Domain.KeyPair;
    using SecurityGroup = Naos.AWS.Domain.SecurityGroup;
    using Subnet = Naos.AWS.Domain.Subnet;
    using UserData = Naos.AWS.Domain.UserData;

    /// <summary>
    /// Operations to be performed on instances.
    /// </summary>
    public static class InstanceExtensionMethods
    {
        /// <summary>
        /// Create a new Instance.
        /// </summary>
        /// <param name="instance">Instance to create.</param>
        /// <param name="userData">User data to use for instance creation.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static async Task<Instance> CreateAsync(this Instance instance, UserData userData, TimeSpan timeout, CredentialContainer credentials = null)
        {
            var localInstance = instance.DeepClone();
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localInstance.Region);

            var placement = new Placement(localInstance.ContainingSubnet.AvailabilityZone);
            var amiId = await localInstance.Ami.DiscoverIdAsync(credentials);
            localInstance.Ami.Id = amiId; // assign for returning to caller (gives visibility for potential debug needs)
            var blockDeviceMappings = localInstance.MappedVolumes.ToAwsBlockDeviceMappings();
            var request = new RunInstancesRequest()
                              {
                                  BlockDeviceMappings = blockDeviceMappings.ToList(),
                                  ClientToken = Guid.NewGuid().ToString().ToUpper(),
                                  DisableApiTermination = localInstance.DisableApiTermination,
                                  ImageId = amiId,
                                  InstanceType = localInstance.InstanceType,
                                  KeyName = localInstance.Key.KeyName,
                                  MinCount = 1,
                                  MaxCount = 1,
                                  Placement = placement,
                                  PrivateIpAddress = localInstance.PrivateIpAddress,
                                  SecurityGroupIds = new[] { localInstance.SecurityGroup.Id }.ToList(),
                                  SubnetId = localInstance.ContainingSubnet.Id,
                                  UserData = userData.ToBase64Representation(),
                              };

            Amazon.EC2.Model.Instance newInstance = null;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.RunInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);

                newInstance = response.Reservation.Instances.Single();
            }

            if (newInstance == null)
            {
                throw new ApplicationException(
                    "Instance failed to get created: " + JsonConvert.SerializeObject(localInstance));
            }

            // save the instance ID and tag the name in AWS
            localInstance.Id = newInstance.InstanceId;
            await localInstance.TagNameInAwsAsync(timeout, credentials);

            // wait until instance is "running" before proceeding.
            await WaitUntil.InstanceInState(
                localInstance,
                InstanceState.Running,
                new[] { InstanceState.ShuttingDown, InstanceState.Stopped, InstanceState.Stopping, InstanceState.Terminated },
                timeout,
                credentials);

            // re-fetch the instance details now that everything should be setup
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var describeInstanceRequest = new DescribeInstancesRequest()
                                                  {
                                                      InstanceIds = new[] { localInstance.Id }.ToList(),
                                                  };

                var describeInstanceResponse = await client.DescribeInstancesAsync(describeInstanceRequest);
                Validator.ThrowOnBadResult(describeInstanceRequest, describeInstanceResponse);

                newInstance = describeInstanceResponse.Reservations.Single().Instances.Single();
            }

            // save the EBS volume ID's and tag with name
            foreach (var mapping in newInstance.BlockDeviceMappings)
            {
                var domainMapping = localInstance.MappedVolumes.Single(_ => _.DeviceName == mapping.DeviceName);
                domainMapping.Id = mapping.Ebs.VolumeId;
                await domainMapping.TagNameInAwsAsync(timeout, credentials);
            }

            if (localInstance.ElasticIp != null)
            {
                if (string.IsNullOrEmpty(localInstance.ElasticIp.Id)
                    || string.IsNullOrEmpty(localInstance.ElasticIp.PublicIpAddress))
                {
                    localInstance.ElasticIp = await Creator.AllocateElasticIp(credentials, regionEndpoint.SystemName, localInstance.ElasticIp.Name);
                    await localInstance.ElasticIp.ExistsOnAwsAsync(credentials);
                }

                await localInstance.ElasticIp.AssociateToInstanceAsync(localInstance.Id, credentials);
            }

            // update the source destination check attribute now that this is created
            var updateSourceDestinationCheckRequest = new ModifyInstanceAttributeRequest()
                                                          {
                                                              InstanceId = localInstance.Id,
                                                              SourceDestCheck = localInstance.EnableSourceDestinationCheck,
                                                          };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var updateSourceDestinationCheckResponse = await client.ModifyInstanceAttributeAsync(updateSourceDestinationCheckRequest);
                Validator.ThrowOnBadResult(updateSourceDestinationCheckRequest, updateSourceDestinationCheckResponse);
            }

            return localInstance;
        }

        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

                var response = await client.DescribeInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Reservations.Any(reservation => reservation.Instances.Any(reservationInstance => reservationInstance.InstanceId == instance.Id));
            }
        }

        /// <summary>
        /// Fills the list.
        /// </summary>
        /// <param name="instances">List to fill. </param>
        /// <param name="region">Region to make call against.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Same collection operating on for fluent usage.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<IList<InstanceWithStatus>> FillFromAwsAsync(this IList<InstanceWithStatus> instances, string region, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeInstancesRequest();

                var response = await client.DescribeInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);

                var typedObjects =
                    response.Reservations.SelectMany(
                        reserveration =>
                        (reserveration.Instances != null && reserveration.Instances.Count > 0
                             ? reserveration.Instances
                             : new List<Amazon.EC2.Model.Instance>()).Select(
                                 _ =>
                                     {
                                         var instanceStatus = new InstanceStatus { InstanceState = _.State == null ? InstanceState.Unknown : (InstanceState)_.State.Code, };

                                         return new InstanceWithStatus()
                                                     {
                                                         InstanceStatus = instanceStatus,
                                                         Id = _.InstanceId,
                                                         InstanceType = _.InstanceType,
                                                         Ami = new Ami { Id = _.ImageId },
                                                         ElasticIp =
                                                             new ElasticIp
                                                                 {
                                                                     Region = region,
                                                                     PublicIpAddress = _.PublicIpAddress,
                                                                 },
                                                         ContainingSubnet = new Subnet { Id = _.SubnetId },
                                                         Region = region,
                                                         PrivateIpAddress = _.PrivateIpAddress,
                                                         Key = new KeyPair { Region = region, KeyName = _.KeyName },
                                                         SecurityGroup =
                                                             new SecurityGroup
                                                                 {
                                                                     Region = region,
                                                                     Id =
                                                                         (_.SecurityGroups.SingleOrDefault()
                                                                          ?? new GroupIdentifier()).GroupId,
                                                                     Name =
                                                                         (_.SecurityGroups.SingleOrDefault()
                                                                          ?? new GroupIdentifier()).GroupName,
                                                                 },
                                                         Tags =
                                                             _.Tags.ToDictionary(
                                                                 keyInput => keyInput.Key,
                                                                 valueInput => valueInput.Value),
                                                         MappedVolumes =
                                                             _.BlockDeviceMappings.Select(
                                                                 mapping =>
                                                                 new EbsVolume
                                                                     {
                                                                         DeviceName = mapping.DeviceName,
                                                                         Id = mapping.Ebs.VolumeId,
                                                                         Region = region,
                                                                     }).ToList(),
                                                     };
                                     })).ToList();

                // No AddRange on the interface...
                foreach (var instance in typedObjects)
                {
                    instances.Add(instance);
                }

                return instances;
            }
        }

        /// <summary>
        /// Gets the status of an instance.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Status of instance.</returns>
        public static async Task<InstanceStatus> GetStatusAsync(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new DescribeInstanceStatusRequest()
                              {
                                  IncludeAllInstances = true, // necessary to get state of a terminated instance...
                                  InstanceIds = new[] { instance.Id }.ToList(),
                              };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DescribeInstanceStatusAsync(request);
                Validator.ThrowOnBadResult(request, response);

                var specificInstanceResult = response.InstanceStatuses.SingleOrDefault(_ => _.InstanceId == instance.Id);
                if (specificInstanceResult != null)
                {
                    var stateCode = specificInstanceResult.InstanceState.Code;
                    var ret = new InstanceStatus
                                  {
                                      InstanceState = (InstanceState)stateCode,
                                      SystemChecks = specificInstanceResult.SystemStatus.Details.ToDictionary(key => key.Name.Value, value => (CheckState)Enum.Parse(typeof(CheckState), value.Status, true)),
                                      InstanceChecks = specificInstanceResult.Status.Details.ToDictionary(key => key.Name.Value, value => (CheckState)Enum.Parse(typeof(CheckState), value.Status, true)),
                                  };

                    return ret;
                }
                else
                {
                    var ret = new InstanceStatus
                        {
                            InstanceState = InstanceState.Unknown,
                            InstanceChecks = new Dictionary<string, CheckState>(),
                            SystemChecks = new Dictionary<string, CheckState>(),
                        };

                    return ret;
                }
            }
        }

        /// <summary>
        /// Gets the status of an instance.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <param name="shouldGetLatest">Should pull latest output; DEFAULT is false (consistent with AWS).</param>
        /// <returns>Status of instance.</returns>
        public static async Task<string> GetConsoleOutputAsync(this Instance instance, CredentialContainer credentials = null, bool shouldGetLatest = false)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new GetConsoleOutputRequest(instance.Id)
                          {
                              Latest = shouldGetLatest,
                          };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.GetConsoleOutputAsync(request);
                Validator.ThrowOnBadResult(request, response);

                var result = response.Output;
                return result;
            }
        }

        /// <summary>
        /// Deletes an instance.
        /// </summary>
        /// <param name="instance">Instance to delete.</param>
        /// <param name="timeout">Optional timeout to wait until object reaches <see cref="InstanceState.Terminated" />; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task TerminateAsync(this Instance instance, TimeSpan timeout, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new TerminateInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.TerminateInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }

            await WaitUntil.InstanceInState(instance, InstanceState.Terminated, new[] { InstanceState.Pending, InstanceState.Running }, timeout, credentials);
        }

        /// <summary>
        /// Gets the administrator password for an instance.
        /// </summary>
        /// <param name="instance">Instance to get the administrator password for.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Administrator password for provided instance.</returns>
        public static async Task<string> GetAdministratorPasswordAsync(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new GetPasswordDataRequest() { InstanceId = instance.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.GetPasswordDataAsync(request);
                Validator.ThrowOnBadResult(request, response);

                if (response.PasswordData == null)
                {
                    throw new NullPasswordDataException(instance.Id);
                }

                var decryptedPassword = response.GetDecryptedPassword(instance.Key.PrivateKey);
                return decryptedPassword;
            }
        }

        /// <summary>
        /// Stops the instance.
        /// </summary>
        /// <param name="instance">Instance to stop.</param>
        /// <param name="force">Force the instance to stop (default is FALSE).</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task StopAsync(this Instance instance, bool force = false, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new StopInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList(), Force = force };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.StopInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Starts the instance.
        /// </summary>
        /// <param name="instance">Instance to start.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task StartAsync(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new StartInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.StartInstancesAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Update the type of the instance in AWS to match the type of the object.
        /// </summary>
        /// <param name="instance">Instance to true up the type of.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task UpdateInstanceTypeAsync(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new ModifyInstanceAttributeRequest() { InstanceId = instance.Id, InstanceType = instance.InstanceType };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.ModifyInstanceAttributeAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
