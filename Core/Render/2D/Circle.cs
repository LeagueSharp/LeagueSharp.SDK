#region

using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

#endregion

namespace LeagueSharp.CommonEx.Core.Render._2D
{
    /// <summary>
    ///     Circle class, holds information for drawing a line onto the screen using SharpDX (Direct3D9 cover) and/or
    ///     draws a circle.
    /// </summary>
    public class Circle
    {
        #region Private Fields

        /// <summary>
        ///     Ready VertexBuffer to draw.
        /// </summary>
        private VertexBuffer buffer;

        #endregion

        #region Class Internal Drawing

        /// <summary>
        ///     Applies VertexBuffer over pipeline and into device and outputs the buffer onto the screen.
        /// </summary>
        public void Draw()
        {
            var device = Drawing.Direct3DDevice;
            var deviceOptions =
                new DeviceOption(device).AddRenderState(RenderState.AlphaBlendEnable, true)
                    .AddTexture(0, null)
                    .AddPixelShader(null)
                    .AddRenderState(RenderState.SourceBlend, RenderState.SourceBlendAlpha)
                    .AddRenderState(RenderState.DestinationBlend, RenderState.DestinationBlendAlpha)
                    .AddStreamSource(0, buffer, 0, Utilities.SizeOf<Vertex>())
                    .AddVertexFormat(VertexFormat.PositionRhw | VertexFormat.Diffuse);

            if (Smooth)
            {
                deviceOptions.AddRenderState(RenderState.MultisampleAntialias, true);
                deviceOptions.AddRenderState(RenderState.AntialiasedLineEnable, true);
            }

            using (new DeviceOptions(deviceOptions))
            {
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, Resolution);
            }
        }

        #endregion

        #region Public Fields

        /// <summary>
        ///     Returns if the Circle would be smooth.
        /// </summary>
        public bool Smooth { get; set; }

        /// <summary>
        ///     Circle Resolution
        /// </summary>
        public int Resolution { get; set; }

        #endregion

        #region Constructors / Overloads

