using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render._2D
{
    /// <summary>
    ///     Circle class, holds information for drawing a line onto the screen using SharpDX (Direct3D9 cover) and/or
    ///     draws a circle.
    /// </summary>
    public class Circle
    {
        /// <summary>
        ///     Draw a circle directly without instancing a new circle, external drawing method.
        /// </summary>
        /// <param name="position">Position to draw the Circle</param>
        /// <param name="radius">Circle Radius</param>
        /// <param name="rotate">Circle Rotation</param>
        /// <param name="type">Circle Type</param>
        /// <param name="smoothing">Smooth Circle</param>
        /// <param name="resolution">Circle Resolution</param>
        /// <param name="color">Circle Color</param>
        public static void Draw(Vector2 position,
            float radius,
            int rotate,
            CircleType type,
            bool smoothing,
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
                vertexVertices[i].Y = (float) (y - radius * Math.Sin(i * (2 * pi / +resolution)));
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

            using (new DeviceOptions(device, DeviceOptionsType._2DCircle, smoothing, buffer))
            {
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, resolution);
            }

            buffer.Dispose();
        }
    }

    /// <summary>
    ///     Circle Type to draw.
    /// </summary>
    public enum CircleType
    {
        /// <summary>
        ///     Full Circle
        /// </summary>
        Full,

        /// <summary>
        ///     Half a Circle
        /// </summary>
        Half,

        /// <summary>
        ///     Quarter a Circle
        /// </summary>
        Quarter
    }
}