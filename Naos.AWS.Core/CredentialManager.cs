// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialManager.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    using Amazon;
    using Amazon.Runtime;
    using Amazon.SecurityToken;
    using Amazon.SecurityToken.Model;

    using Naos.AWS.Contract;

    /// <inheritdoc />
    public class CredentialManager : IManageCredentials
    {
        /// <inheritdoc />
        public CredentialContainer GetSessionTokenCredentials(
            string region,
            TimeSpan tokenLifespan,
            string accessKey, 
            string secretKey, 
            string virtualMfaDeviceId, 
            string mfaValue)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var request = new GetSessionTokenRequest()
                              {
                                  DurationSeconds = (int)tokenLifespan.TotalSeconds,
                                  SerialNumber = virtualMfaDeviceId,
                                  TokenCode = mfaValue,
                              };

            using (var client = new AmazonSecurityTokenServiceClient(accessKey, secretKey, regionEndpoint))
            {
                var token = client.GetSessionToken(request);
                return new CredentialContainer()
                           {
                               CredentialType = CredentialType.Token,
                               AccessKeyId = token.Credentials.AccessKeyId,
                               SecretAccessKey = token.Credentials.SecretAccessKey,
                               SessionToken = token.Credentials.SessionToken,
                               Expiration = token.Credentials.Expiration,
                           };
            }
        }

        /// <summary>
        /// Gets or sets the credentials that can be used in lieu or providing to each method.
        /// </summary>
        public static AWSCredentials Cached { get; set; }

        /// <summary>
        /// Get AWSCredentials from the provided credentials and attempt to use cached if null.
        /// </summary>
        /// <param name="credentials">Optional credential container to make credentials from.</param>
        /// <returns>AWSCredentials from provided or cached.</returns>
        public static AWSCredentials GetAwsCredentials(CredentialContainer credentials)
        {
            var ret = credentials == null ? Cached : credentials.ToAwsCredentials();

            if (ret == null)
            {
                throw new ArgumentException("Must provide credentials to method or set credentials in CredentialManager.Cached.");
            }

            return ret;
        }
    }
}
