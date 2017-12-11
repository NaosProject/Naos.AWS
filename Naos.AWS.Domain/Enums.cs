// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Type of credential.
    /// </summary>
    public enum CredentialType
    {
        /// <summary>
        /// Token based AWS authorization.
        /// </summary>
        Token,

        /// <summary>
        /// Key only based AWS authorization.
        /// </summary>
        Keys,
    }

    /// <summary>
    /// Type of routable item in AWS.
    /// </summary>
    public enum RoutableType
    {
        /// <summary>
        /// And EC2 instance.
        /// </summary>
        Instance,

        /// <summary>
        /// Peered VPC connection (another VPC).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        VpcPeering,

        /// <summary>
        /// Internet gateway of the VPC (out to public internet).
        /// </summary>
        InternetGateway,

        /// <summary>
        /// Virtual private gateway (VPN connection to a resource outside of AWS).
        /// </summary>
        VirtualPrivateGateway,
    }

    /// <summary>
    /// Actions available to rules.
    /// </summary>
    public enum RuleAction
    {
        /// <summary>
        /// Allows the matching request.
        /// </summary>
        Allow,

        /// <summary>
        /// Denies the matching request.
        /// </summary>
        Deny,
    }

    /// <summary>
    /// Behavior for AMI searching when multiple AMI's are found.
    /// </summary>
    public enum MultipleAmiFoundBehavior
    {
        /// <summary>
        /// Throw an exception.
        /// </summary>
        Throw,

        /// <summary>
        /// Sort descending by name and take first item.
        /// </summary>
        FirstSortedDescending,
    }

    /// <summary>
    /// States an instance can be in.
    /// </summary>
    public enum InstanceState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Pending state.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Running state.
        /// </summary>
        Running = 16,

        /// <summary>
        /// Shutting down state.
        /// </summary>
        ShuttingDown = 32,

        /// <summary>
        /// Terminated state.
        /// </summary>
        Terminated = 48,

        /// <summary>
        /// Stopping state.
        /// </summary>
        Stopping = 64,

        /// <summary>
        /// Stopped state.
        /// </summary>
        Stopped = 80,
    }

    /// <summary>
    /// State a check can be in.
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// Unknown state.
        /// </summary>
        Unknown,

        /// <summary>
        /// Initializing state.
        /// </summary>
        Initializing,

        /// <summary>
        /// Insufficient data state.
        /// </summary>
        InsufficientData,

        /// <summary>
        /// Failed state.
        /// </summary>
        Failed,

        /// <summary>
        /// Passed state.
        /// </summary>
        Passed,
    }

    /// <summary>
    /// Types of AWS objects.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    public enum AwsObjectType
    {
        /// <summary>
        /// Virtual private cloud.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vpc", Justification = "Spelling/name is correct.")]
        Vpc,

        /// <summary>
        /// Internet gateway.
        /// </summary>
        InternetGateway,

        /// <summary>
        /// Route table.
        /// </summary>
        RouteTable,

        /// <summary>
        /// Subnet object.
        /// </summary>
        Subnet,

        /// <summary>
        /// Network access control list.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Acl", Justification = "Spelling/name is correct.")]
        NetworkAcl,

        /// <summary>
        /// Security group.
        /// </summary>
        SecurityGroup,

        /// <summary>
        /// Machine instance.
        /// </summary>
        Instance,

        /// <summary>
        /// Elastic block storage volume.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        EbsVolume,

        /// <summary>
        /// Elastic IP.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        ElasticIp,

        /// <summary>
        /// NAT Gateway.
        /// </summary>
        NatGateway,
    }

    /// <summary>
    /// Enumeration of the types of entries you can have.
    /// </summary>
    public enum Route53EntryType
    {
        /// <summary>
        /// A record.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "A", Justification = "Spelling/name is correct.")]
        A,

        /// <summary>
        /// CNAME record.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CNAME", Justification = "Spelling/name is correct.")]
        CNAME,

        /// <summary>
        /// MX record.
        /// </summary>
        MX,

        /// <summary>
        /// TXT record.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "TXT", Justification = "Spelling/name is correct.")]
        TXT,

        /// <summary>
        /// NS record.
        /// </summary>
        NS,

        /// <summary>
        /// SOA record.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SOA", Justification = "Spelling/name is correct.")]
        SOA,
    }
}
