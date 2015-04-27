// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Enums.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Class to hold enumerations used by project.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Type of credential.
        /// </summary>
        public enum CredentialType
        {
            /// <summary>
            /// Token based AWS authorization.
            /// </summary>
            Token
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
            FirstSortedDescending
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
        /// Types of AWS objects.
        /// </summary>
        public enum AwsObjectType
        {
            /// <summary>
            /// Virtual private cloud.
            /// </summary>
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
            EbsVolume,
        }
    }
}
