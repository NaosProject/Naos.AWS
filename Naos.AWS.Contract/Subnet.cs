// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subnet.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Subnet model object.
    /// </summary>
    public class Subnet : IAwsObject
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
        /// Gets or sets the parent VPC.
        /// </summary>
        public Vpc ParentVpc { get; set; }

        /// <summary>
        /// Gets or sets the registered route table.
        /// </summary>
        public RouteTable RegisteredRouteTable { get; set; }

        /// <summary>
        /// Gets or sets the CIDR block.
        /// </summary>
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the availability zone.
        /// </summary>
        public string AvailabilityZone { get; set; }
    }
}
