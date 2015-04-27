// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityGroupRule.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Security group rule model object.
    /// </summary>
    public class SecurityGroupRule
    {
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
    }
}