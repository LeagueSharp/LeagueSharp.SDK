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
        ///     Flag which indicates non damage, purpose to be an invalid flag and indicate a problem with settings of the flags.
        ///     (0)
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
        /// Flag which indicates the damage would include a base array damage of attack damage type treated as percentage of the attack damage. (32)
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
    }
}