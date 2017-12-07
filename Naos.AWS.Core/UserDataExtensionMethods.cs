// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataExtensionMethods.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.AWS.Core
{
    using System;
    using System.Text;

    using Naos.AWS.Domain;

    /// <summary>
    /// Extension methods on the UserData object.
    /// </summary>
    public static class UserDataExtensionMethods
    {
        /// <summary>
        /// Converts the UserData model object to a base 64 string usable by instance creation.
        /// </summary>
        /// <param name="userData">Domain UserData to convert.</param>
        /// <returns>Base64 string representation of the instructions.</returns>
        public static string ToBase64Representation(this UserData userData)
        {
            if (userData == null)
            {
                return null;
            }

            var ret = Convert.ToBase64String(Encoding.UTF8.GetBytes(userData.Data));
            return ret;
        }
    }
}
