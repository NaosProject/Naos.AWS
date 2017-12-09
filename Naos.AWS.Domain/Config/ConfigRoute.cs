// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigRoute.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="RouteEntry" />.
    /// </summary>
    [XmlRoot("route")]
    [XmlType("route")]
    [Serializable]
    public class ConfigRoute
    {
        /// <summary>
        /// Gets or sets the destination name.
        /// </summary>
        [XmlAttribute("destinationRef")]
        public string DestinationRef { get; set; }

        /// <summary>
        /// Gets or sets the target name.
        /// </summary>
        [XmlAttribute("targetRef")]
        public string TargetRef { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        [XmlAttribute("comment")]
        public string Comment { get; set; }
    }
}