using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.Render
{
    /// <summary>
    ///     Render class used to render the RenderObjects.
    /// </summary>
    public static class Render
    {
        private static readonly List<RenderObject> RenderObjects = new List<RenderObject>();
        private static List<RenderObject> _renderVisibleObjects = new List<RenderObject>();
        private static bool _cancelThread;

        static Render()
        {
            Drawing.OnEndScene += Drawing_OnEndScene;
            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
            var thread = new Thread(PrepareObjects);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        ///     Direct3DDevice
        /// </summary>
        public static Device Device
        {
            get { return Drawing.Direct3DDevice; }
        }

        /// <summary>
        ///     Returns if a Point is OnScreen
        /// </summary>
        /// <param name="point"></param>
        /// <returns> Returns if a Point is OnScreen</returns>
        public static bool OnScreen(Vector2 point)
        {
            return point.X > 0 && point.Y > 0 && point.X < Drawing.Width && point.Y < Drawing.Height;
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            _cancelThread = true;
            foreach (var renderObject in RenderObjects)
            {
                renderObject.Dispose();
            }
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            foreach (var renderObject in RenderObjects)
            {
                renderObject.OnPostReset();
            }
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            foreach (var renderObject in RenderObjects)
            {
                renderObject.OnPreReset();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Device == null || Device.IsDisposed)
            {
                return;
            }

            foreach (var renderObject in _renderVisibleObjects)
            {
                renderObject.OnDraw();
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Device == null || Device.IsDisposed)
            {
                return;
            }

            Device.SetRenderState(RenderState.AlphaBlendEnable, true);

            foreach (var renderObject in _renderVisibleObjects)
            {
                renderObject.OnEndScene();
            }
        }

        /// <summary>
        ///     Adds a RenderObject
        /// </summary>
        /// <param name="renderObject">Given render Object</param>
        /// <param name="layer">Layer</param>
        /// <returns>Adds the RenderObject to to the RenderObject list</returns>
        public static RenderObject Add(this RenderObject renderObject, int layer = int.MaxValue)
        {
            renderObject.Layer = layer != int.MaxValue ? layer : renderObject.Layer;
            RenderObjects.Add(renderObject);
            return renderObject;
        }

        /// <summary>
        ///     Removes a RenderObject
        /// </summary>
        /// <param name="renderObject">Given render Object</param>
        public static void Remove(this RenderObject renderObject)
        {
            RenderObjects.Remove(renderObject);
        }

        private static void PrepareObjects()
        {
            while (!_cancelThread)
            {
                try
                {
                    Thread.Sleep(1);
                    _renderVisibleObjects =
                        RenderObjects.Where(
                            obj =>
                                obj.Visible && obj.HasValidLayer())
                            .OrderBy(obj => obj.Layer)
                            .ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"Cannot prepare RenderObjects for drawing. Ex:" + e);
                }
            }
        }

        /// <summary>
        ///     Cleans up unmanaged Resources
        /// </summary>
        public class RenderObject : IDisposable
        {
            /// <summary>
            ///     Visible Condition Delegate
            /// </summary>
            /// <param name="sender">Sender of the Delegate</param>
            public delegate bool VisibleConditionDelegate(RenderObject sender);

            private bool _visible = true;

            /// <summary>
            ///     Layer of the RenderObject
            /// </summary>
            public int Layer;

            /// <summary>
            ///     Visible Condition Delegate
            /// </summary>
            public VisibleConditionDelegate VisibleCondition;

            /// <summary>
            ///     Returns if the RenderObject is Visible
            /// </summary>
            public bool Visible
            {
                get { return VisibleCondition != null ? VisibleCondition(this) : _visible; }
                set { _visible = value; }
            }

            /// <summary>
            ///     Dispose Event
            /// </summary>
            public virtual void Dispose()
            {
            }

            /// <summary>
            ///     OnDraw Event
            /// </summary>
            public virtual void OnDraw()
            {
            }

            /// <summary>
            ///     OnEndScene Event
            /// </summary>
            public virtual void OnEndScene()
            {
            }

            /// <summary>
            ///     OnPreReset Event
            /// </summary>
            public virtual void OnPreReset()
            {
            }

            /// <summary>
            ///     OnPostReset Event
            /// </summary>
            public virtual void OnPostReset()
            {
            }

            /// <summary>
            ///     Boolean if the Layer of the RenderObject is valid
            /// </summary>
            public bool HasValidLayer()
            {
                return Layer >= -5 && Layer <= 5;
            }
        }
    }
}   