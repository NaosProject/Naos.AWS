// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigInternetGateway.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a <see cref="InternetGateway" />.
    /// </summary>
    [XmlRoot("internetGateway")]
    [XmlType("internetGateway")]
    [Serializable]
    public class ConfigInternetGateway : IHaveName
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute("internetGatewayId")]
        public string InternetGatewayId { get; set; }

        /// <summary>
        /// Updates the ID.
        /// </summary>
        /// <param name="id">ID to update with.</param>
        public void UpdateId(string id)
        {
            this.InternetGatewayId = id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}