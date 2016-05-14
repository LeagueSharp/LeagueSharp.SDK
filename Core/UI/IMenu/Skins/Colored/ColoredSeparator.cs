// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredSeparator.cs" company="LeagueSharp">
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
//   Implements <see cref="ADrawable{MenuSeperator}" /> as a custom skin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDKEx.UI.Skins.Colored
{
    using System;

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class ColoredSeparator : ADrawable<MenuSeparator>
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        /// <summary>
        ///     Offset.
        /// </summary>
        private static readonly int Offset = 15;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColoredSeparator" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public ColoredSeparator(MenuSeparator component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        public override void Draw()
        {
            var centerY = ColoredUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    ColoredMenuSettings.FontCaption,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            ColoredMenuSettings.FontCaption.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)centerY.X,
                (int)centerY.Y,
                ColoredMenuSettings.TextCaptionColor);

            Line.Width = 2;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            this.Component.Position.X + Offset,
                            ColoredUtilities.GetContainerRectangle(this.Component).Center.Y),
                        new Vector2(
                            Math.Max(this.Component.Position.X + Offset, centerY.X - 5),
                            ColoredUtilities.GetContainerRectangle(this.Component).Center.Y)
                    },
                MenuSettings.ContainerSelectedColor);
            var newX =
                ColoredMenuSettings.FontCaption.MeasureText(
                    MenuManager.Instance.Sprite,
                    this.Component.DisplayName.ToUpper(),
                    0).Width;
            Line.Draw(
                new[]
                    {
                        new Vector2(centerY.X + newX + 5, ColoredUtilities.GetContainerRectangle(this.Component).Center.Y),
                        new Vector2(
                            Math.Max(
                                centerY.X + newX + 5,
                                ColoredUtilities.GetContainerRectangle(this.Component).Right - Offset),
                            ColoredUtilities.GetContainerRectangle(this.Component).Center.Y)
                    },
                MenuSettings.ContainerSelectedColor);
            Line.End();
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            // Do nothing.
        }

        /// <summary>
        ///     Calculates the Width of an AMenuComponent
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return ColoredUtilities.CalcWidthItem(this.Component);
        }

        #endregion
    }
}