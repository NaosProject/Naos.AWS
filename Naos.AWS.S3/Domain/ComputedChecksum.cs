// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComputedChecksum.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Security.Cryptography;

    /// <summary>
    /// Contains a hash algorithm name and checksum value.
    /// </summary>
    public class ComputedChecksum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComputedChecksum"/> class.
        /// </summary>
        /// <param name="hashAlgorithmName">The name of the hash algorithm name to used to compute the checksum.</param>
        /// <param name="hashValue">The hash value.</param>
        public ComputedChecksum(HashAlgorithmName hashAlgorithmName, string hashValue)
        {
            this.HashAlgorithmName = hashAlgorithmName;
            this.Value = hashValue;
        }

        /// <summary>
        /// Gets the name of the hash algorithm used to compute the checksum.
        /// </summary>
        public HashAlgorithmName HashAlgorithmName { get; private set; }

        /// <summary>
        /// Gets the value of the computed checksum.
        /// </summary>
        public string Value { get; private set; }
    }
}
