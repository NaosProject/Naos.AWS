// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigSerializationTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain.Test
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Test the serialization of config.
    /// </summary>
    public class ConfigSerializationTest
    {
        [Fact]
        public void Region_should_serialize_to_xml()
        {
            // Arrange
            var serializer = new NaosXmlSerializer();
            var region = new ConfigEnvironment
                             {
                                 Name = "us-west-1",
                                 ElasticIps = new[] { new ConfigElasticIp { Name = "eip-Production3Nat@az1-usw1" } },
                                 InternetGateways = new[] { new ConfigInternetGateway { Name = "internetGateway-Production3@az1-usw1" } },
                                 Vpcs = new[]
                                            {
                                                new ConfigVpc
                                                    {
                                                        Name = "vpc-Production3@az1-usw1",
                                                        Cidr = "10.33.0.0/16",
                                                        Tenancy = "default",
                                                        InternetGatewayRef = "internetGateway-Production3@az1-usw1",
                                                        RouteTables =
                                                            new[]
                                                                {
                                                                    new ConfigRouteTable
                                                                        {
                                                                            Name =
                                                                                "routeTable-Production3Default@usw1",
                                                                            IsDefault = true,
                                                                            Routes = new ConfigRoute[0],
                                                                        },
                                                                    new ConfigRouteTable
                                                                        {
                                                                            Name =
                                                                                "routeTable-Production3Private@usw1",
                                                                            IsDefault = false,
                                                                            Routes = new[]
                                                                                         {
                                                                                             new ConfigRoute
                                                                                                 {
                                                                                                     DestinationRef
                                                                                                         = "AllTrafficCidr",
                                                                                                     TargetRef
                                                                                                         = "natGateway-Production3Nat@az1-usw1",
                                                                                                     Comment
                                                                                                         = "Send traffic bound for internet to NAT",
                                                                                                 },
                                                                                         },
                                                                        },
                                                                    new ConfigRouteTable
                                                                        {
                                                                            Name =
                                                                                "routeTable-Production3Public@usw1",
                                                                            IsDefault = false,
                                                                            Routes = new[]
                                                                                         {
                                                                                             new ConfigRoute
                                                                                                 {
                                                                                                     DestinationRef
                                                                                                         = "AllTrafficCidr",
                                                                                                     TargetRef
                                                                                                         = "internetGateway-Production3Nat@az1-usw1",
                                                                                                     Comment
                                                                                                         = "Send traffic bound for internet to Internet Gateway",
                                                                                                 },
                                                                                         },
                                                                        },
                                                                },
                                                        Subnets =
                                                            new[]
                                                                {
                                                                    new ConfigSubnet
                                                                        {
                                                                            Name = "subnet-Production3Nat@az1-usw1",
                                                                            AvailabilityZone = "us-west-1a",
                                                                            Cidr = "10.33.13.0/24",
                                                                            RouteTableRef =
                                                                                "routeTable-Production3Public@usw1",
                                                                        },
                                                                    new ConfigSubnet
                                                                        {
                                                                            Name = "subnet-Production3Public@az1-usw1",
                                                                            AvailabilityZone = "us-west-1a",
                                                                            Cidr = "10.33.12.0/24",
                                                                            RouteTableRef =
                                                                                "routeTable-Production3Public@usw1",
                                                                        },
                                                                    new ConfigSubnet
                                                                        {
                                                                            Name = "subnet-Production3Private@az1-usw1",
                                                                            AvailabilityZone = "us-west-1a",
                                                                            Cidr = "10.33.1.0/24",
                                                                            RouteTableRef =
                                                                                "routeTable-Production3Public@usw1",
                                                                        },
                                                                },
                                                        NatGateways =
                                                            new[]
                                                                {
                                                                    new ConfigNatGateway
                                                                        {
                                                                            Name =
                                                                                "natGateway-Production3Nat@az1-usw1",
                                                                            ElasticIpRef =
                                                                                "eip-Production3Nat@az1-usw1",
                                                                            SubnetRef =
                                                                                "subnet-Production3Nat@az1-usw1",
                                                                        },
                                                                },
                                                        NetworkAcls =
                                                            new[]
                                                                {
                                                                    new ConfigNetworkAcl
                                                                        {
                                                                            Name =
                                                                                "networkAcl-Production3Default@usw1",
                                                                            IsDefault = true,
                                                                            InboundRules =
                                                                                new ConfigNetworkAclInboundRule[0],
                                                                            OutboundRules =
                                                                                new ConfigNetworkAclOutboundRule[0],
                                                                        },
                                                                    new ConfigNetworkAcl
                                                                        {
                                                                            Name =
                                                                                "networkAcl-Production3Default@usw1",
                                                                            IsDefault = true,
                                                                            InboundRules =
                                                                                new[]
                                                                                    {
                                                                                        new
                                                                                            ConfigNetworkAclInboundRule
                                                                                                {
                                                                                                    RuleNumber
                                                                                                        =
                                                                                                        90,
                                                                                                    Type
                                                                                                        = "ALL Traffic",
                                                                                                    Protocol
                                                                                                        = "UDP",
                                                                                                    PortRange
                                                                                                        = "123",
                                                                                                    SourceRef
                                                                                                        = "AllTrafficCidr",
                                                                                                    Action
                                                                                                        = "ALLOW",
                                                                                                    Comment
                                                                                                        = "Allows inbound NTP Traffic",
                                                                                                },
                                                                                    },
                                                                            OutboundRules =
                                                                                new[]
                                                                                    {
                                                                                        new
                                                                                            ConfigNetworkAclOutboundRule
                                                                                                {
                                                                                                    RuleNumber
                                                                                                        =
                                                                                                        90,
                                                                                                    Type
                                                                                                        = "ALL Traffic",
                                                                                                    Protocol
                                                                                                        = "UDP",
                                                                                                    PortRange
                                                                                                        = "123",
                                                                                                    DestinationRef
                                                                                                        = "AllTrafficCidr",
                                                                                                    Action
                                                                                                        = "ALLOW",
                                                                                                    Comment
                                                                                                        = "Allows outbound NTP Traffic",
                                                                                                },
                                                                                    },
                                                                        },
                                                                },
                                                        SecurityGroups =
                                                            new[]
                                                                {
                                                                    new ConfigSecurityGroup
                                                                        {
                                                                            Name =
                                                                                "securityGroup-Production3Default@usw1",
                                                                            IsDefault = true,
                                                                            InboundRules =
                                                                                new[]
                                                                                    {
                                                                                        new
                                                                                            ConfigSecurityGroupInboundRule
                                                                                                {
                                                                                                    Protocol
                                                                                                        = "ALL",
                                                                                                    PortRange
                                                                                                        = "ALL",
                                                                                                    SourceRef
                                                                                                        = "AllTrafficCidr",
                                                                                                    Comment
                                                                                                        = "Allow all traffic and let Network Acls block.",
                                                                                                },
                                                                                    },
                                                                            OutboundRules =
                                                                                new[]
                                                                                    {
                                                                                        new
                                                                                            ConfigSecurityGroupOutboundRule
                                                                                                {
                                                                                                    Protocol
                                                                                                        = "ALL",
                                                                                                    PortRange
                                                                                                        = "ALL",
                                                                                                    DestinationRef
                                                                                                        = "AllTrafficCidr",
                                                                                                    Comment
                                                                                                        = "Allow all traffic and let Network Acls block.",
                                                                                                },
                                                                                    },
                                                                        },
                                                                },
                                                    },
                                            },
                             };

            // Act
            var result = serializer.SerializeToString(region);
            var actual = serializer.Deserialize<ConfigEnvironment>(result);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            actual.Should().NotBeNull();
        }
    }

    public class NaosXmlSerializer
    {
        public string SerializeToString<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        public T Deserialize<T>(string serialized)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(serialized);
            var ret = serializer.Deserialize(stringReader);
            return (T)ret;
        }
    }
}
