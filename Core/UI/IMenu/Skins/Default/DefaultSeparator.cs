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
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    /// <summary>
    ///     Implements <see cref="IDrawableSeparator" /> as a default skin.
    /// </summary>
    public class DefaultSeparator : DefaultComponent, IDrawableSeparator
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        /// <param name="component">The <see cref="MenuSeparator" /></param>
        public void Draw(MenuSeparator component)
        {
            var centerY = GetContainerRectangle(component)
                .GetCenteredText(
                    null, MenuSettings.Font,
                    component.DisplayName, 
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)centerY.X, 
                (int)centerY.Y, 
                MenuSettings.TextColor);
        }

        #endregion
    }
}