// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteTable.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
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
        /// Gets or sets the route entries.
        /// </summary>
        public ICollection<RouteEntry> Routes { get; set; }

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
                              Routes = this.Routes == null ? null : this.Routes.Select(_ => _.DeepClone()).ToList(),
                          };

            return ret;
        }
    }
}
