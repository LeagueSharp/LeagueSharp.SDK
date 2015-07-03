using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.IDrawing
{
    using SharpDX;

    /// <summary>
    ///     The DrawLagFreeCircle class; allows for developers to implement circles which are not as hardware intensive, resulting in better FPS for end-users.
    /// </summary>
    class FastDraw
    {
        /// <summary>
        ///     Initializes the <see cref="FastDraw"/> class.
        /// </summary>
        static FastDraw()
        {
            Quality = 1.5f;
        }

        /// <summary>
        ///     Gets or sets the quality.
        /// </summary>
        /// <value>
        ///     Determines how rigid the polygon will be (higher results in more rigidity).
        /// </value>
        public static float Quality { get; set; }
        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / System.Math.PI);
        }

        /// <summary>
        ///     Calculates the 2D vectors to be added to the IEnumerable return value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="chordLength">Length of the chord.</param>
        /// <returns>
        ///     IEnumarable of Vector2s to draw (in World space).
        /// </returns>
        private static IEnumerable<Vector2> DrawCircle2(double x, double y, double radius, int chordLength)
        {
            var quality2 = System.Math.Max(8, System.Math.Floor(180 / RadianToDegree((System.Math.Asin((chordLength / (2 * radius)))))));

            quality2 = Quality * 2 * System.Math.PI / quality2;
            radius = radius * .92;

            for (double theta = 0; theta < 2 * System.Math.PI + quality2; theta += quality2)
            {
                yield return (new Vector2((float)(x + radius * System.Math.Cos(theta)), (float)(y - radius * System.Math.Sin(theta))));
            }
        }

        /// <summary>
        ///     Calls upon DrawCircle2 to get the IEnumerable, then translates them into screen coordinates.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>
        ///     Returns the IEnumerable of Vector2s, represented in screen coordinates.
        /// </returns>
        private static IEnumerable<Vector2> DrawCircleNextLvl(float x, float y, float radius)
        {
            var k = new Vector2(x, y);

            return (from pt in DrawCircle2(k.X, k.Y, radius, 75)
                    select Drawing.WorldToScreen(new Vector3(pt.X, pt.Y, 0)));
        }

        /// <summary>
        ///     Public method to be called by developers in order to actually draw the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="thickness">The thickness of the circle.</param>
        /// <param name="color">The color.</param>
        public static void DrawCircle(float x, float y, float radius, float thickness, System.Drawing.Color color)
        {
            var pts = DrawCircleNextLvl(x, y, radius).GetEnumerator();

            pts.MoveNext();

            Vector2 huehue = pts.Current;

            while (pts.MoveNext())
            {
                Drawing.DrawLine(huehue, pts.Current, thickness, color);
                huehue = pts.Current;
            }
        }
    }
}
