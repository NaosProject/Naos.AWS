// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3Credentials.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Type;

    /// <summary>
    /// Contains credentials for authenticating against S3.
    /// </summary>
    public partial class S3Credentials : IModelViaCodeGen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="S3Credentials"/> class.
        /// </summary>
        /// <param name="accessKeyId">Access key.</param>
        /// <param name="secretAccessKey">Secret key.</param>
        public S3Credentials(
            string accessKeyId,
            string secretAccessKey)
        {
            new { accessKeyId }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { secretAccessKey }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.AccessKeyId = accessKeyId;
            this.SecretAccessKey = secretAccessKey;
        }

        /// <summary>
        /// Gets the access key ID.
        /// </summary>
        public string AccessKeyId { get; private set; }

        /// <summary>
        /// Gets the secret access key.
        /// </summary>
        public string SecretAccessKey { get; private set; }
    }
}
