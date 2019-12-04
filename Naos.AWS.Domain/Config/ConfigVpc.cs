// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigVpc.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="Vpc" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
    [XmlRoot("vpc")]
    [XmlType("vpc")]
    [Serializable]
    public class ConfigVpc : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CIDR.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        [XmlAttribute("cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the tenancy.
        /// </summary>
        [XmlAttribute("tenancy")]
        public string Tenancy { get; set; }

        /// <summary>
        /// Gets or sets the internet gateway name.
        /// </summary>
        [XmlAttribute("internetGatewayRef")]
        public string InternetGatewayRef { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        [XmlAttribute("vpcId")]
        public string VpcId { get; set; }

        /// <summary>
        /// Gets or sets the NAT gateways.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("natGateways")]
        [XmlArrayItem("natGateway", typeof(ConfigNatGateway))]
        public ConfigNatGateway[] NatGateways { get; set; }

        /// <summary>
        /// Gets or sets the route tables.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("routeTables")]
        [XmlArrayItem("routeTable", typeof(ConfigRouteTable))]
        public ConfigRouteTable[] RouteTables { get; set; }

        /// <summary>
        /// Gets or sets the subnets.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("subnets")]
        [XmlArrayItem("subnet", typeof(ConfigSubnet))]
        public ConfigSubnet[] Subnets { get; set; }

        /// <summary>
        /// Gets or sets the network ACLs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acls", Justification = "Spelling/name is correct.")]
        [XmlArray("networkAcls")]
        [XmlArrayItem("networkAcl", typeof(ConfigNetworkAcl))]
        public ConfigNetworkAcl[] NetworkAcls { get; set; }

        /// <summary>
        /// Gets or sets the security groups.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("securityGroups")]
        [XmlArrayItem("securityGroup", typeof(ConfigSecurityGroup))]
        public ConfigSecurityGroup[] SecurityGroups { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdatedId(string id)
        {
            this.VpcId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
