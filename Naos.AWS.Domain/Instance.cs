// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Instance.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Instance model object.
    /// </summary>
    public class Instance : IAwsObject
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
        /// Gets or sets the computer's name (in windows).
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// Gets or sets the instance type.
        /// </summary>
        public string InstanceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to allow routing to different IP's other than it's own (this MUST be false for a NAT instance).
        /// </summary>
        public bool EnableSourceDestinationCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to allow termination through the SDK/API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api", Justification = "Name/spelling is correct.")]
        public bool DisableApiTermination { get; set; }

        /// <summary>
        /// Gets or sets the private IP address of the instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public string PrivateIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the AMI used to create the instance.
        /// </summary>
        public Ami Ami { get; set; }

        /// <summary>
        /// Gets or sets the elastic IP of the instance (if applicable).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ip", Justification = "Spelling/name is correct.")]
        public ElasticIp ElasticIp { get; set; }

        /// <summary>
        /// Gets or sets the encryption key pair of the instance.
        /// </summary>
        public KeyPair Key { get; set; }

        /// <summary>
        /// Gets or sets the containing subnet.
        /// </summary>
        public Subnet ContainingSubnet { get; set; }

        /// <summary>
        /// Gets or sets the security group to apply to the instance.
        /// </summary>
        public SecurityGroup SecurityGroup { get; set; }

        /// <summary>
        /// Gets or sets the EBS volumes mapped to the instance.
        /// </summary>
        public IReadOnlyCollection<EbsVolume> MappedVolumes { get; set; }

        /// <summary>
        /// Gets or sets the tags as a dictionary of name and value.
        /// </summary>
        public IReadOnlyDictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public Instance DeepClone()
        {
            var ret = new Instance()
                          {
                              Ami = this.Ami.DeepClone(),
                              ComputerName = this.ComputerName,
                              DisableApiTermination = this.DisableApiTermination,
                              ContainingSubnet = this.ContainingSubnet.DeepClone(),
                              ElasticIp = this.ElasticIp?.DeepClone(),
                              EnableSourceDestinationCheck = this.EnableSourceDestinationCheck,
                              Id = this.Id,
                              InstanceType = this.InstanceType,
                              Key = this.Key?.DeepClone(),
                              MappedVolumes = this.MappedVolumes.Select(_ => _.DeepClone()).ToList(),
                              Name = this.Name,
                              PrivateIpAddress = this.PrivateIpAddress,
                              Region = this.Region,
                              SecurityGroup = this.SecurityGroup?.DeepClone(),
                              Tags =
                                  (this.Tags ?? new Dictionary<string, string>()).ToDictionary(
                                      keyInput => keyInput.Key,
                                      valueInput => valueInput.Value),
                          };

            return ret;
        }
    }
}
