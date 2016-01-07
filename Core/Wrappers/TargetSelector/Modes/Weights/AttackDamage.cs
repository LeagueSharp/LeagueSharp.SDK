// <copyright file="AttackDamage.cs" company="LeagueSharp">
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
    ///     Attack Damage
    /// </summary>
    public class AttackDamage : IWeightItem
    {
        #region Fields

        private readonly Dictionary<int, bool> infinity = new Dictionary<int, bool>();

        /// <summary>
        ///     The average armor
        /// </summary>
        private float averageArmor;

        /// <summary>
        ///     The last update
        /// </summary>
        private int lastUpdate;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the default weight.
        /// </summary>
        /// <value>
        ///     The default weight.
        /// </value>
        public int DefaultWeight => 15;

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        public string DisplayName => "Attack Damage";

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
        public string Name => "attack-damage";

        /// <summary>
        ///     Gets or sets the update interval.
        /// </summary>
        /// <value>
        ///     The update interval.
        /// </value>
        public int UpdateInterval { get; set; } = 3000;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero)
        {
            if (Variables.TickCount - this.lastUpdate > this.UpdateInterval)
            {
                this.infinity[hero.NetworkId] = Items.HasItem("InfinityEdge", hero);
                this.averageArmor = GameObjects.AllyHeroes.Select(a => a.Armor).DefaultIfEmpty(0).Average();
                this.lastUpdate = Variables.TickCount;
            }

            var ad = hero.FlatPhysicalDamageMod / 100 * (hero.Crit * 100)
                     * (this.infinity.ContainsKey(hero.NetworkId) && this.infinity[hero.NetworkId] ? 2.5f : 2);
            return ad
                   * (100 / (100 + this.averageArmor * hero.PercentArmorPenetrationMod - hero.FlatArmorPenetrationMod))
                   * (1f / ObjectManager.Player.AttackDelay);
        }

        #endregion
    }
}