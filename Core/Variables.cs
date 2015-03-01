using System;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Class that contains helpful variables.
    /// </summary>
    public class Variables
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