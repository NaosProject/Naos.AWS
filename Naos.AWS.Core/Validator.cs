// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Net;

    using Amazon.Runtime;

    /// <summary>
    /// Validate responses from SDK.
    /// </summary>
    public static class Validator
    {
        #pragma warning disable CS3001 // Argument type is not CLS-compliant

        /// <summary>
        /// Throws an AwsResponseNotOkException when the HTTP response is not 200 (OK).
        /// </summary>
        /// <param name="request">Request sent.</param>
        /// <param name="response">Response received.</param>
        public static void ThrowOnBadResult(AmazonWebServiceRequest request, AmazonWebServiceResponse response)
        {
            if (response?.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new AwsResponseNotOkException(request, response);
            }
        }

        #pragma warning restore CS3001 // Argument type is not CLS-compliant
    }
}
