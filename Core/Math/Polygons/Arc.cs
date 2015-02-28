using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents an Arc <see cref="Polygon" />
    /// </summary>
    public class Arc : Polygon
    {
        private readonly int _quality;

        /// <summary>
        ///     Angle of the Arc.
        /// </summary>
        public float Angle;

        /// <summary>
        ///     End position of the Arc.
        /// </summary>
        public Vector2 EndPos;

        /// <summary>
        ///     Radius of the Arc.
        /// </summary>
        public float Radius;

        /// <summary>
        ///     Starting position of the Arc.
        /// </summary>
        public Vector2 StartPos;

        /// <summary>
        ///     Creates an Arc after converting the points to 2D.
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="direction">Direction</param>
        /// <param name="angle">Angle</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Arc(Vector3 start, Vector3 direction, float angle, float radius, int quality = 20)
            : this(start.ToVector2(), direction.ToVector2(), angle, radius, quality) {}

        /// <summary>
        ///     Creates an Arc.
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="direction">Direction</param>
        /// <param name="angle">Angle</param>
        /// <param name="radius">Radius</param>
        /// <param name="quality">Quality</param>
        public Arc(Vector2 start, Vector2 direction, float angle, float radius, int quality = 20)
        {
            StartPos = start;
            EndPos = (direction - start).Normalized();
            Angle = angle;
            Radius = radius;
            _quality = quality;

            UpdatePolygon();
        }

        /// <summary>
        ///     Updates the Arc. Use this after changing something.
        /// </summary>
        /// <param name="offset"></param>
        public void UpdatePolygon(int offset = 0)
        {
            Points.Clear();

            var outRadius = (Radius + offset) / (float) System.Math.Cos(2 * System.Math.PI / _quality);
            var side1 = EndPos.Rotated(-Angle * 0.5f);

            for (var i = 0; i <= _quality; i++)
            {
                var cDirection = side1.Rotated(i * Angle / _quality).Normalized();
                Points.Add(new Vector2(StartPos.X + outRadius * cDirection.X, StartPos.Y + outRadius * cDirection.Y));
            }
        }
    }
}