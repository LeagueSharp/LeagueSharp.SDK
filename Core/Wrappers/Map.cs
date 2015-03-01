using System.Collections.Generic;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Utility for the Maps in League of Legends.
    /// </summary>
    public class Map
    {
        /// <summary>
        ///     Enum of the names of maps.
        /// </summary>
        public enum MapType
        {
            /// <summary>
            ///     Crystal Scar(Dominion)
            /// </summary>
            CrystalScar,

            /// <summary>
            ///     Howling Abyss(ARAM)
            /// </summary>
            HowlingAbyss,

            /// <summary>
            ///     Summoners Rift(5v5)
            /// </summary>
            SummonersRift,

            /// <summary>
            ///     Twisted Tree Line(3v3)
            /// </summary>
            TwistedTreeline,

            /// <summary>
            ///     Unknown map.(New gamemode/Map)
            /// </summary>
            Unknown
        }

        private static readonly IDictionary<int, Map> MapById = new Dictionary<int, Map>
        {
            {
                8,
                new Map
                {
                    Name = "The Crystal Scar",
                    ShortName = "crystalScar",
                    Type = MapType.CrystalScar,
                    Grid = new Vector2(13894f / 2, 13218f / 2),
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
                    Grid = new Vector2(15436f / 2, 14474f / 2),
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
                    Grid = new Vector2(13982f / 2, 14446f / 2),
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
                    Grid = new Vector2(13120f / 2, 12618f / 2),
                    StartingLevel = 3
                }
            }
        };

        /// <summary>
        ///     Gets the MapType
        /// </summary>
        public MapType Type { get; private set; }

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
        ///     Returns the current map.
        /// </summary>
        /// <returns>The current map</returns>
        public static Map GetMap()
        {
            if (MapById.ContainsKey((int) Game.MapId))
            {
                return MapById[(int) Game.MapId];
            }

            return new Map
            {
                Name = "Unknown",
                ShortName = "unknown",
                Type = MapType.Unknown,
                Grid = new Vector2(0, 0),
                StartingLevel = 1
            };
        }
    }
}