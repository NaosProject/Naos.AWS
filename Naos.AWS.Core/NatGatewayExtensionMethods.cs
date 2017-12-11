// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NatGatewayExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Amazon;

    using Naos.AWS.Domain;

    using static System.FormattableString;

    /// <summary>
    /// Extensions on <see cref="NatGateway" />
    /// </summary>
    public static class NatGatewayExtensionMethods
    {
        /// <summary>
        /// Checks to see if the object exists on the AWS servers.
        /// </summary>
        /// <param name="natGateway">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Whether or not is was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<bool> ExistsOnAwsAsync(this NatGateway natGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(natGateway.Region);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeNatGatewaysRequest() { NatGatewayIds = new[] { natGateway.Id }.ToList() };

                var response = await client.DescribeNatGatewaysAsync(request);
                Validator.ThrowOnBadResult(request, response);
                return response.NatGateways.Any(_ => _.NatGatewayId == natGateway.Id);
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <param name="natGateway">Object to operate on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Current status.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task<NatGatewayState> GetState(this NatGateway natGateway, CredentialContainer credentials = null)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(natGateway.Region);

            Amazon.EC2.Model.NatGateway responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var request = new Amazon.EC2.Model.DescribeNatGatewaysRequest() { NatGatewayIds = new[] { natGateway.Id }.ToList() };

                var response = await client.DescribeNatGatewaysAsync(request);
                Validator.ThrowOnBadResult(request, response);

                // have seen this fail without the second check (no joke!).
                responseObject = response.NatGateways.SingleOrDefault(_ => _.NatGatewayId == natGateway.Id);
            }

            if (responseObject == null)
            {
                throw new ArgumentException(Invariant($"Could not find {nameof(NatGateway)} with ID: {natGateway.Id}"));
            }

            return responseObject.State.ToNaosState();
        }

        /// <summary>
        /// Converts <see cref="NatGatewayState" /> to <see cref="Amazon.EC2.NatGatewayState" />.
        /// </summary>
        /// <param name="state">Input state.</param>
        /// <returns>Converted state.</returns>
        public static Amazon.EC2.NatGatewayState ToAmazonState(this NatGatewayState state)
        {
            switch (state)
            {
                case NatGatewayState.Available:
                    return Amazon.EC2.NatGatewayState.Available;
                case NatGatewayState.Deleted:
                    return Amazon.EC2.NatGatewayState.Deleted;
                case NatGatewayState.Deleting:
                    return Amazon.EC2.NatGatewayState.Deleting;
                case NatGatewayState.Failed:
                    return Amazon.EC2.NatGatewayState.Failed;
                case NatGatewayState.Pending:
                    return Amazon.EC2.NatGatewayState.Pending;
                default:
                    throw new NotSupportedException(Invariant($"Unsupported NatGatewayState: {state}"));
            }
        }

        /// <summary>
        /// Converts <see cref="Amazon.EC2.NatGatewayState" /> to <see cref="NatGatewayState" />.
        /// </summary>
        /// <param name="state">Input state.</param>
        /// <returns>Converted state.</returns>
        public static NatGatewayState ToNaosState(this Amazon.EC2.NatGatewayState state)
        {
            var parsed = Enum.TryParse(state?.Value, true, out NatGatewayState outState);
            if (!parsed)
            {
                throw new NotSupportedException(Invariant($"Unsupported NatGatewayState: {state}"));
            }

            return outState;
        }
    }
}
