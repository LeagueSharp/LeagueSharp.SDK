// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueSeparator2.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI.Skins.Blue2
{
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI.Skins.Blue;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class BlueSeparator2 : BlueSeparator
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlueSeparator" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public BlueSeparator2(MenuSeparator component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        public override void Draw()
        {
            var centerY = BlueUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    BlueMenuSettings.FontCaption,
                    this.Component.DisplayName.ToUpper(),
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            BlueMenuSettings.FontCaption.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.DisplayName.ToUpper(),
                (int)centerY.X,
                (int)centerY.Y,
                BlueMenuSettings.TextCaptionColor);
        }

        #endregion
    }
}