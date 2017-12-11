// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAbstraction.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Console
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml.Serialization;

    using CLAP;

    using Naos.AWS.Core;
    using Naos.AWS.Domain;
    using Naos.Logging.Domain;
    using Naos.Recipes.Configuration.Setup;
    using Naos.Recipes.RunWithRetry;
    using Naos.Serialization.Factory;

    using Spritely.Recipes;

    /// <summary>
    /// Real logic.
    /// </summary>
    public class ConsoleAbstraction : ConsoleAbstractionBase
    {
        /// <summary>
        /// Creates a new environment.
        /// </summary>
        /// <param name="credentialsJson">Credentials for the computing platform provider to use in JSON.</param>
        /// <param name="configFilePath">ConfigEnvironment object serialized as XML file path.</param>
        /// <param name="debug">A value indicating whether or not to launch the debugger.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "All disposables are disposed.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is fine.")]
        [Verb(Aliases = "new", Description = "Creates a new environment.")]
        public static void CreateEnvironment(
            [Aliases("creds")] [Required] [Description("Credentials for the computing platform provider to use in JSON.")] string credentialsJson,
            [Required] [Aliases("file")] [Description("Configuration file path for environment.")] string configFilePath,
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug)
        {
            CommonSetup(debug, logProcessorSettings: new LogProcessorSettings(new[] { new ConsoleLogConfiguration(LogContexts.All, LogContexts.None), }));

            new { credentialsJson }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { configFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var serializer = SerializerFactory.Instance.BuildSerializer(Config.ConfigFileSerializationDescription);
            var updatedConfigFilePath = Path.ChangeExtension(configFilePath, ".created.xml") ?? (configFilePath + ".created");
            var configFileXml = File.ReadAllText(configFilePath);
            var xmlSerializer = new XmlSerializer(typeof(ConfigEnvironment));
            ConfigEnvironment configuration;
            using (var stringReader = new StringReader(configFileXml))
            {
                configuration = (ConfigEnvironment)xmlSerializer.Deserialize(stringReader);
            }

            if (configuration.Vpcs.Length > 1)
            {
                throw new ArgumentException("Cannot create more than one VPC for use with an arcology.");
            }

            void UpdateCallback(ConfigEnvironment environment)
            {
                using (var stringWriter = new StringWriter(CultureInfo.CurrentCulture))
                {
                    xmlSerializer.Serialize(stringWriter, environment);
                    File.WriteAllText(updatedConfigFilePath, stringWriter.ToString());
                }
            }

            var credentials = serializer.Deserialize<CredentialContainer>(credentialsJson);
            var populatedEnvironment = Run.TaskUntilCompletion(Creator.CreateEnvironment(credentials, configuration, UpdateCallback));
            populatedEnvironment.Named("ReturnedConfigEnvironmentShouldNeverBeNull").Must().NotBeNull().OrThrowFirstFailure();
        }

        /// <summary>
        /// Removes a new environment.
        /// </summary>
        /// <param name="credentialsJson">Credentials for the computing platform provider to use in JSON.</param>
        /// <param name="configFilePath">ConfigEnvironment object serialized as XML file path.</param>
        /// <param name="debug">A value indicating whether or not to launch the debugger.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is fine.")]
        [Verb(Aliases = "remove", Description = "Removes an environment.")]
        public static void RemoveEnvironment(
            [Aliases("creds")] [Required] [Description("Credentials for the computing platform provider to use in JSON.")] string credentialsJson,
            [Required] [Aliases("file")] [Description("Configuration file path for environment.")] string configFilePath,
            [Aliases("")] [Description("Launches the debugger.")] [DefaultValue(false)] bool debug)
        {
            CommonSetup(debug, logProcessorSettings: new LogProcessorSettings(new[] { new ConsoleLogConfiguration(LogContexts.All, LogContexts.None), }));

            new { credentialsJson }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();
            new { configFilePath }.Must().NotBeNull().And().NotBeWhiteSpace().OrThrowFirstFailure();

            var serializer = SerializerFactory.Instance.BuildSerializer(Config.ConfigFileSerializationDescription);
            var updatedConfigFilePath = Path.ChangeExtension(configFilePath, ".removed.xml") ?? (configFilePath + ".removed");
            var configFileXml = File.ReadAllText(configFilePath);
            var xmlSerializer = new XmlSerializer(typeof(ConfigEnvironment));
            ConfigEnvironment configuration;
            using (var stringReader = new StringReader(configFileXml))
            {
                configuration = (ConfigEnvironment)xmlSerializer.Deserialize(stringReader);
            }

            if (configuration.Vpcs.Length > 1)
            {
                throw new ArgumentException("Cannot create more than one VPC for use with an arcology.");
            }

            void UpdateCallback(ConfigEnvironment environment)
            {
                using (var stringWriter = new StringWriter(CultureInfo.CurrentCulture))
                {
                    xmlSerializer.Serialize(stringWriter, environment);
                    File.WriteAllText(updatedConfigFilePath, stringWriter.ToString());
                }
            }

            var credentials = serializer.Deserialize<CredentialContainer>(credentialsJson);
            var populatedEnvironment = Run.TaskUntilCompletion(Destroyer.RemoveEnvironment(credentials, configuration, UpdateCallback));
            populatedEnvironment.Named("ReturnedConfigEnvironmentShouldNeverBeNull").Must().NotBeNull().OrThrowFirstFailure();
        }
    }
}