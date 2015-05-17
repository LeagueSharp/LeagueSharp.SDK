// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrbwalkerMode.cs" company="LeagueSharp">
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
//   <c>Orbwalker</c> Modes
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Enumerations
{
    /// <summary>
    ///     <c>Orbwalker</c> Modes
    /// </summary>
    public enum OrbwalkerMode
    {
        /// <summary>
        ///     None Mode
        /// </summary>
        None, 

        /// <summary>
        ///     <c>Orbwalk</c> Mode
        /// </summary>
        Orbwalk, 

        /// <summary>
        ///     Hybrid Mode
        /// </summary>
        Hybrid, 

        /// <summary>
        ///     Last Hit Mode
        /// </summary>
        LastHit, 

        /// <summary>
        ///     Lane Clear Mode
        /// </summary>
        LaneClear, 

        /// <summary>
        ///     Lane Freeze Mode
        /// </summary>
        LaneFreeze
    }
}