// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityGroupExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
    /// Extensions on <see cref="SecurityGroup" />
    /// </summary>
    public static class SecurityGroupExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="securityGroup">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this SecurityGroup securityGroup, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(securityGroup.Region);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeSecurityGroupsRequest() { GroupIds = new[] { securityGroup.Id }.ToList() };

                var response = await client.DescribeSecurityGroupsAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.SecurityGroups.Any(_ => _.GroupId == securityGroup.Id);
            }
        }
    }
}
