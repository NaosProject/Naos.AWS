// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Creator.cs" company="Naos">
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
    using Amazon.EC2;

    using Naos.AWS.Domain;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Operations to be performed on AMI's.
    /// </summary>
    public static class Creator
    {
        /// <summary>
        /// Magic string to use all CIDR (0.0.0.0/0).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public const string AllTrafficCidrName = "AllTrafficCidr";

        /// <summary>
        /// Create an environment in a region.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="environment">Environment config to use.</param>
        /// <returns>Environment config updated with IDs.</returns>
        public static async Task<ConfigEnvironment> CreateEnvironment(CredentialContainer credentials, ConfigEnvironment environment)
        {
            environment.ThrowIfInvalid();

            foreach (var internetGateway in environment.InternetGateways)
            {
                var createdInternetGateway = await CreateInternetGateway(credentials, environment.RegionName, internetGateway.Name);
                internetGateway.UpdateId(createdInternetGateway.Id);
            }

            foreach (var elasticIp in environment.ElasticIps)
            {
                var createdElasticIp = await AllocateElasticIp(credentials, environment.RegionName, elasticIp.Name);
                elasticIp.UpdateId(createdElasticIp.Id);
                elasticIp.UpdateIpAddress(createdElasticIp.PublicIpAddress);
            }

            var nameToCidrMap = new Dictionary<string, string> { { AllTrafficCidrName, "0.0.0.0/0" } };
            foreach (var vpc in environment.Vpcs)
            {
                var createdVpc = await CreateVpc(credentials, environment.RegionName, vpc.Name, vpc.Cidr, vpc.Tenancy);
                vpc.UpdatedId(createdVpc.Id);
                nameToCidrMap.Add(vpc.Name, vpc.Cidr);

                foreach (var routeTable in vpc.RouteTables)
                {
                    if (!routeTable.IsDefault)
                    {
                        var createdRouteTable = await CreateRouteTable(credentials, environment.RegionName, routeTable.Name, vpc.VpcId);
                        routeTable.UpdateId(createdRouteTable.Id);
                    }
                }

                foreach (var subnet in vpc.Subnets)
                {
                    var createdSubnet = await CreateSubnet(credentials, environment.RegionName, subnet.Name, vpc.VpcId, subnet.Cidr, subnet.AvailabilityZone);
                    subnet.UpdateId(createdSubnet.Id);
                    nameToCidrMap.Add(subnet.Name, subnet.Cidr);

                    var routeTable = vpc.RouteTables.SingleOrDefault(_ => _.Name.Equals(subnet.RouteTableRef, StringComparison.CurrentCultureIgnoreCase));
                    if (routeTable != null)
                    {
                        await AssociateRouteTableWithSubnet(credentials, environment.RegionName, routeTable.RouteTableId, subnet.SubnetId);
                    }
                }

                foreach (var networkAcl in vpc.NetworkAcls)
                {
                    if (!networkAcl.IsDefault)
                    {
                        var createdNetworkAcl = await CreateNetworkAcl(credentials, environment.RegionName, networkAcl.Name, vpc.VpcId);
                        networkAcl.UpdateId(createdNetworkAcl.Id);

                        if (!string.IsNullOrWhiteSpace(networkAcl.SubnetRef))
                        {
                            var subnetRefs = networkAcl.SubnetRef.Split(',');
                            foreach (var subnetRef in subnetRefs)
                            {
                                var subnet = vpc.Subnets.SingleOrDefault(_ => _.Name.Equals(subnetRef, StringComparison.CurrentCultureIgnoreCase));
                                if (subnet == null)
                                {
                                    throw new ArgumentException(Invariant($"Must specify a valid subnet name if trying to associate an ACL; {subnetRef} is invalid."));
                                }

                                await AssociateNetworkAclWithSubnet(credentials, environment.RegionName, networkAcl.NetworkAclId, subnet.SubnetId);
                            }
                        }
                    }

                    await RemoveAllRulesFromNetworkAcl(credentials, environment.RegionName, networkAcl.NetworkAclId);

                    await AddRulesToNetworkAcl(credentials, environment.RegionName, networkAcl.NetworkAclId, networkAcl.InboundRules, networkAcl.OutboundRules, nameToCidrMap);
                }

                foreach (var securityGroup in vpc.SecurityGroups)
                {
                    if (!securityGroup.IsDefault)
                    {
                        var createdSecurityGroup = await CreateSecurityGroup(credentials, environment.RegionName, securityGroup.Name, vpc.VpcId);
                        securityGroup.UpdateId(createdSecurityGroup.Id);
                    }

                    await RemoveAllRulesFromSecurityGroup(credentials, environment.RegionName, securityGroup.SecurityGroupId);

                    await AddRulesToSecurityGroup(credentials, environment.RegionName, securityGroup.SecurityGroupId, securityGroup.InboundRules, securityGroup.OutboundRules, nameToCidrMap);
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

                    var createdNatGateway = await CreateNatGateway(credentials, environment.RegionName, natGateway.Name, parentSubnet.SubnetId, elasticIp.AllocationId);
                    natGateway.UpdateId(createdNatGateway.Id);
                }

                foreach (var routeTable in vpc.RouteTables)
                {
                    await RemoveAllRoutesFromRouteTable(credentials, environment.RegionName, routeTable.RouteTableId);

                    await AddRoutesToRouteTable(
                        credentials,
                        environment.RegionName,
                        routeTable.RouteTableId,
                        routeTable.Routes,
                        nameToCidrMap,
                        environment.InternetGateways.ToDictionary(k => k.Name, v => v.InternetGatewayId),
                        vpc.NatGateways.ToDictionary(k => k.Name, v => v.NatGatewayId));
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
        /// <returns>Populated object.</returns>
        public static async Task<InternetGateway> CreateInternetGateway(CredentialContainer credentials, string regionName, string internetGatewayName)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateInternetGatewayRequest();

            Amazon.EC2.Model.InternetGateway responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateInternetGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.InternetGateway;
            }

            var ret = new InternetGateway { Name = internetGatewayName, Id = responseObject.InternetGatewayId, Region = regionName, };

            await ret.TagNameInAwsAsync(credentials);

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
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.AllocateAddressRequest { Domain = DomainType.Standard };

            Amazon.EC2.Model.AllocateAddressResponse responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cidr", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        public static async Task<Vpc> CreateVpc(CredentialContainer credentials, string regionName, string vpcName, string cidr, string tenancy)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateVpcRequest() { CidrBlock = cidr, InstanceTenancy = new Tenancy(tenancy) };

            Amazon.EC2.Model.Vpc responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateVpcAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.Vpc;
            }

            var ret = new Vpc { Id = responseObject.VpcId, Cidr = cidr, Name = vpcName, Region = regionName, Tenancy = tenancy };

            await ret.TagNameInAwsAsync(credentials);

            return ret;
        }

        /// <summary>
        /// Creates a route table.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="vpcId">ID or parent VPC.</param>
        /// <param name="routeTableName">Name of route table.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<RouteTable> CreateRouteTable(CredentialContainer credentials, string regionName, string vpcId, string routeTableName)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateRouteTableRequest { VpcId = vpcId };

            Amazon.EC2.Model.RouteTable responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateRouteTableAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.RouteTable;
            }

            var ret = new RouteTable { Id = responseObject.RouteTableId, Region = regionName, Name = routeTableName, IsDefault = false, VpcId = vpcId };

            await ret.TagNameInAwsAsync(credentials);

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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { routeTableId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
                var routeTable = getRouteTableResponse.RouteTables.SingleOrDefault();
                if (routeTable == null)
                {
                    throw new ArgumentException($"Must specify a valud {nameof(routeTableId)} to remove routes; {routeTableId} is invalid.");
                }

                foreach (var route in routeTable.Routes)
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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { routeTableId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { routes }.Must().NotBeNull().And().NotBeEmptyEnumerable<ConfigRoute>().OrThrowFirstFailure();
            new { nameToCidrMap }.Must().NotBeNull().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                foreach (var route in routes)
                {
                    nameToCidrMap.TryGetValue(route.DestinationRef, out string cidr).Named(Invariant($"Did-not-find-cidr-for-{route.DestinationRef}")).Must().BeTrue().OrThrow();
                    nameToInternetGatewayId.TryGetValue(route.TargetRef, out string igwId);
                    nameToNatGatewayId.TryGetValue(route.TargetRef, out string ngwId);

                    if (!string.IsNullOrWhiteSpace(igwId))
                    {
                        var addRoute = new Amazon.EC2.Model.CreateRouteRequest { RouteTableId = routeTableId, DestinationCidrBlock = cidr, GatewayId = igwId };
                        var addRuleResponse = await client.CreateRouteAsync(addRoute);
                        Validator.ThrowOnBadResult(addRoute, addRuleResponse);
                    }
                    else if (!string.IsNullOrWhiteSpace(igwId))
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
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "cidr", Justification = "Spelling/name is correct.")]
        public static async Task<Subnet> CreateSubnet(CredentialContainer credentials, string regionName, string subnetName, string vpcId, string cidr, string availabilityZone)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateSubnetRequest { VpcId = vpcId, CidrBlock = cidr, AvailabilityZone = availabilityZone };

            Amazon.EC2.Model.Subnet responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateSubnetAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.Subnet;
            }

            var ret = new Subnet { Id = responseObject.SubnetId, Region = regionName, Name = subnetName, AvailabilityZone = availabilityZone, Cidr = cidr };

            await ret.TagNameInAwsAsync(credentials);

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
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.AssociateRouteTableRequest { RouteTableId = routeTableId, SubnetId = subnetId };

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.AssociateRouteTableAsync(request);
                Validator.ThrowOnBadResult(request, response);
            }
        }

        /// <summary>
        /// Creates a network ACL.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="networkAclName">Name of network ACL.</param>
        /// <param name="vpcId">ID of the parent VPC.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        public static async Task<NetworkAcl> CreateNetworkAcl(CredentialContainer credentials, string regionName, string networkAclName, string vpcId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateNetworkAclRequest { VpcId = vpcId };

            Amazon.EC2.Model.NetworkAcl responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateNetworkAclAsync(request);
                Validator.ThrowOnBadResult(request, response);
                responseObject = response.NetworkAcl;
            }

            var ret = new NetworkAcl { Id = responseObject.NetworkAclId, IsDefault = false, Name = networkAclName, Region = regionName };

            await ret.TagNameInAwsAsync(credentials);

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
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { networkAclId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
                var networkAcl = getAclResponse.NetworkAcls.SingleOrDefault();
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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { networkAclId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { inboundRules }.Must().NotBeNull().And().NotBeEmptyEnumerable<ConfigNetworkAclInboundRule>().OrThrowFirstFailure();
            new { outboundRules }.Must().NotBeNull().And().NotBeEmptyEnumerable<ConfigNetworkAclOutboundRule>().OrThrowFirstFailure();
            new { nameToCidrMap }.Must().NotBeNull().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
                                                            RuleAction = rule.Action,
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
        /// Creates a security group.
        /// </summary>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="regionName">Region name to use.</param>
        /// <param name="securityGroupName">Name of security group.</param>
        /// <param name="vpcId">ID of parent VPC.</param>
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "vpc", Justification = "Spelling/name is correct.")]
        public static async Task<SecurityGroup> CreateSecurityGroup(CredentialContainer credentials, string regionName, string securityGroupName, string vpcId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateSecurityGroupRequest { VpcId = vpcId };

            Amazon.EC2.Model.CreateSecurityGroupResponse responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateSecurityGroupAsync(request);
                Validator.ThrowOnBadResult(request, response);
                responseObject = response;
            }

            var ret = new SecurityGroup { Id = responseObject.GroupId, IsDefault = false, Name = securityGroupName, Region = regionName };

            await ret.TagNameInAwsAsync(credentials);

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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { securityGroupId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
                var securityGroup = getAclResponse.SecurityGroups.SingleOrDefault();
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
            new { regionName }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { securityGroupId }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { inboundRules }.Must().NotBeNull().And().NotBeEmptyEnumerable<ConfigSecurityGroupInboundRule>().OrThrowFirstFailure();
            new { outboundRules }.Must().NotBeNull().And().NotBeEmptyEnumerable<ConfigSecurityGroupOutboundRule>().OrThrowFirstFailure();
            new { nameToCidrMap }.Must().NotBeNull().OrThrowFirstFailure();

            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
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
        /// <returns>Populated object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public static async Task<NatGateway> CreateNatGateway(CredentialContainer credentials, string regionName, string natGatewayName, string subnetId, string elasticIpAllocationId)
        {
            var awsCredentials = CredentialManager.GetAwsCredentials(credentials);
            var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);

            var request = new Amazon.EC2.Model.CreateNatGatewayRequest
            {
                                  SubnetId = subnetId,
                                  AllocationId = elasticIpAllocationId,
                              };

            Amazon.EC2.Model.NatGateway responseObject;
            using (var client = new AmazonEC2Client(awsCredentials, regionEndpoint))
            {
                var response = await client.CreateNatGatewayAsync(request);
                Validator.ThrowOnBadResult(request, response);

                responseObject = response.NatGateway;
            }

            var ret = new NatGateway { Id = responseObject.NatGatewayId, Name = natGatewayName, Region = regionName };

            await ret.TagNameInAwsAsync(credentials);

            return ret;
        }
    }
}
