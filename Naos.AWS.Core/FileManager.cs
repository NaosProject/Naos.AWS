// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileManager.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;

    using Naos.AWS.Contract;

    /// <summary>
    /// Class to operate against S3.
    /// </summary>
    public class FileManager : IManageFiles
    {
        private readonly string accessKey;

        private readonly string secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="accessKey">Access key with rights to download files in specified buckets.</param>
        /// <param name="secretKey">Secret key with rights to download files in specified buckets.</param>
        public FileManager(string accessKey, string secretKey)
        {
            this.accessKey = accessKey;
            this.secretKey = secretKey;
        }

        /// <inheritdoc />
        public async Task UploadFileAsync(string region, string bucketName, string keyName, string sourceFilePath)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.accessKey, this.secretKey, regionEndpoint))
            {
                using (var transferUtility = new TransferUtility(client))
                {
                    await transferUtility.UploadAsync(sourceFilePath, bucketName, keyName);
                }
            }
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, string destinationFilePath)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.accessKey, this.secretKey, regionEndpoint))
            {
                using (var transferUtility = new TransferUtility(client))
                {
                    await transferUtility.DownloadAsync(destinationFilePath, bucketName, keyName);
                }
            }
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
            using (var client = new AmazonS3Client(this.accessKey, this.secretKey, regionEndpoint))
            {
                var request = new ListObjectsRequest { BucketName = bucketName, Prefix = keyPrefixSearchPattern };

                var objects = await client.ListObjectsAsync(request);
                var ret = new List<CloudFile>();
                if (objects != null && objects.S3Objects != null && objects.S3Objects.Count > 0)
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
