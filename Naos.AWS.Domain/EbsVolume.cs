// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolume.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Elastic block storage volume model object.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ebs", Justification = "Spelling/name is correct.")]
    public class EbsVolume : IAwsObject
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the virtual name of the volume.
        /// </summary>
        public string VirtualName { get; set; }

        /// <summary>
        /// Gets or sets the device name of the volume (this controls drive letter in windows).
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the type of volume (SSD, magnetic, etc.)
        /// </summary>
        public string VolumeType { get; set; }

        /// <summary>
        /// Gets or sets the volume size in gigabytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Gb", Justification = "Spelling/name is correct.")]
        public int SizeInGb { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public EbsVolume DeepClone()
        {
            var ret = new EbsVolume()
                          {
                              DeviceName = this.DeviceName,
                              Id = this.Id,
                              Name = this.Name,
                              Region = this.Region,
                              SizeInGb = this.SizeInGb,
                              VirtualName = this.VirtualName,
                              VolumeType = this.VolumeType,
                          };

            return ret;
        }
    }
}
