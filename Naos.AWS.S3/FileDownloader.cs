// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDownloader.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;

    using Its.Log.Instrumentation;
    using Spritely.Redo;

    /// <summary>
    /// Class to download files from Amazon S3.
    /// </summary>
    public class FileDownloader : S3FileBase, IDownloadFiles
    {
        /// <inheritdoc cref="S3FileBase"/>
        public FileDownloader(string accessKey, string secretKey)
            : base(accessKey, secretKey)
        {
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, string destinationFilePath)
        {
            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationFilePath);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, Stream destinationStream)
        {
            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationStream);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, string destinationFilePath)
        {
            using (var fileStream = File.Create(destinationFilePath))
            {
                await this.DownloadFileAsync(region, bucketName, keyName, fileStream);
            }
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, Stream destinationStream)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.AccessKey, this.SecretKey, regionEndpoint))
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                using (var response = await
                                          Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                              .WithMaxRetries(3)
                                              .WithReporter(_ => Log.Write(new LogEntry("Retrying Download File due to error.", _)))
                                              // ReSharper disable once AccessToDisposedClosure
                                              .Run(() => client.GetObjectAsync(request))
                                              .Now())
                {
                    using (var responseStream = response.ResponseStream)
                    {
                        await responseStream.CopyToAsync(destinationStream);
                        VerifyChecksum(destinationStream, response);
                    }
                }
            }
        }

        private static void VerifyChecksum(Stream destinationStream, GetObjectResponse response)
        {
            Tuple<HashAlgorithmName, string> checksumInfo = GetSavedChecksum(response);
            if (checksumInfo == null)
            {
                return;
            }

            string computedChecksum = HashAlgorithmHelper.ComputeHash(checksumInfo.Item1, destinationStream);
            if (checksumInfo.Item2 != computedChecksum)
            {
                throw new ChecksumVerificationException(checksumInfo.Item1, checksumInfo.Item2, computedChecksum);
            }
        }

        private static Tuple<HashAlgorithmName, string> GetSavedChecksum(GetObjectResponse response)
        {
            // key will be of the form x-amz-meta-{hashType}-checksum  e.g. x-amz-meta-sha256-checksum
            var firstChecksumKey = response.Metadata.Keys.FirstOrDefault(_ => _.EndsWith(MetadataKeyChecksumSuffix));

            return firstChecksumKey != null ?
                Tuple.Create(new HashAlgorithmName(firstChecksumKey.Split('-').Skip(3).First().ToUpperInvariant()), response.Metadata[firstChecksumKey]) : null;
        }
    }
}
