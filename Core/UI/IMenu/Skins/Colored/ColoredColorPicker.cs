// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredColorPicker.cs" company="LeagueSharp">
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
//   A custom implementation of <see cref="ADrawable{MenuColorPicker}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Colored
{
    using System.Drawing;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Rectangle = SharpDX.Rectangle;
    using Utilities = LeagueSharp.SDK.UI.Utilities;

    internal class ColoredColorPicker : ADrawable<MenuColor>
    {
        #region Constants

        /// <summary>
        ///     The ApplyButton height.
        /// </summary>
        private const int ApplyButtonHeight = 25;

        /// <summary>
        ///     The ApplyButton width.
        /// </summary>
        private const int ApplyButtonWidth = 50;

        /// <summary>
        ///     The border offset.
        /// </summary>
        private const int BorderOffset = 10;

        /// <summary>
        ///     The CancelButton height.
        /// </summary>
        private const int CancelButtonHeight = 25;

        /// <summary>
        ///     The CancelButton width.
        /// </summary>
        private const int CancelButtonWidth = 50;

        /// <summary>
        ///     The ColorBox height.
        /// </summary>
        private const int ColorBoxHeight = 200;

        /// <summary>
        ///     The ColorBox width.
        /// </summary>
        private const int ColorBoxWidth = 200;

        /// <summary>
        ///     The ColorPicker height.
        /// </summary>
        private const int ColorPickerHeight = 270;

        /// <summary>
        ///     The ColorPicker width.
        /// </summary>
        private const int ColorPickerWidth = 330;

        /// <summary>
        ///     The Preview height.
        /// </summary>
        private const int PreviewHeight = 40;

        /// <summary>
        ///     The Preview width.
        /// </summary>
        private const int PreviewWidth = 40;

        /// <summary>
        ///     The VerticalAlphaSlider height.
        /// </summary>
        private const int VerticalAlphaSliderHeight = 200;

        /// <summary>
        ///     The VerticalAlphaSlider width.
        /// </summary>
        private const int VerticalAlphaSliderWidth = 40;

        /// <summary>
        ///     The VerticalColorSlider height.
        /// </summary>
        private const int VerticalColorSliderHeight = 200;

        /// <summary>
        ///     The VerticalColorSlider width.
        /// </summary>
        private const int VerticalColorSliderWidth = 40;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     The ColorBox to display the color spectrum.
        /// </summary>
        protected ColorBox ColorBox;

        /// <summary>
        ///     The VerticalAlphaSlider to change the opacity.
        /// </summary>
        protected VerticalAlphaSlider VerticalAlphaSlider;

        /// <summary>
        ///     The VerticalColorSlider to change the color spectrum.
        /// </summary>
        protected VerticalColorSlider VerticalColorSlider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColoredColorPicker" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public ColoredColorPicker(MenuColor component)
            : base(component)
        {
            Hsl tempHsl;
            this.ColorBox = new ColorBox(new Size(200, 200), true)
                                { Hsl = Utilities.RgbToHsl(this.Component.Color.ToSystemColor()) };

            this.VerticalColorSlider = new VerticalColorSlider(new Size(40, 200), true)
                                           { CbHsl = Utilities.RgbToHsl(this.Component.Color.ToSystemColor()) };
            this.VerticalColorSlider.ColorSliderScroll += () =>
                {
                    tempHsl = this.ColorBox.Hsl;
                    tempHsl.H = this.VerticalColorSlider.CbHsl.H;
                    this.ColorBox.Hsl = tempHsl;
                };

            this.VerticalAlphaSlider = new VerticalAlphaSlider(new Size(40, 200));
            tempHsl = this.VerticalAlphaSlider.CbHsl;
            tempHsl.L = this.ColorBox.Hsl.L;
            this.VerticalAlphaSlider.CbHsl = tempHsl;
            this.VerticalAlphaSlider.AlphaSliderScroll += () =>
                {
                    tempHsl = this.ColorBox.Hsl;
                    tempHsl.L = this.VerticalAlphaSlider.CbHsl.L;
                    this.ColorBox.Hsl = tempHsl;
                };
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether interacting ColorBox.
        /// </summary>
        private bool InteractingColorBox { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting VerticalAlphaSlider.
        /// </summary>
        private bool InteractingVerticalAlphaSlider { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting VerticalColorSlider.
        /// </summary>
        private bool InteractingVerticalColorSlider { get; set; }

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
            var rectangleName = ColoredUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(this.Component.Position.X + MenuSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                MenuSettings.TextColor);

            Line.Width = MenuSettings.ContainerHeight - 7;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f) - 4,
                            this.Component.Position.Y + 1 + 3),
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f) - 4,
                            this.Component.Position.Y + Line.Width + 3)
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
                                this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f) - 4,
                                this.Component.Position.Y + 1 + 3),
                            new Vector2(
                                this.Component.Position.X + this.Component.MenuWidth - (Line.Width / 2f) - 4,
                                this.Component.Position.Y + Line.Width + 3)
                        },
                    MenuSettings.HoverColor);
                Line.End();
            }

            if (this.Component.Active)
            {
                MenuManager.Instance.DrawDelayed(
                    delegate
                        {
                            Line.Width = this.ColorPickerBoundaries().Width;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(this.ColorPickerBoundaries().X + 165, this.ColorPickerBoundaries().Y),
                                        new Vector2(
                                            this.ColorPickerBoundaries().X + 165,
                                            this.ColorPickerBoundaries().Y + this.ColorPickerBoundaries().Height)
                                    },
                                MenuSettings.RootContainerColor);
                            Line.End();

                            this.ColorBox.DrawControl(
                                new Vector2(this.ColorBoxBoundaries().X, this.ColorBoxBoundaries().Y));
                            this.VerticalColorSlider.DrawControl(
                                new Vector2(
                                    this.VerticalColorSliderBoundaries().X,
                                    this.VerticalColorSliderBoundaries().Y));
                            this.VerticalAlphaSlider.DrawControl(
                                new Vector2(
                                    this.VerticalAlphaSliderBoundaries().X,
                                    this.VerticalAlphaSliderBoundaries().Y));

                            Utils.DrawBoxFilled(
                                this.PreviewBoundaries().X,
                                this.PreviewBoundaries().Y,
                                this.PreviewBoundaries().Width,
                                this.PreviewBoundaries().Height,
                                Color.Black);

                            Utils.DrawBoxFilled(
                                this.PreviewBoundaries().X,
                                this.PreviewBoundaries().Y,
                                this.PreviewBoundaries().Width,
                                this.PreviewBoundaries().Height,
                                this.ColorBox.Rgb.ToSharpDxColor());

                            var applyButtonTextWidth =
                                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, "Apply", 0).Width;
                            var cancelButtonTextWidth =
                                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, "Cancel", 0).Width;

                            Line.Width = this.ApplyButtonBoundaries().Width;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(this.ApplyButtonBoundaries().X + 25, this.ApplyButtonBoundaries().Y),
                                        new Vector2(
                                            this.ApplyButtonBoundaries().X + 25,
                                            this.ApplyButtonBoundaries().Y + this.ApplyButtonBoundaries().Height)
                                    },
                                new ColorBGRA(68, 160, 255, 255));
                            Line.End();

                            MenuSettings.Font.DrawText(
                                MenuManager.Instance.Sprite,
                                "Apply",
                                this.ApplyButtonBoundaries().X - applyButtonTextWidth / 2 + 25,
                                (int)
                                this.CancelButtonBoundaries()
                                    .GetCenteredText(null, MenuSettings.Font, "Apply", CenteredFlags.VerticalCenter)
                                    .Y,
                                new ColorBGRA(221, 233, 255, 255));

                            Line.Width = this.CancelButtonBoundaries().Width;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(this.CancelButtonBoundaries().X + 25, this.CancelButtonBoundaries().Y),
                                        new Vector2(
                                            this.CancelButtonBoundaries().X + 25,
                                            this.CancelButtonBoundaries().Y + this.CancelButtonBoundaries().Height)
                                    },
                                new ColorBGRA(68, 160, 255, 255));
                            Line.End();

                            MenuSettings.Font.DrawText(
                                MenuManager.Instance.Sprite,
                                "Cancel",
                                this.CancelButtonBoundaries().X - cancelButtonTextWidth / 2 + 25,
                                (int)
                                this.CancelButtonBoundaries()
                                    .GetCenteredText(null, MenuSettings.Font, "Cancel", CenteredFlags.VerticalCenter)
                                    .Y,
                                new ColorBGRA(221, 233, 255, 255));
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

            if (args.Msg == WindowsMessages.MOUSEMOVE)
            {
                this.Component.HoveringPreview = args.Cursor.IsUnderRectangle(
                    previewRect.X,
                    previewRect.Y,
                    previewRect.Width,
                    previewRect.Height);

                if (this.Component.Active)
                {
                    if (this.InteractingColorBox)
                    {
                        this.ColorBox.ColorBoxMouseMove(args);
                    }
                    else if (this.InteractingVerticalColorSlider)
                    {
                        this.VerticalColorSlider.VerticalColorSliderMouseMove(args);
                    }
                    else if (this.InteractingVerticalAlphaSlider)
                    {
                        this.VerticalAlphaSlider.VerticalAlphaSliderMouseMove(args);
                    }
                }
            }

            if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                if (this.InteractingColorBox)
                {
                    this.InteractingColorBox = false;
                    this.ColorBox.ColorBoxMouseUp(args);
                }
                if (this.InteractingVerticalColorSlider)
                {
                    this.InteractingVerticalColorSlider = false;
                    this.VerticalColorSlider.VerticalColorSliderMouseUp(args);
                }
                if (this.InteractingVerticalAlphaSlider)
                {
                    this.InteractingVerticalAlphaSlider = false;
                    this.VerticalAlphaSlider.VerticalAlphaSliderMouseUp(args);
                }
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                if (args.Cursor.IsUnderRectangle(
                    this.ColorPickerBoundaries().X,
                    this.ColorPickerBoundaries().Y,
                    this.ColorPickerBoundaries().Width,
                    this.ColorPickerBoundaries().Height))
                {
                    this.Component.Active = true;
                    if (args.Cursor.IsUnderRectangle(
                        this.ColorBoxBoundaries().X,
                        this.ColorBoxBoundaries().Y,
                        this.ColorBoxBoundaries().Width,
                        this.ColorBoxBoundaries().Height))
                    {
                        this.InteractingColorBox = true;
                        this.ColorBox.ColorBoxMouseDown(args);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        this.VerticalColorSliderBoundaries().X,
                        this.VerticalColorSliderBoundaries().Y,
                        this.VerticalColorSliderBoundaries().Width,
                        this.VerticalColorSliderBoundaries().Height))
                    {
                        this.InteractingVerticalColorSlider = true;
                        this.VerticalColorSlider.VerticalColorSliderMouseDown(args);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        this.VerticalAlphaSliderBoundaries().X,
                        this.VerticalAlphaSliderBoundaries().Y,
                        this.VerticalAlphaSliderBoundaries().Width,
                        this.VerticalAlphaSliderBoundaries().Height))
                    {
                        this.InteractingVerticalAlphaSlider = true;
                        this.VerticalAlphaSlider.VerticalAlphaSliderMouseDown(args);
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        this.ApplyButtonBoundaries().X,
                        this.ApplyButtonBoundaries().Y,
                        this.ApplyButtonBoundaries().Width,
                        this.ApplyButtonBoundaries().Height))
                    {
                        this.Component.Active = false;
                        this.Component.Color = this.ColorBox.Rgb.ToSharpDxColor();
                    }
                    else if (args.Cursor.IsUnderRectangle(
                        this.CancelButtonBoundaries().X,
                        this.CancelButtonBoundaries().Y,
                        this.CancelButtonBoundaries().Width,
                        this.CancelButtonBoundaries().Height))
                    {
                        this.Component.Active = false;
                        this.ColorBox.Rgb = this.Component.Color.ToSystemColor();
                        this.VerticalColorSlider.Rgb = this.Component.Color.ToSystemColor();
                        this.VerticalAlphaSlider.Rgb = this.Component.Color.ToSystemColor();
                    }
                }
                else if (args.Cursor.IsUnderRectangle(
                    previewRect.X,
                    previewRect.Y,
                    previewRect.Width,
                    previewRect.Height))
                {
                    this.Component.Active = true;
                }
                else
                {
                    this.Component.Active = false;
                }
            }
        }

        /// <summary>
        ///     Gets the width of a <see cref="MenuColor" />
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public override int Width()
        {
            return ColoredUtilities.CalcWidthItem(this.Component) + MenuSettings.ContainerHeight;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private static Rectangle PreviewBoundaries(AMenuComponent component)
        {
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
                (int)component.Position.Y,
                MenuSettings.ContainerHeight,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle ApplyButtonBoundaries()
        {
            return
                new Rectangle(
                    this.VerticalColorSliderBoundaries().X + BorderOffset
                    - this.VerticalColorSliderBoundaries().Width / 2,
                    this.VerticalColorSliderBoundaries().Y + BorderOffset + this.VerticalColorSliderBoundaries().Height,
                    ApplyButtonWidth,
                    ApplyButtonHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle CancelButtonBoundaries()
        {
            return new Rectangle(
                this.ApplyButtonBoundaries().X + BorderOffset + this.ApplyButtonBoundaries().Width,
                this.ApplyButtonBoundaries().Y,
                CancelButtonWidth,
                CancelButtonHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle ColorBoxBoundaries()
        {
            return new Rectangle(
                this.ColorPickerBoundaries().X + BorderOffset,
                this.ColorPickerBoundaries().Y + BorderOffset,
                ColorBoxWidth,
                ColorBoxHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle ColorPickerBoundaries()
        {
            return new Rectangle(
                (int)this.Component.Position.X + this.Component.MenuWidth + BorderOffset,
                (int)this.Component.Position.Y,
                ColorPickerWidth,
                ColorPickerHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle PreviewBoundaries()
        {
            return new Rectangle(
                this.ColorBoxBoundaries().X,
                this.ColorBoxBoundaries().Y + BorderOffset + this.ColorBoxBoundaries().Height,
                PreviewWidth,
                PreviewHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle VerticalAlphaSliderBoundaries()
        {
            return
                new Rectangle(
                    this.VerticalColorSliderBoundaries().X + BorderOffset + this.VerticalColorSliderBoundaries().Width,
                    this.VerticalColorSliderBoundaries().Y,
                    VerticalAlphaSliderWidth,
                    VerticalAlphaSliderHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <returns>The <see cref="SharpDX.Rectangle" /></returns>
        private Rectangle VerticalColorSliderBoundaries()
        {
            return new Rectangle(
                this.ColorBoxBoundaries().X + BorderOffset + this.ColorBoxBoundaries().Width,
                this.ColorBoxBoundaries().Y,
                VerticalColorSliderWidth,
                VerticalColorSliderHeight);
        }

        #endregion
    }
}