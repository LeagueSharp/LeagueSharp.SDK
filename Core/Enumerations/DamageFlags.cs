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
        ///     Flag which indicates non damage, purpose to be an invalid flag and indicate a problem with settings of the flags.
        /// </summary>
        None = 0, 

        /// <summary>
        ///     Flag which indicates the damage would include bonus attack damage.
        /// </summary>
        BonusAttackDamage = 1 << 0, 

        /// <summary>
        ///     Flag which indicates the damage would include attack damage.
        /// </summary>
        AttackDamage = 1 << 1, 

        /// <summary>
        ///     Flag which indicates the damage would include ability power.
        /// </summary>
        AbilityPower = 1 << 2
    }
}