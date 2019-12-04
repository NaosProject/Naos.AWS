// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveRegion.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Interface for objects that have a name.
    /// </summary>
    public interface IHaveRegion
    {
        /// <summary>
        /// Gets the region.
        /// </summary>
        string Region { get; }
    }
}