        #region Vector2, Float, Int16, CircleType, Bool, Int16, Color

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public Circle(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            ColorBGRA color)
        {
            Base(position, radius, rotate, type, smooth, resolution, color);
        }

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public Circle(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            Color color)
        {
            Base(position, radius, rotate, type, smooth, resolution, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region Vector3, Int16, CircleType, Bool, Int16, Color

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public Circle(Vector3 position, int rotate, CircleType type, bool smooth, int resolution, ColorBGRA color)
        {
            Base(Extensions.SharpDX.Vector3.ToVector2(position), position.Z, rotate, type, smooth, resolution, color);
        }

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public Circle(Vector3 position, int rotate, CircleType type, bool smooth, int resolution, Color color)
        {
            Base(
                Extensions.SharpDX.Vector3.ToVector2(position), position.Z, rotate, type, smooth, resolution,
                new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region Base :: Vector2, Float, Int16, CircleType, Bool, Int16, ColorBGRA

        /// <summary>
        ///     Base for all Constructors
        /// </summary>
        /// <param name="position">Circle Position</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        private void Base(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            ColorBGRA color)
        {
            Smooth = smooth;
            Resolution = resolution;

            var vertexVertices = new Vertex[resolution + 2];
            var angle = rotate * Math.PI / 180;
            var x = position.X;
            var y = position.Y;
            var pi = (type == CircleType.Full) ? Math.PI : (type == CircleType.Half) ? Math.PI / 2 : Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X = (float) (x - radius * Math.Cos(i * (2 * pi / resolution)));
                vertexVertices[i].Y = (float) (y - radius * Math.Sin(i * (2 * pi / resolution)));
                vertexVertices[i].Z = 0;
                vertexVertices[i].Rhw = 1;
                vertexVertices[i].Color = color.ToRgba();
            }

            #endregion

            #region Set Angle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X =
                    (float)
                        (x + Math.Cos(angle) * (vertexVertices[i].X - x) - Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + Math.Sin(angle) * (vertexVertices[i].X - x) - Math.Cos(angle) * (vertexVertices[i].Y - y));
            }

            #endregion

            #region Buffer

            var newBuffer = new VertexBuffer(
                device, vertexVertices.Length * Utilities.SizeOf<Vertex>(), Usage.WriteOnly,
                VertexFormat.PositionRhw | VertexFormat.Diffuse, Pool.Default);

            var vertices = newBuffer.Lock(0, vertexVertices.Length * Utilities.SizeOf<Vertex>(), LockFlags.None);
            foreach (var v in vertexVertices)
            {
                vertices.Write(v);
            }
            newBuffer.Unlock();

            #endregion

            buffer = newBuffer;
        }

        #endregion

        #endregion

        #region Class External Drawing

        #region Vector3, Int16, CircleType, Bool, Int16, Color

        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Ricle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector3 position,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            ColorBGRA color)
        {
            Draw(Extensions.SharpDX.Vector3.ToVector2(position), position.Z, rotate, type, smooth, resolution, color);
        }

        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Ricle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector3 position, int rotate, CircleType type, bool smooth, int resolution, Color color)
        {
            Draw(
                Extensions.SharpDX.Vector3.ToVector2(position), position.Z, rotate, type, smooth, resolution,
                new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region Vector2, Float, Int16, CircleType, Bool, Int16, Color

        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Position to draw the Circle</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            Color color)
        {
            Draw(position, radius, rotate, type, smooth, resolution, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region Base :: Vector2, Float, Int16, CircleType, Bool, Int16, ColorBGRA

        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Position to draw the Circle</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smooth">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smooth,
            int resolution,
            ColorBGRA color)
        {
            var vertexVertices = new Vertex[resolution + 2];
            var angle = rotate * Math.PI / 180;
            var x = position.X;
            var y = position.Y;
            var pi = (type == CircleType.Full) ? Math.PI : (type == CircleType.Half) ? Math.PI / 2 : Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X = (float) (x - radius * Math.Cos(i * (2 * pi / resolution)));
                vertexVertices[i].Y = (float) (y - radius * Math.Sin(i * (2 * pi / resolution)));
                vertexVertices[i].Z = 0;
                vertexVertices[i].Rhw = 1;
                vertexVertices[i].Color = color.ToRgba();
            }

            #endregion

            #region Set Angle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X =
                    (float)
                        (x + Math.Cos(angle) * (vertexVertices[i].X - x) - Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + Math.Sin(angle) * (vertexVertices[i].X - x) - Math.Cos(angle) * (vertexVertices[i].Y - y));
            }

            #endregion

            #region Buffer

            var buffer = new VertexBuffer(
                device, vertexVertices.Length * Utilities.SizeOf<Vertex>(), Usage.WriteOnly,
                VertexFormat.PositionRhw | VertexFormat.Diffuse, Pool.Default);

            var vertices = buffer.LockToPointer(0, vertexVertices.Length * Utilities.SizeOf<Vertex>(), 0);
            var pointers = new IntPtr[vertexVertices.Length];
            var result = Marshal.AllocHGlobal(IntPtr.Size * vertexVertices.Length * Utilities.SizeOf<Vertex>());
            for (var i = 0; i < vertexVertices.Length; i++)
            {
                pointers[i] = Marshal.AllocHGlobal(IntPtr.Size);
                Marshal.StructureToPtr(vertexVertices[i], pointers[i], true);
                Marshal.WriteIntPtr(result, i * IntPtr.Size, pointers[i]);
            }
            Marshal.Copy(vertices, pointers, 0, (vertexVertices.Length * Utilities.SizeOf<Vertex>()));
            buffer.Unlock();

            #endregion

            #region Draw

            var deviceOptions =
                new DeviceOption(device).AddRenderState(RenderState.AlphaBlendEnable, true)
                    .AddTexture(0, null)
                    .AddPixelShader(null)
                    .AddRenderState(RenderState.SourceBlend, RenderState.SourceBlendAlpha)
                    .AddRenderState(RenderState.DestinationBlend, RenderState.DestinationBlendAlpha)
                    .AddStreamSource(0, buffer, 0, Utilities.SizeOf<Vertex>())
                    .AddVertexFormat(VertexFormat.PositionRhw | VertexFormat.Diffuse);

            if (smooth)
            {
                deviceOptions.AddRenderState(RenderState.MultisampleAntialias, true);
                deviceOptions.AddRenderState(RenderState.AntialiasedLineEnable, true);
            }

            using (new DeviceOptions(deviceOptions))
            {
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, resolution);
            }

            #endregion

            buffer.Dispose();
        }

        #endregion

        #endregion
    }
}