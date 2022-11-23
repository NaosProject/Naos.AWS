// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfigurationTypes.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;

    /// <summary>
    /// Serialization configuration type definition.
    /// </summary>
    internal static class SerializationConfigurationTypes
    {
        public static BsonSerializationConfigurationType BsonSerializationConfigurationType => typeof(NullBsonSerializationConfiguration).ToBsonSerializationConfigurationType();

        public static JsonSerializationConfigurationType JsonSerializationConfigurationType => typeof(NullJsonSerializationConfiguration).ToJsonSerializationConfigurationType();
    }
}
