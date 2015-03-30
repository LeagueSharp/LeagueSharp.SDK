using System.Collections.Generic;
using System.Drawing;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;
using Rectangle = SharpDX.Rectangle;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Sprite, two-dimensional image or animation that is integrated into a larger scene by draw processing.
    /// </summary>
    public class MenuSprite
    {
        private readonly Dictionary<int, Container> _contents = new Dictionary<int, Container>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="bitmap">Sprite Bitmap</param>
        /// <param name="scale">Sprite Scale</param>
        /// <param name="color">Sprite Color</param>
        public MenuSprite(Bitmap bitmap, Vector2? scale = null, Color? color = null)
        {
            _contents.Add(
                0,
                new Container
                {
                    Bitmap = bitmap,
                    Texture =
                        Texture.FromMemory(
                            Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(bitmap, typeof(byte[])),
                            bitmap.Width, bitmap.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default,
                            Filter.Default, 0)
                });
            Color = color ?? new ColorBGRA(255, 255, 255, 255);
            Scale = scale ?? new Vector2(1, 1);
            IsGlowing = false;
        }

        /// <summary>
        ///     Returns if the sprite is glowing.
        /// </summary>
        public bool IsGlowing { get; set; }

        /// <summary>
        ///     Sprite Color
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Sprite Scale
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        ///     Sprite Rotation
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        ///     Sprite Position
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Current Sprite Width
        /// </summary>
        public int Width
        {
            get { return (int) (_contents[!IsGlowing ? 0 : 1].Bitmap.Width * Scale.X); }
        }

        /// <summary>
        ///     Current Sprite Height
        /// </summary>
        public int Height
        {
            get { return (int) (_contents[!IsGlowing ? 0 : 1].Bitmap.Height * Scale.Y); }
        }

        /// <summary>
        ///     Current Sprite Size
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return new Vector2(_contents[!IsGlowing ? 0 : 1].Bitmap.Width, _contents[!IsGlowing ? 0 : 1].Bitmap.Height);
            }
        }

        /// <summary>
        ///     Boundaries of the sprite.
        /// </summary>
        public Rectangle Boundaries
        {
            get { return new Rectangle((int) Position.X, (int) Position.Y, Width, Height); }
        }

        /// <summary>
        ///     Adds a secondary sprite to switch.
        /// </summary>
        /// <param name="bitmap">Sprite Bitmap</param>
        /// <returns>Instance</returns>
        public MenuSprite AddGlow(Bitmap bitmap)
        {
            _contents.Add(
                1,
                new Container
                {
                    Bitmap = bitmap,
                    Texture =
                        Texture.FromMemory(
                            Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(bitmap, typeof(byte[])),
                            bitmap.Width, bitmap.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default,
                            Filter.Default, 0)
                });
            return this;
        }

        /// <summary>
        ///     Adds a secondary sprite to switch.
        /// </summary>
        /// <returns>Instance</returns>
        public MenuSprite RemoveGlow()
        {
            if (_contents.ContainsKey(1))
            {
                _contents.Remove(1);
            }
            return this;
        }

        /// <summary>
        ///     Changes the glow of the sprite.
        /// </summary>
        /// <param name="bitmap">Bitmap Glow</param>
        /// <returns>Instance</returns>
        public MenuSprite ChangeGlow(Bitmap bitmap)
        {
            if (_contents.ContainsKey(1))
            {
                _contents.Remove(1);
                _contents.Add(
                    1,
                    new Container
                    {
                        Bitmap = bitmap,
                        Texture =
                            Texture.FromMemory(
                                Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(bitmap, typeof(byte[])),
                                bitmap.Width, bitmap.Height, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default,
                                Filter.Default, 0)
                    });
            }
            return this;
        }

        /// <summary>
        ///     Will attempt to set the object to glowing state if it passes the seires of checks.
        /// </summary>
        /// <param name="cursorVector2">Cursor Position</param>
        /// <returns>If glow attempt was successful</returns>
        public bool TryGlow(Vector2 cursorVector2)
        {
            if (_contents.Count > 1)
            {
                if (cursorVector2.IsUnderRectangle(Boundaries.X, Boundaries.Y, Boundaries.Width, Boundaries.Height))
                {
                    return IsGlowing = true;
                }
            }
            return IsGlowing = false;
        }

        /// <summary>
        ///     Calls the drawing of the sprite.
        /// </summary>
        /// <param name="sprite">Sprite handle to draw on</param>
        public void Draw(Sprite sprite)
        {
            var matrix = sprite.Transform;
            var nMatrix = (Matrix.Scaling(Scale.X, Scale.Y, 0)) * Matrix.RotationZ(Rotation) *
                          Matrix.Translation(Position.X, Position.Y + 5f, 0);
            sprite.Transform = nMatrix;
            sprite.Draw(_contents[!IsGlowing ? 0 : 1].Texture, Color);
            sprite.Transform = matrix;
        }

        /// <summary>
        ///     Calls the drawing of the sprite.
        /// </summary>
        /// <param name="sprite">Sprite handle to draw on</param>
        /// <param name="positionY">Y-Axis Position</param>
        public void Draw(Sprite sprite, int positionY)
        {
            var matrix = sprite.Transform;
            var nMatrix = (Matrix.Scaling(Scale.X, Scale.Y, 0)) * Matrix.RotationZ(Rotation) *
                          Matrix.Translation(Position.X, positionY + 5f, 0);
            sprite.Transform = nMatrix;
            sprite.Draw(_contents[!IsGlowing ? 0 : 1].Texture, Color);
            sprite.Transform = matrix;
        }

        private struct Container
        {
            /// <summary>
            ///     Bitmap to contain.
            /// </summary>
            public Bitmap Bitmap;

            /// <summary>
            ///     Texture to contain.
            /// </summary>
            public Texture Texture;
        }
    }
}