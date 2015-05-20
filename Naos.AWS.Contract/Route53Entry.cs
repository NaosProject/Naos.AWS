// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route53Entry.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
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
        public ICollection<string> IpAddresses { get; set; }

        /// <summary>
        /// Gets or sets the type of entry.
        /// </summary>
        public Route53EntryType Type { get; set; }
    }
}