// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkAcl.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Route table model object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
    public class NetworkAcl : IAwsObject
    {
        /// <summary>
        /// Rule number that indicates the last catch all rule.
        /// </summary>
        public const long FallThroughRuleNumber = 32767;

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Want these clearly ordered.")]
        public Subnet[] AssociatedSubnets { get; set; }

        /// <summary>
        /// Gets or sets the inbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Want these clearly ordered.")]
        public NetworkAclRule[] InboundRules { get; set; }

        /// <summary>
        /// Gets or sets the outbound rules.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Want these clearly ordered.")]
        public NetworkAclRule[] OutboundRules { get; set; }
    }
}
