// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDrawableList.cs" company="LeagueSharp">
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
//   Defines how to draw a generic list of items
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a generic list of items
    /// </summary>
    public interface IDrawableList
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Draw a <see cref="MenuList" />
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        void Draw(MenuList component);

        /// <summary>
        ///     Gets the dropdown boundaries (preview)
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle DropDownBoundaries(MenuList component);

        /// <summary>
        ///     Gets the complete dropdown boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        Rectangle DropDownExpandedBoundaries(MenuList component);

        /// <summary>
        ///     Gets the list of dropdown item boundaries.
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>List of <see cref="Rectangle" /></returns>
        List<Rectangle> DropDownListBoundaries(MenuList component);

        /// <summary>
        ///     Gets the width of the <see cref="MenuList" />
        /// </summary>
        /// <param name="menuList">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="int" /></returns>
        int Width(MenuList menuList);

        #endregion
    }
}