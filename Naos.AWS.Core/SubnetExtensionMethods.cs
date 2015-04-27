// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubnetExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using Amazon;
    using Amazon.EC2.Model;

    using CuttingEdge.Conditions;

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
        public static void Create(this Subnet subnet, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(subnet.Region);

            var request = new CreateSubnetRequest()
                              {
                                  AvailabilityZone = subnet.AvailabilityZone,
                                  CidrBlock = subnet.Cidr,
                                  VpcId = subnet.ParentVpc.Id
                              };

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.CreateSubnet(request);
                Validator.ThrowOnBadResult(request, response);

                subnet.Id = response.Subnet.SubnetId;
            }

            subnet.TagNameInAws(credentials);
        }

        /// <summary>
        /// Deletes a subnet.
        /// </summary>
        /// <param name="subnet">Subnet to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Delete(this Subnet subnet, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(subnet.Region);

            var request = new DeleteSubnetRequest() { SubnetId = subnet.Id };

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.DeleteSubnet(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
