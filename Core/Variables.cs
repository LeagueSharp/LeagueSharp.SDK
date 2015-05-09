using LeagueSharp.CommonEx.Core.UI;

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
            get { return (int) (Game.ClockTime * 1000); }
        }

        /// <summary>
        ///     Gets or sets the LeagueSharp menu.
        /// </summary>
        /// <value>
        ///     The LeagueSharp menu.
        /// </value>
        internal static Menu LeagueSharpMenu { get; set; }
    }
}