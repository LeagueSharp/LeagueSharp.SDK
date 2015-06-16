// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultColorPicker.cs" company="LeagueSharp">
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
//   A default implementation of an <see cref="IDrawableColorPicker" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="IDrawableColorPicker" />
    /// </summary>
    public class DefaultColorPicker : DefaultComponent, IDrawableColorPicker
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
        ///     Creates a new instance of a DefaultColorPicker
        /// </summary>
        public DefaultColorPicker()
        {
            this.pickerHeight = (2 * BorderOffset) + DefaultSettings.ContainerHeight + (4 * SliderHeight)
                                + (4 * SliderOffset);
            this.pickerX = (Drawing.Width - PickerWidth) / 2;
            this.pickerY = (Drawing.Height - this.pickerHeight) / 2;
            this.greenWidth = DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, "Opacity", 0).Width;
            this.sliderWidth = PickerWidth - (2 * BorderOffset) - (2 * TextOffset) - this.greenWidth - SliderHeight;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the alpha picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle AlphaPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset, 
                this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (4 * SliderOffset) + (3 * SliderHeight), 
                this.sliderWidth, 
                SliderHeight);
        }

        /// <summary>
        ///     Get the blue picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle BluePickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset, 
                this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (3 * SliderOffset) + (2 * SliderHeight), 
                this.sliderWidth, 
                SliderHeight);
        }

        /// <summary>
        ///     Draws a MenuColor
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        public void Draw(MenuColor component)
        {
            var rectangleName = GetContainerRectangle(component)
                .GetCenteredText(null, DefaultSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(component.Position.X + DefaultSettings.ContainerTextOffset), 
                (int)rectangleName.Y, 
                DefaultSettings.TextColor);

            var previewLine = new Line(Drawing.Direct3DDevice)
                                  {
                                     Width = DefaultSettings.ContainerHeight, GLLines = true 
                                  };
            previewLine.Begin();
            previewLine.Draw(
                new[]
                    {
                        new Vector2(
                            component.Position.X + component.MenuWidth
                            - (DefaultSettings.ContainerHeight / 2f), 
                            component.Position.Y + 1), 
                        new Vector2(
                            component.Position.X + component.MenuWidth
                            - (DefaultSettings.ContainerHeight / 2f), 
                            component.Position.Y + DefaultSettings.ContainerHeight)
                    }, 
                component.Color);
            previewLine.End();
            if (component.HoveringPreview)
            {
                previewLine.Begin();
                previewLine.Draw(
                    new[]
                        {
                            new Vector2(
                                component.Position.X + component.MenuWidth
                                - (DefaultSettings.ContainerHeight / 2f), 
                                component.Position.Y + 1), 
                            new Vector2(
                                component.Position.X + component.MenuWidth
                                - (DefaultSettings.ContainerHeight / 2f), 
                                component.Position.Y + DefaultSettings.ContainerHeight)
                        }, 
                    DefaultSettings.HoverColor);
                previewLine.End();
            }

            if (component.Active)
            {
                var backgroundLine = new Line(Drawing.Direct3DDevice) { Width = PickerWidth, GLLines = true };
                backgroundLine.Begin();
                backgroundLine.Draw(
                    new[]
                        {
                            new Vector2(this.pickerX + (PickerWidth / 2f), this.pickerY), 
                            new Vector2(this.pickerX + (PickerWidth / 2f), this.pickerY + this.pickerHeight)
                        }, 
                    DefaultSettings.RootContainerColor);
                backgroundLine.End();
                backgroundLine.Dispose();

                previewLine.Begin();
                previewLine.Draw(
                    new[]
                        {
                            new Vector2(
                                this.pickerX + BorderOffset, 
                                this.pickerY + BorderOffset + (DefaultSettings.ContainerHeight / 2f)), 
                            new Vector2(
                                this.pickerX + PickerWidth - BorderOffset, 
                                this.pickerY + BorderOffset + (DefaultSettings.ContainerHeight / 2f))
                        }, 
                    component.Color);
                previewLine.End();

                var previewColor = component.Color;

                var detail = string.Format(
                    "R:{0}  G:{1}  B:{2}  A:{3}", 
                    previewColor.R, 
                    previewColor.G, 
                    previewColor.B, 
                    previewColor.A);
                var rectanglePreview =
                    new Rectangle(
                        this.pickerX + BorderOffset, 
                        this.pickerY + BorderOffset, 
                        PickerWidth - (2 * BorderOffset), 
                        DefaultSettings.ContainerHeight).GetCenteredText(
                            null, DefaultSettings.Font,
                            detail, 
                            CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);
                DefaultSettings.Font.DrawText(
                    MenuManager.Instance.Sprite, 
                    detail, 
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
                        this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + SliderOffset, 
                        PickerWidth,
                        SliderHeight).GetCenteredText(null, DefaultSettings.Font, "Green", CenteredFlags.VerticalCenter).Y;

                // DRAW SLIDER NAMES
                string[] lineNames = { "Red", "Green", "Blue", "Opacity" };
                for (var i = 0; i < lineNames.Length; i++)
                {
                    DefaultSettings.Font.DrawText(
                        MenuManager.Instance.Sprite, 
                        lineNames[i], 
                        this.pickerX + BorderOffset, 
                        textY + (i * (SliderOffset + SliderHeight)), 
                        Color.White);
                }

                // DRAW SLIDERS
                var sliderLine = new Line(Drawing.Direct3DDevice) { Width = 1, GLLines = true };
                sliderLine.Begin();
                for (var i = 1; i <= 4; i++)
                {
                    sliderLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + TextOffset, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f)), 
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + TextOffset + this.sliderWidth, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                            }, 
                        Color.White);
                }

                sliderLine.End();
                sliderLine.Dispose();

                // DRAW PREVIEW COLORS
                ColorBGRA[] previewColors =
                    {
                        new ColorBGRA(previewColor.R, 0, 0, 255), 
                        new ColorBGRA(0, previewColor.G, 0, 255), 
                        new ColorBGRA(0, 0, previewColor.B, 255), 
                        new ColorBGRA(255, 255, 255, previewColor.A)
                    };
                var sliderPreviewLine = new Line(Drawing.Direct3DDevice) { Width = SliderHeight, GLLines = true };
                sliderPreviewLine.Begin();
                for (var i = 1; i <= 4; i++)
                {
                    sliderPreviewLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + (2 * TextOffset) + this.sliderWidth, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f)), 
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + (2 * TextOffset) + this.sliderWidth
                                    + SliderHeight, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                            }, 
                        previewColors[i - 1]);
                }

                sliderPreviewLine.End();
                sliderPreviewLine.Dispose();

                // DRAW SLIDER INDICATORS
                byte[] indicators = { previewColor.R, previewColor.G, previewColor.B, previewColor.A };
                var sliderIndicatorLine = new Line(Drawing.Direct3DDevice) { Width = 2, GLLines = true };
                sliderIndicatorLine.Begin();
                for (var i = 0; i < indicators.Length; i++)
                {
                    var x = (indicators[i] / 255f) * this.sliderWidth;
                    sliderIndicatorLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + TextOffset + x, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                    + ((i + 1) * SliderOffset) + (i * SliderHeight)), 
                                new Vector2(
                                    this.pickerX + BorderOffset + this.greenWidth + TextOffset + x, 
                                    this.pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                    + ((i + 1) * SliderOffset) + ((i + 1) * SliderHeight))
                            }, 
                        new ColorBGRA(50, 154, 205, 255));
                }

                sliderIndicatorLine.End();
                sliderIndicatorLine.Dispose();
            }

            previewLine.Dispose();
        }

        /// <summary>
        ///     Get the green picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle GreenPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset, 
                this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + (2 * SliderOffset) + SliderHeight, 
                this.sliderWidth, 
                SliderHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle PickerBoundaries(MenuColor component)
        {
            return new Rectangle(this.pickerX, this.pickerY, PickerWidth, this.pickerHeight);
        }

        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle PreviewBoundaries(MenuColor component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                    (int)component.Position.Y, 
                    DefaultSettings.ContainerHeight, 
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Get the red picker boundaries
        /// </summary>
        /// <param name="component">>The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle RedPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                this.pickerX + BorderOffset + this.greenWidth + TextOffset, 
                this.pickerY + BorderOffset + DefaultSettings.ContainerHeight + SliderOffset, 
                this.sliderWidth, 
                SliderHeight);
        }

        /// <summary>
        ///     Gets the width of the slider
        /// </summary>
        /// <param name="component">>The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="int" /></returns>
        public int SliderWidth(MenuColor component)
        {
            return this.sliderWidth;
        }

        /// <summary>
        ///     Gets the width of a <see cref="MenuColor" />
        /// </summary>
        /// <param name="component">>The <see cref="MenuColor" /></param>
        /// <returns>The <see cref="int" /></returns>
        public int Width(MenuColor component)
        {
            return DefaultSettings.ContainerHeight;
        }

        #endregion
    }
}