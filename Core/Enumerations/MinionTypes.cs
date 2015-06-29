// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinionTypes.cs" company="LeagueSharp">
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
//   Types of minions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Enumerations
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
        Unknown, 

        /// <summary>
        ///     The normal type.
        /// </summary>
        Normal, 

        /// <summary>
        ///     The ranged type.
        /// </summary>
        Ranged, 

        /// <summary>
        ///     The melee type.
        /// </summary>
        Melee, 

        /// <summary>
        ///     The siege type.
        /// </summary>
        Siege, 

        /// <summary>
        ///     The super type.
        /// </summary>
        Super, 

        /// <summary>
        ///     The ward type.
        /// </summary>
        Ward
    }
}