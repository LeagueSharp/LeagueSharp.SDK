// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuSettings.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Default Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDKEx.UI.Skins
{
    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class MenuSettings
    {
        #region Static Fields

        /// <summary>
        ///     Local Font.
        /// </summary>
        private static Font font;

        private static Vector2 position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="MenuSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static MenuSettings()
        {
            ContainerHeight = 30;
            ContainerSelectedColor = new ColorBGRA(255, 255, 255, 255 / 2);
            ContainerSeparatorColor = new ColorBGRA(255, 255, 255, 100);
            Position = new Vector2(30, 30);
            ContainerWidth = 200f;
            Font = new Font(
                Drawing.Direct3DDevice,
                14,
                0,
                FontWeight.DoNotCare,
                0,
                false,
                FontCharacterSet.Default,
                FontPrecision.TrueType,
                FontQuality.ClearTypeNatural,
                FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative | FontPitchAndFamily.Modern,
                "Tahoma");
            ContainerTextMarkOffset = 8f;
            ContainerTextOffset = 15f;
            HoverColor = new ColorBGRA(255, 255, 255, 50);
            RootContainerColor = new ColorBGRA(0, 0, 0, (byte)(255 / 1.5f));
            TextColor = Color.White;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Container Height.
        /// </summary>
        public static int ContainerHeight { get; set; }

        /// <summary>
        ///     Gets or sets the Global Container Selected Color.
        /// </summary>
        public static ColorBGRA ContainerSelectedColor { get; set; }

        /// <summary>
        ///     Gets or sets the Container Line Separator Color.
        /// </summary>
        public static ColorBGRA ContainerSeparatorColor { get; set; }

        /// <summary>
        ///     Gets or sets the Container Text Marking Offset.
        /// </summary>
        public static float ContainerTextMarkOffset { get; set; }

        /// <summary>
        ///     Gets or sets the Global Container Text Mark Width.
        /// </summary>
        public static float ContainerTextMarkWidth { get; set; }

        /// <summary>
        ///     Gets or sets the Container Text Addition Offset.
        /// </summary>
        public static float ContainerTextOffset { get; set; }

        /// <summary>
        ///     Gets or sets the Global Container Width.
        /// </summary>
        public static float ContainerWidth { get; set; }

        /// <summary>
        ///     Gets or sets the Global Font.
        /// </summary>
        public static Font Font
        {
            get
            {
                return font;
            }

            set
            {
                font = value;
                ContainerTextMarkWidth = value.MeasureText(null, "»", 0).Width;
            }
        }

        /// <summary>
        ///     Gets or sets the color of an item when the user is hovering over it.
        /// </summary>
        public static ColorBGRA HoverColor { get; set; }

        /// <summary>
        ///     Gets or sets the default menu zero-position.
        /// </summary>
        public static Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                if (MenuCustomizer.Instance != null)
                {
                    position = value;
                    MenuCustomizer.Instance.PositionX.Value = (int)position.X;
                    MenuCustomizer.Instance.PositionY.Value = (int)position.Y;
                }
                else
                {
                    position = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the Root Container Color.
        /// </summary>
        public static ColorBGRA RootContainerColor { get; set; }

        /// <summary>
        ///     Gets or sets the Global Text Color.
        /// </summary>
        public static ColorBGRA TextColor { get; set; }

        #endregion
    }
}