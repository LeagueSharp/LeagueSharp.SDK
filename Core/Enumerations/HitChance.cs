// <copyright file="HitChance.cs" company="LeagueSharp">
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
    ///     <c>Skillshot</c> HitChance
    /// </summary>
    public enum HitChance
    {
        /// <summary>
        ///     Target is immobile, <c>skillshot</c> will hit.
        /// </summary>
        Immobile = 8, 

        /// <summary>
        ///     Target is dashing to a known location, <c>skillshot</c> will hit.
        /// </summary>
        Dashing = 7, 

        /// <summary>
        ///     Very High Prediction output, <c>skillshot</c> will probably hit.
        /// </summary>
        VeryHigh = 6, 

        /// <summary>
        ///     High Prediction output, <c>skillshot</c> will probably hit.
        /// </summary>
        High = 5, 

        /// <summary>
        ///     Medium Prediction output, accuracy considered low.
        /// </summary>
        Medium = 4, 

        /// <summary>
        ///     Low Prediction output, accuracy considered low.
        /// </summary>
        Low = 3, 

        /// <summary>
        ///     Impossible Hit.
        /// </summary>
        Impossible = 2, 

        /// <summary>
        ///     <c>skillshot</c> is out of range.
        /// </summary>
        OutOfRange = 1, 

        /// <summary>
        ///     Collision before hit onto target.
        /// </summary>
        Collision = 0, 

        /// <summary>
        ///     No HitChance.
        /// </summary>
        None = -1
    }
}