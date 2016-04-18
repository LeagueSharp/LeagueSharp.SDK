namespace LeagueSharp.SDK.Core.UI
{
    using System;
    using System.Collections.Generic;

    using SharpDX;
    using SharpDX.Direct3D9;

    class Utils
    {
        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        /// <summary>
        /// Draws a line from X to Y with a width and a color
        /// </summary>
        /// <param name="xa">Position X1</param>
        /// <param name="ya">Position Y1</param>
        /// <param name="xb">Position X2</param>
        /// <param name="yb">Position Y2</param>
        /// <param name="dwWidth">Width</param>
        /// <param name="color">Color</param>
        public static void DrawLine(float xa, float ya, float xb, float yb, float dwWidth, Color color)
        {
            Vector2[] vLine = new Vector2[2];
            Line.Width = dwWidth;
            Line.Begin();

            vLine[0][0] = xa; // Set points into array
            vLine[0][1] = ya;
            vLine[1][0] = xb;
            vLine[1][1] = yb;

            Line.Draw(new[]
                    {
                        vLine[0],
                        vLine[1]
                    }, color); // Draw with Line, number of lines, and color
            Line.End(); // finish
        }

        /// <summary>
        /// Draws a filled Box
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="color">Color</param>
        public static void DrawBoxFilled(float x, float y, float w, float h, Color color)
        {
            Vector2[] vLine = new Vector2[2];
            Line.Width = w;
            Line.Begin();

            vLine[0][0] = x + w / 2;
            vLine[0][1] = y;
            vLine[1][0] = x + w / 2;
            vLine[1][1] = y + h;

            Line.Draw(new[]
                    {
                        vLine[0],
                        vLine[1]
                    }, color);
            Line.End();
        }

        /// <summary>
        /// Draws a rounded Rectangle
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="iSmooth">Smooth</param>
        /// <param name="color">Color</param>
        public static void RoundedRectangle(int x, int y, int w, int h, int iSmooth, Color color)
        {
            Vector2[] pt = new Vector2[4];

            // Get all corners 
            pt[0].X = x + (w - iSmooth);
            pt[0].Y = y + (h - iSmooth);

            pt[1].X = x + iSmooth;
            pt[1].Y = y + (h - iSmooth);

            pt[2].X = x + iSmooth;
            pt[2].Y = y + iSmooth;

            pt[3].X = x + w - iSmooth;
            pt[3].Y = y + iSmooth;


            // Draw cross 
            DrawBoxFilled(x, y + iSmooth, w, h - iSmooth * 2, color);

            DrawBoxFilled(x + iSmooth, y, w - iSmooth * 2, h, color);


            float fDegree = 0;

            for (int i = 0; i < 4; i++)
            {
                for (float k = fDegree; k < fDegree + (Math.PI * 2) / 4f; k += (float)((1) * (Math.PI / 180.0f)))
                {
                    // Draw quarter circles on every corner 
                    DrawLine(pt[i].X, pt[i].Y,
                            pt[i].X + (float)(Math.Cos(k) * iSmooth),
                            pt[i].Y + (float)(Math.Sin(k) * iSmooth),
                            1, color); // 3 is with line width 
                }

                fDegree += (float)(Math.PI * 2) / 4; // quarter circle offset 
            }

        }

        /// <summary>
        /// Circle Type Enum
        /// </summary>
        public enum CircleType
        {
            Full,
            Half,
            Quarter
        }

        /// <summary>
        /// Draws a Circle (not filled)
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="radius">Radius</param>
        /// <param name="rotate">Rotation 0 - 360</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smoothing">Smooth Antialiasing</param>
        /// <param name="resolution">Real smooth value</param>
        /// <param name="color">Color</param>
        public static void DrawCircle(float x, float y, float radius, int rotate, CircleType type, bool smoothing, int resolution, Color color)
        {
            VertexBuffer vertices = new VertexBuffer(
             Drawing.Direct3DDevice, Utilities.SizeOf<Vector4>() * 2 * (resolution + 4), Usage.WriteOnly, VertexFormat.Diffuse | VertexFormat.PositionRhw, Pool.Default);

            float angle = rotate * (float)Math.PI / 180f;
            float pi = 0.0f;

            if (type == CircleType.Full) pi = (float)Math.PI;        // Full circle
            if (type == CircleType.Half) pi = (float)Math.PI / 2f;      // 1/2 circle
            if (type == CircleType.Quarter) pi = (float)Math.PI / 4f;   // 1/4 circle

            List<Vector4> data = new List<Vector4>();

            for (int i = 0; i < resolution + 4; i++)
            {
                float x1 = x - radius * (float)Math.Cos(i * (2f * pi / resolution));
                float y1 = y - radius * (float)Math.Sin(i * (2f * pi / resolution));
                data.AddRange(new[]
                    {
                        new Vector4(x1, y1, 0f, 1.0f), color.ToVector4()
                    });
            }

            // Rotate matrix
            int res = 2 * resolution + 4;
            for (int i = 0; i < res; i = i + 2)
            {
                data[i] = new Vector4((float)(x + Math.Cos(angle) * (data[i].X - x) - Math.Sin(angle) * (data[i].Y - y)),
                    (float)(y + Math.Sin(angle) * (data[i].X - x) + Math.Cos(angle) * (data[i].Y - y)),
                    data[i].Z, data[i].W);
            }

            vertices.Lock(0, 0, LockFlags.None).WriteRange(data.ToArray());
            vertices.Unlock();

            VertexElement[] vertexElements = {
                    new VertexElement(
                        0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(
                        0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
            };

            VertexDeclaration vertexDeclaration = new VertexDeclaration(Drawing.Direct3DDevice, vertexElements);

            if (smoothing)
            {
                Drawing.Direct3DDevice.SetRenderState(RenderState.MultisampleAntialias, true);
                Drawing.Direct3DDevice.SetRenderState(RenderState.AntialiasedLineEnable, true);
            }
            else
            {
                Drawing.Direct3DDevice.SetRenderState(RenderState.MultisampleAntialias, false);
                Drawing.Direct3DDevice.SetRenderState(RenderState.AntialiasedLineEnable, false);
            }

            var olddec = Drawing.Direct3DDevice.VertexDeclaration;
            Drawing.Direct3DDevice.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() * 2);
            Drawing.Direct3DDevice.VertexDeclaration = vertexDeclaration;
            Drawing.Direct3DDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, resolution);
            Drawing.Direct3DDevice.VertexDeclaration = olddec;

            vertexDeclaration.Dispose();
            vertices.Dispose();
        }

        /// <summary>
        /// Draws a filled Circle
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="rad">Radius</param>
        /// <param name="rotate">Rotation 0 - 360</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smoothing">Smooth Antialiasing</param>
        /// <param name="resolution">Real smooth value</param>
        /// <param name="color">Color</param>
        public static void DrawCircleFilled(float x, float y, float rad, float rotate, CircleType type, bool smoothing, int resolution, Color color)
        {
            VertexBuffer vertices = new VertexBuffer(
             Drawing.Direct3DDevice, Utilities.SizeOf<Vector4>() * 2 * (resolution + 4), Usage.WriteOnly, VertexFormat.Diffuse | VertexFormat.PositionRhw, Pool.Default);

            double angle = rotate * Math.PI / 180d;
            double pi = 0.0d;

            if (type == CircleType.Full) pi = Math.PI;        // Full circle
            if (type == CircleType.Half) pi = Math.PI / 2d;      // 1/2 circle
            if (type == CircleType.Quarter) pi = Math.PI / 4d;   // 1/4 circle

            List<Vector4> data = new List<Vector4>(new []
                                                        {
                                                            new Vector4(x, y, 0f, 1f), color.ToVector4() 
                                                        });

            for (int i = 1; i < resolution + 4; i++)
            {
                float x1 = (float)(x - rad * Math.Cos(pi * ((i - 1) / (resolution / 2.0f))));
                float y1 = (float)(y - rad * Math.Sin(pi * ((i - 1) / (resolution / 2.0f))));
                data.AddRange(new[]
                    {
                        new Vector4(x1, y1, 0f, 1.0f), color.ToVector4()
                    });
            }

            // Rotate matrix
            int res = 2 * resolution + 4;
            for (int i = 0; i < res; i = i + 2)
            {
                data[i] = new Vector4((float)(x + Math.Cos(angle) * (data[i].X - x) - Math.Sin(angle) * (data[i].Y - y)),
                    (float)(y + Math.Sin(angle) * (data[i].X - x) + Math.Cos(angle) * (data[i].Y - y)),
                    data[i].Z, data[i].W);
            }

            vertices.Lock(0, Utilities.SizeOf<Vector4>() * 2 * (resolution + 4), LockFlags.None).WriteRange(data.ToArray());
            vertices.Unlock();

            VertexElement[] vertexElements = {
                    new VertexElement(
                        0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(
                        0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                    VertexElement.VertexDeclarationEnd
            };

            VertexDeclaration vertexDeclaration = new VertexDeclaration(Drawing.Direct3DDevice, vertexElements);

            if (smoothing)
            {
                Drawing.Direct3DDevice.SetRenderState(RenderState.MultisampleAntialias, true);
                Drawing.Direct3DDevice.SetRenderState(RenderState.AntialiasedLineEnable, true);
            }
            else
            {
                Drawing.Direct3DDevice.SetRenderState(RenderState.MultisampleAntialias, false);
                Drawing.Direct3DDevice.SetRenderState(RenderState.AntialiasedLineEnable, false);
            }

            var olddec = Drawing.Direct3DDevice.VertexDeclaration;
            Drawing.Direct3DDevice.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() * 2);
            Drawing.Direct3DDevice.VertexDeclaration = vertexDeclaration;
            Drawing.Direct3DDevice.DrawPrimitives(PrimitiveType.TriangleFan, 0, resolution);
            Drawing.Direct3DDevice.VertexDeclaration = olddec;

            vertexDeclaration.Dispose();
            vertices.Dispose();
        }

        /// <summary>
        /// Draws a Box
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="linewidth">Line Width</param>
        /// <param name="color">Color</param>
        public static void DrawBox(float x, float y, float w, float h, float linewidth, Color color)
        {
            if (linewidth.Equals(0) || linewidth.Equals(1))
            {
                DrawBoxFilled(x, y, w, 1, color);             // Top
                DrawBoxFilled(x, y + h - 1, w, 1, color);         // Bottom
                DrawBoxFilled(x, y + 1, 1, h - 2 * 1, color);       // Left
                DrawBoxFilled(x + w - 1, y + 1, 1, h - 2 * 1, color);   // Right
            }
            else
            {
                DrawBoxFilled(x, y, w, linewidth, color);                                     // Top
                DrawBoxFilled(x, y + h - linewidth, w, linewidth, color);                         // Bottom
                DrawBoxFilled(x, y + linewidth, linewidth, h - 2 * linewidth, color);               // Left
                DrawBoxFilled(x + w - linewidth, y + linewidth, linewidth, h - 2 * linewidth, color);   // Right
            }
        }

        /// <summary>
        /// Draws a bordered Box
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="borderWidth">The border width</param>
        /// <param name="color">Color</param>
        /// <param name="colorBorder">Border Color</param>
        public static void DrawBoxBordered(float x, float y, float w, float h, float borderWidth, Color color, Color colorBorder)
        {
            DrawBoxFilled(x, y, w, h, color);
            DrawBox(x - borderWidth, y - borderWidth, w + 2 * borderWidth, h + borderWidth, borderWidth, colorBorder);
        }

        /// <summary>
        /// Draws a rounded Box. If Smoothing is true it will draw a border too.
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="radius">Radius</param>
        /// <param name="smoothing">Smoothing</param>
        /// <param name="color">Color</param>
        /// <param name="bcolor">Border Color</param>
        /// <param name="ccolor">Corner Color</param>
        public static void DrawBoxRounded(float x, float y, float w, float h, float radius, bool smoothing, Color color, Color bcolor, Color? ccolor = null)
        {
            Color cornerColor;
            if (!ccolor.HasValue)
            {
                cornerColor = bcolor;
            }
            else
            {
                cornerColor = ccolor.Value;
            }

            DrawBoxFilled(x + radius, y + radius, w - 2 * radius - 1, h - 2 * radius - 1, color);   // Center rect.
            DrawBoxFilled(x + radius, y, w - 2 * radius - 1, radius, color);            // Top rect.
            DrawBoxFilled(x + radius, y + h - radius - 1, w - 2 * radius - 1, radius, color);     // Bottom rect.
            DrawBoxFilled(x, y + radius, radius, h - 2 * radius - 1, color);            // Left rect.
            DrawBoxFilled(x + w - radius - 1, y + radius, radius, h - 2 * radius - 1, color);     // Right rect.

            // Smoothing method
            if (smoothing)
            {
                DrawCircleFilled(x + radius, y + radius, radius - 1, 0, CircleType.Quarter, true, 16, color);             // Top-left corner
                DrawCircleFilled(x + w - radius - 1, y + radius, radius - 1, 90, CircleType.Quarter, true, 16, color);        // Top-right corner
                DrawCircleFilled(x + w - radius - 1, y + h - radius - 1, radius - 1, 180, CircleType.Quarter, true, 16, color);   // Bottom-right corner
                DrawCircleFilled(x + radius, y + h - radius - 1, radius - 1, 270, CircleType.Quarter, true, 16, color);       // Bottom-left corner

                DrawCircle(x + radius + 1, y + radius + 1, radius, 0, CircleType.Quarter, true, 16, cornerColor);          // Top-left corner
                DrawCircle(x + w - radius - 1, y + radius + 1, radius, 90, CircleType.Quarter, true, 16, cornerColor);       // Top-right corner
                DrawCircle(x + w - radius - 1, y + h - radius - 1, radius, 180, CircleType.Quarter, true, 16, cornerColor);    // Bottom-right corner
                DrawCircle(x + radius + 1, y + h - radius - 1, radius, 270, CircleType.Quarter, true, 16, cornerColor);      // Bottom-left corner

                DrawLine(x + radius, y, x + w - radius - 1, y, 1, bcolor);       // Top line
                DrawLine(x + radius, y + h - 2, x + w - radius - 1, y + h - 2, 1, bcolor);   // Bottom line
                DrawLine(x, y + radius, x, y + h - radius - 1, 1, bcolor);       // Left line
                DrawLine(x + w - 2, y + radius, x + w - 2, y + h - radius - 1, 1, bcolor);   // Right line
            }
            else
            {
                DrawCircleFilled(x + radius, y + radius, radius, 0, CircleType.Quarter, false, 16, color);             // Top-left corner
                DrawCircleFilled(x + w - radius - 1, y + radius, radius, 90, CircleType.Quarter, false, 16, color);        // Top-right corner
                DrawCircleFilled(x + w - radius - 1, y + h - radius - 1, radius, 180, CircleType.Quarter, false, 16, color);   // Bottom-right corner
                DrawCircleFilled(x + radius, y + h - radius - 1, radius, 270, CircleType.Quarter, false, 16, color);       // Bottom-left corner
            }
        }
    }
}
