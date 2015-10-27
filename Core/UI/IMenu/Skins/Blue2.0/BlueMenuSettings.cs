// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueMenuSettings.cs" company="LeagueSharp">
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
//   Default Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//Concept by User Vasconcellos

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Blue2
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using LeagueSharp.SDK.Core.UI.IMenu.Customizer;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins.Blue;

    using SharpDX;
    using SharpDX.Direct3D9;

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

        /// <summary>
        ///     Gets or sets the Global Container Selected Color.
        /// </summary>
        public static ColorBGRA ContainerSelectedColor { get; set; }
    }
}