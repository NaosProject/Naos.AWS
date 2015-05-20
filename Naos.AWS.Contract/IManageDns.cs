// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageDns.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for interacting with Route 53 DNS management.
    /// </summary>
    public interface IManageDns
    {
        /// <summary>
        /// Creates or updates the specified DNS entry to the provide IP Addresses.
        /// </summary>
        /// <param name="region">Region to perform calls against.</param>
        /// <param name="domainZoneHostingId">Hosting ID of the zone that the domain does/should live in.</param>
        /// <param name="domain">Domain to operate on.</param>
        /// <param name="ipAddresses">IP Addresses to bind to the DNS entry specified.</param>
        void UpsertDnsEntry(string region, string domainZoneHostingId, string domain, ICollection<string> ipAddresses);
    }
}
