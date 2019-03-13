// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Destroyer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Naos.AWS.Domain;

    using static System.FormattableString;

    /// <summary>
    /// Remove resources.
    /// </summary>
    public static class Destroyer
    {
        /// <summary>
        /// Create an environment in a region.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="environment">Environment config to use.</param>
        /// <param name="updateCallback">Optional callback to perform on object model update.</param>
        /// <param name="announcer">Optional callback to log announcements.</param>
        /// <param name="timeout">Optional timeout to wait for operations to complete; DEFAULT is ininity.</param>
        /// <returns>Environment config updated with IDs stripped out.</returns>
        public static async Task<ConfigEnvironment> RemoveEnvironment(CredentialContainer credentials, ConfigEnvironment environment, Action<ConfigEnvironment> updateCallback = null, Action<Func<object>> announcer = null, TimeSpan timeout = default(TimeSpan))
        {
            environment.ThrowIfInvalid(false);

            void NullUpdate(ConfigEnvironment configEnvironment)
            {
                /* no-op */
            }

            void NullAnnounce(object announcement)
            {
                /* no-op */
            }

            var localOnUpdate = updateCallback ?? NullUpdate;
            var localAnnouncer = announcer ?? NullAnnounce;

            foreach (var vpc in environment.Vpcs)
            {
                foreach (var natGateway in vpc.NatGateways)
                {
                    if (!string.IsNullOrWhiteSpace(natGateway.NatGatewayId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(NatGateway)} - {natGateway.Name} ({natGateway.NatGatewayId})"));
                        await RemoveNatGateway(credentials, environment.RegionName, natGateway.NatGatewayId);

                        // make sure we get this before the ID is nulled out...
                        var natGatewayObject = new NatGateway { Id = natGateway.NatGatewayId, Name = natGateway.Name, Region = environment.RegionName };
                        natGateway.UpdateId(null);
                        localOnUpdate(environment);
                        await WaitUntil.NatGatewayInState(
                            natGatewayObject,
                            NatGatewayState.Deleted,
                            new[] { NatGatewayState.Failed, NatGatewayState.Available, NatGatewayState.Pending },
                            timeout,
                            credentials);
                        localAnnouncer(() => Invariant($"< {nameof(NatGateway)}"));
                    }
                }

                foreach (var subnet in vpc.Subnets)
                {
                    if (!string.IsNullOrWhiteSpace(subnet.SubnetId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(Subnet)} - {subnet.Name} ({subnet.SubnetId})"));
                        await RemoveSubnet(credentials, environment.RegionName, subnet.SubnetId);
                        subnet.UpdateId(null);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(Subnet)}"));
                    }
                }

                foreach (var networkAcl in vpc.NetworkAcls)
                {
                    if (!networkAcl.IsDefault && !string.IsNullOrWhiteSpace(networkAcl.NetworkAclId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(NetworkAcl)} - {networkAcl.Name} ({networkAcl.NetworkAclId})"));
                        await RemoveNetworkAcl(credentials, environment.RegionName, networkAcl.NetworkAclId);
                        networkAcl.UpdateId(null);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)}"));
                    }
                }

                foreach (var securityGroup in vpc.SecurityGroups)
                {
                    if (!securityGroup.IsDefault && !string.IsNullOrWhiteSpace(securityGroup.SecurityGroupId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(SecurityGroup)} - {securityGroup.Name} ({securityGroup.SecurityGroupId})"));
                        await RemoveSecurityGroup(credentials, environment.RegionName, securityGroup.SecurityGroupId);
                        securityGroup.UpdateId(null);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(SecurityGroup)}"));
                    }
                }

                foreach (var routeTable in vpc.RouteTables)
                {
                    if (!routeTable.IsDefault && !string.IsNullOrWhiteSpace(routeTable.RouteTableId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(RouteTable)} - {routeTable.Name} ({routeTable.RouteTableId})"));
                        await RemoveRouteTable(credentials, environment.RegionName, routeTable.RouteTableId);
                        routeTable.UpdateId(null);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(RouteTable)}"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(vpc.VpcId))
                {
                    var internetGatewayForVpc = environment.InternetGateways.SingleOrDefault(_ => _.Name.Equals(vpc.InternetGatewayRef, StringComparison.InvariantCultureIgnoreCase));
                    if (internetGatewayForVpc != null && !string.IsNullOrWhiteSpace(internetGatewayForVpc.InternetGatewayId))
                    {
                        localAnnouncer(() => Invariant($"> {nameof(Vpc)} - Detach Internet Gateway - {vpc.Name} ({vpc.VpcId}) {internetGatewayForVpc.InternetGatewayId}"));
                        await DetachInternetGatewayToVpc(credentials, environment.RegionName, vpc.VpcId, internetGatewayForVpc.InternetGatewayId);
                        localAnnouncer(() => Invariant($"< {nameof(Vpc)} - Detach Internet Gateway"));
                    }

                    localAnnouncer(() => Invariant($"> {nameof(Vpc)} - {vpc.Name} ({vpc.VpcId})"));
                    await RemoveVpc(credentials, environment.RegionName, vpc.VpcId);
                    vpc.UpdatedId(null);
                    localOnUpdate(environment);
                    localAnnouncer(() => Invariant($"< {nameof(Vpc)}"));
                }
            }

            foreach (var elasticIp in environment.ElasticIps)
            {
                if (!string.IsNullOrWhiteSpace(elasticIp.AllocationId))
                {
                    localAnnouncer(() => Invariant($"> {nameof(ElasticIp)} - {elasticIp.Name} ({elasticIp.AllocationId})"));
                    await ReleaseElasticIp(credentials, environment.RegionName, elasticIp.AllocationId);
                    elasticIp.UpdateId(null);
                    elasticIp.UpdateIpAddress(null);
                    localOnUpdate(environment);
                    localAnnouncer(() => Invariant($"< {nameof(ElasticIp)}"));
                }
            }

            foreach (var internetGateway in environment.InternetGateways)
            {
                if (!string.IsNullOrWhiteSpace(internetGateway.InternetGatewayId))
                {
                    localAnnouncer(() => Invariant($"> {nameof(InternetGateway)} - {internetGateway.Name} ({internetGateway.InternetGatewayId})"));
                    await RemoveInternetGateway(credentials, environment.RegionName, internetGateway.InternetGatewayId);
                    internetGateway.UpdateId(null);
                    localOnUpdate(environment);
                    localAnnouncer(() => Invariant($"< {nameof(InternetGateway)}"));
                }
            }

            return environment;
        }

        /// <summary>
        /// Removes an internet gateway.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="internetGatewayId">ID of internet gateway.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveInternetGateway(CredentialContainer credentials, string regionName, string internetGatewayId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteInternetGatewayRequest { InternetGatewayId = internetGatewayId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Release an elastic IP.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="elasticIpId">ID of elastic IP.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task ReleaseElasticIp(CredentialContainer credentials, string regionName, string elasticIpId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.ReleaseAddressRequest { AllocationId = elasticIpId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.ReleaseAddressAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a VPC.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="vpcId">ID of VPC.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public static async Task RemoveVpc(CredentialContainer credentials, string regionName, string vpcId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteVpcRequest() { VpcId = vpcId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteVpcAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Detach an internet gateway to a VPC.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="vpcId">ID of VPC.</param>
        /// <param name="internetGatewayId">ID of the internet gateway.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public static async Task DetachInternetGatewayToVpc(CredentialContainer credentials, string regionName, string vpcId, string internetGatewayId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DetachInternetGatewayRequest { VpcId = vpcId, InternetGatewayId = internetGatewayId, };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DetachInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableId">ID of route table.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveRouteTable(CredentialContainer credentials, string regionName, string routeTableId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteRouteTableRequest { RouteTableId = routeTableId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteRouteTableAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a subnet.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="subnetId">ID of subnet.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveSubnet(CredentialContainer credentials, string regionName, string subnetId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteSubnetRequest { SubnetId = subnetId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclId">ID of network ACL.</param>
        /// <returns>Populated object.</returns>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task RemoveNetworkAcl(CredentialContainer credentials, string regionName, string networkAclId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteNetworkAclRequest { NetworkAclId = networkAclId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteNetworkAclAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupId">ID of security group.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteSecurityGroupRequest { GroupId = securityGroupId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteSecurityGroupAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Removes a NAT gateway.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="natGatewayId">ID of NAT gateway.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveNatGateway(CredentialContainer credentials, string regionName, string natGatewayId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DeleteNatGatewayRequest { NatGatewayId = natGatewayId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DeleteNatGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }
    }
}