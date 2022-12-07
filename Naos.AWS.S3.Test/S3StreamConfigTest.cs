// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3StreamConfigTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;

    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Serialization.Recipes;
    using Xunit;

    using static System.FormattableString;

    [SuppressMessage(
        "Microsoft.Maintainability",
        "CA1505:AvoidUnmaintainableCode",
        Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class S3StreamConfigTest
    {
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1505:AvoidUnmaintainableCode",
            Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static S3StreamConfigTest()
        {
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", Justification = "Test code.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Test code.")]
        public static void Deserialize___Should_roundtrip_object___When_serializing_to_and_deserializing_from_string_using_ObcJsonSerializer()
        {
            // Arrange
            var expected = A.Dummy<S3StreamConfig>();

            var serializationConfigurationType = SerializationConfigurationTypes
                                                .JsonSerializationConfigurationType.ConcreteSerializationConfigurationDerivativeType;

            var serializationFormats = new[]
                                       {
                                           SerializationFormat.String,
                                       };

            var appDomainScenarios = AppDomainScenarios.RoundtripInCurrentAppDomain
                                   | AppDomainScenarios.SerializeInCurrentAppDomainAndDeserializeInNewAppDomain;

            // Act, Assert
            expected.RoundtripSerializeViaJsonWithBeEqualToAssertion(serializationConfigurationType, serializationFormats, appDomainScenarios);
        }
    }
}