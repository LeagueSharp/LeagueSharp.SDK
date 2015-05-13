// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTheme.cs" company="LeagueSharp">
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
//   The default theme for the menu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using System.Globalization;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The default theme for the menu.
    /// </summary>
    public class DefaultTheme : Theme
    {
        #region Static Fields

        /// <summary>
        ///     The font.
        /// </summary>
        private static readonly Font Font = DefaultSettings.Font;

        #endregion

        #region Fields

        /// <summary>
        ///     The boolean draw-able.
        /// </summary>
        private Drawable? boolean;

        /// <summary>
        ///     The KeyBind draw-able.
        /// </summary>
        private Drawable? keyBind;

        /// <summary>
        ///     The List draw-able.
        /// </summary>
        private DrawableList? list;

        /// <summary>
        ///     The Separator draw-able.
        /// </summary>
        private Drawable? separator;

        /// <summary>
        ///     The slider draw-able.
        /// </summary>
        private Drawable? slider;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the boolean.
        /// </summary>
        /// <value>
        ///     The boolean.
        /// </value>
        public override Drawable Boolean
        {
            get
            {
                return (Drawable)(this.boolean ?? (this.boolean = this.GetBoolean()));
            }
        }

        /// <summary>
        ///     Gets the key bind.
        /// </summary>
        /// <value>
        ///     The key bind.
        /// </value>
        public override Drawable KeyBind
        {
            get
            {
                return (Drawable)(this.keyBind ?? (this.keyBind = this.GetKeyBind()));
            }
        }

        /// <summary>
        ///     Gets the list.
        /// </summary>
        /// <value>
        ///     The list.
        /// </value>
        public override DrawableList List
        {
            get
            {
                return (DrawableList)(this.list ?? (this.list = GetList()));
            }
        }

        /// <summary>
        ///     Gets the separator.
        /// </summary>
        /// <value>
        ///     The separator.
        /// </value>
        public override Drawable Separator
        {
            get
            {
                return (Drawable)(this.separator ?? (this.separator = GetSeparator()));
            }
        }

        /// <summary>
        ///     Gets the slider.
        /// </summary>
        /// <value>
        ///     The slider.
        /// </value>
        public override Drawable Slider
        {
            get
            {
                return (Drawable)(this.slider ?? (this.slider = GetSlider()));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the width item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns>The <see cref="int" /></returns>
        public override int CalcWidthItem(MenuItem menuItem)
        {
            return (int)(this.CalcWidthText(menuItem.DisplayName) + (DefaultSettings.ContainerTextOffset * 2));
        }

        /// <summary>
        ///     Calculates the width of the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns>The <see cref="int" /></returns>
        public override int CalcWidthMenu(Menu menu)
        {
            return
                (int)
                (this.CalcWidthText(menu.DisplayName + " »") + (DefaultSettings.ContainerTextOffset * 2)
                 + DefaultSettings.ContainerTextMarkWidth);
        }

        /// <summary>
        ///     Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="int" /></returns>
        public override int CalcWidthText(string text)
        {
            return DefaultSettings.Font.MeasureText(null, text, 0).Width;
        }

        /// <summary>
        ///     Called when the Menu is drawn.
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
                    }, 
                DefaultSettings.RootContainerColor);
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

            var contour = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };
            contour.Begin();
            contour.Draw(
                new[] { new Vector2(position.X, position.Y), new Vector2(position.X + width, position.Y) }, 
                Color.Black);
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y + height), new Vector2(position.X + width, position.Y + height)
                    }, 
                Color.Black);
            contour.Draw(
                new[] { new Vector2(position.X, position.Y), new Vector2(position.X, position.Y + height) }, 
                Color.Black);
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X + width, position.Y), new Vector2(position.X + width, position.Y + height)
                    }, 
                Color.Black);
            contour.End();
            contour.Dispose();
        }

        /// <summary>
        ///     Called when the menu is drawn.
        /// </summary>
        /// <param name="menuComponent">The menu component.</param>
        /// <param name="position">The position.</param>
        /// <param name="index">The index.</param>
        public override void OnMenu(Menu menuComponent, Vector2 position, int index)
        {
            if (menuComponent.Hovering && !menuComponent.Toggled && menuComponent.Components.Count > 0)
            {
                DefaultSettings.HoverLine.Begin();
                DefaultSettings.HoverLine.Draw(
                    new[]
                        {
                            new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f), 
                            new Vector2(
                                position.X + menuComponent.MenuWidth, 
                                position.Y + DefaultSettings.ContainerHeight / 2f)
                        }, 
                    DefaultSettings.HoverColor);
                DefaultSettings.HoverLine.End();
            }

            var centerY =
                (int)
                GetContainerRectangle(position, menuComponent)
                    .GetCenteredText(null, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            Font.DrawText(
                null, 
                menuComponent.DisplayName, 
                (int)(position.X + DefaultSettings.ContainerTextOffset), 
                centerY, 
                DefaultSettings.TextColor);

            Font.DrawText(
                null, 
                "»", 
                (int)
                (position.X + menuComponent.MenuWidth - DefaultSettings.ContainerTextMarkWidth
                 + DefaultSettings.ContainerTextMarkOffset), 
                centerY, 
                menuComponent.Components.Count > 0 ? DefaultSettings.TextColor : DefaultSettings.ContainerSeparatorColor);

            if (menuComponent.Toggled)
            {
                DefaultSettings.ContainerLine.Width = menuComponent.MenuWidth;
                DefaultSettings.ContainerLine.Begin();
                DefaultSettings.ContainerLine.Draw(
                    new[]
                        {
                            new Vector2(position.X + menuComponent.MenuWidth / 2f, position.Y), 
                            new Vector2(
                                position.X + menuComponent.MenuWidth / 2f, 
                                position.Y + DefaultSettings.ContainerHeight)
                        }, 
                    DefaultSettings.ContainerSelectedColor);
                DefaultSettings.ContainerLine.End();

                float height = DefaultSettings.ContainerHeight * menuComponent.Components.Count;
                var width = DefaultSettings.ContainerWidth;
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

                for (var i = 0; i < menuComponent.Components.Count; ++i)
                {
                    var childComponent = menuComponent.Components.Values.ToList()[i];
                    if (childComponent != null)
                    {
                        var childPos = new Vector2(
                            position.X + menuComponent.MenuWidth, 
                            position.Y + i * DefaultSettings.ContainerHeight);

                        

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

                        

                        childComponent.OnDraw(childPos, i);
                    }
                }

                var contour = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };
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

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the container rectangle.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="component">
        ///     The component.
        /// </param>
        /// <returns>
        ///     <see cref="Rectangle" /> with information.
        /// </returns>
        private static Rectangle GetContainerRectangle(Vector2 position, AMenuComponent component)
        {
            if (component == null)
            {
                Logging.Write(true)(LogLevel.Error, "Component is null");
                return new Rectangle((int)position.X, (int)position.Y, 0, DefaultSettings.ContainerHeight);
            }

            return new Rectangle((int)position.X, (int)position.Y, component.MenuWidth, DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Gets the Draw-able list.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.DrawableList" /> instance.
        /// </returns>
        private static DrawableList GetList()
        {
            const int ArrowSpacing = 2;
            const int TextSpacing = 8;
            var arrowRectangle = DefaultSettings.Font.MeasureText(null, ">", 0);
            return new DrawableList
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var list = (MenuList)((MenuItem)component).ValueAsObject;

                                   var rectangleName = GetContainerRectangle(position, component)
                                       .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter);

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);

                                   var line = new Line(Drawing.Direct3DDevice)
                                                  {
                                                      Antialias = false, GLLines = true, 
                                                      Width = arrowRectangle.Width + (2 * ArrowSpacing)
                                                  };

                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - (arrowRectangle.Width / 2f) - ArrowSpacing, 
                                                   position.Y), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - (arrowRectangle.Width / 2f) - ArrowSpacing, 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       list.RightArrowHover ? DefaultSettings.ContainerSelectedColor : DefaultSettings.HoverColor);
                                   line.End();

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       ">", 
                                       (int)(position.X + component.MenuWidth - arrowRectangle.Width - ArrowSpacing), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);

                                   var textPos =
                                       new Rectangle(
                                           (int)
                                           (position.X + component.MenuWidth - (2 * ArrowSpacing) - arrowRectangle.Width
                                            - list.MaxStringWidth - TextSpacing), 
                                           (int)position.Y, 
                                           list.MaxStringWidth, 
                                           DefaultSettings.ContainerHeight).GetCenteredText(
                                               null, 
                                               list.SelectedValueAsObject.ToString(), 
                                               CenteredFlags.HorizontalCenter | CenteredFlags.VerticalCenter);
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       list.SelectedValueAsObject.ToString(), 
                                       (int)textPos.X, 
                                       (int)textPos.Y, 
                                       DefaultSettings.TextColor);

                                   

                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - arrowRectangle.Width - (3 * ArrowSpacing)
                                                   - list.MaxStringWidth - (2 * TextSpacing) - (arrowRectangle.Width / 2f), 
                                                   position.Y), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - arrowRectangle.Width - (3 * ArrowSpacing)
                                                   - list.MaxStringWidth - (2 * TextSpacing) - (arrowRectangle.Width / 2f), 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       list.LeftArrowHover ? DefaultSettings.ContainerSelectedColor : DefaultSettings.HoverColor);
                                   line.End();

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       "<", 
                                       (int)
                                       (position.X + component.MenuWidth - (2 * arrowRectangle.Width) - (2 * ArrowSpacing)
                                        - list.MaxStringWidth - (2 * TextSpacing)) - 2, 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);
                                   line.Dispose();

                                   
                               }, 
                           Bounding = GetContainerRectangle, AdditionalBoundries = GetContainerRectangle, 
                           RightArrow =
                               (position, component, menuList) =>
                               new Rectangle(
                                   (int)
                                   (position.X + component.MenuWidth - (2 * ArrowSpacing)
                                    - arrowRectangle.Width), 
                                   (int)position.Y, 
                                   (2 * ArrowSpacing) + arrowRectangle.Width, 
                                   DefaultSettings.ContainerHeight), 
                           Width =
                               list =>
                               list.MaxStringWidth + (2 * TextSpacing) + (4 * ArrowSpacing)
                               + (2 * arrowRectangle.Width), 
                           LeftArrow =
                               (position, component, menuList) =>
                               new Rectangle(
                                   (int)
                                   (position.X + component.MenuWidth - (4 * ArrowSpacing)
                                    - (2 * arrowRectangle.Width) - (2 * TextSpacing)
                                    - menuList.MaxStringWidth), 
                                   (int)position.Y, 
                                   (2 * ArrowSpacing) + arrowRectangle.Width, 
                                   DefaultSettings.ContainerHeight)
                       };
        }

        /// <summary>
        ///     Gets the separator draw-able object.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.Drawable" /> instance.
        /// </returns>
        private static Drawable GetSeparator()
        {
            return new Drawable
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var centerY = GetContainerRectangle(position, component)
                                       .GetCenteredText(
                                           null, 
                                           component.DisplayName, 
                                           CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)centerY.X, 
                                       (int)centerY.Y, 
                                       DefaultSettings.TextColor);
                               }, 
                           Bounding = GetContainerRectangle, AdditionalBoundries = GetContainerRectangle
                       };
        }

        /// <summary>
        ///     Gets the Slider draw-able object.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.Drawable" /> instance.
        /// </returns>
        private static Drawable GetSlider()
        {
            return new Drawable
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var centeredY =
                                       (int)
                                       GetContainerRectangle(position, component)
                                           .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                                           .Y;
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       centeredY, 
                                       DefaultSettings.TextColor);

                                   var currentValue = ((MenuItem<MenuSlider>)component).Value.Value;
                                   var measureText = DefaultSettings.Font.MeasureText(
                                       null, 
                                       currentValue.ToString(CultureInfo.InvariantCulture), 
                                       0);
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       currentValue.ToString(CultureInfo.InvariantCulture), 
                                       (int)
                                       (position.X + component.MenuWidth - DefaultSettings.ContainerTextOffset - measureText.Width), 
                                       centeredY, 
                                       DefaultSettings.TextColor);

                                   var value = ((MenuItem<MenuSlider>)component).Value;
                                   var percent = (value.Value - value.MinValue) / (float)(value.MaxValue - value.MinValue);
                                   var x = position.X + (percent * component.MenuWidth);

                                   var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(x, position.Y + 1), 
                                               new Vector2(x, position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       value.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
                                   line.End();
                                   line.Width = DefaultSettings.ContainerHeight;
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f), 
                                               new Vector2(x, position.Y + DefaultSettings.ContainerHeight / 2f)
                                           }, 
                                       DefaultSettings.HoverColor);
                                   line.End();
                                   line.Dispose();
                               }, 
                           AdditionalBoundries = GetContainerRectangle, Bounding = GetContainerRectangle
                       };
        }

        /// <summary>
        ///     Gets the the boolean draw-able object.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.Drawable" /> instance
        /// </returns>
        private Drawable GetBoolean()
        {
            return new Drawable
                       {
                           AdditionalBoundries =
                               (position, component) =>
                               new Rectangle(
                                   (int)(position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                                   (int)position.Y, 
                                   DefaultSettings.ContainerHeight, 
                                   DefaultSettings.ContainerHeight), 
                           OnDraw = (component, position, index) =>
                               {
                                   var centerY =
                                       (int)
                                       GetContainerRectangle(position, component)
                                           .GetCenteredText(
                                               null, 
                                               component.DisplayName, 
                                               CenteredFlags.VerticalCenter)
                                           .Y;

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       centerY, 
                                       DefaultSettings.TextColor);

                                   var line = new Line(Drawing.Direct3DDevice)
                                                  {
                                                      Antialias = false, GLLines = true, 
                                                      Width = DefaultSettings.ContainerHeight
                                                  };
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   (position.X + component.MenuWidth
                                                    - DefaultSettings.ContainerHeight)
                                                   + DefaultSettings.ContainerHeight / 2f, 
                                                   position.Y + 1), 
                                               new Vector2(
                                                   (position.X + component.MenuWidth
                                                    - DefaultSettings.ContainerHeight)
                                                   + DefaultSettings.ContainerHeight / 2f, 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       ((MenuItem<MenuBool>)component).Value.Value
                                           ? new ColorBGRA(0, 100, 0, 255)
                                           : new ColorBGRA(255, 0, 0, 255));
                                   line.End();
                                   line.Dispose();

                                   var centerX =
                                       (int)
                                       new Rectangle(
                                           (int)
                                           (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                                           (int)position.Y, 
                                           DefaultSettings.ContainerHeight, 
                                           DefaultSettings.ContainerHeight).GetCenteredText(
                                               null, 
                                               ((MenuItem<MenuBool>)component).Value.Value ? "ON" : "OFF", 
                                               CenteredFlags.HorizontalCenter).X;
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       ((MenuItem<MenuBool>)component).Value.Value ? "ON" : "OFF", 
                                       centerX, 
                                       centerY, 
                                       DefaultSettings.TextColor);
                               }, 
                           Bounding = GetContainerRectangle
                       };
        }

        /// <summary>
        ///     Gets the Draw-able KeyBind object.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.Drawable" /> object.
        /// </returns>
        private Drawable GetKeyBind()
        {
            return new Drawable
                       {
                           AdditionalBoundries =
                               (position, component) =>
                               new Rectangle(
                                   (int)(position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                                   (int)position.Y, 
                                   DefaultSettings.ContainerHeight, 
                                   DefaultSettings.ContainerHeight), 
                           OnDraw = (component, position, index) =>
                               {
                                   var value = ((MenuItem<MenuKeyBind>)component).Value;

                                   var centerY =
                                       (int)
                                       GetContainerRectangle(position, component)
                                           .GetCenteredText(
                                               null, 
                                               component.DisplayName, 
                                               CenteredFlags.VerticalCenter)
                                           .Y;
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       value.Interacting ? "Press a key" : component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       centerY, 
                                       DefaultSettings.TextColor);

                                   if (!value.Interacting)
                                   {
                                       var keyString = "[" + value.Key + "]";
                                       DefaultSettings.Font.DrawText(
                                           null, 
                                           keyString, 
                                           (int)
                                           (position.X + component.MenuWidth - DefaultSettings.ContainerHeight
                                            - this.CalcWidthText(keyString)
                                            - DefaultSettings.ContainerTextOffset), 
                                           centerY, 
                                           DefaultSettings.TextColor);
                                   }

                                   var line = new Line(Drawing.Direct3DDevice)
                                                  {
                                                      Antialias = false, GLLines = true, 
                                                      Width = DefaultSettings.ContainerHeight
                                                  };
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   (position.X + component.MenuWidth
                                                    - DefaultSettings.ContainerHeight)
                                                   + DefaultSettings.ContainerHeight / 2f, 
                                                   position.Y + 1), 
                                               new Vector2(
                                                   (position.X + component.MenuWidth
                                                    - DefaultSettings.ContainerHeight)
                                                   + DefaultSettings.ContainerHeight / 2f, 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       value.Active
                                           ? new ColorBGRA(0, 100, 0, 255)
                                           : new ColorBGRA(255, 0, 0, 255));
                                   line.End();
                                   line.Dispose();

                                   var centerX =
                                       (int)
                                       new Rectangle(
                                           (int)
                                           (position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                                           (int)position.Y, 
                                           DefaultSettings.ContainerHeight, 
                                           DefaultSettings.ContainerHeight).GetCenteredText(
                                               null, 
                                               value.Active ? "ON" : "OFF", 
                                               CenteredFlags.HorizontalCenter).X;
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       value.Active ? "ON" : "OFF", 
                                       centerX, 
                                       centerY, 
                                       DefaultSettings.TextColor);
                               }, 
                           Bounding = GetContainerRectangle
                       };
        }

        #endregion
    }
}