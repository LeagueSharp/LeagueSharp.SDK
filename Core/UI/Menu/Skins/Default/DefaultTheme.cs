using System.Globalization;
using System.Linq;

using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     The default theme for the menu.
    /// </summary>
    public class DefaultTheme : Theme
    {
        private static readonly Font Font = DefaultSettings.Font;
        private Drawable? _boolean;
        private Drawable? _keyBind, _separator;
        private Drawable? _slider;
        private DrawableList? _list;

        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <value>
        /// The boolean.
        /// </value>
        public override Drawable Boolean
        {
            get { return (Drawable) (_boolean ?? (_boolean = GetBoolean())); }
        }

        /// <summary>
        /// Gets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        public override Drawable Slider
        {
            get { return (Drawable) (_slider ?? (_slider = GetSlider())); }
        }

        /// <summary>
        /// Gets the key bind.
        /// </summary>
        /// <value>
        /// The key bind.
        /// </value>
        public override Drawable KeyBind
        {
            get { return (Drawable) (_keyBind ?? (_keyBind = GetKeyBind())); }
        }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        public override Drawable Separator
        {
            get { return (Drawable) (_separator ?? (_separator = GetSeparator())); }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public override DrawableList List
        {
            get { return (DrawableList)(_list ?? (_list = GetList())); }
        }

        /// <summary>
        /// Called when the Menu is drawn.
        /// </summary>
        /// <param name="position">The position.</param>
        public override void OnDraw(Vector2 position)
        {
            var menuManager = MenuManager.Instance;
            var height = DefaultSettings.ContainerHeight * menuManager.Menus.Count;
            var width = DefaultSettings.ContainerWidth;
            if (menuManager.Menus.Count > 0)
            {
                width = menuManager.Menus.First().MenuWidth;
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

            for (var i = 0; i < menuManager.Menus.Count; ++i)
            {
                var childPos = new Vector2(position.X, position.Y + i * DefaultSettings.ContainerHeight);

                if (i < menuManager.Menus.Count - 1)
                {
                    DefaultSettings.ContainerSeparatorLine.Begin();
                    DefaultSettings.ContainerSeparatorLine.Draw(
                        new[]
                                {
                                    new Vector2(childPos.X, childPos.Y + DefaultSettings.ContainerHeight),
                                    new Vector2(
                                        childPos.X + menuManager.Menus[i].MenuWidth,
                                        childPos.Y + DefaultSettings.ContainerHeight)
                                },
                        DefaultSettings.ContainerSeparatorColor);
                    DefaultSettings.ContainerSeparatorLine.End();
                }

                menuManager.Menus[i].OnDraw(childPos, i);
            }

            Line contour = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };
            contour.Begin();
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y),
                        new Vector2(position.X + width, position.Y)
                    },
                Color.Black);
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y + height),
                        new Vector2(position.X + width, position.Y + height)
                    },
                Color.Black);
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y),
                        new Vector2(position.X, position.Y + height)
                    },
                Color.Black);
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X + width, position.Y),
                        new Vector2(position.X + width, position.Y + height)
                    },
                Color.Black);
            contour.End();
            contour.Dispose();
        }

        /// <summary>
        /// Called when the menu is drawn.
        /// </summary>
        /// <param name="menuComponent">The menu component.</param>
        /// <param name="position">The position.</param>
        /// <param name="index">The index.</param>
        public override void OnMenu(Menu menuComponent, Vector2 position, int index)
        {
            #region Hovering

            if (menuComponent.Hovering && !menuComponent.Toggled && menuComponent.Components.Count > 0)
            {
                DefaultSettings.HoverLine.Begin();
                DefaultSettings.HoverLine.Draw(
                    new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f),
                        new Vector2(
                            position.X + menuComponent.MenuWidth, position.Y + DefaultSettings.ContainerHeight / 2f)
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
                     DefaultSettings.ContainerTextMarkOffset), centerY,
                menuComponent.Components.Count > 0 ? DefaultSettings.TextColor : DefaultSettings.ContainerSeparatorColor);

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
                    AMenuComponent childComponent = menuComponent.Components.Values.ToList()[i];
                    if (childComponent != null)
                    {
                        var childPos = new Vector2(
                            position.X + menuComponent.MenuWidth, position.Y + i * DefaultSettings.ContainerHeight);

                        #region Separator

                        if (i < menuComponent.Components.Count - 1)
                        {
                            DefaultSettings.ContainerSeparatorLine.Begin();
                            DefaultSettings.ContainerSeparatorLine.Draw(
                                new[]
                                {
                                    new Vector2(childPos.X, childPos.Y + DefaultSettings.ContainerHeight),
                                    new Vector2(
                                        childPos.X + childComponent.MenuWidth,
                                        childPos.Y + DefaultSettings.ContainerHeight)
                                },
                                DefaultSettings.ContainerSeparatorColor);
                            DefaultSettings.ContainerSeparatorLine.End();
                        }

                        #endregion

                        childComponent.OnDraw(childPos, i);
                    }
                }

                Line contour = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };
                contour.Begin();
                contour.Draw(
                    new[]
                    {
                        new Vector2(position.X + menuComponent.MenuWidth, position.Y),
                        new Vector2(position.X + menuComponent.MenuWidth + width, position.Y)
                    },
                    Color.Black);
                contour.Draw(
                    new[]
                    {
                        new Vector2(position.X + menuComponent.MenuWidth, position.Y + height),
                        new Vector2(position.X + menuComponent.MenuWidth + width, position.Y + height)
                    },
                    Color.Black);
                contour.Draw(
                    new[]
                    {
                        new Vector2(position.X + menuComponent.MenuWidth, position.Y),
                        new Vector2(position.X + menuComponent.MenuWidth, position.Y + height)
                    },
                    Color.Black);
                contour.Draw(
                    new[]
                    {
                        new Vector2(position.X + menuComponent.MenuWidth + width, position.Y),
                        new Vector2(position.X + menuComponent.MenuWidth + width, position.Y + height)
                    },
                    Color.Black);
                contour.End();
                contour.Dispose();
            }
        }

        /// <summary>
        /// Calculates the width of the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public override int CalcWidthMenu(Menu menu)
        {
            return
                (int)
                    (CalcWidthText(menu.DisplayName + " »") + (DefaultSettings.ContainerTextOffset * 2) +
                     DefaultSettings.ContainerTextMarkWidth);
        }

        /// <summary>
        /// Calculates the width item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns></returns>
        public override int CalcWidthItem(MenuItem menuItem)
        {
            return (int) (CalcWidthText(menuItem.DisplayName) + (DefaultSettings.ContainerTextOffset * 2));
        }

        /// <summary>
        /// Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public override int CalcWidthText(string text)
        {
            return DefaultSettings.Font.MeasureText(null, text, 0).Width;
        }

        private Drawable GetBoolean()
        {
            return new Drawable
            {
                AdditionalBoundries =
                    (position, component) => new Rectangle(
                        (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), (int) position.Y,
                        DefaultSettings.ContainerHeight, DefaultSettings.ContainerHeight),
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
                                DefaultSettings.ContainerHeight / 2f, position.Y + 1),
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2f, position.Y + DefaultSettings.ContainerHeight)
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
                                (int) (position.Y), DefaultSettings.ContainerHeight,
                                DefaultSettings.ContainerHeight).GetCenteredText(
                                    null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF",
                                    CenteredFlags.HorizontalCenter).X);
                    DefaultSettings.Font.DrawText(
                        null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF", centerX, centerY,
                        DefaultSettings.TextColor);

                    #endregion

                    #endregion
                },
                Bounding = GetContainerRectangle
            };
        }

        private Drawable GetKeyBind()
        {
            return new Drawable
            {
                AdditionalBoundries =
                    (position, component) => new Rectangle(
                        (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), (int) position.Y,
                        DefaultSettings.ContainerHeight, DefaultSettings.ContainerHeight),
                OnDraw = (component, position, index) =>
                {
                    var value = ((MenuItem<MenuKeyBind>) component).Value;

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
                        var keyString = "[" + value.Key + "]";
                        DefaultSettings.Font.DrawText(
                            null, keyString,
                            (int)
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight -
                                 CalcWidthText(keyString) - DefaultSettings.ContainerTextOffset), centerY,
                            DefaultSettings.TextColor);
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
                                DefaultSettings.ContainerHeight / 2f, position.Y + 1),
                            new Vector2(
                                (position.X + component.MenuWidth - DefaultSettings.ContainerHeight) +
                                DefaultSettings.ContainerHeight / 2f, position.Y + DefaultSettings.ContainerHeight)
                        },
                        value.Active ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
                    line.End();
                    line.Dispose();

                    #region Text Draw

                    var centerX =
                        (int)
                            (new Rectangle(
                                (int) (position.X + component.MenuWidth - DefaultSettings.ContainerHeight),
                                (int) (position.Y), DefaultSettings.ContainerHeight,
                                DefaultSettings.ContainerHeight).GetCenteredText(
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
                        null, currentValue.ToString(CultureInfo.InvariantCulture), 0);
                    DefaultSettings.Font.DrawText(
                        null, currentValue.ToString(CultureInfo.InvariantCulture),
                        (int) (position.X + component.MenuWidth - DefaultSettings.ContainerTextOffset - measureText.Width),
                        centeredY, DefaultSettings.TextColor);

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
                            new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f),
                            new Vector2(x, position.Y + DefaultSettings.ContainerHeight / 2f)
                        }, DefaultSettings.HoverColor);
                    line.End();
                    line.Dispose();

                    #endregion
                },
                AdditionalBoundries = GetContainerRectangle,
                Bounding = GetContainerRectangle
            };
        }

        private Drawable GetSeparator()
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
                },
                Bounding = GetContainerRectangle,
                AdditionalBoundries = GetContainerRectangle
            };
        }

        private DrawableList GetList()
        {
            const int arrowSpacing = 2;
            const int textSpacing = 8;
            Rectangle arrowRectangle = DefaultSettings.Font.MeasureText(null, ">", 0);
            return new DrawableList
            {
                OnDraw = (component, position, index) =>
                {
                    var list = (MenuList)((MenuItem)component).ValueAsObject;
                    #region Text Draw
                    var rectangleName =
                            (GetContainerRectangle(position, component)
                                .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                                );

                    DefaultSettings.Font.DrawText(
                        null, component.DisplayName, (int)(position.X + DefaultSettings.ContainerTextOffset), (int) rectangleName.Y,
                        DefaultSettings.TextColor);

                    #endregion

                    #region rightarrow

                    Line line = new Line(Drawing.Direct3DDevice)
                    {
                        Antialias = false,
                        GLLines = true,
                        Width = arrowRectangle.Width + (2 * arrowSpacing)
                    };

                    line.Begin();
                    line.Draw(new[]
                    {
                        new Vector2(position.X + component.MenuWidth - (arrowRectangle.Width /2f) - arrowSpacing, position.Y),
                        new Vector2(position.X + component.MenuWidth - (arrowRectangle.Width /2f) - arrowSpacing, position.Y + DefaultSettings.ContainerHeight)
                    }, list.RightArrowHover ? DefaultSettings.ContainerSelectedColor : DefaultSettings.HoverColor);
                    line.End();

                    DefaultSettings.Font.DrawText(
                        null, ">", (int)(position.X + component.MenuWidth - arrowRectangle.Width - arrowSpacing), (int)rectangleName.Y,
                        DefaultSettings.TextColor);
                    #endregion
                    
                    #region Value

                    Vector2 textPos = new Rectangle(
                        (int)
                            (position.X + component.MenuWidth - (2 * arrowSpacing) - arrowRectangle.Width -
                             list.MaxStringWidth - textSpacing), (int) position.Y, list.MaxStringWidth, DefaultSettings.ContainerHeight)
                        .GetCenteredText(
                            null, list.SelectedValueAsObject.ToString(),
                            CenteredFlags.HorizontalCenter | CenteredFlags.VerticalCenter);
                    DefaultSettings.Font.DrawText(
                        null, list.SelectedValueAsObject.ToString(), (int) textPos.X, (int) textPos.Y,
                        DefaultSettings.TextColor);
                    #endregion

                    #region leftarrow
                    line.Begin();
                    line.Draw(new[]
                    {
                        new Vector2(position.X + component.MenuWidth - arrowRectangle.Width -  (3 *arrowSpacing) - list.MaxStringWidth - (2* textSpacing) - (arrowRectangle.Width/2f), position.Y),
                        new Vector2(position.X + component.MenuWidth - arrowRectangle.Width -  (3 *arrowSpacing) - list.MaxStringWidth - (2* textSpacing) - (arrowRectangle.Width/2f), position.Y + DefaultSettings.ContainerHeight)
                    }, list.LeftArrowHover ? DefaultSettings.ContainerSelectedColor : DefaultSettings.HoverColor);
                    line.End();

                     DefaultSettings.Font.DrawText(
                        null, "<", (int)(position.X + component.MenuWidth - (2 * arrowRectangle.Width) - (2 * arrowSpacing) - list.MaxStringWidth - (2 * textSpacing)) - 2, (int)rectangleName.Y,
                        DefaultSettings.TextColor);
                    line.Dispose();
                    #endregion


                },
                Bounding = GetContainerRectangle,
                AdditionalBoundries = GetContainerRectangle,
                RightArrow =
                    (position, component, menuList) =>
                        new Rectangle(
                            (int) (position.X + component.MenuWidth - (2 * arrowSpacing) - arrowRectangle.Width),
                            (int) position.Y, (2 * arrowSpacing) + arrowRectangle.Width, DefaultSettings.ContainerHeight),
                Width =
                    list => list.MaxStringWidth + (2 * textSpacing) + (4 * arrowSpacing) + (2 * arrowRectangle.Width),
                LeftArrow =
                (position, component, menuList) =>
                    new Rectangle(
                        (int)(position.X + component.MenuWidth - (4 * arrowSpacing) - (2 *arrowRectangle.Width) - (2 * textSpacing) - menuList.MaxStringWidth),
                        (int)position.Y, (2 * arrowSpacing) + arrowRectangle.Width, DefaultSettings.ContainerHeight)
            };
        }

        private Rectangle GetContainerRectangle(Vector2 position, AMenuComponent component)
        {
            if (component == null)
            {
                Logging.Write(true)(LogLevel.Error, "Component is null");
            }

            return new Rectangle(
                (int) position.X, (int) position.Y, component.MenuWidth, DefaultSettings.ContainerHeight);
        }

        
    }
}