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
//   Implements a default ITheme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Implements a default ITheme.
    /// </summary>
    public class DefaultTheme : DefaultComponent, ITheme
    {
        #region Fields

        /// <summary>
        ///     The <c>drawable</c> <c>bool</c>.
        /// </summary>
        private readonly DefaultBool drawableBool;

        /// <summary>
        ///     The <c>drawable</c> button.
        /// </summary>
        private readonly DefaultButton drawableButton;

        /// <summary>
        ///     The <c>drawable</c> color picker.
        /// </summary>
        private readonly DefaultColorPicker drawableColorPicker;

        /// <summary>
        ///     The <c>drawable</c> key bind.
        /// </summary>
        private readonly DefaultKeyBind drawableKeyBind;

        /// <summary>
        ///     The <c>drawable</c> list.
        /// </summary>
        private readonly IDrawableList drawableList;

        /// <summary>
        ///     The <c>drawable</c> separator.
        /// </summary>
        private readonly DefaultSeparator drawableSeparator;

        /// <summary>
        ///     The <c>drawable</c> slider.
        /// </summary>
        private readonly DefaultSlider drawableSlider;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTheme" /> class.
        ///     Creates a new instance of DefaultTheme
        /// </summary>
        public DefaultTheme()
        {
            this.drawableBool = new DefaultBool();
            this.drawableColorPicker = new DefaultColorPicker();
            this.drawableButton = new DefaultButton();
            this.drawableKeyBind = new DefaultKeyBind();
            this.drawableList = new DefaultList();
            this.drawableSeparator = new DefaultSeparator();
            this.drawableSlider = new DefaultSlider();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="IDrawableBool" />
        /// </summary>
        public IDrawableBool Bool
        {
            get
            {
                return this.drawableBool;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableButton" />
        /// </summary>
        public IDrawableButton Button
        {
            get
            {
                return this.drawableButton;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableColorPicker" />
        /// </summary>
        public IDrawableColorPicker ColorPicker
        {
            get
            {
                return this.drawableColorPicker;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableKeyBind" />
        /// </summary>
        public IDrawableKeyBind KeyBind
        {
            get
            {
                return this.drawableKeyBind;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableList" />
        /// </summary>
        public IDrawableList List
        {
            get
            {
                return this.drawableList;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableSeparator" />
        /// </summary>
        public IDrawableSeparator Separator
        {
            get
            {
                return this.drawableSeparator;
            }
        }

        /// <summary>
        ///     Gets the <see cref="IDrawableSlider" />
        /// </summary>
        public IDrawableSlider Slider
        {
            get
            {
                return this.drawableSlider;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculate the item's width.
        /// </summary>
        /// <param name="menuItem">The <see cref="MenuItem" /></param>
        /// <returns>The width</returns>
        public int CalcWidthItem(MenuItem menuItem)
        {
            return (int)(MeasureString(menuItem.DisplayName).Width + (DefaultSettings.ContainerTextOffset * 2));
        }

        /// <summary>
        ///     Calculate the menu's width.
        /// </summary>
        /// <param name="menu">The <see cref="Menu" /></param>
        /// <returns>The width</returns>
        public int CalcWidthMenu(Menu menu)
        {
            return
                (int)
                (MeasureString(menu.DisplayName + " »").Width + (DefaultSettings.ContainerTextOffset * 2)
                 + DefaultSettings.ContainerTextMarkWidth);
        }

        /// <summary>
        ///     Draws a <see cref="Menu" />
        /// </summary>
        /// <param name="menuComponent">The <see cref="Menu" /></param>
        public void DrawMenu(Menu menuComponent)
        {
            var position = menuComponent.Position;
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
                GetContainerRectangle(menuComponent)
                    .GetCenteredText(null, DefaultSettings.Font, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                menuComponent.DisplayName, 
                (int)(position.X + DefaultSettings.ContainerTextOffset), 
                centerY, 
                DefaultSettings.TextColor);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
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

                        childComponent.OnDraw(childPos);
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

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        /// <param name="position">The position</param>
        public void OnDraw(Vector2 position)
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

                menuManager.Menus[i].OnDraw(childPos);
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

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the string measurements.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <returns>
        ///     The measured rectangle.
        /// </returns>
        private static Rectangle MeasureString(string text)
        {
            return DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0);
        }

        #endregion
    }
}