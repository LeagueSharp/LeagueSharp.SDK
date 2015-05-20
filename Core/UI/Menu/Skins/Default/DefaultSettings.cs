// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSettings.cs" company="LeagueSharp">
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
namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class DefaultSettings
    {
        #region Static Fields

        /// <summary>
        ///     Local Global Container Height.
        /// </summary>
        private static int containerHeight = 30;

        /// <summary>
        ///     Local Single Container Line.
        /// </summary>
        private static Line containerLine = new Line(Drawing.Direct3DDevice)
                                                {
                                                   Antialias = false, GLLines = true, Width = 200f 
                                                };

        /// <summary>
        ///     Local Global Container Selected Color.
        /// </summary>
        private static ColorBGRA containerSelectedColor = new ColorBGRA(255, 255, 255, 255 / 2);

        /// <summary>
        ///     Container Line Separator Color.
        /// </summary>
        private static ColorBGRA containerSeparatorColor = new ColorBGRA(255, 255, 255, 100);

        /// <summary>
        ///     Global Container Separator Line.
        /// </summary>
        private static Line containerSeparatorLine = new Line(Drawing.Direct3DDevice)
                                                         {
                                                            Antialias = true, GLLines = true, Width = 1f 
                                                         };

        /// <summary>
        ///     Container Text Marking Offset.
        /// </summary>
        private static float containerTextMarkOffset = 8f;

        /// <summary>
        ///     Container Text Addition Offset.
        /// </summary>
        private static float containerTextOffset = 15f;

        /// <summary>
        ///     Local Container Width.
        /// </summary>
        private static float containerWidth;

        /// <summary>
        ///     Local Font.
        /// </summary>
        private static Font font;

        /// <summary>
        ///     The color of an item when the user is hovering over it.
        /// </summary>
        private static ColorBGRA hoverColor = new ColorBGRA(255, 255, 255, 50);

        /// <summary>
        ///     Local hover line.
        /// </summary>
        private static Line hoverLine = new Line(Drawing.Direct3DDevice)
                                            {
                                               Antialias = false, GLLines = true, Width = ContainerHeight 
                                            };

        /// <summary>
        ///     Root Container Color.
        /// </summary>
        private static ColorBGRA rootContainerColor = new ColorBGRA(0, 0, 0, (byte)(255 / 1.5f));

        /// <summary>
        ///     Local global text color.
        /// </summary>
        private static ColorBGRA textColor = Color.White;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="DefaultSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static DefaultSettings()
        {
            ContainerWidth = 200f;
            Font = Constants.LeagueSharpFont;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Container Height.
        /// </summary>
        public static int ContainerHeight
        {
            get
            {
                return containerHeight;
            }

            set
            {
                containerHeight = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Single Container Line.
        /// </summary>
        public static Line ContainerLine
        {
            get
            {
                return containerLine;
            }

            set
            {
                containerLine = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Container Selected Color.
        /// </summary>
        public static ColorBGRA ContainerSelectedColor
        {
            get
            {
                return containerSelectedColor;
            }

            set
            {
                containerSelectedColor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Container Line Separator Color.
        /// </summary>
        public static ColorBGRA ContainerSeparatorColor
        {
            get
            {
                return containerSeparatorColor;
            }

            set
            {
                containerSeparatorColor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Container Separator Line.
        /// </summary>
        public static Line ContainerSeparatorLine
        {
            get
            {
                return containerSeparatorLine;
            }

            set
            {
                containerSeparatorLine = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Container Text Marking Offset.
        /// </summary>
        public static float ContainerTextMarkOffset
        {
            get
            {
                return containerTextMarkOffset;
            }

            set
            {
                containerTextMarkOffset = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Container Text Mark Width.
        /// </summary>
        public static float ContainerTextMarkWidth { get; set; }

        /// <summary>
        ///     Gets or sets the Container Text Addition Offset.
        /// </summary>
        public static float ContainerTextOffset
        {
            get
            {
                return containerTextOffset;
            }

            set
            {
                containerTextOffset = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Container Width.
        /// </summary>
        public static float ContainerWidth
        {
            get
            {
                return containerWidth;
            }

            set
            {
                containerWidth = value;
                if (ContainerLine != null)
                {
                    ContainerLine.Width = value;
                }
            }
        }

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
        public static ColorBGRA HoverColor
        {
            get
            {
                return hoverColor;
            }

            set
            {
                hoverColor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the hover line
        /// </summary>
        public static Line HoverLine
        {
            get
            {
                return hoverLine;
            }

            set
            {
                hoverLine = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Root Container Color.
        /// </summary>
        public static ColorBGRA RootContainerColor
        {
            get
            {
                return rootContainerColor;
            }

            set
            {
                rootContainerColor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Text Color.
        /// </summary>
        public static ColorBGRA TextColor
        {
            get
            {
                return textColor;
            }

            set
            {
                textColor = value;
            }
        }

        #endregion
    }
}