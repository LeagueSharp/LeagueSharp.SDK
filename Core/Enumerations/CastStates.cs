// <copyright file="CastStates.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK
{
    /// <summary>
    ///     Cast States
    /// </summary>
    public enum CastStates
    {
        /// <summary>
        ///     Spell Successfully Casted
        /// </summary>
        SuccessfullyCasted,

        /// <summary>
        ///     Spell Not Ready
        /// </summary>
        NotReady,

        /// <summary>
        ///     Spell Not Casted
        /// </summary>
        NotCasted,

        /// <summary>
        ///     Spell Out of Range
        /// </summary>
        OutOfRange,

        /// <summary>
        ///     Spell Collision
        /// </summary>
        Collision,

        /// <summary>
        ///     Spell Not Enough Targets
        /// </summary>
        NotEnoughTargets,

        /// <summary>
        ///     Spell Low Hit Chance
        /// </summary>
        LowHitChance,

        /// <summary>
        ///     Spell Invalid Target
        /// </summary>
        InvalidTarget,

        /// <summary>
        ///     Spell Low Mana
        /// </summary>
        LowMana,

        /// <summary>
        ///     Failed Condition
        /// </summary>
        FailedCondition

    }
}