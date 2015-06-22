// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuColor.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The menu color.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     The menu color.
    /// </summary>
    [Serializable]
    public class MenuColor : MenuItem, ISerializable
    {
        private readonly ColorBGRA original;

        #region Constructors and Destructors
        

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="color">
        ///     The color
        /// </param>
        public MenuColor(string name, string displayName, ColorBGRA color, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Color = color;
            original = color;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuColor(SerializationInfo info, StreamingContext context)
        {
            var red = (byte)info.GetValue("red", typeof(byte));
            var green = (byte)info.GetValue("green", typeof(byte));
            var blue = (byte)info.GetValue("blue", typeof(byte));
            var alpha = (byte)info.GetValue("alpha", typeof(byte));
            this.Color = new ColorBGRA(red, green, blue, alpha);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether hovering preview.
        /// </summary>
        public bool HoveringPreview { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting alpha.
        /// </summary>
        public bool InteractingAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting blue.
        /// </summary>
        public bool InteractingBlue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting green.
        /// </summary>
        public bool InteractingGreen { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting red.
        /// </summary>
        public bool InteractingRed { get; set; }

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(MenuItem component)
        {
            this.Color = ((MenuColor)component).Color;
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void Draw()
        {
            ThemeManager.Current.ColorPicker.Draw(this);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void WndProc(WindowsKeys args)
        {
            if (!this.Visible)
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
                this.HoveringPreview = args.Cursor.IsUnderRectangle(
                    previewRect.X, 
                    previewRect.Y, 
                    previewRect.Width, 
                    previewRect.Height);

                if (this.Active)
                {
                    if (this.InteractingRed)
                    {
                        this.UpdateRed(args, redRect);
                    }
                    else if (this.InteractingGreen)
                    {
                        this.UpdateGreen(args, greenRect);
                    }
                    else if (this.InteractingBlue)
                    {
                        this.UpdateBlue(args, blueRect);
                    }
                    else if (this.InteractingAlpha)
                    {
                        this.UpdateAlpha(args, alphaRect);
                    }
                }
            }

            if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.InteractingRed = false;
                this.InteractingGreen = false;
                this.InteractingBlue = false;
                this.InteractingAlpha = false;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                if (args.Cursor.IsUnderRectangle(previewRect.X, previewRect.Y, previewRect.Width, previewRect.Height))
                {
                    this.Active = true;
                }
                else if (args.Cursor.IsUnderRectangle(pickerRect.X, pickerRect.Y, pickerRect.Width, pickerRect.Height)
                         && this.Active)
                {
                    if (args.Cursor.IsUnderRectangle(redRect.X, redRect.Y, redRect.Width, redRect.Height))
                    {
                        this.InteractingRed = true;
                        this.UpdateRed(args, redRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        greenRect.X, 
                        greenRect.Y, 
                        greenRect.Width, 
                        greenRect.Height))
                    {
                        this.InteractingGreen = true;
                        this.UpdateGreen(args, greenRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        blueRect.X, 
                        blueRect.Y, 
                        blueRect.Width, 
                        blueRect.Height))
                    {
                        this.InteractingBlue = true;
                        this.UpdateBlue(args, blueRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        alphaRect.X, 
                        alphaRect.Y, 
                        alphaRect.Width, 
                        alphaRect.Height))
                    {
                        this.InteractingAlpha = true;
                        this.UpdateAlpha(args, alphaRect);
                    }
                }
                else
                {
                    this.Active = false;
                }
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("red", this.Color.R, typeof(byte));
            info.AddValue("green", this.Color.G, typeof(byte));
            info.AddValue("blue", this.Color.B, typeof(byte));
            info.AddValue("alpha", this.Color.A, typeof(byte));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("red", this.Color.R, typeof(byte));
            info.AddValue("green", this.Color.G, typeof(byte));
            info.AddValue("blue", this.Color.B, typeof(byte));
            info.AddValue("alpha", this.Color.A, typeof(byte));
        }

        /// <summary>
        ///     Updates the alpha value.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private void UpdateAlpha(WindowsKeys args, Rectangle rect)
        {
            this.Color = new ColorBGRA(this.Color.R, this.Color.G, this.Color.B, GetByte(args, rect));
            this.FireEvent();
        }

        /// <summary>
        ///     Updates the blue value.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private void UpdateBlue(WindowsKeys args, Rectangle rect)
        {
            this.Color = new ColorBGRA(this.Color.R, this.Color.G, GetByte(args, rect), this.Color.A);
            this.FireEvent();
        }

        /// <summary>
        ///     Updates the green value.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private void UpdateGreen(WindowsKeys args, Rectangle rect)
        {
            this.Color = new ColorBGRA(this.Color.R, GetByte(args, rect), this.Color.B, this.Color.A);
            this.FireEvent();
        }

        /// <summary>
        ///     Updates the red value.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private void UpdateRed(WindowsKeys args, Rectangle rect)
        {
            this.Color = new ColorBGRA(GetByte(args, rect), this.Color.G, this.Color.B, this.Color.A);
            this.FireEvent();
        }

        /// <summary>
        ///     Gets the byte.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        /// <returns>
        ///     The byte.
        /// </returns>
        private static byte GetByte(WindowsKeys args, Rectangle rect)
        {
            if (args.Cursor.X < rect.X)
            {
                return 0;
            }

            if (args.Cursor.X > rect.X + rect.Width)
            {
                return 255;
            }

            return (byte)(((args.Cursor.X - rect.X) / rect.Width) * 255);
        }

        #endregion

        /// <summary>
        /// Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            Color = original;
        }
    }
}