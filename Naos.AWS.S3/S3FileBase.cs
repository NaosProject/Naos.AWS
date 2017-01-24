// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3FileBase.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using Spritely.Recipes;

    /// <summary>
    /// Base class for Amazon S3 operations.
    /// </summary>
    public abstract class S3FileBase
    {
        // Actual Checksum metadata key should be 'x-amz-meta{HashAlgorithmName}-checksum'.
        // If not present Amazon will prepend 'x-amz-meta' to the metadata key.
                 
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
        /// Initializes a new instance of the <see cref="S3FileBase"/> class.
        /// </summary>
        /// <param name="accessKey">Access key with rights to download files in specified buckets.</param>
        /// <param name="secretKey">Secret key with rights to download files in specified buckets.</param>
        protected S3FileBase(string accessKey, string secretKey)
        {
            accessKey.Named(nameof(accessKey)).Must().NotBeEmptyString().OrThrow();
            secretKey.Named(nameof(secretKey)).Must().NotBeEmptyString().OrThrow();

            this.AccessKey = accessKey;
            this.SecretKey = secretKey;
        }
    }
}
