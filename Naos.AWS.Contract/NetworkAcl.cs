// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkAcl.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Route table model object.
    /// </summary>
    public class NetworkAcl : IAwsObject
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this is the default ACL of its VPC.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the subnets the ACL is associated with.
        /// </summary>
        public Subnet[] AssociatedSubnets { get; set; }

        /// <summary>
        /// Gets or sets the inbound rules.
        /// </summary>
        public NetworkAclRule[] InboundRules { get; set; }

        /// <summary>
        /// Gets or sets the outbound rules.
        /// </summary>
        public NetworkAclRule[] OutboundRules { get; set; }
     }
}
