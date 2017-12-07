// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncodingHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;

    using Spritely.Recipes;

    /// <summary>
    /// Helper class for computing hash values.
    /// </summary>
    public static class EncodingHelper
    {
        /// <summary>
        /// Convert a hexadecimal string representing a hash value to a base64 encoded string.
        /// <see href="https://aws.amazon.com/premiumsupport/knowledge-center/data-integrity-s3/"/>
        /// </summary>
        /// <param name="hexString">Hexadecimal string to convert</param>
        /// <returns>The base64-encoded hash value.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Spelling/name is correct.")]
        public static string ConvertHexStringToBase64(string hexString)
        {
            new { hexString }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var buffer = new byte[hexString.Length / 2];
            for (var i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(Convert.ToInt32(hexString.Substring(i, 2), 16));
            }

            return Convert.ToBase64String(buffer);
        }
    }
}
