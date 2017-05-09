// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncodingHelper.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;

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
        public static string ConvertHexStringToBase64(string hexString)
        {
            var buffer = new byte[hexString.Length / 2];
            for (var i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(Convert.ToInt32(hexString.Substring(i, 2), 16));
            }

            return Convert.ToBase64String(buffer);
        }
    }
}
