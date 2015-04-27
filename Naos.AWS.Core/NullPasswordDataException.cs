// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPasswordDataException.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    /// <summary>
    /// Exception for when the password data can't be retrieved.
    /// </summary>
    public class NullPasswordDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullPasswordDataException"/> class.
        /// </summary>
        /// <param name="instanceId">ID of the instance that failed to get a password.</param>
        public NullPasswordDataException(string instanceId) : base("Failed to get PasswordData (either this is not a windows machine or it's not yet ready to provide the password); Instance ID: " + instanceId)
        {
            this.InstanceId = instanceId;
        }

        /// <summary>
        /// Gets the ID of the instance that failed to get password data.
        /// </summary>
        public string InstanceId { get; private set; }
    }
}