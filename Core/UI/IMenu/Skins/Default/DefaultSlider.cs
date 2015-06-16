// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSlider.cs" company="LeagueSharp">
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
//   A default implementation of an <see cref="IDrawableSlider" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Globalization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="IDrawableSlider" />
    /// </summary>
    public class DefaultSlider : DefaultComponent, IDrawableSlider
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the additional boundaries.
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle AdditionalBoundries(MenuSlider component)
        {
            return GetContainerRectangle(component);
        }

        /// <summary>
        ///     Gets the boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle Bounding(MenuSlider component)
        {
            return GetContainerRectangle(component);
        }

        /// <summary>
        ///     Draws a <see cref="MenuSlider" />
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        public void Draw(MenuSlider component)
        {
            var position = component.Position;
            var centeredY =
                (int)
                GetContainerRectangle(component)
                    .GetCenteredText(null, DefaultSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (component.Value - component.MinValue) / (float)(component.MaxValue - component.MinValue);
            var x = position.X + (percent * component.MenuWidth);

            var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
            line.Begin();
            line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + DefaultSettings.ContainerHeight) }, 
                component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
            line.End();

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(position.X + DefaultSettings.ContainerTextOffset), 
                centeredY, 
                DefaultSettings.TextColor);

            var measureText = DefaultSettings.Font.MeasureText(
                null, 
                component.Value.ToString(CultureInfo.InvariantCulture), 
                0);
            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Value.ToString(CultureInfo.InvariantCulture), 
                (int)(position.X + component.MenuWidth - 5 - measureText.Width), 
                centeredY, 
                DefaultSettings.TextColor);

            line.Width = DefaultSettings.ContainerHeight;
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f), 
                        new Vector2(x, position.Y + DefaultSettings.ContainerHeight / 2f)
                    }, 
                DefaultSettings.HoverColor);
            line.End();
            line.Dispose();
        }

        #endregion
    }
}