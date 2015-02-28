#region

using System.Collections.Generic;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Extensions.SharpDX
{
    /// <summary>
    ///     SharpDX/Vector2 Extensions.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        ///     Checks for if the extended Vector2 is valid.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this Vector2 vector2)
        {
            return vector2 != Vector2.Zero;
        }

        /// <summary>
        ///     Normalizes a Vector2.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <returns>Normalized Vector2</returns>
        public static Vector2 Normalized(this Vector2 vector2)
        {
            vector2.Normalize();
            return vector2;
        }

        /// <summary>
        ///     Returns the Perpendicular Vector2 to the Extended Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector2</returns>
        public static Vector2 Perpendicular(this Vector2 vector2, int offset = 0)
        {
            return (offset == 0) ? new Vector2(-vector2.X, vector2.Y) : new Vector2(vector2.X, -vector2.Y);
        }

        /// <summary>
        ///     Rotates the Vector2 to a set angle.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector2</returns>
        public static Vector2 Rotated(this Vector2 vector2, float angle)
        {
            var cos = System.Math.Cos(angle);
            var sin = System.Math.Sin(angle);

            return new Vector2((float) (vector2.X * cos - vector2.Y * sin), (float) (vector2.Y * cos + vector2.X * sin));
        }

        /// <summary>
        ///     Returns the polar for vector angle (in Degrees).
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Polar for Vector Angle (Degrees)</returns>
        public static float Polar(this Vector2 vector2)
        {
            if (System.Math.Abs(vector2.X - 0) <= (float) 1e-9)
            {
                return (vector2.Y > 0) ? 90 : (vector2.Y < 0) ? 270 : 0;
            }

            var theta = (float) (System.Math.Atan((vector2.Y) / (vector2.X)) * (180 / System.Math.PI));
            if (vector2.X < 0)
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
        ///     Returns the projection of the Vector2 on the segment.
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="segmentStart">Start of Segment</param>
        /// <param name="segmentEnd">End of Segment</param>
        /// <returns><see cref="ProjectionInfo" /> containing the projection.</returns>
        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay)) /
                     ((float) System.Math.Pow(bx - ax, 2) + (float) System.Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (@by - ay));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        /// <summary>
        ///     Gets the total distance of a list of vectors.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Total distance of the path</returns>
        public static float GetPathLength(this List<Vector2> path)
        {
            var distance = 0f;

            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }

            return distance;
        }

        #region AngleBetween

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector2 vector2, Vector2 toVector2)
        {
            var theta = vector2.Polar() - toVector2.Polar();
            if (theta < 0)
            {
                theta += 360;
            }
            if (theta > 180)
            {
                theta = 360 - theta;
            }
            return theta;
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector2 vector2, Vector3 toVector3)
        {
            var theta = vector2.Polar() - toVector3.Polar();
            if (theta < 0)
            {
                theta += 360;
            }
            if (theta > 180)
            {
                theta = 360 - theta;
            }
            return theta;
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector4">SharpDX Vector4</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector2 vector2, Vector4 toVector4)
        {
            var theta = vector2.Polar() - toVector4.Polar();
            if (theta < 0)
            {
                theta += 360;
            }
            if (theta > 180)
            {
                theta = 360 - theta;
            }
            return theta;
        }

        #endregion

        #region Closest

        /// <summary>
        ///     Seeks for the closest Vector2 to the extended Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector2 Collection</param>
        /// <returns>Closest Vector2</returns>
        public static Vector2 Closest(this Vector2 vector2, IEnumerable<Vector2> array)
        {
            var result = Vector2.Zero;
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
        ///     Seeks for the closest Vector3 to the extended Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector3 Collection</param>
        /// <returns>Closest Vector3</returns>
        public static Vector3 Closest(this Vector2 vector2, IEnumerable<Vector3> array)
        {
            var result = Vector3.Zero;
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
        ///     Seeks for the closest Vector4 to the extended Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="array">Vector4 Collection</param>
        /// <returns>Closest Vector4</returns>
        public static Vector4 Closest(this Vector2 vector2, IEnumerable<Vector4> array)
        {
            var result = Vector4.Zero;
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
        ///     Extends a Vector2 to another Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static Vector2 Extend(this Vector2 vector2, Vector2 toVector2, float distance)
        {
            return vector2 + distance * (toVector2 - vector2).Normalized();
        }

        /// <summary>
        ///     Extends a Vector2 to a Vector3.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static Vector2 Extend(this Vector2 vector2, Vector3 toVector3, float distance)
        {
            return vector2 + distance * (toVector3.ToVector2() - vector2).Normalized();
        }

        /// <summary>
        ///     Extends a Vector2 to a Vector4.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static Vector2 Extend(this Vector2 vector2, Vector4 toVector4, float distance)
        {
            return vector2 + distance * (toVector4.ToVector2() - vector2).Normalized();
        }

        #endregion

        #region ToVector

        /// <summary>
        ///     Transforms an extended Vector2 into a Vector3.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <param name="z">Float Z-axis (default = 0f)</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this Vector2 vector2, float z = 0f)
        {
            return new Vector3(vector2, z);
        }

        /// <summary>
        ///     Transforms an extended Vector2 into a Vector4.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <param name="z">Float Z-axis (default = 0f)</param>
        /// <param name="w">Float W-axis (default = 0f)</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this Vector2 vector2, float z = 0f, float w = 1f)
        {
            return new Vector4(vector2, z, w);
        }

        #endregion

        #region Distance

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector2.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.Distance(vector2, toVector2);
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector3.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector3">SharpDX Vector3 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.Distance(vector2, toVector3.ToVector2());
        }

        /// <summary>
        ///     Calculates the distance between the extended Vector2 and a Vector4.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2 (From)</param>
        /// <param name="toVector4">SharpDX Vector4 (To)</param>
        /// <returns>Float Units</returns>
        public static float Distance(this Vector2 vector2, Vector4 toVector4)
        {
            return Vector2.Distance(vector2, toVector4.ToVector2());
        }

        #endregion

        #region Distance Squared

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector2 vector2, Vector2 toVector2)
        {
            return Vector2.DistanceSquared(vector2, toVector2);
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector2 vector2, Vector3 toVector3)
        {
            return Vector2.DistanceSquared(vector2, toVector3.ToVector2());
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector4">SharpDX Vector4</param>
        /// <returns>The squared distance between the two vectors.</returns>
        public static float DistanceSquared(this Vector2 vector2, Vector4 toVector4)
        {
            return Vector2.DistanceSquared(vector2, toVector4.ToVector2());
        }

        #endregion
    }

    /// <summary>
    ///     Holds info for the <see cref="Vector2Extensions.ProjectOn" /> method.
    /// </summary>
    public struct ProjectionInfo
    {
        /// <summary>
        ///     Returns if the point is on the segment
        /// </summary>
        public bool IsOnSegment;

        /// <summary>
        ///     Line point
        /// </summary>
        public Vector2 LinePoint;

        /// <summary>
        ///     Segment point
        /// </summary>
        public Vector2 SegmentPoint;

        internal ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
        {
            IsOnSegment = isOnSegment;
            SegmentPoint = segmentPoint;
            LinePoint = linePoint;
        }
    }
}