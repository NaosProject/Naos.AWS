// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsObjectExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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

    using Naos.AWS.Contract;

    using Spritely.Recipes;

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
            new { awsObject }.Must().NotBeNull().OrThrow();

            if (awsObject.Id.StartsWith("i-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.Instance;
            }
            else if (awsObject.Id.StartsWith("vol-", StringComparison.OrdinalIgnoreCase))
            {
                return AwsObjectType.EbsVolume;
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
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task TagNameInAwsAsync(this IAwsObject awsObject, CredentialContainer credentials = null)
        {
            await AddTagInAwsAsync(awsObject, Constants.NameTagKey, awsObject.Name, credentials);
        }

        /// <summary>
        /// Adds a tag to the object.
        /// </summary>
        /// <param name="awsObject">Object to tag.</param>
        /// <param name="tagName">Name of tag.</param>
        /// <param name="tagValue">Value of tag.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task AddTagInAwsAsync(this IAwsObject awsObject, string tagName, string tagValue, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(awsObject.Region);

            // make sure the object is there so we can tag it (eventual consistency issue...)
            await WaitUntil.AwsObjectExists(awsObject, credentials);

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
