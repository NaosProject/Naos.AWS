// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSearcher.cs" company="Naos">
//   Copyright 2017 Naos
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

    /// <summary>
    /// Class to search and list files from Amazon S3.
    /// </summary>
    public class FileSearcher : S3FileBase, IListFiles
    {
        /// <inheritdoc cref="S3FileBase"/>
        public FileSearcher(string accessKey, string secretKey)
            : base(accessKey, secretKey)
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
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.AccessKey, this.SecretKey, regionEndpoint))
            {
                var request = new ListObjectsRequest { BucketName = bucketName, Prefix = keyPrefixSearchPattern };

                var objects = await client.ListObjectsAsync(request);
                var ret = new List<CloudFile>();
                if (objects?.S3Objects != null && objects.S3Objects.Count > 0)
                {
                    ret.AddRange(
                        objects.S3Objects.Select(
                            _ =>
                                new CloudFile
                                {
                                    Region = region,
                                    BucketName = bucketName,
                                    KeyName = _.Key,
                                    OwnerId = _.Owner == null ? "[Null Owner]" : _.Owner.Id,
                                    OwnerName = _.Owner == null ? "[Null Owner]" : _.Owner.DisplayName,
                                    LastModified = _.LastModified,
                                    Size = _.Size
                                }));
                }

                return ret;
            }
        }
    }
}
