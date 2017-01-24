// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileResult.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    /// <summary>
    /// Result of uploading a file to S3.
    /// </summary>
    public class UploadFileResult
    {
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
        /// Gets the checksum of the uploaded file.
        /// </summary>
        public ComputedChecksum Checksum { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileResult"/> class.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name file was uploaded to.</param>
        /// <param name="keyName">Key name of the file.</param>
        /// <param name="computedChecksum">Checksum of the uploaded file.</param>
        public UploadFileResult(string region, string bucketName, string keyName, ComputedChecksum computedChecksum)
        {
            this.Region = region;
            this.BucketName = bucketName;
            this.KeyName = keyName;
            this.Checksum = computedChecksum;
        }
    }
}
