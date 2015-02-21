using System;
using System.Collections.Generic;

namespace LeagueSharp.CommonEx.Core.Extensions.SharpDX
{
    /// <summary>
    ///     SharpDX/Vector2 Extensions
    /// </summary>
    public static class Vector2
    {
        /// <summary>
        ///     Checks for if the extended Vector2 is valid.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this global::SharpDX.Vector2 vector2)
        {
            return vector2 != global::SharpDX.Vector2.Zero;
        }

        /// <summary>
        ///     Normalizes a Vector2
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <returns>Normalized Vector2</returns>
        public static global::SharpDX.Vector2 Normalized(this global::SharpDX.Vector2 vector2)
        {
            vector2.Normalize();
            return vector2;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector2 to the Extended Vector2
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector2</returns>
        public static global::SharpDX.Vector2 Perpendicular(this global::SharpDX.Vector2 vector2, int offset = 0)
        {
            return (offset == 0)
                ? new global::SharpDX.Vector2(-vector2.X, vector2.Y)
                : new global::SharpDX.Vector2(vector2.X, -vector2.Y);
        }

        /// <summary>
        ///     Rotates the Vector2 to a set angle
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector2</returns>
        public static global::SharpDX.Vector2 Rotated(this global::SharpDX.Vector2 vector2, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new global::SharpDX.Vector2(
                (float) (vector2.X * cos - vector2.Y * sin), (float) (vector2.Y * cos + vector2.X * sin));
        }

        #region Closest

        /// <summary>
        ///     Seeks for the closest Vector2 to the extended Vector2
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static global::SharpDX.Vector2 Closest(this global::SharpDX.Vector2 vector2,
            IEnumerable<global::SharpDX.Vector2> array)
        {
            var result = global::SharpDX.Vector2.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector3 to the extended Vector2
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static global::SharpDX.Vector3 Closest(this global::SharpDX.Vector2 vector2,
            IEnumerable<global::SharpDX.Vector3> array)
        {
            var result = global::SharpDX.Vector3.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
                if (distance < temporaryDistance)
                {
                    distance = temporaryDistance;
                    result = vector;
                }
            }

            return result;
        }

        /// <summary>
        ///     Seeks for the closest Vector4 to the extended Vector2
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector4 Collection</param>
        /// <returns>Closest Vector4</returns>
        public static global::SharpDX.Vector4 Closest(this global::SharpDX.Vector2 vector2,
            IEnumerable<global::SharpDX.Vector4> array)
        {
            var result = global::SharpDX.Vector4.Zero;
            var distance = float.MaxValue;

            foreach (var vector in array)
            {
                var temporaryDistance = vector2.Distance(vector);
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
        ///     Extends a Vector2 to another Vector2
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static global::SharpDX.Vector2 Extend(this global::SharpDX.Vector2 vector2,
            global::SharpDX.Vector2 toVector2,
            float distance)
        {
            return vector2 + distance * (toVector2 - vector2).Normalized();
        }

        /// <summary>
        ///     Extends a Vector2 to a Vector3
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static global::SharpDX.Vector2 Extend(this global::SharpDX.Vector2 vector2,
            global::SharpDX.Vector3 toVector3,
            float distance)
        {
            return vector2 + distance * (toVector3.ToVector2() - vector2).Normalized();
        }

        /// <summary>
        ///     Extends a Vector2 to a Vector4
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static global::SharpDX.Vector2 Extend(this global::SharpDX.Vector2 vector2,
            global::SharpDX.Vector4 toVector4,
            float distance)
        {
            return vector2 + distance * (toVector4.ToVector2() - vector2).Normalized();
        }

        #endregion

        #region ToVector

        /// <summary>
        ///     Transforms an extended Vector2 into a Vector3
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <param name="z">Float Z-axis (default = 0f)</param>
        /// <returns>Vector3</returns>
        public static global::SharpDX.Vector3 ToVector3(this global::SharpDX.Vector2 vector2, float z = 0f)
        {
            return new global::SharpDX.Vector3(vector2, z);
        }

        /// <summary>
        ///     Transforms an extended Vector2 into a Vector4
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <param name="z">Float Z-axis (default = 0f)</param>
        /// <param name="w">Float W-axis (default = 0f)</param>
        /// <returns>Vector4</returns>
        public static global::SharpDX.Vector4 ToVector4(this global::SharpDX.Vector2 vector2, float z = 0f, float w = 1f)
        {
            return new global::SharpDX.Vector4(vector2, z, w);
        }

        #endregion

        #region Distance

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector2.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector2 vector2, global::SharpDX.Vector2 toVector2)
        {
            return global::SharpDX.Vector2.Distance(vector2, toVector2);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector3.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector2 vector2, global::SharpDX.Vector3 toVector3)
        {
            return global::SharpDX.Vector2.Distance(vector2, toVector3.ToVector2());
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector4.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector2 vector2, global::SharpDX.Vector4 toVector4)
        {
            return global::SharpDX.Vector2.Distance(vector2, toVector4.ToVector2());
        }

        #endregion
    }
}