using System;
using System.Globalization;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    public class DefaultTheme : Theme
    {
        private static readonly Font Font = DefaultSettings.Font;
        private Drawable? _boolean;
        private Drawable? _keyBind, _seperator;
        private Drawable? _slider;

        public override Drawable Boolean
        {
            get { return (Drawable) (_boolean ?? (_boolean = GetBoolean())); }
        }

        public override Drawable Slider
        {
            get { return (Drawable) (_slider ?? (_slider = GetSlider())); }
        }

        public override Drawable KeyBind
        {
            get { return (Drawable) (_keyBind ?? (_keyBind = GetKeyBind())); }
        }

        public override Drawable Seperator
        {
            get { return (Drawable) (_seperator ?? (_seperator = GetSeperator())); }
        }

        public override void OnDraw(Vector2 position)
        {
            MenuInterface menuInterface = MenuInterface.Instance;
            float height = DefaultSettings.ContainerHeight * menuInterface.Menus.Count;
            float width = DefaultSettings.ContainerWidth;
            if (menuInterface.Menus.Count > 0)
            {
                width = menuInterface.Menus.First().MenuWidth;
            }

            DefaultSettings.ContainerLine.Width = width;
            DefaultSettings.ContainerLine.Begin();
            DefaultSettings.ContainerLine.Draw(
                new[]
                {
                    new Vector2(position.X + (width / 2f), position.Y),
                    new Vector2(position.X + (width / 2), position.Y + height)
                }, DefaultSettings.RootContainerColor);
            DefaultSettings.ContainerLine.End();

            for (int i = 0; i < menuInterface.Menus.Count; ++i)
            {
                menuInterface.Menus[i].OnDraw(
                    new Vector2(position.X, position.Y + i * DefaultSettings.ContainerHeight), i);
            }
        }

        public override void OnMenu(Menu menuComponent, Vector2 position, int index)
        {
            #region Hovering

            if (menuComponent.Hovering && !menuComponent.Toggled)
            {
                DefaultSettings.HoverLine.Begin();
                DefaultSettings.HoverLine.Draw(
                    new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2),
                        new Vector2(
                            position.X + menuComponent.MenuWidth, position.Y + DefaultSettings.ContainerHeight / 2)
                    },
                    DefaultSettings.HoverColor);
                DefaultSettings.HoverLine.End();
            }

            #endregion

            #region Text Draw

            var centerY =
                (int)
                    (GetContainerRectangle(position, menuComponent)
                        .GetCenteredText(null, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
                        .Y);

            Font.DrawText(
                null, menuComponent.DisplayName, (int) (position.X + DefaultSettings.ContainerTextOffset), centerY,
                DefaultSettings.TextColor);

            Font.DrawText(
                null, "»",
                (int)
                    (position.X + menuComponent.MenuWidth - DefaultSettings.ContainerTextMarkWidth +
                     DefaultSettings.ContainerTextMarkOffset), centerY, DefaultSettings.TextColor);

            #endregion

            #region Components Draw

            if ((index + 1) < MenuInterface.Instance.Menus.Count)
            {
                DefaultSettings.ContainerSeperatorLine.Begin();
                DefaultSettings.ContainerSeperatorLine.Draw(
                    new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                        new Vector2(position.X + menuComponent.MenuWidth, position.Y + DefaultSettings.ContainerHeight)
                    }, DefaultSettings.ContainerSeperatorColor);
                DefaultSettings.ContainerSeperatorLine.End();
            }

            #endregion

            if (menuComponent.Toggled)
            {
                #region Selection Mark

                DefaultSettings.ContainerLine.Width = menuComponent.MenuWidth;
                DefaultSettings.ContainerLine.Begin();
                DefaultSettings.ContainerLine.Draw(
                    new[]
                    {
                        new Vector2(position.X + menuComponent.MenuWidth / 2f, position.Y),
                        new Vector2(
                            position.X + menuComponent.MenuWidth / 2f, position.Y + DefaultSettings.ContainerHeight)
                    },
                    DefaultSettings.ContainerSelectedColor);
                DefaultSettings.ContainerLine.End();

                #endregion

                float height = DefaultSettings.ContainerHeight * menuComponent.Components.Count;
                float width = DefaultSettings.ContainerWidth;
                if (menuComponent.Components.Count > 0)
                {
                    width = menuComponent.Components.First().Value.MenuWidth;
                }

                DefaultSettings.ContainerLine.Width = width;
                DefaultSettings.ContainerLine.Begin();
                DefaultSettings.ContainerLine.Draw(
                    new[]
                    {
                        new Vector2((position.X + menuComponent.MenuWidth) + width / 2, position.Y),
                        new Vector2((position.X + menuComponent.MenuWidth) + width / 2, position.Y + height)
                    },
                    DefaultSettings.RootContainerColor);
                DefaultSettings.ContainerLine.End();

                for (int i = 0; i < menuComponent.Components.Count; ++i)
                {
                    if (menuComponent.Components.Values.ToList()[i] != null)
                    {
                        menuComponent.Components.Values.ToList()[i].OnDraw(
                            new Vector2(
                                position.X + menuComponent.MenuWidth, position.Y + i * DefaultSettings.ContainerHeight),
                            i);
                    }
                }
            }
        }

        public override int CalcWidthMenu(Menu menu)
        {
            return
                (int)
                    (CalcWidthText(menu.DisplayName + " »") + (DefaultSettings.ContainerTextOffset * 2) +
                     DefaultSettings.ContainerTextMarkWidth);
        }

        public override int CalcWidthItem(MenuItem menuItem)
        {
            return (int) (CalcWidthText(menuItem.DisplayName) + (DefaultSettings.ContainerTextOffset * 2));
        }

        public override int CalcWidthText(string text)
        {
            return DefaultSettings.Font.MeasureText(null, text, FontDrawFlags.Center).Width;
        }

        private Drawable GetBoolean()
        {
            return new Drawable
            {
                AdditionalBoundries =
                    (Vector2 position, AMenuComponent component) =>
                    {
                        return new Rectangle(
                            (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), (int) position.Y,
                            (int) DefaultSettings.ContainerHeight, (int) DefaultSettings.ContainerHeight);
                    },
                OnDraw = (component, position, index) =>
                {
                    #region Text Draw

                    var centerY =
                        (int)
                            (GetContainerRectangle(position, component)
                                .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                                .Y);

                    DefaultSettings.Font.DrawText(
                        null, component.DisplayName, (int) (position.X + DefaultSettings.ContainerTextOffset), centerY,
                        DefaultSettings.TextColor);

                    #endregion

                    #region Components Draw

                    if ((index + 1) < component.Parent.Components.Count)
                    {
                        DefaultSettings.ContainerSeperatorLine.Begin();
                        DefaultSettings.ContainerSeperatorLine.Draw(
                            new[]
                            {
                                new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                                new Vector2(
                                    position.X + component.MenuWidth, position.Y + DefaultSettings.ContainerHeight)
                            },
                            DefaultSettings.ContainerSeperatorColor);
                        DefaultSettings.ContainerSeperatorLine.End();
                    }

                    #endregion

                    #region Boolean Box Draw

                    var line = new Line(Drawing.Direct3DDevice)
                    {
                        Antialias = false,
                        GLLines = true,
                        Width = DefaultSettings.ContainerHeight
                    };
                    line.Begin();
                    line.Draw(
                        new[]
                        {
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2, position.Y + 1),
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2, position.Y + DefaultSettings.ContainerHeight)
                        },
                        ((MenuItem<MenuBool>) component).Value.Value
                            ? new ColorBGRA(0, 100, 0, 255)
                            : new ColorBGRA(255, 0, 0, 255));
                    line.End();
                    line.Dispose();

                    #region Text Draw

                    var centerX =
                        (int)
                            (new Rectangle(
                                (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight),
                                (int) (position.Y), (int) DefaultSettings.ContainerHeight,
                                (int) DefaultSettings.ContainerHeight).GetCenteredText(
                                    null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF",
                                    CenteredFlags.HorizontalCenter).X);
                    DefaultSettings.Font.DrawText(
                        null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF", centerX, centerY,
                        DefaultSettings.TextColor);

                    #endregion

                    #endregion
                },
                Bounding = GetContainerRectangle,
            };
        }

        private Drawable GetKeyBind()
        {
            return new Drawable
            {
                AdditionalBoundries =
                    (Vector2 position, AMenuComponent component) =>
                    {
                        return new Rectangle(
                            (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), (int) position.Y,
                            (int) DefaultSettings.ContainerHeight, (int) DefaultSettings.ContainerHeight);
                    },
                OnDraw = (component, position, index) =>
                {
                    MenuKeyBind value = ((MenuItem<MenuKeyBind>) component).Value;

                    #region Text

                    var centerY =
                        (int)
                            (GetContainerRectangle(position, component)
                                .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                                .Y);
                    DefaultSettings.Font.DrawText(
                        null, value.Interacting ? "Press a key" : component.DisplayName,
                        (int) (position.X + DefaultSettings.ContainerTextOffset), centerY, DefaultSettings.TextColor);

                    if (!value.Interacting)
                    {
                        string keyString = "[" + value.Key + "]";
                        DefaultSettings.Font.DrawText(
                            null, keyString,
                            (int)
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight -
                                 CalcWidthText(keyString) - DefaultSettings.ContainerTextOffset), centerY,
                            DefaultSettings.TextColor);
                    }

                    #endregion

                    #region Component

                    if ((index + 1) < component.Parent.Components.Count)
                    {
                        DefaultSettings.ContainerSeperatorLine.Begin();
                        DefaultSettings.ContainerSeperatorLine.Draw(
                            new[]
                            {
                                new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                                new Vector2(
                                    position.X + component.MenuWidth, position.Y + DefaultSettings.ContainerHeight)
                            },
                            DefaultSettings.ContainerSeperatorColor);
                        DefaultSettings.ContainerSeperatorLine.End();
                    }

                    #endregion

                    #region KeyBind

                    var line = new Line(Drawing.Direct3DDevice)
                    {
                        Antialias = false,
                        GLLines = true,
                        Width = DefaultSettings.ContainerHeight
                    };
                    line.Begin();
                    line.Draw(
                        new[]
                        {
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2, position.Y + 1),
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2, position.Y + DefaultSettings.ContainerHeight)
                        },
                        value.Active ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
                    line.End();
                    line.Dispose();

                    #region Text Draw

                    var centerX =
                        (int)
                            (new Rectangle(
                                (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight),
                                (int) (position.Y), (int) DefaultSettings.ContainerHeight,
                                (int) DefaultSettings.ContainerHeight).GetCenteredText(
                                    null, value.Active ? "ON" : "OFF", CenteredFlags.HorizontalCenter).X);
                    DefaultSettings.Font.DrawText(
                        null, value.Active ? "ON" : "OFF", centerX, centerY, DefaultSettings.TextColor);

                    #endregion

                    #endregion
                },
                Bounding = GetContainerRectangle
            };
        }

        private Drawable GetSlider()
        {
            return new Drawable
            {
                OnDraw = (component, position, index) =>
                {
                    #region Text

                    var centeredY =
                        (int)
                            (GetContainerRectangle(position, component)
                                .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                                .Y);
                    DefaultSettings.Font.DrawText(
                        null, component.DisplayName, (int) (position.X + DefaultSettings.ContainerTextOffset), centeredY,
                        DefaultSettings.TextColor);

                    int currentValue = ((MenuItem<MenuSlider>) component).Value.Value;
                    Rectangle measureText = DefaultSettings.Font.MeasureText(
                        null, currentValue.ToString(CultureInfo.InvariantCulture), FontDrawFlags.Center);
                    DefaultSettings.Font.DrawText(
                        null, currentValue.ToString(CultureInfo.InvariantCulture),
                        (int) (position.X + component.MenuWidth - DefaultSettings.ContainerTextOffset - measureText.Width),
                        centeredY, DefaultSettings.TextColor);

                    #endregion

                    #region Component

                    if ((index + 1) < component.Parent.Components.Count)
                    {
                        DefaultSettings.ContainerSeperatorLine.Begin();
                        DefaultSettings.ContainerSeperatorLine.Draw(
                            new[]
                            {
                                new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                                new Vector2(position.X + component.MenuWidth, position.Y + DefaultSettings.ContainerHeight)
                            }, DefaultSettings.ContainerSeperatorColor);
                        DefaultSettings.ContainerSeperatorLine.End();
                    }

                    #endregion

                    #region Slider Box Draw

                    MenuSlider value = ((MenuItem<MenuSlider>) component).Value;
                    float percent = (value.Value - value.MinValue) / (float) (value.MaxValue - value.MinValue);
                    float x = position.X + (percent * component.MenuWidth);

                    var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
                    line.Begin();
                    line.Draw(
                        new[]
                        { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + DefaultSettings.ContainerHeight) },
                        value.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
                    line.End();
                    line.Width = DefaultSettings.ContainerHeight;
                    line.Begin();
                    line.Draw(
                        new[]
                        {
                            new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2),
                            new Vector2(x, position.Y + DefaultSettings.ContainerHeight / 2)
                        }, DefaultSettings.HoverColor);
                    line.End();
                    line.Dispose();

                    #endregion
                },
                AdditionalBoundries = GetContainerRectangle,
                Bounding = GetContainerRectangle
            };
        }

        private Drawable GetSeperator()
        {
            return new Drawable
            {
                OnDraw = (component, position, index) =>
                {
                    #region Text Draw

                    Vector2 centerY =
                        (GetContainerRectangle(position, component)
                            .GetCenteredText(
                                null, component.DisplayName, CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter));

                    DefaultSettings.Font.DrawText(
                        null, component.DisplayName, (int) centerY.X, (int) centerY.Y, DefaultSettings.TextColor);

                    #endregion

                    #region Components Draw

                    if ((index + 1) < component.Parent.Components.Count)
                    {
                        DefaultSettings.ContainerSeperatorLine.Begin();
                        DefaultSettings.ContainerSeperatorLine.Draw(
                            new[]
                            {
                                new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                                new Vector2(position.X + component.MenuWidth, position.Y + DefaultSettings.ContainerHeight)
                            }, DefaultSettings.ContainerSeperatorColor);
                        DefaultSettings.ContainerSeperatorLine.End();
                    }

                    #endregion
                },
                Bounding = GetContainerRectangle,
                AdditionalBoundries = GetContainerRectangle
            };
        }

        private Rectangle GetContainerRectangle(Vector2 position, AMenuComponent component)
        {
            if (component == null)
            {
                Console.WriteLine("component is null");
            }

            return new Rectangle(
                (int) position.X, (int) position.Y, component.MenuWidth, (int) DefaultSettings.ContainerHeight);
        }
    }
}