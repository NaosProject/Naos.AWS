// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUploadFiles.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using OBeautifulCode.IO;

    /// <summary>
    /// Interface managing upload files.
    /// </summary>
    public interface IUploadFiles
    {
        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key; existing items will be overwritten.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceFilePath">File path of the file to upload.</param>
        /// <param name="hashAlgorithmNames">Collection of hash algorithm names to use for checksum verification when downloading (if using MD5 then the checksum verification will be done by S3 on file upload).</param>
        /// <param name="userDefinedMetadata">OPTIONAL metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <param name="existingFileWriteAction">OPTIONAL value indicating the action to take in the presence of an already existing file.  DEFAULT is to overwrite the file.</param>
        /// <param name="mediaType">OPTIONAL standard MIME type describing the format of the contents.  DEFAULT is unknown.</param>
        /// <returns>
        /// Results of the upload file including a potentially calculated hash.
        /// </returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            string sourceFilePath,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null,
            ExistingFileWriteAction existingFileWriteAction = ExistingFileWriteAction.OverwriteFile,
            MediaType? mediaType = null);

        /// <summary>
        /// Uploads the provided file to the specified region and bucket named using the provided key; existing items will be overwritten.
        /// </summary>
        /// <param name="region">Region bucket is in.</param>
        /// <param name="bucketName">Bucket name to upload to.</param>
        /// <param name="keyName">Key name to use for file.</param>
        /// <param name="sourceStream">Stream to upload.</param>
        /// <param name="hashAlgorithmNames">Collection of hash algorithm names to use for checksum verification when downloading (if using MD5 then the checksum verification will be done by S3 on file upload).</param>
        /// <param name="userDefinedMetadata">OPTIONAL metadata to store along with the object.  Note that currently S3 limits the size of keys and data to 2 KB. Keys should not include Amazon specific prefix.</param>
        /// <param name="existingFileWriteAction">OPTIONAL value indicating the action to take in the presence of an already existing file.  DEFAULT is to overwrite the file.</param>
        /// <param name="mediaType">OPTIONAL standard MIME type describing the format of the contents.  DEFAULT is unknown.</param>
        /// <returns>
        /// Results of the upload file including a potentially calculated hash.
        /// </returns>
        Task<UploadFileResult> UploadFileAsync(
            string region,
            string bucketName,
            string keyName,
            Stream sourceStream,
            IReadOnlyCollection<HashAlgorithmName> hashAlgorithmNames,
            IReadOnlyDictionary<string, string> userDefinedMetadata = null,
            ExistingFileWriteAction existingFileWriteAction = ExistingFileWriteAction.OverwriteFile,
            MediaType? mediaType = null);
    }
}
