// <copyright file="Vector2Extensions.cs" company="LeagueSharp">
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
    ///     SharpDX/Vector2 Extensions.
    /// </summary>
    public static partial class Extensions
    {
        #region Public Methods and Operators

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
                theta = theta + 360;
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
            return AngleBetween(vector2, toVector3.ToVector2());
        }

        /// <summary>
        ///     Returns the two intersection points between two circles.
        /// </summary>
        /// <param name="center1">Center of Circle 1</param>
        /// <param name="center2">Center of Circle 2</param>
        /// <param name="radius1">Circle 1 Radius</param>
        /// <param name="radius2">Circle 2 Radius</param>
        /// <returns>Array of <see cref="Vector2" /> that contains the intersection points.</returns>
        public static Vector2[] CircleCircleIntersection(
            this Vector2 center1,
            Vector2 center2,
            float radius1,
            float radius2)
        {
            var d = center1.Distance(center2);

            if (d > radius1 + radius2 || (d <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] { };
            }

            var a = ((radius1 * radius1) - (radius2 * radius2) + (d * d)) / (2 * d);
            var h = (float)Math.Sqrt((radius1 * radius1) - (a * a));
            var direction = (center2 - center1).Normalized();
            var pa = center1 + (a * direction);
            var s1 = pa + (h * direction.Perpendicular());
            var s2 = pa - (h * direction.Perpendicular());
            return new[] { s1, s2 };
        }

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
        ///     Counts the ally heroes in range.
        /// </summary>
        /// <param name="vector2">The vector2.</param>
        /// <param name="range">The range.</param>
        /// <param name="originalUnit">The original unit.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountAllyHeroesInRange(this Vector2 vector2, float range, Obj_AI_Base originalUnit = null)
        {
            return vector2.ToVector3().CountAllyHeroesInRange(range, originalUnit);
        }

        /// <summary>
        ///     Counts the enemy heroes in range.
        /// </summary>
        /// <param name="vector2">The vector2.</param>
        /// <param name="range">The range.</param>
        /// <param name="originalUnit">The original unit.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountEnemyHeroesInRange(this Vector2 vector2, float range, Obj_AI_Base originalUnit = null)
        {
            return vector2.ToVector3().CountEnemyHeroesInRange(range, originalUnit);
        }

        /// <summary>
        ///     Returns the cross product Z value.
        /// </summary>
        /// <param name="self">
        ///     The self Vector2.
        /// </param>
        /// <param name="other">
        ///     The other Vector2.
        /// </param>
        /// <returns>
        ///     The <see cref="float" /> cross product.
        /// </returns>
        public static float CrossProduct(this Vector2 self, Vector2 other)
        {
            return (other.Y * self.X) - (other.X * self.Y);
        }

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
        ///     Returns the distance to the line segment.
        /// </summary>
        /// <param name="point">Extended SharpDX Vector2</param>
        /// <param name="segmentStart">Vector2 Segment Start</param>
        /// <param name="segmentEnd">Vector2 Segment End</param>
        /// <param name="onlyIfOnSegment">Only if Segment</param>
        /// <returns>The distance between the point and the segment.</returns>
        public static float Distance(
            this Vector2 point,
            Vector2 segmentStart,
            Vector2 segmentEnd,
            bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false)
                       ? Vector2.Distance(objects.SegmentPoint, point)
                       : float.MaxValue;
        }

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
        ///     Returns the squared distance to the line segment.
        /// </summary>
        /// <param name="point">Extended SharpDX Vector2</param>
        /// <param name="segmentStart">Vector2 Segment Start</param>
        /// <param name="segmentEnd">Vector2 Segment End</param>
        /// <param name="onlyIfOnSegment">Only if Segment</param>
        /// <returns>The squared distance between the point and the segment.</returns>
        public static float DistanceSquared(
            this Vector2 point,
            Vector2 segmentStart,
            Vector2 segmentEnd,
            bool onlyIfOnSegment = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            return (objects.IsOnSegment || onlyIfOnSegment == false)
                       ? Vector2.DistanceSquared(objects.SegmentPoint, point)
                       : float.MaxValue;
        }

        /// <summary>
        ///     Extends a Vector2 to another Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2 (From)</param>
        /// <param name="toVector2">SharpDX Vector2 (To)</param>
        /// <param name="distance">Distance (float units)</param>
        /// <returns>Extended Vector2</returns>
        public static Vector2 Extend(this Vector2 vector2, Vector2 toVector2, float distance)
        {
            return vector2 + (distance * (toVector2 - vector2).Normalized());
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
            return vector2 + (distance * (toVector3.ToVector2() - vector2).Normalized());
        }

        /// <summary>
        ///     Gets the total distance of a list of vectors.
        /// </summary>
        /// <param name="path">The path</param>
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
        ///     Intersects two line segments.
        /// </summary>
        /// <param name="lineSegment1Start">Line Segment 1 (Start)</param>
        /// <param name="lineSegment1End">Line Segment 1 (End)</param>
        /// <param name="lineSegment2Start">Line Segment 2 (Start)></param>
        /// <param name="lineSegment2End">Line Segment 2 (End)</param>
        /// <returns>The intersection result, <seealso cref="IntersectionResult" /></returns>
        public static IntersectionResult Intersection(
            this Vector2 lineSegment1Start,
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

            var denominator = (deltaBAx * deltaDCy) - (deltaBAy * deltaDCx);
            var numerator = (deltaACy * deltaDCx) - (deltaACx * deltaDCy);

            if (Math.Abs(denominator) < float.Epsilon)
            {
                if (Math.Abs(numerator) < float.Epsilon)
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

                    return default(IntersectionResult);
                }

                // parallel
                return default(IntersectionResult);
            }

            var r = numerator / denominator;

            if (r < 0 || r > 1)
            {
                return default(IntersectionResult);
            }

            var s = ((deltaACy * deltaBAx) - (deltaACx * deltaBAy)) / denominator;

            if (s < 0 || s > 1)
            {
                return default(IntersectionResult);
            }

            return new IntersectionResult(
                true,
                new Vector2(
                    (float)(lineSegment1Start.X + (r * deltaBAx)),
                    (float)(lineSegment1Start.Y + (r * deltaBAy))));
        }

        /// <summary>
        ///     Returns if the Vector2 is on the screen.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// ///
        /// <param name="radius">Radius</param>
        /// <returns>Is Vector2 on screen</returns>
        public static bool IsOnScreen(this Vector2 vector2, float radius = 0f)
        {
            return vector2.ToVector3().IsOnScreen(radius);
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector2">SharpDX Vector2</param>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsOrthogonal(this Vector2 vector2, Vector2 toVector2)
        {
            return Math.Abs((vector2.X * toVector2.X) + (vector2.Y * toVector2.Y)) < float.Epsilon;
        }

        /// <summary>
        ///     Returns if the angle is orthogonal.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="toVector3">SharpDX Vector3</param>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsOrthogonal(this Vector2 vector2, Vector3 toVector3)
        {
            return IsOrthogonal(vector2, toVector3.ToVector2());
        }

        /// <summary>
        ///     Returns whether the given position is under a ally turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector2</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderAllyTurret(this Vector2 position)
        {
            return GameObjects.AllyTurrets.Any(turret => !turret.IsDead && turret.Distance(position) < 950);
        }

        /// <summary>
        ///     Returns whether the given position is under a enemy turret
        /// </summary>
        /// <param name="position">Extended SharpDX Vector2</param>
        /// <returns>Is Position under a turret</returns>
        public static bool IsUnderEnemyTurret(this Vector2 position)
        {
            return GameObjects.EnemyTurrets.Any(turret => !turret.IsDead && turret.Distance(position) < 950);
        }

        /// <summary>
        ///     Returns true if the point is under the rectangle
        /// </summary>
        /// <param name="point">
        ///     Extended SharpDX Vector2
        /// </param>
        /// <param name="x">
        ///     Rectangle X-axis
        /// </param>
        /// <param name="y">
        ///     Rectangle Y-axis
        /// </param>
        /// <param name="width">
        ///     Rectangle width
        /// </param>
        /// <param name="height">
        ///     Rectangle height
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsUnderRectangle(this Vector2 point, float x, float y, float width, float height)
        {
            return point.X > x && point.X < x + width && point.Y > y && point.Y < y + height;
        }

        /// <summary>
        ///     Checks for if the extended Vector2 is valid.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <returns>The <see cref="bool" />.</returns>
        public static bool IsValid(this Vector2 vector2)
        {
            return vector2 != Vector2.Zero;
        }

        /// <summary>
        ///     Returns if the Vector2 position is a wall.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Is Vector2 position a wall position</returns>
        public static bool IsWall(this Vector2 vector2)
        {
            return NavMesh.GetCollisionFlags(vector2.X, vector2.Y).HasFlag(CollisionFlags.Wall);
        }

        /// <summary>
        ///     Returns the calculated magnitude of the given Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Magnitude in float-units</returns>
        public static float Magnitude(this Vector2 vector2)
        {
            return (float)Math.Sqrt((vector2.X * vector2.X) + (vector2.Y * vector2.Y));
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
        ///     Returns the total distance of a path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
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
        ///     Returns the Perpendicular Vector2 to the Extended Vector2.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="offset">Axis Offset (0 = X, 1 = Y)</param>
        /// <returns>Perpendicular Vector2</returns>
        public static Vector2 Perpendicular(this Vector2 vector2, int offset = 0)
        {
            return (offset == 0) ? new Vector2(-vector2.Y, vector2.X) : new Vector2(vector2.Y, -vector2.X);
        }

        /// <summary>
        ///     Returns the polar for vector angle (in Degrees).
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <returns>Polar for Vector Angle (Degrees)</returns>
        public static float Polar(this Vector2 vector2)
        {
            if (Math.Abs(vector2.X - 0) <= (float)1e-9)
            {
                return (vector2.Y > 0) ? 90 : (vector2.Y < 0) ? 270 : 0;
            }

            var theta = (float)(Math.Atan(vector2.Y / vector2.X) * (180 / Math.PI));

            if (vector2.X < 0)
            {
                theta += 180;
            }

            if (theta < 0)
            {
                theta += 360;
            }

            return theta;
        }

        /// <summary>
        ///     Returns the projection of the Vector2 on the segment.
        /// </summary>
        /// <param name="point">The Point</param>
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
            var rL = (((cx - ax) * (bx - ax)) + ((cy - ay) * (by - ay)))
                     / ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + (rL * (bx - ax)), ay + (rL * (by - ay)));
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
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + (rS * (bx - ax)), ay + (rS * (@by - ay)));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        /// <summary>
        ///     Rotates the Vector2 to a set angle.
        /// </summary>
        /// <param name="vector2">Extended SharpDX Vector2</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <returns>Rotated Vector2</returns>
        public static Vector2 Rotated(this Vector2 vector2, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector2(
                (float)((vector2.X * cos) - (vector2.Y * sin)),
                (float)((vector2.Y * cos) + (vector2.X * sin)));
        }

        /// <summary>
        ///     Transforms an extended Vector2 into a Vector3.
        /// </summary>
        /// <param name="vector2">SharpDX Vector2</param>
        /// <param name="z">Float Z-axis (default = 0f)</param>
        /// <returns>The <see cref="Vector3" /></returns>
        public static Vector3 ToVector3(this Vector2 vector2, float z = 0f)
        {
            return new Vector3(vector2, z.Equals(0f) ? GameObjects.Player.ServerPosition.Z : z);
        }

        /// <summary>
        ///     Transforms an extended Vector2 List into a Vector3 List.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     Vector3 List
        /// </returns>
        public static List<Vector3> ToVector3(this List<Vector2> path)
        {
            return path.Select(point => point.ToVector3()).ToList();
        }

        /// <summary>
        ///     Calculates movement collision between two vectors points.
        /// </summary>
        /// <param name="pointStartA">
        ///     Point A Start.
        /// </param>
        /// <param name="pointEndA">
        ///     Point A End.
        /// </param>
        /// <param name="pointVelocityA">
        ///     Point A Velocity.
        /// </param>
        /// <param name="pointB">
        ///     Point B.
        /// </param>
        /// <param name="pointVelocityB">
        ///     Point B Velocity.
        /// </param>
        /// <param name="delay">
        ///     Additional Delay.
        /// </param>
        /// <returns>
        ///     The <see cref="MovementCollisionInfo" />.
        /// </returns>
        public static MovementCollisionInfo VectorMovementCollision(
            this Vector2 pointStartA,
            Vector2 pointEndA,
            float pointVelocityA,
            Vector2 pointB,
            float pointVelocityB,
            float delay = 0f)
        {
            return new[] { pointStartA, pointEndA }.VectorMovementCollision(
                pointVelocityA,
                pointB,
                pointVelocityB,
                delay);
        }

        /// <summary>
        ///     Calculates movement collision between two vectors points.
        /// </summary>
        /// <param name="pointA">
        ///     Point A.
        /// </param>
        /// <param name="pointVelocityA">
        ///     Point A Velocity.
        /// </param>
        /// <param name="pointB">
        ///     Point B.
        /// </param>
        /// <param name="pointVelocityB">
        ///     Point B Velocity.
        /// </param>
        /// <param name="delay">
        ///     Additional Delay.
        /// </param>
        /// <returns>
        ///     The <see cref="MovementCollisionInfo" />.
        /// </returns>
        public static MovementCollisionInfo VectorMovementCollision(
            this Vector2[] pointA,
            float pointVelocityA,
            Vector2 pointB,
            float pointVelocityB,
            float delay = 0f)
        {
            if (pointA.Length < 1)
            {
                return default(MovementCollisionInfo);
            }

            float sP1X = pointA[0].X,
                  sP1Y = pointA[0].Y,
                  eP1X = pointA[1].X,
                  eP1Y = pointA[1].Y,
                  sP2X = pointB.X,
                  sP2Y = pointB.Y;

            float d = eP1X - sP1X, e = eP1Y - sP1Y;
            float dist = (float)Math.Sqrt((d * d) + (e * e)), t1 = float.NaN;
            float s = Math.Abs(dist) > float.Epsilon ? pointVelocityA * d / dist : 0,
                  k = (Math.Abs(dist) > float.Epsilon) ? pointVelocityA * e / dist : 0f;

            float r = sP2X - sP1X, j = sP2Y - sP1Y;
            var c = (r * r) + (j * j);

            if (dist > 0f)
            {
                if (Math.Abs(pointVelocityA - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / pointVelocityA;
                    t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(pointVelocityB - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = (s * s) + (k * k) - (pointVelocityB * pointVelocityB), b = (-r * s) - (j * k);

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = (Math.Abs(c) < float.Epsilon) ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = (pointVelocityB * t >= 0f) ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = (b * b) - (a * c);

                        if (sqr >= 0)
                        {
                            var nom = (float)Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = (pointVelocityB * t >= 0f) ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
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
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new MovementCollisionInfo(
                t1,
                !float.IsNaN(t1) ? new Vector2(sP1X + (s * t1), sP1Y + (k * t1)) : default(Vector2));
        }

        #endregion
    }

    /// <summary>
    ///     Holds info for the ProjectOn method.
    /// </summary>
    public struct ProjectionInfo
    {
        #region Fields

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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectionInfo" /> struct.
        /// </summary>
        /// <param name="isOnSegment">
        ///     Is on Segment
        /// </param>
        /// <param name="segmentPoint">
        ///     Segment point
        /// </param>
        /// <param name="linePoint">
        ///     Line point
        /// </param>
        internal ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
        {
            this.IsOnSegment = isOnSegment;
            this.SegmentPoint = segmentPoint;
            this.LinePoint = linePoint;
        }

        #endregion
    }

    /// <summary>
    ///     Holds info for the VectorMovementCollision method.
    /// </summary>
    public struct MovementCollisionInfo
    {
        #region Fields

        /// <summary>
        ///     Collision position.
        /// </summary>
        public Vector2 CollisionPosition;

        /// <summary>
        ///     Collision Time from calculation.
        /// </summary>
        public float CollisionTime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MovementCollisionInfo" /> struct.
        /// </summary>
        /// <param name="collisionTime">
        ///     Collision time from calculation
        /// </param>
        /// <param name="collisionPosition">
        ///     Collision position
        /// </param>
        internal MovementCollisionInfo(float collisionTime, Vector2 collisionPosition)
        {
            this.CollisionTime = collisionTime;
            this.CollisionPosition = collisionPosition;
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Information accessor.
        /// </summary>
        /// <param name="i">
        ///     The Indexer.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object this[int i] => i == 0 ? this.CollisionTime : (object)this.CollisionPosition;

        #endregion
    }

    /// <summary>
    ///     Holds info for the <see cref="Extensions.Intersection" /> method.
    /// </summary>
    public struct IntersectionResult
    {
        #region Fields

        /// <summary>
        ///     Returns if both of the points intersect.
        /// </summary>
        public bool Intersects;

        /// <summary>
        ///     Intersection point
        /// </summary>
        public Vector2 Point;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntersectionResult" /> struct.
        ///     Constructor for Intersection Result
        /// </summary>
        /// <param name="intersects">
        ///     Intersection of input
        /// </param>
        /// <param name="point">
        ///     Intersection Point
        /// </param>
        public IntersectionResult(bool intersects = false, Vector2 point = default(Vector2))
        {
            this.Intersects = intersects;
            this.Point = point;
        }

        #endregion
    }
}