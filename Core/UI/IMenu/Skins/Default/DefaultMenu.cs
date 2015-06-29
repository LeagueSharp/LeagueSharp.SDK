// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMenu.cs" company="LeagueSharp">
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
//   Provides a default implementation of <see cref="ADrawable{Menu}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Properties;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;

    /// <summary>
    ///     Provides a default implementation of <see cref="ADrawable{Menu}" />
    /// </summary>
    public class DefaultMenu : ADrawable<Menu>
    {
        #region Constants

        /// <summary>
        ///     The drag texture size.
        /// </summary>
        private const int DragTextureSize = 16;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { Width = 1f, GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     The drag texture.
        /// </summary>
        private readonly Texture dragTexture;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultMenu" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public DefaultMenu(Menu component)
            : base(component)
        {
            var resized = new Bitmap(Resources.cursor_drag, DragTextureSize, DragTextureSize);
            this.dragTexture = Texture.FromMemory(
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

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            this.dragTexture.Dispose();
        }

        /// <summary>
        ///     Draws an Menu
        /// </summary>
        public override void Draw()
        {
            var position = this.Component.Position;
            if (this.Component.Hovering && !this.Component.Toggled && this.Component.Components.Count > 0)
            {
                MenuSettings.HoverLine.Begin();
                MenuSettings.HoverLine.Draw(
                    new[]
                        {
                            new Vector2(position.X, position.Y + MenuSettings.ContainerHeight / 2f), 
                            new Vector2(
                                position.X + this.Component.MenuWidth, 
                                position.Y + MenuSettings.ContainerHeight / 2f)
                        }, 
                    MenuSettings.HoverColor);
                MenuSettings.HoverLine.End();
            }

            var centerY =
                (int)
                DefaultUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                centerY, 
                MenuSettings.TextColor);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                "»", 
                (int)
                (position.X + this.Component.MenuWidth - MenuSettings.ContainerTextMarkWidth
                 - MenuSettings.ContainerTextMarkOffset), 
                centerY, 
                this.Component.Components.Count > 0 ? MenuSettings.TextColor : MenuSettings.ContainerSeparatorColor);

            if (this.Component.Toggled)
            {
                MenuSettings.ContainerLine.Width = this.Component.MenuWidth;
                MenuSettings.ContainerLine.Begin();
                MenuSettings.ContainerLine.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth / 2f, position.Y), 
                            new Vector2(
                                position.X + this.Component.MenuWidth / 2f, 
                                position.Y + MenuSettings.ContainerHeight)
                        }, 
                    MenuSettings.ContainerSelectedColor);
                MenuSettings.ContainerLine.End();

                float height = MenuSettings.ContainerHeight * this.Component.Components.Count;
                var width = MenuSettings.ContainerWidth;
                if (this.Component.Components.Count > 0)
                {
                    width = this.Component.Components.First().Value.MenuWidth;
                }

                MenuSettings.ContainerLine.Width = width;
                MenuSettings.ContainerLine.Begin();
                MenuSettings.ContainerLine.Draw(
                    new[]
                        {
                            new Vector2((position.X + this.Component.MenuWidth) + width / 2, position.Y), 
                            new Vector2((position.X + this.Component.MenuWidth) + width / 2, position.Y + height)
                        }, 
                    MenuSettings.RootContainerColor);
                MenuSettings.ContainerLine.End();

                for (var i = 0; i < this.Component.Components.Count; ++i)
                {
                    var childComponent = this.Component.Components.Values.ToList()[i];
                    if (childComponent != null)
                    {
                        var childPos = new Vector2(
                            position.X + this.Component.MenuWidth, 
                            position.Y + i * MenuSettings.ContainerHeight);

                        if (i < this.Component.Components.Count - 1)
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

                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y), 
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y)
                        }, 
                    Color.Black);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y + height), 
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y + height)
                        }, 
                    Color.Black);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y), 
                            new Vector2(position.X + this.Component.MenuWidth, position.Y + height)
                        }, 
                    Color.Black);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y), 
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y + height)
                        }, 
                    Color.Black);
                Line.End();
            }

            if (this.Component.HasDragged)
            {
                var sprite = MenuManager.Instance.Sprite;
                var oldMatrix = sprite.Transform;
                var y =
                    (int)(MenuSettings.Position.Y + (MenuManager.Instance.Menus.Count * MenuSettings.ContainerHeight));
                var x = MenuSettings.Position.X - DragTextureSize;
                sprite.Transform = Matrix.Translation(x - 1, y + 2, 0);
                sprite.Draw(this.dragTexture, Color.White);
                sprite.Transform = oldMatrix;

                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(x - 1, y + 1), new Vector2(x - 1 + DragTextureSize, y + 1), 
                            new Vector2(x - 1 + DragTextureSize, y + DragTextureSize + 2), 
                            new Vector2(x - 2, y + DragTextureSize + 2), new Vector2(x - 2, y), 
                        }, 
                    MenuSettings.ContainerSeparatorColor);
                Line.End();
            }
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
        }

        /// <summary>
        ///     Calculates the Width of an AMenuComponent
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return
                (int)
                (DefaultUtilities.MeasureString(this.Component.DisplayName + " »").Width
                 + (MenuSettings.ContainerTextOffset * 2) + MenuSettings.ContainerTextMarkWidth);
        }

        #endregion
    }
}