// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkAclRule.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Network ACL model object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    public class NetworkAclRule
    {
        /// <summary>
        /// Gets or sets the rule number (controls order).
        /// </summary>
        public int RuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of traffic.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Spelling/name is correct.")]
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
        public RuleAction Action { get; set; }
    }
}