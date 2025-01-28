// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadFileResult.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// The result of downloading a file.
    /// </summary>
    public class DownloadFileResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileResult"/> class.
        /// </summary>
        /// <param name="keyExists">A value indicating whether the key exists.</param>
        /// <param name="sizeInBytes">The size of the file in bytes or null if <paramref name="keyExists"/> is false.</param>
        /// <param name="lastModifiedUtc">The last modified date of the file in UTC or null if <paramref name="keyExists"/> is false.</param>
        /// <param name="userDefinedMetadata">Metadata stored along with the object or null if <paramref name="keyExists"/> is false.  Note that currently S3 limits the size of keys and data to 2 KB. Keys will not include Amazon specific prefix.</param>
        public DownloadFileResult(
            bool keyExists,
            long? sizeInBytes,
            DateTime? lastModifiedUtc,
            IReadOnlyDictionary<string, string> userDefinedMetadata)
        {
            sizeInBytes.MustForArg(nameof(sizeInBytes)).BeGreaterThanOrEqualToWhenNotNull((long?)0);
            lastModifiedUtc.MustForArg(nameof(lastModifiedUtc)).BeUtcDateTimeWhenNotNull();

            this.KeyExists = keyExists;
            this.SizeInBytes = sizeInBytes;
            this.LastModifiedUtc = lastModifiedUtc;
            this.UserDefinedMetadata = userDefinedMetadata;
        }

        /// <summary>
        /// Gets a value indicating whether the key exists.
        /// </summary>
        public bool KeyExists { get; private set; }

        /// <summary>
        /// Gets the size of the file in bytes or null if <see cref="KeyExists"/> is false.
        /// </summary>
        public long? SizeInBytes { get; private set; }

        /// <summary>
        /// Gets the last modified date of the file in UTC or null if <see cref="KeyExists"/> is false.
        /// </summary>
        public DateTime? LastModifiedUtc { get; private set; }

        /// <summary>
        /// Gets metadata stored along with the object or null if <see cref="KeyExists"/> is false
        /// Note that currently S3 limits the size of keys and data to 2 KB.
        /// Keys will not include Amazon specific prefix.
        /// </summary>
        public IReadOnlyDictionary<string, string> UserDefinedMetadata { get; private set; }
    }
}
