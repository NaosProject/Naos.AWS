// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFileMetadata.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
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
        /// <returns>Dictionary containing collection of metadata.</returns>
        Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(string region, string bucketName, string keyName);

        /// <summary>
        /// Gets file metadata.
        /// </summary>
        /// <param name="uploadFileResult">Result of previously uploading a file.</param>
        /// <returns>Dictionary containing collection of metadata.</returns>
        Task<IReadOnlyDictionary<string, string>> GetFileMetadataAsync(UploadFileResult uploadFileResult);
    }
}
