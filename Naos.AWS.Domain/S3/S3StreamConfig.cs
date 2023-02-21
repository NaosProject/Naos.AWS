// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3StreamConfig.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using Naos.Database.Domain;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// S3 Implementation of <see cref="IStreamConfig" />.
    /// </summary>
    public partial class S3StreamConfig : StreamConfigBase
    {
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
            : base(name, accessKinds, defaultSerializerRepresentation, defaultSerializationFormat, allLocators)
        {
        }
    }
}
