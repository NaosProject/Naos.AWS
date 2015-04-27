// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveRegion.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
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