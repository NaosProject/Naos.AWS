// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectTagsExtensions.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.IO;
    using OBeautifulCode.Type;

    /// <summary>
    /// Extension methods on object tags.
    /// </summary>
    public static class ObjectTagsExtensions
    {
        /// <summary>
        /// The GuardDuty malware scan status tag key.
        /// </summary>
        public const string GuardDutyMalwareScanStatusTagKey = "GuardDutyMalwareScanStatus";

        /// <summary>
        /// Gets the <see cref="MalwareScanResult"/> a GuardDuty scan.
        /// </summary>
        /// <param name="objectTags">The tags.</param>
        /// <returns>
        /// The <see cref="MalwareScanResult"/> from a GuardDuty scan, or null
        /// GuardDuty is not enabled, the object wasn't yet scanned, or GuardDuty is
        /// configured to not tag after scanning.
        /// </returns>
        public static MalwareScanResult? GetMalwareScanResultFromGuardDutyScan(
            this IReadOnlyCollection<NamedValue<string>> objectTags)
        {
            objectTags.MustForArg(nameof(objectTags)).NotBeNull().And().NotContainAnyNullElements();

            // Note this will throw if the same tag appears multiple times.
            var guardDutyMalwareScanStatusTag = objectTags.SingleOrDefault(_ => _.Name == GuardDutyMalwareScanStatusTagKey);

            MalwareScanResult? result;

            if (guardDutyMalwareScanStatusTag == null)
            {
                result = null;
            }
            else
            {
                switch (guardDutyMalwareScanStatusTag.Value)
                {
                    case "NO_THREATS_FOUND":
                        result = MalwareScanResult.NoThreatsFound;
                        break;
                    case "THREATS_FOUND":
                        result = MalwareScanResult.ThreatsFound;
                        break;
                    case "UNSUPPORTED":
                        result = MalwareScanResult.Unsupported;
                        break;
                    case "ACCESS_DENIED":
                        result = MalwareScanResult.AccessDenied;
                        break;
                    case "FAILED":
                        result = MalwareScanResult.Failed;
                        break;
                    default:
                        result = MalwareScanResult.Unknown;
                        break;
                }
            }

            return result;
        }
    }
}
