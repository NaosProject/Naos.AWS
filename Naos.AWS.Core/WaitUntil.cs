// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitUntil.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Naos.AWS.Contract;

    using Instance = Naos.AWS.Contract.Instance;

    /// <summary>
    /// Class to run an action until some condition is reached.
    /// </summary>
    public class WaitUntil
    {
        /// <summary>
        /// Tries to describe the object against AWS API and wait for non-null response.
        /// </summary>
        /// <param name="awsObject">AWS object to check on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        public static async Task AwsObjectExists(IAwsObject awsObject, CredentialContainer credentials = null)
        {
            var awsObjectType = awsObject.InferObjectTypeFromId();
            switch (awsObjectType)
            {
                case AwsObjectType.Instance:
                    try
                    {
                        var timeToSleepInSeconds = .25;
                        var success = false;
                        while (!success)
                        {
                            timeToSleepInSeconds = timeToSleepInSeconds * 2;
                            Thread.Sleep(TimeSpan.FromSeconds(timeToSleepInSeconds));
                            var objectAsInstance = (Instance)awsObject;
                            success = await objectAsInstance.ExistsOnAwsAsync(credentials);
                        }
                    }
                    catch (Exception)
                    {
                        /* swallow exceptions on purpose... */
                    }

                    break;
                case AwsObjectType.EbsVolume:
                    try
                    {
                        var timeToSleepInSeconds = .25;
                        var success = false;
                        while (!success)
                        {
                            timeToSleepInSeconds = timeToSleepInSeconds * 2;
                            Thread.Sleep(TimeSpan.FromSeconds(timeToSleepInSeconds));
                            var objectAsVolume = (EbsVolume)awsObject;
                            success = await objectAsVolume.ExistsOnAwsAsync(credentials);
                        }
                    }
                    catch (Exception)
                    {
                        /* swallow exceptions on purpose... */
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        "Don't know how to check existence of AWS Object Type: " + awsObjectType);
            }
        }
    }
}
