// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDownloader.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;

    using Its.Log.Instrumentation;

    using Naos.Recipes.Cryptography.Hashing;

    using Spritely.Recipes;
    using Spritely.Redo;

    /// <summary>
    /// Class to download files from Amazon S3.
    /// </summary>
    public class FileDownloader : AwsInteractionBase, IDownloadFiles
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileDownloader(string accessKey, string secretKey)
            : base(accessKey, secretKey)
        {
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, string destinationFilePath, bool validateChecksumsIfPresent = true)
        {
            uploadFileResult.Named(nameof(uploadFileResult)).Must().NotBeNull().OrThrow();

            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationFilePath,
                validateChecksumsIfPresent);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, Stream destinationStream, bool validateChecksumsIfPresent = true)
        {
            uploadFileResult.Named(nameof(uploadFileResult)).Must().NotBeNull().OrThrow();

            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationStream,
                validateChecksumsIfPresent);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, string destinationFilePath, bool validateChecksumsIfPresent = true)
        {
            destinationFilePath.Named(nameof(destinationFilePath)).Must().NotBeWhiteSpace().OrThrow();

            using (var fileStream = File.Create(destinationFilePath))
            {
                await this.DownloadFileAsync(region, bucketName, keyName, fileStream);
            }
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, Stream destinationStream, bool validateChecksumsIfPresent = true)
        {
            region.Named(nameof(region)).Must().NotBeWhiteSpace().OrThrow();
            bucketName.Named(nameof(bucketName)).Must().NotBeWhiteSpace().OrThrow();
            keyName.Named(nameof(keyName)).Must().NotBeWhiteSpace().OrThrow();
            destinationStream.Named(nameof(destinationStream)).Must().NotBeNull().OrThrow();

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
                                              .RunAsync(() => client.GetObjectAsync(request))
                                              .Now())
                {
                    using (var responseStream = response.ResponseStream)
                    {
                        await responseStream.CopyToAsync(destinationStream);
                        if (validateChecksumsIfPresent)
                        {
                            VerifyChecksums(destinationStream, response);
                        }
                    }
                }
            }
        }

        private static void VerifyChecksums(Stream destinationStream, GetObjectResponse response)
        {
            foreach (var computedChecksum in GetSavedChecksums(response))
            {
                var checksum = HashGenerator.ComputeHashFromStream(computedChecksum.HashAlgorithmName, destinationStream);
                if (computedChecksum.Value != checksum)
                {
                    throw new ChecksumVerificationException(computedChecksum.HashAlgorithmName, computedChecksum.Value, checksum);
                }
            }
        }

        private static IEnumerable<ComputedChecksum> GetSavedChecksums(GetObjectResponse response)
        {
            return response.Metadata
                .Keys
                // ReSharper disable once ArrangeStaticMemberQualifier
                .Where(_ => _.EndsWith(AwsInteractionBase.MetadataKeyChecksumSuffix))
                .Select(key => new ComputedChecksum(ExtractHashAlgorithmNameFromMetadataKey(key), response.Metadata[key]));
        }

        /// <summary>
        /// Extract the hash algorithm name (e.g. MD5) from the metadata key.
        /// </summary>
        /// <param name="metadataKey">The metadata key containing the hash algorithm name.</param>
        /// <returns>The appropriate HashAlgorithmName.</returns>
        private static HashAlgorithmName ExtractHashAlgorithmNameFromMetadataKey(string metadataKey)
        {
            // ReSharper disable once ArrangeStaticMemberQualifier
            if (!metadataKey.EndsWith(AwsInteractionBase.MetadataKeyChecksumSuffix))
            {
                throw new ArgumentException("Metadata key containing a checksum must end with with the suffix '-checksum'.", nameof(metadataKey));
            }

            var sanitizedMetadataKey = SanitizeUserDefinedMetadataKey(metadataKey);
            var hashName = sanitizedMetadataKey
                .Substring(0, sanitizedMetadataKey.Length - MetadataKeyChecksumSuffix.Length)
                .ToUpperInvariant();

            return new HashAlgorithmName(hashName);
        }
    }
}
