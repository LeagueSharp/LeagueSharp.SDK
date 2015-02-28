#region

using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Base class representing a polygon.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        ///     List of all points in the polygon.
        /// </summary>
        public List<Vector2> Points = new List<Vector2>();

        /// <summary>
        ///     Adds a Vector2 to the points
        /// </summary>
        /// <param name="point">Point</param>
        public void Add(Vector2 point)
        {
            Points.Add(point);
        }

        /// <summary>
        ///     Converts Vector3 to 2D, then adds it to the points.
        /// </summary>
        /// <param name="point">Point</param>
        public void Add(Vector3 point)
        {
            Points.Add(point.ToVector2());
        }

        /// <summary>
        ///     Adds all of the points in the polygon to this instance.
        /// </summary>
        /// <param name="polygon">Polygon</param>
        public void Add(Polygon polygon)
        {
            foreach (var point in polygon.Points)
            {
                Points.Add(point);
            }
        }

        /// <summary>
        ///     Converts all the points to the Clipper Library format
        /// </summary>
        /// <returns>List of IntPoint's</returns>
        public List<IntPoint> ToClipperPath()
        {
            var result = new List<IntPoint>(Points.Count);
            result.AddRange(Points.Select(point => new IntPoint(point.X, point.Y)));
            return result;
        }

        /// <summary>
        ///     Draws all of the points in the polygon connected.
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="width">Width of lines</param>
        public virtual void Draw(Color color, int width = 1)
        {
            for (var i = 0; i <= Points.Count - 1; i++)
            {
                var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                var from = Drawing.WorldToScreen(Points[i].ToVector3());
                var to = Drawing.WorldToScreen(Points[nextIndex].ToVector3());

                Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            }
        }

        /// <summary>
        ///     Gets if the Vector2 is inside the polygon.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Whether the Vector2 is inside the polygon</returns>
        public bool IsInside(Vector2 point)
        {
            return !IsOutside(point);
        }

        /// <summary>
        ///     Gets if the Vector3 is inside the polygon.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Whether the Vector3 is inside the polygon</returns>
        public bool IsInside(Vector3 point)
        {
            return !IsOutside(point.ToVector2());
        }

        /// <summary>
        ///     Gets if the Objects Position is inside the polygon.
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        /// <returns>Whether the <see cref="GameObject" />'s position is inside the polygon</returns>
        public bool IsInside(GameObject gameObject)
        {
            return !IsOutside(gameObject.Position.ToVector2());
        }

        /// <summary>
        ///     Gets if the position is outside of the polygon.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>Whether the Vector2 is inside the polygon</returns>
        public bool IsOutside(Vector2 point)
        {
            var p = new IntPoint(point.X, point.Y);
            return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
        }
    }
}