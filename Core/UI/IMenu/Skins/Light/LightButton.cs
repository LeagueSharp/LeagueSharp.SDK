// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightButton.cs" company="LeagueSharp">
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
//   A custom implementation of <see cref="ADrawable{MenuButton}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDKEx.UI.Skins.Light
{
    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of <see cref="ADrawable{MenuButton}" />
    /// </summary>
    public class LightButton : ADrawable<MenuButton>
    {
        #region Constants

        /// <summary>
        ///     The text gap.
        /// </summary>
        private const int TextGap = 5;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Fields

        /// <summary>
        ///     The button color.
        /// </summary>
        private readonly ColorBGRA buttonColor = new ColorBGRA(151, 151, 151, 255);

        /// <summary>
        ///     The button hover color.
        /// </summary>
        private readonly ColorBGRA buttonHoverColor = new ColorBGRA(171, 171, 171, 200);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightButton" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public LightButton(MenuButton component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculate the Rectangle that defines the Button
        /// </summary>
        /// <param name="component">The <see cref="MenuButton" /></param>
        /// <returns>
        ///     The <see cref="Rectangle" />
        /// </returns>
        public Rectangle ButtonBoundaries(MenuButton component)
        {
            var buttonTextWidth =
                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - buttonTextWidth - (2 * TextGap)),
                (int)component.Position.Y,
                (2 * TextGap) + buttonTextWidth,
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
        ///     Draws a <see cref="MenuButton" />
        /// </summary>
        public override void Draw()
        {
            var rectangleName = LightUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    MenuSettings.Font,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(this.Component.Position.X + MenuSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                MenuSettings.TextColor);

            var buttonTextWidth =
                MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, this.Component.ButtonText, 0).Width;

            Line.Width = MenuSettings.ContainerHeight - 5;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - buttonTextWidth - (2 * TextGap) + 2,
                            this.Component.Position.Y + (MenuSettings.ContainerHeight / 2f)),
                        new Vector2(
                            this.Component.Position.X + this.Component.MenuWidth - 2,
                            this.Component.Position.Y + (MenuSettings.ContainerHeight / 2f)),
                    },
                this.Component.Hovering ? this.buttonHoverColor : this.buttonColor);
            Line.End();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.ButtonText),
                (int)(this.Component.Position.X + this.Component.MenuWidth - buttonTextWidth - TextGap),
                (int)rectangleName.Y,
                new ColorBGRA(221, 233, 255, 255));
        }

        /// <summary>
        ///     Processes windows events
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            var rect = this.ButtonBoundaries(this.Component);

            if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
            {
                this.Component.Hovering = true;
                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Component.Action?.Invoke();
                }
            }
            else
            {
                this.Component.Hovering = false;
            }
        }

        /// <summary>
        ///     Gets the width of the <see cref="MenuButton" />
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public override int Width()
        {
            return LightUtilities.CalcWidthItem(this.Component) + (2 * TextGap)
                   + MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, this.Component.ButtonText, 0).Width;
        }

        #endregion
    }
}