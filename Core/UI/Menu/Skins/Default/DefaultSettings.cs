using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class DefaultSettings
    {
        /// <summary>
        ///     Local Container Width.
        /// </summary>
        private static float _containerWidth;

        /// <summary>
        ///     Local Font.
        /// </summary>
        private static Font _font;

        /// <summary>
        ///     Global Container Height.
        /// </summary>
        public static float ContainerHeight = 30f;

        /// <summary>
        ///     Global Text Color.
        /// </summary>
        public static ColorBGRA TextColor = Color.White;

        /// <summary>
        ///     Root Container Color.
        /// </summary>
        public static ColorBGRA RootContainerColor = new ColorBGRA(0, 0, 0, (byte) (255 / 1.5f));

        /// <summary>
        ///     Container Line Seperator Color.
        /// </summary>
        public static ColorBGRA ContainerSeperatorColor = Color.White;

        public static ColorBGRA HoverColor = new ColorBGRA(255, 255, 255, 50);

        /// <summary>
        ///     Container Text Addition Offset.
        /// </summary>
        public static float ContainerTextOffset = 15f;

        /// <summary>
        ///     Container Text Marking Offset.
        /// </summary>
        public static float ContainerTextMarkOffset = -8f;

        /// <summary>
        ///     Single Container Line.
        /// </summary>
        public static Line ContainerLine = new Line(Drawing.Direct3DDevice)
        {
            Antialias = false,
            GLLines = true,
            Width = 200f
        };

        public static Line HoverLine = new Line(Drawing.Direct3DDevice)
        {
            Antialias = false,
            GLLines = true,
            Width = ContainerHeight
        };

        /// <summary>
        ///     Global Container Selected Color.
        /// </summary>
        public static ColorBGRA ContainerSelectedColor = new ColorBGRA(255, 255, 255, 255 / 2);

        /// <summary>
        ///     Global Container Seperator Line.
        /// </summary>
        public static Line ContainerSeperatorLine = new Line(Drawing.Direct3DDevice)
        {
            Antialias = true,
            GLLines = true,
            Width = 1f
        };

        /// <summary>
        ///     Default Settings Static Constructor.
        /// </summary>
        static DefaultSettings()
        {
            ContainerWidth = 200f;
            Font = Constants.LeagueSharpFont;
        }

        /// <summary>
        ///     Global Container Width.
        /// </summary>
        public static float ContainerWidth
        {
            get { return _containerWidth; }
            set
            {
                _containerWidth = value;
                if (ContainerLine != null)
                {
                    ContainerLine.Width = value;
                }
            }
        }

        /// <summary>
        ///     Global Container Text Mark Width.
        /// </summary>
        public static float ContainerTextMarkWidth { get; set; }

        /// <summary>
        ///     Global Font.
        /// </summary>
        public static Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                ContainerTextMarkWidth = value.MeasureText(null, "»", 0).Width;
            }
        }
    }
}