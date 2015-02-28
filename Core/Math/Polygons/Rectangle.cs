using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Math.Polygons
{
    /// <summary>
    ///     Represents a Rectangle <see cref="Polygon" />
    /// </summary>
    public class Rectangle : Polygon
    {
        /// <summary>
        ///     End of the rectangle
        /// </summary>
        public Vector2 End;

        /// <summary>
        ///     Start of the rectangle
        /// </summary>
        public Vector2 Start;

        /// <summary>
        ///     Width of the rectangle
        /// </summary>
        public float Width;

        /// <summary>
        ///     Creates a Rectangle after converting the points to 2D.
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="width">Width</param>
        public Rectangle(Vector3 start, Vector3 end, float width) : this(start.ToVector2(), end.ToVector2(), width) {}

        /// <summary>
        ///     Creates a Rectangle
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="width">Width</param>
        public Rectangle(Vector2 start, Vector2 end, float width)
        {
            Start = start;
            End = end;
            Width = width;

            UpdatePolygon();
        }

        /// <summary>
        ///     Gets the direction of the Rectangle(Does not need update)
        /// </summary>
        public Vector2 Direction
        {
            get { return (End - Start).Normalized(); }
        }

        /// <summary>
        ///     Gets a perpendicular direction of the Rectangle(Does not need an update)
        /// </summary>
        public Vector2 Perpendicular
        {
            get { return Direction.Perpendicular(); }
        }

        /// <summary>
        ///     Updates the Polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Extra width</param>
        /// <param name="overrideWidth">New width to use, overriding the set one.</param>
        public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
        {
            Points.Clear();
            Points.Add(
                Start + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
            Points.Add(
                Start - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
            Points.Add(End - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
            Points.Add(End + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
        }
    }
}