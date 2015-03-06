#region

using System;
using System.IO;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Constant values of LeagueSharp.CommonEx
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     The directory where logs will be created.
        /// </summary>
        public static string LogDirectory
        {
            get
            {
                return
                    Cache.Instance.AddOrGetExisting("LogDirectory", () => Path.Combine(LeagueSharpDirectory, "Logs"))
                        .ToString();
            }
        }

        /// <summary>
        ///     The current filename that the logger will write to.
        /// </summary>
        public static string LogFileName
        {
            get
            {
                return
                    Cache.Instance.AddOrGetExisting(
                        "LogFileName", () => DateTime.Now.ToString("d").Replace('/', '-') + ".log").ToString();
            }
        }

        /// <summary>
        ///     Gets the directory where L# resides.
        /// </summary>
        public static string LeagueSharpDirectory
        {
            get
            {
                return
                    Cache.Instance.AddOrGetExisting("LeagueSharpDir", () => AppDomain.CurrentDomain.BaseDirectory)
                        .ToString();
            }
        }
    }
}