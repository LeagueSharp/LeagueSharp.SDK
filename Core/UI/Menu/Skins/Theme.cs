// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Theme.cs" company="LeagueSharp">
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
//   A base class for a theme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Skins
{
    using System;

    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     A base class for a theme.
    /// </summary>
    public abstract class Theme
    {
        #region Public Properties

        /// <summary>
        ///     Gets the boolean.
        /// </summary>
        /// <value>
        ///     The boolean.
        /// </value>
        public abstract Drawable Boolean { get; }

        /// <summary>
        ///     Gets the key bind.
        /// </summary>
        /// <value>
        ///     The key bind.
        /// </value>
        public abstract Drawable KeyBind { get; }

        /// <summary>
        ///     Gets the list.
        /// </summary>
        /// <value>
        ///     The list.
        /// </value>
        public abstract DrawableList List { get; }

        /// <summary>
        ///     Gets the separator.
        /// </summary>
        /// <value>
        ///     The separator.
        /// </value>
        public abstract Drawable Separator { get; }

        /// <summary>
        ///     Gets the slider.
        /// </summary>
        /// <value>
        ///     The slider.
        /// </value>
        public abstract Drawable Slider { get; }

        /// <summary>
        ///     Gets the button.
        /// </summary>
        /// <value>
        ///     The button.
        /// </value>
        public abstract DrawableButton Button { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the width of an item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <returns>The <see cref="int" /></returns>
        public abstract int CalcWidthItem(MenuItem menuItem);

        /// <summary>
        ///     Calculates the width of the menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns>The <see cref="int" /></returns>
        public abstract int CalcWidthMenu(Menu menu);

        /// <summary>
        ///     Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="int" /></returns>
        public abstract int CalcWidthText(string text);

        /// <summary>
        ///     Called when the menu is drawn..
        /// </summary>
        /// <param name="position">The position.</param>
        public abstract void OnDraw(Vector2 position);

        /// <summary>
        ///     Called when is drawn.
        /// </summary>
        /// <param name="menuComponent">The menu component.</param>
        /// <param name="position">The position.</param>
        /// <param name="index">The index.</param>
        public abstract void OnMenu(Menu menuComponent, Vector2 position, int index);

        #endregion

        /// <summary>
        ///     A struct that is draw-able.
        /// </summary>
        public struct Drawable
        {
            #region Fields

            /// <summary>
            ///     Gets the additional boundaries.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> AdditionalBoundries;

            /// <summary>
            ///     The animation
            /// </summary>
            public Animation Animation;

            /// <summary>
            ///     The bounding
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> Bounding;

            /// <summary>
            ///     Draws this item.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;

            #endregion
        }

        /// <summary>
        ///     A draw-able list.
        /// </summary>
        public struct DrawableList
        {
            #region Fields

            /// <summary>
            ///     Gets the additional boundaries.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> AdditionalBoundries;

            /// <summary>
            ///     The animation
            /// </summary>
            public Animation Animation;

            /// <summary>
            ///     Gets the bounding.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> Bounding;

            /// <summary>
            ///     Gets the left arrow
            /// </summary>
            public Func<Vector2, AMenuComponent, MenuList, Rectangle> LeftArrow;

            /// <summary>
            ///     Methods that draws this <c>DrawableList</c>.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;

            /// <summary>
            ///     Gets the right arrow.
            /// </summary>
            public Func<Vector2, AMenuComponent, MenuList, Rectangle> RightArrow;

            /// <summary>
            ///     Gets the width
            /// </summary>
            public Func<MenuList, int> Width;

            #endregion
        }


        /// <summary>
        ///     A draw-able list.
        /// </summary>
        public struct DrawableButton
        {
            #region Fields

            /// <summary>
            ///     Gets the additional boundaries.
            /// </summary>
            public Func<Vector2, AMenuComponent, Rectangle> ButtonBoundaries;

            /// <summary>
            ///     Methods that draws this <c>DrawableList</c>.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw;

            /// <summary>
            ///     Gets the width
            /// </summary>
            public Func<MenuButton, int> Width;

            #endregion
        }

        /// <summary>
        ///     Class for Animations.
        /// </summary>
        public class Animation
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the function that returns <c>true</c> if this object is animating.
            /// </summary>
            public Func<bool> IsAnimating { get; set; }

            /// <summary>
            ///     Gets or sets the method that draws the animation.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnDraw { get; set; }

            #endregion
        }
    }
}