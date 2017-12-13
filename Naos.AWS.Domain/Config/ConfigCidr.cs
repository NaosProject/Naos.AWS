// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigCidr.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Configuration for CIDRs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
    public static class ConfigCidr
    {
        /// <summary>
        /// Magic string to use all CIDR (0.0.0.0/0).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public const string AllTrafficCidrName = "AllTrafficCidr";
    }
}