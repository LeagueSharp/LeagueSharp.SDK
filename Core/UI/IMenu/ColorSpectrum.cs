namespace LeagueSharp.SDK.UI
{
    using System;
    using System.Drawing;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;

    internal class ColorSpectrum
    {
    }

    /// <summary>
    ///     The ColorBox
    /// </summary>
    public class ColorBox
    {
        #region Fields

        /// <summary>
        ///     Height of the ColorBox
        /// </summary>
        private readonly int mHeight;

        /// <summary>
        ///     Width of the ColorBox
        /// </summary>
        private readonly int mWidth;

        /// <summary>
        ///     Colorbox disabled
        /// </summary>
        private bool mDisabled;

        /// <summary>
        ///     Is User dragging the slider
        /// </summary>
        private bool mDragging;

        /// <summary>
        ///     Defines the Display Style
        /// </summary>
        private EDrawStyle mEDrawStyle;

        /// <summary>
        ///     Hue, Saturation, Lightness
        /// </summary>
        private Hsl mHsl;

        /// <summary>
        ///     Marker Color
        /// </summary>
        private Color mMarkerColor;

        /// <summary>
        ///     Marker X Position
        /// </summary>
        private int mMarkerX;

        /// <summary>
        ///     Marker Y Position
        /// </summary>
        private int mMarkerY;

        /// <summary>
        ///     Position on the Screen
        /// </summary>
        private Vector2 mPos;

        /// <summary>
        ///     Reg, Green, Blue
        /// </summary>
        private Color mRgb;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Init the ColorBox
        /// </summary>
        /// <param name="size">The size of the new ColorBox</param>
        /// <param name="disabled"></param>
        public ColorBox(Size size, bool disabled = false)
        {
            this.mHsl = new Hsl { H = 1, S = 1, L = 1 };

            this.mRgb = Utilities.HslToRgb(this.mHsl);
            this.mEDrawStyle = EDrawStyle.Hue;

            this.mWidth = size.Width;
            this.mHeight = size.Height;

            this.mDisabled = disabled;

            this.DrawControl();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     EventHandler for the event <see cref="ColorBox.ColorBoxScrolled" />
        /// </summary>
        public delegate void ColorBoxScrolledEventHandler();

        #endregion

        #region Public Events

        /// <summary>
        ///     The event which gets fired when the color get changed
        /// </summary>
        public event ColorBoxScrolledEventHandler ColorBoxScrolled;

        #endregion

        #region Enums

        /// <summary>
        ///     The DrawStyle enum
        /// </summary>
        public enum EDrawStyle
        {
            /// <summary>
            ///     Hue Style
            /// </summary>
            Hue,

            /// <summary>
            ///     Saturation Style
            /// </summary>
            Saturation,

            /// <summary>
            ///     Brightness Style
            /// </summary>
            Brightness,

            /// <summary>
            ///     Red Style
            /// </summary>
            Red,

            /// <summary>
            ///     Green Style
            /// </summary>
            Green,

            /// <summary>
            ///     Blue Style
            /// </summary>
            Blue
        }

        /// <summary>
        ///     The Orientation enum for the Gradient to draw
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            ///     Horizontal Orientation
            /// </summary>
            Horizontal,

            /// <summary>
            ///     Vertical Orientation
            /// </summary>
            Vertical
        };

        #endregion

        #region Public Properties

        /// <summary>
        ///     The DrawStyle Property
        /// </summary>
        public EDrawStyle DrawStyle
        {
            get
            {
                return this.mEDrawStyle;
            }
            set
            {
                this.mEDrawStyle = value;

                this.ResetMarker(true);
            }
        }

        /// <summary>
        ///     Hue, Saturation, Lightness Property
        /// </summary>
        public Hsl Hsl
        {
            get
            {
                return this.mHsl;
            }
            set
            {
                this.mHsl = value;
                this.mRgb = Utilities.HslToRgb(this.mHsl);

                this.ResetMarker(true);
            }
        }

        /// <summary>
        ///     The MarkerColor Property
        /// </summary>
        public Color MarkerColor
        {
            get
            {
                return this.mMarkerColor;
            }
            set
            {
                this.mMarkerColor = value;
            }
        }

        /// <summary>
        ///     Red, Green, Blue Property
        /// </summary>
        public Color Rgb
        {
            get
            {
                return this.mRgb;
            }
            set
            {
                this.mRgb = value;
                this.mHsl = Utilities.RgbToHsl(this.mRgb);

                this.ResetMarker(true);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets fired when the left mouse button is pressed
        /// </summary>
        /// <param name="args">Keys</param>
        public void ColorBoxMouseDown(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.LBUTTONDOWN && !this.mDisabled)
            {
                this.mDragging = true;
                var x = (int)args.Cursor.X - 2 - (int)this.mPos.X;
                var y = (int)args.Cursor.Y - 2 - (int)this.mPos.Y;

                if (x < 0)
                {
                    x = 0;
                }
                if (x > (this.mWidth - 4))
                {
                    x = (this.mWidth - 4);
                }

                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 4))
                {
                    y = (this.mHeight - 4);
                }

                this.mMarkerX = x;
                this.mMarkerY = y;

                this.DrawMarker(x, y, true);
                // Redraw the marker
                this.ResetHslrgb();
                // Reset the color

                this.ColorBoxScrolled?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is moved and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void ColorBoxMouseMove(WindowsKeys args)
        {
            if (this.mDragging && args.Msg == WindowsMessages.MOUSEMOVE && !this.mDisabled)
            {
                var x = (int)args.Cursor.X - 2 - (int)this.mPos.X;
                var y = (int)args.Cursor.Y - 2 - (int)this.mPos.Y;

                if (x < 0)
                {
                    x = 0;
                }
                if (x > (this.mWidth - 4))
                {
                    x = (this.mWidth - 4);
                }

                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 4))
                {
                    y = (this.mHeight - 4);
                }

                this.mMarkerX = x;
                this.mMarkerY = y;

                this.DrawMarker(x, y, true);
                // Redraw the marker
                this.ResetHslrgb();
                // Reset the color

                this.ColorBoxScrolled?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is released and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void ColorBoxMouseUp(WindowsKeys args)
        {
            if (this.mDragging && args.Msg == WindowsMessages.LBUTTONUP && !this.mDisabled)
            {
                this.mDragging = false;
                var x = (int)args.Cursor.X - 2 - (int)this.mPos.X;
                var y = (int)args.Cursor.Y - 2 - (int)this.mPos.Y;

                if (x < 0)
                {
                    x = 0;
                }
                if (x > (this.mWidth - 4))
                {
                    x = (this.mWidth - 4);
                }

                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 4))
                {
                    y = (this.mHeight - 4);
                }

                this.mMarkerX = x;
                this.mMarkerY = y;

                this.DrawMarker(x, y, true);
                // Redraw the marker
                this.ResetHslrgb();
                // Reset the color

                this.ColorBoxScrolled?.Invoke();
            }
        }

        /// <summary>
        ///     Draws the ColorBox
        /// </summary>
        /// <param name="newPos">Sets a new Position</param>
        public void DrawControl(Vector2 newPos = new Vector2())
        {
            if (!newPos.IsZero)
            {
                this.mPos = newPos;
            }

            this.DrawBorder();
            this.DrawContent();
            this.DrawMarker(this.mMarkerX, this.mMarkerY, true);
        }

        /// <summary>
        ///     Draws a Gradient.
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="startCol">Starting Color</param>
        /// <param name="endCol">Ending Color</param>
        /// <param name="orientation">Orientation</param>
        public void GradientRect(
            float x,
            float y,
            float width,
            float height,
            ColorBGRA startCol,
            ColorBGRA endCol,
            Orientation orientation)
        {
            var vertices = new VertexBuffer(
                Drawing.Direct3DDevice,
                SharpDX.Utilities.SizeOf<Vector4>() * 2 * 4,
                Usage.WriteOnly,
                VertexFormat.Diffuse | VertexFormat.PositionRhw,
                Pool.Default);

            vertices.Lock(0, 0, LockFlags.None)
                .WriteRange(
                    new[]
                        {
                            new Vector4(x, y, 0f, 1.0f), startCol.ToVector4(), new Vector4(x + width, y, 0f, 1.0f),
                            orientation == Orientation.Vertical ? endCol.ToVector4() : startCol.ToVector4(),
                            new Vector4(x, y + height, 0f, 1.0f),
                            orientation == Orientation.Vertical ? startCol.ToVector4() : endCol.ToVector4(),
                            new Vector4(x + width, y + height, 0f, 1.0f), endCol.ToVector4()
                        });
            vertices.Unlock();

            VertexElement[] vertexElements =
                {
                    new VertexElement(
                        0,
                        0,
                        DeclarationType.Float4,
                        DeclarationMethod.Default,
                        DeclarationUsage.Position,
                        0),
                    new VertexElement(
                        0,
                        16,
                        DeclarationType.Float4,
                        DeclarationMethod.Default,
                        DeclarationUsage.Color,
                        0),
                    VertexElement.VertexDeclarationEnd
                };

            var vertexDeclaration = new VertexDeclaration(Drawing.Direct3DDevice, vertexElements);

            var olddec = Drawing.Direct3DDevice.VertexDeclaration;
            Drawing.Direct3DDevice.SetStreamSource(0, vertices, 0, SharpDX.Utilities.SizeOf<Vector4>() * 2);
            Drawing.Direct3DDevice.VertexDeclaration = vertexDeclaration;
            Drawing.Direct3DDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            Drawing.Direct3DDevice.VertexDeclaration = olddec;

            vertexDeclaration.Dispose();
            vertices.Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Draws the Border around the ColorBox
        /// </summary>
        private void DrawBorder()
        {
            ColorBGRA color = Color.FromArgb(37, 37, 37).ToSharpDxColor();

            Utils.DrawLine(this.mPos.X - 1, this.mPos.Y - 1, this.mPos.X + this.mWidth - 4, this.mPos.Y - 1, 1, color);
            //Top
            Utils.DrawLine(this.mPos.X - 1, this.mPos.Y - 1, this.mPos.X - 1, this.mPos.Y + this.mHeight - 1, 1, color);
            //Left
            Utils.DrawLine(
                this.mPos.X - 1,
                this.mPos.Y + this.mHeight - 2,
                this.mPos.X + this.mWidth - 3,
                this.mPos.Y + this.mHeight - 2,
                1,
                color); //Bot
            Utils.DrawLine(
                this.mPos.X + this.mWidth - 4,
                this.mPos.Y - 1,
                this.mPos.X + this.mWidth - 4,
                this.mPos.Y + this.mHeight - 1,
                1,
                color); //Right
        }

        /// <summary>
        ///     Draws in Content of the ColorBox
        /// </summary>
        private void DrawContent()
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.DrawStyleHue();

                    break;
                case EDrawStyle.Saturation:
                    this.DrawStyleSaturation();

                    break;
                case EDrawStyle.Brightness:
                    this.DrawStyleLuminance();

                    break;
                case EDrawStyle.Red:
                    this.DrawStyleRed();

                    break;
                case EDrawStyle.Green:
                    this.DrawStyleGreen();

                    break;
                case EDrawStyle.Blue:
                    this.DrawStyleBlue();

                    break;
            }
        }

        /// <summary>
        ///     Draws the Marker
        /// </summary>
        private void DrawMarker(int x, int y, bool unconditional)
        {
            if (x < 0)
            {
                x = 0;
            }
            if (x > (this.mWidth - 4))
            {
                x = (this.mWidth - 4);
            }

            if (y < 0)
            {
                x = 0;
            }
            if (y > (this.mHeight - 4))
            {
                y = (this.mHeight - 4);
            }

            if (this.mMarkerY == y & this.mMarkerX == x & !unconditional)
            {
                return;
            }

            this.mMarkerX = x;
            this.mMarkerY = y;

            // The selected color will determine whether our marker is white or black
            var hsl = this.GetColor(x, y);

            if (hsl.L < Convert.ToDouble(200d / 255d))
            {
                this.mMarkerColor = Color.White;
            }
            else if ((hsl.H < Convert.ToDouble(26d / 360d)) | (hsl.H > Convert.ToDouble(200d / 360d)))
            {
                this.mMarkerColor = hsl.S > Convert.ToDouble(70d / 255d) ? Color.White : Color.Black;
            }
            else
            {
                this.mMarkerColor = Color.Black;
            }

            Utils.DrawCircle(
                this.mPos.X + x + 2,
                this.mPos.Y + y + 2,
                5,
                0,
                Utils.CircleType.Full,
                false,
                16,
                this.mMarkerColor.ToSharpDxColor());
        }

        /// <summary>
        ///     Draws in Blue Style
        /// </summary>
        private void DrawStyleBlue()
        {
            int blue = this.mRgb.B;

            for (var i = 0; i <= this.mHeight - 4; i++)
            {
                // Calculate Green at this line (Red and Blue are constant)
                var green = Convert.ToInt32(Math.Round(255 - (255 * Convert.ToDouble(i / (this.mHeight - 4d)))));

                this.GradientRect(
                    this.mPos.X,
                    i + this.mPos.Y,
                    this.mWidth - 4,
                    2,
                    Color.FromArgb(0, green, blue).ToSharpDxColor(),
                    Color.FromArgb(255, green, blue).ToSharpDxColor(),
                    Orientation.Vertical);
            }
        }

        /// <summary>
        ///     Draws in Green Style
        /// </summary>
        private void DrawStyleGreen()
        {
            int green = this.mRgb.G;

            for (var i = 0; i <= this.mHeight - 4; i++)
            {
                // Calculate Red at this line (Green and Blue are constant)
                var red = Convert.ToInt32(Math.Round(255 - (255 * Convert.ToDouble(i / (this.mHeight - 4d)))));

                this.GradientRect(
                    this.mPos.X,
                    i + this.mPos.Y,
                    this.mWidth - 4,
                    2,
                    Color.FromArgb(red, green, 0).ToSharpDxColor(),
                    Color.FromArgb(red, green, 255).ToSharpDxColor(),
                    Orientation.Vertical);
            }
        }

        /// <summary>
        ///     Draws in Hue Style
        /// </summary>
        private void DrawStyleHue()
        {
            var hslStart = new Hsl();
            var hslEnd = new Hsl();
            if ((this.mHsl == null))
            {
                this.mHsl = new Hsl { H = 1, S = 1, L = 1 };
                this.mRgb = Utilities.HslToRgb(this.mHsl);
            }

            hslStart.H = this.mHsl.H;
            hslEnd.H = this.mHsl.H;
            hslStart.S = 0.0;
            hslEnd.S = 1.0;

            for (var i = 0; i <= this.mHeight - 4; i = i + 2)
            {
                hslStart.L = 1.0 - Convert.ToDouble(i / (this.mHeight - 4d));
                // Calculate luminance at this line (Hue and Saturation are constant)
                hslEnd.L = hslStart.L;

                var colorStart = Utilities.HslToRgb(hslStart);
                var colorEnd = Utilities.HslToRgb(hslEnd);

                this.GradientRect(
                    this.mPos.X,
                    i + this.mPos.Y,
                    this.mWidth - 4,
                    2,
                    colorStart.ToSharpDxColor(),
                    colorEnd.ToSharpDxColor(),
                    Orientation.Vertical);
            }
        }

        /// <summary>
        ///     Draws in Luminance Style
        /// </summary>
        private void DrawStyleLuminance()
        {
            var hslStart = new Hsl();
            var hslEnd = new Hsl();
            hslStart.L = this.mHsl.L;
            hslEnd.L = this.mHsl.L;
            hslStart.S = 1.0;
            hslEnd.S = 0.0;

            for (var i = 0; i <= this.mWidth - 4; i++)
            {
                hslStart.H = Convert.ToDouble(i / (this.mWidth - 4d));
                // Calculate Hue at this line (Saturation and Luminance are constant)
                hslEnd.H = hslStart.H;

                var colorStart = Utilities.HslToRgb(hslStart);
                var colorEnd = Utilities.HslToRgb(hslEnd);

                this.GradientRect(
                    i + this.mPos.X,
                    this.mPos.Y,
                    2,
                    this.mHeight - 4,
                    colorStart.ToSharpDxColor(),
                    colorEnd.ToSharpDxColor(),
                    Orientation.Horizontal);
            }
        }

        /// <summary>
        ///     Draws in Red Style
        /// </summary>
        private void DrawStyleRed()
        {
            int red = this.mRgb.R;

            for (var i = 0; i <= this.mHeight - 4; i++)
            {
                // Calculate Green at this line (Red and Blue are constant)
                var green = Convert.ToInt32(Math.Round(255 - (255 * Convert.ToDouble(i / (this.mHeight - 4d)))));

                this.GradientRect(
                    this.mPos.X,
                    i + this.mPos.Y,
                    this.mWidth - 4,
                    2,
                    Color.FromArgb(red, green, 0).ToSharpDxColor(),
                    Color.FromArgb(red, green, 255).ToSharpDxColor(),
                    Orientation.Vertical);
            }
        }

        /// <summary>
        ///     Draws in Saturation Style
        /// </summary>
        private void DrawStyleSaturation()
        {
            var hslStart = new Hsl();
            var hslEnd = new Hsl();
            hslStart.S = this.mHsl.S;
            hslEnd.S = this.mHsl.S;
            hslStart.L = 1.0;
            hslEnd.L = 0.0;

            for (var i = 0; i <= this.mWidth - 4; i++)
            {
                hslStart.H = Convert.ToDouble(i / (this.mWidth - 4d));
                // Calculate Hue at this line (Saturation and Luminance are constant)
                hslEnd.H = hslStart.H;

                var colorStart = Utilities.HslToRgb(hslStart);
                var colorEnd = Utilities.HslToRgb(hslEnd);

                this.GradientRect(
                    i + this.mPos.X,
                    this.mPos.Y,
                    2,
                    this.mHeight - 4,
                    colorStart.ToSharpDxColor(),
                    colorEnd.ToSharpDxColor(),
                    Orientation.Horizontal);
            }
        }

        /// <summary>
        ///     Gets the Color of the given Position
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <returns>HSL Color</returns>
        private Hsl GetColor(int x, int y)
        {
            var hsl = new Hsl();

            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    hsl.H = this.mHsl.H;
                    hsl.S = Convert.ToDouble(x / (this.mWidth - 4d));
                    hsl.L = 1.0 - Convert.ToDouble(y / (this.mHeight - 4d));

                    break;
                case EDrawStyle.Saturation:
                    hsl.S = this.mHsl.S;
                    hsl.H = Convert.ToDouble(x / (this.mWidth - 4d));
                    hsl.L = 1.0 - Convert.ToDouble(y / (this.mHeight - 4d));

                    break;
                case EDrawStyle.Brightness:
                    hsl.L = this.mHsl.L;
                    hsl.H = Convert.ToDouble(x / (this.mWidth - 4d));
                    hsl.S = 1.0 - Convert.ToDouble(y / (this.mHeight - 4d));

                    break;
                case EDrawStyle.Red:
                    hsl =
                        Utilities.RgbToHsl(
                            Color.FromArgb(
                                this.mRgb.R,
                                Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(y / (this.mHeight - 4d))))),
                                Convert.ToInt32(Math.Round(255 * Convert.ToDouble(x / (this.mWidth - 4d))))));

                    break;
                case EDrawStyle.Green:
                    hsl =
                        Utilities.RgbToHsl(
                            Color.FromArgb(
                                Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(y / (this.mHeight - 4d))))),
                                this.mRgb.G,
                                Convert.ToInt32(Math.Round(255 * Convert.ToDouble(x / (this.mWidth - 4d))))));

                    break;
                case EDrawStyle.Blue:
                    hsl =
                        Utilities.RgbToHsl(
                            Color.FromArgb(
                                Convert.ToInt32(Math.Round(255 * Convert.ToDouble(x / (this.mWidth - 4d)))),
                                Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(y / (this.mHeight - 4d))))),
                                this.mRgb.B));

                    break;
            }

            return hsl;
        }

        /// <summary>
        ///     Resets the Color
        /// </summary>
        private void ResetHslrgb()
        {
            int red;
            int green;
            int blue;

            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.mHsl.S = Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d));
                    this.mHsl.L = 1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Saturation:
                    this.mHsl.H = Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d));
                    this.mHsl.L = 1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Brightness:
                    this.mHsl.H = Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d));
                    this.mHsl.S = 1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Red:
                    blue = Convert.ToInt32(Math.Round(255 * Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d))));
                    green =
                        Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d)))));
                    this.mRgb = Color.FromArgb(this.mRgb.R, green, blue);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Green:
                    blue = Convert.ToInt32(Math.Round(255 * Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d))));
                    red =
                        Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d)))));
                    this.mRgb = Color.FromArgb(red, this.mRgb.G, blue);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Blue:
                    red = Convert.ToInt32(Math.Round(255 * Convert.ToDouble(this.mMarkerX / (this.mWidth - 4d))));
                    green =
                        Convert.ToInt32(Math.Round(255 * (1.0 - Convert.ToDouble(this.mMarkerY / (this.mHeight - 4d)))));
                    this.mRgb = Color.FromArgb(red, green, this.mRgb.B);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
            }
        }

        /// <summary>
        ///     Resets the Marker
        /// </summary>
        /// <param name="redraw">Force Redraw</param>
        private void ResetMarker(bool redraw)
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.mMarkerX = Convert.ToInt32(Math.Round((this.mWidth - 4d) * this.mHsl.S));
                    this.mMarkerY = Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - this.mHsl.L)));
                    break;
                case EDrawStyle.Saturation:
                    this.mMarkerX = Convert.ToInt32(Math.Round((this.mWidth - 4d) * this.mHsl.H));
                    this.mMarkerY = Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - this.mHsl.L)));
                    break;
                case EDrawStyle.Brightness:
                    this.mMarkerX = Convert.ToInt32(Math.Round((this.mWidth - 4d) * this.mHsl.H));
                    this.mMarkerY = Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - this.mHsl.S)));
                    break;
                case EDrawStyle.Red:
                    this.mMarkerX =
                        Convert.ToInt32(Math.Round((this.mWidth - 4d) * Convert.ToDouble(this.mRgb.B / 255d)));
                    this.mMarkerY =
                        Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - Convert.ToDouble(this.mRgb.G / 255d))));
                    break;
                case EDrawStyle.Green:
                    this.mMarkerX =
                        Convert.ToInt32(Math.Round((this.mWidth - 4d) * Convert.ToDouble(this.mRgb.B / 255d)));
                    this.mMarkerY =
                        Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - Convert.ToDouble(this.mRgb.R / 255d))));
                    break;
                case EDrawStyle.Blue:
                    this.mMarkerX =
                        Convert.ToInt32(Math.Round((this.mWidth - 4d) * Convert.ToDouble(this.mRgb.R / 255d)));
                    this.mMarkerY =
                        Convert.ToInt32(Math.Round((this.mHeight - 4d) * (1.0 - Convert.ToDouble(this.mRgb.G / 255d))));

                    break;
            }

            if (redraw)
            {
                this.DrawMarker(this.mMarkerX, this.mMarkerY, true);
            }
        }

        #endregion
    }

    /// <summary>
    ///     The VerticalColorSlider
    /// </summary>
    public class VerticalColorSlider
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     Height of the ColorBox
        /// </summary>
        private readonly int mHeight;

        /// <summary>
        ///     Width of the ColorBox
        /// </summary>
        private readonly int mWidth;

        /// <summary>
        ///     VerticalColorSlider disabled
        /// </summary>
        private bool mDisabled;

        /// <summary>
        ///     Is User dragging the slider
        /// </summary>
        private bool mDragging;

        /// <summary>
        ///     Defines the Display Style
        /// </summary>
        private EDrawStyle mEDrawStyle;

        /// <summary>
        ///     Hue, Saturation, Lightness
        /// </summary>
        private Hsl mHsl;

        /// <summary>
        ///     Reg, Green, Blue
        /// </summary>
        private Color mRgb;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Init the VerticalColorSlider
        /// </summary>
        /// <param name="size"></param>
        /// <param name="disabled"></param>
        public VerticalColorSlider(Size size, bool disabled = false)
        {
            this.mHsl = new Hsl { H = 1, S = 1, L = 1 };

            this.mRgb = Utilities.HslToRgb(this.mHsl);
            this.mEDrawStyle = EDrawStyle.Hue;

            this.mHeight = size.Height;
            this.mWidth = size.Width;

            this.mDisabled = disabled;

            this.DrawControl();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     EventHandler for the event <see cref="VerticalColorSlider.ColorSliderScroll" />
        /// </summary>
        public delegate void ColorSliderScrollEventHandler();

        #endregion

        #region Public Events

        /// <summary>
        ///     The event which gets fired when the color get changed
        /// </summary>
        public event ColorSliderScrollEventHandler ColorSliderScroll;

        #endregion

        #region Enums

        /// <summary>
        ///     The DrawStyle enum
        /// </summary>
        public enum EDrawStyle
        {
            /// <summary>
            ///     Hue Style
            /// </summary>
            Hue,

            /// <summary>
            ///     Saturation Style
            /// </summary>
            Saturation,

            /// <summary>
            ///     Brightness Style
            /// </summary>
            Brightness,

            /// <summary>
            ///     Red Style
            /// </summary>
            Red,

            /// <summary>
            ///     Green Style
            /// </summary>
            Green,

            /// <summary>
            ///     Blue Style
            /// </summary>
            Blue
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Position of the two Arrows
        /// </summary>
        public int ArrowPos { get; private set; }

        /// <summary>
        ///     Hue, Saturation, Lightness Property
        /// </summary>
        public Hsl CbHsl
        {
            get
            {
                return this.mHsl;
            }
            set
            {
                this.mHsl = value;
                this.mRgb = Utilities.HslToRgb(this.mHsl);

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        /// <summary>
        ///     The DrawStyle Property
        /// </summary>
        public EDrawStyle DrawStyle
        {
            get
            {
                return this.mEDrawStyle;
            }
            set
            {
                this.mEDrawStyle = value;

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        /// <summary>
        ///     Position of the ColorSlider
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Red, Green, Blue Property
        /// </summary>
        public Color Rgb
        {
            get
            {
                return this.mRgb;
            }
            set
            {
                this.mRgb = value;
                this.mHsl = Utilities.RgbToHsl(this.mRgb);

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws the ColorSlider
        /// </summary>
        /// <param name="newPos">Sets a new Position</param>
        public void DrawControl(Vector2 newPos = new Vector2())
        {
            this.Position = newPos;

            this.DrawContent();
            this.DrawSlider(this.ArrowPos, true);
            this.DrawBorder();
        }

        /// <summary>
        ///     Gets fired when the left mouse button is pressed
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalColorSliderMouseDown(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.LBUTTONDOWN && !this.mDisabled)
            {
                this.mDragging = true;

                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;

                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.ColorSliderScroll?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is moved and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalColorSliderMouseMove(WindowsKeys args)
        {
            if (this.mDragging && args.Msg == WindowsMessages.MOUSEMOVE && !this.mDisabled)
            {
                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;
                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.ColorSliderScroll?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is released and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalColorSliderMouseUp(WindowsKeys args)
        {
            if (this.mDragging && args.Msg == WindowsMessages.LBUTTONUP && !this.mDisabled)
            {
                this.mDragging = false;

                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;
                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.ColorSliderScroll?.Invoke();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Draws the Border around the ColorSlider
        /// </summary>
        private void DrawBorder()
        {
            ColorBGRA color = Color.FromArgb(37, 37, 37).ToSharpDxColor();

            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + 3,
                this.Position.X + this.mWidth - 10,
                this.Position.Y + 3,
                1,
                color); //Top
            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + 3,
                this.Position.X + 10,
                this.Position.Y + this.mHeight - 3,
                1,
                color); //Left
            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + this.mHeight - 4,
                this.Position.X + this.mWidth - 10,
                this.Position.Y + this.mHeight - 4,
                1,
                color);
            //Bot
            Utils.DrawLine(
                this.Position.X + this.mWidth - 11,
                this.Position.Y + 3,
                this.Position.X + this.mWidth - 11,
                this.Position.Y + this.mHeight - 3,
                1,
                color);
            //Right
        }

        /// <summary>
        ///     Draws in Content of the ColorSlider
        /// </summary>
        private void DrawContent()
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.DrawStyleHue();

                    break;
                case EDrawStyle.Saturation:
                    this.DrawStyleSaturation();

                    break;
                case EDrawStyle.Brightness:
                    this.DrawStyleLuminance();

                    break;
                case EDrawStyle.Red:
                    this.DrawStyleRed();

                    break;
                case EDrawStyle.Green:
                    this.DrawStyleGreen();

                    break;
                case EDrawStyle.Blue:
                    this.DrawStyleBlue();

                    break;
            }
        }

        /// <summary>
        ///     Draws the Arrows
        /// </summary>
        private void DrawSlider(int position, bool unconditional)
        {
            if (position < 0)
            {
                position = 0;
            }
            if (position > (this.mHeight - 9))
            {
                position = (this.mHeight - 9);
            }

            if (this.ArrowPos == position & !unconditional)
            {
                return;
            }

            this.ArrowPos = position;

            ColorBGRA color = Color.FromArgb(4, 188, 252).ToSharpDxColor();

            Utils.DrawBoxFilled(this.Position.X, this.Position.Y + position, 2, 7, color);
            Utils.DrawBoxFilled(this.Position.X + 2, this.Position.Y + position + 1, 2, 5, color);
            Utils.DrawBoxFilled(this.Position.X + 4, this.Position.Y + position + 2, 2, 3, color);
            Utils.DrawBoxFilled(this.Position.X + 6, this.Position.Y + position + 3, 2, 1, color);

            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2, this.Position.Y + position, 2, 7, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 2, this.Position.Y + position + 1, 2, 5, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 4, this.Position.Y + position + 2, 2, 3, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 6, this.Position.Y + position + 3, 2, 1, color);
        }

        /// <summary>
        ///     Draws in Blue Style
        /// </summary>
        private void DrawStyleBlue()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var blue = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(this.mRgb.R, this.mRgb.G, blue);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Green Style
        /// </summary>
        private void DrawStyleGreen()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var green = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(this.mRgb.R, green, this.mRgb.B);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Hue Style
        /// </summary>
        private void DrawStyleHue()
        {
            var hsl = new Hsl { S = 1, L = 1 };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.H = 1.0d - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Luminance Style
        /// </summary>
        private void DrawStyleLuminance()
        {
            var hsl = new Hsl { H = this.mHsl.H, S = this.mHsl.S };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.L = 1.0 - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Red Style
        /// </summary>
        private void DrawStyleRed()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var red = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(red, this.mRgb.G, this.mRgb.B);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Saturation Style
        /// </summary>
        private void DrawStyleSaturation()
        {
            var hsl = new Hsl { H = this.mHsl.H, L = this.mHsl.L };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.S = 1.0 - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Resets the Color
        /// </summary>
        private void ResetHslrgb()
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.mHsl.H = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Saturation:
                    this.mHsl.S = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Brightness:
                    this.mHsl.L = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Red:
                    this.mRgb =
                        Color.FromArgb(
                            255
                            - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))),
                            this.mRgb.G,
                            this.mRgb.B);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Green:
                    this.mRgb = Color.FromArgb(
                        this.mRgb.R,
                        255 - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))),
                        this.mRgb.B);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Blue:
                    this.mRgb = Color.FromArgb(
                        this.mRgb.R,
                        this.mRgb.G,
                        255 - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))));
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);
                    break;
            }
        }

        /// <summary>
        ///     Resets the Arrows
        /// </summary>
        /// <param name="redraw">Force Redraw</param>
        private void ResetSlider(bool redraw)
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.H));

                    break;
                case EDrawStyle.Saturation:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.S));

                    break;
                case EDrawStyle.Brightness:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.L));

                    break;
                case EDrawStyle.Red:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.R / 255d)));

                    break;
                case EDrawStyle.Green:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.G / 255d)));

                    break;
                case EDrawStyle.Blue:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.B / 255d)));

                    break;
            }

            if (redraw)
            {
                this.DrawSlider(this.ArrowPos, true);
            }
        }

        #endregion
    }

    /// <summary>
    ///     The VerticalAlphaSlider
    /// </summary>
    public class VerticalAlphaSlider
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     Is User dragging the slider
        /// </summary>
        private bool mBDragging;

        /// <summary>
        ///     VerticalAlphaSlider disabled
        /// </summary>
        private bool mDisabled;

        /// <summary>
        ///     Defines the Display Style
        /// </summary>
        private EDrawStyle mEDrawStyle;

        /// <summary>
        ///     Height of the ColorBox
        /// </summary>
        private int mHeight;

        /// <summary>
        ///     Hue, Saturation, Lightness
        /// </summary>
        private Hsl mHsl;

        /// <summary>
        ///     Reg, Green, Blue
        /// </summary>
        private Color mRgb;

        /// <summary>
        ///     Width of the ColorBox
        /// </summary>
        private int mWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Init the VerticalAlphaSlider
        /// </summary>
        /// <param name="size"></param>
        /// <param name="disabled"></param>
        public VerticalAlphaSlider(Size size, bool disabled = false)
        {
            this.mHsl = new Hsl { H = 1, S = 0, L = 1 };

            this.mRgb = Utilities.HslToRgb(this.mHsl);
            this.mEDrawStyle = EDrawStyle.Brightness;

            this.mHeight = size.Height;
            this.mWidth = size.Width;

            this.mDisabled = disabled;

            this.DrawControl();
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     EventHandler for the event <see cref="VerticalAlphaSlider.AlphaSliderScroll" />
        /// </summary>
        public delegate void AlphaSliderScrollEventHandler();

        #endregion

        #region Public Events

        /// <summary>
        ///     The event which gets fired when the color get changed
        /// </summary>
        public event AlphaSliderScrollEventHandler AlphaSliderScroll;

        #endregion

        #region Enums

        /// <summary>
        ///     The DrawStyle enum
        /// </summary>
        public enum EDrawStyle
        {
            /// <summary>
            ///     Hue Style
            /// </summary>
            Hue,

            /// <summary>
            ///     Saturation Style
            /// </summary>
            Saturation,

            /// <summary>
            ///     Brightness Style
            /// </summary>
            Brightness,

            /// <summary>
            ///     Red Style
            /// </summary>
            Red,

            /// <summary>
            ///     Green Style
            /// </summary>
            Green,

            /// <summary>
            ///     Blue Style
            /// </summary>
            Blue
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Position of the two Arrows
        /// </summary>
        public int ArrowPos { get; private set; }

        /// <summary>
        ///     Hue, Saturation, Lightness Property
        /// </summary>
        public Hsl CbHsl
        {
            get
            {
                return this.mHsl;
            }
            set
            {
                this.mHsl = value;
                this.mRgb = Utilities.HslToRgb(this.mHsl);

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        /// <summary>
        ///     The DrawStyle Property
        /// </summary>
        public EDrawStyle DrawStyle
        {
            get
            {
                return this.mEDrawStyle;
            }
            set
            {
                this.mEDrawStyle = value;

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        /// <summary>
        ///     Position of the ColorSlider
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Red, Green, Blue Property
        /// </summary>
        public Color Rgb
        {
            get
            {
                return this.mRgb;
            }
            set
            {
                this.mRgb = value;
                this.mHsl = Utilities.RgbToHsl(this.mRgb);

                this.ResetSlider(true);
                this.DrawControl();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws the AlphaSlider
        /// </summary>
        /// <param name="newPos">Sets a new Position</param>
        public void DrawControl(Vector2 newPos = new Vector2())
        {
            this.Position = newPos;

            this.DrawContent();
            this.DrawSlider(this.ArrowPos, true);
            this.DrawBorder();
        }

        /// <summary>
        ///     Gets fired when the left mouse button is pressed
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalAlphaSliderMouseDown(WindowsKeys args)
        {
            if (args.Msg == WindowsMessages.LBUTTONDOWN && !this.mDisabled)
            {
                this.mBDragging = true;

                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;

                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.AlphaSliderScroll?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is moved and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalAlphaSliderMouseMove(WindowsKeys args)
        {
            if (this.mBDragging && args.Msg == WindowsMessages.MOUSEMOVE && !this.mDisabled)
            {
                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;
                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.AlphaSliderScroll?.Invoke();
            }
        }

        /// <summary>
        ///     Gets fired when the mouse is released and pressed before
        /// </summary>
        /// <param name="args">Keys</param>
        public void VerticalAlphaSliderMouseUp(WindowsKeys args)
        {
            if (this.mBDragging && args.Msg == WindowsMessages.LBUTTONUP && !this.mDisabled)
            {
                this.mBDragging = false;

                var y = (int)args.Cursor.Y - (int)this.Position.Y;
                y -= 4;
                if (y < 0)
                {
                    y = 0;
                }
                if (y > (this.mHeight - 9))
                {
                    y = (this.mHeight - 9);
                }

                this.ArrowPos = y;
                this.DrawSlider(y, false);
                this.ResetHslrgb();
                this.AlphaSliderScroll?.Invoke();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Draws the Border around the AlphaSlider
        /// </summary>
        private void DrawBorder()
        {
            ColorBGRA color = Color.FromArgb(37, 37, 37).ToSharpDxColor();

            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + 3,
                this.Position.X + this.mWidth - 10,
                this.Position.Y + 3,
                1,
                color); //Top
            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + 3,
                this.Position.X + 10,
                this.Position.Y + this.mHeight - 3,
                1,
                color); //Left
            Utils.DrawLine(
                this.Position.X + 10,
                this.Position.Y + this.mHeight - 4,
                this.Position.X + this.mWidth - 10,
                this.Position.Y + this.mHeight - 4,
                1,
                color);
            //Bot
            Utils.DrawLine(
                this.Position.X + this.mWidth - 11,
                this.Position.Y + 3,
                this.Position.X + this.mWidth - 11,
                this.Position.Y + this.mHeight - 3,
                1,
                color);
            //Right
        }

        /// <summary>
        ///     Draws in Content of the AlphaSlider
        /// </summary>
        private void DrawContent()
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.DrawStyleHue();

                    break;
                case EDrawStyle.Saturation:
                    this.DrawStyleSaturation();

                    break;
                case EDrawStyle.Brightness:
                    this.DrawStyleLuminance();

                    break;
                case EDrawStyle.Red:
                    this.DrawStyleRed();

                    break;
                case EDrawStyle.Green:
                    this.DrawStyleGreen();

                    break;
                case EDrawStyle.Blue:
                    this.DrawStyleBlue();

                    break;
            }
        }

        /// <summary>
        ///     Draws the Arrows
        /// </summary>
        private void DrawSlider(int position, bool unconditional)
        {
            if (position < 0)
            {
                position = 0;
            }
            if (position > (this.mHeight - 9))
            {
                position = (this.mHeight - 9);
            }

            if (this.ArrowPos == position & !unconditional)
            {
                return;
            }

            this.ArrowPos = position;

            ColorBGRA color = Color.FromArgb(4, 188, 252).ToSharpDxColor();

            Utils.DrawBoxFilled(this.Position.X, this.Position.Y + position, 2, 7, color);
            Utils.DrawBoxFilled(this.Position.X + 2, this.Position.Y + position + 1, 2, 5, color);
            Utils.DrawBoxFilled(this.Position.X + 4, this.Position.Y + position + 2, 2, 3, color);
            Utils.DrawBoxFilled(this.Position.X + 6, this.Position.Y + position + 3, 2, 1, color);

            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2, this.Position.Y + position, 2, 7, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 2, this.Position.Y + position + 1, 2, 5, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 4, this.Position.Y + position + 2, 2, 3, color);
            Utils.DrawBoxFilled(this.Position.X + this.mWidth - 2 - 6, this.Position.Y + position + 3, 2, 1, color);
        }

        /// <summary>
        ///     Draws in Blue Style
        /// </summary>
        private void DrawStyleBlue()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var blue = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(this.mRgb.R, this.mRgb.G, blue);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Green Style
        /// </summary>
        private void DrawStyleGreen()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var green = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(this.mRgb.R, green, this.mRgb.B);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Hue Style
        /// </summary>
        private void DrawStyleHue()
        {
            var hsl = new Hsl { S = 1, L = 1 };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.H = 1.0d - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Luminance Style
        /// </summary>
        private void DrawStyleLuminance()
        {
            var hsl = new Hsl { H = this.mHsl.H, S = this.mHsl.S };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.L = 1.0 - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Red Style
        /// </summary>
        private void DrawStyleRed()
        {
            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                var red = Convert.ToInt32(255 - Math.Round(255 * Convert.ToDouble(iCx / (this.mHeight - 9d))));
                var col = Color.FromArgb(red, this.mRgb.G, this.mRgb.B);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Draws in Saturation Style
        /// </summary>
        private void DrawStyleSaturation()
        {
            var hsl = new Hsl { H = this.mHsl.H, L = this.mHsl.L };

            Line.Begin();
            for (var iCx = 0; iCx <= this.mHeight - 9; iCx++)
            {
                hsl.S = 1.0 - Convert.ToDouble(iCx / (this.mHeight - 9d));
                var col = Utilities.HslToRgb(hsl);

                Line.Draw(
                    new[]
                        {
                            new Vector2(this.Position.X + 11, this.Position.Y + iCx + 4),
                            new Vector2(this.Position.X + this.mWidth - 11, this.Position.Y + iCx + 4)
                        },
                    col.ToSharpDxColor());
            }
            Line.End();
        }

        /// <summary>
        ///     Resets the Color
        /// </summary>
        private void ResetHslrgb()
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.mHsl.H = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Saturation:
                    this.mHsl.S = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Brightness:
                    this.mHsl.L = 1.0 - Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d));
                    this.mRgb = Utilities.HslToRgb(this.mHsl);

                    break;
                case EDrawStyle.Red:
                    this.mRgb =
                        Color.FromArgb(
                            255
                            - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))),
                            this.mRgb.G,
                            this.mRgb.B);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Green:
                    this.mRgb = Color.FromArgb(
                        this.mRgb.R,
                        255 - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))),
                        this.mRgb.B);
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);

                    break;
                case EDrawStyle.Blue:
                    this.mRgb = Color.FromArgb(
                        this.mRgb.R,
                        this.mRgb.G,
                        255 - Convert.ToInt32(Math.Round(255d * Convert.ToDouble(this.ArrowPos / (this.mHeight - 9d)))));
                    this.mHsl = Utilities.RgbToHsl(this.mRgb);
                    break;
            }
        }

        /// <summary>
        ///     Resets the Arrows
        /// </summary>
        /// <param name="redraw">Force Redraw</param>
        private void ResetSlider(bool redraw)
        {
            switch (this.mEDrawStyle)
            {
                case EDrawStyle.Hue:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.H));

                    break;
                case EDrawStyle.Saturation:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.S));

                    break;
                case EDrawStyle.Brightness:
                    this.ArrowPos = (this.mHeight - 8) - Convert.ToInt32(Math.Round((this.mHeight - 8d) * this.mHsl.L));

                    break;
                case EDrawStyle.Red:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.R / 255d)));

                    break;
                case EDrawStyle.Green:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.G / 255d)));

                    break;
                case EDrawStyle.Blue:
                    this.ArrowPos = (this.mHeight - 8)
                                    - Convert.ToInt32(
                                        Math.Round((this.mHeight - 8d) * Convert.ToDouble(this.mRgb.B / 255d)));

                    break;
            }

            if (redraw)
            {
                this.DrawSlider(this.ArrowPos, true);
            }
        }

        #endregion
    }

    /// <summary>
    ///     Adobe Color Utitlies for HSL
    /// </summary>
    public class AdobeColors
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Modifies brightness of c by brightness
        /// </summary>
        /// <param name="c"></param>
        /// <param name="brightness"></param>
        /// <returns>New color</returns>
        public Color ModifyBrightness(Color c, double brightness)
        {
            var hsl = new Hsl();
            hsl.L *= brightness;
            return Utilities.HslToRgb(hsl);
        }

        /// <summary>
        ///     Modifies hue of c by hue
        /// </summary>
        /// <param name="c"></param>
        /// <param name="hue"></param>
        /// <returns>New color</returns>
        public Color ModifyHue(Color c, double hue)
        {
            var hsl = new Hsl();
            hsl.H *= hue;
            return Utilities.HslToRgb(hsl);
        }

        /// <summary>
        ///     Modifies saturation of c by saturation
        /// </summary>
        /// <param name="c"></param>
        /// <param name="saturation"></param>
        /// <returns>New color</returns>
        public Color ModifySaturation(Color c, double saturation)
        {
            var hsl = new Hsl();
            hsl.S *= saturation;
            return Utilities.HslToRgb(hsl);
        }

        /// <summary>
        ///     Sets brightness of c by brightness
        /// </summary>
        /// <param name="c"></param>
        /// <param name="brightness"></param>
        /// <returns>New color</returns>
        public Color SetBrightness(Color c, double brightness)
        {
            var hsl = new Hsl { L = brightness };
            return Utilities.HslToRgb(hsl);
        }

        /// <summary>
        ///     Sets hue of c by hue
        /// </summary>
        /// <param name="c"></param>
        /// <param name="hue"></param>
        /// <returns>New color</returns>
        public Color SetHue(Color c, double hue)
        {
            var hsl = new Hsl { H = hue };
            return Utilities.HslToRgb(hsl);
        }

        /// <summary>
        ///     Sets saturation of c by saturation
        /// </summary>
        /// <param name="c"></param>
        /// <param name="saturation"></param>
        /// <returns>New color</returns>
        public Color SetSaturation(Color c, double saturation)
        {
            var hsl = new Hsl { S = saturation };
            return Utilities.HslToRgb(hsl);
        }

        #endregion
    }

    /// <summary>
    ///     Color spectrum CMYK
    /// </summary>
    public class Cmyk
    {
        #region Fields

        /// <summary>
        ///     Cyan
        /// </summary>
        private double c;

        /// <summary>
        ///     Key plate (black)
        /// </summary>
        private double k;

        /// <summary>
        ///     Magenta
        /// </summary>
        private double m;

        /// <summary>
        ///     Yellow
        /// </summary>
        private double y;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Cyan
        /// </summary>
        public double C
        {
            get
            {
                return this.c;
            }
            set
            {
                this.c = value;
                if (this.c < 0)
                {
                    this.c = 0;
                }
                else if (this.c > 1)
                {
                    this.c = 1;
                }
            }
        }

        /// <summary>
        ///     Key plate (black)
        /// </summary>
        public double K
        {
            get
            {
                return this.k;
            }
            set
            {
                this.k = value;
                if (this.k < 0)
                {
                    this.k = 0;
                }
                else if (this.k > 1)
                {
                    this.k = 1;
                }
            }
        }

        /// <summary>
        ///     Magenta
        /// </summary>
        public double M
        {
            get
            {
                return this.m;
            }
            set
            {
                this.m = value;
                if (this.m < 0)
                {
                    this.m = 0;
                }
                else if (this.m > 1)
                {
                    this.m = 1;
                }
            }
        }

        /// <summary>
        ///     Yellow
        /// </summary>
        public double Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
                if (this.y < 0)
                {
                    this.y = 0;
                }
                else if (this.y > 1)
                {
                    this.y = 1;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Color spectrum HSL
    /// </summary>
    public class Hsl
    {
        #region Fields

        /// <summary>
        ///     Hue
        /// </summary>
        private double h;

        /// <summary>
        ///     Lightness
        /// </summary>
        private double l;

        /// <summary>
        ///     Saturation
        /// </summary>
        private double s;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Hue
        /// </summary>
        public double H
        {
            get
            {
                return this.h;
            }
            set
            {
                this.h = value;
                if (this.h < 0)
                {
                    this.h = 0;
                }
                else if (this.h > 1)
                {
                    this.h = 1;
                }
            }
        }

        /// <summary>
        ///     Lightness
        /// </summary>
        public double L
        {
            get
            {
                return this.l;
            }
            set
            {
                this.l = value;
                if (this.l < 0)
                {
                    this.l = 0;
                }
                else if (this.l > 1)
                {
                    this.l = 1;
                }
            }
        }

        /// <summary>
        ///     Saturation
        /// </summary>
        public double S
        {
            get
            {
                return this.s;
            }
            set
            {
                this.s = value;
                if (this.s < 0)
                {
                    this.s = 0;
                }
                else if (this.s > 1)
                {
                    this.s = 1;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Color Utilities
    /// </summary>
    internal static class Utilities
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Converts the CMYK color format to RGB
        /// </summary>
        /// <param name="cmyk"></param>
        /// <returns>Converted color</returns>
        public static Color CmykToRgb(Cmyk cmyk)
        {
            // To convert CMYK to RGB we first have to convert CMYK to CMY
            var cyan = (cmyk.C * (1 - cmyk.K)) + cmyk.K;
            var magenta = (cmyk.M * (1 - cmyk.K)) + cmyk.K;
            var yellow = (cmyk.Y * (1 - cmyk.K)) + cmyk.K;

            var red = Convert.ToInt32(Math.Round(cyan * 255d));
            var green = Convert.ToInt32(Math.Round(magenta * 255d));
            var blue = Convert.ToInt32(Math.Round(yellow * 255d));

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        ///     Converts the HSL color format to RGB
        /// </summary>
        /// <param name="hsl"></param>
        /// <returns>Converted color</returns>
        public static Color HslToRgb(Hsl hsl)
        {
            int mid;

            var max = Convert.ToInt32(Math.Round(hsl.L * 255d));
            var min = Convert.ToInt32(Math.Round((1.0d - hsl.S) * (hsl.L / 1.0d) * 255d));
            var q = Convert.ToDouble((max - min) / 255d);

            if (hsl.H >= 0 & hsl.H <= (1d / 6d))
            {
                mid = Convert.ToInt32(Math.Round(((hsl.H - 0d) * q) * 1530 + min));
                return Color.FromArgb(max, mid, min);
            }
            if (hsl.H <= (1d / 3d))
            {
                mid = Convert.ToInt32(Math.Round(-((hsl.H - Convert.ToDouble(1d / 6d)) * q) * 1530 + max));
                return Color.FromArgb(mid, max, min);
            }
            if (hsl.H <= 0.5)
            {
                mid = Convert.ToInt32(Math.Round(((hsl.H - Convert.ToDouble(1d / 3d)) * q) * 1530 + min));
                return Color.FromArgb(min, max, mid);
            }
            if (hsl.H <= (2d / 3d))
            {
                mid = Convert.ToInt32(Math.Round(-((hsl.H - 0.5d) * q) * 1530 + max));
                return Color.FromArgb(min, mid, max);
            }
            if (hsl.H <= (5d / 6d))
            {
                mid = Convert.ToInt32(Math.Round(((hsl.H - Convert.ToDouble(2d / 3d)) * q) * 1530 + min));
                return Color.FromArgb(mid, min, max);
            }
            if (hsl.H <= 1.0)
            {
                mid = Convert.ToInt32(Math.Round(-((hsl.H - (5d / 6d)) * q) * 1530 + max));
                return Color.FromArgb(max, min, mid);
            }
            return Color.FromArgb(0, 0, 0);
        }

        /// <summary>
        ///     Converts the RGB color format to CMYK
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Converted color</returns>
        public static Cmyk RgbToCmyk(Color c)
        {
            var cmyk = new Cmyk();
            double low = 1;

            cmyk.C = Convert.ToDouble((255d - c.R) / 255d);
            if (low > cmyk.C)
            {
                low = cmyk.C;
            }

            cmyk.M = Convert.ToDouble((255d - c.G) / 255d);
            if (low > cmyk.M)
            {
                low = cmyk.M;
            }

            cmyk.Y = Convert.ToDouble((255d - c.B) / 255d);
            if (low > cmyk.Y)
            {
                low = cmyk.Y;
            }

            if (low > 0)
            {
                cmyk.K = low;
            }

            return cmyk;
        }

        /// <summary>
        ///     Converts the RGB color format to HSL
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Converted color</returns>
        public static Hsl RgbToHsl(Color c)
        {
            var hsl = new Hsl();

            int max;
            int min;

            // Of the RBG Values - assign the highest value to _Max and the lowest to _min
            if (c.R > c.G)
            {
                max = c.R;
                min = c.G;
            }
            else
            {
                max = c.G;
                min = c.R;
            }
            if (c.B > max)
            {
                max = c.B;
            }
            if (c.B < min)
            {
                min = c.B;
            }

            var diff = max - min;

            // Luminance (aka Brightness)
            hsl.L = Convert.ToDouble(max / 255d);

            // Saturation
            hsl.S = max == 0 ? 0 : Convert.ToDouble(diff / (double)max);

            // Hue
            // R is situated at the angle of 360 eller noll degrees
            // G vid 120 degrees
            // B vid 240 degrees
            var q = diff == 0 ? 0 : Convert.ToDouble(60d / diff);

            if (max == Convert.ToInt32(c.R))
            {
                if (Convert.ToInt32(c.G) < Convert.ToInt32(c.B))
                {
                    hsl.H = Convert.ToDouble(360d + (q * (Convert.ToInt32(c.G) - Convert.ToInt32(c.B)))) / 360d;
                }
                else
                {
                    hsl.H = Convert.ToDouble(q * (Convert.ToInt32(c.G) - Convert.ToInt32(c.B))) / 360d;
                }
            }
            else if (max == Convert.ToInt32(c.G))
            {
                hsl.H = Convert.ToDouble(120d + (q * (Convert.ToInt32(c.B) - Convert.ToInt32(c.R)))) / 360d;
            }
            else if (max == Convert.ToInt32(c.B))
            {
                hsl.H = Convert.ToDouble(240d + (q * (Convert.ToInt32(c.R) - Convert.ToInt32(c.G)))) / 360d;
            }

            return hsl;
        }

        #endregion
    }
}