// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SesTools.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Naos.CodeAnalysis.Recipes;

    /// <summary>
    /// Simple Email Service tools.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ses", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
    public static class SesTools
    {
        /// <summary>
        /// Converts a secret key to an SMTP password.
        /// </summary>
        /// <remarks>
        /// See: <a href="https://docs.aws.amazon.com/ses/latest/dg/smtp-credentials.html" />.
        /// </remarks>
        /// <param name="secretKey">The secret key.</param>
        /// <param name="region">The region (e.g. 'us-east-1').</param>
        /// <returns>
        /// The SMTP password.
        /// </returns>
        public static string ConvertSecretKeyToSmtpPassword(
            string secretKey,
            string region)
        {
            var service = "ses";
            var date = "11111111";
            var terminal = "aws4_request";
            var message = "SendRawEmail";
            byte version = 0x04;

            #pragma warning disable SA1305 // Field names should not use Hungarian notation

            // Step 1: Create a Date Key
            var kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + secretKey), Encoding.UTF8.GetBytes(date));

            // Step 2: Create a Region Key
            var kRegion = HmacSHA256(kDate, Encoding.UTF8.GetBytes(region));

            // Step 3: Create a Service Key
            var kService = HmacSHA256(kRegion, Encoding.UTF8.GetBytes(service));

            // Step 4: Create a Terminal Key
            var kTerminal = HmacSHA256(kService, Encoding.UTF8.GetBytes(terminal));

            // Step 5: Create a Message Key
            var kMessage = HmacSHA256(kTerminal, Encoding.UTF8.GetBytes(message));

            #pragma warning restore SA1305 // Field names should not use Hungarian notation

            // Step 6: Create Signature and Version Key
            var signatureAndVersion = new[] { version }.Concat(kMessage).ToArray();

            var result = Convert.ToBase64String(signatureAndVersion);

            return result;
        }

        private static byte[] HmacSHA256(
            byte[] key,
            byte[] message)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(message);
            }
        }
    }
}
