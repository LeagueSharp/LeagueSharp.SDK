// <copyright file="ThemeManager.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI.Skins
{
    using LeagueSharp.SDK.UI.Skins.Blue;
    using LeagueSharp.SDK.UI.Skins.Blue2;
    using LeagueSharp.SDK.UI.Skins.Colored;
    using LeagueSharp.SDK.UI.Skins.Default;
    using LeagueSharp.SDK.UI.Skins.Light;
    using LeagueSharp.SDK.UI.Skins.Light2;
    using LeagueSharp.SDK.UI.Skins.Tech;

    /// <summary>
    ///     Manages themes.
    /// </summary>
    public class ThemeManager
    {
        #region Static Fields

        /// <summary>
        ///     The current theme.
        /// </summary>
        private static ITheme current;

        /// <summary>
        ///     The default theme.
        /// </summary>
        private static ITheme @default;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the current ITheme used by the menu.
        /// </summary>
        public static ITheme Current
        {
            get
            {
                return current ?? (current = Default);
            }

            set
            {
                current = value;
            }
        }

        /// <summary>
        ///     Gets the default theme.
        /// </summary>
        /// <value>
        ///     The default theme.
        /// </value>
        public static ITheme Default => @default ?? (@default = new DefaultTheme());

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        private static Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes static members of the <see cref="ThemeManager" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        public static void Initialize(Menu menu)
        {
            Events.OnLoad += (sender, args) =>
                {
                    Menu = new Menu("thememanager", "Theme Manager");

                    Menu.Add(
                        new MenuList<string>(
                            "themeID",
                            "Theme",
                            new[] { "Default", "Blue", "Blue 2", "Light", "Light 2", "Colored", "Tech" })).ValueChanged
                        += (o, eventArgs) =>
                            {
                                Notifications.Add(new Notification("Theme Manager", "Please reload Menu !"));
                            };

                    menu.Add(Menu);

                    switch (Menu["themeID"].GetValue<MenuList>().Index)
                    {
                        case 0:
                            Current = new DefaultTheme();
                            break;
                        case 1:
                            Current = new BlueTheme();
                            break;
                        case 2:
                            Current = new BlueTheme2();
                            break;
                        case 3:
                            Current = new LightTheme();
                            break;
                        case 4:
                            Current = new LightTheme2();
                            break;
                        case 5:
                            Current = new ColoredTheme();
                            break;
                        case 6:
                            Current = new TechTheme();
                            break;
                    }
                };
        }

        #endregion
    }
}