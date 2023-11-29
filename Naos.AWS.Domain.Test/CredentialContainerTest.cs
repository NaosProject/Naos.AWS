// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialContainerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.CodeAnalysis.Recipes;
    using OBeautifulCode.CodeGen.ModelObject.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.String.Recipes;
    using Xunit;

    using static System.FormattableString;

    [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
    public static partial class CredentialContainerTest
    {
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = ObcSuppressBecause.CA1505_AvoidUnmaintainableCode_DisagreeWithAssessment)]
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = ObcSuppressBecause.CA1810_InitializeReferenceTypeStaticFieldsInline_FieldsDeclaredInCodeGeneratedPartialTestClass)]
        static CredentialContainerTest()
        {
            StringRepresentationTestScenarios
                .RemoveAllScenarios()
                .AddScenario(StringRepresentationTestScenario<CredentialContainer>.ForceGeneratedTestsToPassAndWriteMyOwnScenario);
        }

        [Fact]
        public static void ToString____Should_not_have_sensitive_information()
        {
            // Arrange
            var secretKey = "ThisSecretShouldNotBeInToString";
            var sessionToken = "ThisSessionTokenShouldNotBeInToString";
            var expirationToken = new DateTime(2010, 10, 10, 10, 10, 10, DateTimeKind.Utc);

            var credContainerKeys = new CredentialContainer
                                 {
                                     AccessKeyId = "access",
                                     SecretAccessKey = secretKey,
                                     CredentialType = CredentialType.Keys,
                                 };

            var credContainerToken = new CredentialContainer
                                     {
                                         AccessKeyId = "access",
                                         SecretAccessKey = secretKey,
                                         SessionToken = sessionToken,
                                         Expiration = expirationToken,
                                         CredentialType = CredentialType.Token,
                                     };

            // Act
            var actualKeys = credContainerKeys.ToString();
            var actualToken = credContainerToken.ToString();

            // Assert
            actualKeys.MustForTest().BeEqualTo("Naos.AWS.Domain.CredentialContainer: CredentialType = Keys, AccessKeyId = access, SecretAccessKey = ***, SessionToken = <null>, Expiration = 01/01/0001 00:00:00.");
            actualToken.MustForTest().BeEqualTo("Naos.AWS.Domain.CredentialContainer: CredentialType = Token, AccessKeyId = access, SecretAccessKey = ***, SessionToken = ***, Expiration = 10/10/2010 10:10:10.");
        }
    }
}