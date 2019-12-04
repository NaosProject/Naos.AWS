// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudFile.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Representation of a file in S3.
    /// </summary>
    public class CloudFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudFile"/> class.
        /// </summary>
        /// <param name="region">Region of the bucket.</param>
        /// <param name="bucketName">Bucket the file is in.</param>
        /// <param name="keyName">Key name of the file.</param>
        /// <param name="ownerId">ID of the owner of the file.</param>
        /// <param name="ownerName">Name of the owner of the file.</param>
        /// <param name="lastModified">Last modified date of the file.</param>
        /// <param name="size">Size of the file.</param>
        public CloudFile(string region, string bucketName, string keyName, string ownerId, string ownerName, DateTime lastModified, long size)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();
            ownerId.AsArg(nameof(ownerId)).Must().NotBeNullNorWhiteSpace();
            size.AsArg(nameof(size)).Must().BeGreaterThanOrEqualTo(0L);

            this.Region = region;
            this.BucketName = bucketName;
            this.KeyName = keyName;
            this.OwnerId = ownerId;
            this.OwnerName = ownerName;
            this.LastModified = lastModified;
            this.Size = size;
        }

        /// <summary>
        /// Gets the region of the bucket.
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Gets the bucket the file is in.
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// Gets the key name of the file.
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// Gets the id of the owner of the file.
        /// </summary>
        public string OwnerId { get; private set; }

        /// <summary>
        /// Gets the display name of the owner of the file.
        /// </summary>
        public string OwnerName { get; private set; }

        /// <summary>
        /// Gets the last modified date of the file.
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public long Size { get; private set; }
    }
}
