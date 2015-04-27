using System;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render.RenderObjects
{
    /// <summary>
    ///     RenderObject class used to render a Text
    /// </summary>
    public class Text : Render.RenderObject
    {
        /// <summary>
        ///     Position Delegate
        /// </summary>
        public delegate Vector2 PositionDelegate();

        /// <summary>
        ///     Text Delegate
        /// </summary>
        public delegate string TextDelegate();

        private string _text;
        private Font _textFont;
        private int _x;
        private int _y;

        /// <summary>
        ///     Boolean if the Text is centered
        /// </summary>
        public bool Centered = false;

        /// <summary>
        ///     Vector2 Offset of the Text
        /// </summary>
        public Vector2 Offset;

        /// <summary>
        ///     Boolean if the Text is outlined
        /// </summary>
        public bool OutLined = false;

        /// <summary>
        ///     Position Delegate
        /// </summary>
        public PositionDelegate PositionUpdate;

        /// <summary>
        ///     Text Delegate
        /// </summary>
        public TextDelegate TextUpdate;

        /// <summary>
        ///     Unit of the Text
        /// </summary>
        public Obj_AI_Base Unit;

        /// <summary>
        ///     Renders a Text
        /// </summary>
        /// <param name="text">Text of the rendered Text</param>
        /// <param name="x">X Position of the Text</param>
        /// <param name="y">Y Position of the Text</param>
        /// <param name="size">Size of the Text</param>
        /// <param name="color">Color of the Text</param>
        /// <param name="fontName">Fontname of the Text</param>
        public Text(string text, int x, int y, int size, ColorBGRA color, string fontName = "Calibri")
        {
            Color = color;
            this.text = text;

            _x = x;
            _y = y;

            _textFont = new Font(
                Device,
                new FontDescription
                {
                    FaceName = fontName,
                    Height = size,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        /// <summary>
        ///     Renders a Text
        /// </summary>
        /// <param name="text">Text of the rendered Text</param>
        /// <param name="position">Vector2 Position of the Text</param>
        /// <param name="size">Size of the Text</param>
        /// <param name="color">Color of the Text</param>
        /// <param name="fontName">Fontname of the Text</param>
        public Text(string text, Vector2 position, int size, ColorBGRA color, string fontName = "Calibri")
        {
            Color = color;
            this.text = text;

            _x = (int)position.X;
            _y = (int)position.Y;

            _textFont = new Font(
                Device,
                new FontDescription
                {
                    FaceName = fontName,
                    Height = size,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        /// <summary>
        ///     Renders a Text
        /// </summary>
        /// <param name="text">Text of the rendered Text</param>
        /// <param name="unit">Unit of the Text</param>
        /// <param name="offset">Vector2 Offset of the Text</param>
        /// <param name="size">Size of the Text</param>
        /// <param name="color">Color of the Text</param>
        /// <param name="fontName">Fontname of the Text</param>
        public Text(string text, Obj_AI_Base unit, Vector2 offset, int size, ColorBGRA color,
            string fontName = "Calibri")
        {
            Unit = unit;
            Color = color;
            this.text = text;
            Offset = offset;

            var pos = unit.HPBarPosition + offset;

            _x = (int)pos.X;
            _y = (int)pos.Y;

            _textFont = new Font(
                Device,
                new FontDescription
                {
                    FaceName = fontName,
                    Height = size,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        /// <summary>
        ///     Renders a Text
        /// </summary>
        /// <param name="x">X Position of the Text</param>
        /// <param name="y">Y Position of the Text</param>
        /// <param name="text">Text of the rendered Text</param>
        /// <param name="size">Size of the Text</param>
        /// <param name="color">Color of the Text</param>
        /// <param name="fontName">Fontname of the Text</param>
        public Text(int x, int y, string text, int size, ColorBGRA color, string fontName = "Calibri")
        {
            Color = color;
            this.text = text;

            _x = x;
            _y = y;

            _textFont = new Font(
                Device,
                new FontDescription
                {
                    FaceName = fontName,
                    Height = size,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        /// <summary>
        ///     Renders a Text
        /// </summary>
        /// <param name="position">Vector2 Position of the Text</param>
        /// <param name="text">Text of the rendered Text</param>
        /// <param name="size">Size of the Text</param>
        /// <param name="color">Color of the Text</param>
        /// <param name="fontName">Fontname of the Text</param>
        public Text(Vector2 position, string text, int size, ColorBGRA color, string fontName = "Calibri")
        {
            Color = color;
            this.text = text;
            _x = (int)position.X;
            _y = (int)position.Y;
            _textFont = new Font(
                Device,
                new FontDescription
                {
                    FaceName = fontName,
                    Height = size,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });
        }

        /// <summary>
        ///     Direct3DDevice
        /// </summary>
        public static Device Device
        {
            get { return Drawing.Direct3DDevice; }
        }

        /// <summary>
        ///     TextFont Description
        /// </summary>
        public FontDescription TextFontDescription
        {
            get { return _textFont.Description; }

            set
            {
                _textFont.Dispose();
                _textFont = new Font(Device, value);
            }
        }

        /// <summary>
        ///     X Position of the Text
        /// </summary>
        public int X
        {
            get
            {
                var dx = Centered ? -_textFont.MeasureText(null, text, FontDrawFlags.Center).Width / 2 : 0;

                if (PositionUpdate != null)
                {
                    return (int)PositionUpdate().X + dx;
                }

                return _x + dx;
            }
            set { _x = value; }
        }

        /// <summary>
        ///     Y Position of the Text
        /// </summary>
        public int Y
        {
            get
            {
                var dy = Centered ? -_textFont.MeasureText(null, text, FontDrawFlags.Center).Height / 2 : 0;

                if (PositionUpdate != null)
                {
                    return (int)PositionUpdate().Y + dy;
                }
                return _y + dy;
            }
            set { _y = value; }
        }

        /// <summary>
        ///     Width of the Text
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Color of the Text
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Text of the rendered Text
        /// </summary>
        public string text
        {
            get
            {
                if (TextUpdate != null)
                {
                    return TextUpdate();
                }
                return _text;
            }
            set { _text = value; }
        }

        /// <summary>
        ///     OnEndScene Event
        /// </summary>
        public override void OnEndScene()
        {
            try
            {
                if (_textFont.IsDisposed || text == "")
                {
                    return;
                }

                if (Unit != null && Unit.IsValid)
                {
                    var pos = Unit.HPBarPosition + Offset;
                    X = (int)pos.X;
                    Y = (int)pos.Y;
                }

                var xP = X;
                var yP = Y;
                if (OutLined)
                {
                    var outlineColor = new ColorBGRA(0, 0, 0, 255);
                    _textFont.DrawText(null, text, xP - 1, yP - 1, outlineColor);
                    _textFont.DrawText(null, text, xP + 1, yP + 1, outlineColor);
                    _textFont.DrawText(null, text, xP - 1, yP, outlineColor);
                    _textFont.DrawText(null, text, xP + 1, yP, outlineColor);
                }
                _textFont.DrawText(null, text, xP, yP, Color);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Common.Render.Text.OnEndScene: " + e);
            }
        }

        /// <summary>
        ///     OnPreReset Event
        /// </summary>
        public override void OnPreReset()
        {
            _textFont.OnLostDevice();
        }

        /// <summary>
        ///     OnPostReset Event
        /// </summary>
        public override void OnPostReset()
        {
            _textFont.OnResetDevice();
        }

        /// <summary>
        ///     Dispose Event
        /// </summary>
        public override void Dispose()
        {
            if (!_textFont.IsDisposed)
            {
                _textFont.Dispose();
            }
        }
    }
}