// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subnet.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public Vpc ParentVpc { get; set; }

        /// <summary>
        /// Gets or sets the registered route table.
        /// </summary>
        public RouteTable RegisteredRouteTable { get; set; }

        /// <summary>
        /// Gets or sets the CIDR block.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the availability zone.
        /// </summary>
        public string AvailabilityZone { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public Subnet DeepClone()
        {
            var ret = new Subnet()
                          {
                              AvailabilityZone = this.AvailabilityZone,
                              Cidr = this.Cidr,
                              Id = this.Id,
                              Name = this.Name,
                              ParentVpc = this.ParentVpc?.DeepClone(),
                              Region = this.Region,
                              RegisteredRouteTable = this.RegisteredRouteTable?.DeepClone(),
                          };

            return ret;
        }
    }
}
