// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolumeExtensionMethodsTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core.Test
{
    using System;
    using System.Linq;

    using Naos.AWS.Contract;

    using Xunit;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
    public static class EbsVolumeExtensionMethodsTest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeCollection_ValidBlockDeviceMappingList()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 1000,
                                          VirtualName = "OS",
                                          VolumeType = "gp2",
                                      },
                              }.ToList();

            var blockMappings = volumes.ToAwsBlockDeviceMappings();

            Assert.Equal(volumes.Count, blockMappings.Count);
            Assert.Equal(volumes.First().DeviceName, blockMappings.First().DeviceName);
            Assert.Equal(volumes.First().VirtualName, blockMappings.First().VirtualName);
            Assert.Equal(volumes.First().SizeInGb, blockMappings.First().Ebs.VolumeSize);
            Assert.Equal(volumes.First().VolumeType, blockMappings.First().Ebs.VolumeType);
            Assert.Equal(true, blockMappings.First().Ebs.DeleteOnTermination);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iops", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeWithIopsAndNoLevelSpecified_GetsThirtyToOne()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 1000,
                                          VirtualName = "OS",
                                          VolumeType = "io1",
                                      },
                              }.ToList();

            var blockMappings = volumes.ToAwsBlockDeviceMappings();

            Assert.Equal(volumes.Single().VolumeType, blockMappings.Single().Ebs.VolumeType);
            Assert.Equal(volumes.Single().SizeInGb * 30, blockMappings.Single().Ebs.Iops);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iops", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeWithIopsAndLevelSpecified_GetsSpecified()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 150,
                                          VirtualName = "OS",
                                          VolumeType = "io1-4000",
                                      },
                              }.ToList();

            var blockMappings = volumes.ToAwsBlockDeviceMappings();

            Assert.Equal("io1", blockMappings.Single().Ebs.VolumeType);
            Assert.Equal(4000, blockMappings.Single().Ebs.Iops);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iops", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeWithIopsAndLevelSpecifiedOverThirtyToOne_Throws()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 1,
                                          VirtualName = "OS",
                                          VolumeType = "io1-300",
                                      },
                              }.ToList();

            Action testCode = () => volumes.ToAwsBlockDeviceMappings();
            var ex = Assert.Throws<ArgumentException>(testCode);
            Assert.Equal("Specified IOPS: 300 was greated than allowed (30 IOPS:1GB): 30", ex.Message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iops", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gp", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeWithGp2_GetsZeroIops()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 1,
                                          VirtualName = "OS",
                                          VolumeType = "gp2",
                                      },
                              }.ToList();

            var mappings = volumes.ToAwsBlockDeviceMappings();
            Assert.Equal(volumes.Single().VolumeType, mappings.Single().Ebs.VolumeType);
            Assert.Equal(0, mappings.Single().Ebs.Iops);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Iops", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
        [Fact]
        public static void ToAwsBlockDeviceMappings_EbsVolumeWithStandard_GetsZeroIops()
        {
            var volumes = new[]
                              {
                                  new EbsVolume()
                                      {
                                          DeviceName = "MonkeysDevice",
                                          Id = "ebs--someId",
                                          Name = "IamTheEbsVolume",
                                          Region = "us-south-42",
                                          SizeInGb = 1,
                                          VirtualName = "OS",
                                          VolumeType = "standard",
                                      },
                              }.ToList();

            var mappings = volumes.ToAwsBlockDeviceMappings();
            Assert.Equal(volumes.Single().VolumeType, mappings.Single().Ebs.VolumeType);
            Assert.Equal(0, mappings.Single().Ebs.Iops);
        }
    }
}
