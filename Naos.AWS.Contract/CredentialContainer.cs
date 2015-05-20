// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialContainer.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System;

    /// <summary>
    /// Wrapper object to hold the AWSCredentials and allow abstraction from changes in it.
    /// </summary>
    public class CredentialContainer
    {
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
