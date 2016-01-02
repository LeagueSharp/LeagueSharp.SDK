// <copyright file="CenteredFlags.cs" company="LeagueSharp">
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
    ///     CenteredText Drawing Flags
    /// </summary>
    [Flags]
    public enum CenteredFlags
    {
        /// <summary>
        ///     None Flag
        /// </summary>
        None = 0, 

        /// <summary>
        ///     Center Horizontally Left.
        /// </summary>
        HorizontalLeft = 1 << 0, 

        /// <summary>
        ///     Center Horizontally.
        /// </summary>
        HorizontalCenter = 1 << 1, 

        /// <summary>
        ///     Center Horizontally Right.
        /// </summary>
        HorizontalRight = 1 << 2, 

        /// <summary>
        ///     Center Vertically Up.
        /// </summary>
        VerticalUp = 1 << 3, 

        /// <summary>
        ///     Center Vertically.
        /// </summary>
        VerticalCenter = 1 << 4, 

        /// <summary>
        ///     Center Vertically Down.
        /// </summary>
        VerticalDown = 1 << 5
    }
}