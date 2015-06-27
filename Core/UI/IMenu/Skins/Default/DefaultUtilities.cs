using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;

    using SharpDX;

    /// <summary>
    /// Provides a set of functions used in the Default theme.
    /// </summary>
    public class DefaultUtilities
    {

        /// <summary>
        ///     Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="int" /></returns>
        public static int CalcWidthText(string text)
        {
            return MenuSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0).Width;
        }

        /// <summary>
        ///     Calculate the item's width.
        /// </summary>
        /// <param name="menuItem">The <see cref="MenuItem" /></param>
        /// <returns>The width</returns>
        public static int CalcWidthItem(MenuItem menuItem)
        {
            return (int)(MeasureString(menuItem.DisplayName).Width + (MenuSettings.ContainerTextOffset * 2));
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
    }
}
