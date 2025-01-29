// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadFileDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Details related to a downloaded file.
    /// </summary>
    public class DownloadFileDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileDetails"/> class.
        /// </summary>
        /// <param name="sizeInBytes">The size of the file in bytes.</param>
        /// <param name="lastModifiedUtc">The last modified date of the file in UTC.</param>
        /// <param name="userDefinedMetadata">Metadata stored along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys will not include Amazon specific prefix.</param>
        /// <param name="validatedChecksums">The validated checksums or null if not validated.  Validation compares the checksums that were computed and stored when the file was uploaded against the checksums it computes on the downloaded payload.</param>
        public DownloadFileDetails(
            long sizeInBytes,
            DateTime lastModifiedUtc,
            IReadOnlyDictionary<string, string> userDefinedMetadata,
            IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> validatedChecksums)
        {
            sizeInBytes.MustForArg(nameof(sizeInBytes)).BeGreaterThanOrEqualTo(0L);
            lastModifiedUtc.MustForArg(nameof(lastModifiedUtc)).BeUtcDateTime();
            userDefinedMetadata.MustForArg(nameof(userDefinedMetadata)).NotBeNull();

            this.SizeInBytes = sizeInBytes;
            this.LastModifiedUtc = lastModifiedUtc;
            this.UserDefinedMetadata = userDefinedMetadata;
            this.ValidatedChecksums = validatedChecksums;
        }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public long SizeInBytes { get; private set; }

        /// <summary>
        /// Gets the last modified date of the file in UTC.
        /// </summary>
        public DateTime LastModifiedUtc { get; private set; }

        /// <summary>
        /// Gets metadata stored along with the object.
        /// Note that currently S3 limits the size of keys and data to 2 KB.
        /// Keys will not include Amazon specific prefix.
        /// </summary>
        public IReadOnlyDictionary<string, string> UserDefinedMetadata { get; private set; }

        /// <summary>
        /// Gets the validated checksums or null if not validated.
        /// Validation compares the checksums that were computed and stored when the file was uploaded
        /// against the checksums it computes on the downloaded payload.
        /// </summary>
        public IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> ValidatedChecksums { get; private set; }
    }
}
