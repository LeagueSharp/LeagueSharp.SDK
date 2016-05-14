// <copyright file="ADrawable.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.UI.Skins
{
    using LeagueSharp.SDKEx.Utils;

    /// <summary>
    ///     Defines a handler which is responsible for the drawing and interactions of an <see cref="AMenuComponent" />.
    /// </summary>
    public abstract class ADrawable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        ///     Draws the <see cref="AMenuComponent" />.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        ///     Handles the window events for this <see cref="AMenuComponent" />.
        /// </summary>
        /// <param name="args">Event data</param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        ///     Calculates the width of this <see cref="AMenuComponent" />.
        /// </summary>
        /// <returns>
        ///     The width of this <see cref="AMenuComponent" />.
        /// </returns>
        public abstract int Width();

        #endregion
    }

    /// <summary>
    ///     Defines a handler which is responsible for the drawing and interactions of an <see cref="AMenuComponent" />.
    /// </summary>
    /// <typeparam name="T"><see cref="ADrawable" /> type</typeparam>
    public abstract class ADrawable<T> : ADrawable
        where T : AMenuComponent
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ADrawable{T}" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        protected ADrawable(T component)
        {
            this.Component = component;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the <see cref="AMenuComponent" /> where this <see cref="ADrawable" /> is responsible for.
        /// </summary>
        protected T Component { get; private set; }

        #endregion
    }
}