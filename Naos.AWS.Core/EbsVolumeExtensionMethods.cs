// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolumeExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Domain;

    /// <summary>
    /// Extension methods to convert internal objects to AWS SDK objects.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
    public static class EbsVolumeExtensionMethods
    {
        /// <summary>
        /// Convert a CredentialContainer to AWSCredentials.
        /// </summary>
        /// <param name="volumes">Volumes to covert into block device mappings.</param>
        /// <returns>AWSCredentials using supplied values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static IReadOnlyCollection<BlockDeviceMapping> ToAwsBlockDeviceMappings(this IReadOnlyCollection<EbsVolume> volumes)
        {
            return
                volumes.Select(
                    volume =>
                        {
                            var volumeType = volume.VolumeType;
                            var iops = 0;
                            if (volume.VolumeType.StartsWith("io1", StringComparison.OrdinalIgnoreCase))
                            {
                                var dashSplit = volume.VolumeType.Split('-');
                                volumeType = dashSplit[0];

                                var maxIops = volume.SizeInGb * 30;
                                if (dashSplit.Length == 1)
                                {
                                    iops = maxIops;
                                }
                                else
                                {
                                    iops = int.Parse(dashSplit[1], CultureInfo.InvariantCulture);
                                }

                                if (iops > maxIops)
                                {
                                    throw new ArgumentException(FormattableString.Invariant($"Specified IOPS: {iops} was greated than allowed (30 IOPS:1GB): {maxIops}"));
                                }
                            }

                            var ebsBlockDevice = iops == 0
                                                     ? new EbsBlockDevice()
                                                           {
                                                               DeleteOnTermination = true,
                                                               VolumeSize = volume.SizeInGb,
                                                               VolumeType = volumeType,
                                                           }
                                                     : new EbsBlockDevice()
                                                           {
                                                               DeleteOnTermination = true,
                                                               VolumeSize = volume.SizeInGb,
                                                               VolumeType = volumeType,
                                                               Iops = iops,
                                                           };

                            return new BlockDeviceMapping()
                                             {
                                                 DeviceName = volume.DeviceName,
                                                 VirtualName = volume.VirtualName,
                                                 Ebs = ebsBlockDevice,
                                             };
                        }).ToList();
        }

        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="volume">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this EbsVolume volume, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(volume.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeVolumesRequest() { VolumeIds = new[] { volume.Id }.ToList() };

                var response = await client.DescribeVolumesAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Volumes.Any(_ => _.VolumeId == volume.Id);
            }
        }

        /// <summary>
        /// Creates the volume.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="availabilityZone">Availability zone of subnet.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static async Task<EbsVolume> CreateVolume(
            this EbsVolume volume,
            string availabilityZone,
            TimeSpan timeout,
            CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(volume.Region);

            var localVolume = volume.DeepClone();
            var volumeType = volume.VolumeType;

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new CreateVolumeRequest() { AvailabilityZone = availabilityZone, Size = volume.SizeInGb, };
                if (volume.VolumeType.StartsWith("io1", StringComparison.OrdinalIgnoreCase))
                {
                    var dashSplit = volume.VolumeType.Split('-');
                    volumeType = dashSplit[0];
                    int iops;
                    var maxIops = volume.SizeInGb * 30;
                    if (dashSplit.Length == 1)
                    {
                        iops = maxIops;
                    }
                    else
                    {
                        iops = int.Parse(dashSplit[1], CultureInfo.InvariantCulture);
                    }

                    if (iops > maxIops)
                    {
                        throw new ArgumentException(FormattableString.Invariant($"Specified IOPS: {iops} was greated than allowed (30 IOPS:1GB): {maxIops}"));
                    }

                    request.Iops = iops;
                }

                request.VolumeType = volumeType;

                var response = await client.CreateVolumeAsync(request);
                Validator.ThrowOnBadResult(request, response);

                await localVolume.TagNameInAwsAsync(timeout, credentials);

                localVolume.Id = response.Volume.VolumeId;
                return localVolume;
            }
        }

        /// <summary>
        /// Attaches to instance.
        /// </summary>
        /// <param name="volume">The volume.</param>
        /// <param name="instanceId">The instance identifier.</param>
        /// <param name="device">The device.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async.</returns>
        public static async Task AttachToInstance(
            this EbsVolume volume,
            string instanceId,
            string device,
            CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(volume.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new AttachVolumeRequest()
                              {
                                  Device = device,
                                  InstanceId = instanceId,
                                  VolumeId = volume.Id,
                              };

                var response = await client.AttachVolumeAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
