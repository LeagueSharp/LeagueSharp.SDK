using System;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render.RenderObjects
{
    /// <summary>
    ///     RenderObject class used to render a Rectangle
    /// </summary>
    public class Rectangle : Render.RenderObject
    {
        /// <summary>
        ///     Position Delegate
        /// </summary>
        public delegate Vector2 PositionDelegate();

        private readonly SharpDX.Direct3D9.Line _line;
        private int _x;
        private int _y;

        /// <summary>
        ///     Color of the Rectangle
        /// </summary>
        public ColorBGRA Color;

        /// <summary>
        ///     Draws a Rectangle
        /// </summary>
        /// <param name="x">X Position of the Rectangle</param>
        /// <param name="y">Y Position of the Rectangle</param>
        /// <param name="width">Width of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="color">Color of the Rectangle</param>
        public Rectangle(int x, int y, int width, int height, ColorBGRA color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
            _line = new SharpDX.Direct3D9.Line(Device) { Width = height };
        }

        /// <summary>
        ///     Direct3DDevice
        /// </summary>
        public static Device Device
        {
            get { return Drawing.Direct3DDevice; }
        }

        /// <summary>
        ///     X Position of the Rectangle
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
        ///     Y Position of the Rectangle
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
        ///     Width of the Rectangle
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Height of the Rectangle
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Position Delegate
        /// </summary>
        public PositionDelegate PositionUpdate { get; set; }

        /// <summary>
        ///     OnEndScene Event
        /// </summary>
        public override void OnEndScene()
        {
            try
            {
                if (_line.IsDisposed)
                {
                    return;
                }

                _line.Begin();
                _line.Draw(new[] { new Vector2(X, Y + Height / 2), new Vector2(X + Width, Y + Height / 2) }, Color);
                _line.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Common.Render.Rectangle.OnEndScene: " + e);
            }
        }

        /// <summary>
        ///     OnPreReset Event
        /// </summary>
        public override void OnPreReset()
        {
            _line.OnLostDevice();
        }

        /// <summary>
        ///     OnPostReset Event
        /// </summary>
        public override void OnPostReset()
        {
            _line.OnResetDevice();
        }

        /// <summary>
        ///     Dispose Event
        /// </summary>
        public override void Dispose()
        {
            if (!_line.IsDisposed)
            {
                _line.Dispose();
            }
        }
    }
}