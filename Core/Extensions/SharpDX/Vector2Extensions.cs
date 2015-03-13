#region

using System;
using System.Collections.Generic;
using System.Linq;
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
        ///     Returns the calculated mangitude of the given Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Magnitude in float-units</returns>
        public static float Magnitude(this Vector2 vector2)
        {
            return (float) System.Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y));
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

        /// <summary>
        ///     Returns the total distance of a path.
        /// </summary>
        public static float PathLength(this List<Vector2> path)
        {
            var distance = 0f;
            for (var i = 0; i < path.Count - 1; i++)
            {
                distance += path[i].Distance(path[i + 1]);
            }
            return distance;
        }

        /// <summary>
        ///     Returns the cross product Z value.
        /// </summary>
        public static float CrossProduct(this Vector2 self, Vector2 other)
        {
            return other.Y * self.X - other.X * self.Y;
        }

        /// <summary>
        ///     Returns if the Vector2 is on the screen.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Is Vector2 on screen</returns>
        public static bool IsOnScreen(this Vector2 vector2)
        {
            return vector2.ToVector3().IsOnScreen();
        }

        /// <summary>
        ///     Returns if the Vector2 position is a wall.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Is Vector2 position a wall position</returns>
        public static bool IsWall(this Vector2 vector2)
        {
            return vector2.ToVector3().IsWall();
        }

        /// <summary>
        ///     Returns whether the given position is under a turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector2</param>
        /// <param name="enemyTurretsOnly">Include Enemy Turret Only</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderTurret(this Vector2 position, bool enemyTurretsOnly)
        {
            return position.ToVector3().IsUnderTurret(enemyTurretsOnly);
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
            return AngleBetween(vector2, toVector2.ToVector3());
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector2 vector2, Vector3 toVector3)
        {
            var magnitudeA = System.Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y));
            var magnitudeB =
                System.Math.Sqrt(
                    (toVector3.X * toVector3.X) + (toVector3.Y * toVector3.Y) + (toVector3.Z * toVector3.Z));

            var dotProduct = (vector2.X * toVector3.X) + (vector2.Y * toVector3.Y);

            return (float) System.Math.Cos(dotProduct / magnitudeA * magnitudeB);
        }

        /// <summary>
        ///     Returns the angle between two vectors.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector4">SharpDX Vector4</param>
        /// <returns>Angle between two vectors in float-units</returns>
        public static float AngleBetween(this Vector2 vector2, Vector4 toVector4)
        {
            return AngleBetween(vector2, toVector4.ToVector3());
        }

        #endregion

        #region IsOrthogonal

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns></returns>
        public static bool IsOrthogonal(this Vector2 vector2, Vector2 toVector2)
        {
            return IsOrthogonal(vector2, toVector2.ToVector3());
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns></returns>
        public static bool IsOrthogonal(this Vector2 vector2, Vector3 toVector3)
        {
            return System.Math.Abs((vector2.X * toVector3.X) + (vector2.Y * toVector3.Y)) < float.Epsilon;
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector4">SharpDX Vector2</param>
        /// <returns></returns>
        public static bool IsOrthogonal(this Vector2 vector2, Vector4 toVector4)
        {
            return IsOrthogonal(vector2, toVector4.ToVector3());
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

        /// <summary>
        ///     Transforms an extended Vector2 List into a Vector3 List.
        /// </summary>
        /// <returns>Vector3 List</returns>
        public static List<Vector3> ToVector3(this List<Vector2> path)
        {
            return path.Select(point => point.ToVector3()).ToList();
        }

        /// <summary>
        ///     Transforms an extended Vector2 List into a Vector4 List.
        /// </summary>
        /// <returns>Vector4 List</returns>
        public static List<Vector4> ToVector4(this List<Vector2> path)
        {
            return path.Select(point => point.ToVector4()).ToList();
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

        /// <summary>
        ///     Returns the distance to the line segment.
        /// </summary>
        /// <param name="point">Extended SharpDX Vector2</param>
        /// <param name="segmentStart">Vector2 Segment Start</param>
        /// <param name="segmentEnd">Vector2 Segment End</param>
        /// <param name="onlyIfOnSegment">Only if Segment</param>
        /// <returns>The distance between the point and the segment.</returns>
        public static float Distance(this Vector2 point,
            Vector2 segmentStart,
            Vector2 segmentEnd,
            bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false) ? Vector2.Distance(objects.SegmentPoint, point) : float.MaxValue;
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

        /// <summary>
        ///     Returns the squared distance to the line segment.
        /// </summary>
        /// <param name="point">Extended SharpDX Vector2</param>
        /// <param name="segmentStart">Vector2 Segment Start</param>
        /// <param name="segmentEnd">Vector2 Segment End</param>
        /// <param name="onlyIfOnSegment">Only if Segment</param>
        /// <returns>The squared distance between the point and the segment.</returns>
        public static float DistanceSquared(this Vector2 point,
            Vector2 segmentStart,
            Vector2 segmentEnd,
            bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false) ? Vector2.DistanceSquared(objects.SegmentPoint, point) : float.MaxValue;
        }

        #endregion

        #region Intersection

        /// <summary>
        ///     Intersects two line segments.
        /// </summary>
        /// <param name="lineSegment1Start">Line Segment 1 (Start)</param>
        /// <param name="lineSegment1End">Line Segment 1 (End)</param>
        /// <param name="lineSegment2Start">Line Segment 2 (Start)></param>
        /// <param name="lineSegment2End">Line Segment 2 (End)</param>
        /// <returns></returns>
        public static IntersectionResult Intersection(this Vector2 lineSegment1Start,
            Vector2 lineSegment1End,
            Vector2 lineSegment2Start,
            Vector2 lineSegment2End)
        {
            double deltaACy = lineSegment1Start.Y - lineSegment2Start.Y;
            double deltaDCx = lineSegment2End.X - lineSegment2Start.X;
            double deltaACx = lineSegment1Start.X - lineSegment2Start.X;
            double deltaDCy = lineSegment2End.Y - lineSegment2Start.Y;
            double deltaBAx = lineSegment1End.X - lineSegment1Start.X;
            double deltaBAy = lineSegment1End.Y - lineSegment1Start.Y;

            var denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            var numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (System.Math.Abs(denominator) < float.Epsilon)
            {
                if (System.Math.Abs(numerator) < float.Epsilon)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (lineSegment1Start.X >= lineSegment2Start.X && lineSegment1Start.X <= lineSegment2End.X)
                    {
                        return new IntersectionResult(true, lineSegment1Start);
                    }
                    if (lineSegment2Start.X >= lineSegment1Start.X && lineSegment2Start.X <= lineSegment1End.X)
                    {
                        return new IntersectionResult(true, lineSegment2Start);
                    }
                    return new IntersectionResult();
                }
                // parallel
                return new IntersectionResult();
            }

            var r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return new IntersectionResult();
            }

            var s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                return new IntersectionResult();
            }

            return new IntersectionResult(
                true,
                new Vector2((float)(lineSegment1Start.X + r * deltaBAx), (float)(lineSegment1Start.Y + r * deltaBAy)));
        }

        #endregion

        #region Vector Movement Collision

        /// <summary>
        ///     Calculates movement collision between two vectors points.
        /// </summary>
        /// <param name="startPoint1">Starting Point 1</param>
        /// <param name="endPoint1">Ending Point 1</param>
        /// <param name="v1">Velociy 1</param>
        /// <param name="startPoint2">Starting Point 2</param>
        /// <param name="v2">Velociy 2</param>
        /// <param name="delay">Delay</param>
        /// <returns>Object contains a float and a vector2.</returns>
        public static Object[] VectorMovementCollision(this Vector2 startPoint1,
            Vector2 endPoint1,
            float v1,
            Vector2 startPoint2,
            float v2,
            float delay = 0f)
        {
            float sP1X = startPoint1.X,
                sP1Y = startPoint1.Y,
                eP1X = endPoint1.X,
                eP1Y = endPoint1.Y,
                sP2X = startPoint2.X,
                sP2Y = startPoint2.Y;

            float d = eP1X - sP1X, e = eP1Y - sP1Y;
            float dist = (float)System.Math.Sqrt(d * d + e * e), t1 = float.NaN;
            float s = System.Math.Abs(dist) > float.Epsilon ? v1 * d / dist : 0,
                k = (System.Math.Abs(dist) > float.Epsilon) ? v1 * e / dist : 0f;

            float r = sP2X - sP1X, j = sP2Y - sP1Y;
            var c = r * r + j * j;


            if (dist > 0f)
            {
                if (System.Math.Abs(v1 - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / v1;
                    t1 = v2 * t >= 0f ? t : float.NaN;
                }
                else if (System.Math.Abs(v2 - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = s * s + k * k - v2 * v2, b = -r * s - j * k;

                    if (System.Math.Abs(a) < float.Epsilon)
                    {
                        if (System.Math.Abs(b) < float.Epsilon)
                        {
                            t1 = (System.Math.Abs(c) < float.Epsilon) ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = (v2 * t >= 0f) ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = b * b - a * c;
                        if (sqr >= 0)
                        {
                            var nom = (float)System.Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = v2 * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = (v2 * t >= 0f) ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = System.Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (System.Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new Object[] { t1, (!float.IsNaN(t1)) ? new Vector2(sP1X + s * t1, sP1Y + k * t1) : new Vector2() };
        }

        #endregion

        #region Circle to Circle Intersection

        /// <summary>
        ///     Returns the two intersection points between two circles.
        /// </summary>
        /// <param name="center1">Center of Circle 1</param>
        /// <param name="center2">Center of Circle 2</param>
        /// <param name="radius1">Circle 1 Radius</param>
        /// <param name="radius2">Circle 2 Radius</param>
        /// <returns></returns>
        public static Vector2[] CircleCircleIntersection(this Vector2 center1, Vector2 center2, float radius1, float radius2)
        {
            var d = center1.Distance(center2);

            if (d > radius1 + radius2 || (d <= System.Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var a = (radius1 * radius1 - radius2 * radius2 + d * d) / (2 * d);
            var h = (float)System.Math.Sqrt(radius1 * radius1 - a * a);
            var direction = (center2 - center1).Normalized();
            var pa = center1 + a * direction;
            var s1 = pa + h * direction.Perpendicular();
            var s2 = pa - h * direction.Perpendicular();
            return new[] { s1, s2 };
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

    /// <summary>
    ///     Holds info for the <see cref="Vector2Extensions.Intersection"/> method.
    /// </summary>
    public struct IntersectionResult
    {
        /// <summary>
        ///     Returns if both of the points intersect.
        /// </summary>
        public bool Intersects;

        /// <summary>
        ///     Intersection point
        /// </summary>
        public Vector2 Point;

        /// <summary>
        ///     Constructor for Intersection Result
        /// </summary>
        /// <param name="intersects">Intersection of input</param>
        /// <param name="point">Intersection Point</param>
        public IntersectionResult(bool intersects = false, Vector2 point = new Vector2())
        {
            Intersects = intersects;
            Point = point;
        }
    }
}