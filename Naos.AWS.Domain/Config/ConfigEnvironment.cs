// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigEnvironment.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a region (top level environment configuration).
    /// </summary>
    [XmlRoot("region")]
    [XmlType("region")]
    [Serializable]
    public class ConfigEnvironment
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the region name.
        /// </summary>
        [XmlAttribute("regionName")]
        public string RegionName { get; set; }

        /// <summary>
        /// Gets or sets the elastic IPs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ips", Justification = "Spelling/name is correct.")]
        [XmlArray("elasticIps")]
        public ConfigElasticIp[] ElasticIps { get; set; }

        /// <summary>
        /// Gets or sets the internet gateways.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("internetGateways")]
        public ConfigInternetGateway[] InternetGateways { get; set; }

        /// <summary>
        /// Gets or sets the VPCs.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpcs", Justification = "Spelling/name is correct.")]
        [XmlArray("vpcs")]
        [XmlArrayItem("vpc", typeof(ConfigVpc))]
        public ConfigVpc[] Vpcs { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Throws an exception if it is invalidly structured.
        /// </summary>
        /// <param name="checkForIds">Value indicating whether to force missing IDs.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Like this for readability.")]
        public void ThrowIfInvalid(bool checkForIds)
        {
            var ids =
                new string[0].Concat(this.ElasticIps.Select(_ => _.AllocationId))
                        .Concat(this.InternetGateways.Select(_ => _.InternetGatewayId))
                        .Concat(
                        this.Vpcs.SelectMany(
                            v => new[] { v.VpcId }
                                .Concat(v.NatGateways.Select(g => g.NatGatewayId))
                                .Concat(v.NetworkAcls.Select(n => n.NetworkAclId))
                                .Concat(v.RouteTables.Select(r => r.RouteTableId))
                                .Concat(v.SecurityGroups.Select(p => p.SecurityGroupId))
                                .Concat(v.Subnets.Select(s => s.SubnetId)))).ToList();

            if (checkForIds && ids.Any(_ => !string.IsNullOrWhiteSpace(_)))
            {
                throw new ArgumentException("Cannot have any IDs present to create an enviroment; there are non-null/empty ids in: " + string.Join(",", ids));
            }

            var names =
                new string[0].Concat(this.ElasticIps.Select(_ => _.Name))
                        .Concat(this.InternetGateways.Select(_ => _.Name))
                        .Concat(
                        this.Vpcs.SelectMany(
                            v => new[] { v.Name }
                                .Concat(v.NatGateways.Select(g => g.Name))
                                .Concat(v.NetworkAcls.Select(n => n.Name))
                                .Concat(v.RouteTables.Select(r => r.Name))
                                .Concat(v.SecurityGroups.Select(p => p.Name))
                                .Concat(v.Subnets.Select(s => s.Name)))).ToList();

            var distinctNames = names.Distinct().ToList();
            if (names.Count != distinctNames.Count)
            {
                throw new ArgumentException("All names must be distinct; there are duplicates in: " + string.Join(",", names));
            }
        }
    }
}