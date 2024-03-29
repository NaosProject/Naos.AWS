﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialContainer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Globalization;
    using OBeautifulCode.Type;

    /// <summary>
    /// Wrapper object to hold the AWSCredentials and allow abstraction from changes in it.
    /// </summary>
    public partial class CredentialContainer : IModelViaCodeGen, IDeclareToStringMethod
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

        /// <inheritdoc cref="IDeclareToStringMethod" />
        public override string ToString()
        {
            var obfuscationToken = "***";
            var secretKeyPieceOfToString = this.SecretAccessKey != null ? obfuscationToken : "<null>";
            var sessionTokenPieceOfToString = this.SessionToken != null ? obfuscationToken : "<null>";
            var result = FormattableString.Invariant($"Naos.AWS.Domain.CredentialContainer: CredentialType = {this.CredentialType.ToString() ?? "<null>"}, AccessKeyId = {this.AccessKeyId?.ToString(CultureInfo.InvariantCulture) ?? "<null>"}, SecretAccessKey = {secretKeyPieceOfToString}, SessionToken = {sessionTokenPieceOfToString}, Expiration = {this.Expiration.ToString(CultureInfo.InvariantCulture) ?? "<null>"}.");

            return result;
        }
    }
}
