namespace LeagueSharp.SDK.Core.UI.Skins
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a generic list of items
    /// </summary>
    public interface IDrawableList
    {
        /// <summary>
        ///     Draw a MenuList
        /// </summary>
        /// <param name="component">MenuList</param>
        void Draw(MenuList component);

        /// <summary>
        ///     Gets the dropdown boundaries (preview)
        /// </summary>
        /// <param name="component">MenuList</param>
        /// <returns>Rectangle</returns>
        Rectangle DropDownBoundaries(MenuList component);

        /// <summary>
        ///     Gets the list of dropdown item boundaries.
        /// </summary>
        /// <param name="component">MenuList</param>
        /// <returns>List of Rectangles</returns>
        List<Rectangle> DropDownListBoundaries(MenuList component);

        /// <summary>
        ///     Gets the complete dropdown boundaries
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        Rectangle DropDownExpandedBoundaries(MenuList component);

        /// <summary>
        ///     Gets the width of the MenuList
        /// </summary>
        /// <param name="menuList">MenuList</param>
        /// <returns>int</returns>
        int Width(MenuList menuList);
    }
}