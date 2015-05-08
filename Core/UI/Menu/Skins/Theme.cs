using System;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Skins
{
    /// <summary>
    ///     A base class for a theme.
    /// </summary>
    public abstract class Theme
    {
        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <value>
        /// The boolean.
        /// </value>
        public abstract Drawable Boolean { get; }

        /// <summary>
        /// Gets the slider.
        /// </summary>
        /// <value>
        /// The slider.
        /// </value>
        public abstract Drawable Slider { get; }

        /// <summary>
        /// Gets the key bind.
        /// </summary>
        /// <value>
        /// The key bind.
        /// </value>
        public abstract Drawable KeyBind { get; }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public abstract DrawableList List { get; }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        public abstract Drawable Separator { get; }

        /// <summary>
        /// Called when the menu is drawn..
        /// </summary>
        /// <param name="position">The position.</param>
        public abstract void OnDraw(Vector2 position);
        /// <summary>
        /// Called when is drawn.
        /// </summary>
        /// <param name="menuComponent">The menu component.</param>
        /// <param name="position">The position.</param>
        /// <param name="index">The index.</param>
        public abstract void OnMenu(Menu menuComponent, Vector2 position, int index);

        /// <summary>
        /// Calculates the width of the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public abstract int CalcWidthMenu(Menu menu);

        /// <summary>
        /// Calculates the width of an item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns></returns>
        public abstract int CalcWidthItem(MenuItem menuItem);

        /// <summary>
        /// Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public abstract int CalcWidthText(string text);

        /// <summary>
        ///     Class for Animations.
        /// </summary>
        public class Animation
        {
            /// <summary>
            ///     Function that returns <c>true</c> if this object is animating. 
            /// </summary>
            public Func<bool> IsAnimating;

            /// <summary>
            ///     A method that draws the animation.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;
        }

        /// <summary>
        ///     A struct that is drawable.
        /// </summary>
        public struct Drawable
        {
            /// <summary>
            ///     Gets the additional boundries.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> AdditionalBoundries;

            /// <summary>
            /// The animation
            /// </summary>
            public Animation Animation;

            /// <summary>
            /// The bounding
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> Bounding;

            /// <summary>
            ///     Draws this item.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;
        }

        /// <summary>
        ///     A drawable list.
        /// </summary>
        public struct DrawableList
        {
            /// <summary>
            ///     Gets the additional boundries.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> AdditionalBoundries;

            /// <summary>
            /// The animation
            /// </summary>
            public Animation Animation;
            /// <summary>
            ///     Gets the bounding.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> Bounding;

            /// <summary>
            ///     Methods that draws this DrawableList.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;

            /// <summary>
            ///     Gets the right arrow.
            /// </summary>
            public Func<Vector2, AMenuComponent, MenuList, Rectangle> RightArrow;
            /// <summary>
            ///     Gets the left arrow
            /// </summary>
            public Func<Vector2, AMenuComponent, MenuList, Rectangle> LeftArrow;

            /// <summary>
            ///     Gets the width
            /// </summary>
            public Func<MenuList, int> Width;
        }
    }
}