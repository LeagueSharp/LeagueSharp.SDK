// <copyright file="DamageLibrary.cs" company="LeagueSharp">
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
    using System;
    using System.Collections.Generic;

    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Static Fields

        /// <summary>
        ///     Gets the Damage Collection.
        /// </summary>
        internal static IReadOnlyDictionary<string, ChampionDamage> DamageCollection =
            new Dictionary<string, ChampionDamage>();

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="Damage" /> class.
        /// </summary>
        internal static void Initialize()
        {
            Events.OnLoad += (sender, args) =>
                {
                    DamageCollection = Data.Get<DamageDatabase>().Damage;
                    CreatePassives();
                };
        }

        /// <summary>
        ///     Resolves the spell bonus damage.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="spellBonus">
        ///     The spell bonus collection
        /// </param>
        /// <param name="index">
        ///     The index (spell level - 1)
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double ResolveBonusSpellDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            ChampionDamageSpellBonus spellBonus,
            int index)
        {
            var sourceScale = spellBonus.ScalingTarget == DamageScalingTarget.Source ? source : target;
            var percent = spellBonus.DamagePercentages?.Count > 0
                              ? spellBonus.DamagePercentages[Math.Min(index, spellBonus.DamagePercentages.Count - 1)]
                              : 0d;
            var origin = 0f;

            switch (spellBonus.ScalingType)
            {
                case DamageScalingType.BonusAttackPoints:
                    origin = sourceScale.FlatPhysicalDamageMod;
                    break;
                case DamageScalingType.AbilityPoints:
                    origin = sourceScale.TotalMagicalDamage;
                    break;
                case DamageScalingType.AttackPoints:
                    origin = sourceScale.TotalAttackDamage;
                    break;
                case DamageScalingType.MaxHealth:
                    origin = sourceScale.MaxHealth;
                    break;
                case DamageScalingType.CurrentHealth:
                    origin = sourceScale.Health;
                    break;
                case DamageScalingType.MissingHealth:
                    origin = sourceScale.MaxHealth - sourceScale.Health;
                    break;
                case DamageScalingType.BonusHealth:
                    origin = ((Obj_AI_Hero)sourceScale).BonusHealth;
                    break;
                case DamageScalingType.Armor:
                    origin = sourceScale.Armor;
                    break;
                case DamageScalingType.MaxMana:
                    origin = sourceScale.MaxMana;
                    break;
            }

            var dmg = origin
                      * (percent > 0 || percent < 0
                             ? (percent > 0 ? percent : 0)
                               + (spellBonus.ScalePer100Ap > 0
                                      ? Math.Abs(source.TotalMagicalDamage / 100) * spellBonus.ScalePer100Ap
                                      : 0)
                               + (spellBonus.ScalePer100BonusAd > 0
                                      ? Math.Abs(source.FlatPhysicalDamageMod / 100) * spellBonus.ScalePer100BonusAd
                                      : 0)
                               + (spellBonus.ScalePer100Ad > 0
                                      ? Math.Abs(source.TotalAttackDamage / 100) * spellBonus.ScalePer100Ad
                                      : 0)
                             : 0);

            if (target is Obj_AI_Minion && spellBonus.BonusDamageOnMinion?.Count > 0)
            {
                dmg += spellBonus.BonusDamageOnMinion[Math.Min(index, spellBonus.BonusDamageOnMinion.Count - 1)];
            }

            if (!string.IsNullOrEmpty(spellBonus.ScalingBuff))
            {
                var buffCount =
                    (spellBonus.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                        spellBonus.ScalingBuff);
                dmg = buffCount > 0 ? dmg * (buffCount + spellBonus.ScalingBuffOffset) : 0d;
            }

            if (dmg > 0)
            {
                if (spellBonus.MinDamage?.Count > 0)
                {
                    dmg = Math.Max(dmg, spellBonus.MinDamage[Math.Min(index, spellBonus.MinDamage.Count - 1)]);
                }

                if (target is Obj_AI_Minion && spellBonus.MaxDamageOnMinion?.Count > 0)
                {
                    dmg = Math.Min(
                        dmg,
                        spellBonus.MaxDamageOnMinion[Math.Min(index, spellBonus.MaxDamageOnMinion.Count - 1)]);
                }
            }

            return dmg;
        }

        #endregion
    }
}