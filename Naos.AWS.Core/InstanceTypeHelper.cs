// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceTypeHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Useful methods around calculating instance types.
    /// </summary>
    public static class InstanceTypeHelper
    {
        /// <summary>
        /// Gets the largest instance type of the list of provided types.
        /// </summary>
        /// <param name="instanceTypes">List of instance types to filter to largest.</param>
        /// <returns>The largest instance type of the provided list.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Fine for now.")]
        public static string InferLargestInstanceType(ICollection<string> instanceTypes)
        {
            // there's probably a better algorithm for this, it doesn't run very often and it's what i've got... -Lawson 2015/05/01
            if (instanceTypes == null)
            {
                throw new ArgumentException("Cannot infer the largest instance type from a null list or a list with nulls.");
            }

            if (!instanceTypes.Any())
            {
                throw new ArgumentException("Cannot infer the largest instance type from an empty list.");
            }

            if (instanceTypes.Count == 1)
            {
                return instanceTypes.Single();
            }

            var wrappedInstanceTypes = instanceTypes.Select(_ => new { SearchString = _, InstanceType = _ }).ToList();

            // prefixes in order from smallest to largest
            var prefixes = new[] { "t", "m", "c", "g", "r", "i", "d" };

            // if this isn't one then the later filter will need to be updated!!
            var prefixLength = 1;

            // suffixes in order from smallest to largest
            var suffixes = new[] { "micro", "small", "medium", "large", "xlarge", "2xlarge", "4xlarge", "8xlarge" };

            // post suffix numbers from smallest to largest
            var postPrefixNumbers = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            Func<dynamic, dynamic> nullObjectTrimmer = item => item;
            Func<dynamic, dynamic> prefixTrimmer =
                (item) =>
                new
                {
                    SearchString = item.SearchString.Substring(prefixLength, item.SearchString.Length - prefixLength),
                    InstanceType = item.InstanceType,
                };
            Func<dynamic, dynamic> justAfterDotTrimmer =
                (item) => new { SearchString = item.SearchString.Split('.')[1], InstanceType = item.InstanceType };

            var theMatrix = new[]
                                {
                                    new { Trimmer = nullObjectTrimmer, BucketPrefixes = prefixes, },
                                    new { Trimmer = prefixTrimmer, BucketPrefixes = postPrefixNumbers, },
                                    new { Trimmer = justAfterDotTrimmer, BucketPrefixes = suffixes, },
                                };

#pragma warning disable SA1130 // Use lambda syntax - can't use a lambda for the where clause...
            Func<ICollection<dynamic>, string[], ICollection<dynamic>> filterToHighestBucket = delegate(ICollection<dynamic> input, string[] buckets)
#pragma warning restore SA1130 // Use lambda syntax
            {
                var ret = new List<dynamic>();
                for (var idx = buckets.Length - 1; idx >= 0; --idx)
                {
                    var prefixToCheck = buckets[idx];

                    // this is because its a dynamic...
                    Func<dynamic, bool> filterDelegate = (item) => item.SearchString.ToString().StartsWith(prefixToCheck);

                    ret = input.Where(filterDelegate).ToList();

                    if (ret.Any())
                    {
                        return ret;
                    }
                }

                throw new NotSupportedException("The filter list provided did not contain a value that prefix matched any of the search strings.");
            };

            var theList = wrappedInstanceTypes.Select(_ => (dynamic)_).ToList();
            for (int idx = 0; idx < theMatrix.Length; idx++)
            {
                var item = theMatrix[idx];

                theList = theList.Select(item.Trimmer).ToList();

                var filtered = filterToHighestBucket(theList, item.BucketPrefixes);
                if (filtered.Count == 1)
                {
                    return filtered.Single().InstanceType;
                }
            }

            // since they are all the same final bucket take the random first is fine...
            return theList.First().InstanceType;
        }
    }
}
