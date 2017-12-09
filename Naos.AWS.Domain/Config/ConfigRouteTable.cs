// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigRouteTable.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="RouteTable" />.
    /// </summary>
    [XmlRoot("routeTable")]
    [XmlType("routeTable")]
    [Serializable]
    public class ConfigRouteTable : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not it's the default.
        /// </summary>
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute("routeTableId")]
        public string RouteTableId { get; set; }

        /// <summary>
        /// Gets or sets the routes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Keeping arrays due to serialization constraints.")]
        [XmlArray("routes")]
        [XmlArrayItem("route", typeof(ConfigRoute))]
        public ConfigRoute[] Routes { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.RouteTableId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}