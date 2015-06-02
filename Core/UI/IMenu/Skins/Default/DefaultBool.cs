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
                    (component.Container.Position.X + component.Container.MenuWidth - DefaultSettings.ContainerHeight), 
                    (int)component.Container.Position.Y, 
                    DefaultSettings.ContainerHeight, 
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a <see cref="MenuBool" />
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        public void Draw(MenuBool component)
        {
            var centerY =
                (int)
                GetContainerRectangle(component.Container)
                    .GetCenteredText(null, component.Container.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Container.DisplayName, 
                (int)(component.Container.Position.X + DefaultSettings.ContainerTextOffset), 
                centerY, 
                DefaultSettings.TextColor);

            var line = new Line(Drawing.Direct3DDevice)
                           {
                              Antialias = false, GLLines = true, Width = DefaultSettings.ContainerHeight 
                           };
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            (component.Container.Position.X + component.Container.MenuWidth
                             - DefaultSettings.ContainerHeight) + DefaultSettings.ContainerHeight / 2f, 
                            component.Container.Position.Y + 1), 
                        new Vector2(
                            (component.Container.Position.X + component.Container.MenuWidth
                             - DefaultSettings.ContainerHeight) + DefaultSettings.ContainerHeight / 2f, 
                            component.Container.Position.Y + DefaultSettings.ContainerHeight)
                    }, 
                component.Value ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
            line.End();
            line.Dispose();

            var centerX =
                (int)
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - DefaultSettings.ContainerHeight), 
                    (int)component.Container.Position.Y, 
                    DefaultSettings.ContainerHeight, 
                    DefaultSettings.ContainerHeight).GetCenteredText(
                        null, 
                        component.Value ? "ON" : "OFF", 
                        CenteredFlags.HorizontalCenter).X;
            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Value ? "ON" : "OFF", 
                centerX, 
                centerY, 
                DefaultSettings.TextColor);
        }

        #endregion
    }
}