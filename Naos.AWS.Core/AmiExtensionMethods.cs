﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmiExtensionMethods.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;
    using Amazon.EC2.Model;

    using Naos.AWS.Domain;

    /// <summary>
    /// Operations to be performed on AMI's.
    /// </summary>
    public static class AmiExtensionMethods
    {
        /// <summary>
        /// Gets the AMI id (searches for it on AWS if necessary).
        /// </summary>
        /// <param name="ami">AMI to use to get the id for.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>AMI ID, either in the object or discovered through the search strategy.</returns>
        public static async Task<string> DiscoverIdAsync(this Ami ami, CredentialContainer credentials = null)
        {
            if (ami.SearchStrategy == null)
            {
                return ami.Id;
            }

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(ami.Region);

            var request = new DescribeImagesRequest()
                              {
                                  Filters =
                                      new[]
                                          {
                                              new Filter("name", new[] { ami.SearchStrategy.SearchPattern }.ToList()),
                                          }.ToList(),
                              };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DescribeImagesAsync(request);
                Validator.ThrowOnBadResult(request, response);

                if (!response.Images.Any())
                {
                    throw new ApplicationException(
                        "Did not find an AMI; searched on pattern: " + ami.SearchStrategy.SearchPattern);
                }

                var filteredImages =
                    response.Images.Where(
                        _ => _.ImageOwnerAlias == ami.SearchStrategy.OwnerAlias && _.State == ImageState.Available).ToList();

                if (!filteredImages.Any())
                {
                    throw new ApplicationException(
                        "Found AMI's using search pattern: " + ami.SearchStrategy.SearchPattern
                        + " but could not find any that were 'Available' and had the owner alias: "
                        + ami.SearchStrategy.OwnerAlias);
                }

                if (filteredImages.Count == 1)
                {
                    return filteredImages.Single().ImageId;
                }

                switch (ami.SearchStrategy.MultipleFoundBehavior)
                {
                    case MultipleAmiFoundBehavior.Throw:
                        throw new ApplicationException(
                            "Unsupported multiple results, found " + filteredImages.Count + " images matching "
                            + ami.SearchStrategy.SearchPattern);
                    case MultipleAmiFoundBehavior.FirstSortedDescending:
                        var sorted = filteredImages.OrderByDescending(_ => _.Name);
                        return sorted.First().ImageId;
                    default:
                        throw new ApplicationException(
                            "Unsupported MultipleFoundBehavior: " + ami.SearchStrategy.MultipleFoundBehavior);
                }
            }
        }
    }
}
