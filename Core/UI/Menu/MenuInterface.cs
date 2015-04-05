using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public static class MenuInterface
    {
        /// <summary>
        ///     Sends a drawing request towards the menu, happens on an OnDraw present.
        /// </summary>
        public static void OnDraw() {}

        /// <summary>
        ///     Sends a windows process message towards the menu.
        /// </summary>
        /// <param name="keys"></param>
        public static void OnWndProc(WindowsKeys keys) {}

        /// <summary>
        ///     Event is fired when the menu container gets opened.
        /// </summary>
        public static void OnMenuOpen() {}

        /// <summary>
        ///     Event is fired when the menu container gets closed.
        /// </summary>
        public static void OnMenuClose() {}
    }
}