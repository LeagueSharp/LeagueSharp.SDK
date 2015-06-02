// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawText.cs" company="LeagueSharp">
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
//   The DrawText class, contains a set of tool to simply draw pre-defined text and simple text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.IDrawing
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The DrawText class, contains a set of tool to simply draw pre-defined text and simple text.
    /// </summary>
    public class DrawText : DrawObject, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The texture sprite.
        /// </summary>
        private Sprite sprite;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawText" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawText(Vector2? position = null)
            : base(position ?? new Vector2())
        {
            this.Scale = new Vector2(1f, 1f);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawText" /> class.
        /// </summary>
        /// <param name="font">
        ///     The font
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawText(Font font, Vector2? position = null)
            : this(position ?? new Vector2())
        {
            this.Font = font;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawText" /> class.
        /// </summary>
        /// <param name="font">
        ///     The font
        /// </param>
        /// <param name="text">
        ///     The text
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawText(Font font, string text, Vector2? position = null)
            : this(font, position)
        {
            this.Text = text;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DrawText" /> class.
        /// </summary>
        ~DrawText()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the center.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        ///     Gets or sets the corp rectangle.
        /// </summary>
        public Rectangle? CorpRectangle { get; set; }

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        ///     Gets or sets the font draw flags.
        /// </summary>
        public FontDrawFlags FontDrawFlags { get; set; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(this.Font.MeasureText(this.Sprite, this.Text, 0).Height * this.Scale.Y);
            }
        }

        /// <summary>
        ///     Gets or sets the rotation.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        ///     Gets or sets the scale.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        ///     Gets the size.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(
                    this.Font.MeasureText(this.Sprite, this.Text, 0).Height, 
                    this.Font.MeasureText(this.Sprite, this.Text, 0).Width);
            }
        }

        /// <summary>
        ///     Gets or sets the sprite.
        /// </summary>
        public Sprite Sprite
        {
            get
            {
                if (this.sprite != null && !this.sprite.IsDisposed)
                {
                    return this.sprite;
                }

                if (Drawing.Direct3DDevice != null && !Drawing.Direct3DDevice.IsDisposed)
                {
                    return this.sprite = new Sprite(Drawing.Direct3DDevice);
                }

                return null;
            }

            set
            {
                this.sprite = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sprite flags.
        /// </summary>
        public SpriteFlags SpriteFlags { get; set; }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(this.Font.MeasureText(this.Sprite, this.Text, 0).Width * this.Scale.X);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     The drawing event call.
        /// </summary>
        /// <param name="drawType">
        ///     The draw Type.
        /// </param>
        public override void Draw(DrawType drawType)
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed && this.Font != null && !this.Font.IsDisposed
                && this.IsVisible)
            {
                this.Sprite.Begin(this.SpriteFlags);

                if (this.CorpRectangle.HasValue)
                {
                    var vMatrixOriginal = this.Sprite.Transform;
                    this.Sprite.Transform = Matrix.Transformation2D(
                        Vector2.Zero, 
                        0f, 
                        this.Scale, 
                        this.Center, 
                        this.Rotation, 
                        this.Position);
                    this.Font.DrawText(this.Sprite, this.Text, this.CorpRectangle.Value, this.FontDrawFlags, this.Color);
                    this.Sprite.Transform = vMatrixOriginal;
                }
                else
                {
                    this.Font.DrawText(this.Sprite, this.Text, (int)this.Position.X, (int)this.Position.Y, this.Color);
                }

                this.Sprite.Flush();
                this.Sprite.End();
            }

            this.CallOnDraw();
        }

        /// <summary>
        ///     The OnLostDevice event.
        /// </summary>
        public override void OnPostReset()
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed)
            {
                this.Sprite.OnResetDevice();
            }

            if (this.Font != null && !this.Font.IsDisposed)
            {
                this.Font.OnResetDevice();
            }
        }

        /// <summary>
        ///     The OnPreReset event.
        /// </summary>
        public override void OnPreReset()
        {
            if (this.Sprite != null && !this.Sprite.IsDisposed)
            {
                this.Sprite.OnLostDevice();
            }

            if (this.Font != null && !this.Font.IsDisposed)
            {
                this.Font.OnLostDevice();
            }
        }

        /// <summary>
        ///     Transforms the regular text into a rotated text.
        /// </summary>
        /// <param name="degrees">
        ///     The rotation float value in degrees.
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <returns>
        ///     The <see cref="DrawText" />.
        /// </returns>
        public DrawText SetRotation(float degrees, Vector2? position = null)
        {
            if (degrees > 0f)
            {
                this.Rotation = (float)(degrees * 0.0174532925d);
                this.SpriteFlags = SpriteFlags.AlphaBlend;
                this.FontDrawFlags = FontDrawFlags.NoClip | FontDrawFlags.Left;
                var fontMeasure = this.Font.MeasureText(this.Sprite, this.Text, 0);
                this.Center = position ?? new Vector2(this.Position.X + fontMeasure.Width / 2f, this.Position.Y);
                this.Position = new Vector2(-this.Font.MeasureText(this.Sprite, this.Text, 0).Width / 2f, 0f);
                this.CorpRectangle = new Rectangle(
                    (int)this.Center.X, 
                    (int)this.Center.Y, 
                    fontMeasure.Width, 
                    fontMeasure.Height);
            }
            else
            {
                this.Rotation = 0f;
                this.SpriteFlags = 0;
                this.FontDrawFlags = 0;
                this.Center = Vector2.Zero;
                this.Position = position
                                ?? new Vector2(
                                       this.Position.X - this.Font.MeasureText(this.Sprite, this.Text, 0).Width / 2f, 
                                       this.Position.Y);
                this.CorpRectangle = null;
            }

            return this;
        }

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        public override void Update()
        {
            this.CallOnUpdate();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged and managed
        ///     resources.
        /// </summary>
        /// <param name="type">
        ///     The type of free, release or reset of resources.
        /// </param>
        private void Dispose(bool type)
        {
            if (type)
            {
                if (this.Font != null && !this.Font.IsDisposed)
                {
                    this.Font.Dispose();
                    this.Font = null;
                }

                if (this.Sprite != null && !this.Sprite.IsDisposed)
                {
                    this.Sprite.Dispose();
                    this.Sprite = null;
                }
            }
        }

        #endregion
    }
}