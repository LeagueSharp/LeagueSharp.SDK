// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueMenuSettings.cs" company="LeagueSharp">
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
//   Blue Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//Concept by User Vasconcellos

namespace LeagueSharp.SDK.UI.Skins.Blue
{
    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class BlueMenuSettings : MenuSettings
    {
        #region Static Fields

        /// <summary>
        ///     Local Caption Font.
        /// </summary>
        private static Font fontCaption;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="MenuSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static BlueMenuSettings()
        {
            RootContainerColor = new ColorBGRA(0, 0, 0, 255);
            ContainerSeparatorColor = new ColorBGRA(24, 24, 24, 255);
            ContainerSelectedColor = new ColorBGRA(21, 21, 21, 255);

            FontCaption = new Font(
                Drawing.Direct3DDevice,
                14,
                0,
                FontWeight.DoNotCare,
                0,
                true,
                FontCharacterSet.Default,
                FontPrecision.TrueType,
                FontQuality.ClearType,
                FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative | FontPitchAndFamily.Modern,
                "Tahoma");

            TextCaptionColor = new ColorBGRA(0, 185, 252, 255);
            KeyBindColor = new ColorBGRA(5, 168, 235, 255);
            SliderColor = new ColorBGRA(0, 75, 101, 255);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Caption Font.
        /// </summary>
        public static Font FontCaption
        {
            get
            {
                return fontCaption;
            }

            set
            {
                fontCaption = value;
                ContainerTextMarkWidth = value.MeasureText(null, "»", 0).Width;
            }
        }

        /// <summary>
        ///     Gets or sets the Global KeyBind Color.
        /// </summary>
        public static ColorBGRA KeyBindColor { get; set; }

        /// <summary>
        ///     Gets or sets the Global Slider Color.
        /// </summary>
        public static ColorBGRA SliderColor { get; set; }

        /// <summary>
        ///     Gets or sets the Global Text Caption Color.
        /// </summary>
        public static ColorBGRA TextCaptionColor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Used to load the menu settings.
        /// </summary>
        public static void LoadSettings()
        {
        }

        #endregion
    }
}