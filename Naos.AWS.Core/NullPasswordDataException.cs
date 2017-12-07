// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullPasswordDataException.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;

    /// <summary>
    /// Exception for when the password data can't be retrieved.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Want to control properties through constructor.")]
    [Serializable]
    public class NullPasswordDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullPasswordDataException"/> class.
        /// </summary>
        /// <param name="instanceId">ID of the instance that failed to get a password.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PasswordData", Justification = "Spelling/name is correct.")]
        public NullPasswordDataException(string instanceId)
            : base("Failed to get PasswordData (either this is not a windows machine or it's not yet ready to provide the password); Instance ID: " + instanceId)
        {
            this.InstanceId = instanceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullPasswordDataException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected NullPasswordDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.InstanceId = info.GetString(nameof(this.InstanceId));
        }

        /// <inheritdoc cref="Exception" />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.InstanceId), this.InstanceId);
        }

        /// <summary>
        /// Gets the ID of the instance that failed to get password data.
        /// </summary>
        public string InstanceId { get; private set; }
    }
}