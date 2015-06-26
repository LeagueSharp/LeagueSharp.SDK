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
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;

    /// <summary>
    ///     Defines a Theme used to draw components of the menu.
    /// </summary>
    public interface ITheme
    {
        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuBool}"/> 
        /// </summary>
        IDrawable<MenuBool> Bool { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuButton}"/> 
        /// </summary>
        IDrawable<MenuButton> Button { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuColor}"/> 
        /// </summary>
        IDrawable<MenuColor> ColorPicker { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuKeyBind}"/> 
        /// </summary>
        IDrawable<MenuKeyBind> KeyBind { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuList}"/> 
        /// </summary>
        IDrawable<MenuList> List { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuSeparator}"/> 
        /// </summary>
        IDrawable<MenuSeparator> Separator { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{MenuSlider}"/> 
        /// </summary>
        IDrawable<MenuSlider> Slider { get; }

        /// <summary>
        ///     Gets the <see cref="IDrawable{Menu}"/> 
        /// </summary>
        IDrawable<Menu> Menu { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws the list of root menus on the given position.
        /// </summary>
        void Draw();

        #endregion
    }
}