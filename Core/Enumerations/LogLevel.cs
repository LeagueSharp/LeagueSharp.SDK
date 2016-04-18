// <copyright file="LogLevel.cs" company="LeagueSharp">
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
    ///     The level of the information being logged
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     Debug Information
        /// </summary>
        Debug = 2, 

        /// <summary>
        ///     An error occurred somewhere in the code (exception)
        /// </summary>
        Error = 5, 

        /// <summary>
        ///     An error occurred and the program is unable to proceed
        /// </summary>
        Fatal = 6, 

        /// <summary>
        ///     General information
        /// </summary>
        Info = 1, 

        /// <summary>
        ///     Current location of the program
        /// </summary>
        Trace = 3, 

        /// <summary>
        ///     Warning level
        /// </summary>
        Warn = 4
    }
}