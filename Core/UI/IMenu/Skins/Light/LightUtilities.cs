// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightUtilities.cs" company="LeagueSharp">
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
//   Provides a set of functions used in the Custom theme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Light
{
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     Provides a set of functions used in the Default theme.
    /// </summary>
    public class LightUtilities
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculate the item's width.
        /// </summary>
        /// <param name="menuItem">The <see cref="MenuItem" /></param>
        /// <returns>The width</returns>
        public static int CalcWidthItem(MenuItem menuItem)
        {
            return
                (int)
                (MeasureString(MultiLanguage.Translation(menuItem.DisplayName)).Width
                 + (MenuSettings.ContainerTextOffset * 2));
        }

        /// <summary>
        ///     Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="int" /></returns>
        public static int CalcWidthText(string text)
        {
            return MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, MultiLanguage.Translation(text), 0).Width;
        }

        /// <summary>
        ///     Gets the container rectangle.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        /// <returns>
        ///     <see cref="Rectangle" /> with information.
        /// </returns>
        public static Rectangle GetContainerRectangle(AMenuComponent component)
        {
            return new Rectangle(
                (int)component.Position.X,
                (int)component.Position.Y,
                component.MenuWidth,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Calculates the string measurements.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <returns>
        ///     The measured rectangle.
        /// </returns>
        public static Rectangle MeasureString(string text)
        {
            return MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0);
        }

        #endregion
    }
}