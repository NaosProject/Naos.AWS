// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetGatewayExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    using InternetGateway = Naos.AWS.Contract.InternetGateway;

    /// <summary>
    /// Operations to be performed on InternetGateways.
    /// </summary>
    public static class InternetGatewayExtensionMethods
    {
        /// <summary>
        /// Create a new internet gateway.
        /// </summary>
        /// <param name="internetGateway">Internet gateway to create.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Updated copy of the provided object.</returns>
        public static InternetGateway Create(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var localInternetGateway = internetGateway.DeepClone();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localInternetGateway.Region);

            var request = new CreateInternetGatewayRequest();

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.CreateInternetGateway(request);
                Validator.ThrowOnBadResult(request, response);

                localInternetGateway.Id = response.InternetGateway.InternetGatewayId;
            }

            localInternetGateway.TagNameInAws(credentials);

            return localInternetGateway;
        }

        /// <summary>
        /// Deletes an internet gateway.
        /// </summary>
        /// <param name="internetGateway">Internet gateway to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public static void Delete(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(internetGateway.Region);

            var request = new DeleteInternetGatewayRequest() { InternetGatewayId = internetGateway.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = client.DeleteInternetGateway(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
