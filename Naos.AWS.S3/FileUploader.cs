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
    using OBeautifulCode.IO;
    using Spritely.Redo;
    using static System.FormattableString;

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
            IReadOnlyDictionary<string, string> userDefinedMetadata = null,
            ExistingFileWriteAction existingFileWriteAction = ExistingFileWriteAction.OverwriteFile,
            MediaType? mediaType = null)
        {
            sourceFilePath.AsArg(nameof(sourceFilePath)).Must().NotBeNullNorWhiteSpace();

            var result = await this.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceFilePath,
                null,
                hashAlgorithmNames,
                userDefinedMetadata,
                existingFileWriteAction,
                mediaType);

            return result;
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null,
            ExistingFileWriteAction existingFileWriteAction = ExistingFileWriteAction.OverwriteFile,
            MediaType? mediaType = null)
        {
            sourceStream.AsArg(nameof(sourceStream)).Must().NotBeNull();

            var result = await this.UploadFileAsync(
                region,
                bucketName,
                keyName,
                null,
                sourceStream,
                hashAlgorithmNames,
                userDefinedMetadata,
                existingFileWriteAction,
                mediaType);

            return result;
        }

        private async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            Stream sourceFileStream,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata,
            ExistingFileWriteAction existingFileWriteAction,
            MediaType? mediaType = null)
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

                    if (mediaType != null)
                    {
                        var contentType = ((MediaType)mediaType).ToMimeTypeName();
                        transferUtilityUploadRequest.ContentType = contentType;
                    }

                    switch (existingFileWriteAction)
                    {
                        case ExistingFileWriteAction.OverwriteFile:
                            // no-op
                            break;
                        case ExistingFileWriteAction.Throw:
                        case ExistingFileWriteAction.DoNotOverwriteFile:
                            transferUtilityUploadRequest.IfNoneMatch = "*";
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"This {nameof(ExistingFileWriteAction)} is not supported: {existingFileWriteAction}."));
                    }

                    Dictionary<HashAlgorithmName, ComputedChecksum> computedChecksums;
                    if (sourceFilePath != null)
                    {
                        transferUtilityUploadRequest.FilePath = sourceFilePath;

                        computedChecksums = hashAlgorithmNames
                            .Distinct()
                            .ToDictionary(
                                _ => _,
                                _ => new ComputedChecksum(_, HashGenerator.ComputeHashFromFilePath(_, sourceFilePath)));
                    }
                    else
                    {
                        transferUtilityUploadRequest.AutoCloseStream = false;
                        transferUtilityUploadRequest.AutoResetStreamPosition = true;
                        transferUtilityUploadRequest.InputStream = sourceFileStream;

                        computedChecksums = hashAlgorithmNames
                            .Distinct()
                            .ToDictionary(
                                _ => _,
                                _ => new ComputedChecksum(_, HashGenerator.ComputeHashFromStream(_, sourceFileStream)));
                    }

                    // This enables multi-part uploads.  The S3 SDK determines whether to use multi-part
                    // uploads based on the length of the stream passed in and PRIOR to applying AutoResetStreamPosition.
                    // So here we seek to the beginning of the stream so that the SDK can consider the size of the whole
                    // stream when determining whether to use a multi-part upload.
                    if (sourceFileStream.CanSeek)
                    {
                        sourceFileStream.Seek(0, SeekOrigin.Begin);
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

                    var wasWritten = true;

                    try
                    {
                        // Note: if throwIfKeyExists is true, we will attempt to upload multiple times before
                        // throwing because of this re-try logic.  If we're doing a large multi-part upload, this
                        // could take a while before throwing.
                        await
                            Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                .WithMaxRetries(3)
                                .RunAsync(() => localTransferUtility.UploadAsync(transferUtilityUploadRequest))
                                .Now();
                    }
                    catch (AmazonS3Exception ex) when (ex.ErrorCode == "PreconditionFailed")
                    {
                        if (existingFileWriteAction == ExistingFileWriteAction.Throw)
                        {
                            throw new InvalidOperationException(
                                Invariant($"The specified key ('{keyName}') already exists in the specified bucket ('{bucketName}') within the specified region ('{region}').  See inner exception."), ex);
                        }
                        else
                        {
                            wasWritten = false;
                        }
                    }

                    var result = new UploadFileResult(region, bucketName, keyName, computedChecksums, wasWritten);

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
