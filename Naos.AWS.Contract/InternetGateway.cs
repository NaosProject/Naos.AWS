// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternetGateway.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// TODO: Fill out the description.
    /// </summary>
    public class InternetGateway : IAwsObject
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
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public InternetGateway DeepClone()
        {
            var ret = new InternetGateway() { Id = this.Id, Name = this.Name, Region = this.Region, };

            return ret;
        }
    }
}
