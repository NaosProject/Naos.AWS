// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsConfiguration.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    /// <summary>
    /// AWS Configuration used for Integration Tests.
    /// Note: Do not commit this configuration information to the repository as it will reveal Amazon credentials.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Sometimes used for testing.")]
    internal class AwsConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AwsConfiguration"/> class.
        /// </summary>
        /// <param name="accessKey">Access key.</param>
        /// <param name="secretKey">Secret key</param>
        /// <param name="region">Region code.</param>
        /// <param name="bucketName">Bucket name.</param>
        public AwsConfiguration(string accessKey, string secretKey, string region, string bucketName)
        {
            this.AccessKey = accessKey;
            this.SecretKey = secretKey;
            this.Region = region;
            this.BucketName = bucketName;
        }

        /// <summary>
        /// Gets the Access Key for AWS.  To run integration tests enter S3 access key.
        /// </summary>
        public string AccessKey { get; private set; }

        /// <summary>
        /// Gets the Secret Key for AWS.  To run integration tests enter S3 secret key.
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
