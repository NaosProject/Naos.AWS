// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExistingFileWriteAction.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Domain
{
    /// <summary>
    /// Determines what action to take when writing in the presence of an already existing file.
    /// </summary>
    public enum ExistingFileWriteAction
    {
        /// <summary>
        /// Overwrite the file.
        /// </summary>
        OverwriteFile,

        /// <summary>
        /// Do not overwrite the file.
        /// </summary>
        DoNotOverwriteFile,

        /// <summary>
        /// Throw an exception.
        /// </summary>
        Throw,
    }
}
