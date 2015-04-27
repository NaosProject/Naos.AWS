// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkAclRule.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Network ACL model object.
    /// </summary>
    public class NetworkAclRule
    {
        /// <summary>
        /// Gets or sets the rule number (controls order).
        /// </summary>
        public int RuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of traffic.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the protocol to allow or deny.
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the range of ports to apply to.
        /// </summary>
        public string PortRange { get; set; }

        /// <summary>
        /// Gets or sets the source/destination (depending on inbound or outbound rule) CIDR or IP address.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the action to apply to the rule when met.
        /// </summary>
        public Enums.RuleAction Action { get; set; }
    }
}