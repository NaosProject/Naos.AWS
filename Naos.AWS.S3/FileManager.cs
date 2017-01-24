// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileManager.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to manage files in Amazon S3.
    /// </summary>
    public class FileManager : IManageFiles
    {
        private readonly IListFiles fileSearcher;
        private readonly IManageFileMetadata fileMedataManager;
        private readonly IUploadFiles fileUploader;
        private readonly IDownloadFiles fileDownloader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="accessKey">Access key with rights to download files in specified buckets.</param>
        /// <param name="secretKey">Secret key with rights to download files in specified buckets.</param>
        public FileManager(string accessKey, string secretKey)
        {
            this.fileSearcher = new FileSearcher(accessKey, secretKey);
            this.fileMedataManager = new FileMetadataManager(accessKey, secretKey);
            this.fileUploader = new FileUploader(accessKey, secretKey);
            this.fileDownloader = new FileDownloader(accessKey, secretKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManager"/> class.
        /// </summary>
        /// <param name="fileUploader">File uploader</param>
        /// <param name="fileDownloader">File downloader.</param>
        /// <param name="fileMetadataManager">File metadata manager.</param>
        /// <param name="fileSearcher">File searcher.</param>
        public FileManager(
            IUploadFiles fileUploader,
            IDownloadFiles fileDownloader,
            IManageFileMetadata fileMetadataManager,
            IListFiles fileSearcher)
        {
            this.fileUploader = fileUploader;
            this.fileDownloader = fileDownloader;
            this.fileMedataManager = fileMetadataManager;
            this.fileSearcher = fileSearcher;
        }

        #region IDownloadFiles

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, string destinationFilePath)
        {
            await this.fileDownloader.DownloadFileAsync(uploadFileResult, destinationFilePath);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(UploadFileResult uploadFileResult, Stream destinationStream)
        {
            await this.fileDownloader.DownloadFileAsync(uploadFileResult, destinationStream);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, string destinationFilePath)
        {
            await this.fileDownloader.DownloadFileAsync(region, bucketName, keyName, destinationFilePath);
        }

        /// <inheritdoc />
        public async Task DownloadFileAsync(string region, string bucketName, string keyName, Stream destinationStream)
        {
            await this.fileDownloader.DownloadFileAsync(region, bucketName, keyName, destinationStream);
        }

        #endregion

        #region IManageFileMetadata

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(UploadFileResult uploadFileResult)
        {
            return await this.fileMedataManager.GetFileMetadataAsync(uploadFileResult);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName)
        {
            return await this.fileMedataManager.GetFileMetadataAsync(region, bucketName, keyName);
        }

        #endregion

        #region IListFiles

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName)
        {
            return await this.fileSearcher.ListFilesAsync(region, bucketName);
        }

        /// <inheritdoc />
        public async Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName, string keyPrefixSearchPattern)
        {
            return await this.fileSearcher.ListFilesAsync(region, bucketName, keyPrefixSearchPattern);
        }

        #endregion

        #region IUploadFiles

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            HashAlgorithmName hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceFilePath,
                hashAlgorithmName,
                userDefinedMetadata);
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
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceStream,
                hashAlgorithmName,
                userDefinedMetadata);
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceFilePath,
                userDefinedMetadata);
        }

        /// <inheritdoc />
        public async Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null)
        {
            return await this.fileUploader.UploadFileAsync(
                region,
                bucketName,
                keyName,
                sourceStream,
                userDefinedMetadata);
        }

        #endregion
    }
}
