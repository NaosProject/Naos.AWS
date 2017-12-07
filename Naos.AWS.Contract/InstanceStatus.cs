// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceStatus.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Contract
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Instance status model object.
    /// </summary>
    public class InstanceStatus
    {
        /// <summary>
        /// Gets or sets the state of the instance.
        /// </summary>
        public InstanceState InstanceState { get; set; }

        /// <summary>
        /// Gets or sets the system checks.
        /// </summary>
        public IReadOnlyDictionary<string, CheckState> SystemChecks { get; set; }

        /// <summary>
        /// Gets or sets the instance checks.
        /// </summary>
        public IReadOnlyDictionary<string, CheckState> InstanceChecks { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public InstanceStatus DeepClone()
        {
            var ret = new InstanceStatus
                          {
                              InstanceState = this.InstanceState,
                              SystemChecks = (this.SystemChecks ?? new Dictionary<string, CheckState>()).ToDictionary(key => key.Key, value => value.Value),
                              InstanceChecks = (this.InstanceChecks ?? new Dictionary<string, CheckState>()).ToDictionary(key => key.Key, value => value.Value),
                          };
            return ret;
        }
    }
}
