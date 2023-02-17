// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialContainer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Wrapper object to hold the AWSCredentials and allow abstraction from changes in it.
    /// </summary>
    public partial class CredentialContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialContainer"/> class.
        /// </summary>
        public CredentialContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialContainer"/> class.
        /// </summary>
        /// <param name="accessKey">Access key.</param>
        /// <param name="secretKey">Secret key.</param>
        public CredentialContainer(
            string accessKey,
            string secretKey)
        {
            new { accessKey }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { secretKey }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.CredentialType = CredentialType.Keys;
            this.AccessKeyId = accessKey;
            this.SecretAccessKey = secretKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialContainer"/> class.
        /// </summary>
        /// <param name="sessionToken">Session token.</param>
        /// <param name="expiration">Token expiration.</param>
        public CredentialContainer(string sessionToken, DateTime expiration)
        {
            new { sessionToken }.AsArg().Must().NotBeNullNorWhiteSpace();

            this.CredentialType = CredentialType.Token;
            this.Expiration = expiration;
        }

        /// <summary>
        /// Gets or sets the type of AWS credential.
        /// </summary>
        public CredentialType CredentialType { get; set; }

        /// <summary>
        /// Gets or sets the access key ID.
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// Gets or sets the secret access key.
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>
        /// Gets or sets the session token.
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
