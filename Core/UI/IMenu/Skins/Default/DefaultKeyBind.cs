// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultKeyBind.cs" company="LeagueSharp">
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
//   A default implementation of <see cref="IDrawableKeyBind" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="ADrawable{MenuKeyBind}" />
    /// </summary>
    public class DefaultKeyBind : ADrawable<MenuKeyBind>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a new handler responsible for the given <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="component">The menu component</param>
        public DefaultKeyBind(MenuKeyBind component)
            : base(component) {}

        /// <summary>
        ///     Gets the On/Off boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuKeyBind" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuKeyBind component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight), 
                    (int)component.Position.Y, 
                    MenuSettings.ContainerHeight, 
                    MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a MenuKeyBind
        /// </summary>
        public override void Draw()
        {
            var centerY =
                (int)
                DefaultUtilities.GetContainerRectangle(Component)
                    .GetCenteredText(null, MenuSettings.Font, Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.Interacting ? "Press a key" : Component.DisplayName,
                (int)(Component.Position.X + MenuSettings.ContainerTextOffset), 
                centerY, 
                MenuSettings.TextColor);

            if (!Component.Interacting)
            {
                var keyString = "[" + Component.Key + "]";
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite, 
                    keyString, 
                    (int)
                    (Component.Position.X + Component.MenuWidth - MenuSettings.ContainerHeight
                     - DefaultUtilities.CalcWidthText(keyString) - MenuSettings.ContainerTextOffset), 
                    centerY, 
                    MenuSettings.TextColor);
            }

            var line = new Line(Drawing.Direct3DDevice)
                           {
                              Antialias = false, GLLines = true, Width = MenuSettings.ContainerHeight 
                           };
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            (Component.Position.X + Component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            Component.Position.Y + 1), 
                        new Vector2(
                            (Component.Position.X + Component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            Component.Position.Y + MenuSettings.ContainerHeight)
                    },
                Component.Active ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
            line.End();
            line.Dispose();

            var centerX =
                (int)
                new Rectangle(
                    (int)
                    (Component.Position.X + Component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)Component.Position.Y, 
                    MenuSettings.ContainerHeight, 
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null, MenuSettings.Font,
                        Component.Active ? "ON" : "OFF", 
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.Active ? "ON" : "OFF", 
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
        ///     Gets the width of the MenuKeyBind
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(Component) + 
                (int)
                (MenuSettings.ContainerHeight + DefaultUtilities.CalcWidthText("[" + Component.Key + "]")
                 + MenuSettings.ContainerTextOffset);
        }

        #endregion

        /// <summary>
        ///     ChangeKey method.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="newKey">
        ///     The new key
        /// </param>
        private void ChangeKey(MenuKeyBind component, Keys newKey)
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
        private void HandleDown(MenuKeyBind component, Keys expectedKey)
        {
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
        private void HandleUp(MenuKeyBind component, Keys expectedKey)
        {
            if (expectedKey == component.Key)
            {
                switch (component.Type)
                {
                    case KeyBindType.Press:
                        component.Active = false;
                        break;
                    case KeyBindType.Toggle:
                        component.Active = !component.Active;
                        break;
                }
            }
        }

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!MenuGUI.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case WindowsMessages.KEYDOWN:
                        this.HandleDown(Component, args.Key);
                        break;
                    case WindowsMessages.KEYUP:
                        if (Component.Interacting && args.SingleKey != Keys.ShiftKey)
                        {
                            this.ChangeKey(Component, args.SingleKey == Keys.Escape ? Keys.None : args.Key);
                        }
                        else
                        {
                            this.HandleUp(Component, args.Key);
                        }

                        break;
                    case WindowsMessages.XBUTTONDOWN:
                        this.HandleDown(Component, args.SideButton);
                        break;
                    case WindowsMessages.XBUTTONUP:
                        if (Component.Interacting)
                        {
                            this.ChangeKey(Component, args.SideButton);
                        }
                        else
                        {
                            this.HandleUp(Component, args.SideButton);
                        }

                        break;
                    case WindowsMessages.MBUTTONDOWN:
                        this.HandleDown(Component, Keys.MButton);
                        break;
                    case WindowsMessages.MBUTTONUP:
                        if (Component.Interacting)
                        {
                            this.ChangeKey(Component, Keys.MButton);
                        }
                        else
                        {
                            this.HandleUp(Component, Keys.MButton);
                        }

                        break;
                    case WindowsMessages.RBUTTONDOWN:
                        this.HandleDown(Component, Keys.RButton);
                        break;
                    case WindowsMessages.RBUTTONUP:
                        if (Component.Interacting)
                        {
                            this.ChangeKey(Component, Keys.RButton);
                        }
                        else
                        {
                            this.HandleUp(Component, Keys.RButton);
                        }

                        break;
                    case WindowsMessages.LBUTTONDOWN:
                        if (Component.Interacting)
                        {
                            this.ChangeKey(Component, Keys.LButton);
                        }
                        else if (Component.Visible)
                        {
                            var container = ButtonBoundaries(Component);
                            var content = KeyBindBoundaries(Component);

                            if (args.Cursor.IsUnderRectangle(
                                container.X,
                                container.Y,
                                container.Width,
                                container.Height))
                            {
                                Component.Active = !Component.Active;
                            }
                            else if (args.Cursor.IsUnderRectangle(content.X, content.Y, content.Width, content.Height))
                            {
                                Component.Interacting = !Component.Interacting;
                            }
                            else
                            {
                                this.HandleDown(Component, Keys.LButton);
                            }
                        }

                        break;
                    case WindowsMessages.LBUTTONUP:
                        this.HandleUp(Component, Keys.LButton);
                        break;
                }
            }
        }

        /// <summary>
        /// Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            //do nothing
        }
    }
}