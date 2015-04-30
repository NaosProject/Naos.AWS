// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElasticIp.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Elastic (public) IP address model object.
    /// </summary>
    public class ElasticIp : IAwsObject
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
        /// Gets or sets the public IP address of the elastic IP allocation.
        /// </summary>
        public string PublicIpAddress { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public ElasticIp DeepClone()
        {
            throw new System.NotImplementedException();
        }
    }
}