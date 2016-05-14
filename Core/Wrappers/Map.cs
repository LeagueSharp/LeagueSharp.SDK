// <copyright file="Map.cs" company="LeagueSharp">
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
    using System.Collections.Generic;

    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;

    using SharpDX;

    /// <summary>
    ///     Utility for the Maps in League of Legends.
    /// </summary>
    public class Map
    {
        #region Static Fields

        /// <summary>
        ///     Map by ID list.
        /// </summary>
        public static IReadOnlyDictionary<int, MapDataEntry> Maps = Data.Get<MapData>().Maps;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the current game map.
        /// </summary>
        /// <returns>
        ///     The current map information.
        /// </returns>
        public static MapDataEntry GetMap()
        {
            if (Maps.ContainsKey((int)Game.MapId))
            {
                return Maps[(int)Game.MapId];
            }

            return new MapDataEntry
                       {
                           Name = "Unknown", ShortName = "unknown", MapId = 0, Grid = new Vector2(0, 0), StartingLevel = 1
                       };
        }

        #endregion
    }
}