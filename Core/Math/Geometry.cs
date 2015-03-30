using LeagueSharp.CommonEx.Core.Enumerations;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Math
{
    /// <summary>
    ///     Geometry math class, contains geometry calculations.
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        ///     Gets the center of the center of the rectangle where the text should be drawn to be centered by the flags.
        /// </summary>
        /// <param name="rectangle">Rectangle boundaries</param>
        /// <param name="sprite">Sprite which is being drawn on</param>
        /// <param name="dimensions">Object Dimensions</param>
        /// <param name="flags">Centered Flags</param>
        /// <returns>Vector2 with coordinations where to draw the text on the rectangle.</returns>
        public static Vector2 GetCenter(this Rectangle rectangle,
            Sprite sprite,
            Rectangle dimensions,
            CenteredFlags flags)
        {
            var x = 0;
            var y = 0;

            if (flags.HasFlag(CenteredFlags.HorizontalLeft))
            {
                x = rectangle.TopLeft.X;
            }
            else if (flags.HasFlag(CenteredFlags.HorizontalCenter))
            {
                x = rectangle.TopLeft.X + (rectangle.Width - dimensions.Width) / 2;
            }
            else if (flags.HasFlag(CenteredFlags.HorizontalRight))
            {
                x = rectangle.TopRight.X - dimensions.Width;
            }

            if (flags.HasFlag(CenteredFlags.VerticalUp))
            {
                y = rectangle.TopLeft.Y;
            }
            else if (flags.HasFlag(CenteredFlags.VerticalCenter))
            {
                y = rectangle.TopLeft.Y + (rectangle.Height - dimensions.Height) / 2;
            }
            else if (flags.HasFlag(CenteredFlags.VerticalDown))
            {
                y = rectangle.BottomLeft.Y - dimensions.Height;
            }

            return new Vector2(x, y);
        }
    }
}