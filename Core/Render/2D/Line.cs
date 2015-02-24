using SharpDX;
using Color = System.Drawing.Color;

namespace LeagueSharp.CommonEx.Core.Render._2D
{
    /// <summary>
    ///     Line class, holds information for drawing a line onto the screen using SharpDX (Direct3D9 cover) and/or
    ///     draws a line.
    /// </summary>
    public class Line
    {
        #region Private Fields

        /// <summary>
        ///     Direct3D9 Line instance.
        /// </summary>
        private SharpDX.Direct3D9.Line _line;

        #endregion

        #region Class Internal Drawing

        /// <summary>
        ///     Applies Vertices into the Graphical Pipeline and draws them onto the screen buffer.
        /// </summary>
        public void Draw()
        {
            var aa = DrawingLine.Antialias;
            var width = DrawingLine.Width;
            var lines = DrawingLine.GLLines;

            DrawingLine.Antialias = Antialias;
            DrawingLine.Width = Width;
            DrawingLine.GLLines = true;

            DrawingLine.Begin();
            DrawingLine.Draw(Vertices, Color);
            DrawingLine.End();

            DrawingLine.Antialias = aa;
            DrawingLine.Width = width;
            DrawingLine.GLLines = lines;
        }

        #endregion

        #region Constructors / Overloads

        #region Line, Vector2, Vector2, Float, Bool, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line,
            Vector2 start,
            Vector2 end,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            Base(line, new[] { start, end }, width, antialias, color);
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line, Vector2 start, Vector2 end, float width, bool antialias, Color color)
        {
            Base(line, new[] { start, end }, width, antialias, new ColorBGRA(color.ToArgb()));
        }

        #endregion

        #region Line, Vector4, Float, Bool, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, Color color)
        {
            Base(
                line, new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, width, antialias,
                new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, ColorBGRA color)
        {
            Base(
                line, new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, width, antialias,
                color);
        }

        #endregion

        #region Line, Float, Float, Float, Float, Float, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line,
            float sX,
            float sY,
            float eX,
            float eY,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            Base(line, new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, width, antialias, color);
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public Line(SharpDX.Direct3D9.Line line,
            float sX,
            float sY,
            float eX,
            float eY,
            float width,
            bool antialias,
            Color color)
        {
            Base(
                line, new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, width, antialias,
                new ColorBGRA(color.ToArgb()));
        }

        #endregion

        #region Base :: Line, Vector2[], Float, Bool, Color

        private void Base(SharpDX.Direct3D9.Line line, Vector2[] vertices, float width, bool antialias, ColorBGRA color)
        {
            DrawingLine = line;
            Vertices = vertices;
            Width = width;
            Antialias = antialias;
            Color = color;
        }

        #endregion

        #endregion

        #region Public Fields

        /// <summary>
        ///     Direct3D9 Line used to draw the line onto the screen.
        /// </summary>
        public SharpDX.Direct3D9.Line DrawingLine
        {
            get { return _line; }
            set { _line = value ?? new SharpDX.Direct3D9.Line(Drawing.Direct3DDevice); }
        }

        /// <summary>
        ///     Width of the Direct3D9 Line.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Returns if the line will be antialiased on draw.
        /// </summary>
        public bool Antialias { get; set; }

        /// <summary>
        ///     Color of the Direct3D9 Line.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Vertices that state the starting and end of the line.
        /// </summary>
        public Vector2[] Vertices { get; set; }

        #endregion

        #region Class External Drawing

        #region Line, Vector2, Vector2, Float, Bool, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line,
            Vector2 start,
            Vector2 end,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            Draw(line, new[] { start, end }, width, antialias, color);
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line,
            Vector2 start,
            Vector2 end,
            float width,
            bool antialias,
            Color color)
        {
            Draw(line, new[] { start, end }, width, antialias, new ColorBGRA(color.ToArgb()));
        }

        #endregion

        #region Line, Vector4, Float, Bool, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, Color color)
        {
            Draw(
                line, new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, width, antialias,
                new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line,
            Vector4 vector4,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            Draw(
                line, new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, width, antialias,
                color);
        }

        #endregion

        #region Line, Float, Float, Float, Float, Float, Color

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line,
            float sX,
            float sY,
            float eX,
            float eY,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            Draw(line, new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, width, antialias, color);
        }

        /// <summary>
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Antialias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line,
            float sX,
            float sY,
            float eX,
            float eY,
            float width,
            bool antialias,
            Color color)
        {
            Draw(
                line, new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, width, antialias,
                new ColorBGRA(color.ToArgb()));
        }

        #endregion

        #region Base :: Line, Vector2[], Float, Bool, Color

        private static void Draw(SharpDX.Direct3D9.Line line,
            Vector2[] vertices,
            float width,
            bool antialias,
            ColorBGRA color)
        {
            line = line ?? new SharpDX.Direct3D9.Line(Drawing.Direct3DDevice);

            line.Antialias = antialias;
            line.Width = width;
            line.GLLines = true;

            line.Begin();
            line.Draw(vertices, color);
            line.End();
        }

        #endregion

        #endregion
    }
}