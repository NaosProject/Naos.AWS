// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElasticIpExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Linq;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    /// <summary>
    /// Extension methods on the Elastic IP object.
    /// </summary>
    public static class ElasticIpExtensionMethods
    {
        /// <summary>
        /// Allocates an elastic IP.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to allocate.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Newly allocated elastic IP.</returns>
        public static ElasticIp Allocate(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new AllocateAddressRequest { Domain = DomainType.Standard };

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.AllocateAddress(request);
                Validator.ThrowOnBadResult(request, response);

                var ret = elasticIp.DeepClone();
                ret.Id = response.AllocationId;
                ret.PublicIpAddress = response.PublicIp;
                return ret;
            }
        }

        /// <summary>
        /// Releases an elastic IP.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to release.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Release(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new ReleaseAddressRequest
                              {
                                  AllocationId = elasticIp.Id,
                              };

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.ReleaseAddress(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Attaches the elastic IP to a specified instance.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to connect to the provided instance.</param>
        /// <param name="instanceId">Instance to attach elastic IP to.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void AssociateToInstance(this ElasticIp elasticIp, string instanceId, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new AssociateAddressRequest(instanceId, elasticIp.PublicIpAddress);

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.AssociateAddress(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes the elastic IP from the specified instance.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to remove from the provided instance.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void DisassociateFromInstance(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            var request = new DisassociateAddressRequest(elasticIp.PublicIpAddress);

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.DisassociateAddress(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="elasticIp">Elastic IP to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        public static bool ExistsOnAws(this ElasticIp elasticIp, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(elasticIp.Region);

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new DescribeAddressesRequest() { AllocationIds = new[] { elasticIp.Id }.ToList() };

                var response = client.DescribeAddresses(request);
                Validator.ThrowOnBadResult(request, response);

                var existsOnAws = response.Addresses.Any(_ => _.AllocationId == elasticIp.Id);
                return existsOnAws;
            }
        }
    }
}
