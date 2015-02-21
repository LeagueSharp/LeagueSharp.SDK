using System;
using System.Collections.Generic;

namespace LeagueSharp.CommonEx.Core.Extensions.SharpDX
{
    /// <summary>
    ///     SharpDX/Vector3 Extensions
    /// </summary>
    public static class Vector3
    {
        /// <summary>
        ///     Checks for if the extended Vector3 is valid.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this global::SharpDX.Vector3 vector3)
        {
            return vector3 != global::SharpDX.Vector3.Zero;
        }

        /// <summary>
        ///     Normalizes a Vector3
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <returns>Normalized Vector3</returns>
        public static global::SharpDX.Vector3 Normalized(this global::SharpDX.Vector3 vector3)
        {
            vector3.Normalize();
            return vector3;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector3 to the Extended Vector3
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector3</returns>
        public static global::SharpDX.Vector3 Perpendicular(this global::SharpDX.Vector3 vector3, int offset = 0)
        {
            return (offset == 0)
                ? new global::SharpDX.Vector3(-vector3.X, vector3.Y, vector3.Z)
                : new global::SharpDX.Vector3(vector3.X, -vector3.Y, vector3.Z);
        }

        /// <summary>
        ///     Rotates the Vector3 to a set angle
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector3</returns>
        public static global::SharpDX.Vector3 Rotated(this global::SharpDX.Vector3 vector3, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new global::SharpDX.Vector3(
                (float) (vector3.X * cos - vector3.Y * sin), (float) (vector3.Y * cos + vector3.X * sin), vector3.Z);
        }

        #region Closest

        /// <summary>
        ///     Seeks for the closest Vector3 to the extended Vector3
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static global::SharpDX.Vector3 Closest(this global::SharpDX.Vector3 vector3,
            IEnumerable<global::SharpDX.Vector3> array)
        {
            var result = global::SharpDX.Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector2 to the extended Vector3
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static global::SharpDX.Vector2 Closest(this global::SharpDX.Vector3 vector3,
            IEnumerable<global::SharpDX.Vector2> array)
        {
            var result = global::SharpDX.Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector4 to the extended Vector3
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3</param>
        /// <param name="array">Vector4 Collection</param>
        /// <returns>Closest Vector4</returns>
        public static global::SharpDX.Vector4 Closest(this global::SharpDX.Vector3 vector3,
            IEnumerable<global::SharpDX.Vector4> array)
        {
            var result = global::SharpDX.Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector3.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        #endregion

        #region Extend

        /// <summary>
        ///     Extends a Vector3 to another Vector3
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector3</returns>
        public static global::SharpDX.Vector3 Extend(this global::SharpDX.Vector3 vector3,
            global::SharpDX.Vector3 toVector3,
            float distance)
        {
            return vector3 + distance * (toVector3 - vector3).Normalized();
        }

        /// <summary>
        ///     Extends a Vector3 to a Vector2
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector3</returns>
        public static global::SharpDX.Vector3 Extend(this global::SharpDX.Vector3 vector3,
            global::SharpDX.Vector2 toVector2,
            float distance)
        {
            return vector3 + distance * (toVector2.ToVector3(vector3.Z) - vector3).Normalized();
        }

        /// <summary>
        ///     Extends a Vector3 to a Vector4
        /// </summary>
        /// <param name="vector3">Extended SharpDX Vector3 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector3</returns>
        public static global::SharpDX.Vector3 Extend(this global::SharpDX.Vector3 vector3,
            global::SharpDX.Vector4 toVector4,
            float distance)
        {
            return vector3 + distance * (toVector4.ToVector3() - vector3).Normalized();
        }

        #endregion

        #region ToVector

        /// <summary>
        ///     Transforms an extended Vector3 into a Vector2
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <returns>Vector2</returns>
        public static global::SharpDX.Vector2 ToVector2(this global::SharpDX.Vector3 vector3)
        {
            return new global::SharpDX.Vector2(vector3.X, vector3.Y);
        }

        /// <summary>
        ///     Transforms an extended Vector3 into a Vector4
        /// </summary>
        /// <param name="vector3">SharpDX Vector3</param>
        /// <param name="w">Float W-axis (default = 0f)</param>
        /// <returns>Vector4</returns>
        public static global::SharpDX.Vector4 ToVector4(this global::SharpDX.Vector3 vector3, float w = 1f)
        {
            return new global::SharpDX.Vector4(vector3, w);
        }

        #endregion

        #region Distance

        /// <summary>
        ///     Calculates the distance between the extended Vector3 and a Vector3.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector3 vector3, global::SharpDX.Vector3 toVector3)
        {
            return global::SharpDX.Vector3.Distance(vector3, toVector3);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector3 and a Vector3.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector3 vector3, global::SharpDX.Vector2 toVector2)
        {
            return global::SharpDX.Vector3.Distance(vector3, toVector2.ToVector3());
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector3 and a Vector4.
        /// </summary>
        /// <param name="vector3">SharpDX Vector3 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector3 vector3, global::SharpDX.Vector4 toVector4)
        {
            return global::SharpDX.Vector3.Distance(vector3, toVector4.ToVector3());
        }

        #endregion
    }
}