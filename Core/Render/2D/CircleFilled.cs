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
    public class CircleFilled
    {
        #region Private Fields

        /// <summary>
        ///     Ready VertexBuffer to draw.
        /// </summary>
        private VertexBuffer buffer;

        #endregion

        #region Public Fields

        /// <summary>
        ///     Circle Resolution
        /// </summary>
        public int Resolution { get; set; }

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

            using (new DeviceOptions(deviceOptions))
            {
                device.DrawPrimitives(PrimitiveType.TriangleFan, 0, Resolution);
            }
        }

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
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public CircleFilled(Vector2 position, float radius, int rotate, CircleType type, int resolution, ColorBGRA color)
        {
            Base(position, radius, rotate, type, resolution, color);
        }

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public CircleFilled(Vector2 position, float radius, int rotate, CircleType type, int resolution, Color color)
        {
            Base(position, radius, rotate, type, resolution, new ColorBGRA(color.R, color.G, color.B, color.A));
        }

        #endregion

        #region Vector3, Int16, CircleType, Bool, Int16, Color

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public CircleFilled(Vector3 position, int rotate, CircleType type, int resolution, ColorBGRA color)
        {
            Base(Extensions.SharpDX.Vector3Extensions.ToVector2(position), position.Z, rotate, type, resolution, color);
        }

        /// <summary>
        ///     Circle Constructor
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public CircleFilled(Vector3 position, int rotate, CircleType type, int resolution, Color color)
        {
            Base(
                Extensions.SharpDX.Vector3Extensions.ToVector2(position), position.Z, rotate, type, resolution,
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
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        private void Base(Vector2 position, float radius, int rotate, CircleType type, int resolution, ColorBGRA color)
        {
            Resolution = resolution;

            var vertexVertices = new Vertex[resolution + 2];
            var angle = rotate * System.Math.PI / 180;
            var x = position.X;
            var y = position.Y;
            var pi = (type == CircleType.Full) ? System.Math.PI : (type == CircleType.Half) ? System.Math.PI / 2 : System.Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            vertexVertices[0].X = x;
            vertexVertices[0].Y = y;
            vertexVertices[0].Z = 0;
            vertexVertices[0].Rhw = 1;
            vertexVertices[0].Color = color.ToRgba();

            for (var i = 1; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X = (float) (x - radius * System.Math.Cos(pi * ((i - 1) / (resolution / 2.0f))));
                vertexVertices[i].Y = (float) (y - radius * System.Math.Sin(pi * ((i - 1) / (resolution / 2.0f))));
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
                        (x + System.Math.Cos(angle) * (vertexVertices[i].X - x) - System.Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + System.Math.Sin(angle) * (vertexVertices[i].X - x) - System.Math.Cos(angle) * (vertexVertices[i].Y - y));
            }

            #endregion

            #region Buffer

            var newBuffer = new VertexBuffer(
                device, vertexVertices.Length * Utilities.SizeOf<Vertex>(), Usage.WriteOnly,
                VertexFormat.PositionRhw | VertexFormat.Diffuse, Pool.Default);

            var vertices = newBuffer.LockToPointer(0, vertexVertices.Length * Utilities.SizeOf<Vertex>(), 0);
            var pointers = new IntPtr[vertexVertices.Length];
            var result = Marshal.AllocHGlobal(IntPtr.Size * vertexVertices.Length * Utilities.SizeOf<Vertex>());
            for (var i = 0; i < vertexVertices.Length; i++)
            {
                pointers[i] = Marshal.AllocHGlobal(IntPtr.Size);
                Marshal.StructureToPtr(vertexVertices[i], pointers[i], true);
                Marshal.WriteIntPtr(result, i * IntPtr.Size, pointers[i]);
            }
            Marshal.Copy(vertices, pointers, 0, (vertexVertices.Length * Utilities.SizeOf<Vertex>()));
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
        /// <param name="resolution">Ricle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector3 position, int rotate, CircleType type, int resolution, ColorBGRA color)
        {
            Draw(Extensions.SharpDX.Vector3Extensions.ToVector2(position), position.Z, rotate, type, resolution, color);
        }

        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Circle Position and Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="resolution">Ricle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector3 position, int rotate, CircleType type, int resolution, Color color)
        {
            Draw(
                Extensions.SharpDX.Vector3Extensions.ToVector2(position), position.Z, rotate, type, resolution,
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
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector2 position, float radius, int rotate, CircleType type, int resolution, Color color)
        {
            Draw(position, radius, rotate, type, resolution, new ColorBGRA(color.R, color.G, color.B, color.A));
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
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            int resolution,
            ColorBGRA color)
        {
            var vertexVertices = new Vertex[resolution + 2];
            var angle = rotate * System.Math.PI / 180;
            var x = position.X;
            var y = position.Y;
            var pi = (type == CircleType.Full) ? System.Math.PI : (type == CircleType.Half) ? System.Math.PI / 2 : System.Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            vertexVertices[0].X = x;
            vertexVertices[0].Y = y;
            vertexVertices[0].Z = 0;
            vertexVertices[0].Rhw = 1;
            vertexVertices[0].Color = color.ToRgba();

            for (var i = 1; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X = (float)(x - radius * System.Math.Cos(pi * ((i - 1) / (resolution / 2.0f))));
                vertexVertices[i].Y = (float)(y - radius * System.Math.Sin(pi * ((i - 1) / (resolution / 2.0f))));
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
                        (x + System.Math.Cos(angle) * (vertexVertices[i].X - x) - System.Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + System.Math.Sin(angle) * (vertexVertices[i].X - x) - System.Math.Cos(angle) * (vertexVertices[i].Y - y));
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

            using (new DeviceOptions(deviceOptions))
            {
                device.DrawPrimitives(PrimitiveType.TriangleFan, 0, resolution);
            }

            #endregion

            buffer.Dispose();
        }

        #endregion

        #endregion
    }
}