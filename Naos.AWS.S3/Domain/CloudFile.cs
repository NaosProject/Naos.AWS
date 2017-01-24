// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudFile.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
{
    using System;

    /// <summary>
    /// Representation of a file in S3.
    /// </summary>
    public class CloudFile
    {
        /// <summary>
        /// Gets or sets the region of the bucket.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the bucket the file is in.
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// Gets or sets the the key name of the file.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets the id of the owner of the file.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the owner of the file.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Gets or sets the last modified date of the file.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the size of the file in bytes.
        /// </summary>
        public long Size { get; set; }
    }
}