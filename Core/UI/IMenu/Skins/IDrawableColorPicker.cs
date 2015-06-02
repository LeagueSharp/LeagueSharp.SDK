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

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a ColorPicker
    /// </summary>
    public interface IDrawableColorPicker
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Get the alpha picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle AlphaPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the blue picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle BluePickerBoundaries(MenuColor component);

        /// <summary>
        ///     Draws a <see cref="MenuColor"/>
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        void Draw(MenuColor component);

        /// <summary>
        ///     Get the green picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <c>Rectangle</c></returns>
        Rectangle GreenPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle PickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle PreviewBoundaries(MenuColor component);

        /// <summary>
        ///     Get the red picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle RedPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Gets the width of the slider
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="int" /></returns>
        int SliderWidth(MenuColor component);

        /// <summary>
        ///     Gets the width of a <c>MenuColor</c>
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="int" /></returns>
        int Width(MenuColor component);

        #endregion
    }
}