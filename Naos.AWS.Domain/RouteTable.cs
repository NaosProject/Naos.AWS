﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteTable.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Route table model object.
    /// </summary>
    public class RouteTable : IAwsObject
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
        /// Gets or sets a value indicating whether or not this is the default route table of its VPC.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the parent VPC Id.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public string VpcId { get; set; }

        /// <summary>
        /// Gets or sets the route entries.
        /// </summary>
        public IReadOnlyCollection<RouteEntry> Routes { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public RouteTable DeepClone()
        {
            var ret = new RouteTable()
                          {
                              Id = this.Id,
                              IsDefault = this.IsDefault,
                              Name = this.Name,
                              Region = this.Region,
                              VpcId = this.VpcId,
                              Routes = this.Routes?.Select(_ => _.DeepClone()).ToList(),
                          };

            return ret;
        }
    }
}
