// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDownloadFiles.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface managing upload files.
    /// </summary>
    public interface IDownloadFiles
    {
        /// <summary>
        /// Downloads file to provided path from the specified region and bucket and key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to find file in.</param>
        /// <param name="keyName">Key name of file to download.</param>
        /// <param name="destinationFilePath">File path to download to.</param>
        /// <returns>Task to allow for async await use.</returns>
        Task DownloadFileAsync(string region, string bucketName, string keyName, string destinationFilePath);

        /// <summary>
        /// Downloads file to provided path from the specified region and bucket and key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to find file in.</param>
        /// <param name="keyName">Key name of file to download.</param>
        /// <param name="destinationStream">Stream to download to.</param>
        /// <returns>Task to allow for async await use.</returns>
        Task DownloadFileAsync(string region, string bucketName, string keyName, Stream destinationStream);

        /// <summary>
        /// Downloads file to provided path from the specified region and bucket and key.
        /// </summary>
        /// <param name="uploadFileResult">Upload file result.  Typically obtained from previously uploading a file.</param>
        /// <param name="destinationFilePath">File path to download to.</param>
        /// <returns>Task to allow for async await use.</returns>
        Task DownloadFileAsync(UploadFileResult uploadFileResult, string destinationFilePath);

        /// <summary>
        /// Downloads file to provided stream from the specified region and bucket and key.
        /// </summary>
        /// <param name="uploadFileResult">Upload file result.  Typically obtained from previously uploading a file.</param>
        /// <param name="destinationStream">Stream to download to.</param>
        /// <returns>Task to allow for async await use.</returns>
        Task DownloadFileAsync(UploadFileResult uploadFileResult, Stream destinationStream);
    }
}
