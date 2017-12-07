// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityGroupRule.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
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

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public SecurityGroupRule DeepClone()
        {
            var ret = new SecurityGroupRule()
                          {
                              Filter = this.Filter,
                              PortRange = this.PortRange,
                              Protocol = this.Protocol,
                          };

            return ret;
        }
    }
}