using System;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render.RenderObjects
{
    /// <summary>
    ///     RenderObject class used to render a Line
    /// </summary>
    public class Line : Render.RenderObject
    {
        /// <summary>
        ///     Position Delegate
        /// </summary>
        public delegate Vector2 PositionDelegate();

        private readonly SharpDX.Direct3D9.Line _line;
        private Vector2 _end;
        private Vector2 _start;
        private int _width;

        /// <summary>
        ///     Color of Line
        /// </summary>
        public ColorBGRA Color;

        /// <summary>
        ///     Draws a Line
        /// </summary>
        /// <param name="start">Start Position as Vector2</param>
        /// <param name="end">End Position as Vector2</param>
        /// <param name="width">Width of the Line</param>
        /// <param name="color">Color of the Line</param>
        public Line(Vector2 start, Vector2 end, int width, ColorBGRA color)
        {
            _line = new SharpDX.Direct3D9.Line(Device);
            Width = width;
            Color = color;
            _start = start;
            _end = end;
        }

        /// <summary>
        ///     Direct3DDevice
        /// </summary>
        public static Device Device
        {
            get { return Drawing.Direct3DDevice; }
        }

        /// <summary>
        ///     Start Position of the Line
        /// </summary>
        public Vector2 Start
        {
            get { return StartPositionUpdate != null ? StartPositionUpdate() : _start; }
            set { _start = value; }
        }

        /// <summary>
        ///     End Position of the Line
        /// </summary>
        public Vector2 End
        {
            get { return EndPositionUpdate != null ? EndPositionUpdate() : _end; }
            set { _end = value; }
        }

        /// <summary>
        ///     Position Delegate of the StartPosition
        /// </summary>
        public PositionDelegate StartPositionUpdate { get; set; }

        /// <summary>
        ///     Position Delegate of the EndPosition
        /// </summary>
        public PositionDelegate EndPositionUpdate { get; set; }

        /// <summary>
        ///     Width of the Line
        /// </summary>
        public int Width
        {
            get { return _width; }
            set
            {
                _line.Width = value;
                _width = value;
            }
        }

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
                _line.Draw(new[] { Start, End }, Color);
                _line.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Common.Render.Line.OnEndScene: " + e);
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