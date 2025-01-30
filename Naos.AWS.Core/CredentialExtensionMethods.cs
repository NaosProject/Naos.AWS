// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Amazon.Runtime;
    using Amazon.SecurityToken.Model;

    using Naos.AWS.Domain;

    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Extension methods to convert internal objects to AWS SDK objects.
    /// </summary>
    public static class CredentialExtensionMethods
    {
        #pragma warning disable CS3002 // Return type is not CLS-compliant

        /// <summary>
        /// Convert a CredentialContainer to AWSCredentials.
        /// </summary>
        /// <param name="credentials">CredentialContainer to be converted.</param>
        /// <returns>AWSCredentials using supplied values.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static AWSCredentials ToAwsCredentials(this CredentialContainer credentials)
        {
            new { credentials }.AsArg().Must().NotBeNull();

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

        #pragma warning restore CS3002 // Return type is not CLS-compliant
    }
}
