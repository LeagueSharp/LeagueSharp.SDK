// <copyright file="SkillshotType.cs" company="LeagueSharp">
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
    ///     <c>Skillshot</c> Type Enumeration
    /// </summary>
    public enum SkillshotType
    {
        /// <summary>
        ///     Determines that the <c>skillshot</c> is a linear <c>skillshot</c>.
        /// </summary>
        SkillshotLine, 

        /// <summary>
        ///     Determines that the <c>skillshot</c> is a circle <c>skillshot</c>.
        /// </summary>
        SkillshotCircle, 

        /// <summary>
        ///     Determines that the <c>skillshot</c> is a cone <c>skillshot</c>.
        /// </summary>
        SkillshotCone
    }
}