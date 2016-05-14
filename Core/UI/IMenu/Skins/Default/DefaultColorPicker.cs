// <copyright file="DefaultColorPicker.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.UI.Skins.Default
{
    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuColor}" />
    /// </summary>
    public class DefaultColorPicker : ADrawable<MenuColor>
    {
        #region Constants

        /// <summary>
        ///     The border offset.
        /// </summary>
        private const int BorderOffset = 20;

        /// <summary>
        ///     The picker width.
        /// </summary>
        private const int PickerWidth = 300;

        /// <summary>
        ///     The slider height.
        /// </summary>
        private const int SliderHeight = 30;

        /// <summary>
        ///     The slider offset.
        /// </summary>
        private const int SliderOffset = 10;

        /// <summary>
        ///     The text offset.
        /// </summary>
        private const int TextOffset = 10;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     The green width.
        /// </summary>
        private readonly int greenWidth;

        /// <summary>
        ///     The picker height.
        /// </summary>
        private readonly int pickerHeight;

        /// <summary>
        ///     The picker x axis.
        /// </summary>
        private readonly int pickerX;

        /// <summary>
        ///     The picker y axis.
        /// </summary>
        private readonly int pickerY;

        /// <summary>
        ///     The slider width.
        /// </summary>
        private readonly int sliderWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultColorPicker" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public DefaultColorPicker(MenuColor component)
            : base(component)
        {
            this.pickerHeight = (2 * BorderOffset) + MenuSettings.ContainerHeight + (4 * SliderHeight)
                                + (4 * SliderOffset);
            this.pickerX = (Drawing.Width - PickerWidth) / 2;
            this.pickerY = (Drawing.Height - this.pickerHeight) / 2;
            this.greenWidth = MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, "Opacity", 0).Width;
            this.sliderWidth = PickerWidth - (2 * BorderOffset) - (2 * TextOffset) - this.greenWidth - SliderHeight;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // do nothing
        }

        /// <summary>
        ///     Draws a MenuColor
        /// </summary>
        public override void Draw()
        {
            var rectangleName = DefaultUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(this.Component.Position.X + MenuSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                MenuSettings.TextColor);

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f),
                            this.Component.Position.Y + 1),
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f),
                            this.Component.Position.Y + Line.Width)
                    },
                this.Component.Color);
            Line.End();
            if (this.Component.HoveringPreview)
            {
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(
                                this.Component.Position.X + this.Component.MenuWidth
                                - (MenuSettings.ContainerHeight / 2f),
                                this.Component.Position.Y + 1),
                            new Vector2(
                                this.Component.Position.X + this.Component.MenuWidth
                                - (MenuSettings.ContainerHeight / 2f),
                                this.Component.Position.Y + MenuSettings.ContainerHeight)
                        },
                    MenuSettings.HoverColor);
                Line.End();
            }

            if (this.Component.Active)
            {
                MenuManager.Instance.DrawDelayed(
                    () =>
                        {
                            Line.Width = PickerWidth;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(this.pickerX + (PickerWidth / 2f), this.pickerY),
                                        new Vector2(this.pickerX + (PickerWidth / 2f), this.pickerY + this.pickerHeight)
                                    },
                                Color.Black);
                            Line.End();

                            Line.Width = 30f;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            this.pickerX + BorderOffset,
                                            this.pickerY + BorderOffset + (MenuSettings.ContainerHeight / 2f)),
                                        new Vector2(
                                            this.pickerX + PickerWidth - BorderOffset,
                                            this.pickerY + BorderOffset + (MenuSettings.ContainerHeight / 2f))
                                    },
                                this.Component.Color);
                            Line.End();

                            var previewColor = this.Component.Color;

                            var detail =
                                $"R:{previewColor.R}  G:{previewColor.G}  B:{previewColor.B}  A:{previewColor.A}";
                            var rectanglePreview =
                                new Rectangle(
                                    this.pickerX + BorderOffset,
                                    this.pickerY + BorderOffset,
                                    PickerWidth - (2 * BorderOffset),
                                    MenuSettings.ContainerHeight).GetCenteredText(
                                        null,
                                        MenuSettings.Font,
                                        detail,
                                        CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);
                            MenuSettings.Font.DrawText(
                                MenuManager.Instance.Sprite,
                                MultiLanguage.Translation(detail),
                                (int)rectanglePreview.X,
                                (int)rectanglePreview.Y,
                                new ColorBGRA(
                                    previewColor.R > 128 ? 0 : 255,
                                    previewColor.G > 128 ? 0 : 255,
                                    previewColor.B > 128 ? 0 : 255,
                                    255));

                            var textY =
                                (int)
                                new Rectangle(
                                    this.pickerX + BorderOffset,
                                    this.pickerY + BorderOffset + MenuSettings.ContainerHeight + SliderOffset,
                                    PickerWidth,
                                    SliderHeight).GetCenteredText(
                                        null,
                                        MenuSettings.Font,
                                        "Green",
                                        CenteredFlags.VerticalCenter).Y;

                            // DRAW SLIDER NAMES
                            string[] lineNames = { "Red", "Green", "Blue", "Opacity" };
                            for (var i = 0; i < lineNames.Length; i++)
                            {
                                MenuSettings.Font.DrawText(
                                    MenuManager.Instance.Sprite,
                                    MultiLanguage.Translation(lineNames[i]),
                                    this.pickerX + BorderOffset,
                                    textY + (i * (SliderOffset + SliderHeight)),
                                    Color.White);
                            }

                            // DRAW SLIDERS
                            Line.Width = 1;
                            Line.Begin();
                            for (var i = 1; i <= 4; i++)
                            {
                                Line.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + TextOffset,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f)),
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + TextOffset
                                                + this.sliderWidth,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                                        },
                                    Color.White);
                            }

                            Line.End();

                            // DRAW PREVIEW COLORS
                            ColorBGRA[] previewColors =
                                {
                                    new ColorBGRA(255, 0, 0, 255), new ColorBGRA(0, 255, 0, 255),
                                    new ColorBGRA(0, 0, 255, 255),
                                    new ColorBGRA(255, 255, 255, previewColor.A)
                                };

                            Line.Width = SliderHeight;
                            Line.Begin();
                            for (var i = 1; i <= 4; i++)
                            {
                                Line.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + (2 * TextOffset)
                                                + this.sliderWidth,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f)),
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + (2 * TextOffset)
                                                + this.sliderWidth + SliderHeight,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                                        },
                                    previewColors[i - 1]);
                            }

                            Line.End();

                            // DRAW SLIDER INDICATORS
                            byte[] indicators = { previewColor.R, previewColor.G, previewColor.B, previewColor.A };
                            Line.Width = 2;
                            Line.Begin();
                            for (var i = 0; i < indicators.Length; i++)
                            {
                                var x = (indicators[i] / 255f) * this.sliderWidth;
                                Line.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + TextOffset + x,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + ((i + 1) * SliderOffset) + (i * SliderHeight)),
                                            new Vector2(
                                                this.pickerX + BorderOffset + this.greenWidth + TextOffset + x,
                                                this.pickerY + BorderOffset + MenuSettings.ContainerHeight
                                                + ((i + 1) * SliderOffset) + ((i + 1) * SliderHeight))
                                        },
                                    new ColorBGRA(50, 154, 205, 255));
                            }

                            Line.End();
                        });
            }
        }

        /// <summary>
        ///     Processes windows events
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            var previewRect = PreviewBoundaries(this.Component);
            var pickerRect = this.PickerBoundaries();
            var redRect = this.RedPickerBoundaries();
            var greenRect = this.GreenPickerBoundaries();
            var blueRect = this.BluePickerBoundaries();
            var alphaRect = this.AlphaPickerBoundaries();

            if (args.Msg == WindowsMessages.MOUSEMOVE)
            {
                this.Component.HoveringPreview = args.Cursor.IsUnderRectangle(
                    previewRect.X,
                    previewRect.Y,
                    previewRect.Width,
                    previewRect.Height);

                if (this.Component.Active)
                {
                    if (this.Component.InteractingRed)
                    {
                        UpdateRed(this.Component, args, redRect);
                    }
                    else if (this.Component.InteractingGreen)
                    {
                        UpdateGreen(this.Component, args, greenRect);
                    }
                    else if (this.Component.InteractingBlue)
                    {
                        UpdateBlue(this.Component, args, blueRect);
                    }
                    else if (this.Component.InteractingAlpha)
                    {
                        UpdateAlpha(this.Component, args, alphaRect);
                    }
                }
            }

            if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.Component.InteractingRed = false;
                this.Component.InteractingGreen = false;
                this.Component.InteractingBlue = false;
                this.Component.InteractingAlpha = false;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                if (args.Cursor.IsUnderRectangle(previewRect.X, previewRect.Y, previewRect.Width, previewRect.Height))
                {
                    this.Component.Active = true;
                }
                else if (args.Cursor.IsUnderRectangle(pickerRect.X, pickerRect.Y, pickerRect.Width, pickerRect.Height)
                         && this.Component.Active)
                {
                    if (args.Cursor.IsUnderRectangle(redRect.X, redRect.Y, redRect.Width, redRect.Height))
                    {
                        this.Component.InteractingRed = true;
                        UpdateRed(this.Component, args, redRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        greenRect.X,
                        greenRect.Y,
                        greenRect.Width,
                        greenRect.Height))
                    {
                        this.Component.InteractingGreen = true;
                        UpdateGreen(this.Component, args, greenRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        blueRect.X,
                        blueRect.Y,
                        blueRect.Width,
                        blueRect.Height))
                    {
                        this.Component.InteractingBlue = true;
                        UpdateBlue(this.Component, args, blueRect);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        alphaRect.X,
                        alphaRect.Y,
                        alphaRect.Width,
                        alphaRect.Height))
                    {
                        this.Component.InteractingAlpha = true;
                        UpdateAlpha(this.Component, args, alphaRect);
                    }
                }
                else
                {
                    this.Component.Active = false;
                }
            }
        }

        /// <summary>
        ///     Gets the width of the slider
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public int SliderWidth()
        {
            return this.sliderWidth;
        }

        /// <summary>
        ///     Gets the width of a <see cref="MenuColor" />
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(this.Component) + MenuSettings.ContainerHeight;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the byte.
        /// </summary>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        /// <returns>
        ///     The byte.
        /// </returns>
        private static byte GetByte(WindowsKeys args, Rectangle rect)
        {
            if (args.Cursor.X < rect.X)
            {
                return 0;
            }

            if (args.Cursor.X > rect.X + rect.Width)
            {
                return 255;
            }

            return (byte)(((args.Cursor.X - rect.X) / rect.Width) * 255);
        }

        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        private static Rectangle PreviewBoundaries(AMenuComponent component)
        {
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
                (int)component.Position.Y,
                MenuSettings.ContainerHeight,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Updates the alpha value.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private static void UpdateAlpha(MenuColor component, WindowsKeys args, Rectangle rect)
        {
            component.Color = new ColorBGRA(
                component.Color.R,
                component.Color.G,
                component.Color.B,
                GetByte(args, rect));
            component.FireEvent();
        }

        /// <summary>
        ///     Updates the blue value.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private static void UpdateBlue(MenuColor component, WindowsKeys args, Rectangle rect)
        {
            component.Color = new ColorBGRA(
                component.Color.R,
                component.Color.G,
                GetByte(args, rect),
                component.Color.A);
            component.FireEvent();
        }

        /// <summary>
        ///     Updates the green value.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private static void UpdateGreen(MenuColor component, WindowsKeys args, Rectangle rect)
        {
            component.Color = new ColorBGRA(
                component.Color.R,
                GetByte(args, rect),
                component.Color.B,
                component.Color.A);
            component.FireEvent();
        }

        /// <summary>
        ///     Updates the red value.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     The windows keys.
        /// </param>
        /// <param name="rect">
        ///     The <see cref="Rectangle" />
        /// </param>
        private static void UpdateRed(MenuColor component, WindowsKeys args, Rectangle rect)
        {
            component.Color = new ColorBGRA(
                GetByte(args, rect),
                component.Color.G,
                component.Color.B,
                component.Color.A);
            component.FireEvent();
        }

        /// <summary>
        ///     Get the alpha picker boundaries
        /// </summary>
        /// <returns>The <see cref="Rectangle" /></returns>
        private Rectangle AlphaPickerBoundaries()
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset,
                this.pickerY + BorderOffset + MenuSettings.ContainerHeight + (4 * SliderOffset) + (3 * SliderHeight),
                this.sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the blue picker boundaries
        /// </summary>
        /// <returns>The <see cref="Rectangle" /></returns>
        private Rectangle BluePickerBoundaries()
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset,
                this.pickerY + BorderOffset + MenuSettings.ContainerHeight + (3 * SliderOffset) + (2 * SliderHeight),
                this.sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the green picker boundaries
        /// </summary>
        /// <returns>The <see cref="Rectangle" /></returns>
        private Rectangle GreenPickerBoundaries()
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset,
                this.pickerY + BorderOffset + MenuSettings.ContainerHeight + (2 * SliderOffset) + SliderHeight,
                this.sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="Rectangle" /></returns>
        private Rectangle PickerBoundaries()
        {
            return new Rectangle(this.pickerX, this.pickerY, PickerWidth, this.pickerHeight);
        }

        /// <summary>
        ///     Get the red picker boundaries
        /// </summary>
        /// <returns>The <see cref="Rectangle" /></returns>
        private Rectangle RedPickerBoundaries()
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset,
                this.pickerY + BorderOffset + MenuSettings.ContainerHeight + SliderOffset,
                this.sliderWidth,
                SliderHeight);
        }

        #endregion
    }
}