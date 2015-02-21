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
        public static int TickCount
        {
            get { return Environment.TickCount & int.MaxValue; }
        }
    }
}