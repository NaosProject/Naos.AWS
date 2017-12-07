// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageFiles.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.S3
{
    /// <summary>
    /// Interface managing files in S3.
    /// </summary>
    public interface IManageFiles : IUploadFiles, IDownloadFiles, IManageFileMetadata, IListFiles
    {
    }
}