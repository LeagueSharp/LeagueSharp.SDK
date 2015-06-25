// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDrawableColorPicker.cs" company="LeagueSharp">
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
//   Defines how to draw a ColorPicker
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a ColorPicker
    /// </summary>
    public interface IDrawableColorPicker
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Draws a <see cref="MenuColor"/>
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        void Draw(MenuColor component);
        

        /// <summary>
        ///     Gets the width of a <c>MenuColor</c>
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="int" /></returns>
        int Width(MenuColor component);

        /// <summary>
        /// Processes windows events
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">events</param>
        void OnWndProc(MenuColor component, WindowsKeys args);

        #endregion
    }
}