// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsManager.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Amazon;
    using Amazon.Route53;
    using Amazon.Route53.Model;
    using Amazon.Runtime;

    using Naos.AWS.Contract;

    /// <inheritdoc />
    public class DnsManager : IManageDns
    {
        private readonly AWSCredentials awsCredentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsManager"/> class.
        /// </summary>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        public DnsManager(CredentialContainer credentials = null)
        {
            this.awsCredentials = CredentialManager.GetAwsCredentials(credentials);
        }

        /// <inheritdoc />
        public void UpsertDnsEntry(string region, string domainZoneHostingId, string domain, ICollection<string> ipAddresses)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);

            var client = AWSClientFactory.CreateAmazonRoute53Client(this.awsCredentials, regionEndpoint);
            var resourceRecordSet = new ResourceRecordSet()
                                        {
                                            Name = domain,
                                            Type = "A",
                                            TTL = 300,
                                            ResourceRecords =
                                                ipAddresses.Select(_ => new ResourceRecord(_))
                                                .ToList(),
                                        };

            var changes = new[] { new Change(new ChangeAction("CREATE"), resourceRecordSet) };
            var changeBatch = new ChangeBatch(changes.ToList());
            var request = new ChangeResourceRecordSetsRequest(domainZoneHostingId, changeBatch);

            var response = client.ChangeResourceRecordSets(request);
            Validator.ThrowOnBadResult(request, response);
        }
    }
}
