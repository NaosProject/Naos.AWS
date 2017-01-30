// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudFileTest.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using System;

    using Xunit;

    /// <summary>
    /// Test the <see cref="CloudFile"/> class.
    /// </summary>
    public class CloudFileTest : TestBase
    {
        private const string OwnerId = "OwnerId";
        private const string OwnerName = "OwnerName";
        private const int Size = 1;
        private static readonly DateTime LastModified = DateTime.Now;

        /// <summary>
        /// Test constructor when passed invalid input.
        /// </summary>
        [Fact]
        public void Constructor___Should_instantiate_upload_cloud_file___When_passed_valid_input()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CloudFile(Region, BucketName, KeyName, OwnerId, OwnerName, LastModified, Size);
        }

        /// <summary>
        /// Test constructor when passed invalid Region.
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_region()
        {
            Assert.Throws<ArgumentException>(() => new CloudFile(null, BucketName, KeyName, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(null, BucketName, KeyName, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(string.Empty, BucketName, KeyName, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile("   ", BucketName, KeyName, OwnerId, OwnerName, LastModified, Size));
        }

        /// <summary>
        /// Test constructor when passed invalid bucket name.
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_bucket_name()
        {
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, null, KeyName, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, string.Empty, KeyName, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, "   ", KeyName, OwnerId, OwnerName, LastModified, Size));
        }

        /// <summary>
        /// Test constructor when passed invalid key name.
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_key_name()
        {
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, null, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, string.Empty, OwnerId, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, "   ", OwnerId, OwnerName, LastModified, Size));
        }

        /// <summary>
        /// Test constructor when passed invalid owner ID.
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_owner_id()
        {
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, KeyName, null, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, KeyName, string.Empty, OwnerName, LastModified, Size));
            Assert.Throws<ArgumentException>(() => new CloudFile(Region, BucketName, KeyName, "   ", OwnerName, LastModified, Size));
        }

        /// <summary>
        /// Test constructor when passed invalid owner ID.
        /// </summary>
        [Fact]
        public void Constructor___Should_throw_exception___When_passed_invalid_size()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CloudFile(Region, BucketName, KeyName, OwnerId, OwnerName, LastModified, -1));
        }
    }
}
