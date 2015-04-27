// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageCredentials.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System;

    /// <summary>
    /// Calls responsible for creating a session to make calls to other services.
    /// </summary>
    public interface IManageCredentials
    {
        /// <summary>
        /// Gets a new session token for provided login credentials (for multi-factor authentication accounts).
        /// </summary>
        /// <param name="region">Region to request token against.</param>
        /// <param name="tokenLifespan">Lifespan of token to create.</param>
        /// <param name="accessKey">Access key of IAM account.</param>
        /// <param name="secretKey">Secret key of IAM account.</param>
        /// <param name="virtualMfaDeviceId">Id/Resource path for the Virtual MFA device (found in console where MFA is configured).</param>
        /// <param name="mfaValue">Token/rotating key from the MFA device.</param>
        /// <returns>Credential object to be used when performing operations.</returns>
        CredentialContainer GetSessionTokenCredentials(
            string region,
            TimeSpan tokenLifespan,
            string accessKey,
            string secretKey,
            string virtualMfaDeviceId,
            string mfaValue);
    }
}
