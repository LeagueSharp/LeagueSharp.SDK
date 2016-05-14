// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechBool.cs" company="LeagueSharp">
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
//   A custom implementation of a <see cref="ADrawable{MenuBool}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Tech
{
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A blue implementation of a <see cref="ADrawable{MenuBool}" />
    /// </summary>
    public class TechBool : ADrawable<MenuBool>
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TechBool" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        public TechBool(MenuBool component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the Rectangle that defines the on/off button
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuBool component)
        {
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
                (int)component.Position.Y,
                MenuSettings.ContainerHeight,
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
        ///     Draws a <see cref="MenuBool" />
        /// </summary>
        public override void Draw()
        {
            var centerY =
                (int)
                TechUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(
                        null,
                        MenuSettings.Font,
                        MultiLanguage.Translation(this.Component.DisplayName),
                        CenteredFlags.VerticalCenter)
                    .Y;

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(this.Component.Position.X + MenuSettings.ContainerTextOffset),
                centerY,
                MenuSettings.TextColor);

            var centerX =
                (int)
                new Rectangle(
                    (int)(this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)this.Component.Position.Y,
                    MenuSettings.ContainerHeight,
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null,
                        MenuSettings.Font,
                        MultiLanguage.Translation(this.Component.Value ? "On" : "Off"),
                        CenteredFlags.HorizontalCenter).X - 5;

            //Left
            Utils.DrawCircle(
                centerX,
                this.Component.Position.Y + MenuSettings.ContainerHeight / 2f,
                7,
                270,
                Utils.CircleType.Half,
                true,
                32,
                this.Component.Value ? new Color(75, 215, 128, 255) : new Color(36, 204, 205, 255));

            //Right
            Utils.DrawCircle(
                centerX + 15,
                this.Component.Position.Y + MenuSettings.ContainerHeight / 2f,
                7,
                90,
                Utils.CircleType.Half,
                true,
                32,
                this.Component.Value ? new Color(75, 215, 128, 255) : new Color(36, 204, 205, 255));

            //Top
            Line.Width = 1;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(centerX, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f - 8),
                        new Vector2(centerX + 15, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f - 8)
                    },
                this.Component.Value ? new Color(75, 215, 128, 255) : new Color(36, 204, 205, 255));
            Line.End();

            //Bot
            Line.Width = 1;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(centerX, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f + 7),
                        new Vector2(centerX + 15, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f + 7)
                    },
                this.Component.Value ? new Color(75, 215, 128, 255) : new Color(36, 204, 205, 255));
            Line.End();

            //FullCircle
            Utils.DrawCircleFilled(
                this.Component.Value ? centerX + 14 : centerX + 1,
                this.Component.Position.Y + MenuSettings.ContainerHeight / 2f,
                6,
                0,
                Utils.CircleType.Full,
                true,
                32,
                this.Component.Value ? new Color(73, 227, 139, 255) : new Color(17, 65, 65, 255));
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                var rect = this.ButtonBoundaries(this.Component);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    this.Component.Value = !this.Component.Value;
                    this.Component.FireEvent();
                }
            }
        }

        /// <summary>
        ///     Calculates the Width of a <see cref="MenuBool" />
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return TechUtilities.CalcWidthItem(this.Component) + MenuSettings.ContainerHeight;
        }

        #endregion
    }
}