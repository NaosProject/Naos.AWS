// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    using Amazon.Runtime;
    using Amazon.SecurityToken.Model;

    using Naos.AWS.Domain;

    using Spritely.Recipes;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static AWSCredentials ToAwsCredentials(this CredentialContainer credentials)
        {
            new { credentials }.Must().NotBeNull().OrThrow();

            AWSCredentials ret = null;
            switch (credentials.CredentialType)
            {
                case CredentialType.Token:
                    ret = new Credentials(
                        credentials.AccessKeyId,
                        credentials.SecretAccessKey,
                        credentials.SessionToken,
                        credentials.Expiration);
                    break;
                case CredentialType.Keys:
                    ret = new BasicAWSCredentials(credentials.AccessKeyId, credentials.SecretAccessKey);
                    break;
                default:
                    throw new ArgumentException("Unsupported credential type: " + credentials.CredentialType);
            }

            return ret;
        }
    }
}
