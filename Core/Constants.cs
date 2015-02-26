#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
        public static string LogDir
        {
            get
            {
                return
                    Cache.Instance.AddOrGetExisting("LogDir", () => Path.Combine(LeagueSharpDirectory, "Logs"))
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
                        "LogFileName",
                        () => DateTime.Now.Date.ToString(CultureInfo.InvariantCulture).Replace('/', '.') + ".log")
                        .ToString();
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
                    Cache.Instance.AddOrGetExisting(
                        "LeagueSharpDir",
                        () =>
                            Directory.GetParent(
                                Process.GetCurrentProcess()
                                    .Modules.Cast<ProcessModule>()
                                    .First(p => Path.GetFileName(p.ModuleName) == "Leaguesharp.Core.dll")
                                    .FileName).FullName).ToString();
            }
        }
    }
}