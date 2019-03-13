// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatGateway.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// TODO: Fill out the description.
    /// </summary>
    public class NatGateway : IAwsObject
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
        public NatGateway DeepClone()
        {
            var ret = new NatGateway() { Id = this.Id, Name = this.Name, Region = this.Region, };

            return ret;
        }
    }

    /// <summary>
    /// Enumeration of possible states.
    /// </summary>
    public enum NatGatewayState
    {
        /// <summary>
        /// Invalid default state.
        /// </summary>
        Invalid,

        /// <summary>
        /// Available state.
        /// </summary>
        Available,

        /// <summary>
        /// Deleted state.
        /// </summary>
        Deleted,

        /// <summary>
        /// Deleting state.
        /// </summary>
        Deleting,

        /// <summary>
        /// Failed state.
        /// </summary>
        Failed,

        /// <summary>
        /// Pending state.
        /// </summary>
        Pending,
    }
}
