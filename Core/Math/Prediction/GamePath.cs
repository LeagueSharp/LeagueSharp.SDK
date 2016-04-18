// <copyright file="Path.cs" company="LeagueSharp">
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
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     Path class, contains path tracker and a container
    /// </summary>
    public class GamePath
    {
        /// <summary>
        ///     Path Tracker class, tracks a given path.
        /// </summary>
        public static class PathTracker
        {
            #region Constants

            /// <summary>
            ///     Maximum time of a path track.
            /// </summary>
            private const double MaxTime = 1.5d;

            #endregion

            #region Static Fields

            /// <summary>
            ///     Stored Path list
            /// </summary>
            private static readonly Dictionary<int, List<StoredPath>> StoredPaths =
                new Dictionary<int, List<StoredPath>>();

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes static members of the <see cref="PathTracker" /> class.
            /// </summary>
            static PathTracker()
            {
                Obj_AI_Base.OnNewPath += Obj_AI_Hero_OnNewPath;
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Returns the current path of a specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <returns>
            ///     <see cref="StoredPath" />
            /// </returns>
            public static StoredPath GetCurrentPath(Obj_AI_Base unit)
            {
                List<StoredPath> value;
                return StoredPaths.TryGetValue(unit.NetworkId, out value) ? value.LastOrDefault() : new StoredPath();
            }

            /// <summary>
            ///     Returns the Root-mean-squared-speed of the specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <param name="maxT">Max time</param>
            /// <returns>The mean speed</returns>
            public static double GetMeanSpeed(Obj_AI_Base unit, double maxT)
            {
                var paths = GetStoredPaths(unit, MaxTime);
                var distance = 0d;
                if (paths.Count > 0)
                {
                    // Assume that the unit was moving for the first path:
                    distance += (maxT - paths[0].Time) * unit.MoveSpeed;

                    for (var i = 0; i < paths.Count - 1; i++)
                    {
                        var currentPath = paths[i];
                        var nextPath = paths[i + 1];

                        if (currentPath.WaypointCount > 0)
                        {
                            distance += Math.Min(
                                (currentPath.Time - nextPath.Time) * unit.MoveSpeed,
                                currentPath.Path.PathLength());
                        }
                    }

                    // Take into account the last path:
                    var lastPath = paths.Last();
                    if (lastPath.WaypointCount > 0)
                    {
                        distance += Math.Min(lastPath.Time * unit.MoveSpeed, lastPath.Path.PathLength());
                    }
                }
                else
                {
                    return unit.MoveSpeed;
                }

                return distance / maxT;
            }

            /// <summary>
            ///     Returns the stored paths from the list for a specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <param name="maxT">Max time</param>
            /// <returns>List of <see cref="StoredPath" /></returns>
            public static List<StoredPath> GetStoredPaths(Obj_AI_Base unit, double maxT)
            {
                List<StoredPath> value;
                return StoredPaths.TryGetValue(unit.NetworkId, out value)
                           ? value.Where(p => p.Time < maxT).ToList()
                           : new List<StoredPath>();
            }

            #endregion

            #region Methods

            /// <summary>
            ///     On New Path subscribed event function.
            /// </summary>
            /// <param name="sender"><see cref="Obj_AI_Base" /> sender.</param>
            /// <param name="args">Path Data</param>
            private static void Obj_AI_Hero_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
            {
                if (!(sender is Obj_AI_Hero))
                {
                    return;
                }

                if (!StoredPaths.ContainsKey(sender.NetworkId))
                {
                    StoredPaths.Add(sender.NetworkId, new List<StoredPath>());
                }

                var newPath = new StoredPath { Tick = Variables.TickCount, Path = args.Path.ToList().ToVector2() };
                StoredPaths[sender.NetworkId].Add(newPath);

                if (StoredPaths[sender.NetworkId].Count > 50)
                {
                    StoredPaths[sender.NetworkId].RemoveRange(0, 40);
                }
            }

            #endregion
        }

        /// <summary>
        ///     Stored Path Container, contains a stored path
        /// </summary>
        public class StoredPath
        {
            #region Public Properties

            /// <summary>
            ///     Gets the end point.
            /// </summary>
            public Vector2 EndPoint => this.Path.LastOrDefault();

            /// <summary>
            ///     Gets or sets the path.
            /// </summary>
            public List<Vector2> Path { get; set; }

            /// <summary>
            ///     Gets the start point.
            /// </summary>
            public Vector2 StartPoint => this.Path.FirstOrDefault();

            /// <summary>
            ///     Gets or sets the tick.
            /// </summary>
            public int Tick { get; set; }

            /// <summary>
            ///     Gets the current tick of the path.
            /// </summary>
            public double Time => (Variables.TickCount - this.Tick) / 1000d;

            /// <summary>
            ///     Gets the number of waypoints within the path.
            /// </summary>
            public int WaypointCount => this.Path.Count;

            #endregion
        }
    }
}