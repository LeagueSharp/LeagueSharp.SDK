using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Values
{
    using System.Runtime.Serialization;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    using Math = System.Math;

    [Serializable]
    public class MenuColor : AMenuValue, ISerializable
    {
        public override Vector2 Position { get; set; }

        public ColorBGRA Color { get; set; }

        public bool Active { get; set; }

        public bool HoveringPreview { get; set; }

        public bool InteractingRed { get; set; }

        public bool InteractingGreen { get; set; }

        public bool InteractingBlue { get; set; }

        public bool InteractingAlpha { get; set; }

        public MenuColor()
            : this(new ColorBGRA(0, 0, 0, 255)) {}

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

        public override int Width
        {
            get
            {
                return ThemeManager.Current.ColorPicker.Width(this);
            }
        }

        public override void Extract(AMenuValue component)
        {
            Color = ((MenuColor)component).Color;
        }

        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            ThemeManager.Current.ColorPicker.OnDraw(component, position, index);
        }

        public override void OnWndProc(Utils.WindowsKeys args)
        {
            if (!this.Container.Visible)
            {
                return;
            }

            var previewRect = ThemeManager.Current.ColorPicker.PreviewBoundaries(this.Position, this.Container);
            var pickerRect = ThemeManager.Current.ColorPicker.PickerBoundaries(this.Position, this.Container);
            var redRect = ThemeManager.Current.ColorPicker.RedPickerBoundaries(this.Position, this.Container);
            var greenRect = ThemeManager.Current.ColorPicker.GreenPickerBoundaries(this.Position, this.Container);
            var blueRect = ThemeManager.Current.ColorPicker.BluePickerBoundaries(this.Position, this.Container);
            var alphaRect = ThemeManager.Current.ColorPicker.AlphaPickerBoundaries(this.Position, this.Container);

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
        }

        private void UpdateGreen(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, GetByte(args, rect), Color.B, Color.A);
        }

        private void UpdateBlue(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, Color.G, GetByte(args, rect), Color.A);
        }

        private void UpdateAlpha(WindowsKeys args, Rectangle rect)
        {
            Color = new ColorBGRA(Color.R, Color.G, Color.B, GetByte(args, rect));
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("red", Color.R, typeof(byte));
            info.AddValue("green", Color.G, typeof(byte));
            info.AddValue("blue", Color.B, typeof(byte));
            info.AddValue("alpha", Color.A, typeof(byte));
        }
    }
}