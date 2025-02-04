// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDownloadFiles.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
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
        /// <param name="validateChecksumsIfPresent">OPTIONAL value indicating whether to validate checksums if present in the file metadata. DEFAULT and recommended value is true.</param>
        /// <param name="throwIfKeyNotFound">OPTIONAL value indicating whether to throw if the key is not found.  DEFAULT is to throw.</param>
        /// <param name="getTags">OPTIONAL value indicating whether to get the object's tags.  DEFAULT is to skip fetching tags.</param>
        /// <returns>
        /// The result of downloading the file.
        /// </returns>
        Task<DownloadFileResult> DownloadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true,
            bool getTags = false);

        /// <summary>
        /// Downloads file to provided path from the specified region and bucket and key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to find file in.</param>
        /// <param name="keyName">Key name of file to download.</param>
        /// <param name="destinationStream">Stream to download to.</param>
        /// <param name="validateChecksumsIfPresent">OPTIONAL value indicating whether to validate checksums if present in the file metadata. DEFAULT and recommended value is true.</param>
        /// <param name="throwIfKeyNotFound">OPTIONAL value indicating whether to throw if the key is not found.  DEFAULT is to throw.</param>
        /// <param name="getTags">OPTIONAL value indicating whether to get the object's tags.  DEFAULT is to skip fetching tags.</param>
        /// <returns>
        /// The result of downloading the file.
        /// </returns>
        Task<DownloadFileResult> DownloadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream destinationStream,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true,
            bool getTags = false);

        /// <summary>
        /// Downloads file to provided path from the specified region and bucket and key.
        /// </summary>
        /// <param name="uploadFileResult">Upload file result.  Typically obtained from previously uploading a file.</param>
        /// <param name="destinationFilePath">File path to download to.</param>
        /// <param name="validateChecksumsIfPresent">OPTIONAL value indicating whether to validate checksums if present in the file metadata. DEFAULT and recommended value is true.</param>
        /// <param name="throwIfKeyNotFound">OPTIONAL value indicating whether to throw if the key is not found.  DEFAULT is to throw.</param>
        /// <param name="getTags">OPTIONAL value indicating whether to get the object's tags.  DEFAULT is to skip fetching tags.</param>
        /// <returns>
        /// The result of downloading the file.
        /// </returns>
        Task<DownloadFileResult> DownloadFileAsync(
            UploadFileResult uploadFileResult,
            string destinationFilePath,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true,
            bool getTags = false);

        /// <summary>
        /// Downloads file to provided stream from the specified region and bucket and key.
        /// </summary>
        /// <param name="uploadFileResult">Upload file result.  Typically obtained from previously uploading a file.</param>
        /// <param name="destinationStream">Stream to download to.</param>
        /// <param name="validateChecksumsIfPresent">OPTIONAL value indicating whether to validate checksums if present in the file metadata. DEFAULT and recommended value is true.</param>
        /// <param name="throwIfKeyNotFound">OPTIONAL value indicating whether to throw if the key is not found.  DEFAULT is to throw.</param>
        /// <param name="getTags">OPTIONAL value indicating whether to get the object's tags.  DEFAULT is to skip fetching tags.</param>
        /// <returns>
        /// The result of downloading the file.
        /// </returns>
        Task<DownloadFileResult> DownloadFileAsync(
            UploadFileResult uploadFileResult,
            Stream destinationStream,
            bool validateChecksumsIfPresent = true,
            bool throwIfKeyNotFound = true,
            bool getTags = false);
    }
}
