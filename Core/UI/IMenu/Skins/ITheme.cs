// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITheme.cs" company="LeagueSharp">
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
//   Defines a Theme used to draw components of the menu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using SharpDX;

    /// <summary>
    ///     Defines a Theme used to draw components of the menu.
    /// </summary>
    public interface ITheme
    {
        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="IDrawableBool" />
        /// </summary>
        IDrawableBool Bool { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableButton" />
        /// </summary>
        IDrawableButton Button { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableColorPicker" />
        /// </summary>
        IDrawableColorPicker ColorPicker { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableKeyBind" />
        /// </summary>
        IDrawableKeyBind KeyBind { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableList" />
        /// </summary>
        IDrawableList List { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableSeparator" />
        /// </summary>
        IDrawableSeparator Separator { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawableSlider" />
        /// </summary>
        IDrawableSlider Slider { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the width of a MenuItem without his value
        /// </summary>
        /// <param name="menuItem">The <see cref="MenuItem" /></param>
        /// <returns>The Width</returns>
        int CalcWidthItem(MenuItem menuItem);

        /// <summary>
        ///     Calculates the width of a menu or submenu
        /// </summary>
        /// <param name="menu">The <see cref="Menu" /></param>
        /// <returns>The Width</returns>
        int CalcWidthMenu(Menu menu);

        /// <summary>
        ///     Draws a menu
        /// </summary>
        /// <param name="menuComponent">The <see cref="Menu" /></param>
        void DrawMenu(Menu menuComponent);

        /// <summary>
        ///     Draws the list of root menus on the given position.
        /// </summary>
        void Draw();

        #endregion
    }
}