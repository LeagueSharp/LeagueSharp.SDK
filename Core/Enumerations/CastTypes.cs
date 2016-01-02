// <copyright file="CastTypes.cs" company="LeagueSharp">
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
    /// Indicates how a spell can be casted
    /// </summary>
    public enum CastType
    {
        /// <summary>
        /// The spell can be casted on an enemy champion
        /// </summary>
        EnemyChampions,

        /// <summary>
        /// The spell can be casted on an enemy minion
        /// </summary>
        EnemyMinions,

        /// <summary>
        /// The spell can be casted on an enemy tower
        /// </summary>
        EnemyTurrets,

        /// <summary>
        /// The spell can be casted on an ally champion
        /// </summary>
        AllyChampions,

        /// <summary>
        /// The spell can be casted on an ally minion
        /// </summary>
        AllyMinions,

        /// <summary>
        /// The spell can be casted on an ally turret
        /// </summary>
        AllyTurrets,

        /// <summary>
        /// The spell can be casted only on pets.
        /// </summary>
        HeroPets,

        /// <summary>
        /// The spell can be casted on a position
        /// </summary>
        Position,

        /// <summary>
        /// The spell can be casted in a direction
        /// </summary>
        Direction,

        /// <summary>
        /// The spell can be casted on self
        /// </summary>
        Self,

        /// <summary>
        /// The spell is a charging spell
        /// </summary>
        Charging,

        /// <summary>
        /// The spell is a toggleable spell
        /// </summary>
        Toggle,

        /// <summary>
        /// The spell is a channel
        /// </summary>
        Channel,

        /// <summary>
        /// The spell is activable
        /// </summary>
        Activate,

        /// <summary>
        /// The spell can't be casted
        /// </summary>
        ImpossibleToCast
    }
}
