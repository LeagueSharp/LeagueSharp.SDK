// <copyright file="Gold.cs" company="LeagueSharp">
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
    /// <summary>
    ///     Acquired Gold
    /// </summary>
    public class Gold : IWeightItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the assist gold value.
        /// </summary>
        /// <value>
        ///     The assist.
        /// </value>
        public float Assist { get; set; } = 85f;

        /// <summary>
        ///     Gets or sets the champion gold value.
        /// </summary>
        /// <value>
        ///     The champion.
        /// </value>
        public float Champion { get; set; } = 300f;

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
        public string DisplayName => "Acquired Gold";

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IWeightItem" /> is inverted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if inverted; otherwise, <c>false</c>.
        /// </value>
        public bool Inverted => false;

        /// <summary>
        ///     Gets or sets the minion gold value.
        /// </summary>
        /// <value>
        ///     The minion.
        /// </value>
        public float Minion { get; set; } = 27.35f;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name => "acquired-gold";

        /// <summary>
        ///     Gets or sets the neutral minion gold value.
        /// </summary>
        /// <value>
        ///     The neutral minion.
        /// </value>
        public float NeutralMinion { get; set; } = 27.35f;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero)
            =>
                hero.MinionsKilled * this.Minion + hero.NeutralMinionsKilled * this.NeutralMinion
                + hero.ChampionsKilled * this.Champion + hero.Assists * this.Assist;

        #endregion
    }
}