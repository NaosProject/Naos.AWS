// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFileMetadata.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Manage S3 File metadata.
    /// </summary>
    public interface IManageFileMetadata
    {
        /// <summary>
        /// Gets file metadata.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to list files in.</param>
        /// <param name="keyName">Key name of file to download.</param>
        /// <param name="shouldSanitizeKeys">Should amazon specific key prefixes be removed from metadata keys.</param>
        /// <returns>Dictionary containing collection of metadata.</returns>
        Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName, bool shouldSanitizeKeys = true);

        /// <summary>
        /// Gets file metadata.
        /// </summary>
        /// <param name="uploadFileResult">Result of previously uploading a file.</param>
        /// <param name="shouldSanitizeKeys">Should amazon specific key prefixes be removed from metadata keys.</param>
        /// <returns>Dictionary containing collection of metadata.</returns>
        Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(UploadFileResult uploadFileResult, bool shouldSanitizeKeys = true);
    }
}
