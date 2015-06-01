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
    using System.Collections.Generic;
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
        ///     The button draw-able.
        /// </summary>
        private DrawableButton? button;

        /// <summary>
        ///     The slider draw-able.
        /// </summary>
        private DrawableColor? colorPicker;

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
        ///     Gets the button.
        /// </summary>
        /// <value>
        ///     The slider.
        /// </value>
        public override DrawableButton Button
        {
            get
            {
                return (DrawableButton)(this.button ?? (this.button = GetButton()));
            }
        }

        /// <summary>
        ///     Gets the color.
        /// </summary>
        /// <value>
        ///     The color.
        /// </value>
        public override DrawableColor ColorPicker
        {
            get
            {
                return (DrawableColor)(this.colorPicker ?? (this.colorPicker = GetColorPicker()));
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
                new[]
                    {
                        new Vector2(position.X, position.Y), new Vector2(position.X + width, position.Y), 
                        new Vector2(position.X + width, position.Y + height), new Vector2(position.X, position.Y + height), 
                        new Vector2(position.X, position.Y)
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
                 - DefaultSettings.ContainerTextMarkOffset), 
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

        /// <summary>
        ///     Gets the button draw-able object.
        /// </summary>
        /// <returns>
        ///     <see cref="Theme.Drawable" /> instance.
        /// </returns>
        private static DrawableButton GetButton()
        {
            const int TextGap = 5;
            var buttonColor = new ColorBGRA(100, 100, 100, 255);
            var buttonHoverColor = new ColorBGRA(170, 170, 170, 200);
            return new DrawableButton
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var rectangleName = GetContainerRectangle(position, component)
                                       .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter);

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);

                                   var button = (MenuItem<MenuButton>)component;

                                   var buttonTextWidth = DefaultSettings.Font.MeasureText(null, button.Value.ButtonText, 0).Width;

                                   var line = new Line(Drawing.Direct3DDevice)
                                                  {
                                                     Antialias = false, GLLines = true, Width = DefaultSettings.ContainerHeight 
                                                  };
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - buttonTextWidth - (2 * TextGap), 
                                                   position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                                               new Vector2(
                                                   position.X + component.MenuWidth, 
                                                   position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                                           }, 
                                       DefaultSettings.HoverColor);
                                   line.End();
                                   line.Width = DefaultSettings.ContainerHeight - 5;
                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - buttonTextWidth - (2 * TextGap) + 2, 
                                                   position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - 2, 
                                                   position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                                           }, 
                                       button.Value.Hovering ? buttonHoverColor : buttonColor);
                                   line.End();
                                   line.Dispose();

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       button.Value.ButtonText, 
                                       (int)(position.X + component.MenuWidth - buttonTextWidth - TextGap), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);
                               }, 
                           ButtonBoundaries = delegate(Vector2 position, AMenuComponent component)
                               {
                                   var button = (MenuItem<MenuButton>)component;
                                   var buttonTextWidth =
                                       DefaultSettings.Font.MeasureText(null, button.Value.ButtonText, 0)
                                           .Width;
                                   return
                                       new Rectangle(
                                           (int)
                                           (position.X + component.MenuWidth - buttonTextWidth
                                            - (2 * TextGap)), 
                                           (int)position.Y, 
                                           (2 * TextGap) + buttonTextWidth, 
                                           DefaultSettings.ContainerHeight);
                               }, 
                           Width =
                               menuButton =>
                               (2 * TextGap)
                               + DefaultSettings.Font.MeasureText(null, menuButton.ButtonText, 0).Width
                       };
        }

        /// <summary>
        ///     Gets the color picker draw-able.
        /// </summary>
        /// <returns>
        ///     The color picker draw-able.
        /// </returns>
        private static DrawableColor GetColorPicker()
        {
            const int SliderHeight = 30;
            const int SliderOffset = 10;
            const int PickerWidth = 300;
            const int BorderOffset = 20;
            const int TextOffset = 10;
            var pickerHeight = (2 * BorderOffset) + DefaultSettings.ContainerHeight + (4 * SliderHeight)
                               + (4 * SliderOffset);
            var pickerX = (Drawing.Width - PickerWidth) / 2;
            var pickerY = (Drawing.Height - pickerHeight) / 2;
            var greenWidth = DefaultSettings.Font.MeasureText(null, "Green", 0).Width;
            var sliderWidth = PickerWidth - (2 * BorderOffset) - (2 * TextOffset) - greenWidth - SliderHeight;

            return new DrawableColor
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var rectangleName = GetContainerRectangle(position, component)
                                       .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter);

                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       component.DisplayName, 
                                       (int)(position.X + DefaultSettings.ContainerTextOffset), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);

                                   var menuColor = (MenuItem<MenuColor>)component;

                                   var previewLine = new Line(Drawing.Direct3DDevice)
                                                         {
                                                            Width = DefaultSettings.ContainerHeight, GLLines = true 
                                                         };
                                   previewLine.Begin();
                                   previewLine.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - (DefaultSettings.ContainerHeight / 2f), 
                                                   position.Y + 1), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - (DefaultSettings.ContainerHeight / 2f), 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       menuColor.Value.Color);
                                   previewLine.End();
                                   if (menuColor.Value.HoveringPreview)
                                   {
                                       previewLine.Begin();
                                       previewLine.Draw(
                                           new[]
                                               {
                                                   new Vector2(
                                                       position.X + component.MenuWidth - (DefaultSettings.ContainerHeight / 2f), 
                                                       position.Y + 1), 
                                                   new Vector2(
                                                       position.X + component.MenuWidth - (DefaultSettings.ContainerHeight / 2f), 
                                                       position.Y + DefaultSettings.ContainerHeight)
                                               }, 
                                           DefaultSettings.HoverColor);
                                       previewLine.End();
                                   }

                                   if (menuColor.Value.Active)
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
                                           menuColor.Value.Color);
                                       previewLine.End();

                                       var previewColor = menuColor.Value.Color;

                                       var detail = string.Format(
                                           "R:{0}  G:{1}  B:{2}  A:{3}", 
                                           previewColor.R, 
                                           previewColor.G, 
                                           previewColor.B, 
                                           previewColor.A);
                                       var rectanglePreview =
                                           new Rectangle(
                                               pickerX + BorderOffset, 
                                               pickerY + BorderOffset, 
                                               PickerWidth - (2 * BorderOffset), 
                                               DefaultSettings.ContainerHeight).GetCenteredText(
                                                   null, 
                                                   detail, 
                                                   CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);
                                       DefaultSettings.Font.DrawText(
                                           null, 
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
                                       string[] lineNames = {
                                                               "Red", "Green", "Blue", "Alpha" 
                                                            };
                                       for (var i = 0; i < lineNames.Length; i++)
                                       {
                                           DefaultSettings.Font.DrawText(
                                               null, 
                                               lineNames[i], 
                                               pickerX + BorderOffset, 
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
                                                           pickerX + BorderOffset + greenWidth + TextOffset, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f)), 
                                                       new Vector2(
                                                           pickerX + BorderOffset + greenWidth + TextOffset + sliderWidth, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                                                   }, 
                                               Color.White);
                                       }

                                       sliderLine.End();
                                       sliderLine.Dispose();

                                       // DRAW PREVIEW COLORS
                                       var previewColors = new[]
                                                               {
                                                                   new ColorBGRA(previewColor.R, 0, 0, 255), 
                                                                   new ColorBGRA(0, previewColor.G, 0, 255), 
                                                                   new ColorBGRA(0, 0, previewColor.B, 255), 
                                                                   new ColorBGRA(255, 255, 255, previewColor.A)
                                                               };
                                       var sliderPreviewLine = new Line(Drawing.Direct3DDevice)
                                                                   {
                                                                      Width = SliderHeight, GLLines = true 
                                                                   };
                                       sliderPreviewLine.Begin();
                                       for (var i = 1; i <= 4; i++)
                                       {
                                           sliderPreviewLine.Draw(
                                               new[]
                                                   {
                                                       new Vector2(
                                                           pickerX + BorderOffset + greenWidth + (2 * TextOffset) + sliderWidth, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f)), 
                                                       new Vector2(
                                                           pickerX + BorderOffset + greenWidth + (2 * TextOffset) + sliderWidth
                                                           + SliderHeight, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + (i * SliderOffset) + ((i - 1) * SliderHeight) + (SliderHeight / 2f))
                                                   }, 
                                               previewColors[i - 1]);
                                       }

                                       sliderPreviewLine.End();
                                       sliderPreviewLine.Dispose();

                                       // DRAW SLIDER INDICATORS
                                       var indicators = new[] { previewColor.R, previewColor.G, previewColor.B, previewColor.A };
                                       var sliderIndicatorLine = new Line(Drawing.Direct3DDevice) { Width = 2, GLLines = true };
                                       sliderIndicatorLine.Begin();
                                       for (var i = 0; i < indicators.Length; i++)
                                       {
                                           var x = (indicators[i] / 255f) * sliderWidth;
                                           sliderIndicatorLine.Draw(
                                               new[]
                                                   {
                                                       new Vector2(
                                                           pickerX + BorderOffset + greenWidth + TextOffset + x, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + ((i + 1) * SliderOffset) + (i * SliderHeight)), 
                                                       new Vector2(
                                                           pickerX + BorderOffset + greenWidth + TextOffset + x, 
                                                           pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                                           + ((i + 1) * SliderOffset) + ((i + 1) * SliderHeight))
                                                   }, 
                                               new ColorBGRA(50, 154, 205, 255));
                                       }

                                       sliderIndicatorLine.End();
                                       sliderIndicatorLine.Dispose();
                                   }

                                   previewLine.Dispose();
                               }, 
                           PreviewBoundaries =
                               (position, component) =>
                               new Rectangle(
                                   (int)(position.X + component.MenuWidth - DefaultSettings.ContainerHeight), 
                                   (int)position.Y, 
                                   DefaultSettings.ContainerHeight, 
                                   DefaultSettings.ContainerHeight), 
                           PickerBoundaries =
                               (position, component) =>
                               new Rectangle(pickerX, pickerY, PickerWidth, pickerHeight), 
                           RedPickerBoundaries =
                               (position, component) =>
                               new Rectangle(
                                   pickerX + BorderOffset + greenWidth + TextOffset, 
                                   pickerY + BorderOffset + DefaultSettings.ContainerHeight + SliderOffset, 
                                   sliderWidth, 
                                   SliderHeight), 
                           GreenPickerBoundaries =
                               (position, component) =>
                               new Rectangle(
                                   pickerX + BorderOffset + greenWidth + TextOffset, 
                                   pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                   + (2 * SliderOffset) + SliderHeight, 
                                   sliderWidth, 
                                   SliderHeight), 
                           BluePickerBoundaries =
                               (position, component) =>
                               new Rectangle(
                                   pickerX + BorderOffset + greenWidth + TextOffset, 
                                   pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                   + (3 * SliderOffset) + (2 * SliderHeight), 
                                   sliderWidth, 
                                   SliderHeight), 
                           AlphaPickerBoundaries =
                               (position, component) =>
                               new Rectangle(
                                   pickerX + BorderOffset + greenWidth + TextOffset, 
                                   pickerY + BorderOffset + DefaultSettings.ContainerHeight
                                   + (4 * SliderOffset) + (3 * SliderHeight), 
                                   sliderWidth, 
                                   SliderHeight), 
                           Width = menuButton => DefaultSettings.ContainerHeight, 
                           SliderWidth = color => sliderWidth
                       };
        }

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
            const int ArrowSpacing = 6;
            const int TextSpacing = 8;
            var arrowSize = DefaultSettings.Font.MeasureText(null, "V", 0);
            var dropDownButtonWidth = arrowSize.Width + (2 * ArrowSpacing);
            return new DrawableList
                       {
                           OnDraw = (component, position, index) =>
                               {
                                   var list = (MenuList)((MenuItem)component).ValueAsObject;
                                   var dropdownMenuWidth = dropDownButtonWidth + (2 * TextSpacing) + list.MaxStringWidth;

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
                                                     Antialias = false, GLLines = false, Width = dropDownButtonWidth 
                                                  };

                                   line.Begin();
                                   line.Draw(
                                       new[]
                                           {
                                               new Vector2(position.X + component.MenuWidth - (dropDownButtonWidth / 2f), position.Y + 1), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - (dropDownButtonWidth / 2f), 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       DefaultSettings.HoverColor);
                                   line.End();
                                   if (list.Hovering || list.Active)
                                   {
                                       line.Width = DefaultSettings.ContainerHeight;
                                       line.Begin();
                                       line.Draw(
                                           new[]
                                               {
                                                   new Vector2(
                                                       position.X + component.MenuWidth - dropdownMenuWidth, 
                                                       position.Y + line.Width / 2), 
                                                   new Vector2(position.X + component.MenuWidth, position.Y + line.Width / 2)
                                               }, 
                                           DefaultSettings.HoverColor);
                                       line.End();
                                   }

                                   line.Dispose();
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       "V", 
                                       (int)(position.X + component.MenuWidth - dropDownButtonWidth + ArrowSpacing), 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);
                                   DefaultSettings.ContainerSeparatorLine.Draw(
                                       new[]
                                           {
                                               new Vector2(position.X + component.MenuWidth - dropDownButtonWidth - 1, position.Y + 1), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - dropDownButtonWidth - 1, 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       DefaultSettings.ContainerSeparatorColor);
                                   DefaultSettings.Font.DrawText(
                                       null, 
                                       list.SelectedValueAsObject.ToString(), 
                                       (int)position.X + component.MenuWidth - dropDownButtonWidth - TextSpacing - list.MaxStringWidth, 
                                       (int)rectangleName.Y, 
                                       DefaultSettings.TextColor);
                                   DefaultSettings.ContainerSeparatorLine.Draw(
                                       new[]
                                           {
                                               new Vector2(
                                                   position.X + component.MenuWidth - dropDownButtonWidth - (2 * TextSpacing)
                                                   - list.MaxStringWidth, 
                                                   position.Y + 1), 
                                               new Vector2(
                                                   position.X + component.MenuWidth - dropDownButtonWidth - (2 * TextSpacing)
                                                   - list.MaxStringWidth, 
                                                   position.Y + DefaultSettings.ContainerHeight)
                                           }, 
                                       DefaultSettings.ContainerSeparatorColor);

                                   if (list.Active)
                                   {
                                       var valueStrings = list.ValuesAsStrings;
                                       var dropdownMenuHeight = valueStrings.Length * DefaultSettings.ContainerHeight;
                                       MenuManager.Instance.DrawDelayed(
                                           delegate
                                               {
                                                   var backgroundLine = new Line(Drawing.Direct3DDevice)
                                                                            {
                                                                                Width = dropdownMenuWidth, Antialias = false, 
                                                                                GLLines = false
                                                                            };
                                                   backgroundLine.Begin();
                                                   backgroundLine.Draw(
                                                       new[]
                                                           {
                                                               new Vector2(
                                                                   position.X + component.MenuWidth - (backgroundLine.Width / 2), 
                                                                   position.Y + DefaultSettings.ContainerHeight), 
                                                               new Vector2(
                                                                   position.X + component.MenuWidth - (backgroundLine.Width / 2), 
                                                                   position.Y + DefaultSettings.ContainerHeight + dropdownMenuHeight), 
                                                           }, 
                                                       Color.Black);
                                                   backgroundLine.End();
                                                   backgroundLine.Dispose();

                                                   var x =
                                                       (int)
                                                       (position.X + component.MenuWidth - dropDownButtonWidth - TextSpacing
                                                        - list.MaxStringWidth);
                                                   var y = (int)rectangleName.Y;
                                                   for (var i = 0; i < valueStrings.Length; i++)
                                                   {
                                                       if (i == list.HoveringIndex)
                                                       {
                                                           var hoverLine = new Line(Drawing.Direct3DDevice)
                                                                               {
                                                                                   Width = DefaultSettings.ContainerHeight, 
                                                                                   Antialias = false, GLLines = false
                                                                               };
                                                           hoverLine.Begin();
                                                           hoverLine.Draw(
                                                               new[]
                                                                   {
                                                                       new Vector2(
                                                                           position.X + component.MenuWidth - dropdownMenuWidth, 
                                                                           position.Y + ((i + 1) * DefaultSettings.ContainerHeight)
                                                                           + DefaultSettings.ContainerHeight / 2f), 
                                                                       new Vector2(
                                                                           position.X + component.MenuWidth, 
                                                                           position.Y + ((i + 1) * DefaultSettings.ContainerHeight)
                                                                           + DefaultSettings.ContainerHeight / 2f)
                                                                   }, 
                                                               DefaultSettings.HoverColor);
                                                           hoverLine.End();
                                                           hoverLine.Dispose();
                                                       }

                                                       DefaultSettings.ContainerSeparatorLine.Draw(
                                                           new[]
                                                               {
                                                                   new Vector2(
                                                                       position.X + component.MenuWidth - dropdownMenuWidth, 
                                                                       position.Y + (DefaultSettings.ContainerHeight * (i + 1))), 
                                                                   new Vector2(
                                                                       position.X + component.MenuWidth, 
                                                                       position.Y + (DefaultSettings.ContainerHeight * (i + 1))), 
                                                               }, 
                                                           DefaultSettings.ContainerSeparatorColor);
                                                       y += DefaultSettings.ContainerHeight;
                                                       DefaultSettings.Font.DrawText(
                                                           null, 
                                                           valueStrings[i], 
                                                           x, 
                                                           y, 
                                                           DefaultSettings.TextColor);
                                                       if (list.Index == i)
                                                       {
                                                           var checkmarkWidth =
                                                               DefaultSettings.Font.MeasureText(null, "\u221A", 0).Width;
                                                           DefaultSettings.Font.DrawText(
                                                               null, 
                                                               "\u221A", 
                                                               (int)(position.X + component.MenuWidth - checkmarkWidth - TextSpacing), 
                                                               y, 
                                                               DefaultSettings.TextColor);
                                                       }
                                                   }

                                                   DefaultSettings.ContainerSeparatorLine.Draw(
                                                       new[]
                                                           {
                                                               new Vector2(
                                                                   position.X + component.MenuWidth - dropdownMenuWidth, 
                                                                   position.Y + DefaultSettings.ContainerHeight), 
                                                               new Vector2(
                                                                   position.X + component.MenuWidth - dropdownMenuWidth, 
                                                                   position.Y
                                                                   + DefaultSettings.ContainerHeight * (valueStrings.Length + 1)), 
                                                               new Vector2(
                                                                   position.X + component.MenuWidth, 
                                                                   position.Y
                                                                   + DefaultSettings.ContainerHeight * (valueStrings.Length + 1)), 
                                                               new Vector2(
                                                                   position.X + component.MenuWidth, 
                                                                   position.Y + DefaultSettings.ContainerHeight), 
                                                           }, 
                                                       DefaultSettings.ContainerSeparatorColor);
                                               });
                                   }
                               }, 
                           Dropdown =
                               (position, component, menulist) =>
                               new Rectangle(
                                   (int)
                                   (position.X + component.MenuWidth - dropDownButtonWidth
                                    - (2 * TextSpacing) - menulist.MaxStringWidth), 
                                   (int)position.Y, 
                                   dropDownButtonWidth + (2 * TextSpacing) + menulist.MaxStringWidth, 
                                   DefaultSettings.ContainerHeight), 
                           Width = list => list.MaxStringWidth + (2 * TextSpacing) + dropDownButtonWidth, 
                           DropdownList =
                               delegate(Vector2 position, AMenuComponent component, MenuList menulist)
                                   {
                                       var rectangles = new List<Rectangle>();
                                       for (var i = 0; i < menulist.Count; i++)
                                       {
                                           rectangles.Add(
                                               new Rectangle(
                                                   (int)
                                                   (position.X + component.MenuWidth - dropDownButtonWidth
                                                    - (2 * TextSpacing) - menulist.MaxStringWidth), 
                                                   (int)
                                                   (position.Y + ((i + 1) * DefaultSettings.ContainerHeight)), 
                                                   dropDownButtonWidth + (2 * TextSpacing)
                                                   + menulist.MaxStringWidth, 
                                                   DefaultSettings.ContainerHeight + 1));
                                       }

                                       return rectangles;
                                   }, 
                           EntireDropDown =
                               (position, component, menulist) =>
                               new Rectangle(
                                   (int)
                                   (position.X + component.MenuWidth - dropDownButtonWidth
                                    - (2 * TextSpacing) - menulist.MaxStringWidth), 
                                   (int)position.Y, 
                                   dropDownButtonWidth + (2 * TextSpacing) + menulist.MaxStringWidth, 
                                   (menulist.Count + 1) * DefaultSettings.ContainerHeight)
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
                                       (int)(position.X + component.MenuWidth - 5 - measureText.Width), 
                                       centeredY, 
                                       DefaultSettings.TextColor);

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

        #endregion
    }
}