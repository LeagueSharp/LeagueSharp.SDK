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
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="IDrawable{MenuButton}" />
    /// </summary>
    public class DefaultButton : DefaultComponent, IDrawable<MenuButton>
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
                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - buttonTextWidth - (2 * TextGap)), 
                    (int)component.Position.Y, 
                    (2 * TextGap) + buttonTextWidth, 
                    MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a <see cref="MenuButton" />
        /// </summary>
        /// <param name="component">The <see cref="MenuButton" /></param>
        public virtual void Draw(MenuButton component)
        {
            var rectangleName = GetContainerRectangle(component)
                .GetCenteredText(null, MenuSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(component.Position.X + MenuSettings.ContainerTextOffset), 
                (int)rectangleName.Y, 
                MenuSettings.TextColor);

            var buttonTextWidth =
                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;

            var line = new Line(Drawing.Direct3DDevice)
                           {
                              Antialias = false, GLLines = true, Width = MenuSettings.ContainerHeight 
                           };
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            component.Position.X + component.MenuWidth - buttonTextWidth
                            - (2 * TextGap), 
                            component.Position.Y + (MenuSettings.ContainerHeight / 2f)), 
                        new Vector2(
                            component.Position.X + component.MenuWidth, 
                            component.Position.Y + (MenuSettings.ContainerHeight / 2f)), 
                    }, 
                MenuSettings.HoverColor);
            line.End();
            line.Width = MenuSettings.ContainerHeight - 5;
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            component.Position.X + component.MenuWidth - buttonTextWidth
                            - (2 * TextGap) + 2, 
                            component.Position.Y + (MenuSettings.ContainerHeight / 2f)), 
                        new Vector2(
                            component.Position.X + component.MenuWidth - 2, 
                            component.Position.Y + (MenuSettings.ContainerHeight / 2f)), 
                    }, 
                component.Hovering ? this.buttonHoverColor : this.buttonColor);
            line.End();
            line.Dispose();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.ButtonText, 
                (int)(component.Position.X + component.MenuWidth - buttonTextWidth - TextGap), 
                (int)rectangleName.Y, 
                MenuSettings.TextColor);
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
        public virtual int Width(MenuButton menuButton)
        {
            return CalcWidthItem(menuButton) + (2 * TextGap)
                   + MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, menuButton.ButtonText, 0).Width;
        }

        #endregion

        /// <summary>
        /// Processes windows events
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">event</param>
        public virtual void OnWndProc(MenuButton component, WindowsKeys args)
        {
            if (!component.Visible)
            {
                return;
            }

            var rect = ButtonBoundaries(component);

            if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
            {
                component.Hovering = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN && component.Action != null)
                {
                    component.Action();
                }
            }
            else
            {
                component.Hovering = false;
            }
        }
    }
}