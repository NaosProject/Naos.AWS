// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudStorageSettingsBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Base class settings specific for accessing cloud storage.
    /// </summary>
    public abstract partial class CloudStorageSettingsBase : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageSettingsBase"/> class.
        /// </summary>
        /// <param name="accessKey">Access key.</param>
        /// <param name="secretKey">Secret key.</param>
        /// <param name="region">Region code.</param>
        /// <param name="bucketName">Bucket name.</param>
        protected CloudStorageSettingsBase(
            string accessKey,
            string secretKey,
            string region,
            string bucketName)
        {
            new { accessKey }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { secretKey }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { region }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { bucketName }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.AccessKey = accessKey;
            this.SecretKey = secretKey;
            this.Region = region;
            this.BucketName = bucketName;
        }

        /// <summary>
        /// Gets the storage account Access Key.
        /// </summary>
        public string AccessKey { get; private set; }

        /// <summary>
        /// Gets the storage account Secret Key.
        /// </summary>
        public string SecretKey { get; private set; }

        /// <summary>
        /// Gets the Region.
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Gets the Bucket name.
        /// </summary>
        public string BucketName { get; private set; }
    }
}
