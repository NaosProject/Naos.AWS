// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vpc.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Virtual private cloud model object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public string Cidr { get; set; }

        /// <summary>
        /// Gets or sets the tenancy.
        /// </summary>
        public string Tenancy { get; set; }

        /// <summary>
        /// Gets or sets the InternetGateway for the VPC to use.
        /// </summary>
        public InternetGateway InternetGateway { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public Vpc DeepClone()
        {
            var ret = new Vpc()
                          {
                              Cidr = this.Cidr,
                              Id = this.Id,
                              InternetGateway = this.InternetGateway.DeepClone(),
                              Name = this.Name,
                              Region = this.Region,
                              Tenancy = this.Tenancy,
                          };

            return ret;
        }
    }
}
