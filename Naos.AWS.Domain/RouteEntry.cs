// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteEntry.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Route table entry model object.
    /// </summary>
    public class RouteEntry
    {
        /// <summary>
        /// Gets or sets the destination CIDR to send to the routable entity.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cidr", Justification = "Spelling/name is correct.")]
        public string DestinationCidr { get; set; }

        /// <summary>
        /// Gets or sets the type of target.
        /// </summary>
        public RoutableType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the target ID.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets a deep clone of the object.
        /// </summary>
        /// <returns>Deeply cloned version of the object.</returns>
        public RouteEntry DeepClone()
        {
            var ret = new RouteEntry()
                          {
                              DestinationCidr = this.DestinationCidr,
                              TargetId = this.TargetId,
                              TargetType = this.TargetType,
                          };

            return ret;
        }
    }
}
