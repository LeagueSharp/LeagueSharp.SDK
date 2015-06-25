// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDrawableBool.cs" company="LeagueSharp">
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
//   Defines how to draw a On/Off button
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a On/Off button
    /// </summary>
    public interface IDrawableBool
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Draws a <see cref="MenuBool"/>
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        void Draw(MenuBool component);

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event data</param>
        void OnWndProc(MenuBool component, WindowsKeys args);

        /// <summary>
        /// Calculates the Width of a MenuBool
        /// </summary>
        /// <param name="component">menu component</param>
        /// <returns>width</returns>
        int Width(MenuBool component);

        #endregion
    }
}