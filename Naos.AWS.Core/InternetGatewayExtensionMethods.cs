// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetGatewayExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using Amazon;
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
        /// <param name="credentials">Credentials to use.</param>
        public static void Create(this InternetGateway internetGateway, CredentialContainer credentials)
        {
            var awsCredentials = credentials.ToAwsCredentials();
            var regionEndpoint = RegionEndpoint.GetBySystemName(internetGateway.Region);

            var request = new CreateInternetGatewayRequest();

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var ret = client.CreateInternetGateway(request);
                internetGateway.Id = ret.InternetGateway.InternetGatewayId;
            }

            Tagger.TagName(credentials, internetGateway);
        }
    }
}
