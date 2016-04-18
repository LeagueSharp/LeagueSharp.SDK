// <copyright file="DangerLevel.cs" company="LeagueSharp">
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
    ///     The danger level of the spell.
    /// </summary>
    public enum DangerLevel
    {
        /// <summary>
        ///     Low danger level
        /// </summary>
        Low, 

        /// <summary>
        ///     Medium danger level, should be interrupted
        /// </summary>
        Medium, 

        /// <summary>
        ///     High danger level, definitely should be interrupted
        /// </summary>
        High
    }
}