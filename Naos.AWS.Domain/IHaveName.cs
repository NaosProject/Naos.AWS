// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveName.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Interface for objects that have a name.
    /// </summary>
    public interface IHaveName
    {
        /// <summary>
        /// Gets the Name.
        /// </summary>
        string Name { get; }
    }
}