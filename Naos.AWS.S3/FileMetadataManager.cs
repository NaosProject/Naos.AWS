// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMetadataManager.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;

    using Its.Log.Instrumentation;
    using Spritely.Redo;

    /// <summary>
    /// Class to manage file metadata for Amazon S3.
    /// </summary>
    public class FileMetadataManager : S3FileBase, IManageFileMetadata
    {
        /// <inheritdoc cref="S3FileBase"/>
        public FileMetadataManager(string accessKey, string secretKey)
            : base(accessKey, secretKey)
        {
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(UploadFileResult uploadFileResult)
        {
            return await this.GetFileMetadataAsync(uploadFileResult.Region, uploadFileResult.BucketName, uploadFileResult.KeyName);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.AccessKey, this.SecretKey, regionEndpoint))
            {
                var response = await
                                   Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                       .WithMaxRetries(3)
                                       .WithReporter(_ => Log.Write(new LogEntry("Retrying Get File Metadata due to error.", _)))
                                       // ReSharper disable once AccessToDisposedClosure
                                       .Run(() => client.GetObjectMetadataAsync(bucketName, keyName))
                                       .Now();

                IDictionary<string, string> metadata = new Dictionary<string, string>();

                foreach (var key in response.Metadata.Keys)
                {
                    // TODO: should we remove the x-amz-meta- prefix when returning keys?
                    metadata.Add(key, response.Metadata[key]);
                }

                return new ReadOnlyDictionary<string, string>(metadata);
            }
        }
    }
}
