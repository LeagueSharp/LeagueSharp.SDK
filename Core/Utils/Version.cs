using System;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Provides version checking for League of Legends.
    /// </summary>
    public static class Version
    {
        /// <summary>
        ///     Major version.
        /// </summary>
        /// <example>1.X.X.X</example>
        public static int MajorVersion;

        /// <summary>
        ///     Minor Version.
        /// </summary>
        /// <example>X.1.X.X</example>
        public static int MinorVersion;

        /// <summary>
        ///     Build version.
        /// </summary>
        /// <example>X.X.1.X</example>
        public static int Build;

        /// <summary>
        ///     Revision version.
        /// </summary>
        /// <example>X.X.X.1</example>
        public static int Revision;

        /// <summary>
        ///     Current version.
        /// </summary>
        /// <example>X.X.X.X</example>
        public static System.Version CurrentVersion;

        static Version()
        {
            var d = Game.Version.Split('.');
            MajorVersion = Convert.ToInt32(d[0]);
            MinorVersion = Convert.ToInt32(d[1]);
            Build = Convert.ToInt32(d[2]);
            Revision = Convert.ToInt32(d[3]);
            CurrentVersion = new System.Version(MajorVersion, MinorVersion, Build, Revision);
        }

        /// <summary>
        ///     Checks if the string is older then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is older then the current one.</returns>
        public static bool IsOlder(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion < d;
        }

        /// <summary>
        ///     Checks if the Version is older then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Version is older then the current one.</returns>
        public static bool IsOlder(System.Version version)
        {
            return CurrentVersion < version;
        }

        /// <summary>
        ///     Checks if the string is newer then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is newer then the current one.</returns>
        public static bool IsNewer(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion > d;
        }

        /// <summary>
        ///     Checks if the Version is newer then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Version is newer then the current one.</returns>
        public static bool IsNewer(System.Version version)
        {
            return CurrentVersion > version;
        }

        /// <summary>
        ///     Checks if the string is equal to the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is equal to the current one.</returns>
        public static bool IsEqual(string version)
        {
            var d = new System.Version(version);
            return CurrentVersion.Equals(d);
        }

        /// <summary>
        ///     Checks if the version is equal to the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Version is equal to the current one.</returns>
        public static bool IsEqual(System.Version version)
        {
            return CurrentVersion.Equals(version);
        }
    }
}