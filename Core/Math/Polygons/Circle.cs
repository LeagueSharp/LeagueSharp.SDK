#region

using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents a Circle <see cref="Polygon" />
    /// </summary>
    public class Circle : Polygon
    {
        /// <summary>
        ///     Circle Quality
        /// </summary>
        private readonly int _quality;

        /// <summary>
        ///     Center of the Circle.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        ///     Radius of Circle.
        /// </summary>
        public float Radius;

        /// <summary>
        ///     Creates a Circle after converting the center to 2D.
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Circle(Vector3 center, float radius, int quality = 20) : this(center.ToVector2(), radius, quality) {}

        /// <summary>
        ///     Creates a Circle.
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Circle(Vector2 center, float radius, int quality = 20)
        {
            Center = center;
            Radius = radius;
            _quality = quality;

            UpdatePolygon();
        }

        /// <summary>
        ///     Updates the Polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Extra radius</param>
        /// <param name="overrideWidth">New width to use, overriding the set one.</param>
        public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
        {
            Points.Clear();

            var outRadius = (overrideWidth > 0
                ? overrideWidth
                : (offset + Radius) / (float) System.Math.Cos(2 * System.Math.PI / _quality));

            for (var i = 1; i <= _quality; i++)
            {
                var angle = i * 2 * System.Math.PI / _quality;
                var point = new Vector2(
                    Center.X + outRadius * (float) System.Math.Cos(angle),
                    Center.Y + outRadius * (float) System.Math.Sin(angle));

                Points.Add(point);
            }
        }
    }
}