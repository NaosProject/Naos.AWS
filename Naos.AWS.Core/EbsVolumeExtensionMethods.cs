// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolumeExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Amazon;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    /// <summary>
    /// Extension methods to convert internal objects to AWS SDK objects.
    /// </summary>
    public static class EbsVolumeExtensionMethods
    {
        /// <summary>
        /// Convert a CredentialContainer to AWSCredentials.
        /// </summary>
        /// <param name="volumes">Volumes to covert into block device mappings.</param>
        /// <returns>AWSCredentials using supplied values.</returns>
        public static List<BlockDeviceMapping> ToAwsBlockDeviceMappings(this List<EbsVolume> volumes)
        {
            return
                volumes.Select(
                    volume =>
                    new BlockDeviceMapping()
                        {
                            DeviceName = volume.DeviceName,
                            VirtualName = volume.VirtualName,
                            Ebs =
                                new EbsBlockDevice()
                                    {
                                        DeleteOnTermination = true,
                                        VolumeSize = volume.SizeInGb,
                                        VolumeType = volume.VolumeType
                                    }
                        }).ToList();
        }

        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="volume">Instance to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        public static bool ExistsOnAws(this EbsVolume volume, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(volume.Region);

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeVolumesRequest() { VolumeIds = new[] { volume.Id }.ToList() };

                var response = client.DescribeVolumes(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Volumes.Any(_ => _.VolumeId == volume.Id);
            }
        }
    }
}
