// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveId.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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