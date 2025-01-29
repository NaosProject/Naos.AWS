// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadFileResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// The result of downloading a file.
    /// </summary>
    public class DownloadFileResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileResult"/> class.
        /// </summary>
        /// <param name="region">The region that the bucket is in.</param>
        /// <param name="bucketName">The name of the bucket that the file was downloaded from.</param>
        /// <param name="keyName">The name of the key that was downloaded.</param>
        /// <param name="details">Details about the downloaded file if the key exists.  Null if the key does not exist.</param>
        public DownloadFileResult(
            string region,
            string bucketName,
            string keyName,
            DownloadFileDetails details)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();

            this.Region = region;
            this.BucketName = bucketName;
            this.KeyName = keyName;
            this.Details = details;
        }

        /// <summary>
        /// Gets the region that the bucket is in.
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Gets the name of the bucket that the file was downloaded from.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the name of the key that was downloaded.
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// Gets details about the downloaded file if the key exists.  Null if the key does not exist.
        /// </summary>
        public DownloadFileDetails Details { get; private set; }
    }
}
