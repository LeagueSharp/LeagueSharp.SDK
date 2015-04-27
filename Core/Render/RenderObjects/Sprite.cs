using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render.RenderObjects
{
    /// <summary>
    ///     RenderObject class used to render a Sprite
    /// </summary>
    public class Sprite : Render.RenderObject
    {
        /// <summary>
        ///     Reset Delegate
        /// </summary>
        /// <param name="sprite"></param>
        public delegate void OnResetting(Sprite sprite);

        /// <summary>
        ///     Position Delegate
        /// </summary>
        public delegate Vector2 PositionDelegate();

        private readonly SharpDX.Direct3D9.Sprite _sprite = new SharpDX.Direct3D9.Sprite(Device);
        private ColorBGRA _color = SharpDX.Color.White;
        private SharpDX.Rectangle? _crop;
        private bool _hide;
        private Texture _originalTexture;
        private Vector2 _scale = new Vector2(1, 1);
        private Texture _texture;
        private int _x;
        private int _y;

        /// <summary>
        ///     Renders a Sprite
        /// </summary>
        /// <param name="bitmap">Bitmap of the Sprite</param>
        /// <param name="position">Position of the Sprite</param>
        public Sprite(Bitmap bitmap, Vector2 position)
        {
            UpdateTextureBitmap(bitmap, position);
        }

        /// <summary>
        ///     Renders a Sprite
        /// </summary>
        /// <param name="texture">Texture of the Sprite</param>
        /// <param name="position">Position of the Sprite</param>
        public Sprite(BaseTexture texture, Vector2 position)
        {
            UpdateTextureBitmap(
                (Bitmap)Image.FromStream(BaseTexture.ToStream(texture, ImageFileFormat.Bmp)), position);
        }

        /// <summary>
        ///     Renders a Sprite
        /// </summary>
        /// <param name="stream">Stream of the Sprite</param>
        /// <param name="position">Position of the Sprite</param>
        public Sprite(Stream stream, Vector2 position)
        {
            UpdateTextureBitmap(new Bitmap(stream), position);
        }

        /// <summary>
        ///     Renders a Sprite
        /// </summary>
        /// <param name="bytesArray">Byte Array of the Sprite</param>
        /// <param name="position">Position of the Sprite</param>
        public Sprite(byte[] bytesArray, Vector2 position)
        {
            UpdateTextureBitmap((Bitmap)Image.FromStream(new MemoryStream(bytesArray)), position);
        }

        /// <summary>
        ///     Renders a Sprite
        /// </summary>
        /// <param name="fileLocation">FileDirection of the Sprite </param>
        /// <param name="position">Position of the Sprite</param>
        public Sprite(string fileLocation, Vector2 position)
        {
            if (!File.Exists((fileLocation)))
            {
                return;
            }

            UpdateTextureBitmap(new Bitmap(fileLocation), position);
        }

        /// <summary>
        ///     Direct3DDevice
        /// </summary>
        public static Device Device
        {
            get { return Drawing.Direct3DDevice; }
        }

        /// <summary>
        ///     X Position of the Sprite
        /// </summary>
        public int X
        {
            get
            {
                if (PositionUpdate != null)
                {
                    return (int)PositionUpdate().X;
                }
                return _x;
            }
            set { _x = value; }
        }

        /// <summary>
        ///     Y Position of the Sprite
        /// </summary>
        public int Y
        {
            get
            {
                if (PositionUpdate != null)
                {
                    return (int)PositionUpdate().Y;
                }
                return _y;
            }
            set { _y = value; }
        }

        /// <summary>
        ///     BitMap of the Sprite
        /// </summary>
        public Bitmap Bitmap { get; set; }

        /// <summary>
        ///     Width of the Sprite
        /// </summary>
        public int Width
        {
            get { return (int)(Bitmap.Width * _scale.X); }
        }

        /// <summary>
        ///     Height of the Sprite
        /// </summary>
        public int Height
        {
            get { return (int)(Bitmap.Height * _scale.Y); }
        }

        /// <summary>
        ///     Size of the Sprite
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(Bitmap.Width, Bitmap.Height); }
        }

        /// <summary>
        ///     Position of the Sprite
        /// </summary>
        public Vector2 Position
        {
            set
            {
                X = (int)value.X;
                Y = (int)value.Y;
            }

            get { return new Vector2(X, Y); }
        }

        /// <summary>
        ///     Position Delegate
        /// </summary>
        public PositionDelegate PositionUpdate { get; set; }

        /// <summary>
        ///     Scale of the Sprite as Vector2
        /// </summary>
        public Vector2 Scale
        {
            set { _scale = value; }
            get { return _scale; }
        }

        /// <summary>
        ///     Rotation of the Sprite
        /// </summary>
        public float Rotation { set; get; }

        /// <summary>
        ///     Color of the Sprite
        /// </summary>
        public ColorBGRA Color
        {
            set { _color = value; }
            get { return _color; }
        }

        /// <summary>
        ///     OnReset Event
        /// </summary>
        public event OnResetting OnReset;

        /// <summary>
        ///     Renders a Crop
        /// </summary>
        /// <param name="x">X Position of the Crop</param>
        /// <param name="y">Y Position of the Crop</param>
        /// <param name="w">Width of the Crop</param>
        /// <param name="h">Height of the Crop</param>
        /// <param name="scale">Scale of the Crop</param>
        public void Crop(int x, int y, int w, int h, bool scale = false)
        {
            _crop = new SharpDX.Rectangle(x, y, w, h);

            if (scale)
            {
                _crop = new SharpDX.Rectangle(
                    (int)(_scale.X * x), (int)(_scale.Y * y), (int)(_scale.X * w), (int)(_scale.Y * h));
            }
        }

        /// <summary>
        ///     Rendera a Crop
        /// </summary>
        /// <param name="rect">RectAngle of the Crop</param>
        /// <param name="scale">Scale of the Crop</param>
        public void Crop(SharpDX.Rectangle rect, bool scale = false)
        {
            _crop = rect;

            if (scale)
            {
                _crop = new SharpDX.Rectangle(
                    (int)(_scale.X * rect.X), (int)(_scale.Y * rect.Y), (int)(_scale.X * rect.Width),
                    (int)(_scale.Y * rect.Height));
            }
        }

        /// <summary>
        ///     Shows the Sprite
        /// </summary>
        public void Show()
        {
            _hide = false;
        }

        /// <summary>
        ///     Hides the Sprite
        /// </summary>
        public void Hide()
        {
            _hide = true;
        }

        /// <summary>
        ///     Resets the Sprite
        /// </summary>
        public void Reset()
        {
            UpdateTextureBitmap(
                (Bitmap)Image.FromStream(BaseTexture.ToStream(_originalTexture, ImageFileFormat.Bmp)));

            if (OnReset != null)
            {
                OnReset(this);
            }
        }

        /// <summary>
        ///     GrayScales the Sprite
        /// </summary>
        public void GrayScale()
        {
            SetSaturation(0.0f);
        }

        /// <summary>
        ///     Fades the Sprite
        /// </summary>
        public void Fade()
        {
            SetSaturation(0.5f);
        }

        /// <summary>
        ///     Complements the Sprite
        /// </summary>
        public void Complement()
        {
            SetSaturation(-1.0f);
        }

        /// <summary>
        ///     Sets a saturation
        /// </summary>
        /// <param name="saturiation">Saturation of the Crop</param>
        public void SetSaturation(float saturiation)
        {
            UpdateTextureBitmap(SaturateBitmap(Bitmap, saturiation));
        }

        private static Bitmap SaturateBitmap(Image original, float saturation)
        {
            const float rWeight = 0.3086f;
            const float gWeight = 0.6094f;
            const float bWeight = 0.0820f;

            var a = (1.0f - saturation) * rWeight + saturation;
            var b = (1.0f - saturation) * rWeight;
            var c = (1.0f - saturation) * rWeight;
            var d = (1.0f - saturation) * gWeight;
            var e = (1.0f - saturation) * gWeight + saturation;
            var f = (1.0f - saturation) * gWeight;
            var g = (1.0f - saturation) * bWeight;
            var h = (1.0f - saturation) * bWeight;
            var i = (1.0f - saturation) * bWeight + saturation;

            var newBitmap = new Bitmap(original.Width, original.Height);
            var gr = Graphics.FromImage(newBitmap);

            // ColorMatrix elements
            float[][] ptsArray =
            {
                new[] {a, b, c, 0, 0}, new[] {d, e, f, 0, 0}, new[] {g, h, i, 0, 0},
                new float[] {0, 0, 0, 1, 0}, new float[] {0, 0, 0, 0, 1}
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
                original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width,
                original.Height, GraphicsUnit.Pixel, imgAttribs);
            gr.Dispose();

            return newBitmap;
        }

        /// <summary>
        ///     Updates the BitMap of the Sprite
        /// </summary>
        /// <param name="newBitmap">New BitMap of the Sprite</param>
        /// <param name="position">Position of the new Sprite</param>
        public void UpdateTextureBitmap(Bitmap newBitmap, Vector2 position = new Vector2())
        {
            if (position.IsValid())
            {
                Position = position;
            }

            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }
            Bitmap = newBitmap;

            _texture = Texture.FromMemory(
                Device, (byte[])new ImageConverter().ConvertTo(newBitmap, typeof(byte[])), Width, Height, 0,
                Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

            if (_originalTexture == null)
            {
                _originalTexture = _texture;
            }
        }

        /// <summary>
        ///     OnEndScene Event
        /// </summary>
        public override void OnEndScene()
        {
            try
            {
                if (_sprite.IsDisposed || _texture.IsDisposed || !Position.IsValid() || _hide)
                {
                    return;
                }

                _sprite.Begin();
                var matrix = _sprite.Transform;
                var nMatrix = (Matrix.Scaling(Scale.X, Scale.Y, 0)) * Matrix.RotationZ(Rotation) *
                              Matrix.Translation(Position.X, Position.Y, 0);
                _sprite.Transform = nMatrix;
                _sprite.Draw(_texture, _color, _crop);
                _sprite.Transform = matrix;
                _sprite.End();
            }
            catch (Exception e)
            {
                Reset();
                Console.WriteLine(@"Common.Render.Sprite.OnEndScene: " + e);
            }
        }

        /// <summary>
        ///     OnPreReset Event
        /// </summary>
        public override void OnPreReset()
        {
            _sprite.OnLostDevice();
        }

        /// <summary>
        ///     OnPostReset Event
        /// </summary>
        public override void OnPostReset()
        {
            _sprite.OnResetDevice();
        }

        /// <summary>
        ///     Dispose Event
        /// </summary>
        public override void Dispose()
        {
            if (!_sprite.IsDisposed)
            {
                _sprite.Dispose();
            }

            if (!_texture.IsDisposed)
            {
                _texture.Dispose();
            }

            if (!_originalTexture.IsDisposed)
            {
                _originalTexture.Dispose();
            }
        }
    }
}   