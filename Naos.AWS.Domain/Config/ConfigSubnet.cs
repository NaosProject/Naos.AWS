// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigSubnet.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="Subnet" />.
    /// </summary>
    [XmlRoot("subnet")]
    [XmlType("subnet")]
    [Serializable]
    public class ConfigSubnet : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attached route table name.
        /// </summary>
        [XmlAttribute("routeTableRef")]
        public string RouteTableRef { get; set; }

        /// <summary>
        /// Gets or sets the CIDR.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        [XmlAttribute("cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the availability zone.
        /// </summary>
        [XmlAttribute("az")]
        public string AvailabilityZone { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute("subnetId")]
        public string SubnetId { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.SubnetId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
