﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.177.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain.Test
{
    using global::System.CodeDom.Compiler;
    using global::System.Diagnostics.CodeAnalysis;

    using global::FakeItEasy;

    using global::OBeautifulCode.Assertion.Recipes;

    using global::Xunit;

    [ExcludeFromCodeCoverage]
    [GeneratedCode("OBeautifulCode.CodeGen.ModelObject", "1.0.177.0")]
    public static partial class AWSDummyFactoryTest
    {
        [Fact]
        public static void AWSDummyFactory___Should_derive_from_DefaultAWSDummyFactory___When_reflecting()
        {
            // Arrange
            var dummyFactoryType = typeof(AWSDummyFactory);

            var defaultDummyFactoryType = typeof(DefaultAWSDummyFactory);

            // Act, Assert
            defaultDummyFactoryType.GetInterface(nameof(IDummyFactory)).AsTest().Must().NotBeNull();

            dummyFactoryType.BaseType.AsTest().Must().BeEqualTo(defaultDummyFactoryType);
        }
    }
}