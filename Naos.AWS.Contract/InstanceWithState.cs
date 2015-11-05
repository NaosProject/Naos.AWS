// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceWithState.cs" company="Naos">
//   Copyright 2015 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Instance model object.
    /// </summary>
    public class InstanceWithState : Instance
    {
        /// <summary>
        /// Gets or sets the state of the instance.
        /// </summary>
        public InstanceState InstanceState { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public new InstanceWithState DeepClone()
        {
            var ret = new InstanceWithState()
                          {
                              Ami = this.Ami.DeepClone(),
                              ComputerName = this.ComputerName,
                              DisableApiTermination = this.DisableApiTermination,
                              ContainingSubnet = this.ContainingSubnet.DeepClone(),
                              ElasticIp = this.ElasticIp == null ? null : this.ElasticIp.DeepClone(),
                              EnableSourceDestinationCheck = this.EnableSourceDestinationCheck,
                              Id = this.Id,
                              InstanceType = this.InstanceType,
                              Key = this.Key == null ? null : this.Key.DeepClone(),
                              MappedVolumes = this.MappedVolumes.Select(_ => _.DeepClone()).ToList(),
                              Name = this.Name,
                              PrivateIpAddress = this.PrivateIpAddress,
                              Region = this.Region,
                              SecurityGroup =
                                  this.SecurityGroup == null ? null : this.SecurityGroup.DeepClone(),
                              Tags =
                                  (this.Tags ?? new Dictionary<string, string>()).ToDictionary(
                                      keyInput => keyInput.Key,
                                      valueInput => valueInput.Value),
                              InstanceState = this.InstanceState
                          };

            return ret;
        }
    }
}
