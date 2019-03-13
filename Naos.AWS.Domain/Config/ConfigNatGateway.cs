// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigNatGateway.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="NatGateway" />.
    /// </summary>
    [XmlRoot("natGateway")]
    [XmlType("natGateway")]
    [Serializable]
    public class ConfigNatGateway : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent subnet name.
        /// </summary>
        [XmlAttribute("subnetRef")]
        public string SubnetRef { get; set; }

        /// <summary>
        /// Gets or sets the elastic IP name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [XmlAttribute("elasticIpRef")]
        public string ElasticIpRef { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute("natGatewayId")]
        public string NatGatewayId { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.NatGatewayId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}