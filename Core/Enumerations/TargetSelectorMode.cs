// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetSelectorMode.cs" company="LeagueSharp">
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
//   Enumeration that defines the priority in which the target selector should organize targets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Enumerations
{
    /// <summary>
    ///     Enumeration that defines the priority in which the target selector should organize targets.
    /// </summary>
    public enum TargetSelectorMode
    {
        /// <summary>
        ///     Focuses targets based on how many auto attacks it takes to kill the units.
        /// </summary>
        LessAttacksToKill, 

        /// <summary>
        ///     Focuses targets based on the amount of AP they have.
        /// </summary>
        MostAbilityPower, 

        /// <summary>
        ///     Focuses targets based on the amount of AD they have.
        /// </summary>
        MostAttackDamage, 

        /// <summary>
        ///     Focuses targets based on the distance between the player and target.
        /// </summary>
        Closest, 

        /// <summary>
        ///     Focuses targets base on the distance between the target and the mouse.
        /// </summary>
        NearMouse,

        /// <summary>
        ///     Focuses targets by their class.
        /// </summary>
        AutoPriority, 

        /// <summary>
        ///     Focuses targets by their health.
        /// </summary>
        LeastHealth
    }
}