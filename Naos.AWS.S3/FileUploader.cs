// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUploader.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Transfer;

    using Its.Log.Instrumentation;
    using Spritely.Recipes;
    using Spritely.Redo;

    /// <summary>
    /// Class to upload files to Amazon S3.
    /// </summary>
    public class FileUploader : S3FileBase, IUploadFiles
    {
        /// <inheritdoc cref="S3FileBase"/>
        public FileUploader(string accessKey, string secretKey)
            : base(accessKey, secretKey)
        {
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            HashAlgorithmName hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceFilePath.Named(nameof(sourceFilePath)).Must().NotBeEmptyString().OrThrow();

            return await this.UploadFileAsync(region, bucketName, keyName, sourceFilePath, null, hashAlgorithmName, userDefinedMetadata);
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            HashAlgorithmName hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceStream.Named(nameof(sourceStream)).Must().NotBeNull().OrThrow();

            return await this.UploadFileAsync(region, bucketName, keyName, null, sourceStream, hashAlgorithmName, userDefinedMetadata);
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceFilePath.Named(nameof(sourceFilePath)).Must().NotBeEmptyString().OrThrow();

            return await this.UploadFileAsync(region, bucketName, keyName, sourceFilePath, null, null, userDefinedMetadata);
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            sourceStream.Named(nameof(sourceStream)).Must().NotBeNull().OrThrow();

            return await this.UploadFileAsync(region, bucketName, keyName, null, sourceStream, null, userDefinedMetadata);
        }

        private async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            Stream sourceFileStream,
            HashAlgorithmName? hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata)
        {
            region.Named(nameof(region)).Must().NotBeNull().OrThrow();
            bucketName.Named(nameof(bucketName)).Must().NotBeEmptyString().OrThrow();
            keyName.Named(nameof(keyName)).Must().NotBeEmptyString();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            using (var client = new AmazonS3Client(this.AccessKey, this.SecretKey, regionEndpoint))
            {
                using (var transferUtility = new TransferUtility(client))
                {
                    var transferUtilityUploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        Key = keyName
                    };

                    if (sourceFilePath != null)
                    {
                        transferUtilityUploadRequest.FilePath = sourceFilePath;
                    }
                    else
                    {
                        transferUtilityUploadRequest.AutoCloseStream = false;
                        transferUtilityUploadRequest.AutoResetStreamPosition = true;
                        transferUtilityUploadRequest.InputStream = sourceFileStream;
                    }

                    AddUserDefinedMetadataToRequest(transferUtilityUploadRequest, userDefinedMetadata);
                    var computedChecksum = ComputeChecksumAndAddToRequest(hashAlgorithmName, transferUtilityUploadRequest, sourceFilePath, sourceFileStream);

                    await
                        Using.LinearBackOff(TimeSpan.FromSeconds(5))
                            .WithMaxRetries(3)
                            .WithReporter(_ => Log.Write(new LogEntry("Retrying Upload File due to error.", _)))
                            // ReSharper disable once AccessToDisposedClosure
                            .Run(() => transferUtility.UploadAsync(transferUtilityUploadRequest))
                            .Now();

                    return new UploadFileResult(region, bucketName, keyName, computedChecksum);
                }
            }
        }

        private static void AddUserDefinedMetadataToRequest(
            TransferUtilityUploadRequest transferUtilityUploadRequest,
            IReadOnlyDictionary<string, string> userDefinedMetadata)
        {
            if (userDefinedMetadata == null)
            {
                return;
            }

            foreach (var key in userDefinedMetadata.Keys)
            {
                transferUtilityUploadRequest.Metadata.Add(key, userDefinedMetadata[key]);
            }
        }

        private static ComputedChecksum ComputeChecksumAndAddToRequest(
            HashAlgorithmName? hashAlgorithmName,
            TransferUtilityUploadRequest transferUtilityUploadRequest,
            string sourceFilePath,
            Stream sourceFileStream)
        {
            // Always compute MD5 because S3 will automatically perform checksum verification
            var md5ComputedChecksum = ComputeChecksum(HashAlgorithmName.MD5, sourceFilePath, sourceFileStream);
            transferUtilityUploadRequest.Headers.ContentMD5 = HashAlgorithmHelper.ConvertHexStringToBase64(md5ComputedChecksum.Value);

            // If caller did not request an algorithm then use MD5
            var computedChecksum = ComputeChecksum(hashAlgorithmName, sourceFilePath, sourceFileStream) ?? md5ComputedChecksum;
            transferUtilityUploadRequest.Metadata.Add(computedChecksum.HashAlgorithmName.Name + S3FileBase.MetadataKeyChecksumSuffix, computedChecksum.Value);

            return computedChecksum;
        }

        private static ComputedChecksum ComputeChecksum(HashAlgorithmName? hashAlgorithmName, string sourceFilePath, Stream sourceFileStream)
        {
            if (hashAlgorithmName == null)
            {
                return null;
            }

            return sourceFilePath != null ?
                new ComputedChecksum(hashAlgorithmName.Value, HashAlgorithmHelper.ComputeHash(hashAlgorithmName.Value, sourceFilePath)) :
                new ComputedChecksum(hashAlgorithmName.Value, HashAlgorithmHelper.ComputeHash(hashAlgorithmName.Value, sourceFileStream));
        }
    }
}
