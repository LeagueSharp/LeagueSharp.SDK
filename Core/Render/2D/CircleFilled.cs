#region

using System;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
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
            Base(position.ToVector2(), position.Z, rotate, type, resolution, color);
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
                position.ToVector2(), position.Z, rotate, type, resolution,
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
            var pi = (type == CircleType.Full)
                ? System.Math.PI
                : (type == CircleType.Half) ? System.Math.PI / 2 : System.Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            byte[] b = { color.R, color.G, color.B, color.A };
            var bgr = BitConverter.ToInt32(b, 0);

            vertexVertices[0] = new Vertex { X = x, Y = y, Z = 0f, Rhw = 1f, Color = bgr };

            for (var i = 1; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i] = new Vertex
                {
                    X = (float) (x - radius * System.Math.Cos(pi * ((i - 1) / (resolution / 2.0f)))),
                    Y = (float) (y - radius * System.Math.Sin(pi * ((i - 1) / (resolution / 2.0f)))),
                    Z = 0f,
                    Rhw = 1f,
                    Color = bgr
                };
            }

            #endregion

            #region Set Angle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X =
                    (float)
                        (x + System.Math.Cos(angle) * (vertexVertices[i].X - x) -
                         System.Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + System.Math.Sin(angle) * (vertexVertices[i].X - x) -
                         System.Math.Cos(angle) * (vertexVertices[i].Y - y));
            }

            #endregion

            #region Buffer

            buffer = new VertexBuffer(
                device, vertexVertices.Length * Utilities.SizeOf<Vertex>(), Usage.WriteOnly, VertexFormat.Diffuse,
                Pool.Default);

            var vertices = buffer.Lock(0, vertexVertices.Length * Utilities.SizeOf<Vertex>(), LockFlags.None);
            foreach (var v in vertexVertices)
            {
                vertices.Write(v);
            }
            buffer.Unlock();

            #endregion
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
            Draw(position.ToVector2(), position.Z, rotate, type, resolution, color);
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
                position.ToVector2(), position.Z, rotate, type, resolution,
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
            var pi = (type == CircleType.Full)
                ? System.Math.PI
                : (type == CircleType.Half) ? System.Math.PI / 2 : System.Math.PI / 4;
            var device = Drawing.Direct3DDevice;

            #region Circle

            byte[] b = { color.R, color.G, color.B, color.A };
            var bgr = BitConverter.ToInt32(b, 0);

            vertexVertices[0] = new Vertex { X = x, Y = y, Z = 0f, Rhw = 1f, Color = bgr };

            for (var i = 1; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i] = new Vertex
                {
                    X = (float) (x - radius * System.Math.Cos(pi * ((i - 1) / (resolution / 2.0f)))),
                    Y = (float) (y - radius * System.Math.Sin(pi * ((i - 1) / (resolution / 2.0f)))),
                    Z = 0f,
                    Rhw = 1f,
                    Color = bgr
                };
            }

            #endregion

            #region Set Angle

            for (var i = 0; i < vertexVertices.Length; ++i)
            {
                vertexVertices[i].X =
                    (float)
                        (x + System.Math.Cos(angle) * (vertexVertices[i].X - x) -
                         System.Math.Sin(angle) * (vertexVertices[i].Y - y));
                vertexVertices[i].Y =
                    (float)
                        (y + System.Math.Sin(angle) * (vertexVertices[i].X - x) -
                         System.Math.Cos(angle) * (vertexVertices[i].Y - y));
            }

            #endregion

            #region Buffer

            var buffer = new VertexBuffer(
                device, vertexVertices.Length * Utilities.SizeOf<Vertex>(), Usage.WriteOnly, VertexFormat.Diffuse,
                Pool.Default);

            var vertices = buffer.Lock(0, vertexVertices.Length * Utilities.SizeOf<Vertex>(), LockFlags.None);
            foreach (var v in vertexVertices)
            {
                vertices.Write(v);
            }
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