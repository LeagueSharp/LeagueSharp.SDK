// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSeparator.cs" company="LeagueSharp">
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
//   Implements <see cref="IDrawableSeparator" /> as a default skin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class DefaultSeparator : ADrawable<MenuSeparator>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Creates a new handler responsible for the given <see cref="AMenuComponent"/>.
        /// </summary>
        /// <param name="component">The menu component</param>
        public DefaultSeparator(MenuSeparator component)
            : base(component) {}

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        public override void Draw()
        {
            var centerY = DefaultUtilities.GetContainerRectangle(Component)
                .GetCenteredText(
                    null, MenuSettings.Font,
                    Component.DisplayName, 
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.DisplayName, 
                (int)centerY.X, 
                (int)centerY.Y, 
                MenuSettings.TextColor);
        }

        #endregion

        /// <summary>
        /// Calculates the Width of an AMenuComponent
        /// </summary>
        /// <returns>width</returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(Component);
        }

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            //nothing
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