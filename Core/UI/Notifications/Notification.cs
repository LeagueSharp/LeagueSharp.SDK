// <copyright file="Notification.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Properties;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Font = SharpDX.Direct3D9.Font;
    using Rectangle = SharpDX.Rectangle;

    /// <summary>
    ///     The notification.
    /// </summary>
    public class Notification : ANotification
    {
        #region Constants

        /// <summary>
        ///     The maximum body line length.
        /// </summary>
        private const int MaximumBodyLineLength = 283;

        /// <summary>
        ///     The maximum header line length.
        /// </summary>
        private const int MaximumHeaderLineLength = 250;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The body font.
        /// </summary>
        private static readonly Font BodyFont = new Font(
            Drawing.Direct3DDevice,
            13,
            0,
            FontWeight.DoNotCare,
            5,
            false,
            FontCharacterSet.Default,
            FontPrecision.Character,
            FontQuality.Antialiased,
            FontPitchAndFamily.Mono | FontPitchAndFamily.Decorative,
            "Tahoma");

        /// <summary>
        ///     The header font.
        /// </summary>
        private static readonly Font HeaderFont = new Font(
            Drawing.Direct3DDevice,
            16,
            0,
            FontWeight.Bold,
            5,
            false,
            FontCharacterSet.Default,
            FontPrecision.Character,
            FontQuality.Antialiased,
            FontPitchAndFamily.Mono | FontPitchAndFamily.Decorative,
            "Tahoma");

        /// <summary>
        ///     The hide icon texture.
        /// </summary>
        private static readonly Texture HideTexture = Texture.FromMemory(
            Drawing.Direct3DDevice,
            (byte[])new ImageConverter().ConvertTo(Resources.notifications_arrow, typeof(byte[])),
            Resources.notifications_arrow.Width,
            Resources.notifications_arrow.Height,
            0,
            Usage.None,
            Format.A1,
            Pool.Managed,
            Filter.Default,
            Filter.Default,
            0);

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice)
                                                { Antialias = true, GLLines = true, Width = 300f };

        /// <summary>
        ///     The sprite.
        /// </summary>
        private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

        #endregion

        #region Fields

        /// <summary>
        ///     The animation tick.
        /// </summary>
        private int animationTick;

        /// <summary>
        ///     The body.
        /// </summary>
        private string body;

        /// <summary>
        ///     The footer.
        /// </summary>
        private string footer;

        /// <summary>
        ///     The header.
        /// </summary>
        private string header;

        /// <summary>
        ///     The hide animation boolean.
        /// </summary>
        private bool hideAnimation;

        /// <summary>
        ///     The hide offset x axis.
        /// </summary>
        private float hideOffsetX;

        /// <summary>
        ///     The notification icon.
        /// </summary>
        private NotificationIconType icon;

        /// <summary>
        ///     The icon color tick.
        /// </summary>
        private int iconColorTick;

        /// <summary>
        ///     The icon flash.
        /// </summary>
        private bool iconFlash;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Notification" /> class.
        /// </summary>
        /// <param name="header">
        ///     The header
        /// </param>
        /// <param name="body">
        ///     The body
        /// </param>
        public Notification(string header, string body)
            : this()
        {
            this.Header = header;
            this.Body = body;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Notification" /> class.
        /// </summary>
        /// <param name="header">
        ///     The header
        /// </param>
        /// <param name="body">
        ///     The body
        /// </param>
        /// <param name="footer">
        ///     The footer
        /// </param>
        public Notification(string header, string body, string footer)
            : this(header, body)
        {
            this.Footer = footer;
        }

        /// <summary>
        ///     Prevents a default instance of the <see cref="Notification" /> class from being created.
        /// </summary>
        private Notification()
        {
            this.IsVisible = true;
            this.ExtraFooterPadding = 10f;
            this.IconOffset = 33f;
            this.HeaderHeight = 30f;
            this.BodyHeight = 60f;

            this.HeaderTextColor = Color.Gold;
            this.BodyTextColor = Color.White;
            this.FooterTextColor = Color.White;
            this.LinesList = new List<string>();
            this.IconColor = Color.White;
            this.ActiveIconColor = Color.White;
            this.SecondIconColor = Color.White;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the active icon color.
        /// </summary>
        public ColorBGRA ActiveIconColor { get; set; }

        /// <summary>
        ///     Gets or sets the body.
        /// </summary>
        public string Body
        {
            get
            {
                return this.body;
            }

            set
            {
                this.LinesList = this.FormatText(value, true);
                this.body = string.IsNullOrEmpty(value) ? " " : value;
            }
        }

        /// <summary>
        ///     Gets or sets the body text color.
        /// </summary>
        public ColorBGRA BodyTextColor { get; set; }

        /// <summary>
        ///     Gets or sets the extra footer padding.
        /// </summary>
        public float ExtraFooterPadding { get; set; }

        /// <summary>
        ///     Gets or sets the footer.
        /// </summary>
        public string Footer
        {
            get
            {
                return this.footer;
            }

            set
            {
                this.footer = string.IsNullOrEmpty(value) ? " " : value;
            }
        }

        /// <summary>
        ///     Gets the footer height.
        /// </summary>
        public float FooterHeight => 20f;

        /// <summary>
        ///     Gets or sets the footer text color.
        /// </summary>
        public ColorBGRA FooterTextColor { get; set; }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                if (HeaderFont.MeasureText(Sprite, value, 0).Width > MaximumHeaderLineLength)
                {
                    string final = null;
                    for (var i = value.Length; i > 0; --i)
                    {
                        if (HeaderFont.MeasureText(Sprite, value.Substring(0, i) + "...", 0).Width
                            <= MaximumHeaderLineLength)
                        {
                            final = value.Substring(0, i) + "...";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(final))
                    {
                        this.header = final;
                        return;
                    }
                }

                this.header = string.IsNullOrEmpty(value) ? " " : value;
            }
        }

        /// <summary>
        ///     Gets the header height.
        /// </summary>
        public float HeaderHeight { get; private set; }

        /// <summary>
        ///     Gets or sets the header text color.
        /// </summary>
        public ColorBGRA HeaderTextColor { get; set; }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public NotificationIconType Icon
        {
            get
            {
                return this.icon;
            }

            set
            {
                this.icon = value;
                this.IconTexture = NotificationIcons.GetIcon(value);
            }
        }

        /// <summary>
        ///     Gets or sets the icon color.
        /// </summary>
        public ColorBGRA IconColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether icon flash.
        /// </summary>
        public bool IconFlash
        {
            get
            {
                return this.iconFlash;
            }

            set
            {
                this.iconFlash = value;
                if (!value)
                {
                    this.ActiveIconColor = this.IconColor;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the icon offset.
        /// </summary>
        public float IconOffset { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the notification is open.
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the notification is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        ///     Gets or sets the string lines list.
        /// </summary>
        public List<string> LinesList { get; set; }

        /// <summary>
        ///     Gets or sets the second icon color.
        /// </summary>
        public ColorBGRA SecondIconColor { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public float Width => Line.Width;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the hide icon bitmap.
        /// </summary>
        private static Bitmap HideBitmap => Resources.notifications_arrow;

        /// <summary>
        ///     Gets or sets the body height.
        /// </summary>
        private float BodyHeight { get; set; }

        /// <summary>
        ///     Gets or sets the draw body height.
        /// </summary>
        private float DrawBodyHeight { get; set; }

        /// <summary>
        ///     Gets or sets the draw footer height.
        /// </summary>
        private float DrawFooterHeight { get; set; }

        /// <summary>
        ///     Gets or sets the icon texture.
        /// </summary>
        private Texture IconTexture { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Formats the given text into lines.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="htmlSupport">
        ///     Indicates whether to support HTML tags.
        /// </param>
        /// <returns>
        ///     The formatted list.
        /// </returns>
        public List<string> FormatText(string value, bool htmlSupport)
        {
            var lines = (BodyFont.MeasureText(Sprite, value, 0).Width / MaximumBodyLineLength) + 1;
            var lastIndex = 0;
            var format = false;
            var linesList = new List<string>();

            if (value.Contains("</br>") && htmlSupport)
            {
                lines = 0;
                var formatLines = new List<string>();
                var valueCopy = value;
                while (valueCopy.Contains("</br>"))
                {
                    var breakLine = valueCopy.Substring(0, valueCopy.IndexOf("</br>", StringComparison.Ordinal));
                    ++lines;
                    formatLines.Add(!string.IsNullOrEmpty(breakLine) ? breakLine : string.Empty);

                    valueCopy = valueCopy.Substring(
                        valueCopy.IndexOf("</br>", StringComparison.Ordinal) + 5,
                        valueCopy.Length - valueCopy.IndexOf("</br>", StringComparison.Ordinal) - 5);
                }

                formatLines.Add(string.IsNullOrEmpty(valueCopy) ? " " : valueCopy);
                this.BodyHeight += BodyFont.Description.Height;

                foreach (var line in formatLines)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        linesList.Add(" ");
                        this.BodyHeight += BodyFont.Description.Height;
                        continue;
                    }

                    for (var j = line.Length; j > -1; --j)
                    {
                        if (j - 1 > -1 && line.Length - lastIndex - j >= 0
                            && BodyFont.MeasureText(Sprite, line.Substring(lastIndex, line.Length - lastIndex - j), 0)
                                   .Width < MaximumBodyLineLength)
                        {
                            continue;
                        }

                        var original = line.Substring(lastIndex, line.Length - lastIndex - j);

                        if (!string.IsNullOrEmpty(original))
                        {
                            linesList.Add(original);
                        }

                        lastIndex = line.Length - j;
                    }

                    lastIndex = 0;
                }

                format = true;
            }

            if (!format)
            {
                for (var j = value.Length; j > -1; --j)
                {
                    if (j - 1 > -1 && value.Length - lastIndex - j >= 0
                        && BodyFont.MeasureText(Sprite, value.Substring(lastIndex, value.Length - lastIndex - j), 0)
                               .Width < MaximumBodyLineLength)
                    {
                        continue;
                    }

                    var original = value.Substring(lastIndex, value.Length - lastIndex - j);

                    if (!string.IsNullOrEmpty(original))
                    {
                        linesList.Add(original);
                    }

                    lastIndex = value.Length - j;
                }
            }

            if (lines > 4)
            {
                this.BodyHeight += BodyFont.Description.Height * (lines - 4);
            }

            return linesList;
        }

        /// <summary>
        ///     Gets the notification reserved height.
        /// </summary>
        /// <returns>
        ///     The reserved height in float units.
        /// </returns>
        public override float GetReservedHeight()
        {
            return this.HeaderHeight + this.DrawBodyHeight + this.DrawFooterHeight + this.ExtraFooterPadding;
        }

        /// <summary>
        ///     Gets the notification reserved width.
        /// </summary>
        /// <returns>
        ///     The reserved width in float units.
        /// </returns>
        public override float GetReservedWidth()
        {
            return this.Width;
        }

        /// <summary>
        ///     OnDraw event, specifies a drawing callback which is after IDirect3DDevice9::BeginScene and before
        ///     IDirect3DDevice9::EndScene.
        /// </summary>
        /// <param name="basePosition">
        ///     The base position
        /// </param>
        public override void OnDraw(Vector2 basePosition)
        {
            if (this.IsVisible)
            {
                basePosition.X += this.hideOffsetX;

                Sprite.Begin(SpriteFlags.AlphaBlend);

                Line.Begin();

                Line.Draw(
                    new[]
                        {
                            new Vector2(basePosition.X - (this.Width / 2f), basePosition.Y),
                            new Vector2(basePosition.X - (this.Width / 2f), basePosition.Y + this.HeaderHeight)
                        },
                    new ColorBGRA(0, 0, 0, 255 / 2));

                if (this.IsOpen || this.DrawBodyHeight > 0 || this.DrawFooterHeight > 0)
                {
                    Line.Draw(
                        new[]
                            {
                                new Vector2(basePosition.X - (this.Width / 2f), basePosition.Y + this.HeaderHeight),
                                new Vector2(
                                    basePosition.X - (this.Width / 2f),
                                    basePosition.Y + this.HeaderHeight + this.DrawBodyHeight)
                            },
                        new ColorBGRA(0, 0, 0, (byte)(255 / 1.5f)));
                    Line.Draw(
                        new[]
                            {
                                new Vector2(
                                    basePosition.X - (this.Width / 2f),
                                    basePosition.Y + this.HeaderHeight + this.DrawBodyHeight),
                                new Vector2(
                                    basePosition.X - (this.Width / 2f),
                                    basePosition.Y + this.HeaderHeight + this.DrawFooterHeight + this.DrawBodyHeight)
                            },
                        new ColorBGRA(0, 0, 0, (byte)(255 / 1.25f)));
                }

                Line.End();

                HeaderFont.DrawText(Sprite, this.Header, this.GetHeaderRectangle(basePosition), 0, this.HeaderTextColor);

                var rectangle = this.GetBodyRectangle(basePosition);
                if (this.IsOpen || rectangle.Height > 0)
                {
                    for (var i = 0; i < this.LinesList.Count; ++i)
                    {
                        var lineRectangle = rectangle;
                        lineRectangle.Y += BodyFont.Description.Height * i;
                        lineRectangle.Height -= BodyFont.Description.Height * i;

                        BodyFont.DrawText(Sprite, this.LinesList[i], lineRectangle, 0, this.BodyTextColor);
                    }
                }

                var matrix = Sprite.Transform;

                if (this.Icon != NotificationIconType.None && this.IconTexture != null && !this.IconTexture.IsDisposed)
                {
                    Sprite.Transform = Matrix.Translation(basePosition.X - this.IconOffset, basePosition.Y + 0.5f, 0);
                    Sprite.Draw(this.IconTexture, this.IconColor);
                }

                if (HideTexture != null && !HideTexture.IsDisposed)
                {
                    Sprite.Transform = Matrix.Scaling(0.7f, 0.7f, 0f)
                                       * Matrix.Translation(
                                           basePosition.X - this.Width + 5f,
                                           basePosition.Y + this.HeaderHeight + this.BodyHeight + 1.5f,
                                           0f);
                    Sprite.Draw(
                        HideTexture,
                        Color.White,
                        new Rectangle(0, 0, HideBitmap.Width, (int)this.DrawFooterHeight));
                }

                Sprite.Transform = matrix;

                Sprite.End();
            }
        }

        /// <summary>
        ///     OnUpdate event, occurring after a game tick.
        /// </summary>
        public override void OnUpdate()
        {
            if (Variables.TickCount - this.animationTick > 3)
            {
                if ((this.IsOpen && this.DrawBodyHeight <= this.BodyHeight && !this.hideAnimation)
                    || (!this.IsOpen && this.DrawFooterHeight <= 0 && this.DrawBodyHeight > 0))
                {
                    this.DrawBodyHeight += this.IsOpen ? 1 : -1;
                }

                if ((this.IsOpen && this.DrawBodyHeight >= this.BodyHeight && this.DrawFooterHeight <= this.FooterHeight
                     && !this.hideAnimation) || (!this.IsOpen && this.DrawFooterHeight > 0))
                {
                    this.DrawFooterHeight += this.IsOpen ? 1 : -1;
                }

                if (this.hideAnimation && this.hideOffsetX < this.Width + 5)
                {
                    this.hideOffsetX++;
                }

                if (this.hideAnimation && this.hideOffsetX >= this.Width + 5)
                {
                    if (this.HeaderHeight > 0 && this.DrawBodyHeight <= 0)
                    {
                        this.HeaderHeight--;
                    }

                    if (this.DrawBodyHeight > 0 && this.DrawFooterHeight <= 0)
                    {
                        this.DrawBodyHeight--;
                    }

                    if (this.DrawFooterHeight > 0)
                    {
                        this.DrawFooterHeight--;
                    }

                    if (this.DrawFooterHeight <= 0 && this.DrawBodyHeight <= 0 && this.HeaderHeight <= 0)
                    {
                        Notifications.Remove(this);
                    }
                }

                this.animationTick = Variables.TickCount;
            }

            if (Variables.TickCount - this.iconColorTick > 400 && this.IconFlash)
            {
                this.IconColor = this.ActiveIconColor == this.IconColor ? this.SecondIconColor : this.IconColor;
                this.iconColorTick = Variables.TickCount;
            }
        }

        /// <summary>
        ///     <c>OnWndProc</c> event, occurs on a windows process message to the thread.
        /// </summary>
        /// <param name="basePosition">
        ///     The base position
        /// </param>
        /// <param name="windowsKeys">
        ///     The windows keys
        /// </param>
        /// <param name="isEdit">
        ///     Indicates whether it's an edit message.
        /// </param>
        public override void OnWndProc(Vector2 basePosition, WindowsKeys windowsKeys, bool isEdit)
        {
            basePosition.X += this.hideOffsetX;

            if (!isEdit)
            {
                if (windowsKeys.Msg == WindowsMessages.LBUTTONDOWN
                    && windowsKeys.Cursor.IsUnderRectangle(
                        basePosition.X - this.Width - 5,
                        basePosition.Y,
                        this.Width,
                        this.HeaderHeight))
                {
                    windowsKeys.Process = false;
                    this.IsOpen = !this.IsOpen;
                }

                if (windowsKeys.Msg == WindowsMessages.LBUTTONDOWN
                    && windowsKeys.Cursor.IsUnderRectangle(
                        basePosition.X - this.Width + 5f,
                        basePosition.Y + this.HeaderHeight + this.BodyHeight + 1.5f,
                        HideBitmap.Width - 3,
                        HideBitmap.Height * 0.7f) && this.DrawBodyHeight >= this.BodyHeight
                    && this.DrawFooterHeight >= this.FooterHeight)
                {
                    windowsKeys.Process = false;
                    this.hideAnimation = !this.hideAnimation;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get body rectangle.
        /// </summary>
        /// <param name="basePosition">
        ///     The base Position.
        /// </param>
        /// <returns>
        ///     The <see cref="SharpDX.Rectangle" />.
        /// </returns>
        private Rectangle GetBodyRectangle(Vector2 basePosition)
        {
            const int BaseLeftPosition = 144;

            var x = basePosition.X - (this.Width / 2f) - BaseLeftPosition;
            var y = basePosition.Y + this.HeaderHeight + 2f;

            return new Rectangle((int)x, (int)y, (int)this.Width, (int)this.DrawBodyHeight);
        }

        /// <summary>
        ///     Get header rectangle.
        /// </summary>
        /// <param name="basePosition">
        ///     The base Position.
        /// </param>
        /// <returns>
        ///     The <see cref="System.Drawing.Rectangle" />.
        /// </returns>
        private Rectangle GetHeaderRectangle(Vector2 basePosition)
        {
            const int BaseLeftPosition = 140;

            var x = basePosition.X - (this.Width / 2f) - BaseLeftPosition;
            var y = basePosition.Y
                    + new Rectangle(0, 0, 0, (int)this.HeaderHeight).GetCenteredText(
                        Sprite,
                        this.Header,
                        CenteredFlags.VerticalCenter).Y;

            return new Rectangle((int)x, (int)y, (int)this.Width, (int)this.HeaderHeight);
        }

        #endregion
    }
}