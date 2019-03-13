// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityGroup.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Security group model object.
    /// </summary>
    public class SecurityGroup : IAwsObject
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
        /// Gets or sets a value indicating whether or not this is the default security group of its VPC.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the inbound rules.
        /// </summary>
        public IReadOnlyCollection<SecurityGroupRule> InboundRules { get; set; }

        /// <summary>
        /// Gets or sets the outbound rules.
        /// </summary>
        public IReadOnlyCollection<SecurityGroupRule> OutboundRules { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public SecurityGroup DeepClone()
        {
            var ret = new SecurityGroup()
                          {
                              Id = this.Id,
                              InboundRules = this.InboundRules?.Select(_ => _.DeepClone()).ToList(),
                              IsDefault = this.IsDefault,
                              Name = this.Name,
                              OutboundRules = this.OutboundRules?.Select(_ => _.DeepClone()).ToList(),
                              Region = this.Region,
                          };

            return ret;
        }
    }
}
