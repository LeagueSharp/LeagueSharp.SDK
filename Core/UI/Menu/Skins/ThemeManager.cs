using LeagueSharp.CommonEx.Core.UI.Skins.Default;

namespace LeagueSharp.CommonEx.Core.UI.Skins
{
    /// <summary>
    ///     Manages themes.
    /// </summary>
    public class ThemeManager
    {
        private static Theme _default, _current;

        /// <summary>
        /// Gets the default theme.
        /// </summary>
        /// <value>
        /// The default theme.
        /// </value>
        public static Theme Default
        {
            get { return (_default ?? (_default = new DefaultTheme())); }
        }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        /// <value>
        /// The current theme.
        /// </value>
        public static Theme Current
        {
            get { return (_current ?? (_current = Default)); }
            set { _current = value; }
        }
    }
}