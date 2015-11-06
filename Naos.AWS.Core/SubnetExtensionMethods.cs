// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubnetExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    using Subnet = Naos.AWS.Contract.Subnet;

    /// <summary>
    /// Operations to be performed on Subnet.
    /// </summary>
    public static class SubnetExtensionMethods
    {
        /// <summary>
        /// Create a new subnet.
        /// </summary>
        /// <param name="subnet">Subnet to create.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static async Task<Subnet> CreateAsync(this Subnet subnet, CredentialContainer credentials = null)
        {
            var localSubnet = subnet.DeepClone();
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localSubnet.Region);

            var request = new CreateSubnetRequest()
                              {
                                  AvailabilityZone = localSubnet.AvailabilityZone,
                                  CidrBlock = localSubnet.Cidr,
                                  VpcId = localSubnet.ParentVpc.Id
                              };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);

                localSubnet.Id = response.Subnet.SubnetId;
            }

            await localSubnet.TagNameInAwsAsync(credentials);

            return localSubnet;
        }

        /// <summary>
        /// Deletes a subnet.
        /// </summary>
        /// <param name="subnet">Subnet to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        public static async Task DeleteAsync(this Subnet subnet, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(subnet.Region);

            var request = new DeleteSubnetRequest() { SubnetId = subnet.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
