// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitUntil.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Threading.Tasks;

    using Naos.AWS.Domain;

    using Instance = Naos.AWS.Domain.Instance;

    /// <summary>
    /// Class to run an action until some condition is reached.
    /// </summary>
    public static class WaitUntil
    {
        /// <summary>
        /// Tries to describe the object against AWS API and wait for non-null response.
        /// </summary>
        /// <param name="awsObject">AWS object to check on.</param>
        /// <param name="credentials">Credentials to use (will use the credentials from CredentialManager.Cached if null...).</param>
        /// <returns>Task for async/await</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "aws", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        public static async Task AwsObjectExists(IAwsObject awsObject, CredentialContainer credentials = null)
        {
            var awsObjectType = awsObject.InferObjectTypeFromId();
            switch (awsObjectType)
            {
                case AwsObjectType.Instance:
                    var objectAsInstance = (Instance)awsObject;
                    var timeToSleepInSecondsInstance = .25;
                    var successInstance = false;
                    while (!successInstance)
                    {
                        timeToSleepInSecondsInstance = timeToSleepInSecondsInstance * 2;
                        await Task.Delay(TimeSpan.FromSeconds(timeToSleepInSecondsInstance));
                        try
                        {
                            successInstance = await objectAsInstance.ExistsOnAwsAsync(credentials);
                        }
                        catch (Exception)
                        {
                            /* swallow exceptions on purpose...should add a throw after timeout */
                        }
                    }

                    break;
                case AwsObjectType.EbsVolume:
                    var objectAsVolume = (EbsVolume)awsObject;
                    var timeToSleepInSecondsVolume = .25;
                    var successVolume = false;
                    while (!successVolume)
                    {
                        timeToSleepInSecondsVolume = timeToSleepInSecondsVolume * 2;
                        await Task.Delay(TimeSpan.FromSeconds(timeToSleepInSecondsVolume));
                        try
                        {
                            successVolume = await objectAsVolume.ExistsOnAwsAsync(credentials);
                        }
                        catch (Exception)
                        {
                            /* swallow exceptions on purpose...should add a throw after timeout */
                        }
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        "Don't know how to check existence of AWS Object Type: " + awsObjectType);
            }
        }
    }
}
