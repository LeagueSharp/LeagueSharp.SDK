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

        private static readonly int[] VersionArray;

        static Version()
        {
            var d = Game.Version.Split('.');
            MajorVersion = Convert.ToInt32(d[0]);
            MinorVersion = Convert.ToInt32(d[1]);
            Build = Convert.ToInt32(d[2]);
            Revision = Convert.ToInt32(d[3]);

            VersionArray = new[] { MajorVersion, MinorVersion, Build, Revision };
        }

        /// <summary>
        ///     Checks if the string is older then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is older then the current one.</returns>
        public static bool IsOlder(string version)
        {
            var d = version.Split('.');
            return MinorVersion < Convert.ToInt32(d[1]);
        }

        /// <summary>
        ///     Checks if the string is newer then the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is newer then the current one.</returns>
        public static bool IsNewer(string version)
        {
            var d = version.Split('.');
            return MinorVersion > Convert.ToInt32(d[1]);
        }

        /// <summary>
        ///     Checks if the string is equal to the current one.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>String is equal to the current one.</returns>
        public static bool IsEqual(string version)
        {
            var d = version.Split('.');

            for (var i = 0; i <= d.Length; i++)
            {
                if (d[i] == null || Convert.ToInt32(d[i]) != VersionArray[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}