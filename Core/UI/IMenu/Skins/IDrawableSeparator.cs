// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDrawableSeparator.cs" company="LeagueSharp">
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
//   Defines how to draw a separator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins
{
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    /// <summary>
    ///     Defines how to draw a separator.
    /// </summary>
    public interface IDrawableSeparator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        /// <param name="component">The <see cref="MenuSeparator" /></param>
        void Draw(MenuSeparator component);

        #endregion
    }
}