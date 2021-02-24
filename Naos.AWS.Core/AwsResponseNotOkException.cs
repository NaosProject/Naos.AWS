// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsResponseNotOkException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    using Amazon.Runtime;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Exception signaling that a result from AWS was not HttpStatus of 200 (OK).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Want to force parameters.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "Objects do not serialize great.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    [Serializable]
    public class AwsResponseNotOkException : Exception
    {
        private static readonly IStringSerialize ErrorSerializer = new ObcJsonSerializer();

        /// <summary>
        /// Initializes a new instance of the <see cref="AwsResponseNotOkException"/> class.
        /// </summary>
        /// <param name="request">Request sent.</param>
        /// <param name="response">Response received.</param>
        public AwsResponseNotOkException(AmazonWebServiceRequest request, AmazonWebServiceResponse response)
            : base("Request: " + ErrorSerializer.SerializeToString(request) + Environment.NewLine + "Response: " + ErrorSerializer.SerializeToString(response))
        {
            this.Request = request;
            this.Response = response;
        }

        /// <summary>
        /// Gets the request object.
        /// </summary>
        public AmazonWebServiceRequest Request { get; private set; }

        /// <summary>
        /// Gets the response object.
        /// </summary>
        public AmazonWebServiceResponse Response { get; private set; }
    }
}
