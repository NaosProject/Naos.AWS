﻿// --------------------------------------------------------------------------------------------------------------------
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
    using static System.FormattableString;

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
        public async Task<DownloadFileResult> DownloadFileAsync(
            UploadFileResult uploadFileResult,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            uploadFileResult.AsArg(nameof(uploadFileResult)).Must().NotBeNull();

            var result = await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationFilePath,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);

            return result;
        }

        /// <inheritdoc />
        public async Task<DownloadFileResult> DownloadFileAsync(
            UploadFileResult uploadFileResult,
            Stream destinationStream,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            uploadFileResult.AsArg(nameof(uploadFileResult)).Must().NotBeNull();

            var result = await this.DownloadFileAsync(
                uploadFileResult.Region,
                uploadFileResult.BucketName,
                uploadFileResult.KeyName,
                destinationStream,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);

            return result;
        }

        /// <inheritdoc />
        public async Task<DownloadFileResult> DownloadFileAsync(
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
                var result = await this.DownloadFileAsync(region, bucketName, keyName, fileStream, throwIfKeyNotFound);

                return result;
            }
        }

        /// <inheritdoc />
        public async Task<DownloadFileResult> DownloadFileAsync(
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

            DownloadFileResult result;

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
                    // Note: if throwIfKeyNotFound is true, we will attempt to download multiple times before
                    // throwing because of this re-try logic.
                    using (var response = await
                               Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                   .WithMaxRetries(3)
                                   .RunAsync(() => localClient.GetObjectAsync(request))
                                   .Now())
                    {
                        using (var responseStream = response.ResponseStream)
                        {
                            await responseStream.CopyToAsync(destinationStream);

                            IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> validatedChecksums = null;

                            if (validateChecksumsIfPresent)
                            {
                                validatedChecksums = VerifyChecksums(destinationStream, response);
                            }

                            var userDefinedMetadata = GetUserDefinedMetadata(response);

                            result = new DownloadFileResult(
                                region,
                                bucketName,
                                keyName,
                                new DownloadFileDetails(
                                    response.ContentLength,
                                    response.LastModified,
                                    userDefinedMetadata,
                                    validatedChecksums));
                        }
                    }
                }
                catch (AmazonS3Exception ex) when (ex.ErrorCode == "NoSuchKey")
                {
                    if (throwIfKeyNotFound)
                    {
                        throw new InvalidOperationException(
                            Invariant($"The specified key ('{keyName}') does not exist in the specified bucket ('{bucketName}') within the specified region ('{region}').  See inner exception."), ex);
                    }
                    else
                    {
                        result = new DownloadFileResult(region, bucketName, keyName, details: null);
                    }
                }
            }

            return result;
        }

        private static IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> VerifyChecksums(
            Stream destinationStream,
            GetObjectResponse response)
        {
            var result = new Dictionary<HashAlgorithmName, ComputedChecksum>();

            foreach (var computedChecksum in GetSavedChecksums(response))
            {
                var checksum = HashGenerator.ComputeHashFromStream(computedChecksum.HashAlgorithmName, destinationStream);

                if (computedChecksum.Value != checksum)
                {
                    throw new ChecksumVerificationException(computedChecksum.HashAlgorithmName, computedChecksum.Value, checksum);
                }

                result.Add(computedChecksum.HashAlgorithmName, computedChecksum);
            }

            return result;
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

        private static IReadOnlyDictionary<string, string> GetUserDefinedMetadata(
            GetObjectResponse response)
        {
            var result = response.Metadata
                .Keys
                .Where(_ => !_.EndsWith(MetadataKeyChecksumSuffix, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(SanitizeUserDefinedMetadataKey, key => response.Metadata[key]);

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
