// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IListFiles.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface managing upload files.
    /// </summary>
    public interface IListFiles
    {
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
