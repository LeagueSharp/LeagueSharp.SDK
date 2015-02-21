using System;
using System.Collections.Generic;

namespace LeagueSharp.CommonEx.Core.Extensions.SharpDX
{
    /// <summary>
    ///     SharpDX/Vector4 Extensions
    /// </summary>
    public static class Vector4
    {
        /// <summary>
        ///     Checks for if the extended Vector4 is valid.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this global::SharpDX.Vector4 vector4)
        {
            return vector4 != global::SharpDX.Vector4.Zero;
        }

        /// <summary>
        ///     Normalizes a Vector4
        /// </summary>
        /// <param name="vector4">SharpDX Vector4</param>
        /// <returns>Normalized Vector4</returns>
        public static global::SharpDX.Vector4 Normalized(this global::SharpDX.Vector4 vector4)
        {
            vector4.Normalize();
            return vector4;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector4 to the Extended Vector4
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector4</returns>
        public static global::SharpDX.Vector4 Perpendicular(this global::SharpDX.Vector4 vector4, int offset = 0)
        {
            return (offset == 0)
                ? new global::SharpDX.Vector4(-vector4.X, vector4.Y, vector4.Z, vector4.W)
                : new global::SharpDX.Vector4(vector4.X, -vector4.Y, vector4.Z, vector4.W);
        }

        /// <summary>
        ///     Rotates the Vector4 to a set angle
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector4</returns>
        public static global::SharpDX.Vector4 Rotated(this global::SharpDX.Vector4 vector4, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new global::SharpDX.Vector4(
                (float) (vector4.X * cos - vector4.Y * sin), (float) (vector4.Y * cos + vector4.X * sin), vector4.Z,
                vector4.W);
        }

        #region Closest

        /// <summary>
        ///     Seeks for the closest Vector4 to the extended Vector4
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector4 Collection</param>
        /// <returns>Closest Vector4</returns>
        public static global::SharpDX.Vector4 Closest(this global::SharpDX.Vector4 vector4,
            IEnumerable<global::SharpDX.Vector4> array)
        {
            var result = global::SharpDX.Vector4.Zero;
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
        ///     Seeks for the closest Vector3 to the extended Vector4
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static global::SharpDX.Vector3 Closest(this global::SharpDX.Vector4 vector4,
            IEnumerable<global::SharpDX.Vector3> array)
        {
            var result = global::SharpDX.Vector3.Zero;
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
        ///     Seeks for the closest Vector2 to the extended Vector4
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static global::SharpDX.Vector2 Closest(this global::SharpDX.Vector4 vector4,
            IEnumerable<global::SharpDX.Vector2> array)
        {
            var result = global::SharpDX.Vector2.Zero;
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

        #endregion

        #region Extend

        /// <summary>
        ///     Extends a Vector4 to another Vector4
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static global::SharpDX.Vector4 Extend(this global::SharpDX.Vector4 vector4,
            global::SharpDX.Vector4 toVector4,
            float distance)
        {
            return vector4 + distance * (toVector4 - vector4).Normalized();
        }

        /// <summary>
        ///     Extends a Vector4 to a Vector2
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static global::SharpDX.Vector4 Extend(this global::SharpDX.Vector4 vector4,
            global::SharpDX.Vector2 toVector2,
            float distance)
        {
            return vector4 + distance * (toVector2.ToVector4(vector4.Z) - vector4).Normalized();
        }

        /// <summary>
        ///     Extends a Vector4 to a Vector3
        /// </summary>
        /// <param name="vector4">Extended SharpDX Vector4 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector4</returns>
        public static global::SharpDX.Vector4 Extend(this global::SharpDX.Vector4 vector4,
            global::SharpDX.Vector3 toVector3,
            float distance)
        {
            return vector4 + distance * (toVector3.ToVector4() - vector4).Normalized();
        }

        #endregion

        #region ToVector

        /// <summary>
        ///     Transforms an extended Vector4 into a Vector2
        /// </summary>
        /// <param name="vector4">SharpDX Vector3</param>
        /// <returns>Vector2</returns>
        public static global::SharpDX.Vector2 ToVector2(this global::SharpDX.Vector4 vector4)
        {
            return new global::SharpDX.Vector2(vector4.X, vector4.Y);
        }

        /// <summary>
        ///     Transforms an extended Vector3 into a Vector4
        /// </summary>
        /// <param name="vector4">SharpDX Vector4</param>
        /// <returns>Vector4</returns>
        public static global::SharpDX.Vector3 ToVector3(this global::SharpDX.Vector4 vector4)
        {
            return new global::SharpDX.Vector3(vector4.X, vector4.Y, vector4.Z);
        }

        #endregion

        #region Distance

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector4.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector4 vector4, global::SharpDX.Vector4 toVector4)
        {
            return global::SharpDX.Vector4.Distance(vector4, toVector4);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector2.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector4 vector4, global::SharpDX.Vector2 toVector2)
        {
            return global::SharpDX.Vector4.Distance(vector4, toVector2.ToVector4());
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector4 and a Vector3.
        /// </summary>
        /// <param name="vector4">SharpDX Vector4 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this global::SharpDX.Vector4 vector4, global::SharpDX.Vector3 toVector3)
        {
            return global::SharpDX.Vector4.Distance(vector4, toVector3.ToVector4());
        }

        #endregion
    }
}