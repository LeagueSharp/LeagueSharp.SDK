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
        ///     Returns the center position of the rendering object on the rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle boundaries</param>
        /// <param name="sprite">Sprite which is being drawn on</param>
        /// <param name="dimensions">Object Dimensions</param>
        /// <param name="flags">Centered Flags</param>
        /// <returns>Vector2 center position of the rendering object on the rectangle.</returns>
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

        /// <summary>
        ///     Returns the center position of the text on the rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle boundaries</param>
        /// <param name="sprite">Sprite which is being drawn on</param>
        /// <param name="text">Text</param>
        /// <param name="flags">Centered Flags</param>
        /// <returns>Returns the center position of the text on the rectangle.</returns>
        public static Vector2 GetCenteredText(this Rectangle rectangle, Sprite sprite, string text, CenteredFlags flags)
        {
            return rectangle.GetCenter(sprite, Constants.LeagueSharpFont.MeasureText(sprite, text, 0), flags);
        }

        /// <summary>
        ///     Returns the center position of the text on the rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle boundaries</param>
        /// <param name="sprite">Sprite which is being drawn on</param>
        /// <param name="font">Text Font</param>
        /// <param name="text">Text</param>
        /// <param name="flags">Centered Flags</param>
        /// <returns>Returns the center position of the text on the rectangle.</returns>
        public static Vector2 GetCenteredText(this Rectangle rectangle, Sprite sprite, Font font, string text, CenteredFlags flags)
        {
            return font == null
                ? rectangle.GetCenteredText(sprite, text, flags)
                : rectangle.GetCenter(sprite, font.MeasureText(sprite, text, 0), flags);
        }
    }
}