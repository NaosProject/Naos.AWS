// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudStorageWriterSettings.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using OBeautifulCode.Type;

    /// <summary>
    /// Contains settings for writing to cloud storage.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class CloudStorageWriterSettings : CloudStorageSettingsBase, IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageWriterSettings"/> class.
        /// </summary>
        /// <param name="accessKey">Access key.</param>
        /// <param name="secretKey">Secret key.</param>
        /// <param name="region">Region code.</param>
        /// <param name="bucketName">Bucket name.</param>
        public CloudStorageWriterSettings(
            string accessKey,
            string secretKey,
            string region,
            string bucketName)
            : base(accessKey, secretKey, region, bucketName)
        {
        }
    }
}
