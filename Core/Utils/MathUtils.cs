// <copyright file="MathUtils.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Polygons;

    using SharpDX;

    /// <summary>
    ///     Provides helpful extension and methods concerning Geometry.
    /// </summary>
    public static class MathUtils
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Creates a double list of the list of <see cref="Polygon" /> broken up into <see cref="IntPoint" />'s.
        /// </summary>
        /// <param name="polygons">List of <see cref="Polygon" />s</param>
        /// <returns>Double list of <see cref="IntPoint" />, each list clipping a polygon that was broken up.</returns>
        public static List<List<IntPoint>> ClipPolygons(List<Polygon> polygons)
        {
            var subj = new List<List<IntPoint>>(polygons.Count);
            var clip = new List<List<IntPoint>>(polygons.Count);

            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }

            var solution = new List<List<IntPoint>>();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.PtSubject, true);
            c.AddPaths(clip, PolyType.PtClip, true);
            c.Execute(ClipType.CtUnion, solution, PolyFillType.PftPositive, PolyFillType.PftEvenOdd);

            return solution;
        }

        /// <summary>
        ///     Removes vectors past a distance from a list.
        /// </summary>
        /// <param name="path">The Path</param>
        /// <param name="distance">The Distance</param>
        /// <returns>The paths in range</returns>
        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var result = new List<Vector2>();
            for (var i = 0; i < path.Count - 1; i++)
            {
                var dist = path[i].Distance(path[i + 1]);
                if (dist > distance)
                {
                    result.Add(path[i] + (distance * (path[i + 1] - path[i]).Normalized()));

                    for (var j = i + 1; j < path.Count; j++)
                    {
                        result.Add(path[j]);
                    }

                    break;
                }

                distance -= dist;
            }

            return result.Count > 0 ? result : new List<Vector2> { path.Last() };
        }

        /// <summary>
        ///     Returns the path of the unit appending the ServerPosition at the start, works even if the unit just entered fog of
        ///     war.
        /// </summary>
        /// <param name="unit">Unit to get the waypoints for</param>
        /// <returns>List of waypoints(<see cref="Vector2" />)</returns>
        public static List<Vector2> GetWaypoints(this Obj_AI_Base unit)
        {
            var result = new List<Vector2>();

            if (unit.IsVisible)
            {
                result.Add(unit.ServerPosition.ToVector2());
                result.AddRange(unit.Path.Select(point => point.ToVector2()));
            }
            else
            {
                List<Vector2> value;
                if (WaypointTracker.StoredPaths.TryGetValue(unit.NetworkId, out value))
                {
                    var path = value;
                    var timePassed = (Variables.TickCount - WaypointTracker.StoredTick[unit.NetworkId]) / 1000f;
                    if (path.GetPathLength() >= unit.MoveSpeed * timePassed)
                    {
                        result = CutPath(path, (int)(unit.MoveSpeed * timePassed));
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets the position after a set time, speed, and delay.
        /// </summary>
        /// <param name="self">List of <see cref="Vector2" />'s.</param>
        /// <param name="time">The Time</param>
        /// <param name="speed">The Speed</param>
        /// <param name="delay">The Delay</param>
        /// <returns>The position after calculations</returns>
        public static Vector2 PositionAfter(this List<Vector2> self, int time, int speed, int delay = 0)
        {
            var distance = Math.Max(0, time - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int)to.Distance(from);
                if (d > distance)
                {
                    return from + (distance * (to - from).Normalized());
                }

                distance -= d;
            }

            return self[self.Count - 1];
        }

        /// <summary>
        ///     Converts a list of <see cref="IntPoint" />s to a <see cref="Polygon" />
        /// </summary>
        /// <param name="list">List of <see cref="Polygon" /></param>
        /// <returns>Polygon made up of <see cref="IntPoint" />s</returns>
        public static Polygon ToPolygon(this List<IntPoint> list)
        {
            var polygon = new Polygon();
            foreach (var point in list)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }

            return polygon;
        }

        /// <summary>
        ///     Converts a list of lists of <see cref="IntPoint" /> to a polygon.
        /// </summary>
        /// <param name="v">List of <see cref="IntPoint" />.</param>
        /// <returns>List of polygons.</returns>
        public static List<Polygon> ToPolygons(this List<List<IntPoint>> v)
        {
            return v.Select(path => path.ToPolygon()).ToList();
        }

        #endregion

        /// <summary>
        ///     Waypoint Tracker data container.
        /// </summary>
        internal static class WaypointTracker
        {
            #region Static Fields

            /// <summary>
            ///     Stored Paths.
            /// </summary>
            public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();

            /// <summary>
            ///     Stored Ticks.
            /// </summary>
            public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();

            #endregion
        }
    }
}