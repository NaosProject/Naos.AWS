﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteEntry.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Route table entry model object.
    /// </summary>
    public class RouteEntry
    {
        /// <summary>
        /// Gets or sets the destination CIDR to send to the routable entity.
        /// </summary>
        public string DestinationCidr { get; set; }

        /// <summary>
        /// Gets or sets the type of target.
        /// </summary>
        public Enums.RoutableType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the target ID.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public RouteEntry DeepClone()
        {
            var ret = new RouteEntry()
                          {
                              DestinationCidr = this.DestinationCidr,
                              TargetId = this.TargetId,
                              TargetType = this.TargetType,
                          };

            return ret;
        }
    }
}