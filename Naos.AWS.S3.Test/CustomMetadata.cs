// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomMetadata.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    /// <summary>
    /// Custom metadata class.
    /// </summary>
    internal class CustomMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMetadata"/> class.
        /// </summary>
        /// <param name="metadataItem1">First metadata item.</param>
        /// <param name="metadataItem2">Second metadata item.</param>
        /// <param name="thisIsMetadata">is metadata boolean.</param>
        public CustomMetadata(string metadataItem1, int metadataItem2, bool thisIsMetadata)
        {
            this.ThisIsMetadata = thisIsMetadata;
            this.MetadataItem2 = metadataItem2;
            this.MetadataItem1 = metadataItem1;
        }

        /// <summary>
        /// Gets the metadata item 1.
        /// </summary>
        public string MetadataItem1 { get; private set; }

        /// <summary>
        /// Gets the metadata item 2.
        /// </summary>
        public int MetadataItem2 { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it's metadata.
        /// </summary>
        public bool ThisIsMetadata { get; private set; }
    }
}
