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
    using System;
    using System.Globalization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="IDrawable{MenuSlider}" />
    /// </summary>
    public class DefaultSlider : DefaultComponent, IDrawable<MenuSlider>
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
        public virtual void Draw(MenuSlider component)
        {
            var position = component.Position;
            var centeredY =
                (int)
                GetContainerRectangle(component)
                    .GetCenteredText(null, MenuSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (component.Value - component.MinValue) / (float)(component.MaxValue - component.MinValue);
            var x = position.X + (percent * component.MenuWidth);

            var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
            line.Begin();
            line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) }, 
                component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
            line.End();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                centeredY, 
                MenuSettings.TextColor);

            var measureText = MenuSettings.Font.MeasureText(
                null, 
                component.Value.ToString(CultureInfo.InvariantCulture), 
                0);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Value.ToString(CultureInfo.InvariantCulture), 
                (int)(position.X + component.MenuWidth - 5 - measureText.Width), 
                centeredY, 
                MenuSettings.TextColor);

            line.Width = MenuSettings.ContainerHeight;
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y + MenuSettings.ContainerHeight / 2f), 
                        new Vector2(x, position.Y + MenuSettings.ContainerHeight / 2f)
                    }, 
                MenuSettings.HoverColor);
            line.End();
            line.Dispose();
        }

        #endregion

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event data</param>
        public virtual void OnWndProc(MenuSlider component, WindowsKeys args)
        {
            if (!component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.MOUSEMOVE && component.Interacting)
            {
                this.CalculateNewValue(component, args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN && !component.Interacting)
            {
                var container = Bounding(component);

                if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                {
                    component.Interacting = true;
                    CalculateNewValue(component, args);
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                component.Interacting = false;
            }
        }

        /// <summary>
        ///     Calculate the new value based onto the cursor position.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        private void CalculateNewValue(MenuSlider component, WindowsKeys args)
        {
            var newValue =
                (int)
                Math.Round(
                    component.MinValue
                    + ((args.Cursor.X - component.Position.X) * (component.MaxValue - component.MinValue))
                    / component.MenuWidth);
            if (newValue < component.MinValue)
            {
                newValue = component.MinValue;
            }
            else if (newValue > component.MaxValue)
            {
                newValue = component.MaxValue;
            }

            if (newValue != component.Value)
            {
                component.Value = newValue;
                component.FireEvent();
            }
        }

        /// <summary>
        /// Calculates the width of this component
        /// </summary>
        /// <param name="component">menu component</param>
        /// <returns>width</returns>
        public virtual int Width(MenuSlider component)
        {
            return CalcWidthItem(component) + 100;
        }
    }
}