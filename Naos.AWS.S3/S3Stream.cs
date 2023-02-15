// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3Stream.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Naos.AWS.Domain;
    using Naos.CodeAnalysis.Recipes;
    using Naos.Database.Domain;

    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Execution.Recipes;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.String.Recipes;
    using OBeautifulCode.Type;
    using static System.FormattableString;

    /// <summary>
    /// Implementation of <see cref="IStandardStream"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public class S3Stream : StandardStreamBase
    {
        private readonly IManageFiles fileManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3Stream"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="serializerFactory">The serializer factory to get serializers of existing records or to put new ones.</param>
        /// <param name="defaultSerializerRepresentation">The default serializer representation.</param>
        /// <param name="defaultSerializationFormat">The default serialization format.</param>
        /// <param name="resourceLocatorProtocols">Protocol to get appropriate resource locator(s).</param>
        /// <param name="fileManager">Backing interface to interact with S3.</param>
        public S3Stream(
            string name,
            ISerializerFactory serializerFactory,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IResourceLocatorProtocols resourceLocatorProtocols,
            IManageFiles fileManager)
            : base(name, serializerFactory, defaultSerializerRepresentation, defaultSerializationFormat, resourceLocatorProtocols)
        {
            fileManager.MustForArg(nameof(fileManager)).NotBeNull();
            this.fileManager = fileManager;
        }

        /// <inheritdoc />
        public override long Execute(
            StandardGetNextUniqueLongOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            var identifierTypeRepresentation = typeof(string).ToRepresentation();
            var objectTypeRepresentation = typeof(byte[]).ToRepresentation();

            operation.RecordNotFoundStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordNotFoundStrategy)}"))
                     .BeEqualTo(RecordNotFoundStrategy.ReturnDefault);
            operation.RecordFilter.DeprecatedIdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}"))
                     .BeNull();
            operation.RecordFilter.IdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}"))
                     .BeNull();
            operation.RecordFilter.InternalRecordIds.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}"))
                     .BeNull();
            operation.RecordFilter.ObjectTypes.Single().MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}"))
                     .BeEqualTo(objectTypeRepresentation);
            operation.RecordFilter.Tags.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}"))
                     .BeNull();
            operation.RecordFilter.Ids.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                     .NotBeEmptyEnumerable().And().HaveCount(1);

            var id = operation.RecordFilter.Ids.Select(
                                   _ =>
                                   {
                                       _.IdentifierType.RemoveAssemblyVersions()
                                        .MustForArg(
                                             Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                                        .BeEqualTo(identifierTypeRepresentation.RemoveAssemblyVersions());
                                       return _.StringSerializedId;
                                   })
                              .Single();

            var resourceLocator = this.TryGetSingleLocator(operation);

            using (var destinationStream = new MemoryStream())
            {
                Func<Task> downloadFileAsyncFunc = () => this.fileManager
                                                             .DownloadFileAsync(
                                                                  resourceLocator.Region,
                                                                  resourceLocator.BucketName,
                                                                  id,
                                                                  destinationStream);

                downloadFileAsyncFunc.ExecuteSynchronously();
                var resultMetadata = new StreamRecordMetadata(
                    id,
                    this.DefaultSerializerRepresentation,
                    identifierTypeRepresentation.ToWithAndWithoutVersion(),
                    objectTypeRepresentation.ToWithAndWithoutVersion(),
                    new NamedValue<string>[0],
                    DateTime.UtcNow,
                    null);
                var result = new StreamRecord(
                    -1,
                    resultMetadata,
                    new BinaryDescribedSerialization(objectTypeRepresentation, this.DefaultSerializerRepresentation, destinationStream.ToArray()));
                return result;
            }
        }

        /// <inheritdoc />
        public override TryHandleRecordResult Execute(
            StandardTryHandleRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "uploadResult", Justification = "Prefer to see result is coming back.")]
        public override PutRecordResult Execute(
            StandardPutRecordOp operation)
        {
            operation.ExistingRecordStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.ExistingRecordStrategy)}")).BeEqualTo(ExistingRecordStrategy.None, Invariant($"No support for {nameof(ExistingRecordStrategy)}."));
            operation.Payload.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.Payload)}")).BeAssignableToType<BinaryDescribedSerialization>(Invariant($"Only binary payloads supported."));
            var resourceLocator = this.TryGetSingleLocator(operation);

            var userDefinedMetadata = operation
                                     .Metadata
                                     .Tags
                                    ?.Select(_ => new KeyValuePair<string, string>(_.Name, _.Value))
                                     .ToDictionary(k => k.Key, v => v.Value)
                                   ?? new Dictionary<string, string>();

            if (operation.Metadata.ObjectTimestampUtc != null)
            {
                userDefinedMetadata.Add(
                    nameof(operation.Metadata.ObjectTimestampUtc),
                    operation.Metadata.ObjectTimestampUtc.ToStringInvariantPreferred());
            }

            var binaryPayload = (BinaryDescribedSerialization)operation.Payload;
            using (var sourceStream = new MemoryStream(binaryPayload.SerializedPayload))
            {
                Func<Task<UploadFileResult>> uploadFileAsyncFunc = () => this.fileManager.UploadFileAsync(
                                                                       resourceLocator.Region,
                                                                       resourceLocator.BucketName,
                                                                       operation.Metadata.StringSerializedId,
                                                                       sourceStream,
                                                                       new[]
                                                                       {
                                                                           HashAlgorithmName.MD5,
                                                                           HashAlgorithmName.SHA256,
                                                                           HashAlgorithmName.SHA1,
                                                                       },
                                                                       userDefinedMetadata);

                var uploadResult = uploadFileAsyncFunc.ExecuteSynchronously();

                var result = new PutRecordResult(-1);
                return result;
            }
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<long> Execute(
            StandardGetInternalRecordIdsOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyDictionary<long, HandlingStatus> Execute(
            StandardGetHandlingStatusOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyList<StreamRecordHandlingEntry> Execute(
            StandardGetHandlingHistoryOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardUpdateHandlingStatusForRecordOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IReadOnlyCollection<StringSerializedIdentifier> Execute(
            StandardGetDistinctStringSerializedIdsOp operation)
        {
            var identifierType = typeof(string).ToRepresentation();

            operation.RecordFilter.DeprecatedIdTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}"))
                     .BeNull();
            operation.RecordFilter.IdTypes.Single().MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}"))
                     .BeEqualTo(identifierType);
            operation.RecordFilter.InternalRecordIds.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}"))
                     .BeNull();
            operation.RecordFilter.ObjectTypes.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}"))
                     .BeNull();
            operation.RecordFilter.Tags.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}"))
                     .BeNull();
            operation.RecordFilter.Ids.MustForArg(
                          Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}"))
                     .BeNull();

            var resourceLocator = this.TryGetSingleLocator(operation);
            Func<Task<ICollection<CloudFile>>> listFilesAsyncFunc =
                () => this.fileManager.ListFilesAsync(resourceLocator.Region, resourceLocator.BucketName);
            var listedFiles = listFilesAsyncFunc.ExecuteSynchronously();

            var result = listedFiles
                        .Select(_ => new StringSerializedIdentifier(_.KeyName, identifierType))
                        .ToList();

            return result;
        }

        /// <inheritdoc />
        public override string Execute(
            StandardGetLatestStringSerializedObjectOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override CreateStreamResult Execute(
            StandardCreateStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardDeleteStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Execute(
            StandardPruneStreamOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IStreamRepresentation StreamRepresentation => new StreamRepresentation(this.Name);

        private S3ResourceLocator TryGetSingleLocator(
            ISpecifyResourceLocator operation = null)
        {
            var resourceLocator =
                (S3ResourceLocator)(operation?.SpecifiedResourceLocator ?? this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp()).Single());
            return resourceLocator;
        }
    }
}
