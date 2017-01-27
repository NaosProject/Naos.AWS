// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsInteractionBase.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using Spritely.Recipes;

    /// <summary>
    /// Base class for Amazon S3 operations.
    /// </summary>
    public abstract class AwsInteractionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AwsInteractionBase"/> class.
        /// </summary>
        /// <param name="accessKey">Access key with rights to read and write  files in specified buckets.</param>
        /// <param name="secretKey">Secret key with rights to read and write  files in specified buckets.</param>
        protected AwsInteractionBase(string accessKey, string secretKey)
        {
            accessKey.Named(nameof(accessKey)).Must().NotBeWhiteSpace().OrThrow();
            secretKey.Named(nameof(secretKey)).Must().NotBeWhiteSpace().OrThrow();

            this.AccessKey = accessKey;
            this.SecretKey = secretKey;
        }

        /// <summary>
        /// Metadata key suffix for the checksum entry. 
        /// </summary>
        protected const string MetadataKeyChecksumSuffix = "-checksum";

        /// <summary>
        /// Gets Amazon S3 Access Key.
        /// </summary>
        protected string AccessKey { get; private set; }

        /// <summary>
        /// Gets Amazon S3 Secret Key.
        /// </summary>
        protected string SecretKey { get; private set; }

        /// <summary>
        /// User defined metadata key prefix that AWS adds to user defined metadata. 
        /// </summary>
        private const string AwsUserMetadataKeyPrefix = "x-amz-meta-";

        /// <summary>
        /// Remove the value that AWS prefixes the metadata key with.
        /// </summary>
        /// <param name="awsMetadataKey">The metadata key received from AWS.</param>
        /// <returns>The metadata key with the prefix removed.</returns>
        protected static string SanitizeUserDefinedMetadataKey(string awsMetadataKey)
        {
            if (string.IsNullOrWhiteSpace(awsMetadataKey))
            {
                return awsMetadataKey;
            }

            return awsMetadataKey.StartsWith(AwsUserMetadataKeyPrefix) ? awsMetadataKey.Substring(AwsUserMetadataKeyPrefix.Length) : awsMetadataKey;
        }
    }
}
