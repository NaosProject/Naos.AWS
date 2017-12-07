// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ami.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// AMI model object.
    /// </summary>
    public class Ami : IAwsObject
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
        /// Gets or sets the optional ability to find the AMI id using provided logic.
        /// </summary>
        public AmiSearchStrategy SearchStrategy { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public Ami DeepClone()
        {
            var ret = new Ami()
                          {
                              Id = this.Id,
                              Name = this.Name,
                              Region = this.Region,
                              SearchStrategy = this.SearchStrategy == null ? null : this.SearchStrategy.DeepClone(),
                          };

            return ret;
        }
    }
}
