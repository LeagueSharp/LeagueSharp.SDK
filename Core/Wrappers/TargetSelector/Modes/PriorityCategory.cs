// <copyright file="PriorityCategory.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Modes
{
    using System.Collections.Generic;

    /// <summary>
    ///     Category class for Priorities
    /// </summary>
    public class PriorityCategory
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the champions.
        /// </summary>
        /// <value>
        ///     The champions.
        /// </value>
        public HashSet<string> Champions { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public int Value { get; set; }

        #endregion
    }
}