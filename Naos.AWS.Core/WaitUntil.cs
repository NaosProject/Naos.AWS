﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitUntil.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.EC2;

    using Naos.AWS.Contract;

    using Instance = Naos.AWS.Contract.Instance;

    /// <summary>
    /// Class to run an action until some condition is reached.
    /// </summary>
    public class WaitUntil
    {
        /// <summary>
        /// Runs the provided function until true is returned.
        /// </summary>
        /// <param name="function">The code to execute.</param>
        /// <param name="swallowExceptions">Optional value to indicate whether or not to ignore errors (default is TRUE).</param>
        /// <returns>Task for async/await</returns>
        public static async Task SuccessIsReturned(Task<bool> function, bool swallowExceptions = true)
        {
            try
            {
                var timeToSleepInSeconds = .25;
                var success = false;
                while (!success)
                {
                    timeToSleepInSeconds = timeToSleepInSeconds * 2;
                    Thread.Sleep(TimeSpan.FromSeconds(timeToSleepInSeconds));
                    success = await function;
                }
            }
            catch (Exception)
            {
                if (!swallowExceptions)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Tries to describe the object against AWS API and wait for non-null response.
        /// </summary>
        /// <param name="awsObject">AWS object to check on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        public static async Task AwsObjectExists(IAwsObject awsObject, CredentialContainer credentials = null)
        {
            Task<bool> action = Task.Run(
                () =>
                    {
                        var awsObjectType = awsObject.InferObjectTypeFromId();
                        switch (awsObjectType)
                        {
                            case AwsObjectType.Instance:
                                return (awsObject as Instance).ExistsOnAwsAsync(credentials);
                            case AwsObjectType.EbsVolume:
                                return (awsObject as EbsVolume).ExistsOnAwsAsync(credentials);
                            default:
                                throw new NotSupportedException(
                                    "Don't know how to check existence of AWS Object Type: " + awsObjectType);
                        }
                    });

            await SuccessIsReturned(action);
        }
    }
}
