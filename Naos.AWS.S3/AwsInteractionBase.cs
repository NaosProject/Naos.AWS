// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsInteractionBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;

    using Amazon.Runtime;
    using Amazon.SecurityToken.Model;

    using Naos.AWS.Domain;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Base class for Amazon S3 operations.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    public abstract class AwsInteractionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AwsInteractionBase"/> class.
        /// </summary>
        /// <param name="credentials">Credentials with rights to read and write  files in specified buckets.</param>
        protected AwsInteractionBase(CredentialContainer credentials)
        {
            new { credentials }.Must().NotBeNull();

            this.Credentials = credentials;
        }

        /// <summary>
        /// Metadata key suffix for the checksum entry.
        /// </summary>
        protected const string MetadataKeyChecksumSuffix = "-checksum";

        /// <summary>
        /// Gets Amazon S3 Access Key.
        /// </summary>
        protected CredentialContainer Credentials { get; private set; }

        /// <summary>
        /// User defined metadata key prefix that AWS adds to user defined metadata.
        /// </summary>
        private const string AwsUserMetadataKeyPrefix = "x-amz-meta-";

        /// <summary>
        /// Remove the value that AWS prefixes the metadata key with.
        /// </summary>
        /// <param name="awsMetadataKey">The metadata key received from AWS.</param>
        /// <returns>The metadata key with the prefix removed.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        protected static string SanitizeUserDefinedMetadataKey(string awsMetadataKey)
        {
            if (string.IsNullOrWhiteSpace(awsMetadataKey))
            {
                return awsMetadataKey;
            }

            return awsMetadataKey.StartsWith(AwsUserMetadataKeyPrefix, StringComparison.OrdinalIgnoreCase) ? awsMetadataKey.Substring(AwsUserMetadataKeyPrefix.Length) : awsMetadataKey;
        }
    }

    /// <summary>
    /// Extension methods to convert internal objects to AWS SDK objects. (Duplicate from Naos.AWS.Core!!!).
    /// </summary>
    internal static class CredentialExtensionMethods
    {
        /// <summary>
        /// Convert a CredentialContainer to AWSCredentials.
        /// </summary>
        /// <param name="credentials">CredentialContainer to be converted.</param>
        /// <returns>AWSCredentials using supplied values.</returns>
        public static AWSCredentials ToAwsCredentials(this CredentialContainer credentials)
        {
            new { credentials }.Must().NotBeNull();

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
