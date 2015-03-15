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
        public static int TickCount
        {
            get { return (int)(Game.ClockTime * 1000); }
        }
    }
}