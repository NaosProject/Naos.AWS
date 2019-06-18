// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElasticIpExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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

    /// <summary>
    /// Extension methods on the Elastic IP object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
    public static class ElasticIpExtensionMethods
    {
        /// <summary>
        /// Releases an elastic IP.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to release.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task ReleaseAsync(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new ReleaseAddressRequest
                              {
                                  AllocationId = elasticIp.Id,
                              };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.ReleaseAddressAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Attaches the elastic IP to a specified instance.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to connect to the provided instance.</param>
        /// <param name="instanceId">Instance to attach elastic IP to.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task AssociateToInstanceAsync(this ElasticIp elasticIp, string instanceId, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new AssociateAddressRequest(instanceId, elasticIp.PublicIpAddress);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.AssociateAddressAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes the elastic IP from the specified instance.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to remove from the provided instance.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task DisassociateFromInstanceAsync(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new DisassociateAddressRequest(elasticIp.PublicIpAddress);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DisassociateAddressAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeAddressesRequest() { AllocationIds = new[] { elasticIp.Id }.ToList() };

                var response = await client.DescribeAddressesAsync(request);
                Validator.ThrowOnBadResult(request, response);

                var existsOnAws = response.Addresses.Any(_ => _.AllocationId == elasticIp.Id);
                return existsOnAws;
            }
        }
    }
}
