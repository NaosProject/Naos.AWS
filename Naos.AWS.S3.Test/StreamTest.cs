// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using System;
    using FakeItEasy;
    using Naos.AWS.Domain;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Compression.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;
    using Xunit;

    /// <summary>
    /// Performs unit tests of the <see cref="S3Stream"/> class.
    /// </summary>
    public static class StreamTest
    {
        /// <summary>
        /// Test the S3 stream.
        /// </summary>
        [Fact(Skip = "Needs valid credentials. Debug Only.")]
        public static void S3StreamTest()
        {
            var credsJson = @"JsonSerializedCredentialContainer";
            var credentials = new ObcJsonSerializer().Deserialize<CredentialContainer>(credsJson);
            var fileManager = new FileManager(credentials);
            var stream = new S3Stream(
                "Testing",
                new ObcSimplifyingSerializerFactory(new BsonSerializerFactory(CompressorFactory.Instance)),
                new SerializerRepresentation(SerializationKind.Bson),
                SerializationFormat.Binary,
                new SingleResourceLocatorProtocols(
                    new S3ResourceLocator
                    {
                        Region = "us-east-1",
                        BucketName = "stream-testing",
                    }),
                fileManager);

            var newFileName = FormattableString.Invariant($"StreamTest_{Guid.NewGuid()}.bin");
            var newFileContents = A.Dummy<byte[]>();

            var idsBefore = stream.GetDistinctIds<string>();
            idsBefore.MustForTest().NotContainElement(newFileName);

            stream.PutWithId(newFileName, newFileContents);
            
            var ids = stream.GetDistinctIds<string>();
            ids.MustForTest().ContainElement(newFileName);

            var fetchedFileContents = stream.GetLatestObjectById<string, byte[]>(newFileName);
            fetchedFileContents.MustForTest().BeEqualTo(newFileContents);
        }
    }
}
