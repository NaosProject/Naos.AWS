// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tagger.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Linq;
    using System.Net;

    using Amazon;
    using Amazon.EC2.Model;

    using Naos.AWS.Contract;

    /// <summary>
    /// Class to tag an object.
    /// </summary>
    public static class Tagger
    {
        private const string NameTagKey = "Name";

        /// <summary>
        /// Adds a name tag to the instance using the name on the object.
        /// </summary>
        /// <param name="credentials">Credentials to name the object.</param>
        /// <param name="awsObject">Object to add tag name for.</param>
        public static void TagName(CredentialContainer credentials, IAwsObject awsObject)
        {
            var awsCredentials = credentials.ToAwsCredentials();
            var regionEndpoint = RegionEndpoint.GetBySystemName(awsObject.Region);

            var request = new CreateTagsRequest()
                              {
                                  Resources = new[] { awsObject.Id }.ToList(),
                                  Tags = new[] { new Tag(NameTagKey, awsObject.Name) }.ToList(),
                              };

            using (var client = AWSClientFactory.CreateAmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var ret = client.CreateTags(request);
                if (ret.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new AwsCallResultNotOkException("Adding tag Name:" + awsObject.Name, awsObject.Id);
                }
            }
        }
    }
}
