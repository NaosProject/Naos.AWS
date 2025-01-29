// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using OBeautifulCode.Assertion.Recipes;

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
        /// <param name="wasWritten">A value indicating whether the file was written.  This is only ever false when the file already exists AND the <see cref="ExistingFileWriteAction.DoNotOverwriteFile"/> action is used when uploading.</param>
        public UploadFileResult(
            string region,
            string bucketName,
            string keyName,
            IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> computedChecksums,
            bool wasWritten)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();
            computedChecksums.AsArg(nameof(computedChecksums)).Must().NotBeNull();

            this.Region = region;
            this.BucketName = bucketName;
            this.KeyName = keyName;
            this.Checksums = computedChecksums;
            this.WasWritten = wasWritten;
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

        /// <summary>
        /// Gets a value indicating whether the file was written.
        /// This is only ever false when the file already exists AND the <see cref="ExistingFileWriteAction.DoNotOverwriteFile"/> action is used when uploading.
        /// </summary>
        public bool WasWritten { get; private set; }
    }
}
