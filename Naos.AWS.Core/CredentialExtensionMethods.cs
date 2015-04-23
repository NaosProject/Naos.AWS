// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    using Amazon.Runtime;
    using Amazon.SecurityToken.Model;

    using Naos.AWS.Contract;

    /// <summary>
    /// Extension methods to convert internal objects to AWS SDK objects.
    /// </summary>
    public static class CredentialExtensionMethods
    {
        /// <summary>
        /// Convert a CredentialContainer to AWSCredentials.
        /// </summary>
        /// <param name="credentials">CredentialContainer to be converted.</param>
        /// <returns>AWSCredentials using supplied values.</returns>
        public static AWSCredentials ToAwsCredentials(this CredentialContainer credentials)
        {
            AWSCredentials ret = null;
            switch (credentials.CredentialType)
            {
                case Enums.CredentialType.Token:
                    ret = new Credentials(
                        credentials.AccessKeyId,
                        credentials.SecretAccessKey,
                        credentials.SessionToken,
                        credentials.Expiration);
                    break;
                default:
                    throw new ArgumentException("Unsupported credential type: " + credentials.CredentialType);
            }

            return ret;
        }
    }
}
