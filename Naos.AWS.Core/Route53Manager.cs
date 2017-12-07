// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route53Manager.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.Route53;
    using Amazon.Route53.Model;
    using Amazon.Runtime;

    using Naos.AWS.Domain;

    /// <inheritdoc />
    public class Route53Manager : IManageRoute53
    {
        private readonly AWSCredentials awsCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="Route53Manager"/> class.
        /// </summary>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public Route53Manager(CredentialContainer credentials = null)
        {
            this.awsCredentials = CredentialManager.GetAwsCredentials(credentials);
        }

        /// <inheritdoc />
        public async Task UpsertDnsEntryAsync(string region, string domainZoneHostingId, Route53EntryType type, string domain, ICollection<string> ipAddresses)
        {
            var action = "UPSERT";

            await this.InternalRunActionAsync(region, domainZoneHostingId, domain, ipAddresses, type.ToString(), action);
        }

        private async Task InternalRunActionAsync(
            string region,
            string domainZoneHostingId,
            string domain,
            ICollection<string> ipAddresses,
            string type,
            string action)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            var client = new AmazonRoute53Client(this.awsCredentials, regionEndpoint);
            var resourceRecordSet = new ResourceRecordSet()
                                        {
                                            Name = domain,
                                            Type = type,
                                            TTL = 300,
                                            ResourceRecords =
                                                ipAddresses.Select(_ => new ResourceRecord(_)).ToList(),
                                        };

            var changes = new[] { new Change(new ChangeAction(action), resourceRecordSet) };
            var changeBatch = new ChangeBatch(changes.ToList());
            var request = new ChangeResourceRecordSetsRequest(domainZoneHostingId, changeBatch);

            var response = await client.ChangeResourceRecordSetsAsync(request);
            Validator.ThrowOnBadResult(request, response);
        }

        /// <inheritdoc />
        public async Task<ICollection<Route53Entry>> GetDnsEntriesAsync(string region, string domainZoneHostingId)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            var client = new AmazonRoute53Client(this.awsCredentials, regionEndpoint);

            var request = new ListResourceRecordSetsRequest(domainZoneHostingId);

            var response = await client.ListResourceRecordSetsAsync(request);
            Validator.ThrowOnBadResult(request, response);

            var ret =
                response.ResourceRecordSets.Select(
                    set =>
                    new Route53Entry
                        {
                            Domain = set.Name,
                            IpAddresses = set.ResourceRecords.Select(_ => _.Value).ToList(),
                            Type = (Route53EntryType)Enum.Parse(typeof(Route53EntryType), set.Type.Value),
                        }).ToList();

            return ret;
        }
    }
}
