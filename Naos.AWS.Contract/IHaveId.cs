// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveId.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Interface for objects that have an id.
    /// </summary>
    public interface IHaveId
    {
        /// <summary>
        /// Gets the Id.
        /// </summary>
        string Id { get; }
    }
}