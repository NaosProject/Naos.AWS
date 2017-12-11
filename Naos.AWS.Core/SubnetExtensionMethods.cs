// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubnetExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;

    using Naos.AWS.Domain;

    /// <summary>
    /// Operations to be performed on Subnet.
    /// </summary>
    public static class SubnetExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="subnet">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this Subnet subnet, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(subnet.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeSubnetsRequest() { SubnetIds = new[] { subnet.Id }.ToList() };

                var response = await client.DescribeSubnetsAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.Subnets.Any(_ => _.SubnetId == subnet.Id);
            }
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

            var request = new Amazon.EC2.Model.DeleteSubnetRequest() { SubnetId = subnet.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
