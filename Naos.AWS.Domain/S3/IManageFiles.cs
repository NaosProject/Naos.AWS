// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFiles.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Interface managing files in S3.
    /// </summary>
    public interface IManageFiles : IUploadFiles, IDownloadFiles, IManageFileMetadata, IListFiles
    {
    }
}
