// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinionOrderTypes.cs" company="LeagueSharp">
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
//   Enumeration that specifies how to order minions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Enumerations
{
    /// <summary>
    ///     Enumeration that specifies how to order minions
    /// </summary>
    public enum MinionOrderTypes
    {
        /// <summary>
        ///     Don't order minion
        /// </summary>
        None, 

        /// <summary>
        ///     Order by health
        /// </summary>
        Health, 

        /// <summary>
        ///     Order by the maximum health
        /// </summary>
        MaxHealth
    }
}