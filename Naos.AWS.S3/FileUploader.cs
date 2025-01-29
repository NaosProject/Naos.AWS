// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploader.cs" company="Naos Project">
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
    using Amazon.S3.Transfer;
    using Naos.AWS.Domain;
    using Naos.Recipes.Cryptography.Hashing;
    using OBeautifulCode.Assertion.Recipes;
    using Spritely.Redo;

    /// <summary>
    /// Class to upload files to Amazon S3.
    /// </summary>
    public class FileUploader : AwsInteractionBase, IUploadFiles
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileUploader(string accessKey, string secretKey)
            : this(new CredentialContainer { AccessKeyId = accessKey, SecretAccessKey = secretKey })
        {
        }

        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileUploader(CredentialContainer credentials)
            : base(credentials)
        {
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceFilePath.AsArg(nameof(sourceFilePath)).Must().NotBeNullNorWhiteSpace();

            var result = await this.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceFilePath,
                null,
                hashAlgorithmNames,
                userDefinedMetadata);

            return result;
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceStream.AsArg(nameof(sourceStream)).Must().NotBeNull();

            var result = await this.UploadFileAsync(
                region,
                bucketName,
                keyName,
                null,
                sourceStream,
                hashAlgorithmNames,
                userDefinedMetadata);

            return result;
        }

        private async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            Stream sourceFileStream,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();
            hashAlgorithmNames.AsArg(nameof(hashAlgorithmNames)).Must().NotBeNull();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();
            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                using (var transferUtility = new TransferUtility(client))
                {
                    var transferUtilityUploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                    };

                    Dictionary<HashAlgorithmName, ComputedChecksum> computedChecksums;
                    if (sourceFilePath != null)
                    {
                        transferUtilityUploadRequest.FilePath = sourceFilePath;

                        computedChecksums = hashAlgorithmNames
                            .Distinct()
                            .ToDictionary(_ => _, _ => new ComputedChecksum(_, HashGenerator.ComputeHashFromFilePath(_, sourceFilePath)));
                    }
                    else
                    {
                        transferUtilityUploadRequest.AutoCloseStream = false;
                        transferUtilityUploadRequest.AutoResetStreamPosition = true;
                        transferUtilityUploadRequest.InputStream = sourceFileStream;

                        computedChecksums = hashAlgorithmNames
                            .Distinct()
                            .ToDictionary(_ => _, _ => new ComputedChecksum(_, HashGenerator.ComputeHashFromStream(_, sourceFileStream)));
                    }

                    // If there is an MD5 hash passed in then add to ContentMD5 in order to have
                    // S3 perform a checksum verification before persisting the file.
                    if (computedChecksums.ContainsKey(HashAlgorithmName.MD5))
                    {
                        transferUtilityUploadRequest.Headers.ContentMD5 = EncodingHelper.ConvertHexStringToBase64(computedChecksums[HashAlgorithmName.MD5].Value);
                    }

                    foreach (var computedChecksum in computedChecksums)
                    {
                        var checksumMetadataKey = CreateChecksumMetadataKey(computedChecksum.Key);

                        transferUtilityUploadRequest.Metadata.Add(checksumMetadataKey, computedChecksum.Value.Value);
                    }

                    if (userDefinedMetadata != null)
                    {
                        foreach (var keyValuePair in userDefinedMetadata)
                        {
                            transferUtilityUploadRequest.Metadata.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }

                    var localTransferUtility = transferUtility;
                    await
                        Using.LinearBackOff(TimeSpan.FromSeconds(5))
                            .WithMaxRetries(3)
                            .RunAsync(() => localTransferUtility.UploadAsync(transferUtilityUploadRequest))
                            .Now();

                    var result = new UploadFileResult(region, bucketName, keyName, computedChecksums);

                    return result;
                }
            }
        }

        private static string CreateChecksumMetadataKey(
            HashAlgorithmName hashAlgorithmName)
        {
            // Actual Checksum metadata key in S3 will be 'x-amz-meta-{HashAlgorithmName}-checksum' since
            // Amazon will prepend 'x-amz-meta-' to the metadata key if not already present.
            var result = hashAlgorithmName + MetadataKeyChecksumSuffix;

            return result;
        }
    }
}
