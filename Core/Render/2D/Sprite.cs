// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sprite.cs" company="LeagueSharp">
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
//   Sprite class which supports both <see cref="SharpDX.Direct3D9.Texture" /> and <see cref="Bitmap" /> for direct
//   drawing onto a projection, also utilizes a handful of functions for easy texture manipulation, bitmaps are there
//   to serve for any reverses as the original mapping of the picture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Render._2D
{
    using System;
    using System.Drawing;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;

    /// <summary>
    ///     Sprite class which supports both <see cref="SharpDX.Direct3D9.Texture" /> and <see cref="Bitmap" /> for direct
    ///     drawing onto a projection, also utilizes a handful of functions for easy texture manipulation, bitmaps are there
    ///     to serve for any reverses as the original mapping of the picture.
    /// </summary>
    public class Sprite : IDisposable, IComparable, ICloneable
    {
        #region Fields

        /// <summary>
        ///     Mapping of a picture in a range of bits.
        /// </summary>
        private Bitmap bitmap;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sprite" /> class.
        ///     Initializes a new <see cref="Sprite" /> class.
        /// </summary>
        public Sprite()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sprite" /> class.
        ///     Initializes a new <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="bitmap">
        ///     The Bitmap
        /// </param>
        public Sprite(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The Texture
        /// </param>
        /// <param name="scale">
        ///     The Scale
        /// </param>
        /// <param name="rotation">
        ///     The Rotation
        /// </param>
        /// <param name="color">
        ///     The Color
        /// </param>
        public Sprite(Texture texture, Vector2? scale, float rotation, Color? color)
        {
            this.Texture = texture;
            this.bitmap = (Bitmap)Image.FromStream(BaseTexture.ToStream(texture, ImageFileFormat.Bmp));
            this.Scale = scale ?? new Vector2(1, 1);
            this.Rotation = rotation;
            this.Color = color ?? Color.White;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="texture">
        ///     The Texture
        /// </param>
        /// <param name="bitmap">
        ///     The Bitmap
        /// </param>
        /// <param name="scale">
        ///     The Scale
        /// </param>
        /// <param name="rotation">
        ///     The Rotation
        /// </param>
        /// <param name="color">
        ///     The Color
        /// </param>
        internal Sprite(Texture texture, Bitmap bitmap, Vector2 scale, float rotation, Color color)
        {
            this.Texture = texture;
            this.bitmap = bitmap;
            this.Scale = scale;
            this.Rotation = rotation;
            this.Color = color;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Sprite" /> class.
        /// </summary>
        ~Sprite()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the mapping of a picture in a range of bits, creates a <see cref="Texture" /> from a memory buffer
        ///     once replaced.
        /// </summary>
        /// <unmanaged>
        ///     HRESULT D3DXCreateTextureFromFileInMemory([In] IDirect3DDevice9* pDevice,[In] const void* pSrcData,[In]
        ///     unsigned int SrcDataSize,[In] IDirect3DTexture9** ppTexture)
        /// </unmanaged>
        public Bitmap Bitmap
        {
            get
            {
                return this.bitmap;
            }

            set
            {
                this.bitmap = value;
                if (value != null)
                {
                    this.Texture = Texture.FromMemory(
                        Drawing.Direct3DDevice, 
                        (byte[])new ImageConverter().ConvertTo(value, typeof(byte[])), 
                        value.Width, 
                        value.Height, 
                        0, 
                        Usage.None, 
                        Format.A1, 
                        Pool.Managed, 
                        Filter.Default, 
                        Filter.Default, 
                        0);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the color value of the sprite.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Gets the height component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(this.Bitmap.Height * this.Scale.Y);
            }
        }

        /// <summary>
        ///     Gets or sets the rotation value in float degrees of the sprite.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        ///     Gets or sets the size value of the sprite.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        ///     Gets the size component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(this.Bitmap.Width, this.Bitmap.Height);
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="Texture" /> class which is created from a memory buffer.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        ///     Gets the width component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(this.Bitmap.Width * this.Scale.X);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clones the <see cref="Sprite" /> class instance, and returns it as a general object.
        /// </summary>
        /// <returns>Object type of <see cref="Sprite" /></returns>
        public object Clone()
        {
            return new Sprite(this.Texture, this.Bitmap, this.Scale, this.Rotation, this.Color);
        }

        /// <summary>
        ///     Compares the <see cref="Sprite" /> class to another given object from a different or same type.
        /// </summary>
        /// <param name="obj">Object to be compared to</param>
        /// <returns>
        ///     If function successfully compares between the two objects it will return a valid number of 1, if the compare
        ///     function is unsuccessful then a invalid number of 0 will be returned, if the function is unable to find a possible
        ///     comparable object it will return an invalid number of -1.
        /// </returns>
        public int CompareTo(object obj)
        {
            var sprite = obj as Sprite;
            if (sprite != null)
            {
                return sprite.Bitmap == this.Bitmap && sprite.Texture == this.Texture ? 1 : 0;
            }

            var texture = obj as Texture;
            if (texture != null)
            {
                return texture == this.Texture ? 1 : 0;
            }

            var bitmapObject = obj as Bitmap;
            if (bitmapObject != null)
            {
                return bitmapObject == this.Bitmap ? 1 : 0;
            }

            return -1;
        }

        /// <summary>
        ///     Disposes the <see cref="Sprite" /> class safely.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Draws the sprite based on a sprite-sheet at the given position, method must be called inside a BeginScene ...
        ///     EndScene block.
        /// </summary>
        /// <param name="sprite">Sprite sheet</param>
        /// <param name="position">The Position</param>
        public void Draw(SharpDX.Direct3D9.Sprite sprite, Vector2 position)
        {
            if (sprite == null)
            {
                return;
            }

            var matrix = sprite.Transform;
            var nMatrix = Matrix.Scaling(this.Scale.X, this.Scale.Y, 0) * Matrix.RotationZ(this.Rotation)
                          * Matrix.Translation(position.X, position.Y, 0);
            sprite.Transform = nMatrix;
            sprite.Draw(this.Texture, this.Color);
            sprite.Transform = matrix;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Safe dispose of the sprite in both of finalization mode and dispose in fast-call.
        /// </summary>
        /// <param name="value">Finalize indicator</param>
        private void Dispose(bool value)
        {
            if (this.Texture != null)
            {
                this.Texture.Dispose();
                this.Texture = null;
            }

            if (value)
            {
                this.Bitmap.Dispose();
                this.Bitmap = null;
            }
        }

        #endregion
    }
}