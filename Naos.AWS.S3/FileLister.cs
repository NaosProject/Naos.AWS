// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLister.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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

    using Naos.AWS.Contract;

    using Spritely.Recipes;

    /// <summary>
    /// Class to list files from Amazon S3.
    /// </summary>
    public class FileLister : AwsInteractionBase, IListFiles
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileLister(string accessKey, string secretKey)
            : this(new CredentialContainer(accessKey, secretKey))
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
            region.Named(nameof(region)).Must().NotBeWhiteSpace().OrThrow();
            bucketName.Named(nameof(bucketName)).Must().NotBeWhiteSpace().OrThrow();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();
            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                var request = new ListObjectsRequest { BucketName = bucketName, Prefix = keyPrefixSearchPattern };

                var objects = await client.ListObjectsAsync(request);
                var ret = new List<CloudFile>();
                if (objects?.S3Objects != null && objects.S3Objects.Count > 0)
                {
                    ret.AddRange(
                        objects.S3Objects.Select(
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

                return ret;
            }
        }
    }
}
