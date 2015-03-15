#region

using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Math.Prediction
{
    /// <summary>
    ///     Path class, contains path tracker and a containter
    /// </summary>
    internal class Path
    {
        /// <summary>
        ///     Stored Path Container, contains a stored path
        /// </summary>
        internal class StoredPath
        {
            /// <summary>
            ///     Vector2 list of the path.
            /// </summary>
            public List<Vector2> Path;

            /// <summary>
            ///     Tick of the stored path.
            /// </summary>
            public int Tick;

            /// <summary>
            ///     Currect tick of the path.
            /// </summary>
            public double Time
            {
                get { return (Variables.TickCount - Tick) / 1000d; }
            }

            /// <summary>
            ///     Number of waypoints within the path.
            /// </summary>
            public int WaypointCount
            {
                get { return Path.Count; }
            }

            /// <summary>
            ///     Starting point of the path.
            /// </summary>
            public Vector2 StartPoint
            {
                get { return Path.FirstOrDefault(); }
            }

            /// <summary>
            ///     Ending point of the path.
            /// </summary>
            public Vector2 EndPoint
            {
                get { return Path.LastOrDefault(); }
            }
        }

        /// <summary>
        ///     Path Tracker class, tracks a given path.
        /// </summary>
        internal static class PathTracker
        {
            /// <summary>
            ///     Maximum time of a path track.
            /// </summary>
            private const double MaxTime = 1.5d;

            /// <summary>
            ///     Stored Path list
            /// </summary>
            private static readonly Dictionary<int, List<StoredPath>> StoredPaths =
                new Dictionary<int, List<StoredPath>>();

            /// <summary>
            ///     Static consturctor
            /// </summary>
            static PathTracker()
            {
                Obj_AI_Base.OnNewPath += Obj_AI_Hero_OnNewPath;
            }

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

            /// <summary>
            ///     Returns the stored paths from the list for a specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <param name="maxT">Max time</param>
            /// <returns>List of <see cref="StoredPath" /></returns>
            public static List<StoredPath> GetStoredPaths(Obj_AI_Base unit, double maxT)
            {
                return StoredPaths.ContainsKey(unit.NetworkId)
                    ? StoredPaths[unit.NetworkId].Where(p => p.Time < maxT).ToList()
                    : new List<StoredPath>();
            }

            /// <summary>
            ///     Returns the current path of a specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <returns>
            ///     <see cref="StoredPath" />
            /// </returns>
            public static StoredPath GetCurrentPath(Obj_AI_Base unit)
            {
                return StoredPaths.ContainsKey(unit.NetworkId)
                    ? StoredPaths[unit.NetworkId].LastOrDefault()
                    : new StoredPath();
            }

            /// <summary>
            ///     Returns the Root-mean-squared-speed of the specific unit.
            /// </summary>
            /// <param name="unit">The specific unit</param>
            /// <param name="maxT">Max time</param>
            /// <returns></returns>
            public static double GetMeanSpeed(Obj_AI_Base unit, double maxT)
            {
                var paths = GetStoredPaths(unit, MaxTime);
                var distance = 0d;
                if (paths.Count > 0)
                {
                    //Assume that the unit was moving for the first path:
                    distance += (maxT - paths[0].Time) * unit.MoveSpeed;

                    for (var i = 0; i < paths.Count - 1; i++)
                    {
                        var currentPath = paths[i];
                        var nextPath = paths[i + 1];

                        if (currentPath.WaypointCount > 0)
                        {
                            distance += System.Math.Min(
                                (currentPath.Time - nextPath.Time) * unit.MoveSpeed, currentPath.Path.PathLength());
                        }
                    }

                    //Take into account the last path:
                    var lastPath = paths.Last();
                    if (lastPath.WaypointCount > 0)
                    {
                        distance += System.Math.Min(lastPath.Time * unit.MoveSpeed, lastPath.Path.PathLength());
                    }
                }
                else
                {
                    return unit.MoveSpeed;
                }


                return distance / maxT;
            }
        }
    }
}