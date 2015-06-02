using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// Implements a default ITheme.
    /// </summary>
    public class DefaultTheme : DefaultComponent, ITheme
    {
        private readonly DefaultBool drawableBool;
        private readonly DefaultColorPicker drawableColorPicker;
        private readonly DefaultButton drawableButton;
        private readonly DefaultKeyBind drawableKeyBind;
        private readonly IDrawableList drawableList;
        private readonly DefaultSeparator drawableSeparator;
        private readonly DefaultSlider drawableSlider;

        /// <summary>
        /// Creates a new instance of DefaultTheme
        /// </summary>
        public DefaultTheme()
        {
            drawableBool = new DefaultBool();
            drawableColorPicker = new DefaultColorPicker();
            drawableButton = new DefaultButton();
            drawableKeyBind = new DefaultKeyBind();
            drawableList = new DefaultList();
            drawableSeparator= new DefaultSeparator();
            drawableSlider = new DefaultSlider();
        }

        /// <summary>
        /// Gets the IDrawableBool
        /// </summary>
        public IDrawableBool Bool
        {
            get { return drawableBool; }
        }

        /// <summary>
        /// Gets the IDrawableColorPicker
        /// </summary>
        public IDrawableColorPicker ColorPicker
        {
            get{return drawableColorPicker;}
        }

        /// <summary>
        /// Gets the IDrawableButton
        /// </summary>
        public IDrawableButton Button
        {
            get { return drawableButton; }
        }

        /// <summary>
        /// Gets the IDrawableKeyBind
        /// </summary>
        public IDrawableKeyBind KeyBind
        {
            get { return drawableKeyBind; }
        }

        /// <summary>
        /// Gets the IDrawableList
        /// </summary>
        public IDrawableList List
        {
            get { return drawableList; }
        }

        /// <summary>
        /// Gets the IDrawableSeparator
        /// </summary>
        public IDrawableSeparator Separator
        {
            get { return drawableSeparator; }
        }

        /// <summary>
        /// Gets the IDrawableSlider
        /// </summary>
        public IDrawableSlider Slider
        {
            get { return drawableSlider; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public int CalcWidthMenu(Menu menu)
        {
            return
                (int)
                (CalcString(menu.DisplayName + " »").Width + (DefaultSettings.ContainerTextOffset * 2)
                 + DefaultSettings.ContainerTextMarkWidth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public int CalcWidthItem(MenuItem menuItem)
        {
            return (int)(CalcString(menuItem.DisplayName).Width + (DefaultSettings.ContainerTextOffset * 2));
        }

        private Rectangle CalcString(string text)
        {
            return DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Draws a menu
        /// </summary>
        /// <param name="menuComponent">Menu</param>
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
                    .GetCenteredText(null, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
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
    }
}
