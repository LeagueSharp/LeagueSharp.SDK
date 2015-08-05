// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DamageFlags.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The damage flags.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Enumerations
{
    using System;

    /// <summary>
    ///     The damage flags.
    /// </summary>
    [Flags]
    public enum DamageFlags
    {
        /// <summary>
        ///     Flag which indicates no special damage flag, default flag for all. (0)
        /// </summary>
        None = 0, 

        /// <summary>
        ///     Flag which indicates the damage would include bonus attack damage. (1)
        /// </summary>
        BonusAttackDamage = 1 << 0, 

        /// <summary>
        ///     Flag which indicates the damage would include attack damage. (2)
        /// </summary>
        AttackDamage = 1 << 1, 

        /// <summary>
        ///     Flag which indicates the damage would include ability power. (4)
        /// </summary>
        AbilityPower = 1 << 2, 

        /// <summary>
        ///     Flag which indicates the damage would include a percentage of enemy max health. (8)
        /// </summary>
        EnemyMaxHealth = 1 << 3, 

        /// <summary>
        ///     Flag which indicates the damage would include a percentage of enemy max health multiplied by a percent of ability
        ///     power. (16)
        /// </summary>
        AbilityPowerEnemyMaxHealth = 1 << 4, 

        /// <summary>
        ///     Flag which indicates the damage would include a base array damage of attack damage type treated as percentage of
        ///     the attack damage. (32)
        /// </summary>
        BaseAttackDamagePercent = 1 << 5, 

        /// <summary>
        ///     Flag which indicates the damage would include a base array based on the champion level. (64)
        /// </summary>
        BaseChampionLevel = 1 << 6, 

        /// <summary>
        ///     Flag which indicates that damage would include a percentage of the champion's max health. (128)
        /// </summary>
        MaxHealth = 1 << 7, 

        /// <summary>
        ///     Flag which indicates that damage would be changed based on the distance of target from the source. (256)
        /// </summary>
        SpecialDistance = 1 << 8, 

        /// <summary>
        ///     Flag which indicates that damage would be based on target's current health. (512)
        /// </summary>
        TargetHealth = 1 << 9, 

        /// <summary>
        ///     Flag which indicates that damage would be based around a passive and the spell holds two possible forms. (1024)
        /// </summary>
        SpecialPassiveAlternative = 1 << 10, 

        /// <summary>
        ///     Flag which indicates the damage would include a base array damage of ability power type treated as percentage of
        ///     the ability power. (2048)
        /// </summary>
        BaseAbilityPowerPercent = 1 << 11, 

        /// <summary>
        ///     Flag which indicates the damage would include a base array damage of bonus attack damage type treated as percentage
        ///     of the bonus attack damage. (4098)
        /// </summary>
        BaseBonusAttackDamagePercent = 1 << 12, 

        /// <summary>
        ///     Flag which indicates the damage arrays would scale with a different spell slot. (8196)
        /// </summary>
        LevelScale = 1 << 13, 

        /// <summary>
        ///     Flag which indicates the damage would be based off a passive. (16384)
        /// </summary>
        SpecialPassive = 1 << 14, 

        /// <summary>
        ///     Flag which indicates the damage would be based off the source's max <c>mana</c>. (32768)
        /// </summary>
        MaxMana = 1 << 15
    }
}