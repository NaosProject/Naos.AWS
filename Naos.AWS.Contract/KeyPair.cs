﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyPair.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Encryption key pair model object.
    /// </summary>
    public class KeyPair : IHaveName, IHaveRegion
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the key name (AWS side name).
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Gets or sets the contents of the PEM file used for encryption.
        /// </summary>
        public string PrivateKey { get; set; }
    }
}