// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTrafficRuleBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Serialization;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Base configuration for a <see cref="ConfigNetworkAclRuleBase" /> or <see cref="ConfigSecurityGroupRuleBase" />.
    /// </summary>
    [Serializable]
    public abstract class ConfigTrafficRuleBase
    {
        /// <summary>
        /// Magic string to signify all protocols.
        /// </summary>
        public const string AllProtocolsValue = "ALL";

        /// <summary>
        /// Magic string to signify the TCP protocol.
        /// </summary>
        public const string TcpProtocolValue = "TCP";

        /// <summary>
        /// Magic string to signify the UDP protocol.
        /// </summary>
        public const string UdpProtocolValue = "UDP";

        /// <summary>
        /// Magic string to signify all ports.
        /// </summary>
        public const string AllPortsValue = "ALL";

        /// <summary>
        /// Gets or sets the protocol.
        /// </summary>
        [XmlAttribute("protocol")]
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the port range.
        /// </summary>
        [XmlAttribute("portRange")]
        public string PortRange { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [XmlAttribute("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets the rule direction.
        /// </summary>
        [XmlIgnore]
        public abstract TrafficRuleDirection RuleDirection { get; }

        /// <summary>
        /// Gets the name of the item to find the CIDR on.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        [XmlIgnore]
        public abstract string CidrNameRef { get; }

        /// <summary>
        /// The get <see cref="Protocol" /> value for use with the API.
        /// </summary>
        /// <returns>Protocol to use with API.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Justification = "Name/spelling is correct.")]
        public string GetProtocolForApi()
        {
            if (this.Protocol.Equals(AllProtocolsValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return "-1";
            }
            else if (this.Protocol.Equals(TcpProtocolValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return "6";
            }
            else if (this.Protocol.Equals(UdpProtocolValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return "17";
            }
            else
            {
                return this.Protocol;
            }
        }

        /// <summary>
        /// The get <see cref="PortRange" /> value for use with the API.
        /// </summary>
        /// <returns>PortRange to use with API.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Justification = "Name/spelling is correct.")]
        public PortRangeContainer GetPortRangeForApi()
        {
            if (this.PortRange.Equals(AllPortsValue, StringComparison.CurrentCultureIgnoreCase))
            {
                return new PortRangeContainer { From = -1, To = -1 };
            }
            else if (this.PortRange.Contains("-"))
            {
                var split = this.PortRange.Split('-');
                return new PortRangeContainer { From = int.Parse(split[0], CultureInfo.InvariantCulture), To = int.Parse(split[1], CultureInfo.InvariantCulture) };
            }
            else
            {
                return new PortRangeContainer { From = int.Parse(this.PortRange, CultureInfo.InvariantCulture), To = int.Parse(this.PortRange, CultureInfo.InvariantCulture) };
            }
        }

        /// <summary>
        /// The get <see cref="CidrNameRef" /> value for use with the API.
        /// </summary>
        /// <param name="nameToCidrMap">Name to available CIDR map.</param>
        /// <returns>Dererenced CIDR to use with API.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Justification = "Name/spelling is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public string GetCidrForApi(IReadOnlyDictionary<string, string> nameToCidrMap)
        {
            new { nameToCidrMap }.AsArg().Must().NotBeNull();

            nameToCidrMap.TryGetValue(this.CidrNameRef, out string cidr).AsOp(FormattableString.Invariant($"Did-not-find-cidr-for-{this.CidrNameRef}")).Must().BeTrue();

            return cidr;
        }

        /// <summary>
        /// Container object to carry port ranges.
        /// </summary>
        public class PortRangeContainer
        {
            /// <summary>
            /// Gets or sets the from port.
            /// </summary>
            public int From { get; set; }

            /// <summary>
            /// Gets or sets the to port.
            /// </summary>
            public int To { get; set; }
        }

        /// <summary>
        /// Enumeration of directions for rules.
        /// </summary>
        public enum TrafficRuleDirection
        {
            /// <summary>
            /// Invalid default.
            /// </summary>
            Invalid,

            /// <summary>
            /// Inbound direction.
            /// </summary>
            Inbound,

            /// <summary>
            /// Outbound direction.
            /// </summary>
            Outbound,
        }
    }
}
