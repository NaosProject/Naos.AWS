// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VpcExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    using Vpc = Naos.AWS.Contract.Vpc;

    /// <summary>
    /// Operations to be performed on VPCs.
    /// </summary>
    public static class VpcExtensionMethods
    {
        /// <summary>
        /// Create a new VPC.
        /// </summary>
        /// <param name="vpc">VPC to create.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static Vpc Create(this Vpc vpc, CredentialContainer credentials = null)
        {
            var localVpc = vpc.DeepClone();
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localVpc.Region);

            var request = new CreateVpcRequest() { CidrBlock = localVpc.Cidr, InstanceTenancy = new Tenancy(localVpc.Tenancy) };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.CreateVpc(request);
                Validator.ThrowOnBadResult(request, response);

                localVpc.Id = response.Vpc.VpcId;
            }

            localVpc.TagNameInAws(credentials);

            return localVpc;
        }

        /// <summary>
        /// Deletes a VPC.
        /// </summary>
        /// <param name="vpc">VPC to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Delete(this Vpc vpc, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(vpc.Region);

            var request = new DeleteVpcRequest() { VpcId = vpc.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.DeleteVpc(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
