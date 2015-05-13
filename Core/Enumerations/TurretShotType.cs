// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TurretShotType.cs" company="LeagueSharp">
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
//   Enumeration containing the type of turret shot.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.Enumerations
{
    /// <summary>
    ///     Enumeration containing the type of turret shot.
    /// </summary>
    public enum TurretShotType
    {
        /// <summary>
        ///     The turret fired a shot.
        /// </summary>
        TurretShot, 

        /// <summary>
        ///     The turret shot hit the unit.
        /// </summary>
        TurretShotHit
    }
}