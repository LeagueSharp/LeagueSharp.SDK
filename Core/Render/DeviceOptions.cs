#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
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
        ///     Device Option list.
        /// </summary>
        private readonly List<DeviceOption> options;

        /// <summary>
        ///     Creates a new Device Options handle, commonly used as a block on draw.
        /// </summary>
        /// <param name="options">Options</param>
        public DeviceOptions(params DeviceOption[] options)
        {
            this.options = options.ToList();

            foreach (var option in options)
            {
                var device = option.Device;
                foreach (var state in option.RenderStates)
                {
                    device.SetRenderState(state.Key, state.Value[0]);
                }
                foreach (var use in option.Use)
                {
                    switch (use.Key)
                    {
                        case DeviceOptionIdentity.Texture:
                            device.SetTexture(option.TextureStage, option.Texture[0]);
                            break;
                        case DeviceOptionIdentity.PixelShader:
                            device.PixelShader = option.PixelShader[0];
                            break;
                        case DeviceOptionIdentity.StreamSource:
                            device.SetStreamSource(
                                option.StreamNumber, option.Buffer[0], option.BufferOffsetInBytes[0],
                                option.BufferStride[0]);
                            break;
                        case DeviceOptionIdentity.VertexFormat:
                            device.VertexFormat = option.VertexFormat[0];
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Dispose of the DeviceOptions and returning the device to default values.
        /// </summary>
        public void Dispose()
        {
            foreach (var option in options)
            {
                var device = option.Device;
                foreach (var state in option.RenderStates)
                {
                    device.SetRenderState(state.Key, state.Value[1]);
                }
                foreach (var use in option.Use)
                {
                    switch (use.Key)
                    {
                        case DeviceOptionIdentity.Texture:
                            device.SetTexture(option.TextureStage, option.Texture[1]);
                            break;
                        case DeviceOptionIdentity.PixelShader:
                            device.PixelShader = option.PixelShader[1];
                            break;
                        case DeviceOptionIdentity.StreamSource:
                            device.SetStreamSource(
                                option.StreamNumber, option.Buffer[1], option.BufferOffsetInBytes[1],
                                option.BufferStride[1]);
                            break;
                        case DeviceOptionIdentity.VertexFormat:
                            device.VertexFormat = option.VertexFormat[1];
                            break;
                    }
                }
            }

            options.Clear();
        }
    }

    /// <summary>
    ///     DeviceOption class, used to mark which options to change on the device and to restore later on.
    /// </summary>
    public class DeviceOption
    {
        /// <summary>
        ///     DeviceOption constructor.
        /// </summary>
        /// <param name="device">Device it will be flipping options</param>
        public DeviceOption(Device device)
        {
            Device = device;

            RenderStates = new Dictionary<RenderState, int[]>();
            Use = new Dictionary<DeviceOptionIdentity, bool>();
        }

        /// <summary>
        ///     The Device.
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        ///     RenderStates to change, containing old and new values.
        /// </summary>
        public Dictionary<RenderState, int[]> RenderStates { get; private set; }

        /// <summary>
        ///     DeviceOptions to change.
        /// </summary>
        public Dictionary<DeviceOptionIdentity, bool> Use { get; private set; }

        /// <summary>
        ///     Buffer to change if needed, contains old and new values.
        /// </summary>
        public VertexBuffer[] Buffer { get; private set; }

        /// <summary>
        ///     Stream number of Buffer to change if needed.
        /// </summary>
        public int StreamNumber { get; private set; }

        /// <summary>
        ///     Buffer Offset in Bytes, contains old and new values.
        /// </summary>
        public int[] BufferOffsetInBytes { get; private set; }

        /// <summary>
        ///     Buffer Stride, contains old and new values.
        /// </summary>
        public int[] BufferStride { get; private set; }

        /// <summary>
        ///     PixelShader, contains old and new values.
        /// </summary>
        public PixelShader[] PixelShader { get; private set; }

        /// <summary>
        ///     TextureStage for Texture flipping.
        /// </summary>
        public int TextureStage { get; private set; }

        /// <summary>
        ///     Texture, contains old and new values.
        /// </summary>
        public BaseTexture[] Texture { get; private set; }

        /// <summary>
        ///     VertexFormat, contains old and new values.
        /// </summary>
        public VertexFormat[] VertexFormat { get; private set; }

        /// <summary>
        ///     Adds a render state change to the device.
        /// </summary>
        /// <param name="renderState">RenderState to change</param>
        /// <param name="newValue">New value for the RenderState</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddRenderState(RenderState renderState, int newValue)
        {
            RenderStates.Add(renderState, new[] { newValue, Device.GetRenderState(renderState) });

            return this;
        }

        /// <summary>
        ///     Adds a render state change to the device.
        /// </summary>
        /// <param name="renderState">RenderState to change</param>
        /// <param name="newValue">New value for the RenderState</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddRenderState(RenderState renderState, RenderState newValue)
        {
            AddRenderState(renderState, Device.GetRenderState(newValue));

            return this;
        }

        /// <summary>
        ///     Adds a render state change to the device.
        /// </summary>
        /// <param name="renderState">RenderState to change</param>
        /// <param name="newValue">New value for the RenderState</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddRenderState(RenderState renderState, bool newValue)
        {
            AddRenderState(renderState, (newValue) ? 1 : 0);

            return this;
        }

        /// <summary>
        ///     Adds a Stream Source change to the device.
        /// </summary>
        /// <param name="streamNumber">Stream Number</param>
        /// <param name="buffer">VertexBuffer</param>
        /// <param name="offsetInBytes">Offset In Bytes</param>
        /// <param name="stride">Stride</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddStreamSource(int streamNumber, VertexBuffer buffer, int offsetInBytes, int stride)
        {
            Use.Add(DeviceOptionIdentity.StreamSource, true);

            VertexBuffer oldBuffer;
            int oldOffsetInBytes;
            int oldStride;
            Device.GetStreamSource(streamNumber, out oldBuffer, out oldOffsetInBytes, out oldStride);

            StreamNumber = streamNumber;
            Buffer = new[] { buffer, oldBuffer };
            BufferOffsetInBytes = new[] { offsetInBytes, oldOffsetInBytes };
            BufferStride = new[] { stride, oldStride };

            return this;
        }

        /// <summary>
        ///     Adds a Texture change to the device based on texture stage.
        /// </summary>
        /// <param name="stage">Stage</param>
        /// <param name="texture">Texture</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddTexture(int stage, BaseTexture texture)
        {
            Use.Add(DeviceOptionIdentity.Texture, true);

            var oldTexture = Device.GetTexture(stage);
            TextureStage = stage;
            Texture = new[] { texture, oldTexture };

            return this;
        }

        /// <summary>
        ///     Adds a PixelShader change to the device.
        /// </summary>
        /// <param name="pixelShader">PixelShader</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddPixelShader(PixelShader pixelShader)
        {
            Use.Add(DeviceOptionIdentity.PixelShader, true);

            var oldShader = Device.PixelShader;
            PixelShader = new[] { pixelShader, oldShader };

            return this;
        }

        /// <summary>
        ///     Adds a VertexFormat change to the device.
        /// </summary>
        /// <param name="vertexFormat">VertexFormat</param>
        /// <returns>Device Option Instance</returns>
        public DeviceOption AddVertexFormat(VertexFormat vertexFormat)
        {
            Use.Add(DeviceOptionIdentity.VertexFormat, true);

            var oldFormat = Device.VertexFormat;
            VertexFormat = new[] { vertexFormat, oldFormat };

            return this;
        }
    }
}