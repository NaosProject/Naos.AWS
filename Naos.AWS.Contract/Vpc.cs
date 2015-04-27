// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vpc.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Virtual private cloud model object.
    /// </summary>
    public class Vpc : IAwsObject
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
        /// Gets or sets the CIDR.
        /// </summary>
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the tenancy.
        /// </summary>
        public string Tenancy { get; set; }

        /// <summary>
        /// Gets or sets the InternetGateway for the VPC to use.
        /// </summary>
        public InternetGateway InternetGateway { get; set; }
    }
}
