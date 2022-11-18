// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3StreamConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Database.Domain;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// S3 Implementation of <see cref="IStreamConfig" />.
    /// </summary>
    public class S3StreamConfig : IStreamConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="S3StreamConfig"/> class.
        /// </summary>
        public S3StreamConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="S3StreamConfig"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="accessKinds">The access kinds.</param>
        /// <param name="defaultSerializerRepresentation">The default serializer representation.</param>
        /// <param name="defaultSerializationFormat">The default serialization format.</param>
        /// <param name="allLocators">All locators.</param>
        public S3StreamConfig(
            string name,
            StreamAccessKinds accessKinds,
            SerializerRepresentation defaultSerializerRepresentation,
            SerializationFormat defaultSerializationFormat,
            IReadOnlyCollection<IResourceLocator> allLocators)
        {
            name.MustForArg(nameof(name)).NotBeNullNorWhiteSpace();
            accessKinds.MustForArg(nameof(accessKinds)).NotBeEqualTo(StreamAccessKinds.None);
            defaultSerializerRepresentation.MustForArg(nameof(defaultSerializerRepresentation)).NotBeNull();
            defaultSerializationFormat.MustForArg(nameof(defaultSerializationFormat)).NotBeEqualTo(SerializationFormat.Invalid);
            allLocators.MustForArg(nameof(allLocators)).NotBeNullNorEmptyEnumerableNorContainAnyNulls();
            allLocators.ToList().ForEach(_ => _.MustForArg(nameof(allLocators) + "-item").BeOfType<S3ResourceLocator>());

            this.Name = name;
            this.AccessKinds = accessKinds;
            this.DefaultSerializerRepresentation = defaultSerializerRepresentation;
            this.DefaultSerializationFormat = defaultSerializationFormat;
            this.AllLocators = allLocators;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public StreamAccessKinds AccessKinds { get; set; }

        /// <inheritdoc />
        public SerializerRepresentation DefaultSerializerRepresentation { get; set; }

        /// <inheritdoc />
        public SerializationFormat DefaultSerializationFormat { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IResourceLocator> AllLocators { get; set; }
    }
}
