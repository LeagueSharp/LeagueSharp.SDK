using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Drawing;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Properties;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;

    /// <summary>
    /// Provides a default implementation of <see cref="IDrawable{Menu}"/>
    /// </summary>
    public class DefaultMenu : DefaultComponent, IDrawable<Menu>
    {

        private readonly Texture dragTexture;

        private const int DragTextureSize = 16;

        /// <summary>
        /// Default constructor of <see cref="DefaultMenu"/>
        /// </summary>
        public DefaultMenu()
        {
            Bitmap resized = new Bitmap(Resources.cursor_drag, DragTextureSize, DragTextureSize);
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

        /// <summary>
        ///     Draws an Menu
        /// </summary>
        /// <param name="menuComponent">The <see cref="Menu" /></param>
        public virtual void Draw(Menu menuComponent)
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

            if (menuComponent.HasDragged)
            {
                var sprite = MenuManager.Instance.Sprite;
                var oldMatrix = sprite.Transform;
                int y = (int)(MenuSettings.Position.Y + (MenuManager.Instance.Menus.Count * MenuSettings.ContainerHeight));
                float x = MenuSettings.Position.X - DragTextureSize;
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
                            new Vector2(x - 1 + DragTextureSize, y + 1),
                            new Vector2(x - 1 + DragTextureSize, y + DragTextureSize + 2), 
                            new Vector2(x - 2, y + DragTextureSize + 2),
                            new Vector2(x - 2, y),
                        },
                        MenuSettings.ContainerSeparatorColor);
                line.End();
                line.Dispose();
            }
        }

        /// <summary>
        /// Calculates the Width of an AMenuComponent
        /// </summary>
        /// <param name="component">menu component</param>
        /// <returns>width</returns>
        public virtual int Width(Menu component)
        {
            return
                (int)
                (MeasureString(component.DisplayName + " »").Width + (MenuSettings.ContainerTextOffset * 2)
                 + MenuSettings.ContainerTextMarkWidth);
        }

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event data</param>
        public virtual void OnWndProc(Menu component, WindowsKeys args)
        {
            //
        }
    }
}
