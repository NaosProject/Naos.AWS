// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Route53Manager.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Amazon;
    using Amazon.Route53;
    using Amazon.Route53.Model;
    using Amazon.Runtime;

    using Naos.AWS.Contract;

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
        public void UpsertDnsEntry(string region, string domainZoneHostingId, Route53EntryType type, string domain, ICollection<string> ipAddresses)
        {
            var action = "UPSERT";

            this.InternalRunAction(region, domainZoneHostingId, domain, ipAddresses, type.ToString(), action);
        }

        private void InternalRunAction(
            string region,
            string domainZoneHostingId,
            string domain,
            ICollection<string> ipAddresses,
            string type,
            string action)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            var client = AWSClientFactory.CreateAmazonRoute53Client(this.awsCredentials, regionEndpoint);
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

            var response = client.ChangeResourceRecordSets(request);
            Validator.ThrowOnBadResult(request, response);
        }

        /// <inheritdoc />
        public ICollection<Route53Entry> GetDnsEntries(string region, string domainZoneHostingId)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            var client = AWSClientFactory.CreateAmazonRoute53Client(this.awsCredentials, regionEndpoint);

            var request = new ListResourceRecordSetsRequest(domainZoneHostingId);

            var response = client.ListResourceRecordSets(request);
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
