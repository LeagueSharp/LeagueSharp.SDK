// <copyright file="AbilityPower.cs" company="LeagueSharp">
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

    using System.Linq;

    #endregion

    /// <summary>
    ///     Ability Power
    /// </summary>
    public class AbilityPower : IWeightItem
    {
        #region Fields

        /// <summary>
        ///     The average mr
        /// </summary>
        private float averageMr;

        /// <summary>
        ///     The last update
        /// </summary>
        private int lastUpdate;

        #endregion

        #region Public Properties

        public int DefaultWeight => 15;

        public string DisplayName => "Ability Power";

        public bool Inverted => false;

        public string Name => "ability-power";

        /// <summary>
        ///     Gets or sets the update interval.
        /// </summary>
        /// <value>
        ///     The update interval.
        /// </value>
        public int UpdateInterval { get; set; } = 3000;

        #endregion

        #region Public Methods and Operators

        public float GetValue(Obj_AI_Hero hero)
        {
            if (Variables.TickCount - this.lastUpdate > this.UpdateInterval)
            {
                this.averageMr = GameObjects.AllyHeroes.Select(a => a.SpellBlock).DefaultIfEmpty(0).Average();
                this.lastUpdate = Variables.TickCount;
            }
            return hero.FlatMagicDamageMod
                   * (100 / (100 + this.averageMr * hero.PercentMagicPenetrationMod - hero.FlatMagicPenetrationMod));
        }

        #endregion
    }
}