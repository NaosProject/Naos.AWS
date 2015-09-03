// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Linq;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    using Newtonsoft.Json;

    using Instance = Naos.AWS.Contract.Instance;
    using InstanceState = Naos.AWS.Contract.InstanceState;
    using UserData = Naos.AWS.Contract.UserData;

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
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static Instance Create(this Instance instance, UserData userData, CredentialContainer credentials = null)
        {
            var localInstance = instance.DeepClone();
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localInstance.Region);

            var placement = new Placement(localInstance.ContainingSubnet.AvailabilityZone);
            var amiId = localInstance.Ami.DiscoverId(credentials);
            localInstance.Ami.Id = amiId; // assign for returning to caller (gives visibility for potential debug needs)
            var blockDeviceMappings = localInstance.MappedVolumes.ToAwsBlockDeviceMappings();
            var request = new RunInstancesRequest()
                              {
                                  BlockDeviceMappings = blockDeviceMappings,
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
                var response = client.RunInstances(request);
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
            localInstance.TagNameInAws(credentials);

            // wait until instance is "running" before proceeding.
            localInstance.WaitForState(InstanceState.Running, credentials);

            // re-fetch the instance details now that everything should be setup
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var describeInstanceRequest = new DescribeInstancesRequest()
                                                  {
                                                      InstanceIds =
                                                          new[] { localInstance.Id }.ToList()
                                                  };

                var describeInstanceResponse = client.DescribeInstances(describeInstanceRequest);
                Validator.ThrowOnBadResult(describeInstanceRequest, describeInstanceResponse);

                newInstance = describeInstanceResponse.Reservations.Single().Instances.Single();
            }

            // save the EBS volume ID's and tag with name
            foreach (var mapping in newInstance.BlockDeviceMappings)
            {
                var contractMapping = localInstance.MappedVolumes.Single(_ => _.DeviceName == mapping.DeviceName);
                contractMapping.Id = mapping.Ebs.VolumeId;
                contractMapping.TagNameInAws(credentials);
            }

            if (localInstance.ElasticIp != null)
            {
                if (string.IsNullOrEmpty(localInstance.ElasticIp.Id)
                    || string.IsNullOrEmpty(localInstance.ElasticIp.PublicIpAddress))
                {
                    localInstance.ElasticIp = localInstance.ElasticIp.Allocate(credentials);
                    localInstance.ElasticIp.ExistsOnAws(credentials);
                }

                localInstance.ElasticIp.AssociateToInstance(localInstance.Id, credentials);
            }

            // update the source destination check attribute now that this is created
            var updateSourceDestinationCheckRequest = new ModifyInstanceAttributeRequest()
                                                          {
                                                              InstanceId = localInstance.Id,
                                                              SourceDestCheck = localInstance.EnableSourceDestinationCheck,
                                                          };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var updateSourceDestinationCheckResponse = client.ModifyInstanceAttribute(updateSourceDestinationCheckRequest);
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
        public static bool ExistsOnAws(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

                var response = client.DescribeInstances(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Reservations.Any(_ => _.Instances.Any(__ => __.InstanceId == instance.Id));
            }
        }

        /// <summary>
        /// Gets the state of an instance.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>State of instance.</returns>
        public static InstanceState GetState(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new DescribeInstanceStatusRequest()
                              {
                                  IncludeAllInstances = true, // necessary to get state of a terminated instance...
                                  InstanceIds = new[] { instance.Id }.ToList()
                              };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.DescribeInstanceStatus(request);
                Validator.ThrowOnBadResult(request, response);

                var specificInstanceResult = response.InstanceStatuses.SingleOrDefault(_ => _.InstanceId == instance.Id);
                if (specificInstanceResult != null)
                {
                    var stateCode = specificInstanceResult.InstanceState.Code;
                    return (InstanceState)stateCode;
                }
                else
                {
                    return InstanceState.Unknown;
                }
            }
        }

        /// <summary>
        /// Waits until the instance gets to a certain expected state.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="expectedState">State of instance to wait for.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void WaitForState(this Instance instance, InstanceState expectedState, CredentialContainer credentials = null)
        {
            WaitUntil.SuccessIsReturned(() => instance.GetState(credentials) == expectedState);
        }

        /// <summary>
        /// Deletes an instance.
        /// </summary>
        /// <param name="instance">Instance to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Terminate(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new TerminateInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.TerminateInstances(request);
                Validator.ThrowOnBadResult(request, response);
            }

            instance.WaitForState(InstanceState.Terminated, credentials);
        }

        /// <summary>
        /// Gets the administrator password for an instance.
        /// </summary>
        /// <param name="instance">Instance to get the administrator password for.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Administrator password for provided instance.</returns>
        public static string GetAdministratorPassword(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new GetPasswordDataRequest() { InstanceId = instance.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.GetPasswordData(request);
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
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Stop(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new StopInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.StopInstances(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Starts the instance.
        /// </summary>
        /// <param name="instance">Instance to start.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Start(this Instance instance, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(instance.Region);

            var request = new StartInstancesRequest() { InstanceIds = new[] { instance.Id }.ToList() };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.StartInstances(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
