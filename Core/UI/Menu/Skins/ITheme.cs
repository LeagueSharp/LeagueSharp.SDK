using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Abstracts;

    using SharpDX;

    /// <summary>
    /// Defines a Theme used to draw components of the menu.
    /// </summary>
    public interface ITheme
    {
        /// <summary>
        /// Gets the IDrawableBool
        /// </summary>
        IDrawableBool Bool { get; }
        /// <summary>
        /// Gets the IDrawableColorPicker
        /// </summary>
        IDrawableColorPicker ColorPicker { get; }
        /// <summary>
        /// Gets the IDrawableButton
        /// </summary>
        IDrawableButton Button { get; }
        /// <summary>
        /// Gets the IDrawableKeyBind
        /// </summary>
        IDrawableKeyBind KeyBind { get; }
        /// <summary>
        /// Gets the IDrawableList
        /// </summary>
        IDrawableList List { get; }
        /// <summary>
        /// Gets the IDrawableSeparator
        /// </summary>
        IDrawableSeparator Separator { get; }
        /// <summary>
        /// Gets the IDrawableSlider
        /// </summary>
        IDrawableSlider Slider { get; }

        /// <summary>
        /// Calculates the width of a menu or submenu
        /// </summary>
        /// <param name="menu">Menu</param>
        /// <returns>Width</returns>
        int CalcWidthMenu(Menu menu);

        /// <summary>
        /// Calculates the width of a MenuItem without his value
        /// </summary>
        /// <param name="menuItem">MenuItem </param>
        /// <returns>Width</returns>
        int CalcWidthItem(MenuItem menuItem);

        /// <summary>
        /// Draws the list of root menus on the given position.
        /// </summary>
        /// <param name="position">Upperlef position of the menu</param>
        void OnDraw(Vector2 position);

        /// <summary>
        /// Draws a menu
        /// </summary>
        /// <param name="menuComponent">Menu</param>
        void DrawMenu(Menu menuComponent);

    }
}
