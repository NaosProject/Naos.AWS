// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validator.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
    }
}