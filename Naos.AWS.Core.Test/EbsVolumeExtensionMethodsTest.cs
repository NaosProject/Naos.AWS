// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolumeExtensionMethodsTest.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core.Test
{
    using System.Linq;

    using Naos.AWS.Contract;

    using Xunit;

    public class EbsVolumeExtensionMethodsTest
    {
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
                                          VolumeType = "gp2"
                                      }
                              }.ToList();

            var blockMappings = volumes.ToAwsBlockDeviceMappings();

            Assert.Equal(volumes.Count, blockMappings.Count);
            Assert.Equal(volumes.First().DeviceName, blockMappings.First().DeviceName);
            Assert.Equal(volumes.First().VirtualName, blockMappings.First().VirtualName);
            Assert.Equal(volumes.First().SizeInGb, blockMappings.First().Ebs.VolumeSize);
            Assert.Equal(volumes.First().VolumeType, blockMappings.First().Ebs.VolumeType);
            Assert.Equal(true, blockMappings.First().Ebs.DeleteOnTermination);
        }
    }
}
