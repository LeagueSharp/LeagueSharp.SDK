// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeManager.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Manages themes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Skins.Default;

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
        public static ITheme Default
        {
            get
            {
                return @default ?? (@default = new DefaultTheme());
            }
        }

        #endregion
    }
}