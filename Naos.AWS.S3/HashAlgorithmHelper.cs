// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmHelper.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using Spritely.Recipes;

    /// <summary>
    /// Helper class for computing hash values.
    /// </summary>
    public static class HashAlgorithmHelper
    {
        private static readonly IDictionary<HashAlgorithmName, Func<HashAlgorithm>> HashAlgorithmFactory =
            new Dictionary<HashAlgorithmName, Func<HashAlgorithm>>
            {
                { HashAlgorithmName.MD5, MD5.Create },
                { HashAlgorithmName.SHA1, SHA1.Create },
                { HashAlgorithmName.SHA256, SHA256.Create },
                { HashAlgorithmName.SHA384, SHA384.Create },
                { HashAlgorithmName.SHA512, SHA512.Create }
            };

        /// <summary>
        /// Convert a hexadecimal string representing a hash value to a base64 encoded string.
        /// <see href="https://aws.amazon.com/premiumsupport/knowledge-center/data-integrity-s3/"/>
        /// </summary>
        /// <param name="hexString">Hexadecimal string to convert</param>
        /// <returns>The base64-encoded hash value.</returns>
        public static string ConvertHexStringToBase64(string hexString)
        {
            var buffer = new byte[hexString.Length / 2];
            for (var i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(Convert.ToInt32(hexString.Substring(i, 2), 16));
            }

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        ///  Compute a hash value and returns a hexadecimal-formatted string.
        /// </summary>
        /// <param name="hashAlgorithmName">Name of the hash algorithm to use.</param>
        /// <param name="sourceStream">Source stream to use as the input.</param>
        /// <returns>Hexadecimal-formatted string of the hash value.</returns>
        public static string ComputeHash(HashAlgorithmName hashAlgorithmName, Stream sourceStream)
        {
            sourceStream.Named(nameof(sourceStream)).Must().NotBeNull().OrThrow();

            using (var hashAlgorithm = GetHashAlgorithm(hashAlgorithmName))
            {
                if (sourceStream.CanSeek)
                {
                    sourceStream.Position = 0;
                }

                return CreateHashString(hashAlgorithm.ComputeHash(sourceStream));
            }
        }

        /// <summary>
        ///  Compute a hash value and returns a hexadecimal-formatted string.
        /// </summary>
        /// <param name="hashAlgorithmName">Name of the hash algorithm to use.</param>
        /// <param name="sourceFilePath">Source file path to use as the input.</param>
        /// <returns>Hexadecimal-formatted string of the hash value.</returns>
        public static string ComputeHash(HashAlgorithmName hashAlgorithmName, string sourceFilePath)
        {
            sourceFilePath.Named(nameof(sourceFilePath)).Must().NotBeEmptyString().OrThrow();

            using (var hashAlgorithm = GetHashAlgorithm(hashAlgorithmName))
            {
                using (var file = File.OpenRead(sourceFilePath))
                {
                    return CreateHashString(hashAlgorithm.ComputeHash(file));
                }
            }
        }

        /// <summary>
        /// Gets a HashAlgorithm implementation based on the hash algorithm name.
        /// </summary>
        /// <param name="hashAlgorithmName">Name of the hash algorithm.</param>
        /// <returns>Hash algorithm to use in computing a hash value.</returns>
        private static HashAlgorithm GetHashAlgorithm(HashAlgorithmName hashAlgorithmName)
        {
            if (!HashAlgorithmFactory.ContainsKey(hashAlgorithmName))
            {
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), hashAlgorithmName, "Unsupported Hash Algorithm Name.");
            }

            return HashAlgorithmFactory[hashAlgorithmName]();
        }

        private static string CreateHashString(IEnumerable<byte> bytes)
        {
            var hashValue = new StringBuilder();

            foreach (var b in bytes)
            {
                hashValue.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return hashValue.ToString();
        }
    }
}
