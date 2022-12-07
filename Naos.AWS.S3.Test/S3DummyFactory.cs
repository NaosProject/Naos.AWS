// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3DummyFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using global::FakeItEasy;
    using global::Naos.AWS.Domain;
    using global::OBeautifulCode.AutoFakeItEasy;
    using global::OBeautifulCode.Math.Recipes;
    using global::System;
    using Naos.Database.Domain;
    using OBeautifulCode.Serialization;

    /// <summary>
    /// Hand rolled dummy factory for this situation.
    /// </summary>
    internal class S3DummyFactory : IDummyFactory
    {
        public S3DummyFactory()
        {
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new S3ResourceLocator
                      {
                          BucketName = A.Dummy<string>(),
                          Credentials = new CredentialContainer(A.Dummy<string>(), A.Dummy<string>()),
                      });
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => (IResourceLocator)new S3ResourceLocator
                      {
                          BucketName = A.Dummy<string>(),
                          Credentials = new CredentialContainer(A.Dummy<string>(), A.Dummy<string>()),
                      });

            AutoFixtureBackedDummyFactory.UseRandomInterfaceImplementationForDummy<IResourceLocator>();

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new S3StreamConfig(
                    A.Dummy<string>(),
                    A.Dummy<StreamAccessKinds>().ThatIsNot(StreamAccessKinds.None),
                    A.Dummy<SerializerRepresentation>(),
                    A.Dummy<SerializationFormat>(),
                    new[]
                    {
                        A.Dummy<S3ResourceLocator>(),
                    }));
        }

        /// <inheritdoc />
        public Priority Priority => new FakeItEasy.Priority(1);

        /// <inheritdoc />
        public bool CanCreate(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        public object Create(Type type)
        {
            return null;
        }
    }
}