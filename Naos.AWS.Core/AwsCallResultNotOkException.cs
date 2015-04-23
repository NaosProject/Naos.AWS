// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsCallResultNotOkException.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    /// <summary>
    /// Exception signaling that a result from AWS was not HttpStatus of 200 (OK).
    /// </summary>
    public class AwsCallResultNotOkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AwsCallResultNotOkException class.
        /// </summary>
        /// <param name="message">Message about the event causing the exception.</param>
        /// <param name="id">Id of the resource causing the exception.</param>
        public AwsCallResultNotOkException(string message, string id)
            : base(message + " - Resource: " + id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the Id of the resource causing the exception.
        /// </summary>
        public string Id { get; private set; }
    }
}