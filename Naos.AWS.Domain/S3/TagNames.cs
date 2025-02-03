// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagNames.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Tag name constants.
    /// </summary>
    public static class TagNames
    {
        /// <summary>
        /// The tag name for <see cref="OBeautifulCode.IO.MediaType"/>.
        /// </summary>
        /// <remarks>
        /// Note that this is purposefully not content-type so that it doesn't conflict the content-type
        /// system defined metadata item.  When this tag is used, the S3 Stream will deserialize the MediaType
        /// and pass it along IUploadFiles.
        /// </remarks>
        public const string MediaType = "media-type";
    }
}
