// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileResult.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.Security.Cryptography;

    using Spritely.Recipes;

    /// <summary>
    /// Result of uploading a file to S3.
    /// </summary>
    public class UploadFileResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileResult"/> class.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name file was uploaded to.</param>
        /// <param name="keyName">Key name of the file.</param>
        /// <param name="computedChecksums">Computed checksums of the uploaded file.</param>
        public UploadFileResult(string region, string bucketName, string keyName, IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> computedChecksums)
        {
            region.Named(nameof(region)).Must().NotBeWhiteSpace().OrThrow();
            bucketName.Named(nameof(bucketName)).Must().NotBeWhiteSpace().OrThrow();
            keyName.Named(nameof(keyName)).Must().NotBeWhiteSpace().OrThrow();
            computedChecksums.Named(nameof(computedChecksums)).Must().NotBeNull().OrThrow();

            this.Region = region;
            this.BucketName = bucketName;
            this.KeyName = keyName;
            this.Checksums = computedChecksums;
        }

        /// <summary>
        /// Gets the region the file was uploaded to.
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Gets the bucket name the file was uploaded to.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the key name of the file.
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// Gets the checksums for the uploaded file.
        /// </summary>
        public IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> Checksums { get; private set; }
    }
}
