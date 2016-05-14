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

namespace LeagueSharp.SDKEx.TSModes.Weights
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
        public float Assist { get; set; } = 85f;

        /// <summary>
        ///     Gets or sets the champion gold value.
        /// </summary>
        public float Champion { get; set; } = 300f;

        /// <inheritdoc />
        public int DefaultWeight => 0;

        /// <inheritdoc />
        public string DisplayName => "Acquired Gold";

        /// <inheritdoc />
        public bool Inverted => false;

        /// <summary>
        ///     Gets or sets the minion gold value.
        /// </summary>
        public float Minion { get; set; } = 27.35f;

        /// <inheritdoc />
        public string Name => "acquired-gold";

        /// <summary>
        ///     Gets or sets the neutral minion gold value.
        /// </summary>
        public float NeutralMinion { get; set; } = 27.35f;

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />
        public float GetValue(Obj_AI_Hero hero)
            =>
                (hero.MinionsKilled * this.Minion) + (hero.NeutralMinionsKilled * this.NeutralMinion)
                + (hero.ChampionsKilled * this.Champion) + (hero.Assists * this.Assist);

        #endregion
    }
}