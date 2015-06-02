using System;

namespace LeagueSharp.SDK.Core.UI.Values
{
    using System.Runtime.Serialization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     The menu color.
    /// </summary>
    [Serializable]
    public class MenuColor : AMenuValue, ISerializable
    {
        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether hovering preview.
        /// </summary>
        public bool HoveringPreview { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting red.
        /// </summary>
        public bool InteractingRed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting green.
        /// </summary>
        public bool InteractingGreen { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting blue.
        /// </summary>
        public bool InteractingBlue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting alpha.
        /// </summary>
        public bool InteractingAlpha { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        public MenuColor()
            : this(new ColorBGRA(0, 0, 0, 255)) {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="color">
        ///     The color
        /// </param>
        public MenuColor(ColorBGRA color)
        {
            Color = color;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuColor(SerializationInfo info, StreamingContext context)
        {
            byte red = (byte)info.GetValue("red", typeof(byte));
            byte green = (byte)info.GetValue("green", typeof(byte));
            byte blue = (byte)info.GetValue("blue", typeof(byte));
            byte alpha = (byte)info.GetValue("alpha", typeof(byte));
            this.Color = new ColorBGRA(red, green, blue, alpha);
        }

        /// <summary>
        ///     Gets the Value Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return ThemeManager.Current.ColorPicker.Width(this);
            }
        }

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
            Color = ((MenuColor)component).Color;
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void OnDraw()
        {
            ThemeManager.Current.ColorPicker.Draw(this);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(Utils.WindowsKeys args)
        {
            if (!this.Container.Visible)
            {
                return;
            }

            var previewRect = ThemeManager.Current.ColorPicker.PreviewBoundaries(this);
            var pickerRect = ThemeManager.Current.ColorPicker.PickerBoundaries(this);
            var redRect = ThemeManager.Current.ColorPicker.RedPickerBoundaries(this);
            var greenRect = ThemeManager.Current.ColorPicker.GreenPickerBoundaries(this);
            var blueRect = ThemeManager.Current.ColorPicker.BluePickerBoundaries(this);
            var alphaRect = ThemeManager.Current.ColorPicker.AlphaPickerBoundaries(this);

            if (args.Msg == WindowsMessages.MOUSEMOVE)
            {
                if (args.Cursor.IsUnderRectangle(previewRect.X, previewRect.Y, previewRect.Width, previewRect.Height))
                {
                    HoveringPreview = true;
                }
                else
                {
                    HoveringPreview = false;
                }

                if (Active)
                {
                    if (InteractingRed)
                    {
                        UpdateRed(args, redRect);
                    }
                    else if (InteractingGreen)
                    {
                        UpdateGreen(args, greenRect);
                    }
                    else if (InteractingBlue)
                    {
                        UpdateBlue(args, blueRect);
                    }
                    else if (InteractingAlpha)
                    {
                        UpdateAlpha(args, alphaRect);
                    }
                }
                
            }

            if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                InteractingRed = false;
                InteractingGreen = false;
                InteractingBlue = false;
                InteractingAlpha = false;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                if (args.Cursor.IsUnderRectangle(previewRect.X, previewRect.Y, previewRect.Width, previewRect.Height))
                {
                    Active = true;
                }
                else if (args.Cursor.IsUnderRectangle(pickerRect.X, pickerRect.Y, pickerRect.Width, pickerRect.Height) && Active)
                {
                    if (args.Cursor.IsUnderRectangle(redRect.X, redRect.Y, redRect.Width, redRect.Height))
                    {
                        InteractingRed = true;
                        UpdateRed(args, redRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(greenRect.X, greenRect.Y, greenRect.Width, greenRect.Height))
                    {
                        InteractingGreen = true;
                        UpdateGreen(args, greenRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(blueRect.X, blueRect.Y, blueRect.Width, blueRect.Height))
                    {
                        InteractingBlue = true;
                        UpdateBlue(args, blueRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(alphaRect.X, alphaRect.Y, alphaRect.Width, alphaRect.Height))
                    {
                        InteractingAlpha = true;
                        UpdateAlpha(args, alphaRect);
                    }
                }
                else
                {
                    Active = false;
                }
            }
        }

        private void UpdateRed(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(GetByte(args, rect), Color.G, Color.B, Color.A);
            FireEvent();
        }

        private void UpdateGreen(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, GetByte(args, rect), Color.B, Color.A);
            FireEvent();
        }

        private void UpdateBlue(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, Color.G, GetByte(args, rect), Color.A);
            FireEvent();
        }

        private void UpdateAlpha(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, Color.G, Color.B, GetByte(args, rect));
            FireEvent();
        }

        private byte GetByte(WindowsKeys args, Rectangle rect)
        {
            if (args.Cursor.X < rect.X)
            {
                return 0;
            }
            
            if (args.Cursor.X > rect.X + rect.Width)
            {
                return 255;
            }
            return (byte) (((args.Cursor.X - rect.X) / rect.Width) * 255);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param><param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param><exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("red", Color.R, typeof(byte));
            info.AddValue("green", Color.G, typeof(byte));
            info.AddValue("blue", Color.B, typeof(byte));
            info.AddValue("alpha", Color.A, typeof(byte));
        }
    }
}