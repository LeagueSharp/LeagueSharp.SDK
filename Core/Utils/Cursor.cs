using SharpDX;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Cursor utility, tracks after cursor actions.
    /// </summary>
    public class Cursor
    {
        /// <summary>
        ///     Saved Cursor X-axis position on the screen
        /// </summary>
        private static int _posX;

        /// <summary>
        ///     Saved Cursor Y-axis position on the screen
        /// </summary>
        private static int _posY;

        /// <summary>
        ///     Static Constructor
        /// </summary>
        static Cursor()
        {
            Game.OnWndProc += Game_OnWndProc;
        }

        /// <summary>
        ///     Returns the current Cursor position in a Vector2.
        /// </summary>
        public static Vector2 Position
        {
            get { return new Vector2(_posX, _posY); }
        }

        /// <summary>
        ///     Windows Process Message subscribed event function.
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs"/></param>
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 0x0200)
            {
                _posX = unchecked ((short) args.LParam);
                _posY = unchecked ((short) ((long) args.LParam >> 16));
            }
        }
    }
}