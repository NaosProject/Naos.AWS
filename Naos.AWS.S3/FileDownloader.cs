// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDownloader.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
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
    using Naos.AWS.Domain;
    using Naos.Recipes.Cryptography.Hashing;
    using OBeautifulCode.Assertion.Recipes;
    using Spritely.Redo;

    /// <summary>
    /// Class to download files from Amazon S3.
    /// </summary>
    public class FileDownloader : AwsInteractionBase, IDownloadFiles
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileDownloader(
            string accessKey,
            string secretKey)
            : this(
                new CredentialContainer
                {
                    AccessKeyId = accessKey,
                    SecretAccessKey = secretKey,
                })
        {
        }

        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileDownloader(
            CredentialContainer credentials)
            : base(credentials)
        {
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(
            UploadFileResult uploadFileResult,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            uploadFileResult.AsArg(nameof(uploadFileResult)).Must().NotBeNull();

            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationFilePath,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(
            UploadFileResult uploadFileResult,
            Stream destinationStream,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            uploadFileResult.AsArg(nameof(uploadFileResult)).Must().NotBeNull();

            await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationStream,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            destinationFilePath.AsArg(nameof(destinationFilePath)).Must().NotBeNullNorWhiteSpace();

            using (var fileStream = File.Create(destinationFilePath))
            {
                await this.DownloadFileAsync(region, bucketName, keyName, fileStream, throwIfKeyNotFound);
            }
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream destinationStream,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();
            destinationStream.AsArg(nameof(destinationStream)).Must().NotBeNull();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();

            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                };

                var localClient = client;
                try
                {
                    using (var response = await
                               Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                   .WithMaxRetries(3)
                                   .RunAsync(() => localClient.GetObjectAsync(request))
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
                catch (AmazonS3Exception ex) when ((!throwIfKeyNotFound) && (ex.ErrorCode == "NoSuchKey"))
                {
                    // no-op
                }
            }
        }

        private static void VerifyChecksums(
            Stream destinationStream,
            GetObjectResponse response)
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

        private static IEnumerable<ComputedChecksum> GetSavedChecksums(
            GetObjectResponse response)
        {
            var result = response.Metadata
                .Keys
                .Where(_ => _.EndsWith(MetadataKeyChecksumSuffix, StringComparison.OrdinalIgnoreCase))
                .Select(key => new ComputedChecksum(ExtractHashAlgorithmNameFromMetadataKey(key), response.Metadata[key]));

            return result;
        }

        private static HashAlgorithmName ExtractHashAlgorithmNameFromMetadataKey(
            string metadataKey)
        {
            if (!metadataKey.EndsWith(MetadataKeyChecksumSuffix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Metadata key containing a checksum must end with with the suffix '-checksum'.", nameof(metadataKey));
            }

            var sanitizedMetadataKey = SanitizeUserDefinedMetadataKey(metadataKey);
            var hashName = sanitizedMetadataKey
                .Substring(0, sanitizedMetadataKey.Length - MetadataKeyChecksumSuffix.Length)
                .ToUpperInvariant();

            var result = new HashAlgorithmName(hashName);

            return result;
        }
    }
}
