// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Version.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Provides version checking for League of Legends.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System;

    /// <summary>
    ///     Provides version checking for League of Legends.
    /// </summary>
    public static class Version
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Version" /> class.
        /// </summary>
        static Version()
        {
            var d = Game.Version.Split('.');
            MajorVersion = Convert.ToInt32(d[0]);
            MinorVersion = Convert.ToInt32(d[1]);
            Build = Convert.ToInt32(d[2]);
            Revision = Convert.ToInt32(d[3]);
            CurrentVersion = new System.Version(MajorVersion, MinorVersion, Build, Revision);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Build version.
        /// </summary>
        /// <example>
        ///     <c>X.X.1.X</c>
        /// </example>
        public static int Build { get; set; }

        /// <summary>
        ///     Gets or sets the Current version.
        /// </summary>
        /// <example>
        ///     <c>X.X.X.X</c>
        /// </example>
        public static System.Version CurrentVersion { get; set; }

        /// <summary>
        ///     Gets or sets the Major version.
        /// </summary>
        /// <example>
        ///     <c>1.X.X.X</c>
        /// </example>
        public static int MajorVersion { get; set; }

        /// <summary>
        ///     Gets or sets the Minor Version.
        /// </summary>
        /// <example>
        ///     <c>X.1.X.X</c>
        /// </example>
        public static int MinorVersion { get; set; }

        /// <summary>
        ///     Gets or sets the Revision version.
        /// </summary>
        /// <example>
        ///     <c>X.X.X.1</c>
        /// </example>
        public static int Revision { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks if the string is equal to the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>String is equal to the current one.</returns>
        public static bool IsEqual(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion.Equals(d);
        }

        /// <summary>
        ///     Checks if the version is equal to the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>Version is equal to the current one.</returns>
        public static bool IsEqual(System.Version version)
        {
            return CurrentVersion.Equals(version);
        }

        /// <summary>
        ///     Checks if the string is newer then the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>String is newer then the current one.</returns>
        public static bool IsNewer(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion > d;
        }

        /// <summary>
        ///     Checks if the Version is newer then the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>Version is newer then the current one.</returns>
        public static bool IsNewer(System.Version version)
        {
            return CurrentVersion > version;
        }

        /// <summary>
        ///     Checks if the string is older then the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>String is older then the current one.</returns>
        public static bool IsOlder(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion < d;
        }

        /// <summary>
        ///     Checks if the Version is older then the current one.
        /// </summary>
        /// <param name="version">The Version</param>
        /// <returns>Version is older then the current one.</returns>
        public static bool IsOlder(System.Version version)
        {
            return CurrentVersion < version;
        }

        #endregion
    }
}