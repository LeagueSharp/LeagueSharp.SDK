// <copyright file="ITargetSelectorMode.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx
{
    using System.Collections.Generic;

    using LeagueSharp.SDKEx.UI;

    /// <summary>
    ///     Interface for modes.
    /// </summary>
    public interface ITargetSelectorMode
    {
        #region Public Properties

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds to menu.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        void AddToMenu(Menu menu);

        /// <summary>
        ///     Orders the champions.
        /// </summary>
        /// <param name="heroes">
        ///     The heroes.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="Obj_AI_Hero" />.
        /// </returns>
        List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes);

        #endregion
    }
}