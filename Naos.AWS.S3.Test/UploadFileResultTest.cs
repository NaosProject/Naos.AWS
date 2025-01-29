// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadFileResultTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using FakeItEasy;
    using Naos.AWS.Domain;
    using Xunit;

    /// <summary>
    /// Test the <see cref="UploadFileResult"/> class.
    /// </summary>
    public class UploadFileResultTest : TestBase
    {
        private static readonly IReadOnlyDictionary<HashAlgorithmName, ComputedChecksum> HashAlgorithms = new Dictionary<HashAlgorithmName, ComputedChecksum>
        {
            { HashAlgorithmName.MD5, new ComputedChecksum(HashAlgorithmName.MD5, "abc123") },
        };

        /// <summary>
        /// Test constructor when passed invalid input.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Keeping instance for base class.")]
        [Fact]
        public void Constructor___Should_instantiate_upload_file_result___When_passed_valid_input()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadFileResult(Region, BucketName, KeyName, HashAlgorithms, A.Dummy<bool>());
        }

        /// <summary>
        /// Test constructor when passed invalid Region.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Keeping instance for base class.")]
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_region()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(null, BucketName, KeyName, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult(string.Empty, BucketName, KeyName, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult("   ", BucketName, KeyName, HashAlgorithms, A.Dummy<bool>()));
        }

        /// <summary>
        /// Test constructor when passed invalid bucket name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Keeping instance for base class.")]
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_bucket_name()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(Region, null, KeyName, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult(Region, string.Empty, KeyName, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult(Region, "   ", KeyName, HashAlgorithms, A.Dummy<bool>()));
        }

        /// <summary>
        /// Test constructor when passed invalid key name.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Keeping instance for base class.")]
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_key_name()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(Region, BucketName, null, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult(Region, BucketName, string.Empty, HashAlgorithms, A.Dummy<bool>()));
            Assert.Throws<ArgumentException>(() => new UploadFileResult(Region, BucketName, "   ", HashAlgorithms, A.Dummy<bool>()));
        }

        /// <summary>
        /// Test constructor when passed null computed checksum dictionary.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Keeping instance for base class.")]
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_null_computed_checksum_dictionary()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(Region, BucketName, KeyName, null, A.Dummy<bool>()));
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(Region, BucketName, KeyName, null, A.Dummy<bool>()));
            Assert.Throws<ArgumentNullException>(() => new UploadFileResult(Region, BucketName, KeyName, null, A.Dummy<bool>()));
        }
    }
}
