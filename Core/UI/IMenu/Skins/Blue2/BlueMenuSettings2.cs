// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueMenuSettings2.cs" company="LeagueSharp">
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
//   Blue 2 Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//Concept by User Vasconcellos

namespace LeagueSharp.SDK.UI.Skins.Blue2
{
    using LeagueSharp.SDK.UI.Skins.Blue;

    using SharpDX;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class BlueMenuSettings2 : BlueMenuSettings
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="MenuSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static BlueMenuSettings2()
        {
            ContainerSelectedColor = new ColorBGRA(0, 37, 53, 255);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Container Selected Color.
        /// </summary>
        public static ColorBGRA ContainerSelectedColor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Used to load the menu settings.
        /// </summary>
        public static void LoadSettings()
        {
            BlueMenuSettings.LoadSettings();
        }

        #endregion
    }
}