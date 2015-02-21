#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

#endregion

namespace LeagueSharp.CommonEx
{
    /// <summary>
    ///     Constant values of LeagueSharp.CommonEx
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     Saved instance of LeagueSharpDirectory
        /// </summary>
        private static string _leagueSharpDirectory;

        /// <summary>
        ///     The directory where logs will be created.
        /// </summary>
        public static string LogDir
        {
            get { return Path.Combine(LeagueSharpDirectory, "Logs"); }
        }

        /// <summary>
        ///     The current filename that the logger will write to.
        /// </summary>
        public static string LogFileName
        {
            get { return DateTime.Now.Date.ToString(CultureInfo.InvariantCulture).Replace('/', '.') + ".log"; }
        }

        /// <summary>
        ///     Gets the directory where L# resides.
        /// </summary>
        public static string LeagueSharpDirectory
        {
            get
            {
                if (_leagueSharpDirectory == null)
                {
                    try
                    {
                        _leagueSharpDirectory =
                            Process.GetCurrentProcess()
                                .Modules.Cast<ProcessModule>()
                                .First(p => Path.GetFileName(p.ModuleName) == "Leaguesharp.Core.dll")
                                .FileName;
                        _leagueSharpDirectory =
                            Directory.GetParent(Path.GetDirectoryName(_leagueSharpDirectory)).FullName;
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(@"Could not resolve LeagueSharp directory: " + ee);
                        _leagueSharpDirectory = Directory.GetCurrentDirectory();
                    }
                }

                return _leagueSharpDirectory;
            }
        }
    }
}