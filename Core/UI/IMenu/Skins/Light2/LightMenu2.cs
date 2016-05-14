// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightMenu2.cs" company="LeagueSharp">
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
//   Provides a custom implementation of <see cref="ADrawable{Menu}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDKEx.UI.Skins.Light2
{
    using System.Linq;

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.UI.Skins.Light;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Provides a default implementation of <see cref="ADrawable{Menu}" />
    /// </summary>
    public class LightMenu2 : LightMenu
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     Gets or sets a value indicating whether the user is dragging the menu.
        /// </summary>
        private bool dragging;

        /// <summary>
        ///     Gets or sets a value indicating whether the user has moved the menu at least 1 pixel.
        /// </summary>
        private bool hasDragged;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Menu" /> is hovering.
        /// </summary>
        /// <value>
        ///     <c>true</c> if hovering; otherwise, <c>false</c>.
        /// </value>
        private bool hovering;

        /// <summary>
        ///     The x-axis.
        /// </summary>
        private float xd;

        /// <summary>
        ///     The y-axis.
        /// </summary>
        private float yd;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightMenu" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public LightMenu2(Menu component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        ///     Draws an Menu
        /// </summary>
        public override void Draw()
        {
            var position = this.Component.Position;
            if (this.hovering && !this.Component.Toggled && this.Component.Components.Count > 0)
            {
                Line.Width = MenuSettings.ContainerHeight;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X, position.Y + MenuSettings.ContainerHeight / 2f),
                            new Vector2(
                                position.X + this.Component.MenuWidth,
                                position.Y + MenuSettings.ContainerHeight / 2f)
                        },
                    MenuSettings.HoverColor);
                Line.End();
            }

            var centerY =
                (int)
                LightUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(
                        null,
                        MenuSettings.Font,
                        MultiLanguage.Translation(this.Component.DisplayName),
                        CenteredFlags.VerticalCenter)
                    .Y;

            if (this.Component.Toggled)
            {
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    (int)(position.X + MenuSettings.ContainerTextOffset),
                    centerY,
                    new ColorBGRA(237, 245, 254, 255));

                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    "\u25B6",
                    (int)
                    (position.X + this.Component.MenuWidth - MenuSettings.ContainerTextMarkWidth
                     - MenuSettings.ContainerTextMarkOffset),
                    centerY,
                    this.Component.Components.Count > 0
                        ? new ColorBGRA(237, 245, 254, 255)
                        : MenuSettings.ContainerSeparatorColor);
            }
            else
            {
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    (int)(position.X + MenuSettings.ContainerTextOffset),
                    centerY,
                    new ColorBGRA(62, 151, 251, 255));

                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    "\u25B6",
                    (int)
                    (position.X + this.Component.MenuWidth - MenuSettings.ContainerTextMarkWidth
                     - MenuSettings.ContainerTextMarkOffset),
                    centerY,
                    this.Component.Components.Count > 0
                        ? new ColorBGRA(5, 151, 250, 255)
                        : MenuSettings.ContainerSeparatorColor);
            }

            if (this.Component.Toggled)
            {
                Line.Width = this.Component.MenuWidth;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth / 2f, position.Y),
                            new Vector2(
                                position.X + this.Component.MenuWidth / 2f,
                                position.Y + MenuSettings.ContainerHeight)
                        },
                    MenuSettings.ContainerSelectedColor);
                Line.End();

                float height = MenuSettings.ContainerHeight * this.Component.Components.Count;
                var width = MenuSettings.ContainerWidth;
                if (this.Component.Components.Count > 0)
                {
                    width = this.Component.Components.First().Value.MenuWidth;
                }

                Line.Width = width;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2((position.X + this.Component.MenuWidth) + width / 2, position.Y),
                            new Vector2((position.X + this.Component.MenuWidth) + width / 2, position.Y + height)
                        },
                    new ColorBGRA(255, 255, 255, 255));
                Line.End();

                for (var i = 0; i < this.Component.Components.Count; ++i)
                {
                    var childComponent = this.Component.Components.Values.ToList()[i];
                    if (childComponent != null)
                    {
                        var childPos = new Vector2(
                            position.X + this.Component.MenuWidth,
                            position.Y + i * MenuSettings.ContainerHeight);

                        childComponent.OnDraw(childPos);
                    }
                }

                var contourColor = new ColorBGRA(254, 255, 255, 255);

                Line.Width = 1f;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y),
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y)
                        },
                    contourColor);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y + height),
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y + height)
                        },
                    contourColor);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth, position.Y),
                            new Vector2(position.X + this.Component.MenuWidth, position.Y + height)
                        },
                    contourColor);
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y),
                            new Vector2(position.X + this.Component.MenuWidth + width, position.Y + height)
                        },
                    contourColor);
                Line.End();
            }

            if (this.hasDragged && !MenuCustomizer.Instance.LockPosition.Value)
            {
                var sprite = MenuManager.Instance.Sprite;
                var oldMatrix = sprite.Transform;
                var y =
                    (int)(MenuSettings.Position.Y + (MenuManager.Instance.Menus.Count * MenuSettings.ContainerHeight));
                var dragTexture = LightTextures2.Instance[LightTexture2.Dragging];
                var x = MenuSettings.Position.X - dragTexture.Width;
                sprite.Transform = Matrix.Translation(x - 1, y + 2, 0);
                sprite.Draw(dragTexture.Texture, Color.White);
                sprite.Transform = oldMatrix;

                Line.Width = 1f;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(x - 1, y + 1), new Vector2(x - 1 + dragTexture.Width, y + 1),
                            new Vector2(x - 1 + dragTexture.Width, y + dragTexture.Width + 2),
                            new Vector2(x - 2, y + dragTexture.Width + 2), new Vector2(x - 2, y),
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
            if (this.Component.Visible)
            {
                if (args.Msg == WindowsMessages.MOUSEMOVE && this.dragging
                    && !MenuCustomizer.Instance.LockPosition.Value)
                {
                    MenuSettings.Position = new Vector2(args.Cursor.X - this.xd, args.Cursor.Y - this.yd);
                    this.hasDragged = true;
                }

                if (args.Cursor.IsUnderRectangle(
                    this.Component.Position.X,
                    this.Component.Position.Y,
                    this.Component.MenuWidth,
                    MenuSettings.ContainerHeight))
                {
                    if (args.Msg == WindowsMessages.LBUTTONDOWN && this.Component.Root)
                    {
                        var pos = MenuSettings.Position;
                        this.xd = args.Cursor.X - pos.X;
                        this.yd = args.Cursor.Y - pos.Y;
                        this.dragging = true;
                    }

                    this.hovering = true;
                    if (args.Msg == WindowsMessages.LBUTTONUP && !this.hasDragged)
                    {
                        this.Component.Toggle();
                    }
                }
                else
                {
                    this.hovering = false;
                }
            }

            if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.hasDragged = false;
                this.dragging = false;
            }
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
                (LightUtilities.MeasureString(MultiLanguage.Translation(this.Component.DisplayName) + " \u25B6").Width
                 + (MenuSettings.ContainerTextOffset * 2) + MenuSettings.ContainerTextMarkWidth);
        }

        #endregion
    }
}