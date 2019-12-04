// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMetadataManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.S3;

    using Naos.AWS.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using Spritely.Redo;

    /// <summary>
    /// Class to manage file metadata for Amazon S3.
    /// </summary>
    public class FileMetadataManager : AwsInteractionBase, IManageFileMetadata
    {
        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileMetadataManager(string accessKey, string secretKey)
            : this(new CredentialContainer(accessKey, secretKey))
        {
        }

        /// <inheritdoc cref="AwsInteractionBase"/>
        public FileMetadataManager(CredentialContainer credentials)
            : base(credentials)
        {
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(UploadFileResult uploadFileResult, bool shouldSanitizeKeys = true)
        {
            uploadFileResult.AsArg(nameof(uploadFileResult)).Must().NotBeNull();

            return await this.GetFileMetadataAsync(uploadFileResult.Region, uploadFileResult.BucketName, uploadFileResult.KeyName, shouldSanitizeKeys);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName, bool shouldSanitizeKeys = true)
        {
            region.AsArg(nameof(region)).Must().NotBeNullNorWhiteSpace();
            bucketName.AsArg(nameof(bucketName)).Must().NotBeNullNorWhiteSpace();
            keyName.AsArg(nameof(keyName)).Must().NotBeNullNorWhiteSpace();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();
            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                var localClient = client;
                var response = await
                                   Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                       .WithMaxRetries(3)
                                       .RunAsync(() => localClient.GetObjectMetadataAsync(bucketName, keyName))
                                       .Now();

                // ReSharper disable once ArrangeStaticMemberQualifier
                return response.Metadata.Keys
                    .ToDictionary(key => shouldSanitizeKeys ? AwsInteractionBase.SanitizeUserDefinedMetadataKey(key) : key, key => response.Metadata[key]);
            }
        }
    }
}
