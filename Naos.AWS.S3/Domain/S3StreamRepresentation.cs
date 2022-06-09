// --------------------------------------------------------------------------------------------------------------------
// <copyright file="S3StreamRepresentation.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    using Naos.Database.Domain;

    /// <summary>
    /// S3 implementation of <see cref="IStreamRepresentation"/>.
    /// </summary>
    public partial class S3StreamRepresentation : StreamRepresentationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="S3StreamRepresentation"/> class.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        public S3StreamRepresentation(
            string name)
            : base(name)
        {
        }
    }
}
