// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialContainer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using OBeautifulCode.Type;

    /// <summary>
    /// Wrapper object to hold the AWSCredentials and allow abstraction from changes in it.
    /// </summary>
    public partial class CredentialContainer : IModelViaCodeGen
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
