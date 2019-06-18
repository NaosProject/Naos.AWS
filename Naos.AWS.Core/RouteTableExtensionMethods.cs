// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteTableExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;

    using Naos.AWS.Domain;

    /// <summary>
    /// Extensions on <see cref="RouteTable" />.
    /// </summary>
    public static class RouteTableExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="routeTable">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this RouteTable routeTable, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(routeTable.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeRouteTablesRequest() { RouteTableIds = new[] { routeTable.Id }.ToList() };

                var response = await client.DescribeRouteTablesAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.RouteTables.Any(_ => _.RouteTableId == routeTable.Id);
            }
        }
    }
}
