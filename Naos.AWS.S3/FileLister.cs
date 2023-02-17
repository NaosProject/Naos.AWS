// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLister.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;

    using Naos.AWS.Domain;
    using OBeautifulCode.Assertion.Recipes;

    /// <summary>
    /// Class to list files from Amazon S3.
    /// </summary>
    public class FileLister : AwsInteractionBase, IListFiles
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileLister(string accessKey, string secretKey)
            : this(new CredentialContainer { AccessKeyId = accessKey, SecretAccessKey = secretKey })
        {
        }

        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileLister(CredentialContainer credentials)
            : base(credentials)
        {
        }

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName)
        {
            return await this.ListFilesAsync(region, bucketName, null);
        }

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName, string keyPrefixSearchPattern)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();
            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                var result = new List<CloudFile>();

                ListObjectsResponse response = null;

                do
                {
                    var request = new ListObjectsRequest
                    {
                        BucketName = bucketName,
                        Prefix = keyPrefixSearchPattern,
                        Marker = response?.NextMarker,
                    };

                    response = await client.ListObjectsAsync(request);

                    if (response?.S3Objects != null && response.S3Objects.Count > 0)
                    {
                        result.AddRange(
                            response.S3Objects.Select(
                                _ =>
                                    new CloudFile(
                                        region,
                                        bucketName,
                                        _.Key,
                                        _.Owner == null ? "[Null Owner]" : _.Owner.Id,
                                        _.Owner == null ? "[Null Owner]" : _.Owner.DisplayName,
                                        _.LastModified,
                                        _.Size)));
                    }
                }
                while (response?.IsTruncated ?? false);

                return result;
            }
        }
    }
}
