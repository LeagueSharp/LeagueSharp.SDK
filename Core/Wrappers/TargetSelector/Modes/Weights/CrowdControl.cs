// <copyright file="CrowdControl.cs" company="LeagueSharp">
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
    #region

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    /// <summary>
    ///     Crowd Control
    /// </summary>
    public class CrowdControl : IWeightItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrowdControl" /> class.
        /// </summary>
        public CrowdControl()
        {
            this.BuffTypes = new List<BuffType>
                                 {
                                     BuffType.Charm, BuffType.Knockback, BuffType.Suppression, BuffType.Fear,
                                     BuffType.Taunt, BuffType.Stun, BuffType.Slow, BuffType.Silence, BuffType.Snare,
                                     BuffType.Polymorph, BuffType.Snare
                                 };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the buff types.
        /// </summary>
        /// <value>
        ///     The buff types.
        /// </value>
        public List<BuffType> BuffTypes { get; set; }

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
        public string DisplayName => "Crowd Control";

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
        public string Name => "crowd-control";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero)
        {
            var buffs = hero.Buffs.Where(b => this.BuffTypes.Contains(b.Type)).ToList();
            return buffs.Any() ? buffs.Select(x => x.EndTime).DefaultIfEmpty(0).Max() : 0;
        }

        #endregion
    }
}