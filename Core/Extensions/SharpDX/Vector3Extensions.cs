// <copyright file="Vector3Extensions.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     SharpDX/Vector3 Extensions
    /// </summary>
    public static partial class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.ToVector2().AngleBetween(toVector3);
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.ToVector2().AngleBetween(toVector2);
        }

        /// <summary>
        ///     Seeks for the closest Vector3 to the extended Vector3.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static Vector3 Closest(this Vector3 vector3, IEnumerable<Vector3> array)
        {
            return vector3.ToVector2().Closest(array);
        }

        /// <summary>
        ///     Seeks for the closest Vector2 to the extended Vector3.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static Vector2 Closest(this Vector3 vector3, IEnumerable<Vector2> array)
        {
            return vector3.ToVector2().Closest(array);
        }

        /// <summary>
        ///     Counts the ally heroes in range.
        /// </summary>
        /// <param name="vector3">The vector3.</param>
        /// <param name="range">The range.</param>
        /// <param name="originalUnit">The original unit.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountAllyHeroesInRange(this Vector3 vector3, float range, Obj_AI_Base originalUnit = null)
        {
            return GameObjects.AllyHeroes.Count(h => !h.Compare(originalUnit) && h.IsValidTarget(range, false, vector3));
        }

        /// <summary>
        ///     Counts the enemy heroes in range.
        /// </summary>
        /// <param name="vector3">The vector3.</param>
        /// <param name="range">The range.</param>
        /// <param name="originalUnit">The original unit.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountEnemyHeroesInRange(this Vector3 vector3, float range, Obj_AI_Base originalUnit = null)
        {
            return GameObjects.EnemyHeroes.Count(h => !h.Compare(originalUnit) && h.IsValidTarget(range, true, vector3));
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector3 and a Vector3.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.ToVector2().Distance(toVector3);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector3 and a Vector3.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.ToVector2().Distance(toVector2);
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector3 vector3, Vector3 toVector3)
        {
            return vector3.ToVector2().DistanceSquared(toVector3);
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector3 vector3, Vector2 toVector2)
        {
            return vector3.ToVector2().DistanceSquared(toVector2);
        }

        /// <summary>
        ///     Extends a Vector3 to another Vector3.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector3</returns>
        public static Vector3 Extend(this Vector3 vector3, Vector3 toVector3, float distance)
        {
            return vector3 + (distance * (toVector3 - vector3).Normalized());
        }

        /// <summary>
        ///     Extends a Vector3 to a Vector2.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector3</returns>
        public static Vector3 Extend(this Vector3 vector3, Vector2 toVector2, float distance)
        {
            return vector3 + (distance * (toVector2.ToVector3(vector3.Z) - vector3).Normalized());
        }

        /// <summary>
        ///     Gets the total distance of a list of vectors.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>Total distance of the path</returns>
        public static float GetPathLength(this List<Vector3> path)
        {
            var distance = 0f;

            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        /// <summary>
        ///     Returns if the Vector3 is on the screen.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// ///
        /// <param name="radius">Radius</param>
        /// <returns>Is Vector3 on screen</returns>
        public static bool IsOnScreen(this Vector3 vector3, float radius = 0f)
        {
            var pos = Drawing.WorldToScreen(vector3);
            return !(pos.X + radius < 0) && !(pos.X - radius > Drawing.Width) && !(pos.Y + radius < 0)
                   && !(pos.Y - radius > Drawing.Height);
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>Returns if the angle is orthogonal</returns>
        public static bool IsOrthogonal(Vector3 vector3, Vector3 toVector3)
        {
            return vector3.ToVector2().IsOrthogonal(toVector3);
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>Returns if the angle is orthogonal</returns>
        public static bool IsOrthogonal(Vector3 vector3, Vector2 toVector2)
        {
            return vector3.ToVector2().IsOrthogonal(toVector2);
        }

        /// <summary>
        ///     Returns whether the given position is under a ally turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector3</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderAllyTurret(this Vector3 position)
        {
            return position.ToVector2().IsUnderAllyTurret();
        }

        /// <summary>
        ///     Returns whether the given position is under a enemy turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector3</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderEnemyTurret(this Vector3 position)
        {
            return position.ToVector2().IsUnderEnemyTurret();
        }

        /// <summary>
        ///     Checks for if the extended Vector3 is valid.
        /// </summary>
        /// <param name="vector3">
        ///     SharpDX Vector3
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValid(this Vector3 vector3)
        {
            return vector3.ToVector2().IsValid();
        }

        /// <summary>
        ///     Returns if the Vector3 position is a wall.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <returns>Is Vector3 position a wall position</returns>
        public static bool IsWall(this Vector3 vector3)
        {
            return vector3.ToVector2().IsWall();
        }

        /// <summary>
        ///     Returns the calculated magnitude of the given Vector3.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <returns>Magnitude in float-units</returns>
        public static float Magnitude(this Vector3 vector3)
        {
            return (float)Math.Sqrt((vector3.X * vector3.X) + (vector3.Y * vector3.Y) + (vector3.Z * vector3.Z));
        }

        /// <summary>
        ///     Normalizes a Vector3.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <returns>Normalized Vector3</returns>
        public static Vector3 Normalized(this Vector3 vector3)
        {
            vector3.Normalize();
            return vector3;
        }

        /// <summary>
        ///     Returns the total distance of a path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float PathLength(this List<Vector3> path)
        {
            var distance = 0f;

            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector3 to the Extended Vector3.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector3</returns>
        public static Vector3 Perpendicular(this Vector3 vector3, int offset = 0)
        {
            return (offset == 0)
                       ? new Vector3(-vector3.Y, vector3.X, vector3.Z)
                       : new Vector3(vector3.Y, -vector3.X, vector3.Z);
        }

        /// <summary>
        ///     Returns the polar for vector angle (in Degrees).
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector2</param>
        /// <returns>Polar for Vector Angle (Degrees)</returns>
        public static float Polar(this Vector3 vector3)
        {
            return vector3.ToVector2().Polar();
        }

        /// <summary>
        ///     Converts the points to 2D, then returns the projection of the Vector2 on the segment.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="segmentStart">Start of Segment</param>
        /// <param name="segmentEnd">End of Segment</param>
        /// <returns><see cref="ProjectionInfo" /> containing the projection.</returns>
        public static ProjectionInfo ProjectOn(this Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            return point.ToVector2().ProjectOn(segmentStart.ToVector2(), segmentEnd.ToVector2());
        }

        /// <summary>
        ///     Rotates the Vector3 to a set angle.
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector3</returns>
        public static Vector3 Rotated(this Vector3 vector3, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector3(
                (float)((vector3.X * cos) - (vector3.Y * sin)),
                (float)((vector3.Y * cos) + (vector3.X * sin)),
                vector3.Z);
        }

        /// <summary>
        ///     Returns the modified Vector3 with a quick changed Z-axis value.
        /// </summary>
        /// <param name="v">Extended SharpDX Vector3</param>
        /// <param name="value">Switched Z value in float-units</param>
        /// <returns>Modified Vector3.</returns>
        public static Vector3 SetZ(this Vector3 v, float? value = null)
        {
            v.Z = value ?? Game.CursorPos.Z;

            return v;
        }

        /// <summary>
        ///     Transforms an extended Vector3 into a Vector2.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <returns>The <see cref="Vector2" /></returns>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }

        /// <summary>
        ///     Transforms an extended Vector3 List into a Vector2 List.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     Vector2 List
        /// </returns>
        public static List<Vector2> ToVector2(this List<Vector3> path)
        {
            return path.Select(point => point.ToVector2()).ToList();
        }

        #endregion
    }
}