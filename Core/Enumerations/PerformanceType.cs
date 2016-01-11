// <copyright file="PerformanceType.cs" company="LeagueSharp">
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
    ///     Performance Type to log.
    /// </summary>
    public enum PerformanceType
    {
        /// <summary>
        ///     Logs the Tick Count(CPU Ticks).
        /// </summary>
        TickCount, 

        /// <summary>
        ///     Logs the number of milliseconds.
        /// </summary>
        Milliseconds, 

        /// <summary>
        ///     Logs the time spanned in TimeSpam format.
        /// </summary>
        TimeSpan
    }
}