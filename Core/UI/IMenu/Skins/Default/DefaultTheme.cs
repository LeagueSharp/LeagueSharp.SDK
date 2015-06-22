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
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Properties;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Rectangle = SharpDX.Rectangle;

    /// <summary>
    ///     Implements a default ITheme.
    /// </summary>
    public class DefaultTheme : DefaultComponent, ITheme
    {
        #region Fields

        /// <summary>
        ///     The <c>drawable</c> <c>bool</c>.
        /// </summary>
        private readonly IDrawableBool drawableBool;

        /// <summary>
        ///     The <c>drawable</c> button.
        /// </summary>
        private readonly IDrawableButton drawableButton;

        /// <summary>
        ///     The <c>drawable</c> color picker.
        /// </summary>
        private readonly IDrawableColorPicker drawableColorPicker;

        /// <summary>
        ///     The <c>drawable</c> key bind.
        /// </summary>
        private readonly IDrawableKeyBind drawableKeyBind;

        /// <summary>
        ///     The <c>drawable</c> list.
        /// </summary>
        private readonly IDrawableList drawableList;

        /// <summary>
        ///     The <c>drawable</c> separator.
        /// </summary>
        private readonly IDrawableSeparator drawableSeparator;

        /// <summary>
        ///     The <c>drawable</c> slider.
        /// </summary>
        private readonly IDrawableSlider drawableSlider;

        #endregion


        private Texture dragTexture;

        private const int dragTextureSize = 16;

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

            Bitmap resized = new Bitmap(Resources.cursor_drag, dragTextureSize, dragTextureSize);
            dragTexture = Texture.FromMemory(
                Drawing.Direct3DDevice,
                (byte[])new ImageConverter().ConvertTo(resized, typeof(byte[])),
                resized.Width,
                resized.Height,
                0,
                Usage.None,
                Format.A1,
                Pool.Managed,
                Filter.Default,
                Filter.Default,
                0);
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
            return (int)(MeasureString(menuItem.DisplayName).Width + (MenuSettings.ContainerTextOffset * 2));
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
                (MeasureString(menu.DisplayName + " »").Width + (MenuSettings.ContainerTextOffset * 2)
                 + MenuSettings.ContainerTextMarkWidth);
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
                MenuSettings.HoverLine.Begin();
                MenuSettings.HoverLine.Draw(
                    new[]
                        {
                            new Vector2(position.X, position.Y + MenuSettings.ContainerHeight / 2f), 
                            new Vector2(
                                position.X + menuComponent.MenuWidth, 
                                position.Y + MenuSettings.ContainerHeight / 2f)
                        }, 
                    MenuSettings.HoverColor);
                MenuSettings.HoverLine.End();
            }

            var centerY =
                (int)
                GetContainerRectangle(menuComponent)
                    .GetCenteredText(null, MenuSettings.Font, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                menuComponent.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                centerY, 
                MenuSettings.TextColor);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                "»", 
                (int)
                (position.X + menuComponent.MenuWidth - MenuSettings.ContainerTextMarkWidth
                 - MenuSettings.ContainerTextMarkOffset), 
                centerY, 
                menuComponent.Components.Count > 0 ? MenuSettings.TextColor : MenuSettings.ContainerSeparatorColor);

            if (menuComponent.Toggled)
            {
                MenuSettings.ContainerLine.Width = menuComponent.MenuWidth;
                MenuSettings.ContainerLine.Begin();
                MenuSettings.ContainerLine.Draw(
                    new[]
                        {
                            new Vector2(position.X + menuComponent.MenuWidth / 2f, position.Y), 
                            new Vector2(
                                position.X + menuComponent.MenuWidth / 2f, 
                                position.Y + MenuSettings.ContainerHeight)
                        }, 
                    MenuSettings.ContainerSelectedColor);
                MenuSettings.ContainerLine.End();

                float height = MenuSettings.ContainerHeight * menuComponent.Components.Count;
                var width = MenuSettings.ContainerWidth;
                if (menuComponent.Components.Count > 0)
                {
                    width = menuComponent.Components.First().Value.MenuWidth;
                }

                MenuSettings.ContainerLine.Width = width;
                MenuSettings.ContainerLine.Begin();
                MenuSettings.ContainerLine.Draw(
                    new[]
                        {
                            new Vector2((position.X + menuComponent.MenuWidth) + width / 2, position.Y), 
                            new Vector2((position.X + menuComponent.MenuWidth) + width / 2, position.Y + height)
                        }, 
                    MenuSettings.RootContainerColor);
                MenuSettings.ContainerLine.End();

                for (var i = 0; i < menuComponent.Components.Count; ++i)
                {
                    var childComponent = menuComponent.Components.Values.ToList()[i];
                    if (childComponent != null)
                    {
                        var childPos = new Vector2(
                            position.X + menuComponent.MenuWidth, 
                            position.Y + i * MenuSettings.ContainerHeight);

                        if (i < menuComponent.Components.Count - 1)
                        {
                            MenuSettings.ContainerSeparatorLine.Begin();
                            MenuSettings.ContainerSeparatorLine.Draw(
                                new[]
                                    {
                                        new Vector2(childPos.X, childPos.Y + MenuSettings.ContainerHeight), 
                                        new Vector2(
                                            childPos.X + childComponent.MenuWidth, 
                                            childPos.Y + MenuSettings.ContainerHeight)
                                    }, 
                                MenuSettings.ContainerSeparatorColor);
                            MenuSettings.ContainerSeparatorLine.End();
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
        public void Draw()
        {
            var position = MenuSettings.Position;
            var menuManager = MenuManager.Instance;
            var height = MenuSettings.ContainerHeight * menuManager.Menus.Count;
            var width = MenuSettings.ContainerWidth;
            if (menuManager.Menus.Count > 0)
            {
                width = menuManager.Menus.First().MenuWidth;
            }

            MenuSettings.ContainerLine.Width = width;
            MenuSettings.ContainerLine.Begin();
            MenuSettings.ContainerLine.Draw(
                new[]
                    {
                        new Vector2(position.X + (width / 2f), position.Y), 
                        new Vector2(position.X + (width / 2), position.Y + height)
                    }, 
                MenuSettings.RootContainerColor);
            MenuSettings.ContainerLine.End();

            for (var i = 0; i < menuManager.Menus.Count; ++i)
            {
                var childPos = new Vector2(position.X, position.Y + i * MenuSettings.ContainerHeight);

                if (i < menuManager.Menus.Count - 1)
                {
                    MenuSettings.ContainerSeparatorLine.Begin();
                    MenuSettings.ContainerSeparatorLine.Draw(
                        new[]
                            {
                                new Vector2(childPos.X, childPos.Y + MenuSettings.ContainerHeight), 
                                new Vector2(
                                    childPos.X + menuManager.Menus[i].MenuWidth, 
                                    childPos.Y + MenuSettings.ContainerHeight)
                            }, 
                        MenuSettings.ContainerSeparatorColor);
                    MenuSettings.ContainerSeparatorLine.End();
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

            if (menuManager.HasDragged)
            {
                var sprite = MenuManager.Instance.Sprite;
                var oldMatrix = sprite.Transform;
                int y = (int)(MenuSettings.Position.Y + (MenuManager.Instance.Menus.Count * MenuSettings.ContainerHeight));
                float x = MenuSettings.Position.X - dragTextureSize;
                sprite.Transform = Matrix.Translation(x - 1, y + 2, 0);
                sprite.Draw(
                    dragTexture,
                    Color.White);
                sprite.Transform = oldMatrix;

                Line line = new Line(Drawing.Direct3DDevice)
                {
                    Width = 1
                };
                line.Begin();
                line.Draw(
                        new[]
                        {
                            new Vector2(x - 1, y + 1), 
                            new Vector2(x - 1 + dragTextureSize, y + 1),
                            new Vector2(x - 1 + dragTextureSize, y + dragTextureSize + 2), 
                            new Vector2(x - 2, y + dragTextureSize + 2),
                            new Vector2(x - 2, y),
                        },
                        MenuSettings.ContainerSeparatorColor);
                line.End();
                line.Dispose();
            }
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
            return MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0);
        }

        #endregion
    }
}