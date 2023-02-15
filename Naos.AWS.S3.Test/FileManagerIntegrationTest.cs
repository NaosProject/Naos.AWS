// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileManagerIntegrationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using Amazon.S3;
    using Naos.AWS.Domain;
    using Naos.Recipes.Cryptography.Hashing;
    using OBeautifulCode.Serialization.Json;
    using Xunit;
    using static System.FormattableString;

    /// <summary>
    /// Performs integration tests of the <see cref="FileManager"/> class.   These are integration tests so files *are* written to S3.
    /// <remarks>
    /// Pre-requisites:
    /// o An Amazon AWS account configured for S3 access
    /// o network connection capable of accessing Amazon AWS
    /// o The following AWS information: AccessKey, SecretKey, BucketName, Region
    /// <para />
    /// STEPS TO RUN INTEGRATION TESTS
    /// 1. Copy the file config\awsConfiguration.json.example to config\awsConfiguration.json.  DO NOT DO THIS IN VISUAL STUDIO!  We don't want to add this file to the project file (it won't be committed due to .gitignore).
    /// 2. Edit config\awsConfiguration.json and, in the appropriate place, add the AWS information: AccessKey, SecretKey, BucketName, Region
    /// 3. Set IntegrationTestSkipReason = null so that test will not be skipped.  Do not commit this IntegrationTestSkipReason = null change.
    /// </remarks>
    /// </summary>
    public class FileManagerIntegrationTest
    {
        /// <summary>
        /// Reason for skipping local integration tests.  If this constant is set to null then integration tests will be run (not skipped).
        /// This gives an easy way to enable/disable all local tests in one place.
        /// </summary>
        private const string IntegrationTestSkipReason = "Only run integration tests locally.";

        /// <summary>
        /// Test uploading a file using actual S3 storage.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "hashnames", Justification = "Spelling/name is correct.")]
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Upload___Should_upload_file_with_no_errors_and_return_upload_result_with_no_checksums___When_no_hashnames_specified()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName>();
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("Upload___Should_upload_file_with_no_errors_and_return_upload_result_with_no_checksums___When_no_hashnames_specified");

            // Act
            var uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms);

            // Assert
            Assert.NotNull(uploadFileResult);
            Assert.Equal(awsConfiguration.BucketName, uploadFileResult.BucketName);
            Assert.Equal(keyName, uploadFileResult.KeyName);
            Assert.Equal(awsConfiguration.Region, uploadFileResult.Region);
            Assert.Equal(0, uploadFileResult.Checksums.Count);
        }

        /// <summary>
        /// Test uploading a file using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Upload___Should_upload_file_with_no_errors_and_return_upload_result___When_using_file_path()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("Upload___Should_upload_file_with_no_errors_and_return_upload_result___When_using_file_path");

            // Act
            var uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms);

            // Assert
            Assert.NotNull(uploadFileResult);
            Assert.Equal(awsConfiguration.BucketName, uploadFileResult.BucketName);
            Assert.Equal(keyName, uploadFileResult.KeyName);
            Assert.Equal(awsConfiguration.Region, uploadFileResult.Region);
            Assert.Equal(2, uploadFileResult.Checksums.Count);

            var checkSum = uploadFileResult.Checksums[HashAlgorithmName.MD5];
            Assert.Equal(HashAlgorithmName.MD5, checkSum.HashAlgorithmName);
            Assert.Equal("6213670071ee759dc3e03d0ec76134fa", checkSum.Value);

            checkSum = uploadFileResult.Checksums[HashAlgorithmName.SHA256];
            Assert.Equal(HashAlgorithmName.SHA256, checkSum.HashAlgorithmName);
            Assert.Equal("3182733115b54d2df1c7c474d59b10bdca43f0d40661bef591753dae6d5f3af6", checkSum.Value);
        }

        /// <summary>
        /// Test uploading a file using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Upload___Should_upload_file_with_no_errors_and_return_upload_result___When_using_stream()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("Upload___Should_upload_file_with_no_errors_and_return_upload_result___When_using_stream");

            // Act
            UploadFileResult uploadFileResult;
            using (var fileStream = File.OpenRead(uploadFilePath))
            {
                uploadFileResult = await fileManager.UploadFileAsync(
                    awsConfiguration.Region,
                    awsConfiguration.BucketName,
                    keyName,
                    fileStream,
                    hashAlgorithms);
            }

            // Assert
            Assert.NotNull(uploadFileResult);
            Assert.Equal(awsConfiguration.BucketName, uploadFileResult.BucketName);
            Assert.Equal(keyName, uploadFileResult.KeyName);
            Assert.Equal(awsConfiguration.Region, uploadFileResult.Region);
            Assert.Equal(2, uploadFileResult.Checksums.Count);

            var checkSum = uploadFileResult.Checksums[HashAlgorithmName.MD5];
            Assert.Equal(HashAlgorithmName.MD5, checkSum.HashAlgorithmName);
            Assert.Equal("6213670071ee759dc3e03d0ec76134fa", checkSum.Value);

            checkSum = uploadFileResult.Checksums[HashAlgorithmName.SHA256];
            Assert.Equal(HashAlgorithmName.SHA256, checkSum.HashAlgorithmName);
            Assert.Equal("3182733115b54d2df1c7c474d59b10bdca43f0d40661bef591753dae6d5f3af6", checkSum.Value);
        }

        /// <summary>
        /// Test persisting submissions using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Download___Should_upload_file_and_download_to_file_path_with_no_errors___When_object_exists()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("Download___Should_upload_file_and_download_to_file_path_with_no_errors___When_object_exists");

            UploadFileResult uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms);

            var downloadFile = Path.GetTempFileName();

            // Act
            await fileManager.DownloadFileAsync(uploadFileResult, downloadFile);

            // Assert
            Assert.True(File.Exists(downloadFile));

            string md5Hash = HashGenerator.ComputeHashFromFilePath(HashAlgorithmName.MD5, downloadFile);
            Assert.Equal(uploadFileResult.Checksums[HashAlgorithmName.MD5].Value, md5Hash);

            string sha256Hash = HashGenerator.ComputeHashFromFilePath(HashAlgorithmName.SHA256, downloadFile);
            Assert.Equal(uploadFileResult.Checksums[HashAlgorithmName.SHA256].Value, sha256Hash);
        }

        /// <summary>
        /// Test persisting submissions using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Download___Should_upload_file_and_download_to_file_stream_with_no_errors___When_object_exists()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("Download___Should_upload_file_and_download_to_file_stream_with_no_errors___When_object_exists");

            UploadFileResult uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms);

            var downloadFile = Path.GetTempFileName();

            // Act
            using (var fileStream = File.Open(downloadFile, FileMode.Create, FileAccess.ReadWrite))
            {
                await fileManager.DownloadFileAsync(uploadFileResult.Region, uploadFileResult.BucketName, uploadFileResult.KeyName, fileStream);
            }

            // Assert
            Assert.True(File.Exists(downloadFile));

            string md5Hash = HashGenerator.ComputeHashFromFilePath(HashAlgorithmName.MD5, downloadFile);
            Assert.Equal(uploadFileResult.Checksums[HashAlgorithmName.MD5].Value, md5Hash);

            string sha256Hash = HashGenerator.ComputeHashFromFilePath(HashAlgorithmName.SHA256, downloadFile);
            Assert.Equal(uploadFileResult.Checksums[HashAlgorithmName.SHA256].Value, sha256Hash);
        }

        /// <summary>
        /// Test persisting submissions using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void Download___Should_throw_exception___When_key_does_not_exist()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyNameThatDoesNotExist = Guid.NewGuid().ToString("N");
            var downloadFile = Path.GetTempFileName();

            // Act
            Exception exception = null;
            using (var fileStream = File.Open(downloadFile, FileMode.Create, FileAccess.ReadWrite))
            {
                // ReSharper disable once AccessToDisposedClosure
                var exceptionAsync = Record.ExceptionAsync(() => fileManager.DownloadFileAsync(awsConfiguration.Region, awsConfiguration.BucketName, keyNameThatDoesNotExist, fileStream));
                if (exceptionAsync != null)
                {
                    exception = await exceptionAsync;
                }
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<AmazonS3Exception>(exception);
        }

        /// <summary>
        /// Test persisting submissions using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void ListFiles___Should_return_cloud_files___When_searching_by_key_prefix()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("ListFiles___Should_return_cloud_files___When_searching_by_key_prefix");

            UploadFileResult uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms);

            // Act
            var cloudFiles = await fileManager.ListFilesAsync(uploadFileResult.Region, uploadFileResult.BucketName, "IntegrationTesting/FileManager/");

            // Assert
            Assert.NotNull(cloudFiles);
            Assert.True(cloudFiles.Count > 0);
            Assert.NotEmpty(cloudFiles.Where(_ => _.KeyName == keyName));
        }

        /// <summary>
        /// Test persisting submissions using actual S3 storage.
        /// </summary>
        [Fact(Skip = IntegrationTestSkipReason)]
        public async void ManageMetadata___Should_return_return_custom_metadata___When_custom_metadata_is_added_to_upload()
        {
            // Arrange
            var awsConfiguration = GetAwsConfiguration();
            string uploadFilePath = GetFilePath(@"TestFiles\QuickBrownFox.txt");
            var hashAlgorithms = new List<HashAlgorithmName> { HashAlgorithmName.MD5 };
            var fileManager = new FileManager(awsConfiguration.AccessKey, awsConfiguration.SecretKey);
            string keyName = CreateKeyName("ManageMetadata___Should_return_return_custom_metadata___When_custom_metadata_is_added_to_upload");
            var customMetadata = new CustomMetadata("First item", 1234, true);
            var serializedMetadata = new ObcJsonSerializer<CompactFormatJsonSerializationConfiguration<NullJsonSerializationConfiguration>>().SerializeToString(customMetadata);
            var customMetaDictionary = new Dictionary<string, string>
            {
                { typeof(CustomMetadata).Name, serializedMetadata },
                { "abc", "def" },
            };

            UploadFileResult uploadFileResult = await fileManager.UploadFileAsync(
                awsConfiguration.Region,
                awsConfiguration.BucketName,
                keyName,
                uploadFilePath,
                hashAlgorithms,
                customMetaDictionary);

            // Act
            var metadata = await fileManager.GetFileMetadataAsync(uploadFileResult);

            // Assert
            Assert.NotEmpty(metadata);

            // Ensure keys are sanitized by stripping of user defined metadata prefix.
            Assert.DoesNotContain(metadata.Keys, _ => _.Contains("x-amz-meta-"));

            // Should be 3 metadata keys -- one for the hash, one for the custom data, and one string value.
            Assert.Equal(3, metadata.Count);

            var customMetadataFromAws = new ObcJsonSerializer<CompactFormatJsonSerializationConfiguration<NullJsonSerializationConfiguration>>().Deserialize<CustomMetadata>(metadata[typeof(CustomMetadata).Name.ToLowerInvariant()]);
            Assert.Equal(customMetadata.ThisIsMetadata, customMetadataFromAws.ThisIsMetadata);
            Assert.Equal(customMetadata.MetadataItem1, customMetadataFromAws.MetadataItem1);
            Assert.Equal(customMetadata.MetadataItem2, customMetadataFromAws.MetadataItem2);
            Assert.Equal("def", metadata["abc"]);
        }

        /// <summary>
        /// Creates an S3 key name for integration tests.
        /// </summary>
        /// <param name="fileName">The name of the upload file.</param>
        /// <returns>Key name.</returns>
        private static string CreateKeyName(string fileName)
        {
            var dateFolder = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return Invariant($"IntegrationTesting/FileManager/{dateFolder}/{fileName}");
        }

        /// <summary>
        /// Get a rooted file path by combining the CodeBase path and a relative file path.
        /// </summary>
        /// <param name="relativeFilePath">Relative path to the file.</param>
        /// <returns>Rooted file path.</returns>
        private static string GetFilePath(string relativeFilePath)
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return Path.Combine(dirPath, relativeFilePath);
        }

        /// <summary>
        /// Retrieve AWS configuration from configuration file.
        /// </summary>
        /// <returns>AwsConfiguration object deserialized from configuration file.</returns>
        private static AwsConfiguration GetAwsConfiguration()
        {
            var configFile = GetFilePath(@"config\awsConfiguration.json");
            var serializedAwsConfiguration = File.ReadAllText(configFile);
            var awsConfiguration = new ObcJsonSerializer<CompactFormatJsonSerializationConfiguration<NullJsonSerializationConfiguration>>().Deserialize<AwsConfiguration>(serializedAwsConfiguration);

            return awsConfiguration;
        }
    }
}
