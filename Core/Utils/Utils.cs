#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     General Utils.
    /// </summary>
    public class Utils
    {

        private static string _leagueSharpDirectory;

        /// <summary>
        ///     Safe TickCount.
        /// </summary>
        public static int TickCount
        {
            get { return Environment.TickCount & int.MaxValue; }
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