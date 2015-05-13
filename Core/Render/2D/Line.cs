// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Line.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Line class, holds information for drawing a line onto the screen using SharpDX (Direct3D9 cover) and/or
//   draws a line.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Render._2D
{
    using System;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     Line class, holds information for drawing a line onto the screen using SharpDX (Direct3D9 cover) and/or
    ///     draws a line.
    /// </summary>
    public sealed class Line : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Direct3D9 Line instance.
        /// </summary>
        private SharpDX.Direct3D9.Line line;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="start">
        ///     Starting Vector
        /// </param>
        /// <param name="end">
        ///     Ending Vector
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(
            SharpDX.Direct3D9.Line line, 
            Vector2 start, 
            Vector2 end, 
            float width, 
            bool antialias, 
            ColorBGRA color)
        {
            this.Base(line, new[] { start, end }, width, antialias, color);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="start">
        ///     Starting Vector
        /// </param>
        /// <param name="end">
        ///     Ending Vector
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(SharpDX.Direct3D9.Line line, Vector2 start, Vector2 end, float width, bool antialias, Color color)
        {
            this.Base(line, new[] { start, end }, width, antialias, new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="vector4">
        ///     Starting Vector (X,Y) and Ending Vector (Z,W)
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, Color color)
        {
            this.Base(
                line, 
                new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, 
                width, 
                antialias, 
                new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="vector4">
        ///     Starting Vector (X,Y) and Ending Vector (Z,W)
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, ColorBGRA color)
        {
            this.Base(
                line, 
                new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, 
                width, 
                antialias, 
                color);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="sX">
        ///     Starting X-axis
        /// </param>
        /// <param name="sY">
        ///     Starting Y-axis
        /// </param>
        /// <param name="eX">
        ///     Ending X-axis
        /// </param>
        /// <param name="eY">
        ///     Ending Y-axis
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(
            SharpDX.Direct3D9.Line line, 
            float sX, 
            float sY, 
            float eX, 
            float eY, 
            float width, 
            bool antialias, 
            ColorBGRA color)
        {
            this.Base(line, new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, width, antialias, color);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        ///     Line Constructor.
        /// </summary>
        /// <param name="line">
        ///     Line Instance [Can be NULL]
        /// </param>
        /// <param name="sX">
        ///     Starting X-axis
        /// </param>
        /// <param name="sY">
        ///     Starting Y-axis
        /// </param>
        /// <param name="eX">
        ///     Ending X-axis
        /// </param>
        /// <param name="eY">
        ///     Ending Y-axis
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias
        /// </param>
        /// <param name="color">
        ///     Line Color
        /// </param>
        public Line(
            SharpDX.Direct3D9.Line line, 
            float sX, 
            float sY, 
            float eX, 
            float eY, 
            float width, 
            bool antialias, 
            Color color)
        {
            this.Base(
                line, 
                new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, 
                width, 
                antialias, 
                new ColorBGRA(color.ToArgb()));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the line will be drawn anti-aliased.
        /// </summary>
        public bool Antialias { get; set; }

        /// <summary>
        ///     Gets or sets the color of the Direct3D9 Line.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Gets or sets the Direct3D9 Line used to draw the line onto the screen.
        /// </summary>
        public SharpDX.Direct3D9.Line DrawingLine
        {
            get
            {
                return this.line;
            }

            set
            {
                this.line = value ?? new SharpDX.Direct3D9.Line(Drawing.Direct3DDevice);
            }
        }

        /// <summary>
        ///     Gets or sets the vertices that state the starting and end of the line.
        /// </summary>
        public Vector2[] Vertices { get; set; }

        /// <summary>
        ///     Gets or sets the width of the Direct3D9 Line.
        /// </summary>
        public float Width { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
            Vector2 start, 
            Vector2 end, 
            float width, 
            bool antialias, 
            ColorBGRA color)
        {
            Draw(line, new[] { start, end }, width, antialias, color);
        }

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="start">Starting Vector</param>
        /// <param name="end">Ending Vector</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
            Vector2 start, 
            Vector2 end, 
            float width, 
            bool antialias, 
            Color color)
        {
            Draw(line, new[] { start, end }, width, antialias, new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(SharpDX.Direct3D9.Line line, Vector4 vector4, float width, bool antialias, Color color)
        {
            Draw(
                line, 
                new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, 
                width, 
                antialias, 
                new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vector4">Starting Vector (X,Y) and Ending Vector (Z,W)</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
            Vector4 vector4, 
            float width, 
            bool antialias, 
            ColorBGRA color)
        {
            Draw(
                line, 
                new[] { new Vector2(vector4.X, vector4.Y), new Vector2(vector4.Z, vector4.W) }, 
                width, 
                antialias, 
                color);
        }

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
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
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="sX">Starting X-axis</param>
        /// <param name="sY">Starting Y-axis</param>
        /// <param name="eX">Ending X-axis</param>
        /// <param name="eY">Ending Y-axis</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
            float sX, 
            float sY, 
            float eX, 
            float eY, 
            float width, 
            bool antialias, 
            Color color)
        {
            Draw(
                line, 
                new[] { new Vector2(sX, sY), new Vector2(eX, eY) }, 
                width, 
                antialias, 
                new ColorBGRA(color.ToArgb()));
        }

        /// <summary>
        ///     Draw a line directly without instancing a new line, external drawing method.
        /// </summary>
        /// <param name="line">Line Instance [Can be NULL]</param>
        /// <param name="vertices">Vertices that contain the correct data to draw the line.</param>
        /// <param name="width">Line Width</param>
        /// <param name="antialias">Line Anti-alias</param>
        /// <param name="color">Line Color</param>
        public static void Draw(
            SharpDX.Direct3D9.Line line, 
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

        /// <summary>
        ///     Line Dispose.
        /// </summary>
        public void Dispose()
        {
            this.line.Dispose();
            this.DrawingLine.Dispose();
        }

        /// <summary>
        ///     Applies Vertices into the Graphical Pipeline and draws them onto the screen buffer.
        /// </summary>
        public void Draw()
        {
            var aa = this.DrawingLine.Antialias;
            var width = this.DrawingLine.Width;
            var lines = this.DrawingLine.GLLines;

            this.DrawingLine.Antialias = this.Antialias;
            this.DrawingLine.Width = this.Width;
            this.DrawingLine.GLLines = true;

            this.DrawingLine.Begin();
            this.DrawingLine.Draw(this.Vertices, this.Color);
            this.DrawingLine.End();

            this.DrawingLine.Antialias = aa;
            this.DrawingLine.Width = width;
            this.DrawingLine.GLLines = lines;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The base constructor.
        /// </summary>
        /// <param name="directLine">
        ///     Line instance
        /// </param>
        /// <param name="vertices">
        ///     The Vertices
        /// </param>
        /// <param name="width">
        ///     Line Width
        /// </param>
        /// <param name="antialias">
        ///     Line Anti-alias flag
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        private void Base(
            SharpDX.Direct3D9.Line directLine, 
            Vector2[] vertices, 
            float width, 
            bool antialias, 
            ColorBGRA color)
        {
            this.DrawingLine = directLine;
            this.Vertices = vertices;
            this.Width = width;
            this.Antialias = antialias;
            this.Color = color;
        }

        #endregion
    }
}