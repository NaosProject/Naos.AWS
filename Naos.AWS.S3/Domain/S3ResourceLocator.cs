// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3ResourceLocator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using Naos.Database.Domain;

    /// <summary>
    /// S3 implementation of <see cref="IResourceLocator"/>.
    /// </summary>
    public partial class S3ResourceLocator : ResourceLocatorBase
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        public string BucketName { get; set; }
    }
}
