// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileManager.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Naos.AWS.Domain;

    /// <summary>
    /// Class to manage files in Amazon S3.
    /// </summary>
    public class FileManager : IManageFiles
    {
        private readonly IListFiles fileLister;
        private readonly IManageFileMetadata fileMetadataManager;
        private readonly IUploadFiles fileUploader;
        private readonly IDownloadFiles fileDownloader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="accessKey">Access key with rights to read and write files in specified buckets.</param>
        /// <param name="secretKey">Secret key with rights to read and write files in specified buckets.</param>
        public FileManager(
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="credentials">Credentials with rights to read and write files in specified buckets.</param>
        public FileManager(
            CredentialContainer credentials)
        {
            this.fileLister = new FileLister(credentials);
            this.fileMetadataManager = new FileMetadataManager(credentials);
            this.fileUploader = new FileUploader(credentials);
            this.fileDownloader = new FileDownloader(credentials);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="fileUploader">File uploader.</param>
        /// <param name="fileDownloader">File downloader.</param>
        /// <param name="fileMetadataManager">File metadata manager.</param>
        /// <param name="fileLister">File searcher.</param>
        public FileManager(
            IUploadFiles fileUploader,
            IDownloadFiles fileDownloader,
            IManageFileMetadata fileMetadataManager,
            IListFiles fileLister)
        {
            this.fileUploader = fileUploader;
            this.fileDownloader = fileDownloader;
            this.fileMetadataManager = fileMetadataManager;
            this.fileLister = fileLister;
        }

        #pragma warning disable SA1124 // Do not use regions - good use of regions here...

        #region IUploadFiles

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceFilePath,
                hashAlgorithmNames,
                userDefinedMetadata);
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
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceStream,
                hashAlgorithmNames,
                userDefinedMetadata);
        }

        #endregion

        #region IDownloadFiles

        /// <inheritdoc />
        public async Task DownloadFileAsync(
            UploadFileResult uploadFileResult,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true)
        {
            await this.fileDownloader.DownloadFileAsync(
                uploadFileResult,
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
            await this.fileDownloader.DownloadFileAsync(
                uploadFileResult,
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
            await this.fileDownloader.DownloadFileAsync(
                region,
                bucketName,
                keyName,
                destinationFilePath,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);
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
            await this.fileDownloader.DownloadFileAsync(
                region,
                bucketName,
                keyName,
                destinationStream,
                validateChecksumsIfPresent,
                throwIfKeyNotFound);
        }

        #endregion

        #region IListFiles

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(
            string region,
            string bucketName)
        {
            return await this.fileLister.ListFilesAsync(region, bucketName);
        }

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(
            string region,
            string bucketName,
            string keyPrefixSearchPattern)
        {
            return await this.fileLister.ListFilesAsync(region, bucketName, keyPrefixSearchPattern);
        }

        #endregion

        #region IManageFileMetadata

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(
            UploadFileResult uploadFileResult,
            bool shouldSanitizeKeys = true)
        {
            return await this.fileMetadataManager.GetFileMetadataAsync(uploadFileResult, shouldSanitizeKeys);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(
            string region,
            string bucketName,
            string keyName,
            bool shouldSanitizeKeys = true)
        {
            return await this.fileMetadataManager.GetFileMetadataAsync(region, bucketName, keyName, shouldSanitizeKeys);
        }

        #endregion

        #pragma warning restore SA1124 // Do not use regions
    }
}
