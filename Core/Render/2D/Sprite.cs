using System;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;

namespace LeagueSharp.CommonEx.Core.Render._2D
{
    /// <summary>
    ///     Sprite class which supports both <see cref="SharpDX.Direct3D9.Texture" /> and <see cref="Bitmap" /> for direct
    ///     drawing onto a projection, also utilizes a handfull of functions for easy texture manipulation, bitmaps are there
    ///     to serve for any reverses as the original mapping of the picture.
    /// </summary>
    public class Sprite : IDisposable, IComparable, ICloneable
    {
        /// <summary>
        ///     Mapping of a picture in a range of bits.
        /// </summary>
        private Bitmap _bitmap;

        /// <summary>
        ///     Initializes a new <see cref="Sprite" /> class.
        /// </summary>
        public Sprite() {}

        /// <summary>
        ///     Initializes a new <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="bitmap">Bitmap</param>
        public Sprite(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        /// <summary>
        ///     Initializes a new <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="scale">Scale</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="color">Color</param>
        public Sprite(Texture texture, Vector2? scale, float rotation, Color? color)
        {
            Texture = texture;
            _bitmap = (Bitmap) Image.FromStream(BaseTexture.ToStream(texture, ImageFileFormat.Bmp));
            Scale = scale ?? new Vector2(1, 1);
            Rotation = rotation;
            Color = color ?? Color.White;
        }

        /// <summary>
        ///     Internally intializes a new <see cref="Sprite" /> class, used for clone-ability.
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="bitmap">Bitmap</param>
        /// <param name="scale">Scale</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="color">Color</param>
        internal Sprite(Texture texture, Bitmap bitmap, Vector2 scale, float rotation, Color color)
        {
            Texture = texture;
            _bitmap = bitmap;
            Scale = scale;
            Rotation = rotation;
            Color = color;
        }

        /// <summary>
        ///     <see cref="Texture" /> class which is created from a memory buffer.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        ///     Size value of the sprite.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        ///     Rotation value in float degrees of the sprite.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        ///     Color value of the sprite.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Mapping of a picture in a range of bits, creates a <see cref="Texture" /> from a memory buffer once replaced.
        /// </summary>
        /// <unmanaged>
        ///     HRESULT D3DXCreateTextureFromFileInMemory([In] IDirect3DDevice9* pDevice,[In] const void* pSrcData,[In]
        ///     unsigned int SrcDataSize,[In] IDirect3DTexture9** ppTexture)
        /// </unmanaged>
        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                if (value != null)
                {
                    Texture = Texture.FromMemory(
                        Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(value, typeof(byte[])),
                        value.Width, value.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default,
                        Filter.Default, 0);
                }
            }
        }

        /// <summary>
        ///     Width component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public int Width
        {
            get { return (int) (Bitmap.Width * Scale.X); }
        }

        /// <summary>
        ///     Height component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public int Height
        {
            get { return (int) (Bitmap.Height * Scale.Y); }
        }

        /// <summary>
        ///     Size component of the <see cref="Bitmap" /> currently stored in the <see cref="Sprite" /> instance.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(Bitmap.Width, Bitmap.Height); }
        }

        /// <summary>
        ///     Clones the <see cref="Sprite" /> class instance, and returns it as a general object.
        /// </summary>
        /// <returns>Object type of <see cref="Sprite" /></returns>
        public object Clone()
        {
            return new Sprite(Texture, Bitmap, Scale, Rotation, Color);
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
                return sprite.Bitmap == Bitmap && sprite.Texture == Texture ? 1 : 0;
            }

            var texture = obj as Texture;
            if (texture != null)
            {
                return texture == Texture ? 1 : 0;
            }

            var bitmap = obj as Bitmap;
            if (bitmap != null)
            {
                return bitmap == Bitmap ? 1 : 0;
            }

            return -1;
        }

        /// <summary>
        ///     Disposes the <see cref="Sprite" /> class safely.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Draws the sprite based on a sprite-sheet at the given position, method must be called inside a BeginScene ...
        ///     EndScene block.
        /// </summary>
        /// <param name="sprite">Sprite sheet</param>
        /// <param name="position">Position</param>
        public void Draw(SharpDX.Direct3D9.Sprite sprite, Vector2 position)
        {
            if (sprite == null)
            {
                return;
            }

            var matrix = sprite.Transform;
            var nMatrix = (Matrix.Scaling(Scale.X, Scale.Y, 0)) * Matrix.RotationZ(Rotation) *
                          Matrix.Translation(position.X, position.Y, 0);
            sprite.Transform = nMatrix;
            sprite.Draw(Texture, Color);
            sprite.Transform = matrix;
        }

        /// <summary>
        ///     Safe dispose of the sprite in both of finalization mode and dispose in fastcall.
        /// </summary>
        /// <param name="value"></param>
        private void Dispose(bool value)
        {
            if (Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }
            if (value)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }
        }

        /// <summary>
        ///     Finalization of the class, thrown by GC when forgotten in memory.
        /// </summary>
        ~Sprite()
        {
            Dispose(false);
        }
    }
}