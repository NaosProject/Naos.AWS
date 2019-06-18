// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Creator.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Naos.AWS.Domain;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Create resources.
    /// </summary>
    public static class Creator
    {
        /// <summary>
        /// Create an environment in a region.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="environment">Environment config to use.</param>
        /// <param name="updateCallback">Optional callback to perform on object model update.</param>
        /// <param name="announcer">Optional callback to log announcements.</param>
        /// <param name="timeout">Optional timeout to wait for operations to complete; DEFAULT is infinity.</param>
        /// <returns>Environment config updated with IDs.</returns>
        public static async Task<ConfigEnvironment> CreateEnvironment(CredentialContainer credentials, ConfigEnvironment environment, Action<ConfigEnvironment> updateCallback = null, Action<Func<object>> announcer = null, TimeSpan timeout = default(TimeSpan))
        {
            environment.ThrowIfInvalid(true);

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

            foreach (var internetGateway in environment.InternetGateways)
            {
                localAnnouncer(() => Invariant($"> {nameof(InternetGateway)} - {internetGateway.Name}"));
                var createdInternetGateway = await CreateInternetGateway(credentials, environment.RegionName, internetGateway.Name, timeout);
                internetGateway.UpdateId(createdInternetGateway.Id);
                localOnUpdate(environment);
                localAnnouncer(() => Invariant($"< {nameof(InternetGateway)} - {internetGateway.InternetGatewayId}"));
            }

            foreach (var elasticIp in environment.ElasticIps)
            {
                localAnnouncer(() => Invariant($"> {nameof(ElasticIp)} - {elasticIp.Name}"));
                var createdElasticIp = await AllocateElasticIp(credentials, environment.RegionName, elasticIp.Name);
                elasticIp.UpdateId(createdElasticIp.Id);
                elasticIp.UpdateIpAddress(createdElasticIp.PublicIpAddress);
                localOnUpdate(environment);
                localAnnouncer(() => Invariant($"< {nameof(ElasticIp)} - {elasticIp.AllocationId}"));
            }

            var nameToCidrMap = new Dictionary<string, string> { { ConfigCidr.AllTrafficCidrName, "0.0.0.0/0" } };
            foreach (var vpc in environment.Vpcs)
            {
                localAnnouncer(() => Invariant($"> {nameof(Vpc)} - {vpc.Name}"));
                var createdVpc = await CreateVpc(credentials, environment.RegionName, vpc.Name, vpc.Cidr, vpc.Tenancy, timeout);
                vpc.UpdatedId(createdVpc.Id);
                localOnUpdate(environment);
                nameToCidrMap.Add(vpc.Name, vpc.Cidr);
                localAnnouncer(() => Invariant($"< {nameof(Vpc)} - {vpc.VpcId}"));

                var internetGatewayForVpc = environment.InternetGateways.SingleOrDefault(_ => _.Name.Equals(vpc.InternetGatewayRef, StringComparison.InvariantCultureIgnoreCase));
                if (internetGatewayForVpc != null)
                {
                    localAnnouncer(() => Invariant($"> {nameof(Vpc)} - Attach Internet Gateway - {vpc.Name} ({vpc.VpcId}) {internetGatewayForVpc.InternetGatewayId}"));
                    await AttachInternetGatewayToVpc(credentials, environment.RegionName, vpc.VpcId, internetGatewayForVpc.InternetGatewayId);
                    localAnnouncer(() => Invariant($"< {nameof(Vpc)} - Attach Internet Gateway"));
                }

                var defaultRouteTable = vpc.RouteTables.Single(_ => _.IsDefault);
                localAnnouncer(() => Invariant($"> {nameof(RouteTable)} - {defaultRouteTable.Name}"));
                var updatedRouteTable = await NameDefaultRouteTable(credentials, environment.RegionName, defaultRouteTable.Name, vpc.VpcId, timeout);
                defaultRouteTable.UpdateId(updatedRouteTable.Id);
                localOnUpdate(environment);
                localAnnouncer(() => Invariant($"< {nameof(RouteTable)} - {defaultRouteTable.RouteTableId}"));

                foreach (var routeTable in vpc.RouteTables)
                {
                    if (!routeTable.IsDefault)
                    {
                        localAnnouncer(() => Invariant($"> {nameof(RouteTable)} - {routeTable.Name}"));
                        var createdRouteTable = await CreateRouteTable(credentials, environment.RegionName, routeTable.Name, vpc.VpcId, timeout);
                        routeTable.UpdateId(createdRouteTable.Id);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(RouteTable)} - {routeTable.RouteTableId}"));
                    }
                }

                foreach (var subnet in vpc.Subnets)
                {
                    localAnnouncer(() => Invariant($"> {nameof(Subnet)} - {subnet.Name}"));
                    var createdSubnet = await CreateSubnet(credentials, environment.RegionName, subnet.Name, vpc.VpcId, subnet.Cidr, subnet.AvailabilityZone, timeout);
                    subnet.UpdateId(createdSubnet.Id);
                    localOnUpdate(environment);
                    nameToCidrMap.Add(subnet.Name, subnet.Cidr);
                    localAnnouncer(() => Invariant($"< {nameof(Subnet)} - {subnet.SubnetId}"));

                    var routeTable = vpc.RouteTables.SingleOrDefault(_ => _.Name.Equals(subnet.RouteTableRef, StringComparison.CurrentCultureIgnoreCase));
                    if (routeTable != null)
                    {
                        localAnnouncer(() => Invariant($"> {nameof(Subnet)} - Associate Route Table - {subnet.Name} ({subnet.SubnetId}) {routeTable.Name}"));
                        await AssociateRouteTableWithSubnet(credentials, environment.RegionName, routeTable.RouteTableId, subnet.SubnetId);
                        localAnnouncer(() => Invariant($"< {nameof(Subnet)} - Associate Route Table"));
                    }
                }

                foreach (var networkAcl in vpc.NetworkAcls)
                {
                    if (!networkAcl.IsDefault)
                    {
                        localAnnouncer(() => Invariant($"> {nameof(NetworkAcl)} - {networkAcl.Name}"));
                        var createdNetworkAcl = await CreateNetworkAcl(credentials, environment.RegionName, networkAcl.Name, vpc.VpcId, timeout);
                        networkAcl.UpdateId(createdNetworkAcl.Id);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)} - {networkAcl.NetworkAclId}"));

                        if (!string.IsNullOrWhiteSpace(networkAcl.SubnetRef))
                        {
                            var subnetRefs = networkAcl.SubnetRef.Split(',');
                            foreach (var subnetRef in subnetRefs)
                            {
                                var subnet = vpc.Subnets.SingleOrDefault(_ => _.Name.Equals(subnetRef, StringComparison.CurrentCultureIgnoreCase));
                                if (subnet == null)
                                {
                                    throw new ArgumentException(
                                        Invariant($"Must specify a valid subnet name if trying to associate an ACL; {subnetRef} is invalid."));
                                }

                                localAnnouncer(
                                    () => Invariant(
                                        $"> {nameof(NetworkAcl)} - Associate Subnet - {networkAcl.Name} ({networkAcl.NetworkAclId}) {subnet.Name}"));
                                await AssociateNetworkAclWithSubnet(credentials, environment.RegionName, networkAcl.NetworkAclId, subnet.SubnetId);
                                localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)} - Associate Subnet"));
                            }
                        }
                    }
                    else
                    {
                        localAnnouncer(() => Invariant($"> {nameof(NetworkAcl)} - {networkAcl.Name}"));
                        var defaultNetworkAcl = await NameDefaultNetworkAcl(credentials, environment.RegionName, networkAcl.Name, vpc.VpcId, timeout);
                        networkAcl.UpdateId(defaultNetworkAcl.Id);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)} - {networkAcl.NetworkAclId}"));
                    }

                    localAnnouncer(() => Invariant($"> {nameof(NetworkAcl)} - Remove All Rules - {networkAcl.Name} ({networkAcl.NetworkAclId})"));
                    await RemoveAllRulesFromNetworkAcl(credentials, environment.RegionName, networkAcl.NetworkAclId);
                    localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)} - Remove All Rules"));

                    localAnnouncer(() => Invariant($"> {nameof(NetworkAcl)} - Add Rules - {networkAcl.Name} ({networkAcl.NetworkAclId})"));
                    await AddRulesToNetworkAcl(credentials, environment.RegionName, networkAcl.NetworkAclId, networkAcl.InboundRules, networkAcl.OutboundRules, nameToCidrMap);
                    localAnnouncer(() => Invariant($"< {nameof(NetworkAcl)} - Add Rules"));
                }

                foreach (var securityGroup in vpc.SecurityGroups)
                {
                    if (!securityGroup.IsDefault)
                    {
                        localAnnouncer(() => Invariant($"> {nameof(SecurityGroup)} - {securityGroup.Name}"));
                        var createdSecurityGroup = await CreateSecurityGroup(credentials, environment.RegionName, securityGroup.Name, vpc.VpcId, timeout);
                        securityGroup.UpdateId(createdSecurityGroup.Id);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(SecurityGroup)} - {securityGroup.SecurityGroupId}"));
                    }
                    else
                    {
                        localAnnouncer(() => Invariant($"> {nameof(SecurityGroup)} - {securityGroup.Name}"));
                        var defaultSecurityGroup = await NameDefaultSecurityGroup(credentials, environment.RegionName, securityGroup.Name, vpc.VpcId, timeout);
                        securityGroup.UpdateId(defaultSecurityGroup.Id);
                        localOnUpdate(environment);
                        localAnnouncer(() => Invariant($"< {nameof(SecurityGroup)} - {securityGroup.SecurityGroupId}"));
                    }

                    localAnnouncer(() => Invariant($"> {nameof(SecurityGroup)} - Remove All Rules - {securityGroup.Name} ({securityGroup.SecurityGroupId})"));
                    await RemoveAllRulesFromSecurityGroup(credentials, environment.RegionName, securityGroup.SecurityGroupId);
                    localAnnouncer(() => Invariant($"< {nameof(SecurityGroup)} - Remove All Rules"));

                    localAnnouncer(() => Invariant($"> {nameof(SecurityGroup)} - Add Rules - {securityGroup.Name} ({securityGroup.SecurityGroupId})"));
                    await AddRulesToSecurityGroup(credentials, environment.RegionName, securityGroup.SecurityGroupId, securityGroup.InboundRules, securityGroup.OutboundRules, nameToCidrMap);
                    localAnnouncer(() => Invariant($"< {nameof(SecurityGroup)} - Add Rules"));
                }

                foreach (var natGateway in vpc.NatGateways)
                {
                    var parentSubnet = vpc.Subnets.SingleOrDefault(_ => _.Name.Equals(natGateway.SubnetRef, StringComparison.CurrentCultureIgnoreCase));
                    if (parentSubnet == null)
                    {
                        throw new ArgumentException(Invariant($"Must specify a valid subnet name to create a Nat Gateway; {natGateway.SubnetRef} is invalid."));
                    }

                    var elasticIp = environment.ElasticIps.SingleOrDefault(_ => _.Name.Equals(natGateway.ElasticIpRef, StringComparison.CurrentCultureIgnoreCase));
                    if (elasticIp == null)
                    {
                        throw new ArgumentException(Invariant($"Must specify a valid elastic ip name to create a Nat Gateway; {natGateway.ElasticIpRef} is invalid."));
                    }

                    localAnnouncer(() => Invariant($"> {nameof(NatGateway)} - {natGateway.Name}"));
                    var createdNatGateway = await CreateNatGateway(credentials, environment.RegionName, natGateway.Name, parentSubnet.SubnetId, elasticIp.AllocationId, timeout);
                    natGateway.UpdateId(createdNatGateway.Id);
                    localOnUpdate(environment);
                    localAnnouncer(() => Invariant($"< {nameof(NatGateway)} - {natGateway.NatGatewayId}"));
                }

                foreach (var natGateway in vpc.NatGateways)
                {
                    var natGatewayObject = new NatGateway { Id = natGateway.NatGatewayId, Name = natGateway.Name, Region = environment.RegionName };
                    await WaitUntil.NatGatewayInState(
                        natGatewayObject,
                        NatGatewayState.Available,
                        new[] { NatGatewayState.Failed, NatGatewayState.Deleted, NatGatewayState.Deleting },
                        timeout,
                        credentials);
                }

                foreach (var routeTable in vpc.RouteTables)
                {
                    localAnnouncer(() => Invariant($"> {nameof(RouteTable)} - Remove Routes - {routeTable.Name} ({routeTable.RouteTableId})"));
                    await RemoveAllRoutesFromRouteTable(credentials, environment.RegionName, routeTable.RouteTableId);
                    localAnnouncer(() => Invariant($"< {nameof(RouteTable)} - Remove Routes"));

                    localAnnouncer(() => Invariant($"> {nameof(RouteTable)} - Add Routes - {routeTable.Name} ({routeTable.RouteTableId})"));
                    await AddRoutesToRouteTable(
                        credentials,
                        environment.RegionName,
                        routeTable.RouteTableId,
                        routeTable.Routes,
                        nameToCidrMap,
                        environment.InternetGateways.ToDictionary(k => k.Name, v => v.InternetGatewayId),
                        vpc.NatGateways.ToDictionary(k => k.Name, v => v.NatGatewayId));
                    localAnnouncer(() => Invariant($"< {nameof(RouteTable)} - Add Routes"));
                }
            }

            return environment;
        }

        /// <summary>
        /// Creates an internet gateway.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="internetGatewayName">Name of internet gateway.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        public static async Task<InternetGateway> CreateInternetGateway(CredentialContainer credentials, string regionName, string internetGatewayName, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateInternetGatewayRequest();

            Amazon.EC2.Model.InternetGateway responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.InternetGateway;
            }

            var ret = new InternetGateway { Name = internetGatewayName, Id = responseObject.InternetGatewayId, Region = regionName, };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Allocates an elastic IP.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="elasticIpName">Name of elastic IP.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task<ElasticIp> AllocateElasticIp(CredentialContainer credentials, string regionName, string elasticIpName)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.AllocateAddressRequest { Domain = Amazon.EC2.DomainType.Standard };

            Amazon.EC2.Model.AllocateAddressResponse responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.AllocateAddressAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response;
            }

            var ret = new ElasticIp { Id = responseObject.AllocationId, Name = elasticIpName, Region = regionName, PublicIpAddress = responseObject.PublicIp };

            // can't tag these guys yet...
            return ret;
        }

        /// <summary>
        /// Creates a VPC.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="vpcName">Name of VPC.</param>
        /// <param name="cidr">CIDR of VPC.</param>
        /// <param name="tenancy">Tenancy of VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cidr", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public static async Task<Vpc> CreateVpc(CredentialContainer credentials, string regionName, string vpcName, string cidr, string tenancy, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateVpcRequest() { CidrBlock = cidr, InstanceTenancy = new Amazon.EC2.Tenancy(tenancy) };

            Amazon.EC2.Model.Vpc responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateVpcAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.Vpc;
            }

            var ret = new Vpc { Id = responseObject.VpcId, Cidr = cidr, Name = vpcName, Region = regionName, Tenancy = tenancy };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Attach an internet gateway to a VPC.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="vpcId">ID of VPC.</param>
        /// <param name="internetGatewayId">ID of the internet gateway.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public static async Task AttachInternetGatewayToVpc(CredentialContainer credentials, string regionName, string vpcId, string internetGatewayId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.AttachInternetGatewayRequest { VpcId = vpcId, InternetGatewayId = internetGatewayId, };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.AttachInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Name default route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableName">Route table name.</param>
        /// <param name="vpcId">ID or parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is infinity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<RouteTable> NameDefaultRouteTable(CredentialContainer credentials, string regionName, string routeTableName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DescribeRouteTablesRequest
                              {
                                  Filters = new[]
                                                {
                                                    new Amazon.EC2.Model.Filter("vpc-id", new[] { vpcId }.ToList()),
                                                }.ToList(),
                              };

            Amazon.EC2.Model.RouteTable responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DescribeRouteTablesAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.RouteTables.Single();
            }

            var ret = new RouteTable
                          {
                              Id = responseObject.RouteTableId,
                              Region = regionName,
                              Name = routeTableName,
                              IsDefault = true,
                              VpcId = vpcId,
                          };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Creates a route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableName">Name of route table.</param>
        /// <param name="vpcId">ID or parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<RouteTable> CreateRouteTable(CredentialContainer credentials, string regionName, string routeTableName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateRouteTableRequest { VpcId = vpcId };

            Amazon.EC2.Model.RouteTable responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateRouteTableAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.RouteTable;
            }

            var ret = new RouteTable { Id = responseObject.RouteTableId, Region = regionName, Name = routeTableName, IsDefault = false, VpcId = vpcId };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Remove all routes from a route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableId">ID of route table.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveAllRoutesFromRouteTable(CredentialContainer credentials, string regionName, string routeTableId)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { routeTableId }.Must().NotBeNullNorWhiteSpace();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var gatewayIdOfFixedVpcRoute = "local";

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // silly API restriction, you have to get entries then delete them by destination cidr...
                var getRouteTableRequest =
                    new Amazon.EC2.Model.DescribeRouteTablesRequest
                        {
                            Filters = new[]
                                          {
                                              new Amazon.EC2.Model.Filter(
                                                  "route-table-id",
                                                  new[] { routeTableId }.ToList()),
                                          }.ToList(),
                        };

                var getRouteTableResponse = await client.DescribeRouteTablesAsync(getRouteTableRequest);
                var routeTable = getRouteTableResponse.RouteTables.SingleOrDefault(_ => _.RouteTableId.Equals(routeTableId, StringComparison.InvariantCultureIgnoreCase));
                if (routeTable == null)
                {
                    throw new ArgumentException($"Must specify a valud {nameof(routeTableId)} to remove routes; {routeTableId} is invalid.");
                }

                foreach (var route in routeTable.Routes.Where(_ => !_.GatewayId.Equals(gatewayIdOfFixedVpcRoute, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var removeRouteRequest = new Amazon.EC2.Model.DeleteRouteRequest { RouteTableId = routeTableId, DestinationCidrBlock = route.DestinationCidrBlock };
                    var removeRouteResponse = await client.DeleteRouteAsync(removeRouteRequest);
                    Validator.ThrowOnBadResult(removeRouteRequest, removeRouteResponse);
                }
            }
        }

        /// <summary>
        /// Add routes to a route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableId">ID of route table.</param>
        /// <param name="routes">Routes to add.</param>
        /// <param name="nameToCidrMap">Name to available CIDRs map.</param>
        /// <param name="nameToInternetGatewayId">Name to available internet gateways map.</param>
        /// <param name="nameToNatGatewayId">Name to available NAT gateways map.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public static async Task AddRoutesToRouteTable(CredentialContainer credentials, string regionName, string routeTableId, IReadOnlyCollection<ConfigRoute> routes, IReadOnlyDictionary<string, string> nameToCidrMap, IReadOnlyDictionary<string, string> nameToInternetGatewayId, IReadOnlyDictionary<string, string> nameToNatGatewayId)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { routeTableId }.Must().NotBeNullNorWhiteSpace();
            new { routes }.Must().NotBeNull();
            new { nameToCidrMap }.Must().NotBeNull();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                foreach (var route in routes)
                {
                    nameToCidrMap.TryGetValue(route.DestinationRef, out string cidr).Named(Invariant($"Did-not-find-cidr-for-{route.DestinationRef}")).Must().BeTrue();
                    nameToInternetGatewayId.TryGetValue(route.TargetRef, out string igwId);
                    nameToNatGatewayId.TryGetValue(route.TargetRef, out string ngwId);

                    if (!string.IsNullOrWhiteSpace(igwId))
                    {
                        var addRoute = new Amazon.EC2.Model.CreateRouteRequest { RouteTableId = routeTableId, DestinationCidrBlock = cidr, GatewayId = igwId };
                        var addRuleResponse = await client.CreateRouteAsync(addRoute);
                        Validator.ThrowOnBadResult(addRoute, addRuleResponse);
                    }
                    else if (!string.IsNullOrWhiteSpace(ngwId))
                    {
                        var addRoute = new Amazon.EC2.Model.CreateRouteRequest { RouteTableId = routeTableId, DestinationCidrBlock = cidr, NatGatewayId = ngwId };
                        var addRuleResponse = await client.CreateRouteAsync(addRoute);
                        Validator.ThrowOnBadResult(addRoute, addRuleResponse);
                    }
                    else
                    {
                        throw new ArgumentException(Invariant($"Must have a valid gateway to target traffic; {route.TargetRef} is invalid."));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a subnet.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="subnetName">Name of subnet.</param>
        /// <param name="vpcId">ID of parent VPC.</param>
        /// <param name="cidr">CIDR of subnet.</param>
        /// <param name="availabilityZone">Availability zone of subnet.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cidr", Justification = "Spelling/name is correct.")]
        public static async Task<Subnet> CreateSubnet(CredentialContainer credentials, string regionName, string subnetName, string vpcId, string cidr, string availabilityZone, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateSubnetRequest { VpcId = vpcId, CidrBlock = cidr, AvailabilityZone = availabilityZone };

            Amazon.EC2.Model.Subnet responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.Subnet;
            }

            var ret = new Subnet { Id = responseObject.SubnetId, Region = regionName, Name = subnetName, AvailabilityZone = availabilityZone, Cidr = cidr };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Associates a route table with a subnet.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="routeTableId">Route table ID.</param>
        /// <param name="subnetId">Subnet ID.</param>
        /// <returns>Task for async.</returns>
        public static async Task AssociateRouteTableWithSubnet(CredentialContainer credentials, string regionName, string routeTableId, string subnetId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.AssociateRouteTableRequest { RouteTableId = routeTableId, SubnetId = subnetId };

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.AssociateRouteTableAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Name the default network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclName">Network ACL name.</param>
        /// <param name="vpcId">ID of the parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task<NetworkAcl> NameDefaultNetworkAcl(CredentialContainer credentials, string regionName, string networkAclName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DescribeNetworkAclsRequest
                              {
                                  Filters = new[]
                                                {
                                                    new Amazon.EC2.Model.Filter("vpc-id", new[] { vpcId }.ToList()),
                                                }.ToList(),
                              };

            Amazon.EC2.Model.NetworkAcl responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DescribeNetworkAclsAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.NetworkAcls.Single(_ => _.IsDefault);
            }

            var ret = new NetworkAcl
                          {
                              Id = responseObject.NetworkAclId,
                              Region = regionName,
                              Name = networkAclName,
                              IsDefault = true,
                          };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Creates a network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclName">Name of network ACL.</param>
        /// <param name="vpcId">ID of the parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task<NetworkAcl> CreateNetworkAcl(CredentialContainer credentials, string regionName, string networkAclName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateNetworkAclRequest { VpcId = vpcId };

            Amazon.EC2.Model.NetworkAcl responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateNetworkAclAsync(request);
                Validator.ThrowOnBadResult(request, response);
                responseObject = response.NetworkAcl;
            }

            var ret = new NetworkAcl { Id = responseObject.NetworkAclId, IsDefault = false, Name = networkAclName, Region = regionName };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Associates a network ACL with a subnet.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclId">ID of the network ACL.</param>
        /// <param name="subnetId">ID of the subnet.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task AssociateNetworkAclWithSubnet(CredentialContainer credentials, string regionName, string networkAclId, string subnetId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // silly API restriction, you have to get the association ID first then replace the Network ACL which will update the one used by the subnet...
                var getAclRequest = new Amazon.EC2.Model.DescribeNetworkAclsRequest
                                        {
                                            Filters = new[]
                                                          {
                                                              new Amazon.EC2.Model.Filter(
                                                                  "association.subnet-id",
                                                                  new[] { subnetId }.ToList()),
                                                          }.ToList(),
                                        };

                var getAclResponse = await client.DescribeNetworkAclsAsync(getAclRequest);

                // there should ONLY be the default ACL association which was put there during subnet creation
                var networkAclAssociationId = getAclResponse.NetworkAcls.SelectMany(_ => _.Associations.Where(a => a.SubnetId == subnetId)).SingleOrDefault()?.NetworkAclAssociationId;
                if (string.IsNullOrWhiteSpace(networkAclAssociationId))
                {
                    throw new ArgumentException(Invariant($"Must specify a valid {nameof(subnetId)} to associate a NetworkAcl to; {subnetId} is invalid."));
                }

                var replaceAssociationRequest = new Amazon.EC2.Model.ReplaceNetworkAclAssociationRequest
                                  {
                                      NetworkAclId = networkAclId,
                                      AssociationId = networkAclAssociationId,
                                  };

                var response = await client.ReplaceNetworkAclAssociationAsync(replaceAssociationRequest);
                Validator.ThrowOnBadResult(replaceAssociationRequest, response);
            }
        }

        /// <summary>
        /// Removes all rules from a network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclId">ID of the network ACL.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task RemoveAllRulesFromNetworkAcl(CredentialContainer credentials, string regionName, string networkAclId)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { networkAclId }.Must().NotBeNullNorWhiteSpace();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // silly API restriction, you have to get entries then delete them by rule number...
                var getAclRequest = new Amazon.EC2.Model.DescribeNetworkAclsRequest
                                        {
                                            Filters = new[]
                                                          {
                                                              new Amazon.EC2.Model.Filter(
                                                                  "network-acl-id",
                                                                  new[] { networkAclId }.ToList()),
                                                          }.ToList(),
                                        };

                var getAclResponse = await client.DescribeNetworkAclsAsync(getAclRequest);
                var networkAcl = getAclResponse.NetworkAcls.SingleOrDefault(_ => _.NetworkAclId.Equals(networkAclId, StringComparison.InvariantCultureIgnoreCase));
                if (networkAcl == null)
                {
                    throw new ArgumentException($"Must specify a valud {nameof(networkAclId)} to remove rules; {networkAclId} is invalid.");
                }

                foreach (var entry in networkAcl.Entries.Where(_ => _.RuleNumber != NetworkAcl.FallThroughRuleNumber))
                {
                    var removeRuleRequest = new Amazon.EC2.Model.DeleteNetworkAclEntryRequest
                                                        {
                                                            NetworkAclId = networkAclId,
                                                            Egress = entry.Egress,
                                                            RuleNumber = entry.RuleNumber,
                                                        };

                    var removeRuleResponse = await client.DeleteNetworkAclEntryAsync(removeRuleRequest);
                    Validator.ThrowOnBadResult(removeRuleRequest, removeRuleResponse);
                }
            }
        }

        /// <summary>
        /// Add rules to a network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclId">ID of network ACL.</param>
        /// <param name="inboundRules">Inbound rules.</param>
        /// <param name="outboundRules">Outbound rules.</param>
        /// <param name="nameToCidrMap">Name to available CIDRs map.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task AddRulesToNetworkAcl(CredentialContainer credentials, string regionName, string networkAclId, IReadOnlyCollection<ConfigNetworkAclInboundRule> inboundRules, IReadOnlyCollection<ConfigNetworkAclOutboundRule> outboundRules, IReadOnlyDictionary<string, string> nameToCidrMap)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { networkAclId }.Must().NotBeNullNorWhiteSpace();
            new { inboundRules }.Must().NotBeNull();
            new { outboundRules }.Must().NotBeNull();
            new { nameToCidrMap }.Must().NotBeNull();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // ReSharper disable once RedundantEnumerableCastCall - prefer to have it convert both...
                var allRules = inboundRules.Cast<ConfigNetworkAclRuleBase>().Concat(outboundRules.Cast<ConfigNetworkAclRuleBase>()).ToList();
                foreach (var rule in allRules)
                {
                    var type = rule.GetTypeForApi();

                    var protocol = rule.GetProtocolForApi();

                    var portRange = rule.GetPortRangeForApi();

                    var cidr = rule.GetCidrForApi(nameToCidrMap);

                    var direction = rule.RuleDirection;
                    bool egress;
                    if (direction == ConfigTrafficRuleBase.TrafficRuleDirection.Inbound)
                    {
                        egress = false;
                    }
                    else if (direction == ConfigTrafficRuleBase.TrafficRuleDirection.Outbound)
                    {
                        egress = true;
                    }
                    else
                    {
                        throw new ArgumentException(Invariant($"RuleDirectionCannotBeInvalid-CidrRef-{rule.CidrNameRef}-{rule.Comment}"));
                    }

                    var addRuleRequest = new Amazon.EC2.Model.CreateNetworkAclEntryRequest
                    {
                        NetworkAclId = networkAclId,
                        Egress = egress,
                        RuleNumber = rule.RuleNumber,
                        RuleAction = rule.Action.ToUpper(),
                        CidrBlock = cidr,
                        IcmpTypeCode = new Amazon.EC2.Model.IcmpTypeCode { Type = type },
                        PortRange = new Amazon.EC2.Model.PortRange { From = portRange.From, To = portRange.To },
                        Protocol = protocol,
                    };

                    var addRuleResponse = await client.CreateNetworkAclEntryAsync(addRuleRequest);
                    Validator.ThrowOnBadResult(addRuleRequest, addRuleResponse);
                }
            }
        }

        /// <summary>
        /// Name default security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupName">Name of security group.</param>
        /// <param name="vpcId">ID of parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<SecurityGroup> NameDefaultSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.DescribeSecurityGroupsRequest
            {
                Filters = new[]
                {
                    new Amazon.EC2.Model.Filter("vpc-id", new[] { vpcId }.ToList()),
                    new Amazon.EC2.Model.Filter("group-name", new[] { "default" }.ToList()),
                }.ToList(),
            };

            Amazon.EC2.Model.SecurityGroup responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.DescribeSecurityGroupsAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.SecurityGroups.Single();
            }

            var ret = new SecurityGroup
                          {
                              Id = responseObject.GroupId,
                              Region = regionName,
                              Name = securityGroupName,
                              IsDefault = true,
                          };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Creates a security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupName">Name of security group.</param>
        /// <param name="vpcId">ID of parent VPC.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<SecurityGroup> CreateSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupName, string vpcId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateSecurityGroupRequest { VpcId = vpcId };

            Amazon.EC2.Model.CreateSecurityGroupResponse responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateSecurityGroupAsync(request);
                Validator.ThrowOnBadResult(request, response);
                responseObject = response;
            }

            var ret = new SecurityGroup { Id = responseObject.GroupId, IsDefault = false, Name = securityGroupName, Region = regionName };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }

        /// <summary>
        /// Remove rules from security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupId">ID of the security group.</param>
        /// <returns>Task for async.</returns>
        public static async Task RemoveAllRulesFromSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupId)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { securityGroupId }.Must().NotBeNullNorWhiteSpace();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // silly API restriction, you have to get entries then delete them by permission...
                var getGroupRequest =
                    new Amazon.EC2.Model.DescribeSecurityGroupsRequest
                        {
                            Filters = new[]
                                          {
                                              new Amazon.EC2.Model.Filter(
                                                  "group-id",
                                                  new[] { securityGroupId }.ToList()),
                                          }.ToList(),
                        };

                var getAclResponse = await client.DescribeSecurityGroupsAsync(getGroupRequest);
                var securityGroup = getAclResponse.SecurityGroups.SingleOrDefault(_ => _.GroupId.Equals(securityGroupId, StringComparison.InvariantCultureIgnoreCase));
                if (securityGroup == null)
                {
                    throw new ArgumentException($"Must specify a valud {nameof(securityGroupId)} to remove rules; {securityGroupId} is invalid.");
                }

                foreach (var permission in securityGroup.IpPermissions)
                {
                    var removeRuleRequest = new Amazon.EC2.Model.RevokeSecurityGroupIngressRequest { GroupId = securityGroupId, IpPermissions = new[] { permission }.ToList(), };
                    var removeRuleResponse = await client.RevokeSecurityGroupIngressAsync(removeRuleRequest);
                    Validator.ThrowOnBadResult(removeRuleRequest, removeRuleResponse);
                }

                foreach (var permission in securityGroup.IpPermissionsEgress)
                {
                    var removeRuleRequest = new Amazon.EC2.Model.RevokeSecurityGroupEgressRequest { GroupId = securityGroupId, IpPermissions = new[] { permission }.ToList(), };
                    var removeRuleResponse = await client.RevokeSecurityGroupEgressAsync(removeRuleRequest);
                    Validator.ThrowOnBadResult(removeRuleRequest, removeRuleResponse);
                }
            }
        }

        /// <summary>
        /// Add rules to a security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupId">ID of security group.</param>
        /// <param name="inboundRules">Inbound rules.</param>
        /// <param name="outboundRules">Outbound rules.</param>
        /// <param name="nameToCidrMap">Name to available CIDR map.</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public static async Task AddRulesToSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupId, IReadOnlyCollection<ConfigSecurityGroupInboundRule> inboundRules, IReadOnlyCollection<ConfigSecurityGroupOutboundRule> outboundRules, IReadOnlyDictionary<string, string> nameToCidrMap)
        {
            new { regionName }.Must().NotBeNullNorWhiteSpace();
            new { securityGroupId }.Must().NotBeNullNorWhiteSpace();
            new { inboundRules }.Must().NotBeNull();
            new { outboundRules }.Must().NotBeNull();
            new { nameToCidrMap }.Must().NotBeNull();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                // ReSharper disable once RedundantEnumerableCastCall - prefer to have it convert both...
                var allRules = inboundRules.Cast<ConfigSecurityGroupRuleBase>().Concat(outboundRules.Cast<ConfigSecurityGroupRuleBase>()).ToList();
                foreach (var rule in allRules)
                {
                    var protocol = rule.GetProtocolForApi();

                    var portRange = rule.GetPortRangeForApi();

                    var cidr = rule.GetCidrForApi(nameToCidrMap);

                    var permission = new Amazon.EC2.Model.IpPermission
                                         {
                                             IpProtocol = protocol,
                                             FromPort = portRange.From,
                                             ToPort = portRange.To,
                                             Ipv4Ranges = new[] { new Amazon.EC2.Model.IpRange { CidrIp = cidr } }.ToList(),
                                         };

                    var direction = rule.RuleDirection;
                    if (direction == ConfigTrafficRuleBase.TrafficRuleDirection.Inbound)
                    {
                        var addRuleRequest =
                            new Amazon.EC2.Model.AuthorizeSecurityGroupIngressRequest
                                {
                                    GroupId = securityGroupId,
                                    IpPermissions = new[] { permission }.ToList(),
                                };

                        var addRuleResponse = await client.AuthorizeSecurityGroupIngressAsync(addRuleRequest);
                        Validator.ThrowOnBadResult(addRuleRequest, addRuleResponse);
                    }
                    else if (direction == ConfigTrafficRuleBase.TrafficRuleDirection.Outbound)
                    {
                        var addRuleRequest =
                            new Amazon.EC2.Model.AuthorizeSecurityGroupEgressRequest
                                {
                                    GroupId = securityGroupId,
                                    IpPermissions = new[] { permission }.ToList(),
                                };

                        var addRuleResponse = await client.AuthorizeSecurityGroupEgressAsync(addRuleRequest);
                        Validator.ThrowOnBadResult(addRuleRequest, addRuleResponse);
                    }
                    else
                    {
                        throw new ArgumentException(Invariant($"RuleDirectionCannotBeInvalid-CidrRef-{rule.CidrNameRef}-{rule.Comment}"));
                    }
                }
            }
        }

        /// <summary>
        /// Creates a NAT gateway.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="natGatewayName">Name of NAT gateway.</param>
        /// <param name="subnetId">ID of containing subnet.</param>
        /// <param name="elasticIpAllocationId">ID of IP address to use.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task<NatGateway> CreateNatGateway(CredentialContainer credentials, string regionName, string natGatewayName, string subnetId, string elasticIpAllocationId, TimeSpan timeout)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateNatGatewayRequest
            {
                SubnetId = subnetId,
                AllocationId = elasticIpAllocationId,
            };

            Amazon.EC2.Model.NatGateway responseObject;
            using (var client = new Amazon.EC2.AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateNatGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.NatGateway;
            }

            var ret = new NatGateway { Id = responseObject.NatGatewayId, Name = natGatewayName, Region = regionName };

            await ret.TagNameInAwsAsync(timeout, credentials);

            return ret;
        }
    }
}
