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

namespace LeagueSharp.SDK.Core.Wrappers
{
    using System.Collections.Generic;

    using Enumerations;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Utility for the Maps in League of Legends.
    /// </summary>
    [ResourceImport]
    public class Map
    {
        #region Static Fields

        /// <summary>
        ///     Map by ID list.
        /// </summary>
        [ResourceImport("Data.Map.json")]
        public static IDictionary<int, Map> MapById = new Dictionary<int, Map>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Grid of the Map
        /// </summary>
        public Vector2 Grid { get; private set; }

        /// <summary>
        ///     Gets the name of the map
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the short name of the map.
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        ///     Gets the level the players start at
        /// </summary>
        public int StartingLevel { get; private set; }

        /// <summary>
        ///     Gets the MapType
        /// </summary>
        public GameMapId MapId { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the current map.
        /// </summary>
        /// <returns>The current map</returns>
        public static Map GetMap()
        {
            if (MapById.ContainsKey((int)Game.MapId))
            {
                return MapById[(int)Game.MapId];
            }

            return new Map
                       {
                           Name = "Unknown", ShortName = "unknown", MapId = (GameMapId) 0, Grid = new Vector2(0, 0),
                           StartingLevel = 1
                       };
        }

        #endregion
    }
}