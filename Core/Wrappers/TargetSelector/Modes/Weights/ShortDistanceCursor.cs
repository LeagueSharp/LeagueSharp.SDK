﻿// <copyright file="ShortDistanceCursor.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers.TargetSelector.Modes.Weights
{
    using LeagueSharp.SDK.Core.Extensions;

    /// <summary>
    ///     Short Distance to Cursor
    /// </summary>
    public class ShortDistanceCursor : IWeightItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets the default weight.
        /// </summary>
        /// <value>
        ///     The default weight.
        /// </value>
        public int DefaultWeight => 0;

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName => "Short Distance to Cursor";

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IWeightItem" /> is inverted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if inverted; otherwise, <c>false</c>.
        /// </value>
        public bool Inverted => true;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name => "short-distance-cursor";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero) => hero.Distance(Game.CursorPos);

        #endregion
    }
}