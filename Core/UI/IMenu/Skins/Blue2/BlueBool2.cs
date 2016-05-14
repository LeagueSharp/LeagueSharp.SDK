// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueBool2.cs" company="LeagueSharp">
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
//   A custom implementation of a <see cref="ADrawable{MenuBool}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Blue2
{
    using LeagueSharp.SDK.UI.Skins.Blue;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     A blue implementation of a <see cref="ADrawable{MenuBool}" />
    /// </summary>
    public class BlueBool2 : BlueBool
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlueBool" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        public BlueBool2(MenuBool component)
            : base(component)
        {
        }

        #endregion
    }
}