#region

using System;
using System.Collections.Generic;
using System.Linq;
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
        ///     Device Pointer/Instance.
        /// </summary>
        private readonly Device device;

        /// <summary>
        ///     Device Option list.
        /// </summary>
        private readonly List<DeviceOption> options;

        /// <summary>
        ///     Creates a new Device Options handle, commonly used as a block on draw.
        /// </summary>
        /// <param name="device">Device</param>
        /// <param name="options">Options</param>
        public DeviceOptions(Device device, params DeviceOption[] options)
        {
            if (device == null || device.IsDisposed)
            {
                return;
            }

            this.device = device;
            this.options = options.ToList();

            foreach (var option in options)
            {
                switch (option.Type)
                {
                    case DeviceOptionIdentity.RenderState:
                        option.OldValue = device.GetRenderState((RenderState) option.Key);
                        device.SetRenderState((RenderState) option.Key, (int) option.Value);
                        break;
                    case DeviceOptionIdentity.Texture:
                        var stage = option.Key as int? ?? 0;
                        option.OldValue = device.GetTexture(stage);
                        device.SetTexture(stage, (BaseTexture) option.Value);
                        break;
                    default:
                        if (option.Type == DeviceOptionIdentity.Texture)
                        {
                            option.OldValue = device.PixelShader;
                            device.PixelShader = (PixelShader) option.Value;
                        }
                        else if (option.Type == DeviceOptionIdentity.StreamSource && ((object[]) option.Value).Length > 2)
                        {
                            VertexBuffer streamDataOut;
                            int offsetInBytesRef;
                            int stride;
                            device.GetStreamSource(0, out streamDataOut, out offsetInBytesRef, out stride);

                            option.OldValue = new object[] { streamDataOut, offsetInBytesRef, stride };

                            var value = ((object[]) option.Value);
                            device.SetStreamSource(0, (VertexBuffer) value[0], (int) value[1], (int) value[2]);
                        }
                        else if (option.Type == DeviceOptionIdentity.VertexFormat)
                        {
                            option.OldValue = device.VertexFormat;
                            device.VertexFormat = (VertexFormat) option.Value;
                        }
                        break;
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
                switch (option.Type)
                {
                    case DeviceOptionIdentity.RenderState:
                        device.SetRenderState((RenderState) option.Key, (int) option.OldValue);
                        break;
                    case DeviceOptionIdentity.Texture:
                        device.SetTexture(option.Key as int? ?? 0, (BaseTexture) option.OldValue);
                        break;
                    default:
                        if (option.Type == DeviceOptionIdentity.Texture)
                        {
                            device.PixelShader = (PixelShader) option.OldValue;
                        }
                        else if (option.Type == DeviceOptionIdentity.StreamSource && ((object[]) option.Value).Length > 2)
                        {
                            var value = ((object[]) option.OldValue);
                            device.SetStreamSource(0, (VertexBuffer) value[0], (int) value[1], (int) value[2]);
                        }
                        else if (option.Type == DeviceOptionIdentity.VertexFormat)
                        {
                            device.VertexFormat = (VertexFormat) option.OldValue;
                        }
                        break;
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
        ///     DeviceOption RenderState constructor.
        /// </summary>
        /// <param name="state">RenderState</param>
        /// <param name="value">New Value</param>
        public DeviceOption(RenderState state, object value)
        {
            Type = DeviceOptionIdentity.RenderState;

            Key = state;
            Value = value;
        }

        /// <summary>
        ///     Identity constructor.
        /// </summary>
        /// <param name="identity">Device Option Identity</param>
        /// <param name="value">New Value</param>
        /// <param name="key">Key [Optional]</param>
        public DeviceOption(DeviceOptionIdentity identity, object value, object key = null)
        {
            Type = identity;
            Value = value;
            Key = key;
        }

        /// <summary>
        ///     The type of the device option, (identity).
        /// </summary>
        public DeviceOptionIdentity Type { get; set; }

        /// <summary>
        ///     The key of the device option.
        /// </summary>
        public object Key { get; set; }

        /// <summary>
        ///     The new value to be set on changes request.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     The old value reference.
        /// </summary>
        public object OldValue { get; set; }
    }

    /// <summary>
    ///     Device Option Indetity types.
    /// </summary>
    public enum DeviceOptionIdentity
    {
        /// <summary>
        ///     RenderState Type
        /// </summary>
        RenderState,

        /// <summary>
        ///     Texture Type
        /// </summary>
        Texture,

        /// <summary>
        ///     PixelShader Type
        /// </summary>
        PixelShader,

        /// <summary>
        ///     StreamSource Type
        /// </summary>
        StreamSource,

        /// <summary>
        ///     VertexFormat Type
        /// </summary>
        VertexFormat
    }
}