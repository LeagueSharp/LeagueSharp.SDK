#region

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;

#endregion

namespace LeagueSharp.CommonEx.Core.Render
{
    /// <summary>
    ///     Device Options to use in a block and then release to original device state afterwards.
    /// </summary>
    public class DeviceOptions : IDisposable
    {
        /// <summary>
        ///     Creates a new Device Options handle, commonly used as a block on draw.
        /// </summary>
        /// <param name="device">Device to modify</param>
        /// <param name="options">Built-in device options</param>
        /// <param name="extraObjects">Extra objects</param>
        public DeviceOptions(Device device, DeviceOptionsType options, params object[] extraObjects)
        {
            if (device == null || device.IsDisposed)
            {
                return;
            }

            this.device = device;
            this.options = options;

            switch (options)
            {
                    #region 2D Circle

                case DeviceOptionsType._2DCircle:
                {
                    texture = device.GetTexture(0);
                    pixelShader = device.PixelShader;

                    device.SetTexture(0, null);
                    device.PixelShader = null;

                    var smooth = extraObjects.Length > 0 && extraObjects[0] is Boolean && (bool) extraObjects[0];

                    if (smooth)
                    {
                        states.Add(
                            RenderState.MultisampleAntialias, device.GetRenderState(RenderState.MultisampleAntialias));
                        states.Add(
                            RenderState.AntialiasedLineEnable, device.GetRenderState(RenderState.AntialiasedLineEnable));

                        device.SetRenderState(RenderState.MultisampleAntialias, true);
                        device.SetRenderState(RenderState.AntialiasedLineEnable, true);
                    }

                    states.Add(RenderState.AlphaBlendEnable, device.GetRenderState(RenderState.AlphaBlendEnable));
                    states.Add(RenderState.SourceBlend, device.GetRenderState(RenderState.SourceBlend));
                    states.Add(RenderState.DestinationBlend, device.GetRenderState(RenderState.DestinationBlend));

                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.SourceBlend, RenderState.SourceBlendAlpha);
                    device.SetRenderState(RenderState.DestinationBlend, RenderState.DestinationBlendAlpha);

                    device.GetStreamSource(0, out vertexBuffer, out vertexBufferoffsetInBytes, out vertexBufferStride);
                    if (extraObjects.Length > 1 && extraObjects[1] is VertexBuffer)
                    {
                        device.SetStreamSource(0, (VertexBuffer) extraObjects[1], 0, Utilities.SizeOf<Vertex>());
                    }

                    format = device.VertexFormat;
                    device.VertexFormat = VertexFormat.PositionRhw | VertexFormat.Diffuse;

                    break;
                }

                    #endregion
            }
        }

        /// <summary>
        ///     Disposal of the DeviceOptions block, returning all original values to the device.
        /// </summary>
        public void Dispose()
        {
            if (options == DeviceOptionsType._2DCircle)
            {
                device.SetTexture(0, texture);
                device.PixelShader = pixelShader;
                device.SetStreamSource(0, vertexBuffer, vertexBufferoffsetInBytes, vertexBufferStride);
                device.VertexFormat = format;
            }
            foreach (var state in states)
            {
                device.SetRenderState(state.Key, state.Value);
            }
        }

        #region Fields

        /// <summary>
        ///     Device Handle.
        /// </summary>
        private readonly Device device;

        /// <summary>
        ///     Old VertexFormat.
        /// </summary>
        private readonly VertexFormat format;

        /// <summary>
        ///     Device Options Type.
        /// </summary>
        private readonly DeviceOptionsType options;

        /// <summary>
        ///     Old PixelShader.
        /// </summary>
        private readonly PixelShader pixelShader;

        /// <summary>
        ///     Saved RenderStates.
        /// </summary>
        private readonly Dictionary<RenderState, int> states = new Dictionary<RenderState, int>();

        /// <summary>
        ///     Old Texture
        /// </summary>
        private readonly BaseTexture texture;

        /// <summary>
        ///     Old VertexBuffer
        /// </summary>
        private readonly VertexBuffer vertexBuffer;

        /// <summary>
        ///     Old VertexBuffer offsetInBytes.
        /// </summary>
        private readonly int vertexBufferoffsetInBytes;

        /// <summary>
        ///     Old VertexBuffer stride.
        /// </summary>
        private readonly int vertexBufferStride;

        #endregion
    }

    /// <summary>
    ///     Built in device options types to switch for specific drawing methods.
    /// </summary>
    public enum DeviceOptionsType
    {
        /// <summary>
        ///     No Device Option Type.
        /// </summary>
        None,

        /// <summary>
        ///     Switch to the correct rendering states for drawing the 2D Circle.
        /// </summary>
        _2DCircle
    }
}