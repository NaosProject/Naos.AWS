// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsObjectExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Domain;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Extension methods on the AWS object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    public static class AwsObjectExtensionMethods
    {
        /// <summary>
        /// Inspects the ID and infers the type of AWS object it is.
        /// </summary>
        /// <param name="awsObject">AWS object to get type of.</param>
        /// <returns>Object type that was inferred from ID prefix.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        public static AwsObjectType InferObjectTypeFromId(this IHaveId awsObject)
        {
            new { awsObject }.Must().NotBeNull();

            if (awsObject.Id.StartsWith("i-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.Instance;
            }
            else if (awsObject.Id.StartsWith("vol-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.EbsVolume;
            }
            else if (awsObject.Id.StartsWith("igw-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.InternetGateway;
            }
            else if (awsObject.Id.StartsWith("eipalloc-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.ElasticIp;
            }
            else if (awsObject.Id.StartsWith("vpc-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.Vpc;
            }
            else if (awsObject.Id.StartsWith("rtb-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.RouteTable;
            }
            else if (awsObject.Id.StartsWith("subnet-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.Subnet;
            }
            else if (awsObject.Id.StartsWith("acl-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.NetworkAcl;
            }
            else if (awsObject.Id.StartsWith("sg-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.SecurityGroup;
            }
            else if (awsObject.Id.StartsWith("nat-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.NatGateway;
            }
            else
            {
                throw new ArgumentException("Can't infer AWS object type from the ID: " + awsObject.Id, nameof(awsObject));
            }
        }

        /// <summary>
        /// Adds a tag to the object.
        /// </summary>
        /// <param name="awsObject">Object to tag with name.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task TagNameInAwsAsync(this IAwsObject awsObject, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            await AddTagInAwsAsync(awsObject, Constants.NameTagKey, awsObject.Name, timeout, credentials);
        }

        /// <summary>
        /// Adds a tag to the object.
        /// </summary>
        /// <param name="awsObject">Object to tag.</param>
        /// <param name="tagName">Name of tag.</param>
        /// <param name="tagValue">Value of tag.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task AddTagInAwsAsync(this IAwsObject awsObject, string tagName, string tagValue, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(awsObject.Region);

            // make sure the object is there so we can tag it (eventual consistency issue...)
            await WaitUntil.AwsObjectExists(awsObject, timeout, credentials);

            var request = new CreateTagsRequest()
            {
                Resources = new[] { awsObject.Id }.ToList(),
                Tags = new[] { new Tag(tagName, tagValue) }.ToList(),
            };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                CreateTagsResponse result = await client.CreateTagsAsync(request);
                Validator.ThrowOnBadResult(request, result);
            }
        }
    }
}
