// <copyright file="TeamFocus.cs" company="LeagueSharp">
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
    using System;
    using System.Linq;

    /// <summary>
    ///     Team Focus
    /// </summary>
    public class TeamFocus : IWeightItem
    {
        #region Public Properties

        /// <summary>
        ///     The default weight
        /// </summary>
        public int DefaultWeight => 0;

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName => "Team Focus";

        /// <summary>
        ///     Gets or sets the fade time.
        /// </summary>
        /// <value>
        ///     The fade time.
        /// </value>
        public int FadeTime { get; set; } = 10000;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IWeightItem" /> is inverted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if inverted; otherwise, <c>false</c>.
        /// </value>
        public bool Inverted => false;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name => "team-focus";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero)
            =>
                Aggro.Entries.Where(a => a.Value.Target.Compare(hero))
                    .Select(a => Math.Max(0, this.FadeTime - (Variables.TickCount - a.Value.TickCount)))
                    .DefaultIfEmpty(0)
                    .Sum();

        #endregion
    }
}