﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwsObjectExtensionMethods.cs" company="Naos">
//   Copyright 2015 Naos
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

    /// <summary>
    /// Extension methods on the AWS object.
    /// </summary>
    public static class AwsObjectExtensionMethods
    {
        /// <summary>
        /// Inspects the ID and infers the type of AWS object it is.
        /// </summary>
        /// <param name="awsObject">AWS object to get type of.</param>
        /// <returns>Object type that was inferred from ID prefix.</returns>
        public static AwsObjectType InferObjectTypeFromId(this IAwsObject awsObject)
        {
            if (awsObject.Id.StartsWith("i-"))
            {
                return AwsObjectType.Instance;
            }
            else if (awsObject.Id.StartsWith("vol-"))
            {
                return AwsObjectType.EbsVolume;
            }
            else
            {
                throw new ArgumentException("Can't infer AWS object type from the ID: " + awsObject.Id, "awsObject");
            }
        }

        /// <summary>
        /// Adds a tag to the object.
        /// </summary>
        /// <param name="awsObject">Object to tag with name.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
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
