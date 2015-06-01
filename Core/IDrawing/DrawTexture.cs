// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawTexture.cs" company="LeagueSharp">
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
//   The DrawTexture class, contains a set of tool to simply draw pre-defined textures and a simple texture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.IDrawing
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Matrix = SharpDX.Matrix;
    using Rectangle = SharpDX.Rectangle;

    /// <summary>
    ///     The DrawTexture class, contains a set of tool to simply draw pre-defined textures and a simple texture.
    /// </summary>
    public class DrawTexture : DrawObject, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The texture height.
        /// </summary>
        private int height;

        /// <summary>
        ///     The texture sprite.
        /// </summary>
        private Sprite sprite;

        /// <summary>
        ///     The texture width.
        /// </summary>
        private int width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(Vector2? position = null)
            : base(position ?? new Vector2())
        {
            this.Scale = new Vector2(1f, 1f);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="bitmap">
        ///     The bitmap
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(Bitmap bitmap, Vector2? position = null)
            : this(position)
        {
            if (bitmap != null)
            {
                this.Bitmap = bitmap;
                this.Texture = Texture.FromMemory(
                    Drawing.Direct3DDevice, 
                    (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])), 
                    bitmap.Width, 
                    bitmap.Height, 
                    0, 
                    Usage.None, 
                    Format.A1, 
                    Pool.Managed, 
                    Filter.Default, 
                    Filter.Default, 
                    0);
                this.width = bitmap.Width;
                this.height = bitmap.Height;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="bytes">
        ///     The bytes
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(byte[] bytes, Vector2? position = null)
            : this((Bitmap)Image.FromStream(new MemoryStream(bytes)), position)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="stream">
        ///     The stream
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(Stream stream, Vector2? position = null)
            : this((Bitmap)Image.FromStream(stream), position)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The texture
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        /// <param name="format">
        ///     The texture image format.
        /// </param>
        public DrawTexture(Texture texture, Vector2? position = null, ImageFileFormat format = ImageFileFormat.Bmp)
            : this(position)
        {
            this.Texture = texture;
            this.Bitmap = Image.FromStream(BaseTexture.ToStream(texture, format)) as Bitmap;
            if (this.Bitmap != null)
            {
                this.width = this.Bitmap.Width;
                this.height = this.Bitmap.Height;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The texture
        /// </param>
        /// <param name="width">
        ///     The width
        /// </param>
        /// <param name="height">
        ///     The height
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(Texture texture, int width, int height, Vector2? position = null)
            : this(position)
        {
            this.Texture = texture;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        /// <param name="fileLocation">
        ///     The file location
        /// </param>
        /// <param name="position">
        ///     The position
        /// </param>
        public DrawTexture(string fileLocation, Vector2? position = null)
            : this(File.Exists(fileLocation) ? new Bitmap(fileLocation) : null, position)
        {
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DrawTexture" /> class.
        /// </summary>
        ~DrawTexture()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the bitmap.
        /// </summary>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        ///     Gets or sets the corp rectangle.
        /// </summary>
        public Rectangle? CorpRectangle { get; set; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(this.height * this.Scale.Y);
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
                return new Vector2(this.width, this.height);
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
        ///     Gets or sets the texture.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(this.width * this.Scale.X);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Combines an external image to the bitmap.
        /// </summary>
        /// <param name="image">
        ///     The image
        /// </param>
        /// <param name="x">
        ///     The x axis
        /// </param>
        /// <param name="y">
        ///     The y axis
        /// </param>
        public void Combine(Image image, float x, float y)
        {
            var bitmap = new Bitmap(this.Bitmap);
            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(image, x, y);
                canvas.Save();
            }

            this.Bitmap = bitmap;
            this.Texture = Texture.FromMemory(
                Drawing.Direct3DDevice, 
                (byte[])new ImageConverter().ConvertTo(this.Bitmap, typeof(byte[])), 
                this.Bitmap.Width, 
                this.Bitmap.Height, 
                0, 
                Usage.None, 
                Format.A1, 
                Pool.Managed, 
                Filter.Default, 
                Filter.Default, 
                0);
        }

        /// <summary>
        ///     Combines an external image to the bitmap.
        /// </summary>
        /// <param name="image">
        ///     The image
        /// </param>
        /// <param name="bitmapX">
        ///     The bitmap x axis
        /// </param>
        /// <param name="bitmapY">
        ///     The bitmap y axis
        /// </param>
        /// <param name="imageX">
        ///     The image x axis
        /// </param>
        /// <param name="imageY">
        ///     The image y axis
        /// </param>
        /// <param name="imageWidth">
        ///     The image width
        /// </param>
        /// <param name="imageHeight">
        ///     The image height
        /// </param>
        public void Combine(
            Image image, 
            float bitmapX, 
            float bitmapY, 
            float imageX, 
            float imageY, 
            int imageWidth, 
            int imageHeight)
        {
            var bitmap = new Bitmap(this.width, this.height);
            using (var canvas = Graphics.FromImage(bitmap))
            {
                canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                canvas.DrawImage(bitmap, bitmapX, bitmapY);
                canvas.DrawImage(image, imageX, imageY);
                canvas.Save();
            }

            this.Bitmap = bitmap;
            this.Texture = Texture.FromMemory(
                Drawing.Direct3DDevice, 
                (byte[])new ImageConverter().ConvertTo(this.Bitmap, typeof(byte[])), 
                this.Bitmap.Width, 
                this.Bitmap.Height, 
                0, 
                Usage.None, 
                Format.A1, 
                Pool.Managed, 
                Filter.Default, 
                Filter.Default, 
                0);
            this.width = imageWidth;
            this.height = imageHeight;
        }

        /// <summary>
        ///     Sets the saturation to complement.
        /// </summary>
        public void Complement()
        {
            this.SetSaturation(-1.0f);
        }

        /// <summary>
        ///     Creates and sets a corp towards the texture.
        /// </summary>
        /// <param name="x">
        ///     The x axis
        /// </param>
        /// <param name="y">
        ///     The y axis
        /// </param>
        /// <param name="w">
        ///     The width
        /// </param>
        /// <param name="h">
        ///     The height
        /// </param>
        /// <param name="scale">
        ///     The scale
        /// </param>
        public void Crop(int x, int y, int w, int h, bool scale = false)
        {
            this.CorpRectangle = new Rectangle(x, y, w, h);

            if (scale)
            {
                this.CorpRectangle = new Rectangle(
                    (int)(this.Scale.X * x), 
                    (int)(this.Scale.Y * y), 
                    (int)(this.Scale.X * w), 
                    (int)(this.Scale.Y * h));
            }
        }

        /// <summary>
        ///     Creates and sets a corp towards the texture.
        /// </summary>
        /// <param name="rectangle">
        ///     The rectangle
        /// </param>
        /// <param name="scale">
        ///     The scale
        /// </param>
        public void Crop(Rectangle rectangle, bool scale = false)
        {
            this.CorpRectangle = rectangle;

            if (scale)
            {
                this.CorpRectangle = new Rectangle(
                    (int)(this.Scale.X * rectangle.X), 
                    (int)(this.Scale.Y * rectangle.Y), 
                    (int)(this.Scale.X * rectangle.Width), 
                    (int)(this.Scale.Y * rectangle.Height));
            }
        }

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
            if (this.Sprite != null && !this.Sprite.IsDisposed && this.Texture != null && !this.Texture.IsDisposed
                && this.IsVisible)
            {
                var matrix = this.Sprite.Transform;
                this.Sprite.Begin(this.SpriteFlags);

                this.Sprite.Transform = Matrix.Scaling(this.Scale.X, this.Scale.Y, 0) * Matrix.RotationZ(this.Rotation)
                                        * Matrix.Translation(this.Position.X, this.Position.Y, 0);
                this.Sprite.Draw(this.Texture, this.Color, this.CorpRectangle);
                this.Sprite.Transform = matrix;

                this.sprite.Flush();
                this.Sprite.End();
            }
        }

        /// <summary>
        ///     Sets the saturation to fade.
        /// </summary>
        public void Fade()
        {
            this.SetSaturation(0.5f);
        }

        /// <summary>
        ///     Sets the saturation to gray scale.
        /// </summary>
        public void GrayScale()
        {
            this.SetSaturation(0.0f);
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
        }

        /// <summary>
        ///     Set the current bitmap saturation
        /// </summary>
        /// <param name="saturation">
        ///     The saturation value
        /// </param>
        public void SetSaturation(float saturation)
        {
            this.Bitmap = SaturateBitmap(this.Bitmap, saturation);
            this.Texture = Texture.FromMemory(
                Drawing.Direct3DDevice, 
                (byte[])new ImageConverter().ConvertTo(this.Bitmap, typeof(byte[])), 
                this.Bitmap.Width, 
                this.Bitmap.Height, 
                0, 
                Usage.None, 
                Format.A1, 
                Pool.Managed, 
                Filter.Default, 
                Filter.Default, 
                0);
            this.width = this.Bitmap.Width;
            this.height = this.Bitmap.Height;
        }

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        public override void Update()
        {
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
                if (this.Texture != null && !this.Texture.IsDisposed)
                {
                    this.Texture.Dispose();
                    this.Texture = null;
                }

                if (this.Sprite != null && !this.Sprite.IsDisposed)
                {
                    this.Sprite.Dispose();
                    this.Sprite = null;
                }

                if (this.Bitmap != null)
                {
                    this.Bitmap.Dispose();
                    this.Bitmap = null;
                }
            }
        }

        /// <summary>
        ///     Saturates the bitmap.
        /// </summary>
        /// <param name="original">
        ///     The original image
        /// </param>
        /// <param name="saturation">
        ///     The saturation value
        /// </param>
        /// <returns>
        ///     The saturated bitmap.
        /// </returns>
        private static Bitmap SaturateBitmap(Image original, float saturation)
        {
            const float RWeight = 0.3086f;
            const float GWeight = 0.6094f;
            const float BWeight = 0.0820f;

            var a = (1.0f - saturation) * RWeight + saturation;
            var b = (1.0f - saturation) * RWeight;
            var c = (1.0f - saturation) * RWeight;
            var d = (1.0f - saturation) * GWeight;
            var e = (1.0f - saturation) * GWeight + saturation;
            var f = (1.0f - saturation) * GWeight;
            var g = (1.0f - saturation) * BWeight;
            var h = (1.0f - saturation) * BWeight;
            var i = (1.0f - saturation) * BWeight + saturation;

            var newBitmap = new Bitmap(original.Width, original.Height);
            var gr = Graphics.FromImage(newBitmap);

            // ColorMatrix elements
            float[][] ptsArray =
                {
                    new[] { a, b, c, 0, 0 }, new[] { d, e, f, 0, 0 }, new[] { g, h, i, 0, 0 }, 
                    new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 }
                };

            // Create ColorMatrix
            var clrMatrix = new ColorMatrix(ptsArray);

            // Create ImageAttributes
            var imgAttribs = new ImageAttributes();

            // Set color matrix
            imgAttribs.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

            // Draw Image with no effects
            gr.DrawImage(original, 0, 0, original.Width, original.Height);

            // Draw Image with image attributes
            gr.DrawImage(
                original, 
                new System.Drawing.Rectangle(0, 0, original.Width, original.Height), 
                0, 
                0, 
                original.Width, 
                original.Height, 
                GraphicsUnit.Pixel, 
                imgAttribs);
            gr.Dispose();

            return newBitmap;
        }

        #endregion
    }
}