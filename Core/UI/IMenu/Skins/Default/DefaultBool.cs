// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBool.cs" company="LeagueSharp">
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
//   A default implementation of <see cref="IDrawableBool" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="IDrawableBool" />
    /// </summary>
    public class DefaultBool : DefaultComponent, IDrawableBool
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the Rectangle that defines the on/off button
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuBool component)
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
        ///     Draws a <see cref="MenuBool" />
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        public virtual void Draw(MenuBool component)
        {
            var centerY =
                (int)
                GetContainerRectangle(component)
                    .GetCenteredText(null, MenuSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(component.Position.X + MenuSettings.ContainerTextOffset), 
                centerY, 
                MenuSettings.TextColor);

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
                component.Value ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
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
                        component.Value ? "ON" : "OFF", 
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Value ? "ON" : "OFF", 
                centerX, 
                centerY, 
                MenuSettings.TextColor);
        }

        #endregion

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event data</param>
        public virtual void OnWndProc(MenuBool component, Utils.WindowsKeys args)
        {
            if (!component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                var rect = ButtonBoundaries(component);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    component.Value = !component.Value;
                    component.FireEvent();
                }
            }
        }

        /// <summary>
        /// Calculates the Width of a MenuBool
        /// </summary>
        /// <param name="component">menu component</param>
        /// <returns>width</returns>
        public virtual int Width(MenuBool component)
        {
            return MenuSettings.ContainerHeight;
        }
    }
}