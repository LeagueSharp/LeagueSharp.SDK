namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using LeagueSharp.SDK.Enumerations;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;

    /// <summary>
    ///     The render class allows you to draw stuff using SharpDX easier.
    /// </summary>
    public static class Render
    {
        #region Static Fields

        private static readonly List<RenderObject> RenderObjects = new List<RenderObject>();

        private static readonly object RenderObjectsLock = new object();

        private static bool cancelThread;

        private static List<RenderObject> renderVisibleObjects = new List<RenderObject>();

        #endregion

        #region Constructors and Destructors

        static Render()
        {
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += OnDraw;

            var thread = new Thread(PrepareObjects);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the device.
        /// </summary>
        /// <value>The device.</value>
        public static Device Device => Drawing.Direct3DDevice;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified layer.
        /// </summary>
        /// <param name="renderObject">The render object.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>RenderObject.</returns>
        public static RenderObject Add(this RenderObject renderObject, float layer = float.MaxValue)
        {
            renderObject.Layer = !layer.Equals(float.MaxValue) ? layer : renderObject.Layer;
            lock (RenderObjectsLock)
            {
                RenderObjects.Add(renderObject);
            }
            return renderObject;
        }

        /// <summary>
        ///     Determines if the point is on the screen.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the point is on the screen, <c>false</c> otherwise.</returns>
        public static bool OnScreen(Vector2 point)
        {
            return point.X > 0 && point.Y > 0 && point.X < Drawing.Width && point.Y < Drawing.Height;
        }

        /// <summary>
        ///     Removes the specified render object.
        /// </summary>
        /// <param name="renderObject">The render object.</param>
        public static void Remove(this RenderObject renderObject)
        {
            lock (RenderObjectsLock)
            {
                RenderObjects.Remove(renderObject);
            }
        }

        #endregion

        #region Methods

        private static void OnDraw(EventArgs args)
        {
            if (Device == null || Device.IsDisposed)
            {
                return;
            }

            renderVisibleObjects.ForEach(i => i.OnDraw());
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Device == null || Device.IsDisposed)
            {
                return;
            }

            Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            renderVisibleObjects.ForEach(i => i.OnEndScene());
        }

        private static void PrepareObjects()
        {
            while (!cancelThread)
            {
                try
                {
                    Thread.Sleep(1);
                    lock (RenderObjectsLock)
                    {
                        renderVisibleObjects =
                            RenderObjects.Where(obj => obj.Visible && obj.HasValidLayer)
                                .OrderBy(obj => obj.Layer)
                                .ToList();
                    }
                }
                catch (Exception e)
                {
                    Logging.Write()(LogLevel.Error, "[SDK.Render] Cannot prepare RenderObjects for drawing. Ex:" + e);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Draws circles.
        /// </summary>
        public class Circle : RenderObject
        {
            #region Static Fields

            private static Effect effect;

            private static bool initialized;

            private static Vector3 offset = new Vector3(0, 0, 0);

            private static EffectHandle technique;

            private static VertexDeclaration vertexDeclaration;

            private static VertexElement[] vertexElements;

            private static VertexBuffer vertices;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Circle" /> class.
            /// </summary>
            /// <param name="unit">The unit.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="color">The color.</param>
            /// <param name="width">The width.</param>
            /// <param name="zDeep">if set to <c>true</c> [z deep].</param>
            public Circle(GameObject unit, float radius, Color color, int width = 1, bool zDeep = false)
            {
                this.Color = color;
                this.Unit = unit;
                this.Radius = radius;
                this.Width = width;
                this.ZDeep = zDeep;
                this.SubscribeToResetEvents();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Circle" /> class.
            /// </summary>
            /// <param name="unit">The unit.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="color">The color.</param>
            /// <param name="width">The width.</param>
            /// <param name="zDeep">if set to <c>true</c> [z deep].</param>
            public Circle(GameObject unit, Vector3 offset, float radius, Color color, int width = 1, bool zDeep = false)
            {
                this.Color = color;
                this.Unit = unit;
                this.Radius = radius;
                this.Width = width;
                this.ZDeep = zDeep;
                this.Offset = offset;
                this.SubscribeToResetEvents();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Circle" /> class.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="color">The color.</param>
            /// <param name="width">The width.</param>
            /// <param name="zDeep">if set to <c>true</c> [z deep].</param>
            public Circle(
                Vector3 position,
                Vector3 offset,
                float radius,
                Color color,
                int width = 1,
                bool zDeep = false)
            {
                this.Color = color;
                this.Position = position;
                this.Radius = radius;
                this.Width = width;
                this.ZDeep = zDeep;
                this.Offset = offset;
                this.SubscribeToResetEvents();
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Circle" /> class.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="color">The color.</param>
            /// <param name="width">The width.</param>
            /// <param name="zDeep">if set to <c>true</c> [z deep].</param>
            public Circle(Vector3 position, float radius, Color color, int width = 1, bool zDeep = false)
            {
                this.Color = color;
                this.Position = position;
                this.Radius = radius;
                this.Width = width;
                this.ZDeep = zDeep;
                this.SubscribeToResetEvents();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            /// <value>The color.</value>
            public Color Color { get; set; }

            /// <summary>
            ///     Gets or sets the offset.
            /// </summary>
            /// <value>The offset.</value>
            public Vector3 Offset
            {
                get
                {
                    return offset;
                }
                set
                {
                    offset = value;
                }
            }

            /// <summary>
            ///     Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector3 Position { get; set; }

            /// <summary>
            ///     Gets or sets the radius.
            /// </summary>
            /// <value>The radius.</value>
            public float Radius { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            /// <value>The unit.</value>
            public GameObject Unit { get; set; }

            /// <summary>
            ///     Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether to enable depth buffering.
            /// </summary>
            /// <value><c>true</c> if depth buffering enabled; otherwise, <c>false</c>.</value>
            public bool ZDeep { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Creates the vertexes.
            /// </summary>
            public static void CreateVertexes()
            {
                const float X = 6000f;
                vertices = new VertexBuffer(
                    Device,
                    Utilities.SizeOf<Vector4>() * 2 * 6,
                    Usage.WriteOnly,
                    VertexFormat.None,
                    Pool.Managed);

                vertices.Lock(0, 0, LockFlags.None).WriteRange(
                    new[]
                        {
                            //T1
                            new Vector4(-X, 0f, -X, 1.0f), new Vector4(), new Vector4(-X, 0f, X, 1.0f), new Vector4(),
                            new Vector4(X, 0f, -X, 1.0f), new Vector4(),

                            //T2
                            new Vector4(-X, 0f, X, 1.0f), new Vector4(), new Vector4(X, 0f, X, 1.0f), new Vector4(),
                            new Vector4(X, 0f, -X, 1.0f), new Vector4()
                        });
                vertices.Unlock();

                vertexElements = new[]
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

                vertexDeclaration = new VertexDeclaration(Device, vertexElements);

                #region Effect

                try
                {
                    var compiledEffect = new byte[]
                                             {
                                                 0x01, 0x09, 0xFF, 0xFE, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x03, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
                                                 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00,
                                                 0x50, 0x72, 0x6F, 0x6A, 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x4D, 0x61,
                                                 0x74, 0x72, 0x69, 0x78, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0xA4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x43, 0x69, 0x72, 0x63,
                                                 0x6C, 0x65, 0x43, 0x6F, 0x6C, 0x6F, 0x72, 0x00, 0x03, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0xD4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x52, 0x61, 0x64, 0x69,
                                                 0x75, 0x73, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x07, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x72, 0x64, 0x65, 0x72, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x01, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00,
                                                 0x7A, 0x45, 0x6E, 0x61, 0x62, 0x6C, 0x65, 0x64, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                                                 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x10, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                                                 0x0F, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                                                 0x50, 0x30, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x4D, 0x61, 0x69, 0x6E,
                                                 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x03, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
                                                 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x78, 0x00, 0x00, 0x00, 0x94, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0xB4, 0x00, 0x00, 0x00, 0xD0, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x00, 0x00,
                                                 0xFC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x0C, 0x01, 0x00, 0x00, 0x28, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0xF4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0xEC, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x40, 0x01, 0x00, 0x00, 0x3C, 0x01, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x60, 0x01, 0x00, 0x00, 0x5C, 0x01, 0x00, 0x00,
                                                 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x01, 0x00, 0x00,
                                                 0x7C, 0x01, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xA0, 0x01, 0x00, 0x00, 0x9C, 0x01, 0x00, 0x00, 0x92, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0xC0, 0x01, 0x00, 0x00, 0xBC, 0x01, 0x00, 0x00,
                                                 0x93, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xD8, 0x01, 0x00, 0x00,
                                                 0xD4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                                                 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x04, 0x00, 0x00,
                                                 0x00, 0x02, 0xFF, 0xFF, 0xFE, 0xFF, 0x38, 0x00, 0x43, 0x54, 0x41, 0x42,
                                                 0x1C, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x02, 0xFF, 0xFF,
                                                 0x03, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20,
                                                 0xA3, 0x00, 0x00, 0x00, 0x58, 0x00, 0x00, 0x00, 0x02, 0x00, 0x05, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0x00,
                                                 0x80, 0x00, 0x00, 0x00, 0x02, 0x00, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x8C, 0x00, 0x00, 0x00, 0x70, 0x00, 0x00, 0x00, 0x9C, 0x00, 0x00, 0x00,
                                                 0x02, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00,
                                                 0x70, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x72, 0x64, 0x65, 0x72, 0x00, 0xAB,
                                                 0x00, 0x00, 0x03, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x43, 0x69, 0x72, 0x63,
                                                 0x6C, 0x65, 0x43, 0x6F, 0x6C, 0x6F, 0x72, 0x00, 0x01, 0x00, 0x03, 0x00,
                                                 0x01, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x52, 0x61, 0x64, 0x69, 0x75, 0x73, 0x00, 0x70, 0x73, 0x5F, 0x32, 0x5F,
                                                 0x30, 0x00, 0x4D, 0x69, 0x63, 0x72, 0x6F, 0x73, 0x6F, 0x66, 0x74, 0x20,
                                                 0x28, 0x52, 0x29, 0x20, 0x48, 0x4C, 0x53, 0x4C, 0x20, 0x53, 0x68, 0x61,
                                                 0x64, 0x65, 0x72, 0x20, 0x43, 0x6F, 0x6D, 0x70, 0x69, 0x6C, 0x65, 0x72,
                                                 0x20, 0x39, 0x2E, 0x32, 0x39, 0x2E, 0x39, 0x35, 0x32, 0x2E, 0x33, 0x31,
                                                 0x31, 0x31, 0x00, 0xAB, 0xFE, 0xFF, 0x7C, 0x00, 0x50, 0x52, 0x45, 0x53,
                                                 0x01, 0x02, 0x58, 0x46, 0xFE, 0xFF, 0x30, 0x00, 0x43, 0x54, 0x41, 0x42,
                                                 0x1C, 0x00, 0x00, 0x00, 0x8B, 0x00, 0x00, 0x00, 0x01, 0x02, 0x58, 0x46,
                                                 0x02, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x20,
                                                 0x88, 0x00, 0x00, 0x00, 0x44, 0x00, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x4C, 0x00, 0x00, 0x00, 0x5C, 0x00, 0x00, 0x00,
                                                 0x6C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x78, 0x00, 0x00, 0x00, 0x5C, 0x00, 0x00, 0x00, 0x42, 0x6F, 0x72, 0x64,
                                                 0x65, 0x72, 0x00, 0xAB, 0x00, 0x00, 0x03, 0x00, 0x01, 0x00, 0x01, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x43, 0x69, 0x72, 0x63, 0x6C, 0x65, 0x43, 0x6F, 0x6C, 0x6F, 0x72, 0x00,
                                                 0x01, 0x00, 0x03, 0x00, 0x01, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x74, 0x78, 0x00, 0x4D, 0x69, 0x63, 0x72, 0x6F,
                                                 0x73, 0x6F, 0x66, 0x74, 0x20, 0x28, 0x52, 0x29, 0x20, 0x48, 0x4C, 0x53,
                                                 0x4C, 0x20, 0x53, 0x68, 0x61, 0x64, 0x65, 0x72, 0x20, 0x43, 0x6F, 0x6D,
                                                 0x70, 0x69, 0x6C, 0x65, 0x72, 0x20, 0x39, 0x2E, 0x32, 0x39, 0x2E, 0x39,
                                                 0x35, 0x32, 0x2E, 0x33, 0x31, 0x31, 0x31, 0x00, 0xFE, 0xFF, 0x0C, 0x00,
                                                 0x50, 0x52, 0x53, 0x49, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xFE, 0xFF, 0x1A, 0x00, 0x43, 0x4C, 0x49, 0x54, 0x0C, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xBF,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xFE, 0xFF, 0x1F, 0x00, 0x46, 0x58, 0x4C, 0x43, 0x03, 0x00, 0x00, 0x00,
                                                 0x01, 0x00, 0x30, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x40, 0xA0,
                                                 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                                                 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                                 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
                                                 0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00,
                                                 0xF0, 0xF0, 0xF0, 0xF0, 0x0F, 0x0F, 0x0F, 0x0F, 0xFF, 0xFF, 0x00, 0x00,
                                                 0x51, 0x00, 0x00, 0x05, 0x06, 0x00, 0x0F, 0xA0, 0x00, 0x00, 0xE0, 0x3F,
                                                 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x00, 0x00,
                                                 0x1F, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x07, 0xB0,
                                                 0x05, 0x00, 0x00, 0x03, 0x00, 0x00, 0x08, 0x80, 0x00, 0x00, 0xAA, 0xB0,
                                                 0x00, 0x00, 0xAA, 0xB0, 0x04, 0x00, 0x00, 0x04, 0x00, 0x00, 0x01, 0x80,
                                                 0x00, 0x00, 0x00, 0xB0, 0x00, 0x00, 0x00, 0xB0, 0x00, 0x00, 0xFF, 0x80,
                                                 0x07, 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x00, 0x80,
                                                 0x06, 0x00, 0x00, 0x02, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x00, 0x80,
                                                 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x00, 0x81,
                                                 0x04, 0x00, 0x00, 0xA0, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x02, 0x80,
                                                 0x00, 0x00, 0x00, 0x81, 0x05, 0x00, 0x00, 0xA1, 0x58, 0x00, 0x00, 0x04,
                                                 0x00, 0x00, 0x02, 0x80, 0x00, 0x00, 0x55, 0x80, 0x06, 0x00, 0x55, 0xA0,
                                                 0x06, 0x00, 0xAA, 0xA0, 0x02, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x80,
                                                 0x00, 0x00, 0x00, 0x80, 0x05, 0x00, 0x00, 0xA1, 0x58, 0x00, 0x00, 0x04,
                                                 0x00, 0x00, 0x02, 0x80, 0x00, 0x00, 0xAA, 0x80, 0x06, 0x00, 0x55, 0xA0,
                                                 0x00, 0x00, 0x55, 0x80, 0x05, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x80,
                                                 0x00, 0x00, 0x00, 0x80, 0x06, 0x00, 0x00, 0xA0, 0x58, 0x00, 0x00, 0x04,
                                                 0x00, 0x00, 0x01, 0x80, 0x00, 0x00, 0x00, 0x80, 0x06, 0x00, 0xAA, 0xA0,
                                                 0x06, 0x00, 0x55, 0xA0, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x08, 0x80,
                                                 0x06, 0x00, 0x55, 0xA0, 0x58, 0x00, 0x00, 0x04, 0x00, 0x00, 0x01, 0x80,
                                                 0x01, 0x00, 0x00, 0xA0, 0x00, 0x00, 0xFF, 0x80, 0x00, 0x00, 0x00, 0x80,
                                                 0x05, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0x80, 0x00, 0x00, 0xAA, 0x80,
                                                 0x00, 0x00, 0x00, 0xA0, 0x23, 0x00, 0x00, 0x02, 0x00, 0x00, 0x04, 0x80,
                                                 0x00, 0x00, 0xAA, 0x80, 0x04, 0x00, 0x00, 0x04, 0x00, 0x00, 0x04, 0x80,
                                                 0x03, 0x00, 0xFF, 0xA0, 0x00, 0x00, 0xAA, 0x81, 0x03, 0x00, 0xFF, 0xA0,
                                                 0x58, 0x00, 0x00, 0x04, 0x00, 0x00, 0x02, 0x80, 0x00, 0x00, 0x55, 0x80,
                                                 0x06, 0x00, 0xFF, 0xA0, 0x00, 0x00, 0xAA, 0x80, 0x58, 0x00, 0x00, 0x04,
                                                 0x00, 0x00, 0x08, 0x80, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x55, 0x80,
                                                 0x03, 0x00, 0xFF, 0xA0, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x07, 0x80,
                                                 0x02, 0x00, 0xE4, 0xA0, 0x01, 0x00, 0x00, 0x02, 0x00, 0x08, 0x0F, 0x80,
                                                 0x00, 0x00, 0xE4, 0x80, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x4C, 0x01, 0x00, 0x00, 0x00, 0x02, 0xFE, 0xFF,
                                                 0xFE, 0xFF, 0x34, 0x00, 0x43, 0x54, 0x41, 0x42, 0x1C, 0x00, 0x00, 0x00,
                                                 0x9B, 0x00, 0x00, 0x00, 0x00, 0x02, 0xFE, 0xFF, 0x01, 0x00, 0x00, 0x00,
                                                 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x94, 0x00, 0x00, 0x00,
                                                 0x30, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
                                                 0x44, 0x00, 0x00, 0x00, 0x54, 0x00, 0x00, 0x00, 0x50, 0x72, 0x6F, 0x6A,
                                                 0x65, 0x63, 0x74, 0x69, 0x6F, 0x6E, 0x4D, 0x61, 0x74, 0x72, 0x69, 0x78,
                                                 0x00, 0xAB, 0xAB, 0xAB, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00, 0x04, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x76, 0x73, 0x5F, 0x32, 0x5F, 0x30, 0x00, 0x4D, 0x69, 0x63, 0x72, 0x6F,
                                                 0x73, 0x6F, 0x66, 0x74, 0x20, 0x28, 0x52, 0x29, 0x20, 0x48, 0x4C, 0x53,
                                                 0x4C, 0x20, 0x53, 0x68, 0x61, 0x64, 0x65, 0x72, 0x20, 0x43, 0x6F, 0x6D,
                                                 0x70, 0x69, 0x6C, 0x65, 0x72, 0x20, 0x39, 0x2E, 0x32, 0x39, 0x2E, 0x39,
                                                 0x35, 0x32, 0x2E, 0x33, 0x31, 0x31, 0x31, 0x00, 0x1F, 0x00, 0x00, 0x02,
                                                 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x0F, 0x90, 0x1F, 0x00, 0x00, 0x02,
                                                 0x0A, 0x00, 0x00, 0x80, 0x01, 0x00, 0x0F, 0x90, 0x09, 0x00, 0x00, 0x03,
                                                 0x00, 0x00, 0x01, 0xC0, 0x00, 0x00, 0xE4, 0x90, 0x00, 0x00, 0xE4, 0xA0,
                                                 0x09, 0x00, 0x00, 0x03, 0x00, 0x00, 0x02, 0xC0, 0x00, 0x00, 0xE4, 0x90,
                                                 0x01, 0x00, 0xE4, 0xA0, 0x09, 0x00, 0x00, 0x03, 0x00, 0x00, 0x04, 0xC0,
                                                 0x00, 0x00, 0xE4, 0x90, 0x02, 0x00, 0xE4, 0xA0, 0x09, 0x00, 0x00, 0x03,
                                                 0x00, 0x00, 0x08, 0xC0, 0x00, 0x00, 0xE4, 0x90, 0x03, 0x00, 0xE4, 0xA0,
                                                 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x0F, 0xD0, 0x01, 0x00, 0xE4, 0x90,
                                                 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x0F, 0xE0, 0x00, 0x00, 0xE4, 0x90,
                                                 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xE0, 0x00, 0x00, 0x00, 0x00, 0x02, 0x58, 0x46, 0xFE, 0xFF, 0x25, 0x00,
                                                 0x43, 0x54, 0x41, 0x42, 0x1C, 0x00, 0x00, 0x00, 0x5F, 0x00, 0x00, 0x00,
                                                 0x00, 0x02, 0x58, 0x46, 0x01, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00,
                                                 0x00, 0x01, 0x00, 0x20, 0x5C, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00,
                                                 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x3C, 0x00, 0x00, 0x00,
                                                 0x4C, 0x00, 0x00, 0x00, 0x7A, 0x45, 0x6E, 0x61, 0x62, 0x6C, 0x65, 0x64,
                                                 0x00, 0xAB, 0xAB, 0xAB, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x01, 0x00,
                                                 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x74, 0x78, 0x00, 0x4D, 0x69, 0x63, 0x72, 0x6F, 0x73, 0x6F, 0x66, 0x74,
                                                 0x20, 0x28, 0x52, 0x29, 0x20, 0x48, 0x4C, 0x53, 0x4C, 0x20, 0x53, 0x68,
                                                 0x61, 0x64, 0x65, 0x72, 0x20, 0x43, 0x6F, 0x6D, 0x70, 0x69, 0x6C, 0x65,
                                                 0x72, 0x20, 0x39, 0x2E, 0x32, 0x39, 0x2E, 0x39, 0x35, 0x32, 0x2E, 0x33,
                                                 0x31, 0x31, 0x31, 0x00, 0xFE, 0xFF, 0x02, 0x00, 0x43, 0x4C, 0x49, 0x54,
                                                 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0x0C, 0x00, 0x46, 0x58, 0x4C, 0x43,
                                                 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                 0xF0, 0xF0, 0xF0, 0xF0, 0x0F, 0x0F, 0x0F, 0x0F, 0xFF, 0xFF, 0x00, 0x00
                                             };
                    effect = Effect.FromMemory(Device, compiledEffect, ShaderFlags.None);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
                }

                #endregion

                technique = effect.GetTechnique(0);

                if (initialized)
                {
                    return;
                }

                initialized = true;
                Drawing.OnPreReset += CircleOnPreReset;
                Drawing.OnPreReset += CircleOnPostReset;
                AppDomain.CurrentDomain.DomainUnload += CircleDispose;
            }

            /// <summary>
            ///     Draws the circle.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="radius">The radius.</param>
            /// <param name="color">The color.</param>
            /// <param name="width">The width.</param>
            /// <param name="zDeep">if set to <c>true</c> the circle will be drawn with depth buffering.</param>
            public static void DrawCircle(
                Vector3 position,
                float radius,
                Color color,
                int width = 3,
                bool zDeep = false)
            {
                try
                {
                    if (Device == null || Device.IsDisposed)
                    {
                        return;
                    }

                    if (vertices == null)
                    {
                        CreateVertexes();
                    }

                    if (vertices == null || vertices.IsDisposed || vertexDeclaration.IsDisposed || effect.IsDisposed
                        || technique.IsDisposed)
                    {
                        return;
                    }

                    position = new Vector3(position.X, position.Z, position.Y);
                    var olddec = Device.VertexDeclaration;

                    effect.Technique = technique;

                    effect.Begin();
                    effect.BeginPass(0);
                    effect.SetValue(
                        "ProjectionMatrix",
                        Matrix.Translation(position) * Drawing.View * Drawing.Projection);
                    effect.SetValue(
                        "CircleColor",
                        new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
                    effect.SetValue("Radius", radius);
                    effect.SetValue("Border", 2f + width);
                    effect.SetValue("zEnabled", zDeep);

                    Device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() * 2);
                    Device.VertexDeclaration = vertexDeclaration;

                    Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

                    effect.EndPass();
                    effect.End();

                    Device.VertexDeclaration = olddec;
                }
                catch (Exception e)
                {
                    vertices = null;
                    Logging.Write()(LogLevel.Error, "SDK.Render.Circle.DrawCircle: " + e);
                }
            }

            /// <summary>
            ///     Called when the circle is drawn.
            /// </summary>
            public override void OnDraw()
            {
                try
                {
                    if (this.Unit != null && this.Unit.IsValid)
                    {
                        DrawCircle(this.Unit.Position + offset, this.Radius, this.Color, this.Width, this.ZDeep);
                    }
                    else if ((this.Position + offset).IsValid())
                    {
                        DrawCircle(this.Position + offset, this.Radius, this.Color, this.Width, this.ZDeep);
                    }
                }
                catch (Exception e)
                {
                    Logging.Write()(LogLevel.Error, "SDK.Render.Circle.OnEndScene: " + e);
                }
            }

            #endregion

            #region Methods

            private static void CircleDispose(object sender, EventArgs e)
            {
                CircleOnPreReset(EventArgs.Empty);

                if (effect != null && !effect.IsDisposed)
                {
                    effect.Dispose();
                }

                if (vertices != null && !vertices.IsDisposed)
                {
                    vertices.Dispose();
                }

                if (vertexDeclaration != null && !vertexDeclaration.IsDisposed)
                {
                    vertexDeclaration.Dispose();
                }
            }

            private static void CircleOnPostReset(EventArgs args)
            {
                if (effect != null && !effect.IsDisposed)
                {
                    effect.OnResetDevice();
                }
            }

            private static void CircleOnPreReset(EventArgs args)
            {
                if (effect != null && !effect.IsDisposed)
                {
                    effect.OnLostDevice();
                }
            }

            #endregion
        }

        /// <summary>
        ///     Draws lines.
        /// </summary>
        public class Line : RenderObject
        {
            #region Fields

            /// <summary>
            ///     The color
            /// </summary>
            public ColorBGRA Color;

            private readonly SharpDX.Direct3D9.Line line;

            private int width;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Line" /> class.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="width">The width.</param>
            /// <param name="color">The color.</param>
            public Line(Vector2 start, Vector2 end, int width, ColorBGRA color)
            {
                this.line = new SharpDX.Direct3D9.Line(Device);
                this.Width = width;
                this.Color = color;
                this.Start = start;
                this.End = end;
                Game.OnUpdate += this.LineOnUpdate;
                this.SubscribeToResetEvents();
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Delegate to get the position of the line.
            /// </summary>
            /// <returns>Vector2.</returns>
            public delegate Vector2 PositionDelegate();

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the end.
            /// </summary>
            /// <value>The end.</value>
            public Vector2 End { get; set; }

            /// <summary>
            ///     Gets or sets the delegate that gets the end position.
            /// </summary>
            /// <value>The end position update.</value>
            public PositionDelegate EndPositionUpdate { get; set; }

            /// <summary>
            ///     Gets or sets the start.
            /// </summary>
            /// <value>The start.</value>
            public Vector2 Start { get; set; }

            /// <summary>
            ///     Gets or sets the delegate that sets the start position.
            /// </summary>
            /// <value>The start position update.</value>
            public PositionDelegate StartPositionUpdate { get; set; }

            /// <summary>
            ///     Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width
            {
                get
                {
                    return this.width;
                }
                set
                {
                    this.line.Width = value;
                    this.width = value;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public override void Dispose()
            {
                this.OnPreReset();

                if (!this.line.IsDisposed)
                {
                    this.line.Dispose();
                }
                Game.OnUpdate -= this.LineOnUpdate;
            }

            /// <summary>
            ///     Called when the scene has ended.
            /// </summary>
            public override void OnEndScene()
            {
                try
                {
                    if (this.line.IsDisposed)
                    {
                        return;
                    }

                    this.line.Begin();
                    this.line.Draw(new[] { this.Start, this.End }, this.Color);
                    this.line.End();
                }
                catch (Exception e)
                {
                    Logging.Write()(LogLevel.Error, "SDK.Render.Line.OnEndScene: " + e);
                }
            }

            /// <summary>
            ///     Called after the DirectX is reset.
            /// </summary>
            public override void OnPostReset()
            {
                this.line.OnResetDevice();
            }

            /// <summary>
            ///     Called before the DirectX device is reset.
            /// </summary>
            public override void OnPreReset()
            {
                this.line.OnLostDevice();
            }

            #endregion

            #region Methods

            private void LineOnUpdate(EventArgs args)
            {
                if (this.StartPositionUpdate != null)
                {
                    this.Start = this.StartPositionUpdate();
                }

                if (this.EndPositionUpdate != null)
                {
                    this.End = this.EndPositionUpdate();
                }
            }

            #endregion
        }

        /// <summary>
        ///     Draws a Rectangle.
        /// </summary>
        public class Rectangle : RenderObject
        {
            #region Fields

            /// <summary>
            ///     The color of the rectangle
            /// </summary>
            public ColorBGRA Color;

            private readonly SharpDX.Direct3D9.Line line;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Rectangle" /> class.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="color">The color.</param>
            public Rectangle(int x, int y, int width, int height, ColorBGRA color)
            {
                this.X = x;
                this.Y = y;
                this.Width = width;
                this.Height = height;
                this.Color = color;
                this.line = new SharpDX.Direct3D9.Line(Device) { Width = height };
                Game.OnUpdate += this.RectangleOnUpdate;
                this.SubscribeToResetEvents();
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Delegate to get the position of the rectangle.
            /// </summary>
            /// <returns>Vector2.</returns>
            public delegate Vector2 PositionDelegate();

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the height.
            /// </summary>
            /// <value>The height.</value>
            public int Height { get; set; }

            /// <summary>
            ///     Gets or sets the delegate that gets the position.
            /// </summary>
            /// <value>The position update.</value>
            public PositionDelegate PositionUpdate { get; set; }

            /// <summary>
            ///     Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width { get; set; }

            /// <summary>
            ///     Gets or sets the x.
            /// </summary>
            /// <value>The x.</value>
            public int X { get; set; }

            /// <summary>
            ///     Gets or sets the y.
            /// </summary>
            /// <value>The y.</value>
            public int Y { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public override void Dispose()
            {
                this.OnPreReset();

                if (!this.line.IsDisposed)
                {
                    this.line.Dispose();
                }
                Game.OnUpdate -= this.RectangleOnUpdate;
            }

            /// <summary>
            ///     Called when [end scene].
            /// </summary>
            public override void OnEndScene()
            {
                try
                {
                    if (this.line.IsDisposed)
                    {
                        return;
                    }

                    this.line.Begin();
                    this.line.Draw(
                        new[]
                            {
                                new Vector2(this.X, this.Y + this.Height / 2),
                                new Vector2(this.X + this.Width, this.Y + this.Height / 2)
                            },
                        this.Color);
                    this.line.End();
                }
                catch (Exception e)
                {
                    Logging.Write()(LogLevel.Error, "SDK.Render.Rectangle.OnEndScene: " + e);
                }
            }

            /// <summary>
            ///     Called after the DirectX device is reset.
            /// </summary>
            public override void OnPostReset()
            {
                this.line.OnResetDevice();
            }

            /// <summary>
            ///     Called before the DirectX device is reset.
            /// </summary>
            public override void OnPreReset()
            {
                this.line.OnLostDevice();
            }

            #endregion

            #region Methods

            private void RectangleOnUpdate(EventArgs args)
            {
                if (this.PositionUpdate == null)
                {
                    return;
                }

                var pos = this.PositionUpdate();
                this.X = (int)pos.X;
                this.Y = (int)pos.Y;
            }

            #endregion
        }

        /// <summary>
        ///     A base class that renders objects.
        /// </summary>
        public class RenderObject : IDisposable
        {
            #region Fields

            /// <summary>
            ///     The layer
            /// </summary>
            public float Layer;

            /// <summary>
            ///     The visible condition delegate.
            /// </summary>
            public VisibleConditionDelegate VisibleCondition;

            private bool visible = true;

            #endregion

            #region Constructors and Destructors

            ~RenderObject()
            {
                this.OnPreReset();
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Delegate that gets if the object is visible.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <returns><c>true</c> if the object is visible, <c>false</c> otherwise.</returns>
            public delegate bool VisibleConditionDelegate(RenderObject sender);

            #endregion

            #region Public Properties

            /// <summary>
            ///     Determines whether this instace has a valid layer.
            /// </summary>
            /// <returns><c>true</c> if has a valid layer; otherwise, <c>false</c>.</returns>
            public bool HasValidLayer => this.Layer >= -5 && this.Layer <= 5;

            /// <summary>
            ///     Gets or sets a value indicating whether this <see cref="RenderObject" /> is visible.
            /// </summary>
            /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
            public bool Visible
            {
                get
                {
                    return this.VisibleCondition?.Invoke(this) ?? this.visible;
                }
                set
                {
                    this.visible = value;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public virtual void Dispose()
            {
            }

            /// <summary>
            ///     Called when the render object is drawn.
            /// </summary>
            public virtual void OnDraw()
            {
            }

            /// <summary>
            ///     Called when the scene has ended..
            /// </summary>
            public virtual void OnEndScene()
            {
            }

            /// <summary>
            ///     Called after the DirectX device is reset.
            /// </summary>
            public virtual void OnPostReset()
            {
            }

            /// <summary>
            ///     Called before the DirectX device is reset.
            /// </summary>
            public virtual void OnPreReset()
            {
            }

            #endregion

            #region Methods

            internal void SubscribeToResetEvents()
            {
                Drawing.OnPreReset += delegate { this.OnPreReset(); };
                Drawing.OnPostReset += delegate { this.OnPostReset(); };
                AppDomain.CurrentDomain.DomainUnload += delegate { this.OnPreReset(); };
            }

            #endregion
        }

        /// <summary>
        ///     Draws a sprite image.
        /// </summary>
        public class Sprite : RenderObject
        {
            #region Fields

            private readonly SharpDX.Direct3D9.Sprite sprite = new SharpDX.Direct3D9.Sprite(Device);

            private SharpDX.Rectangle? crop;

            private bool hide;

            private Texture originalTexture;

            private Texture texture;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sprite" /> class.
            /// </summary>
            /// <param name="bitmap">The bitmap.</param>
            /// <param name="position">The position.</param>
            public Sprite(Bitmap bitmap, Vector2 position)
                : this()
            {
                this.UpdateTextureBitmap(bitmap, position);
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sprite" /> class.
            /// </summary>
            /// <param name="texture">The texture.</param>
            /// <param name="position">The position.</param>
            public Sprite(BaseTexture texture, Vector2 position)
                : this()
            {
                this.UpdateTextureBitmap(
                    (Bitmap)Image.FromStream(BaseTexture.ToStream(texture, ImageFileFormat.Bmp)),
                    position);
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sprite" /> class.
            /// </summary>
            /// <param name="stream">The stream.</param>
            /// <param name="position">The position.</param>
            public Sprite(Stream stream, Vector2 position)
                : this()
            {
                this.UpdateTextureBitmap(new Bitmap(stream), position);
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sprite" /> class.
            /// </summary>
            /// <param name="bytesArray">The bytes array.</param>
            /// <param name="position">The position.</param>
            public Sprite(byte[] bytesArray, Vector2 position)
                : this()
            {
                this.UpdateTextureBitmap((Bitmap)Image.FromStream(new MemoryStream(bytesArray)), position);
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sprite" /> class.
            /// </summary>
            /// <param name="fileLocation">The file location.</param>
            /// <param name="position">The position.</param>
            public Sprite(string fileLocation, Vector2 position)
                : this()
            {
                if (!File.Exists((fileLocation)))
                {
                    return;
                }

                this.UpdateTextureBitmap(new Bitmap(fileLocation), position);
            }

            private Sprite()
            {
                Game.OnUpdate += this.SpriteOnUpdate;
                this.SubscribeToResetEvents();
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Delegate for when the sprite is reset.
            /// </summary>
            /// <param name="sprite">The sprite.</param>
            public delegate void OnResetting(Sprite sprite);

            /// <summary>
            ///     Delegate that gets the position of the sprite.
            /// </summary>
            /// <returns>Vector2.</returns>
            public delegate Vector2 PositionDelegate();

            #endregion

            #region Public Events

            /// <summary>
            ///     Occurs when the sprite is reset.
            /// </summary>
            public event OnResetting OnReset;

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the bitmap.
            /// </summary>
            /// <value>The bitmap.</value>
            public Bitmap Bitmap { get; set; }

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            /// <value>The color.</value>
            public ColorBGRA Color { set; get; } = SharpDX.Color.White;

            /// <summary>
            ///     Gets the height.
            /// </summary>
            /// <value>The height.</value>
            public int Height => (int)(this.Bitmap.Height * this.Scale.Y);

            /// <summary>
            ///     Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector2 Position
            {
                set
                {
                    this.X = (int)value.X;
                    this.Y = (int)value.Y;
                }
                get
                {
                    return new Vector2(this.X, this.Y);
                }
            }

            /// <summary>
            ///     Gets or sets the delegate that gets the position.
            /// </summary>
            /// <value>The position update.</value>
            public PositionDelegate PositionUpdate { get; set; }

            /// <summary>
            ///     Gets or sets the rotation.
            /// </summary>
            /// <value>The rotation.</value>
            public float Rotation { set; get; }

            /// <summary>
            ///     Gets or sets the scale.
            /// </summary>
            /// <value>The scale.</value>
            public Vector2 Scale { set; get; } = new Vector2(1, 1);

            /// <summary>
            ///     Gets the size.
            /// </summary>
            /// <value>The size.</value>
            public Vector2 Size => new Vector2(this.Bitmap.Width, this.Bitmap.Height);

            /// <summary>
            ///     Gets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width => (int)(this.Bitmap.Width * this.Scale.X);

            /// <summary>
            ///     Gets or sets the x.
            /// </summary>
            /// <value>The x.</value>
            public int X { get; set; }

            /// <summary>
            ///     Gets or sets the y.
            /// </summary>
            /// <value>The y.</value>
            public int Y { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Complements this instance.
            /// </summary>
            public void Complement()
            {
                this.SetSaturation(-1.0f);
            }

            /// <summary>
            ///     Crops the sprite.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="w">The width.</param>
            /// <param name="h">The height.</param>
            /// <param name="scale">if set to <c>true</c>, crops with the scale.</param>
            public void Crop(int x, int y, int w, int h, bool scale = false)
            {
                this.crop = new SharpDX.Rectangle(x, y, w, h);

                if (scale)
                {
                    this.crop = new SharpDX.Rectangle(
                        (int)(this.Scale.X * x),
                        (int)(this.Scale.Y * y),
                        (int)(this.Scale.X * w),
                        (int)(this.Scale.Y * h));
                }
            }

            /// <summary>
            ///     Crops the sprite.
            /// </summary>
            /// <param name="rect">The rectangle.</param>
            /// <param name="scale">if set to <c>true</c>, crops with the scale.</param>
            public void Crop(SharpDX.Rectangle rect, bool scale = false)
            {
                this.crop = rect;

                if (scale)
                {
                    this.crop = new SharpDX.Rectangle(
                        (int)(this.Scale.X * rect.X),
                        (int)(this.Scale.Y * rect.Y),
                        (int)(this.Scale.X * rect.Width),
                        (int)(this.Scale.Y * rect.Height));
                }
            }

            /// <summary>
            ///     Disposes this instance.
            /// </summary>
            public override void Dispose()
            {
                this.OnPreReset();
                Game.OnUpdate -= this.SpriteOnUpdate;

                if (!this.sprite.IsDisposed)
                {
                    this.sprite.Dispose();
                }

                if (!this.texture.IsDisposed)
                {
                    this.texture.Dispose();
                }

                if (!this.originalTexture.IsDisposed)
                {
                    this.originalTexture.Dispose();
                }
            }

            /// <summary>
            ///     Fades this instance. (Saturation is 1/2)
            /// </summary>
            public void Fade()
            {
                this.SetSaturation(0.5f);
            }

            /// <summary>
            ///     Makes the sprite black and white.
            /// </summary>
            public void GrayScale()
            {
                this.SetSaturation(0.0f);
            }

            /// <summary>
            ///     Hides this instance.
            /// </summary>
            public void Hide()
            {
                this.hide = true;
            }

            /// <summary>
            ///     Called when the scene has ended.
            /// </summary>
            public override void OnEndScene()
            {
                try
                {
                    if (this.sprite.IsDisposed || this.texture.IsDisposed || !this.Position.IsValid() || this.hide)
                    {
                        return;
                    }

                    this.sprite.Begin();
                    var matrix = this.sprite.Transform;
                    var nMatrix = (Matrix.Scaling(this.Scale.X, this.Scale.Y, 0)) * Matrix.RotationZ(this.Rotation)
                                  * Matrix.Translation(this.Position.X, this.Position.Y, 0);
                    this.sprite.Transform = nMatrix;
                    this.sprite.Draw(this.texture, this.Color, this.crop);
                    this.sprite.Transform = matrix;
                    this.sprite.End();
                }
                catch (Exception e)
                {
                    this.Reset();
                    Logging.Write()(LogLevel.Error, "SDK.Render.Sprite.OnEndScene: " + e);
                }
            }

            /// <summary>
            ///     Called after the DirectX device is reset.
            /// </summary>
            public override void OnPostReset()
            {
                if (this.sprite != null && !this.sprite.IsDisposed)
                {
                    this.sprite?.OnResetDevice();
                }
            }

            /// <summary>
            ///     Called before the DirectX device is reset..
            /// </summary>
            public override void OnPreReset()
            {
                if (this.sprite != null && !this.sprite.IsDisposed)
                {
                    this.sprite?.OnLostDevice();
                }
            }

            /// <summary>
            ///     Resets this instance.
            /// </summary>
            public void Reset()
            {
                this.UpdateTextureBitmap(
                    (Bitmap)Image.FromStream(BaseTexture.ToStream(this.originalTexture, ImageFileFormat.Bmp)));
                this.OnReset?.Invoke(this);
            }

            /// <summary>
            ///     Sets the saturation.
            /// </summary>
            /// <param name="saturiation">The saturiation.</param>
            public void SetSaturation(float saturiation)
            {
                this.UpdateTextureBitmap(SaturateBitmap(this.Bitmap, saturiation));
            }

            /// <summary>
            ///     Shows this instance.
            /// </summary>
            public void Show()
            {
                this.hide = false;
            }

            /// <summary>
            ///     Updates the texture bitmap.
            /// </summary>
            /// <param name="newBitmap">The new bitmap.</param>
            /// <param name="position">The position.</param>
            public void UpdateTextureBitmap(Bitmap newBitmap, Vector2 position = new Vector2())
            {
                if (position.IsValid())
                {
                    this.Position = position;
                }

                this.Bitmap?.Dispose();
                this.Bitmap = newBitmap;

                this.texture = Texture.FromMemory(
                    Device,
                    (byte[])new ImageConverter().ConvertTo(newBitmap, typeof(byte[])),
                    this.Width,
                    this.Height,
                    0,
                    Usage.None,
                    Format.A1,
                    Pool.Managed,
                    Filter.Default,
                    Filter.Default,
                    0);

                if (this.originalTexture == null)
                {
                    this.originalTexture = this.texture;
                }
            }

            #endregion

            #region Methods

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

            private void SpriteOnUpdate(EventArgs args)
            {
                if (this.PositionUpdate == null)
                {
                    return;
                }

                var pos = this.PositionUpdate();
                this.X = (int)pos.X;
                this.Y = (int)pos.Y;
            }

            #endregion
        }

        /// <summary>
        ///     Object used to draw text on the screen.
        /// </summary>
        public class Text : RenderObject
        {
            #region Fields

            /// <summary>
            ///     <c>true</c> if the text should be centered at the position.
            /// </summary>
            public bool Centered = false;

            /// <summary>
            ///     The offset
            /// </summary>
            public Vector2 Offset;

            /// <summary>
            ///     <c>true</c> if the text should have an outline.
            /// </summary>
            public bool OutLined = false;

            /// <summary>
            ///     The delegate that updates the position of the text.
            /// </summary>
            public PositionDelegate PositionUpdate;

            /// <summary>
            ///     The delegate that updates the text.
            /// </summary>
            public TextDelegate TextUpdate;

            /// <summary>
            ///     The unit
            /// </summary>
            public Obj_AI_Base Unit;

            private string text;

            private Font textFont;

            private int x;

            private int xCalculated;

            private int y;

            private int yCalculated;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="Text" /> class.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="size">The size.</param>
            /// <param name="color">The color.</param>
            /// <param name="fontName">Name of the font.</param>
            public Text(string text, int x, int y, int size, ColorBGRA color, string fontName = "Calibri")
                : this(text, fontName, size, color)
            {
                this.x = x;
                this.y = y;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Text" /> class.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="position">The position.</param>
            /// <param name="size">The size.</param>
            /// <param name="color">The color.</param>
            /// <param name="fontName">Name of the font.</param>
            public Text(string text, Vector2 position, int size, ColorBGRA color, string fontName = "Calibri")
                : this(text, fontName, size, color)
            {
                this.x = (int)position.X;
                this.y = (int)position.Y;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Text" /> class.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="unit">The unit.</param>
            /// <param name="offset">The offset.</param>
            /// <param name="size">The size.</param>
            /// <param name="color">The color.</param>
            /// <param name="fontName">Name of the font.</param>
            public Text(
                string text,
                Obj_AI_Base unit,
                Vector2 offset,
                int size,
                ColorBGRA color,
                string fontName = "Calibri")
                : this(text, fontName, size, color)
            {
                this.Unit = unit;
                this.Offset = offset;

                var pos = unit.HPBarPosition + offset;
                this.x = (int)pos.X;
                this.y = (int)pos.Y;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Text" /> class.
            /// </summary>
            /// <param name="x">The x.</param>
            /// <param name="y">The y.</param>
            /// <param name="text">The text.</param>
            /// <param name="size">The size.</param>
            /// <param name="color">The color.</param>
            /// <param name="fontName">Name of the font.</param>
            public Text(int x, int y, string text, int size, ColorBGRA color, string fontName = "Calibri")
                : this(text, fontName, size, color)
            {
                this.x = x;
                this.y = y;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Text" /> class.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="text">The text.</param>
            /// <param name="size">The size.</param>
            /// <param name="color">The color.</param>
            /// <param name="fontName">Name of the font.</param>
            public Text(Vector2 position, string text, int size, ColorBGRA color, string fontName = "Calibri")
                : this(text, fontName, size, color)
            {
                this.x = (int)position.X;
                this.y = (int)position.Y;
            }

            private Text(string text, string fontName, int size, ColorBGRA color)
            {
                this.textFont = new Font(
                    Device,
                    new FontDescription
                        {
                            FaceName = fontName, Height = size, OutputPrecision = FontPrecision.Default,
                            Quality = FontQuality.Default
                        });
                this.Color = color;
                this.TextString = text;
                Game.OnUpdate += this.TextOnUpdate;
                this.SubscribeToResetEvents();
            }

            #endregion

            #region Delegates

            /// <summary>
            ///     Delegate that gets the position of the text.
            /// </summary>
            /// <returns>Vector2.</returns>
            public delegate Vector2 PositionDelegate();

            /// <summary>
            ///     Delegate that gets the text.
            /// </summary>
            /// <returns>System.String.</returns>
            public delegate string TextDelegate();

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the color.
            /// </summary>
            /// <value>The color.</value>
            public ColorBGRA Color { get; set; }

            /// <summary>
            ///     Gets the height.
            /// </summary>
            /// <value>The height.</value>
            public int Height { get; private set; }

            /// <summary>
            ///     Gets or sets the text font description.
            /// </summary>
            /// <value>The text font description.</value>
            public FontDescription TextFontDescription
            {
                get
                {
                    return this.textFont.Description;
                }
                set
                {
                    this.textFont.Dispose();
                    this.textFont = new Font(Device, value);
                }
            }

            /// <summary>
            ///     Gets or sets the text.
            /// </summary>
            /// <value>The text.</value>
            public string TextString
            {
                get
                {
                    return this.text;
                }
                set
                {
                    if (value != this.text && this.textFont != null && !this.textFont.IsDisposed
                        && !string.IsNullOrEmpty(value))
                    {
                        var size = this.textFont.MeasureText(null, value, 0);
                        this.Width = size.Width;
                        this.Height = size.Height;
                        this.textFont.PreloadText(value);
                    }
                    this.text = value;
                }
            }

            /// <summary>
            ///     Gets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width { get; private set; }

            /// <summary>
            ///     Gets or sets the x.
            /// </summary>
            /// <value>The x.</value>
            public int X
            {
                get
                {
                    return this.PositionUpdate != null ? this.xCalculated : this.x + this.XOffset;
                }
                set
                {
                    this.x = value;
                }
            }

            /// <summary>
            ///     Gets or sets the y.
            /// </summary>
            /// <value>The y.</value>
            public int Y
            {
                get
                {
                    return this.PositionUpdate != null ? this.yCalculated : this.y + this.YOffset;
                }
                set
                {
                    this.y = value;
                }
            }

            #endregion

            #region Properties

            private int XOffset => this.Centered ? -this.Width / 2 : 0;

            private int YOffset => this.Centered ? -this.Height / 2 : 0;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Disposes this instance.
            /// </summary>
            public override void Dispose()
            {
                Game.OnUpdate -= this.TextOnUpdate;
                this.OnPreReset();

                if (!this.textFont.IsDisposed)
                {
                    this.textFont.Dispose();
                }
            }

            /// <summary>
            ///     Called when the scene has ended.
            /// </summary>
            public override void OnEndScene()
            {
                try
                {
                    if (this.textFont.IsDisposed || this.TextString == "")
                    {
                        return;
                    }

                    if (this.Unit != null && this.Unit.IsValid)
                    {
                        var pos = this.Unit.HPBarPosition + this.Offset;
                        this.X = (int)pos.X;
                        this.Y = (int)pos.Y;
                    }

                    var xP = this.X;
                    var yP = this.Y;
                    if (this.OutLined)
                    {
                        var outlineColor = new ColorBGRA(0, 0, 0, 255);
                        this.textFont.DrawText(null, this.TextString, xP - 1, yP - 1, outlineColor);
                        this.textFont.DrawText(null, this.TextString, xP + 1, yP + 1, outlineColor);
                        this.textFont.DrawText(null, this.TextString, xP - 1, yP, outlineColor);
                        this.textFont.DrawText(null, this.TextString, xP + 1, yP, outlineColor);
                    }
                    this.textFont.DrawText(null, this.TextString, xP, yP, this.Color);
                }
                catch (Exception e)
                {
                    Logging.Write()(LogLevel.Error, "SDK.Render.Text.OnEndScene: " + e);
                }
            }

            /// <summary>
            ///     Called after the DirectX device has been reset.
            /// </summary>
            public override void OnPostReset()
            {
                this.textFont.OnResetDevice();
            }

            /// <summary>
            ///     Called before the DirectX device is reset.
            /// </summary>
            public override void OnPreReset()
            {
                this.textFont.OnLostDevice();
            }

            #endregion

            #region Methods

            private void TextOnUpdate(EventArgs args)
            {
                if (!this.Visible)
                {
                    return;
                }

                if (this.TextUpdate != null)
                {
                    this.TextString = this.TextUpdate();
                }

                if (this.PositionUpdate != null && !string.IsNullOrEmpty(this.TextString))
                {
                    var pos = this.PositionUpdate();
                    this.xCalculated = (int)pos.X + this.XOffset;
                    this.yCalculated = (int)pos.Y + this.YOffset;
                }
            }

            #endregion
        }
    }
}