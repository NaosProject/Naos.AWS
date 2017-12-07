// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageRoute53.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for dealing with Route53's API.
    /// </summary>
    public interface IManageRoute53
    {
        /// <summary>
        /// Creates or updates the DNS entry in the specified zone to the provided IP Addresses.
        /// </summary>
        /// <param name="region">Region the call should be made against.</param>
        /// <param name="domainZoneHostingId">AWS id of the zone.</param>
        /// <param name="type">Type of entry to add.</param>
        /// <param name="domain">Domain to use.</param>
        /// <param name="ipAddresses">IP Addresses to attach to the domain.</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Upsert", Justification = "Spelling/name is correct.")]
        Task UpsertDnsEntryAsync(string region, string domainZoneHostingId, Route53EntryType type, string domain, ICollection<string> ipAddresses);

        /// <summary>
        /// Gets the DNS entries of the specified zone.
        /// </summary>
        /// <param name="region">Region the call should be made against.</param>
        /// <param name="domainZoneHostingId">AWS id of the zone.</param>
        /// <returns>Collection of DNS entries in the zone.</returns>
        /// <returns>Task for async/await</returns>
        Task<ICollection<Route53Entry>> GetDnsEntriesAsync(string region, string domainZoneHostingId);
    }
}
