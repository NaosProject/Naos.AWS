// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
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
