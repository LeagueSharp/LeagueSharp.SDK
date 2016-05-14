// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightMenuSettings2.cs" company="LeagueSharp">
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
//   Light 2 Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//Concept by User Vasconcellos

namespace LeagueSharp.SDK.UI.Skins.Light2
{
    using LeagueSharp.SDK.UI.Skins.Light;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class LightMenuSettings2 : MenuSettings
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="MenuSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static LightMenuSettings2()
        {
            RootContainerColor = new ColorBGRA(255, 255, 255, 255);
            ContainerSeparatorColor = new ColorBGRA(210, 210, 210, 255);
            ContainerSelectedColor = new ColorBGRA(1, 149, 255, 255);

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

            TextColor = new ColorBGRA(0, 0, 0, 255);
            TextCaptionColor = new ColorBGRA(40, 40, 40, 255);
            KeyBindColor = new ColorBGRA(67, 159, 255, 255);
            SliderColor = new ColorBGRA(163, 202, 241, 255);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Caption Font.
        /// </summary>
        public static Font FontCaption { get; set; }

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
            LightMenuSettings.LoadSettings();
        }

        #endregion
    }
}