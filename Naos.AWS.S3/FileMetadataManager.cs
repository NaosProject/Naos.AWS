// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMetadataManager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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

    using Its.Log.Instrumentation;

    using Naos.AWS.Domain;

    using Spritely.Recipes;
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
            uploadFileResult.Named(nameof(uploadFileResult)).Must().NotBeNull().OrThrow();

            return await this.GetFileMetadataAsync(uploadFileResult.Region, uploadFileResult.BucketName, uploadFileResult.KeyName, shouldSanitizeKeys);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName, bool shouldSanitizeKeys = true)
        {
            region.Named(nameof(region)).Must().NotBeWhiteSpace().OrThrow();
            bucketName.Named(nameof(bucketName)).Must().NotBeWhiteSpace().OrThrow();
            keyName.Named(nameof(keyName)).Must().NotBeWhiteSpace().OrThrow();

            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            var awsCredentials = this.Credentials.ToAwsCredentials();
            using (var client = new AmazonS3Client(awsCredentials, regionEndpoint))
            {
                var localClient = client;
                var response = await
                                   Using.LinearBackOff(TimeSpan.FromSeconds(5))
                                       .WithMaxRetries(3)
                                       .WithReporter(_ => Log.Write(new LogEntry("Retrying Get File Metadata due to error.", _)))
                                       .RunAsync(() => localClient.GetObjectMetadataAsync(bucketName, keyName))
                                       .Now();

                // ReSharper disable once ArrangeStaticMemberQualifier
                return response.Metadata.Keys
                    .ToDictionary(key => shouldSanitizeKeys ? AwsInteractionBase.SanitizeUserDefinedMetadataKey(key) : key, key => response.Metadata[key]);
            }
        }
    }
}
