// <copyright file="DefaultKeyBind.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.UI.Skins.Default
{
    using System.Windows.Forms;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="ADrawable{MenuKeyBind}" />
    /// </summary>
    public class DefaultKeyBind : ADrawable<MenuKeyBind>
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultKeyBind" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public DefaultKeyBind(MenuKeyBind component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the On/Off boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuKeyBind" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuKeyBind component)
        {
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
                (int)component.Position.Y,
                MenuSettings.ContainerHeight,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draws a MenuKeyBind
        /// </summary>
        public override void Draw()
        {
            var centerY =
                (int)
                DefaultUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.Interacting ? "Press a key" : this.Component.DisplayName),
                (int)(this.Component.Position.X + MenuSettings.ContainerTextOffset),
                centerY,
                MenuSettings.TextColor);

            if (!this.Component.Interacting)
            {
                var keyString = "[" + this.Component.Key + "]";
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite,
                    keyString,
                    (int)
                    (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight
                     - DefaultUtilities.CalcWidthText(keyString) - MenuSettings.ContainerTextOffset),
                    centerY,
                    MenuSettings.TextColor);
            }

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight)
                            + (MenuSettings.ContainerHeight / 2f),
                            this.Component.Position.Y + 1),
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight)
                            + (MenuSettings.ContainerHeight / 2f),
                            this.Component.Position.Y + MenuSettings.ContainerHeight)
                    },
                this.Component.Active ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
            Line.End();

            var centerX =
                (int)
                new Rectangle(
                    (int)(this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)this.Component.Position.Y,
                    MenuSettings.ContainerHeight,
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null,
                        MenuSettings.Font,
                        this.Component.Active ? "ON" : "OFF",
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.Active ? "ON" : "OFF"),
                centerX,
                centerY,
                MenuSettings.TextColor);
        }

        /// <summary>
        ///     Gets the <c>keybind</c> boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuKeyBind" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle KeyBindBoundaries(MenuKeyBind component)
        {
            return DefaultUtilities.GetContainerRectangle(component);
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!MenuGUI.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case WindowsMessages.KEYDOWN:
                        HandleDown(this.Component, args.Key);
                        break;
                    case WindowsMessages.KEYUP:
                        if (this.Component.Interacting && args.SingleKey != Keys.ShiftKey)
                        {
                            ChangeKey(this.Component, args.SingleKey == Keys.Escape ? Keys.None : args.Key);
                        }
                        else
                        {
                            HandleUp(this.Component, args.Key);
                        }

                        break;
                    case WindowsMessages.XBUTTONDOWN:
                        HandleDown(this.Component, args.SideButton);
                        break;
                    case WindowsMessages.XBUTTONUP:
                        if (this.Component.Interacting)
                        {
                            ChangeKey(this.Component, args.SideButton);
                        }
                        else
                        {
                            HandleUp(this.Component, args.SideButton);
                        }

                        break;
                    case WindowsMessages.MBUTTONDOWN:
                        HandleDown(this.Component, Keys.MButton);
                        break;
                    case WindowsMessages.MBUTTONUP:
                        if (this.Component.Interacting)
                        {
                            ChangeKey(this.Component, Keys.MButton);
                        }
                        else
                        {
                            HandleUp(this.Component, Keys.MButton);
                        }

                        break;
                    case WindowsMessages.RBUTTONDOWN:
                        HandleDown(this.Component, Keys.RButton);
                        break;
                    case WindowsMessages.RBUTTONUP:
                        if (this.Component.Interacting)
                        {
                            ChangeKey(this.Component, Keys.RButton);
                        }
                        else
                        {
                            HandleUp(this.Component, Keys.RButton);
                        }

                        break;
                    case WindowsMessages.LBUTTONDOWN:
                        if (this.Component.Interacting)
                        {
                            ChangeKey(this.Component, Keys.LButton);
                        }
                        else if (this.Component.Visible)
                        {
                            var container = this.ButtonBoundaries(this.Component);
                            var content = this.KeyBindBoundaries(this.Component);

                            if (args.Cursor.IsUnderRectangle(
                                container.X,
                                container.Y,
                                container.Width,
                                container.Height))
                            {
                                this.Component.Active = !this.Component.Active;
                            }
                            else if (args.Cursor.IsUnderRectangle(content.X, content.Y, content.Width, content.Height))
                            {
                                this.Component.Interacting = !this.Component.Interacting;
                            }
                            else
                            {
                                HandleDown(this.Component, Keys.LButton);
                            }
                        }

                        break;
                    case WindowsMessages.LBUTTONUP:
                        HandleUp(this.Component, Keys.LButton);
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets the width of the MenuKeyBind
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(this.Component)
                   + (int)
                     (MenuSettings.ContainerHeight + DefaultUtilities.CalcWidthText("[" + this.Component.Key + "]")
                      + MenuSettings.ContainerTextOffset);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     ChangeKey method.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="newKey">
        ///     The new key
        /// </param>
        private static void ChangeKey(MenuKeyBind component, Keys newKey)
        {
            component.Key = newKey;
            component.Interacting = false;
            MenuManager.Instance.ResetWidth();
        }

        /// <summary>
        ///     HandleDown method.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="expectedKey">
        ///     The expected key
        /// </param>
        private static void HandleDown(MenuKeyBind component, Keys expectedKey)
        {
            if (component.Key == Keys.ControlKey && expectedKey == (Keys.Control | Keys.ControlKey))
            {
                expectedKey = Keys.ControlKey;
            }

            if (!component.Interacting && expectedKey == component.Key && component.Type == KeyBindType.Press)
            {
                component.Active = true;
            }
        }

        /// <summary>
        ///     HandleUp method.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="expectedKey">
        ///     The expected key
        /// </param>
        private static void HandleUp(MenuKeyBind component, Keys expectedKey)
        {
            switch (component.Type)
            {
                case KeyBindType.Press:
                    if ((expectedKey.HasFlag(Keys.Shift) && expectedKey.HasFlag(component.Key))
                        || (expectedKey == component.Key))
                    {
                        component.Active = false;
                    }

                    break;
                case KeyBindType.Toggle:
                    if (expectedKey == component.Key)
                    {
                        component.Active = !component.Active;
                    }

                    break;
            }
        }

        #endregion
    }
}