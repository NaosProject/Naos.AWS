// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CredentialExtensionMethodsTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core.Test
{
    using System;

    using Amazon.SecurityToken.Model;

    using Naos.AWS.Domain;

    using Xunit;

    public static class CredentialExtensionMethodsTest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
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
