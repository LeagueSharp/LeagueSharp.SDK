using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents a Line <see cref="Polygon" />
    /// </summary>
    public class Line : Polygon
    {
        /// <summary>
        ///     End of the Line.
        /// </summary>
        public Vector2 LineEnd;

        /// <summary>
        ///     Start of the Line.
        /// </summary>
        public Vector2 LineStart;

        /// <summary>
        ///     Creates a Line, after converting the points to 2D.
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="length">Length of line(will be automatically set if -1)</param>
        public Line(Vector3 start, Vector3 end, float length = -1) : this(start.ToVector2(), end.ToVector2(), length) {}

        /// <summary>
        ///     Creates a Line.
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="length">Length of line(will be automatically set if -1)</param>
        public Line(Vector2 start, Vector2 end, float length = -1)
        {
            LineStart = start;
            LineEnd = end;

            if (length > 0)
            {
                Length = length;
            }

            UpdatePolygon();
        }

        /// <summary>
        ///     Gets the length of the Line. (Does not have to be updated)
        /// </summary>
        public float Length
        {
            get { return LineStart.Distance(LineEnd); }
            set { LineEnd = (LineEnd - LineStart).Normalized() * value + LineStart; }
        }

        /// <summary>
        ///     Updates the polygon. Use this after changing something.
        /// </summary>
        public void UpdatePolygon()
        {
            Points.Clear();
            Points.Add(LineStart);
            Points.Add(LineEnd);
        }
    }
}