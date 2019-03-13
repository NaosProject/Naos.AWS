// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChecksumVerificationException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;

    /// <summary>
    /// Exception when checksum verification fails.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Want to control properties through constructor.")]
    [Serializable]
    public class ChecksumVerificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumVerificationException"/> class.
        /// </summary>
        /// <param name="hashAlgorithmName">Name of the algorithm used to perform the checksum.</param>
        /// <param name="expectedChecksum">Expected checksum value.</param>
        /// <param name="computedChecksum">Computed checksum value.</param>
        public ChecksumVerificationException(HashAlgorithmName hashAlgorithmName, string expectedChecksum, string computedChecksum)
            : base(FormattableString.Invariant($"Checksum Verification Failed using algorithm: {hashAlgorithmName}. Expected Checksum: {expectedChecksum}.  Computed Checksum: {computedChecksum}."))
        {
            this.HashAlgorithmName = hashAlgorithmName;
            this.ExpectedChecksum = expectedChecksum;
            this.ComputedChecksum = computedChecksum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChecksumVerificationException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected ChecksumVerificationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.HashAlgorithmName = new HashAlgorithmName(info.GetString(nameof(this.HashAlgorithmName)));
            this.ExpectedChecksum = info.GetString(nameof(this.ExpectedChecksum));
            this.ComputedChecksum = info.GetString(nameof(this.ComputedChecksum));
        }

        /// <inheritdoc cref="Exception" />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.HashAlgorithmName), this.HashAlgorithmName.ToString());
            info.AddValue(nameof(this.ExpectedChecksum), this.ExpectedChecksum);
            info.AddValue(nameof(this.ComputedChecksum), this.ComputedChecksum);
        }

        /// <summary>
        /// Gets the Hash Algorithm Name.
        /// </summary>
        public HashAlgorithmName HashAlgorithmName { get; private set; }

        /// <summary>
        /// Gets the expected checksum.
        /// </summary>
        public string ExpectedChecksum { get; private set; }

        /// <summary>
        /// Gets the expected checksum.
        /// </summary>
        public string ComputedChecksum { get; private set; }
    }
}