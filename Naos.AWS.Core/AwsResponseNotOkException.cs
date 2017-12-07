// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsResponseNotOkException.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;

    using Amazon.Runtime;

    using Newtonsoft.Json;

    /// <summary>
    /// Exception signaling that a result from AWS was not HttpStatus of 200 (OK).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Want to force parameters.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly", Justification = "Objects do not serialize great.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    [Serializable]
    public class AwsResponseNotOkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AwsResponseNotOkException"/> class.
        /// </summary>
        /// <param name="request">Request sent.</param>
        /// <param name="response">Response received.</param>
        public AwsResponseNotOkException(AmazonWebServiceRequest request, AmazonWebServiceResponse response)
            : base("Request: " + JsonConvert.SerializeObject(request) + Environment.NewLine + "Response: " + JsonConvert.SerializeObject(response))
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