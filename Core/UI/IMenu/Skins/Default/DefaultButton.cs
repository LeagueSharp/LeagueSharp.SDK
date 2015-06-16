// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultButton.cs" company="LeagueSharp">
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
//   A default implementation of <see cref="IDrawableButton" />
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
    ///     A default implementation of <see cref="IDrawableButton" />
    /// </summary>
    public class DefaultButton : DefaultComponent, IDrawableButton
    {
        #region Constants

        /// <summary>
        ///     The text gap.
        /// </summary>
        private const int TextGap = 5;

        #endregion

        #region Fields

        /// <summary>
        ///     The button color.
        /// </summary>
        private readonly ColorBGRA buttonColor = new ColorBGRA(100, 100, 100, 255);

        /// <summary>
        ///     The button hover color.
        /// </summary>
        private readonly ColorBGRA buttonHoverColor = new ColorBGRA(170, 170, 170, 200);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculate the Rectangle that defines the Button
        /// </summary>
        /// <param name="component">The <see cref="MenuButton" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuButton component)
        {
            var buttonTextWidth =
                DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;
            return
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth - (2 * TextGap)), 
                    (int)component.Container.Position.Y, 
                    (2 * TextGap) + buttonTextWidth, 
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a <see cref="MenuButton" />
        /// </summary>
        /// <param name="component">The <see cref="MenuButton" /></param>
        public void Draw(MenuButton component)
        {
            var rectangleName = GetContainerRectangle(component.Container)
                .GetCenteredText(null, DefaultSettings.Font, component.Container.DisplayName, CenteredFlags.VerticalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.Container.DisplayName, 
                (int)(component.Container.Position.X + DefaultSettings.ContainerTextOffset), 
                (int)rectangleName.Y, 
                DefaultSettings.TextColor);

            var buttonTextWidth =
                DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;

            var line = new Line(Drawing.Direct3DDevice)
                           {
                              Antialias = false, GLLines = true, Width = DefaultSettings.ContainerHeight 
                           };
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth
                            - (2 * TextGap), 
                            component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth, 
                            component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                    }, 
                DefaultSettings.HoverColor);
            line.End();
            line.Width = DefaultSettings.ContainerHeight - 5;
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth
                            - (2 * TextGap) + 2, 
                            component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                        new Vector2(
                            component.Container.Position.X + component.Container.MenuWidth - 2, 
                            component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)), 
                    }, 
                component.Hovering ? this.buttonHoverColor : this.buttonColor);
            line.End();
            line.Dispose();

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.ButtonText, 
                (int)(component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth - TextGap), 
                (int)rectangleName.Y, 
                DefaultSettings.TextColor);
        }

        /// <summary>
        ///     Gets the width of the <see cref="MenuButton" />
        /// </summary>
        /// <param name="menuButton">
        ///     The <see cref="MenuButton" />
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int Width(MenuButton menuButton)
        {
            return (2 * TextGap)
                   + DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, menuButton.ButtonText, 0).Width;
        }

        #endregion
    }
}