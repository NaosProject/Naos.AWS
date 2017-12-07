// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAwsObject.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Interface on all AWS objects.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Aws", Justification = "Spelling/name is correct.")]
    public interface IAwsObject : IHaveName, IHaveId, IHaveRegion
    {
    }
}
