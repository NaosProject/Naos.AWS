// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetGatewayExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Threading.Tasks;

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
        public static async Task<InternetGateway> CreateAsync(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var localInternetGateway = internetGateway.DeepClone();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(localInternetGateway.Region);

            var request = new CreateInternetGatewayRequest();

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);

                localInternetGateway.Id = response.InternetGateway.InternetGatewayId;
            }

            await localInternetGateway.TagNameInAwsAsync(credentials);

            return localInternetGateway;
        }

        /// <summary>
        /// Deletes an internet gateway.
        /// </summary>
        /// <param name="internetGateway">Internet gateway to delete.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        public static async Task DeleteAsync(this InternetGateway internetGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(internetGateway.Region);

            var request = new DeleteInternetGatewayRequest() { InternetGatewayId = internetGateway.Id };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}
