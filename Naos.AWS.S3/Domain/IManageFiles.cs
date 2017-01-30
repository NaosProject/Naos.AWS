// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFiles.cs" company="Naos">
//   Copyright 2017 Naos
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Naos.AWS.S3
{
    /// <summary>
    /// Interface managing files in S3.
    /// </summary>
    public interface IManageFiles : IUploadFiles, IDownloadFiles, IManageFileMetadata, IListFiles
    {
    }
}