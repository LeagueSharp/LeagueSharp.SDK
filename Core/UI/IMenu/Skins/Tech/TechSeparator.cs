// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechSeparator.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Tech
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class TechSeparator : ADrawable<MenuSeparator>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TechSeparator" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public TechSeparator(MenuSeparator component)
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
            var centerY = TechUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    TechMenuSettings.FontCaption,
                    this.Component.DisplayName.ToUpper(),
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            TechMenuSettings.FontCaption.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.DisplayName.ToUpper(),
                (int)centerY.X,
                (int)centerY.Y,
                TechMenuSettings.TextCaptionColor);
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
            return TechUtilities.CalcWidthItem(this.Component);
        }

        #endregion
    }
}