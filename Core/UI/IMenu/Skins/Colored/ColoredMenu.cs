// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredMenu.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI.Skins.Colored
{
    using System.Linq;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Provides a default implementation of <see cref="ADrawable{Menu}" />
    /// </summary>
    public class ColoredMenu : ADrawable<Menu>
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
        ///     Initializes a new instance of the <see cref="ColoredMenu" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public ColoredMenu(Menu component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws a rounded Box. If Smoothing is true it will draw also a border.
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="radius">Radius</param>
        /// <param name="color">Color</param>
        /// <param name="bcolor">Border Color</param>
        public static void DrawBoxBotRounded(
            float x,
            float y,
            float w,
            float h,
            float radius,
            Color color,
            Color bcolor)
        {
            Utils.DrawBoxFilled(x + radius, y + radius, w - 2 * radius - 1, h - 2 * radius - 1, color); // Center rect.
            Utils.DrawBoxFilled(x + radius, y + 1, w - 2 * radius - 1, radius - 1, color); // Top rect.
            Utils.DrawBoxFilled(x + radius, y + h - radius - 1, w - 2 * radius - 1, radius, color); // Bottom rect.
            Utils.DrawBoxFilled(x + 1, y + radius, radius - 1, h - 2 * radius - 1, color); // Left rect.
            Utils.DrawBoxFilled(x + w - radius - 1, y + radius, radius, h - 2 * radius - 1, color); // Right rect.

            Utils.DrawCircleFilled(x + radius, y + radius, radius - 1, 0, Utils.CircleType.Quarter, true, 16, color);
            // Top-left corner
            Utils.DrawCircleFilled(
                x + w - radius - 1,
                y + radius,
                radius - 1,
                90,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Top-right corner
            Utils.DrawCircleFilled(
                x + w - radius - 1,
                y + h - radius - 1,
                radius - 1,
                180,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Bottom-right corner
            Utils.DrawCircleFilled(
                x + radius,
                y + h - radius - 1,
                radius - 1,
                270,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Bottom-left corner

            Utils.DrawCircle(x + radius + 1, y + radius + 1, radius, 0, Utils.CircleType.Quarter, true, 16, bcolor);
            // Top-left corner
            Utils.DrawCircle(x + w - radius - 1, y + radius + 1, radius, 90, Utils.CircleType.Quarter, true, 16, bcolor);
            // Top-right corner
            Utils.DrawCircle(
                x + w - radius - 1,
                y + h - radius - 1,
                radius,
                180,
                Utils.CircleType.Quarter,
                true,
                16,
                bcolor); // Bottom-right corner
            Utils.DrawCircle(
                x + radius + 1,
                y + h - radius - 1,
                radius,
                270,
                Utils.CircleType.Quarter,
                true,
                16,
                bcolor); // Bottom-left corner

            Utils.DrawLine(x + radius, y + 1, x + w - radius - 1, y + 1, 1, bcolor); // Top line
            Utils.DrawLine(x + radius, y + h - 2, x + w - radius - 1, y + h - 2, 1, bcolor); // Bottom line
            Utils.DrawLine(x + 1, y + radius, x + 1, y + h - radius - 1, 1, bcolor); // Left line
            Utils.DrawLine(x + w - 2, y + radius, x + w - 2, y + h - radius - 1, 1, bcolor); // Right line
        }

        /// <summary>
        ///     Draws a rounded Box. If Smoothing is true it will draw also a border.
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="radius">Radius</param>
        /// <param name="color">Color</param>
        /// <param name="bcolor">Border Color</param>
        public static void DrawBoxTopRounded(
            float x,
            float y,
            float w,
            float h,
            float radius,
            Color color,
            Color bcolor)
        {
            Utils.DrawBoxFilled(x + radius, y + radius, w - 2 * radius - 1, h - 2 * radius - 1, color); // Center rect.
            Utils.DrawBoxFilled(x + radius, y + 1, w - 2 * radius - 1, radius - 1, color); // Top rect.
            Utils.DrawBoxFilled(x + radius, y + h - radius - 1, w - 2 * radius - 1, radius, color); // Bottom rect.
            Utils.DrawBoxFilled(x + 1, y + radius, radius - 1, h - 2 * radius - 1, color); // Left rect.
            Utils.DrawBoxFilled(x + w - radius - 1, y + radius, radius, h - 2 * radius - 1, color); // Right rect.

            Utils.DrawCircleFilled(x + radius, y + radius, radius - 1, 0, Utils.CircleType.Quarter, true, 16, color);
            // Top-left corner
            Utils.DrawCircleFilled(
                x + w - radius - 1,
                y + radius,
                radius - 1,
                90,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Top-right corner
            Utils.DrawCircleFilled(
                x + w - radius - 1,
                y + h - radius - 1,
                radius - 1,
                180,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Bottom-right corner
            Utils.DrawCircleFilled(
                x + radius,
                y + h - radius - 1,
                radius - 1,
                270,
                Utils.CircleType.Quarter,
                true,
                16,
                color); // Bottom-left corner

            Utils.DrawCircle(x + radius + 1, y + radius + 1, radius, 0, Utils.CircleType.Quarter, true, 16, bcolor);
            // Top-left corner
            Utils.DrawCircle(x + w - radius - 1, y + radius + 1, radius, 90, Utils.CircleType.Quarter, true, 16, bcolor);
            // Top-right corner
            Utils.DrawCircle(
                x + w - radius - 1,
                y + h - radius - 1,
                radius,
                180,
                Utils.CircleType.Quarter,
                true,
                16,
                bcolor); // Bottom-right corner
            Utils.DrawCircle(
                x + radius + 1,
                y + h - radius - 1,
                radius,
                270,
                Utils.CircleType.Quarter,
                true,
                16,
                bcolor); // Bottom-left corner

            Utils.DrawLine(x + radius, y + 1, x + w - radius - 1, y + 1, 1, bcolor); // Top line
            Utils.DrawLine(x + radius, y + h - 2, x + w - radius - 1, y + h - 2, 1, bcolor); // Bottom line
            Utils.DrawLine(x + 1, y + radius, x + 1, y + h - radius - 1, 1, bcolor); // Left line
            Utils.DrawLine(x + w - 2, y + radius, x + w - 2, y + h - radius - 1, 1, bcolor); // Right line
        }

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
                ColoredUtilities.GetContainerRectangle(this.Component)
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
            }
            else
            {
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    (int)(position.X + MenuSettings.ContainerTextOffset),
                    centerY,
                    MenuSettings.TextColor);
            }

            MenuManager.Instance.DrawDelayed(
                delegate
                    {
                        var symbolCenterY =
                            (int)
                            ColoredUtilities.GetContainerRectangle(this.Component)
                                .GetCenteredText(
                                    null,
                                    ColoredMenuSettings.FontMenuSymbol,
                                    this.Component.DisplayName,
                                    CenteredFlags.VerticalCenter)
                                .Y;

                        Utils.DrawCircleFilled(
                            (position.X + this.Component.MenuWidth - MenuSettings.ContainerTextMarkWidth
                             - MenuSettings.ContainerTextMarkOffset) + 4,
                            symbolCenterY + 11,
                            6,
                            0,
                            Utils.CircleType.Full,
                            true,
                            16,
                            new ColorBGRA(252, 248, 245, 255));
                        ColoredMenuSettings.FontMenuSymbol.DrawText(
                            MenuManager.Instance.Sprite,
                            "›",
                            (int)
                            (position.X + this.Component.MenuWidth - MenuSettings.ContainerTextMarkWidth
                             - MenuSettings.ContainerTextMarkOffset) + 1,
                            symbolCenterY,
                            this.Component.Components.Count > 0
                                ? ColoredMenuSettings.TextCaptionColor
                                : MenuSettings.ContainerSeparatorColor);
                    });

            if (this.Component.Toggled)
            {
                Line.Width = this.Component.MenuWidth - 3;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(position.X + this.Component.MenuWidth / 2f - 1, position.Y + 1),
                            new Vector2(
                                position.X + this.Component.MenuWidth / 2f - 1,
                                position.Y + MenuSettings.ContainerHeight - 1)
                        },
                    MenuSettings.ContainerSelectedColor);
                Line.End();

                float height = MenuSettings.ContainerHeight * this.Component.Components.Count;
                var width = MenuSettings.ContainerWidth;
                if (this.Component.Components.Count > 0)
                {
                    width = this.Component.Components.First().Value.MenuWidth;
                }

                Utils.DrawBoxRounded(
                    position.X + this.Component.MenuWidth,
                    position.Y,
                    width,
                    height,
                    4,
                    true,
                    MenuSettings.RootContainerColor,
                    new ColorBGRA(55, 76, 95, 255));

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
            }

            if (this.hasDragged && !MenuCustomizer.Instance.LockPosition.Value)
            {
                var sprite = MenuManager.Instance.Sprite;
                var oldMatrix = sprite.Transform;
                var y =
                    (int)(MenuSettings.Position.Y + (MenuManager.Instance.Menus.Count * MenuSettings.ContainerHeight));
                var dragTexture = ColoredTextures.Instance[ColoredTexture.Dragging];
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
                (ColoredUtilities.MeasureString(MultiLanguage.Translation(this.Component.DisplayName) + " »").Width
                 + (MenuSettings.ContainerTextOffset * 2) + MenuSettings.ContainerTextMarkWidth);
        }

        #endregion
    }
}