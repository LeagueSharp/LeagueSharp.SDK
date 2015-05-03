using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Math.Polygons;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Provides helpful extension and methods concerning Geometry.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        ///     Converts a list of lists of <see cref="IntPoint" /> to a polygon.
        /// </summary>
        /// <param name="v">List of <see cref="IntPoint" />.</param>
        /// <returns>List of polygons.</returns>
        public static List<Polygon> ToPolygons(this List<List<IntPoint>> v)
        {
            return v.Select(path => path.ToPolygon()).ToList();
        }

        /// <summary>
        ///     Gets the position after a set time, speed, and delay.
        /// </summary>
        /// <param name="self">List of <see cref="Vector2" />'s.</param>
        /// <param name="time">Time</param>
        /// <param name="speed">Speed</param>
        /// <param name="delay">Delay</param>
        /// <returns>The position after caculations</returns>
        public static Vector2 PositionAfter(this List<Vector2> self, int time, int speed, int delay = 0)
        {
            var distance = System.Math.Max(0, time - delay) * speed / 1000;
            for (var i = 0; i <= self.Count - 2; i++)
            {
                var from = self[i];
                var to = self[i + 1];
                var d = (int) to.Distance(from);
                if (d > distance)
                {
                    return from + distance * (to - from).Normalized();
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
        ///     Creates a double list of the list of <see cref="Polygon" /> broken up into <see cref="IntPoint" />'s.
        /// </summary>
        /// <param name="polygons">List of <see cref="Polygon" />s</param>
        /// <returns>Double list of <see cref="IntPoint" />, each list mathing a polygon that was broken up.</returns>
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
        /// <param name="path">Path</param>
        /// <param name="distance">Distance</param>
        /// <returns>The paths in range</returns>
        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var result = new List<Vector2>();
            for (var i = 0; i < path.Count - 1; i++)
            {
                var dist = path[i].Distance(path[i + 1]);
                if (dist > distance)
                {
                    result.Add(path[i] + distance * (path[i + 1] - path[i]).Normalized());

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
        ///     Returns the path of the unit appending the ServerPosition at the start, works even if the unit just entered fow.
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
            else if (WaypointTracker.StoredPaths.ContainsKey(unit.NetworkId))
            {
                var path = WaypointTracker.StoredPaths[unit.NetworkId];
                var timePassed = (Variables.TickCount - WaypointTracker.StoredTick[unit.NetworkId]) / 1000f;
                if (path.GetPathLength() >= unit.MoveSpeed * timePassed)
                {
                    result = CutPath(path, (int) (unit.MoveSpeed * timePassed));
                }
            }

            return result;
        }

        internal static class WaypointTracker
        {
            public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();
            public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();
        }
    }
}