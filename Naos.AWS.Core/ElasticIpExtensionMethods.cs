// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElasticIpExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using Amazon;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    /// <summary>
    /// Extension methods on the Elastic IP object.
    /// </summary>
    public static class ElasticIpExtensionMethods
    {
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
    }
}
