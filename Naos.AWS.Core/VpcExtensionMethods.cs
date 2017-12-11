// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VpcExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Domain;

    using Vpc = Naos.AWS.Domain.Vpc;

    /// <summary>
    /// Operations to be performed on VPCs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
    public static class VpcExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="vpc">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this Vpc vpc, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(vpc.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeVpcsRequest() { VpcIds = new[] { vpc.Id }.ToList() };

                var response = await client.DescribeVpcsAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Vpcs.Any(_ => _.VpcId == vpc.Id);
            }
        }

        /// <summary>
        /// Deletes a VPC.
        /// </summary>
        /// <param name="vpc">VPC to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task DeleteAsync(this Vpc vpc, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(vpc.Region);

            var request = new DeleteVpcRequest() { VpcId = vpc.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteVpcAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
