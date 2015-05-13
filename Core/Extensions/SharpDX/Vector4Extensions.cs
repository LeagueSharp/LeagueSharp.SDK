// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vector4Extensions.cs" company="LeagueSharp">
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
//   SharpDX/Vector4 Extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Extensions.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::SharpDX;

    /// <summary>
    ///     SharpDX/Vector4 Extensions.
    /// </summary>
    public static class Vector4Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector4">SharpDX Vector4</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector4 vector4, Vector4 toVector4)
        {
            return AngleBetween(vector4, toVector4.ToVector3());
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector4 vector4, Vector2 toVector2)
        {
            return AngleBetween(vector4, toVector2.ToVector3());
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector4 vector4, Vector3 toVector3)
        {
            var magnitudeA = Math.Sqrt((vector4.X * vector4.X) + (vector4.Y * vector4.Y));
            var magnitudeB =
                Math.Sqrt((toVector3.X * toVector3.X) + (toVector3.Y * toVector3.Y) + (toVector3.Z * toVector3.Z));

            var dotProduct = (vector4.X * toVector3.X) + (vector4.Y * toVector3.Y);

            return (float)Math.Cos(dotProduct / magnitudeA * magnitudeB);
        }

        /// <summary>
        ///     Seeks for the closest Vector4 to the extended Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector4 Collection</param>
        /// <returns>Closest Vector4</returns>
        public static Vector4 Closest(this Vector4 vector4, IEnumerable<Vector4> array)
        {
            var result = Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector3 to the extended Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static Vector3 Closest(this Vector4 vector4, IEnumerable<Vector3> array)
        {
            var result = Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector2 to the extended Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static Vector2 Closest(this Vector4 vector4, IEnumerable<Vector2> array)
        {
            var result = Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector4.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector4.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector4 vector4, Vector4 toVector4)
        {
            return Vector4.Distance(vector4, toVector4);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector2.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector4 vector4, Vector2 toVector2)
        {
            return Vector4.Distance(vector4, toVector2.ToVector4());
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector3.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector4 vector4, Vector3 toVector3)
        {
            return Vector4.Distance(vector4, toVector3.ToVector4());
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector4">SharpDX Vector4</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector4 vector4, Vector4 toVector4)
        {
            return Vector4.DistanceSquared(vector4, toVector4);
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector4 vector4, Vector2 toVector2)
        {
            return Vector4.DistanceSquared(vector4, toVector2.ToVector4());
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector4 vector4, Vector3 toVector3)
        {
            return Vector4.DistanceSquared(vector4, toVector3.ToVector4());
        }

        /// <summary>
        ///     Extends a Vector4 to another Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static Vector4 Extend(this Vector4 vector4, Vector4 toVector4, float distance)
        {
            return vector4 + distance * (toVector4 - vector4).Normalized();
        }

        /// <summary>
        ///     Extends a Vector4 to a Vector2.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static Vector4 Extend(this Vector4 vector4, Vector2 toVector2, float distance)
        {
            return vector4 + distance * (toVector2.ToVector4(vector4.Z) - vector4).Normalized();
        }

        /// <summary>
        ///     Extends a Vector4 to a Vector3.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static Vector4 Extend(this Vector4 vector4, Vector3 toVector3, float distance)
        {
            return vector4 + distance * (toVector3.ToVector4() - vector4).Normalized();
        }

        /// <summary>
        ///     Returns if the Vector4 is on the screen.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <returns>Is Vector4 on screen</returns>
        public static bool IsOnScreen(this Vector4 vector4)
        {
            return vector4.ToVector3().IsOnScreen();
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector4">SharpDX Vector2</param>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsOrthogonal(this Vector4 vector4, Vector4 toVector4)
        {
            return IsOrthogonal(vector4, toVector4.ToVector3());
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsOrthogonal(this Vector4 vector4, Vector2 toVector2)
        {
            return IsOrthogonal(vector4, toVector2.ToVector3());
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector4">
        ///     Extended SharpDX Vector4
        /// </param>
        /// <param name="toVector3">
        ///     SharpDX Vector3
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsOrthogonal(this Vector4 vector4, Vector3 toVector3)
        {
            return Math.Abs((vector4.X * toVector3.X) + (vector4.Y * toVector3.Y)) < float.Epsilon;
        }

        /// <summary>
        ///     Returns whether the given position is under a turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector4</param>
        /// <param name="enemyTurretsOnly">Include Enemy Turret Only</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderTurret(this Vector4 position, bool enemyTurretsOnly)
        {
            return position.ToVector3().IsUnderTurret(enemyTurretsOnly);
        }

        /// <summary>
        ///     Checks for if the extended Vector4 is valid.
        /// </summary>
        /// <param name="vector4">
        ///     SharpDX Vector4
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValid(this Vector4 vector4)
        {
            return vector4 != Vector4.Zero;
        }

        /// <summary>
        ///     Returns if the Vector4 position is a wall.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <returns>Is Vector4 position a wall position</returns>
        public static bool IsWall(this Vector4 vector4)
        {
            return vector4.ToVector3().IsWall();
        }

        /// <summary>
        ///     Returns the calculated magnitude of the given Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <returns>Magnitude in float-units</returns>
        public static float Magnitude(this Vector4 vector4)
        {
            return (float)Math.Sqrt((vector4.X * vector4.X) + (vector4.Y * vector4.Y) + (vector4.Z * vector4.Z));
        }

        /// <summary>
        ///     Normalizes a Vector4.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4</param>
        /// <returns>Normalized Vector4</returns>
        public static Vector4 Normalized(this Vector4 vector4)
        {
            vector4.Normalize();
            return vector4;
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
        public static float PathLength(this List<Vector4> path)
        {
            var distance = 0f;
            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector4 to the Extended Vector4.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector4</returns>
        public static Vector4 Perpendicular(this Vector4 vector4, int offset = 0)
        {
            return (offset == 0)
                       ? new Vector4(-vector4.X, vector4.Y, vector4.Z, vector4.W)
                       : new Vector4(vector4.X, -vector4.Y, vector4.Z, vector4.W);
        }

        /// <summary>
        ///     Returns the polar for vector angle (in Degrees).
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <returns>Polar for Vector Angle (Degrees)</returns>
        public static float Polar(this Vector4 vector4)
        {
            if (Math.Abs(vector4.X - 0) <= (float)1e-9)
            {
                return (vector4.Y > 0) ? 90 : (vector4.Y < 0) ? 270 : 0;
            }

            var theta = (float)(Math.Atan(vector4.Y / vector4.X) * (180 / Math.PI));
            if (vector4.X < 0)
            {
                theta += 180;
            }

            if (theta < 0)
            {
                theta += 180;
            }

            return theta;
        }

        /// <summary>
        ///     Rotates the Vector4 to a set angle.
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector4</returns>
        public static Vector4 Rotated(this Vector4 vector4, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector4(
                (float)(vector4.X * cos - vector4.Y * sin), 
                (float)(vector4.Y * cos + vector4.X * sin), 
                vector4.Z, 
                vector4.W);
        }

        /// <summary>
        ///     Returns the modified Vector4 with a quick changed W-axis value.
        /// </summary>
        /// <param name="v">Extended SharpDX Vector4</param>
        /// <param name="value">Switched W value in float-units</param>
        /// <returns>Modified Vector4.</returns>
        public static Vector4 SetW(this Vector4 v, float? value = null)
        {
            if (value == null)
            {
                v.W = 1.0f;
            }
            else
            {
                v.W = (float)value;
            }

            return v;
        }

        /// <summary>
        ///     Returns the modified Vector4 with a quick changed Z-axis value.
        /// </summary>
        /// <param name="v">Extended SharpDX Vector4</param>
        /// <param name="value">Switched Z value in float-units</param>
        /// <returns>Modified Vector4.</returns>
        public static Vector4 SetZ(this Vector4 v, float? value = null)
        {
            if (value == null)
            {
                v.Z = Game.CursorPos.Z;
            }
            else
            {
                v.Z = (float)value;
            }

            return v;
        }

        /// <summary>
        ///     Transforms an extended Vector4 into a Vector2.
        /// </summary>
        /// <param name="vector4">SharpDX Vector3</param>
        /// <returns>The <see cref="Vector2" /></returns>
        public static Vector2 ToVector2(this Vector4 vector4)
        {
            return new Vector2(vector4.X, vector4.Y);
        }

        /// <summary>
        ///     Transforms an extended Vector4 List into a Vector2 List.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     Vector3 List
        /// </returns>
        public static List<Vector2> ToVector2(this List<Vector4> path)
        {
            return path.Select(point => point.ToVector2()).ToList();
        }

        /// <summary>
        ///     Transforms an extended Vector3 into a Vector4.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4</param>
        /// <returns>The <see cref="Vector3" /></returns>
        public static Vector3 ToVector3(this Vector4 vector4)
        {
            return new Vector3(vector4.X, vector4.Y, vector4.Z);
        }

        /// <summary>
        ///     Transforms an extended Vector4 List into a Vector3 List.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     Vector4 List
        /// </returns>
        public static List<Vector3> ToVector3(this List<Vector4> path)
        {
            return path.Select(point => point.ToVector3()).ToList();
        }

        #endregion
    }
}