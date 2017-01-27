// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileManagerTest.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable ArrangeStaticMemberQualifier
namespace Naos.AWS.S3.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;

    using Xunit;

    /// <summary>
    /// Performs unit tests of the <see cref="FileManager"/> class.
    /// </summary>
    public class FileManagerTest : TestBase
    {
        private const string AccessKey = "AccessKey";
        private const string SecretKey = "SecretKey";
        private const string FilePath = "FilePath";

        private static readonly List<HashAlgorithmName> HashAlgorithmNames = new List<HashAlgorithmName>();

        #region Constructor Tests

        /// <summary>
        /// Test constructor when passed invalid input
        /// </summary>
        [Fact]
        public void Constructor___Should_instantiate_file_manager___When_passed_valid_input()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new FileManager(AccessKey, SecretKey);
        }

        /// <summary>
        /// Test constructor when passed invalid input
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_input()
        {
            Assert.Throws<ArgumentException>(() => new FileManager(null, SecretKey));
            Assert.Throws<ArgumentException>(() => new FileManager(string.Empty, SecretKey));
            Assert.Throws<ArgumentException>(() => new FileManager("   ", SecretKey));

            Assert.Throws<ArgumentException>(() => new FileManager(AccessKey, null));
            Assert.Throws<ArgumentException>(() => new FileManager(AccessKey, string.Empty));
            Assert.Throws<ArgumentException>(() => new FileManager(AccessKey, "   "));
        }

        #endregion

        #region Upload Tests

        /// <summary>
        /// Test upload argument validation with the sourceFilePath string overload.
        /// </summary>
        [Fact]
        public async void Upload___Should_throw_exception___When_passed_invalid_input_to_file_path_overload()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(null, TestBase.BucketName, TestBase.KeyName, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(string.Empty, TestBase.BucketName, TestBase.KeyName, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync("   ", TestBase.BucketName, TestBase.KeyName, FilePath, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, null, TestBase.KeyName, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, string.Empty, TestBase.KeyName, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, "   ", TestBase.KeyName, FilePath, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, null, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, string.Empty, FilePath, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, "   ", FilePath, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, (string)null, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, string.Empty, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, "   ", HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, FilePath, null));
        }

        /// <summary>
        /// Test upload argument validation with the sourceStream Stream overload.
        /// </summary>
        [Fact]
        public async void Upload___Should_throw_exception___When_passed_invalid_input_to_stream_overload()
        {
            var stream = new MemoryStream();
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(null, TestBase.BucketName, TestBase.KeyName, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(string.Empty, TestBase.BucketName, TestBase.KeyName, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync("   ", TestBase.BucketName, TestBase.KeyName, stream, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, null, TestBase.KeyName, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, string.Empty, TestBase.KeyName, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, "   ", TestBase.KeyName, stream, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, null, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, string.Empty, stream, HashAlgorithmNames));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, "   ", stream, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, (Stream)null, HashAlgorithmNames));

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.UploadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, stream, null));
        }

        #endregion

        #region Download Tests

        /// <summary>
        /// Test download argument validation with the destination filePath overload.
        /// </summary>
        [Fact]
        public async void Download___Should_throw_exception___When_passed_invalid_input_to_file_path_overload()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(null, TestBase.BucketName, TestBase.KeyName, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(string.Empty, TestBase.BucketName, TestBase.KeyName, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync("   ", TestBase.BucketName, TestBase.KeyName, FilePath));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, null, TestBase.KeyName, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, string.Empty, TestBase.KeyName, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, "   ", TestBase.KeyName, FilePath));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, null, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, string.Empty, FilePath));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, "   ", FilePath));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, (string)null));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, string.Empty));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, "   "));
        }

        /// <summary>
        /// Test download argument validation with the destination Stream overload.
        /// </summary>
        [Fact]
        public async void Download___Should_throw_exception___When_passed_invalid_input_to_stream_overload()
        {
            var stream = new MemoryStream();
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(null, TestBase.BucketName, TestBase.KeyName, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(string.Empty, TestBase.BucketName, TestBase.KeyName, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync("   ", TestBase.BucketName, TestBase.KeyName, stream));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, null, TestBase.KeyName, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, string.Empty, TestBase.KeyName, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, "   ", TestBase.KeyName, stream));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, null, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, string.Empty, stream));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, "   ", stream));

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.DownloadFileAsync(TestBase.Region, TestBase.BucketName, TestBase.KeyName, (Stream)null));
        }

        /// <summary>
        /// Test download argument validation with the destination filePath overload.
        /// </summary>
        [Fact]
        public async void Download___Should_throw_exception___When_passing_null_upload_file_result_to_file_path_overload()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.DownloadFileAsync(null, FilePath));
        }

        /// <summary>
        /// Test download argument validation with the destination Stream overload.
        /// </summary>
        [Fact]
        public async void Download___Should_throw_exception___When_passing_null_upload_file_result_to_stream_overload()
        {
            var stream = new MemoryStream();
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.DownloadFileAsync(null, stream));
        }

        #endregion

        #region List File Tests

        /// <summary>
        /// Test list files argument validation.
        /// </summary>
        [Fact]
        public async void ListFiles___Should_throw_exception___When_passed_invalid_input()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync(null, TestBase.BucketName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync(string.Empty, TestBase.BucketName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync("   ", TestBase.BucketName));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync(TestBase.Region, null));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync(TestBase.Region, string.Empty));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.ListFilesAsync(TestBase.Region, "   "));
        }

        #endregion

        #region Metadata Tests

        /// <summary>
        /// Test list files argument validation.
        /// </summary>
        [Fact]
        public async void ManageMetadata___Should_throw_exception___When_passed_invalid_input()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(null, TestBase.BucketName, TestBase.KeyName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(string.Empty, TestBase.BucketName, TestBase.KeyName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync("   ", TestBase.BucketName, TestBase.KeyName));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, null, TestBase.KeyName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, string.Empty, TestBase.KeyName));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, "   ", TestBase.KeyName));

            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, TestBase.BucketName, null));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, TestBase.BucketName, string.Empty));
            await Assert.ThrowsAsync<ArgumentException>(() => fileManager.GetFileMetadataAsync(TestBase.Region, TestBase.BucketName, "   "));
        }

        /// <summary>
        /// Test download argument validation with the destination Stream overload.
        /// </summary>
        [Fact]
        public async void ManageMetadata___Should_throw_exception___When_passing_null_upload_file_result()
        {
            var fileManager = new FileManager(AccessKey, SecretKey);

            await Assert.ThrowsAsync<ArgumentNullException>(() => fileManager.GetFileMetadataAsync(null));
        }

        #endregion
    }
}
// ReSharper restore ArrangeStaticMemberQualifier
