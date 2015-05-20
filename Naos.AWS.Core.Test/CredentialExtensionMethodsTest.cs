// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialExtensionMethodsTest.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core.Test
{
    using System;

    using Amazon.SecurityToken.Model;

    using Naos.AWS.Contract;

    using Xunit;

    public class CredentialExtensionMethodsTest
    {
        [Fact]
        public static void ToAwsCredentials_TokenCredentials_ValidAwsCredentials()
        {
            var credentialContainer = new CredentialContainer()
                                          {
                                              CredentialType = CredentialType.Token,
                                              AccessKeyId = "AccessKeyId",
                                              SecretAccessKey = "SecretAccessKey",
                                              SessionToken = "SessionToken",
                                              Expiration = DateTime.Now.AddDays(1),
                                          };

            var awsCredentials = credentialContainer.ToAwsCredentials();

            var tokenCredentails = awsCredentials as Credentials;

            Assert.NotNull(tokenCredentails);
            Assert.Equal(credentialContainer.AccessKeyId, tokenCredentails.AccessKeyId);
            Assert.Equal(credentialContainer.SecretAccessKey, tokenCredentails.SecretAccessKey);
            Assert.Equal(credentialContainer.SessionToken, tokenCredentails.SessionToken);
            Assert.Equal(credentialContainer.Expiration, tokenCredentails.Expiration);
        }
    }
}
