#region

using System;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     General Utils.
    /// </summary>
    public class Utils
    {
        /// <summary>
        ///     Safe TickCount.
        /// </summary>
        public static long TickCount
        {
            get { return DateTime.Now.Ticks / 10000; }
        }
    }
}