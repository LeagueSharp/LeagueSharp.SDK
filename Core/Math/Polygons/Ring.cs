using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents a Ring <see cref="Polygon" />
    /// </summary>
    public class Ring : Polygon
    {
        private readonly int _quality;

        /// <summary>
        ///     Center of the ring
        /// </summary>
        public Vector2 Center;

        /// <summary>
        ///     Inner Radius
        /// </summary>
        public float InnerRadius;

        /// <summary>
        ///     Outer Radius
        /// </summary>
        public float OuterRadius;

        /// <summary>
        ///     Creates a ring after converting the Vector3's to 2D.
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="innerRadius">Inner Radius</param>
        /// <param name="outerRadius">Outer Radius</param>
        /// <param name="quality">Quality</param>
        public Ring(Vector3 center, float innerRadius, float outerRadius, int quality = 20)
            : this(center.ToVector2(), innerRadius, outerRadius, quality) {}

        /// <summary>
        ///     Creates a ring.
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="innerRadius">Inner Radius</param>
        /// <param name="outerRadius">Outer Radius</param>
        /// <param name="quality">Quality</param>
        public Ring(Vector2 center, float innerRadius, float outerRadius, int quality = 20)
        {
            Center = center;
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
            _quality = quality;

            UpdatePolygon();
        }

        /// <summary>
        ///     Updates the polygon. Call this after you change something.
        /// </summary>
        /// <param name="offset">Added radius</param>
        public void UpdatePolygon(int offset = 0)
        {
            Points.Clear();

            var outRadius = (offset + InnerRadius + OuterRadius) /
                            (float) System.Math.Cos(2 * System.Math.PI / _quality);
            var innerRadius = InnerRadius - OuterRadius - offset;

            for (var i = 0; i <= _quality; i++)
            {
                var angle = i * 2 * System.Math.PI / _quality;
                var point = new Vector2(
                    Center.X - outRadius * (float) System.Math.Cos(angle),
                    Center.Y - outRadius * (float) System.Math.Sin(angle));

                Points.Add(point);
            }

            for (var i = 0; i <= _quality; i++)
            {
                var angle = i * 2 * System.Math.PI / _quality;
                var point = new Vector2(
                    Center.X + innerRadius * (float) System.Math.Cos(angle),
                    Center.Y - innerRadius * (float) System.Math.Sin(angle));

                Points.Add(point);
            }
        }
    }
}