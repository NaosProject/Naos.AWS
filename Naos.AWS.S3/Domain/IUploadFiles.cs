﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUploadFiles.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface managing upload files.
    /// </summary>
    public interface IUploadFiles
    {
        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceFilePath">File path of the file to upload.</param>
        /// <param name="hashAlgorithmName">Optional hash algorithm name to use for calculating checksum for validating file download (and if using MD5 checksum of file to be used by S3 to validate upload.</param>
        /// <param name="userDefinedMetadata">Optional metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <returns>Results of the upload file including a potential calculated hash.</returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            HashAlgorithmName hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null);

        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceStream">Stream to upload.</param>
        /// <param name="hashAlgorithmName">Optional hash algorithm name to use for calculating checksum for validating file download (and if using MD5 checksum of file to be used by S3 to validate upload.</param>
        /// <param name="userDefinedMetadata">Optional metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <returns>Results of the upload file including a potential calculated hash.</returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            HashAlgorithmName hashAlgorithmName,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null);

        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceFilePath">File path of the file to upload.</param>
        /// <param name="userDefinedMetadata">Optional metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <returns>Results of the upload file including a potential calculated hash.</returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null);

        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceStream">Stream to upload.</param>
        /// <param name="userDefinedMetadata">Optional metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <returns>Results of the upload file including a potential calculated hash.</returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null);
    }
}
