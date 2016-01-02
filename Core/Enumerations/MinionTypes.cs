// <copyright file="MinionTypes.cs" company="LeagueSharp">
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
    using System;

    /// <summary>
    ///     Types of minions
    /// </summary>
    [Flags]
    public enum MinionTypes
    {
        /// <summary>
        ///     The unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The normal type.
        /// </summary>
        Normal = 1 << 0,

        /// <summary>
        ///     The ranged type.
        /// </summary>
        Ranged = 1 << 1,

        /// <summary>
        ///     The melee type.
        /// </summary>
        Melee = 1 << 2,

        /// <summary>
        ///     The siege type.
        /// </summary>
        Siege = 1 << 3,

        /// <summary>
        ///     The super type.
        /// </summary>
        Super = 1 << 4,

        /// <summary>
        ///     The ward type.
        /// </summary>
        Ward = 1 << 5
    }
}