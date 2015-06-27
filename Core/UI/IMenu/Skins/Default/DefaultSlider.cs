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
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSlider}" />
    /// </summary>
    public class DefaultSlider : ADrawable<MenuSlider>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a new handler responsible for the given <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="component">The menu component</param>
        public DefaultSlider(MenuSlider component)
            : base(component) {}

        /// <summary>
        ///     Gets the additional boundaries.
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle AdditionalBoundries(MenuSlider component)
        {
            return DefaultUtilities.GetContainerRectangle(component);
        }

        /// <summary>
        ///     Gets the boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle Bounding(MenuSlider component)
        {
            return DefaultUtilities.GetContainerRectangle(component);
        }

        /// <summary>
        ///     Draws a <see cref="MenuSlider" />
        /// </summary>
        public override void Draw()
        {
            var position = Component.Position;
            var centeredY =
                (int)
                DefaultUtilities.GetContainerRectangle(Component)
                    .GetCenteredText(null, MenuSettings.Font, Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (Component.Value - Component.MinValue) / (float)(Component.MaxValue - Component.MinValue);
            var x = position.X + (percent * Component.MenuWidth);

            var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
            line.Begin();
            line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) },
                Component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
            line.End();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                centeredY, 
                MenuSettings.TextColor);

            var measureText = MenuSettings.Font.MeasureText(
                null,
                Component.Value.ToString(CultureInfo.InvariantCulture), 
                0);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.Value.ToString(CultureInfo.InvariantCulture),
                (int)(position.X + Component.MenuWidth - 5 - measureText.Width), 
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
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!Component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.MOUSEMOVE && Component.Interacting)
            {
                this.CalculateNewValue(Component, args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN && !Component.Interacting)
            {
                var container = Bounding(Component);

                if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                {
                    Component.Interacting = true;
                    CalculateNewValue(Component, args);
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                Component.Interacting = false;
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
        /// <returns>width</returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(Component) + 100;
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