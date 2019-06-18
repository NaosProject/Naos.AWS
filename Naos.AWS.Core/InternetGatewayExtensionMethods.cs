// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetGatewayExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
    /// Operations to be performed on InternetGateways.
    /// </summary>
    public static class InternetGatewayExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="internetGateway">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(internetGateway.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeInternetGatewaysRequest() { InternetGatewayIds = new[] { internetGateway.Id }.ToList() };

                var response = await client.DescribeInternetGatewaysAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.InternetGateways.Any(_ => _.InternetGatewayId == internetGateway.Id);
            }
        }

        /// <summary>
        /// Deletes an internet gateway.
        /// </summary>
        /// <param name="internetGateway">Internet gateway to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task DeleteAsync(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(internetGateway.Region);

            var request = new Amazon.EC2.Model.DeleteInternetGatewayRequest() { InternetGatewayId = internetGateway.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
