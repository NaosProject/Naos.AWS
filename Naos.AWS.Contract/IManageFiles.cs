// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFiles.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface managing files in S3.
    /// </summary>
    public interface IManageFiles
    {
        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceFilePath">File path of the file to upload.</param>
        /// <returns>Task to allow for async await use.</returns>
        Task UploadFileAsync(string region, string bucketName, string keyName, string sourceFilePath);

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
        /// Lists the files in the specified region and bucket.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to list files in.</param>
        /// <returns>Files from the bucket.</returns>
        Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName);

        /// <summary>
        /// Lists the files in the specified region and bucket matching the provided prefix search pattern.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to list files in.</param>
        /// <param name="keyPrefixSearchPattern">Key prefix search pattern to match (do not include '*' at the end, this is implied).</param>
        /// <returns>Files from the bucket.</returns>
        Task<ICollection<CloudFile>> ListFilesAsync(string region, string bucketName, string keyPrefixSearchPattern);
    }
}