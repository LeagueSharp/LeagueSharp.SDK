// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Map.cs" company="LeagueSharp">
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
//   Utility for the Maps in League of Legends.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Wrappers
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.Enumerations;

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
        private static readonly IDictionary<int, Map> MapById = new Dictionary<int, Map>
                                                                    {
                                                                        {
                                                                            8, 
                                                                            new Map
                                                                                {
                                                                                    Name = "The Crystal Scar", 
                                                                                    ShortName = "crystalScar", 
                                                                                    Type = MapType.CrystalScar, 
                                                                                    Grid =
                                                                                        new Vector2(13894f / 2, 13218f / 2), 
                                                                                    StartingLevel = 3
                                                                                }
                                                                        }, 
                                                                        {
                                                                            10, 
                                                                            new Map
                                                                                {
                                                                                    Name = "The Twisted Treeline", 
                                                                                    ShortName = "twistedTreeline", 
                                                                                    Type = MapType.TwistedTreeline, 
                                                                                    Grid =
                                                                                        new Vector2(15436f / 2, 14474f / 2), 
                                                                                    StartingLevel = 1
                                                                                }
                                                                        }, 
                                                                        {
                                                                            11, 
                                                                            new Map
                                                                                {
                                                                                    Name = "Summoner's Rift", 
                                                                                    ShortName = "summonerRift", 
                                                                                    Type = MapType.SummonersRift, 
                                                                                    Grid =
                                                                                        new Vector2(13982f / 2, 14446f / 2), 
                                                                                    StartingLevel = 1
                                                                                }
                                                                        }, 
                                                                        {
                                                                            12, 
                                                                            new Map
                                                                                {
                                                                                    Name = "Howling Abyss", 
                                                                                    ShortName = "howlingAbyss", 
                                                                                    Type = MapType.HowlingAbyss, 
                                                                                    Grid =
                                                                                        new Vector2(13120f / 2, 12618f / 2), 
                                                                                    StartingLevel = 3
                                                                                }
                                                                        }
                                                                    };

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
        public MapType Type { get; private set; }

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
                           Name = "Unknown", ShortName = "unknown", Type = MapType.Unknown, Grid = new Vector2(0, 0), 
                           StartingLevel = 1
                       };
        }

        #endregion
    }
}