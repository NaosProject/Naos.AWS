// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route53Entry.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Model object of a DNS record entry in Route53.
    /// </summary>
    public class Route53Entry
    {
        /// <summary>
        /// Gets or sets domain.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the IP Addresses.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public IReadOnlyCollection<string> IpAddresses { get; set; }

        /// <summary>
        /// Gets or sets the type of entry.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Spelling/name is correct.")]
        public Route53EntryType Type { get; set; }
    }
}