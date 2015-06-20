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
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="IDrawableKeyBind" />
    /// </summary>
    public class DefaultKeyBind : DefaultComponent, IDrawableKeyBind
    {
        #region Public Methods and Operators

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
        /// <param name="component">The <see cref="MenuKeyBind" /></param>
        public void Draw(MenuKeyBind component)
        {
            var centerY =
                (int)
                GetContainerRectangle(component)
                    .GetCenteredText(null, MenuSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Interacting ? "Press a key" : component.DisplayName, 
                (int)(component.Position.X + MenuSettings.ContainerTextOffset), 
                centerY, 
                MenuSettings.TextColor);

            if (!component.Interacting)
            {
                var keyString = "[" + component.Key + "]";
                MenuSettings.Font.DrawText(
                    MenuManager.Instance.Sprite, 
                    keyString, 
                    (int)
                    (component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight
                     - this.CalcWidthText(keyString) - MenuSettings.ContainerTextOffset), 
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
                            (component.Position.X + component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            component.Position.Y + 1), 
                        new Vector2(
                            (component.Position.X + component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            component.Position.Y + MenuSettings.ContainerHeight)
                    }, 
                component.Active ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
            line.End();
            line.Dispose();

            var centerX =
                (int)
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight), 
                    (int)component.Position.Y, 
                    MenuSettings.ContainerHeight, 
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null, MenuSettings.Font,
                        component.Active ? "ON" : "OFF", 
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Active ? "ON" : "OFF", 
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
            return GetContainerRectangle(component);
        }

        /// <summary>
        ///     Gets the width of the MenuKeyBind
        /// </summary>
        /// <param name="keyBind">The <see cref="MenuKeyBind" /></param>
        /// <returns>The <see cref="int" /></returns>
        public int Width(MenuKeyBind keyBind)
        {
            return
                (int)
                (MenuSettings.ContainerHeight + this.CalcWidthText("[" + keyBind.Key + "]")
                 + MenuSettings.ContainerTextOffset);
        }

        #endregion
    }
}