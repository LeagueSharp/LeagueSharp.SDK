using System.IO;
using LeagueSharp.CommonEx.Core.Enumerations;
using SharpDX;
using Color = System.Drawing.Color;

namespace LeagueSharp.CommonEx.Core.UI.Settings
{
    /// <summary>
    ///     MenuSettings class used to manage the Settings
    /// </summary>
    public static class MenuSettings
    {
        /// <summary>
        ///     Base Position of the Menu
        /// </summary>
        public static Vector2 BasePosition = new Vector2(10, 10);

        private static bool _drawTheMenu;

        static MenuSettings()
        {
            Game.OnWndProc += Game_OnWndProc;
            _drawTheMenu = MenuGlobals.DrawMenu;
        }

        /// <summary>
        ///     Boolean if the Menu should be Drawn
        /// </summary>
        public static bool DrawMenu
        {
            get { return _drawTheMenu; }
            set
            {
                _drawTheMenu = value;
                MenuGlobals.DrawMenu = value;
            }
        }

        /// <summary>
        ///     Path of the MenuConfig
        /// </summary>
        public static string MenuConfigPath
        {
            get { return Path.Combine(Constants.LeagueSharpAppData, "MenuConfig"); }
        }

        /// <summary>
        ///     Width of the Menu Items
        /// </summary>
        public static int MenuItemWidth
        {
            get { return 160; }
        }

        /// <summary>
        ///     Height of the Menu Items
        /// </summary>
        public static int MenuItemHeight
        {
            get { return 30; }
        }

        /// <summary>
        ///     Background Color of the Menu
        /// </summary>
        public static Color BackgroundColor
        {
            get { return Color.FromArgb(200, Color.Black); }
        }

        /// <summary>
        ///     Background Color of activated Items
        /// </summary>
        public static Color ActiveBackgroundColor
        {
            get { return Color.DimGray; }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if ((args.Msg == (uint)WindowsMessages.KEYUP || args.Msg == (uint)WindowsMessages.KEYDOWN) &&
                args.WParam == Constants.ShowMenuPressKey)
            {
                DrawMenu = args.Msg == (uint)WindowsMessages.KEYDOWN;
            }

            if (args.Msg == (uint)WindowsMessages.KEYUP && args.WParam == Constants.ShowMenuToggleKey)
            {
                DrawMenu = !DrawMenu;
            }
        }
    }
}