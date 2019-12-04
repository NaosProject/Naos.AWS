// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigSecurityGroup.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="SecurityGroup" />.
    /// </summary>
    [XmlRoot("securityGroup")]
    [XmlType("securityGroup")]
    [Serializable]
    public class ConfigSecurityGroup : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it's the default.
        /// </summary>
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute("securityGroupId")]
        public string SecurityGroupId { get; set; }

        /// <summary>
        /// Gets or sets the inbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("inboundRules")]
        [XmlArrayItem("inboundRule", typeof(ConfigSecurityGroupInboundRule))]
        public ConfigSecurityGroupInboundRule[] InboundRules { get; set; }

        /// <summary>
        /// Gets or sets the outbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("outboundRules")]
        [XmlArrayItem("outboundRule", typeof(ConfigSecurityGroupOutboundRule))]
        public ConfigSecurityGroupOutboundRule[] OutboundRules { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.SecurityGroupId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Base configuration for a <see cref="ConfigSecurityGroupInboundRule" /> or <see cref="ConfigSecurityGroupOutboundRule" />.
    /// </summary>
    [Serializable]
    public abstract class ConfigSecurityGroupRuleBase : ConfigTrafficRuleBase
    {
    }

    /// <summary>
    /// Configuration for an inbound <see cref="SecurityGroupRule" />.
    /// </summary>
    [Serializable]
    public class ConfigSecurityGroupInboundRule : ConfigSecurityGroupRuleBase
    {
        /// <summary>
        /// Gets or sets the source name.
        /// </summary>
        [XmlAttribute("sourceRef")]
        public string SourceRef { get; set; }

        /// <summary>
        /// Gets the rule direction.
        /// </summary>
        [XmlIgnore]
        public override TrafficRuleDirection RuleDirection => TrafficRuleDirection.Inbound;

        /// <summary>
        /// Gets the name of the item to find the CIDR on.
        /// </summary>
        [XmlIgnore]
        public override string CidrNameRef => this.SourceRef;
    }

    /// <summary>
    /// Configuration for an outbound <see cref="SecurityGroupRule" />.
    /// </summary>
    [Serializable]
    public class ConfigSecurityGroupOutboundRule : ConfigSecurityGroupRuleBase
    {
        /// <summary>
        /// Gets or sets the destination name.
        /// </summary>
        [XmlAttribute("destinationRef")]
        public string DestinationRef { get; set; }

        /// <summary>
        /// Gets the rule direction.
        /// </summary>
        [XmlIgnore]
        public override TrafficRuleDirection RuleDirection => TrafficRuleDirection.Outbound;

        /// <summary>
        /// Gets the name of the item to find the CIDR on.
        /// </summary>
        [XmlIgnore]
        public override string CidrNameRef => this.DestinationRef;
    }
}
