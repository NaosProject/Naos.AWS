// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigNetworkAcl.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="NetworkAcl" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    [XmlRoot("networkAcl")]
    [XmlType("networkAcl")]
    [Serializable]
    public class ConfigNetworkAcl : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it is default.
        /// </summary>
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the attached subnet name.
        /// </summary>
        [XmlAttribute("subnetRef")]
        public string SubnetRef { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        [XmlAttribute("networkAclId")]
        public string NetworkAclId { get; set; }

        /// <summary>
        /// Gets or sets the inbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("inboundRules")]
        [XmlArrayItem("inboundRule", typeof(ConfigNetworkAclInboundRule))]
        public ConfigNetworkAclInboundRule[] InboundRules { get; set; }

        /// <summary>
        /// Gets or sets the outbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("outboundRules")]
        [XmlArrayItem("outboundRule", typeof(ConfigNetworkAclOutboundRule))]
        public ConfigNetworkAclOutboundRule[] OutboundRules { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.NetworkAclId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Base configuration for a <see cref="ConfigNetworkAclInboundRule" /> or <see cref="ConfigNetworkAclOutboundRule" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    [Serializable]
    public abstract class ConfigNetworkAclRuleBase : ConfigTrafficRuleBase
    {
        /// <summary>
        /// Constant for "type" all types.
        /// </summary>
        public const string AllTypesValue = "ALL Traffic";

        /// <summary>
        /// Gets or sets the rule number.
        /// </summary>
        [XmlAttribute("ruleNumber")]
        public int RuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the ICMP type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Spelling/name is correct.")]
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [XmlAttribute("action")]
        public string Action { get; set; }

        /// <summary>
        /// The get <see cref="Type" /> value for use with the API.
        /// </summary>
        /// <returns>Type to use with API.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Justification = "Name/spelling is correct.")]
        public int GetTypeForApi()
        {
            if (this.Type.Equals(AllTypesValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return -1;
            }
            else
            {
                return int.Parse(this.Type, CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Configuration for an inbound <see cref="NetworkAclRule" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    [Serializable]
    public class ConfigNetworkAclInboundRule : ConfigNetworkAclRuleBase
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
    /// Configuration for an outbound <see cref="NetworkAclRule" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    [Serializable]
    public class ConfigNetworkAclOutboundRule : ConfigNetworkAclRuleBase
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