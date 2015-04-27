// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbsVolume.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    /// <summary>
    /// Elastic block storage volume model object.
    /// </summary>
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
        public int SizeInGb { get; set; }
    }
}