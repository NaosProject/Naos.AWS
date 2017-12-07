// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceWithStatus.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Instance model object with status.
    /// </summary>
    public class InstanceWithStatus : Instance
    {
        /// <summary>
        /// Gets or sets the status of the instance.
        /// </summary>
        public InstanceStatus InstanceStatus { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public new InstanceWithStatus DeepClone()
        {
            var ret = new InstanceWithStatus()
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
                              InstanceStatus = this.InstanceStatus.DeepClone(),
                          };

            return ret;
        }
    }
}
