// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveName.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
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