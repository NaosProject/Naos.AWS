// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitUntil.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using Naos.AWS.Domain;
    using Naos.CodeAnalysis.Recipes;
    using static System.FormattableString;

    /// <summary>
    /// Class to run an action until some condition is reached.
    /// </summary>
    public static class WaitUntil
    {
        /// <summary>
        /// Tries to describe the object against AWS API and wait for non-null response.
        /// </summary>
        /// <param name="awsObject">AWS object to check on.</param>
        /// <param name="timeout">Optional timeout - default is infinity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task AwsObjectExists(IAwsObject awsObject, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            var awsObjectType = awsObject.InferObjectTypeFromId();
            Func<Task<bool>> successFunc;
            switch (awsObjectType)
            {
                case AwsObjectType.Instance:
                    var objectAsInstance = (Instance)awsObject;
                    successFunc = async () => await objectAsInstance.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.EbsVolume:
                    var objectAsVolume = (EbsVolume)awsObject;
                    successFunc = async () => await objectAsVolume.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.Vpc:
                    var objectAsVpc = (Vpc)awsObject;
                    successFunc = async () => await objectAsVpc.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.InternetGateway:
                    var objectAsInternetGateway = (InternetGateway)awsObject;
                    successFunc = async () => await objectAsInternetGateway.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.RouteTable:
                    var objectAsRouteTable = (RouteTable)awsObject;
                    successFunc = async () => await objectAsRouteTable.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.Subnet:
                    var objectAsSubnet = (Subnet)awsObject;
                    successFunc = async () => await objectAsSubnet.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.NetworkAcl:
                    var objectAsNetworkAcl = (NetworkAcl)awsObject;
                    successFunc = async () => await objectAsNetworkAcl.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.SecurityGroup:
                    var objectAsSecurityGroup = (SecurityGroup)awsObject;
                    successFunc = async () => await objectAsSecurityGroup.ExistsOnAwsAsync(credentials);
                    break;
                case AwsObjectType.NatGateway:
                    var objectAsNatGateway = (NatGateway)awsObject;
                    successFunc = async () => await objectAsNatGateway.ExistsOnAwsAsync(credentials);
                    break;
                default:
                    throw new NotSupportedException(
                        "Don't know how to check existence of AWS Object Type: " + awsObjectType);
            }

            async Task<bool> Operation()
            {
                return await successFunc();
            }

            bool SuccessCheck(bool currentState)
            {
                return currentState;
            }

            void ThrowOnUnexpected(bool currentState)
            {
                /* no-op */
            }

            await CriteriaMet(Operation, SuccessCheck, ThrowOnUnexpected, timeout);
        }

        /// <summary>
        /// Block until the provided <see cref="NatGateway" /> is in the specified state.  Can optionally specify states that are unexpected and will trigger an exception.
        /// </summary>
        /// <param name="natGateway">Gateway to check.</param>
        /// <param name="expectedState">Expected state to wait for.</param>
        /// <param name="unexpectedStates">Optional unexpected states to throw on.</param>
        /// <param name="timeout">Optional timeout - default is infinity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InState", Justification = "Spelling/name is correct.")]
        public static async Task NatGatewayInState(NatGateway natGateway, NatGatewayState expectedState, IReadOnlyCollection<NatGatewayState> unexpectedStates = null, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            var localUnexpectedStates = unexpectedStates ?? new List<NatGatewayState>();

            async Task<NatGatewayState> Operation()
            {
                var currentState = await natGateway.GetState(credentials);
                return currentState;
            }

            bool SuccessCheck(NatGatewayState currentState)
            {
                return currentState == expectedState;
            }

            void ThrowOnUnexpected(NatGatewayState currentState)
            {
                if (localUnexpectedStates.Contains(currentState))
                {
                    throw new ArgumentException(Invariant($"Unexpected state ({currentState}) reached on {nameof(NatGateway)} {natGateway.Name} ({natGateway.Id})."));
                }
            }

            await CriteriaMet(Operation, SuccessCheck, ThrowOnUnexpected, timeout);
        }

        /// <summary>
        /// Waits until the instance gets to a certain expected state.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="expectedState">State of instance to wait for.</param>
        /// <param name="unexpectedStates">Optional unexpected states to throw on.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is infinity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InState", Justification = "Spelling/name is correct.")]
        public static async Task InstanceInState(Instance instance, InstanceState expectedState, IReadOnlyCollection<InstanceState> unexpectedStates = null, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            var localUnexpectedStates = unexpectedStates ?? new List<InstanceState>();

            async Task<InstanceState> Operation()
            {
                var status = await instance.GetStatusAsync(credentials);
                return status.InstanceState;
            }

            bool SuccessCheck(InstanceState currentState)
            {
                return currentState == expectedState;
            }

            void ThrowOnUnexpected(InstanceState currentState)
            {
                if (localUnexpectedStates.Contains(currentState))
                {
                    throw new ArgumentException(Invariant($"Unexpected state ({currentState}) reached on {nameof(Instance)} {instance.Name} ({instance.Id})."));
                }
            }

            await CriteriaMet(Operation, SuccessCheck, ThrowOnUnexpected, timeout);
        }

        /// <summary>
        /// Waits until the volume gets to a certain expected state.
        /// </summary>
        /// <param name="volume">EbsVolume to operate on.</param>
        /// <param name="expectedStatus">State of volume to wait for.</param>
        /// <param name="unexpectedStates">Optional unexpected states to throw on.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is infinity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = NaosSuppressBecause.CA1704_IdentifiersShouldBeSpelledCorrectly_SpellingIsCorrectInContextOfTheDomain)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InState", Justification = "Spelling/name is correct.")]
        public static async Task EbsVolumeInState(EbsVolume volume, EbsVolumeStatus expectedStatus, IReadOnlyCollection<EbsVolumeStatus> unexpectedStates = null, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            var localUnexpectedStates = unexpectedStates ?? new List<EbsVolumeStatus>();

            async Task<EbsVolumeStatus> Operation()
            {
                var status = await volume.GetStatusAsync(credentials);
                return status;
            }

            bool SuccessCheck(EbsVolumeStatus currentState)
            {
                return currentState == expectedStatus;
            }

            void ThrowOnUnexpected(EbsVolumeStatus currentState)
            {
                if (localUnexpectedStates.Contains(currentState))
                {
                    throw new ArgumentException(Invariant($"Unexpected state ({currentState}) reached on {nameof(EbsVolume)} {volume.Name} ({volume.Id})."));
                }
            }

            await CriteriaMet(Operation, SuccessCheck, ThrowOnUnexpected, timeout);
        }

        /// <summary>
        /// Waits until the instance has successful status checks.
        /// </summary>
        /// <param name="instance">Instance to operate on.</param>
        /// <param name="timeout">Optional timeout to wait until object exists; DEFAULT is ininity.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await.</returns>
        public static async Task SuccessfulChecks(Instance instance, TimeSpan timeout = default(TimeSpan), CredentialContainer credentials = null)
        {
            async Task<bool> Operation()
            {
                var instanceStatus = await instance.GetStatusAsync(credentials);
                var success = instanceStatus.SystemChecks.All(_ => _.Value == CheckState.Passed)
                              && instanceStatus.InstanceChecks.All(_ => _.Value == CheckState.Passed);
                return success;
            }

            bool SuccessCheck(bool currentState)
            {
                return currentState;
            }

            void ThrowOnUnexpected(bool currentState)
            {
                /* no-op */
            }

            await CriteriaMet(Operation, SuccessCheck, ThrowOnUnexpected, timeout);
        }

        private static async Task CriteriaMet<T>(Func<Task<T>> operation, Func<T, bool> successCheck, Action<T> throwOnUnexpected, TimeSpan timeout)
        {
            var timeToSleepInSeconds = .25;
            DateTime? expiration = null;
            if (timeout != default(TimeSpan))
            {
                expiration = DateTime.UtcNow.Add(timeout);
            }

            while (true)
            {
                timeToSleepInSeconds = timeToSleepInSeconds * 2;
                await Task.Delay(TimeSpan.FromSeconds(timeToSleepInSeconds));

                T currentState = default(T);
                try
                {
                    currentState = await operation();
                    var success = successCheck(currentState);
                    if (success)
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    if (expiration != null && expiration < DateTime.UtcNow)
                    {
                        throw;
                    }
                }

                throwOnUnexpected(currentState);

                if (expiration != null && expiration < DateTime.UtcNow)
                {
                    throw new TimeoutException(Invariant($"Timeout {timeout} exceeded."));
                }
            }
        }
    }
}
