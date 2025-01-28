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
    using OBeautifulCode.Type.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Implementation of <see cref="IStandardStream"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = NaosSuppressBecause.CA1711_IdentifiersShouldNotHaveIncorrectSuffix_TypeNameAddedAsSuffixForTestsWhereTypeIsPrimaryConcern)]
    public class S3Stream : StandardStreamBase
    {
        private const string MetadataKeyTagNamePrefix = "tag-";
        private static readonly TypeRepresentation IdentifierTypeRepresentation = typeof(string).ToRepresentation();
        private static readonly TypeRepresentation ObjectTypeRepresentation = typeof(byte[]).ToRepresentation();
        private static readonly TypeRepresentation IdentifierTypeRepresentationWithoutAssemblyVersions = IdentifierTypeRepresentation.RemoveAssemblyVersions();
        private static readonly TypeRepresentation ObjectTypeRepresentationWithoutAssemblyVersions = ObjectTypeRepresentation.RemoveAssemblyVersions();
        private static readonly TypeRepresentationWithAndWithoutVersion IdentifierTypeRepresentationWithAndWithoutVersion = IdentifierTypeRepresentation.ToWithAndWithoutVersion();
        private static readonly TypeRepresentationWithAndWithoutVersion ObjectTypeRepresentationWithAndWithoutVersion = ObjectTypeRepresentation.ToWithAndWithoutVersion();

        private readonly IManageFiles fileManager;
        private readonly IStringSerializeAndDeserialize tagSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3Stream"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="serializerFactory">The serializer factory to get serializers of existing records or to put new ones.</param>
        /// <param name="defaultSerializerRepresentation">The default serializer representation.</param>
        /// <param name="defaultSerializationFormat">The default serialization format.</param>
        /// <param name="resourceLocatorProtocols">Protocol to get appropriate resource locator(s).</param>
        /// <param name="fileManager">Backing interface to interact with S3.</param>
        /// <param name="tagSerializer">The serializer to use when serializing tags.</param>
        public S3Stream(
            string name,
            ISerializerFactory serializerFactory,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IResourceLocatorProtocols resourceLocatorProtocols,
            IManageFiles fileManager,
            IStringSerializeAndDeserialize tagSerializer)
            : base(name, serializerFactory, defaultSerializerRepresentation, defaultSerializationFormat, resourceLocatorProtocols)
        {
            fileManager.MustForArg(nameof(fileManager)).NotBeNull();
            fileManager.MustForArg(nameof(tagSerializer)).NotBeNull();

            this.fileManager = fileManager;
            this.tagSerializer = tagSerializer;
        }

        /// <inheritdoc />
        public override IStreamRepresentation StreamRepresentation => new StreamRepresentation(this.Name);

        /// <inheritdoc />
        public override long Execute(
            StandardGetNextUniqueLongOp operation)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
        public override StreamRecord Execute(
            StandardGetLatestRecordOp operation)
        {
            // All of the GetAll... operations and the GetLatestObjects (plural, not singular) use internal record ids.
            // So an S3 stream cannot be used for those operations.
            operation.RecordFilter.InternalRecordIds.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.InternalRecordIds)}."));
            operation.RecordFilter.Ids.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}")).NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().HaveCount(1, Invariant($"{nameof(RecordFilter.Ids)} must be specified, only one is supported, and it must be a {typeof(string).ToStringReadable()}."));
            operation.RecordFilter.Ids.Single().IdentifierType.RemoveAssemblyVersions().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}.Single().{nameof(StringSerializedIdentifier.IdentifierType)}")).BeEqualTo(IdentifierTypeRepresentationWithoutAssemblyVersions, Invariant($"{nameof(RecordFilter.Ids)} must be specified, only one is supported, and it must be a {typeof(string).ToStringReadable()}."));

            // Looking at Naos.Database, none of the operations that filter on id (e.g. GetLatestObjectByIdOp<TId, TObject>) also filter on id type.
            // The only ones that filter on id type are GetLatestObjectOp<TObject> and GetLatestRecordOp<TObject> and those don't specify id which is required.
            operation.RecordFilter.IdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.IdTypes)}."));

            operation.RecordFilter.ObjectTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}")).NotBeNullNorEmptyEnumerableNorContainAnyNulls().And().HaveCount(1, Invariant($"{nameof(RecordFilter.ObjectTypes)} must be specified, only one is supported, and it must be a {typeof(byte[]).ToStringReadable()}."));
            operation.RecordFilter.ObjectTypes.Single().RemoveAssemblyVersions().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}.Single()")).BeEqualTo(ObjectTypeRepresentationWithoutAssemblyVersions, Invariant($"{nameof(RecordFilter.ObjectTypes)} must be specified, only one is supported, and it must be a {typeof(byte[]).ToStringReadable()}."));
            operation.RecordFilter.VersionMatchStrategy.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.VersionMatchStrategy)}")).BeEqualTo(VersionMatchStrategy.Any, Invariant($"The only supported {nameof(RecordFilter.VersionMatchStrategy)} is {nameof(VersionMatchStrategy.Any)}."));
            operation.RecordFilter.Tags.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.Tags)} to filter on."));
            operation.RecordFilter.DeprecatedIdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}")).BeNull(Invariant($"No support for {nameof(RecordFilter.DeprecatedIdTypes)}."));

            var id = operation.RecordFilter.Ids.Single().StringSerializedId;

            var resourceLocator = this.TryGetSingleLocator(operation);

            bool throwIfKeyNotFound;
            switch (operation.RecordNotFoundStrategy)
            {
                case RecordNotFoundStrategy.ReturnDefault:
                    throwIfKeyNotFound = false;
                    break;
                case RecordNotFoundStrategy.Throw:
                    throwIfKeyNotFound = true;
                    break;
                default:
                    throw new NotSupportedException(Invariant($"This {nameof(RecordNotFoundStrategy)} is not supported: {operation.RecordNotFoundStrategy}."));
            }

            using (var destinationStream = new MemoryStream())
            {
                // ReSharper disable once AccessToDisposedClosure
                Func<Task<DownloadFileResult>> downloadFileAsyncFunc = () => this.fileManager
                    .DownloadFileAsync(
                        resourceLocator.Region,
                        resourceLocator.BucketName,
                        id,
                        destinationStream,
                        validateChecksumsIfPresent: true,
                        throwIfKeyNotFound);

                var downloadFileResult = downloadFileAsyncFunc.ExecuteSynchronously();

                StreamRecord result;

                if (downloadFileResult.KeyExists)
                {
                    var tags = this.GetTags(downloadFileResult.UserDefinedMetadata);

                    // ReSharper disable once PossibleInvalidOperationException - LastModifiedUtc guaranteed to not be null when KeyExists == true
                    var resultMetadata = new StreamRecordMetadata(
                        id,
                        this.DefaultSerializerRepresentation,
                        IdentifierTypeRepresentationWithAndWithoutVersion,
                        ObjectTypeRepresentationWithAndWithoutVersion,
                        tags,
                        (DateTime)downloadFileResult.LastModifiedUtc,
                        null); // the object is a byte array so it doesn't implement IHaveTimestampUtc

                    DescribedSerializationBase describedSerializationBase;

                    switch (operation.StreamRecordItemsToInclude)
                    {
                        case StreamRecordItemsToInclude.MetadataOnly:
                            describedSerializationBase = new NullDescribedSerialization(
                                resultMetadata.TypeRepresentationOfObject.WithVersion,
                                resultMetadata.SerializerRepresentation);
                            break;
                        case StreamRecordItemsToInclude.MetadataAndPayload:
                            describedSerializationBase = new BinaryDescribedSerialization(
                                resultMetadata.TypeRepresentationOfObject.WithVersion,
                                resultMetadata.SerializerRepresentation,
                                destinationStream.ToArray());
                            break;
                        default:
                            throw new NotSupportedException(Invariant($"This {nameof(StreamRecordItemsToInclude)} is not supported: {operation.StreamRecordItemsToInclude}."));
                    }

                    result = new StreamRecord(-1, resultMetadata, describedSerializationBase);
                }
                else
                {
                    // Key doesn't exist and we haven't thrown per throwIfKeyNotFound, so RecordNotFoundStrategy is ReturnDefault
                    result = null;
                }

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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = NaosSuppressBecause.CA1506_AvoidExcessiveClassCoupling_DisagreeWithAssessment)]
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

            operation.RecordFilter.DeprecatedIdTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.DeprecatedIdTypes)}")).BeNull();
            operation.RecordFilter.IdTypes.Single().MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.IdTypes)}")).BeEqualTo(identifierType);
            operation.RecordFilter.InternalRecordIds.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.InternalRecordIds)}")).BeNull();
            operation.RecordFilter.ObjectTypes.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.ObjectTypes)}")).BeNull();
            operation.RecordFilter.Tags.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Tags)}")).BeNull();
            operation.RecordFilter.Ids.MustForArg(Invariant($"{nameof(operation)}.{nameof(operation.RecordFilter)}.{nameof(operation.RecordFilter.Ids)}")).BeNull();

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

        private S3ResourceLocator TryGetSingleLocator(
            ISpecifyResourceLocator operation = null)
        {
            var resourceLocator = operation?.SpecifiedResourceLocator ??
                                  this.ResourceLocatorProtocols.Execute(new GetAllResourceLocatorsOp()).Single();

            var result = (S3ResourceLocator)resourceLocator;

            return result;
        }

        private IReadOnlyCollection<NamedValue<string>> GetTags(
            IReadOnlyDictionary<string, string> userDefinedMetadata)
        {
            var result = new List<NamedValue<string>>();

            foreach (var metadataKey in userDefinedMetadata.Keys)
            {
                if (metadataKey.StartsWith(MetadataKeyTagNamePrefix))
                {
                    var metadataValue = userDefinedMetadata[metadataKey];

                    var tag = this.tagSerializer.Deserialize<NamedValue<string>>(metadataValue);

                    result.Add(tag);
                }
            }

            return result;
        }
    }
}
