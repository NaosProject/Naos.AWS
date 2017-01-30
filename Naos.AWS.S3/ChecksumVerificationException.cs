// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChecksumVerificationException.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Exception when checksum verification fails.
    /// </summary>
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