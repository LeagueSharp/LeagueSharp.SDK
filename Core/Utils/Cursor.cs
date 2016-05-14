// <copyright file="Cursor.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.Utils
{
    using SharpDX;

    /// <summary>
    ///     Cursor utility, tracks after cursor actions.
    /// </summary>
    public class Cursor
    {
        #region Static Fields

        /// <summary>
        ///     Saved Cursor X-axis position on the screen
        /// </summary>
        private static int posX;

        /// <summary>
        ///     Saved Cursor Y-axis position on the screen
        /// </summary>
        private static int posY;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Cursor" /> class.
        ///     Static Constructor
        /// </summary>
        static Cursor()
        {
            Game.OnWndProc += Game_OnWndProc;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the current Cursor Game position as a Screen Vector2.
        /// </summary>
        public static Vector2 GameScreenPosition => Drawing.WorldToScreen(Game.CursorPos);

        /// <summary>
        ///     Returns if the cursor is over a HUD item.
        /// </summary>
        public static bool IsOverHUD => Position != GameScreenPosition;

        /// <summary>
        ///     Gets the current Cursor position in a Vector2.
        /// </summary>
        public static Vector2 Position => new Vector2(posX, posY);

        #endregion

        #region Methods

        /// <summary>
        ///     Windows Process Message subscribed event function.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WndEventArgs" /> data
        /// </param>
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 0x0200)
            {
                posX = unchecked((short)args.LParam);
                posY = unchecked((short)((long)args.LParam >> 16));
            }
        }

        #endregion
    }
}