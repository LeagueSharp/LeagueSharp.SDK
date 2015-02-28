using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents a Sector <see cref="Polygon" />
    /// </summary>
    public class Sector : Polygon
    {
        private readonly int _quality;

        /// <summary>
        ///     Angle of the sector
        /// </summary>
        public float Angle;

        /// <summary>
        ///     Center of the Sector
        /// </summary>
        public Vector2 Center;

        /// <summary>
        ///     Direction of the Sector
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        ///     Radius of the Sector
        /// </summary>
        public float Radius;

        /// <summary>
        ///     Creates a Sector, after converting the Vector3's to 2D.
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="direction">Direction</param>
        /// <param name="angle">Angle</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Sector(Vector3 center, Vector3 direction, float angle, float radius, int quality = 20)
            : this(center.ToVector2(), direction.ToVector2(), angle, radius, quality) {}

        /// <summary>
        ///     Creates a Sector
        /// </summary>
        /// <param name="center">Center</param>
        /// <param name="direction">Direction</param>
        /// <param name="angle">Angle</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Sector(Vector2 center, Vector2 direction, float angle, float radius, int quality = 20)
        {
            Center = center;
            Direction = (direction - center).Normalized();
            Angle = angle;
            Radius = radius;
            _quality = quality;

            UpdatePolygon();
        }

        /// <summary>
        ///     Updates the polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Added radius</param>
        public void UpdatePolygon(int offset = 0)
        {
            Points.Clear();

            var outRadius = (Radius + offset) / (float) System.Math.Cos(2 * System.Math.PI / _quality);
            Points.Add(Center);
            var side1 = Direction.Rotated(-Angle * 0.5f);

            for (var i = 0; i <= _quality; i++)
            {
                var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                Points.Add(new Vector2(Center.X + outRadius * cDirection.X, Center.Y + outRadius * cDirection.Y));
            }
        }
    }
}