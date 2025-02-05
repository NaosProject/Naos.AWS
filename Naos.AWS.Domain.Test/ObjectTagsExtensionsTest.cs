// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectTagsExtensionsTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain.Test
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.IO;
    using OBeautifulCode.Type;
    using Xunit;

    public static class ObjectTagsExtensionsTest
    {
        [Fact]
        public static void GetMalwareScanResultFromGuardDutyScan___Should_throw_ArgumentNullException___When_parameter_objectTags_is_null()
        {
            // Arrange, Act
            var actual = Record.Exception(() => ObjectTagsExtensions.GetMalwareScanResultFromGuardDutyScan(null));

            // Assert
            actual.AsTest().Must().BeOfType<ArgumentNullException>();
            actual.Message.AsTest().Must().ContainString("objectTags");
        }

        [Fact]
        public static void GetMalwareScanResultFromGuardDutyScan___Should_throw_ArgumentException___When_parameter_objectTags_contains_a_null_element()
        {
            // Arrange
            var tags = new[] { A.Dummy<NamedValue<string>>(), null, A.Dummy<NamedValue<string>>() };

            // Act
            var actual = Record.Exception(() => tags.GetMalwareScanResultFromGuardDutyScan());

            // Assert
            actual.AsTest().Must().BeOfType<ArgumentException>();
            actual.Message.AsTest().Must().ContainString("Provided value (name: 'objectTags') contains at least one null element");
        }

        [Fact]
        public static void GetMalwareScanResultFromGuardDutyScan___Should_return_null___When_objectTags_does_not_contain_scan_result_tag_key()
        {
            // Arrange
            var tags = Some.ReadOnlyDummies<NamedValue<string>>();

            // Act
            var actual = tags.GetMalwareScanResultFromGuardDutyScan();

            // Assert
            actual.AsTest().Must().BeNull();
        }

        [Fact]
        public static void GetMalwareScanResultFromGuardDutyScan___Should_return_MalwareScanResult_Unknown___When_objectTags_contain_unexpected_value_for_scan_result_key()
        {
            // Arrange
            var tags = new[] { new NamedValue<string>(ObjectTagsExtensions.GuardDutyMalwareScanStatusTagKey, A.Dummy<string>()) };

            // Act
            var actual = tags.GetMalwareScanResultFromGuardDutyScan();

            // Assert
            actual.AsTest().Must().BeEqualTo((MalwareScanResult?)MalwareScanResult.Unknown);
        }

        [Fact]
        public static void GetMalwareScanResultFromGuardDutyScan___Should_return_expected_MalwareScanResult___When_objectTags_contain_scan_result_key_and_valid_value()
        {
            // Arrange
            var tagsAndExpected = new[]
            {
                new
                {
                    TagValue = "NO_THREATS_FOUND",
                    Expected = MalwareScanResult.NoThreatsFound,
                },
                new
                {
                    TagValue = "THREATS_FOUND",
                    Expected = MalwareScanResult.ThreatsFound,
                },
                new
                {
                    TagValue = "UNSUPPORTED",
                    Expected = MalwareScanResult.Unsupported,
                },
                new
                {
                    TagValue = "ACCESS_DENIED",
                    Expected = MalwareScanResult.AccessDenied,
                },
                new
                {
                    TagValue = "FAILED",
                    Expected = MalwareScanResult.Failed,
                },
            };

            var tags = tagsAndExpected
                .Select(_ => Some.ReadOnlyDummies<NamedValue<string>>()
                    .Concat(
                        new[]
                        {
                            new NamedValue<string>(ObjectTagsExtensions.GuardDutyMalwareScanStatusTagKey, _.TagValue),
                        })
                    .ToList())
                .ToList();

            var expected = tagsAndExpected.Select(_ => (MalwareScanResult?)_.Expected).ToList();

            // Act
            var actual = tags.Select(_ => _.GetMalwareScanResultFromGuardDutyScan()).ToList();

            // Assert
            actual.AsTest().Must().BeEqualTo(expected);
        }
    }
}
