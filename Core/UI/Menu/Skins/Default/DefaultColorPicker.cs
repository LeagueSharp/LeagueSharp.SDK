namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using System;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// A default implementation of an IDrawableColorPicker
    /// </summary>
    public class DefaultColorPicker : DefaultComponent, IDrawableColorPicker
    {
        private readonly int pickerHeight;

        private readonly int pickerX;

        private readonly int pickerY;

        private readonly int greenWidth;

        private readonly int sliderWidth;

        private const int SliderHeight = 30;

        private const int SliderOffset = 10;

        private const int PickerWidth = 300;

        private const int BorderOffset = 20;

        private const int TextOffset = 10;

        /// <summary>
        /// Creates a new instance of a DefaultColorPicker
        /// </summary>
        public DefaultColorPicker()
        {
            pickerHeight = (2 * BorderOffset) + DefaultSettings.ContainerHeight + (4 * SliderHeight)
                           + (4 * SliderOffset);
            pickerX = (Drawing.Width - PickerWidth) / 2;
            pickerY = (Drawing.Height - pickerHeight) / 2;
            greenWidth = DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, "Green", 0).Width;
            sliderWidth = PickerWidth - (2 * BorderOffset) - (2 * TextOffset) - greenWidth - SliderHeight;
        }

        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle PreviewBoundaries(MenuColor component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - DefaultSettings.ContainerHeight),
                    (int)component.Container.Position.Y,
                    DefaultSettings.ContainerHeight,
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle PickerBoundaries(MenuColor component)
        {
            return new Rectangle(pickerX, pickerY, PickerWidth, pickerHeight);
        }

        /// <summary>
        ///     Get the red picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle RedPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                pickerX + BorderOffset + greenWidth + TextOffset,
                pickerY + BorderOffset + DefaultSettings.ContainerHeight + SliderOffset,
                sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the green picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle GreenPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                pickerX + BorderOffset + greenWidth + TextOffset,
                pickerY + BorderOffset + DefaultSettings.ContainerHeight + (2 * SliderOffset) + SliderHeight,
                sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the blue picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle BluePickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                pickerX + BorderOffset + greenWidth + TextOffset,
                pickerY + BorderOffset + DefaultSettings.ContainerHeight + (3 * SliderOffset) + (2 * SliderHeight),
                sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Get the alpha picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        public Rectangle AlphaPickerBoundaries(MenuColor component)
        {
            return new Rectangle(
                pickerX + BorderOffset + greenWidth + TextOffset,
                pickerY + BorderOffset + DefaultSettings.ContainerHeight + (4 * SliderOffset) + (3 * SliderHeight),
                sliderWidth,
                SliderHeight);
        }

        /// <summary>
        ///     Draws a MenuColor
        /// </summary>
        /// <param name="component">MenuColor</param>
        public void Draw(MenuColor component)
        {
            Vector2 rectangleName = GetContainerRectangle(component.Container)
                .GetCenteredText(null, component.Container.DisplayName, CenteredFlags.VerticalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Container.DisplayName,
                (int)(component.Container.Position.X + DefaultSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                DefaultSettings.TextColor);

            var previewLine = new Line(Drawing.Direct3DDevice)
                                  { Width = DefaultSettings.ContainerHeight, GLLines = true };
            previewLine.Begin();
            previewLine.Draw(
                new[]
                    {
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth
                            - (DefaultSettings.ContainerHeight / 2f),
                            component.Container.Position.Y + 1),
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth
                            - (DefaultSettings.ContainerHeight / 2f),
                            component.Container.Position.Y + DefaultSettings.ContainerHeight)
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
                                component.Container.Position.X + component.Container.MenuWidth
                                - (DefaultSettings.ContainerHeight / 2f),
                                component.Container.Position.Y + 1),
                            new Vector2(
                                component.Container.Position.X + component.Container.MenuWidth
                                - (DefaultSettings.ContainerHeight / 2f),
                                component.Container.Position.Y + DefaultSettings.ContainerHeight)
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
                            new Vector2(pickerX + (PickerWidth / 2f), pickerY),
                            new Vector2(pickerX + (PickerWidth / 2f), pickerY + pickerHeight)
                        },
                    DefaultSettings.RootContainerColor);
                backgroundLine.End();
                backgroundLine.Dispose();

                previewLine.Begin();
                previewLine.Draw(
                    new[]
                        {
                            new Vector2(
                                pickerX + BorderOffset,
                                pickerY + BorderOffset + (DefaultSettings.ContainerHeight / 2f)),
                            new Vector2(
                                pickerX + PickerWidth - BorderOffset,
                                pickerY + BorderOffset + (DefaultSettings.ContainerHeight / 2f))
                        },
                    component.Color);
                previewLine.End();

                ColorBGRA previewColor = component.Color;

                String detail = string.Format(
                    "R:{0}  G:{1}  B:{2}  A:{3}",
                    previewColor.R,
                    previewColor.G,
                    previewColor.B,
                    previewColor.A);
                Vector2 rectanglePreview =
                    new Rectangle(
                        pickerX + BorderOffset,
                        pickerY + BorderOffset,
                        PickerWidth - (2 * BorderOffset),
                        DefaultSettings.ContainerHeight).GetCenteredText(
                            null,
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
                        pickerX + BorderOffset,
                        pickerY + BorderOffset + DefaultSettings.ContainerHeight + SliderOffset,
                        PickerWidth,
                        SliderHeight).GetCenteredText(null, "Green", CenteredFlags.VerticalCenter).Y;

                // DRAW SLIDER NAMES
                string[] lineNames = { "Red", "Green", "Blue", "Alpha" };
                for (int i = 0; i < lineNames.Length; i++)
                {
                    DefaultSettings.Font.DrawText(
                        MenuManager.Instance.Sprite,
                        lineNames[i],
                        pickerX + BorderOffset,
                        textY + (i * (SliderOffset + SliderHeight)),
                        Color.White);
                }

                //DRAW SLIDERS
                var sliderLine = new Line(Drawing.Direct3DDevice) { Width = 1, GLLines = true };
                sliderLine.Begin();
                for (int i = 1; i <= 4; i++)
                {
                    sliderLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + TextOffset,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f)),
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + TextOffset + sliderWidth,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                            },
                        Color.White);
                }
                sliderLine.End();
                sliderLine.Dispose();

                //DRAW PREVIEW COLORS
                ColorBGRA[] previewColors =
                {
                    new ColorBGRA(previewColor.R, 0, 0, 255),
                    new ColorBGRA(0, previewColor.G, 0, 255),
                    new ColorBGRA(0, 0, previewColor.B, 255),
                    new ColorBGRA(255, 255, 255, previewColor.A)
                };
                var sliderPreviewLine = new Line(Drawing.Direct3DDevice) { Width = SliderHeight, GLLines = true };
                sliderPreviewLine.Begin();
                for (int i = 1; i <= 4; i++)
                {
                    sliderPreviewLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + (2 * TextOffset) + sliderWidth,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f)),
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + (2 * TextOffset) + sliderWidth + SliderHeight,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + (i * SliderOffset)
                                    + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                            },
                        previewColors[i - 1]);
                }
                sliderPreviewLine.End();
                sliderPreviewLine.Dispose();

                //DRAW SLIDER INDICATORS
                byte[] indicators = { previewColor.R, previewColor.G, previewColor.B, previewColor.A };
                var sliderIndicatorLine = new Line(Drawing.Direct3DDevice) { Width = 2, GLLines = true };
                sliderIndicatorLine.Begin();
                for (int i = 0; i < indicators.Length; i++)
                {
                    float x = ((indicators[i] / 255f) * sliderWidth);
                    sliderIndicatorLine.Draw(
                        new[]
                            {
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + TextOffset + x,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + ((i + 1) * SliderOffset)
                                    + (i * SliderHeight)),
                                new Vector2(
                                    pickerX + BorderOffset + greenWidth + TextOffset + x,
                                    pickerY + BorderOffset + DefaultSettings.ContainerHeight + ((i + 1) * SliderOffset)
                                    + ((i + 1) * SliderHeight))
                            },
                        new ColorBGRA(50, 154, 205, 255));
                }
                sliderIndicatorLine.End();
                sliderIndicatorLine.Dispose();
            }

            previewLine.Dispose();
        }

        /// <summary>
        ///     Gets the width of a MenuCOlor
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>int</returns>
        public int Width(MenuColor component)
        {
            return DefaultSettings.ContainerHeight;
        }

        /// <summary>
        ///     Gets the width of the slider
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>int</returns>
        public int SliderWidth(MenuColor component)
        {
            return sliderWidth;
        }
    }
}