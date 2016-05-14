// <copyright file="ADrawableAdapter.cs" company="LeagueSharp">
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
    ///     Provides an implementation of <see cref="ADrawable" /> that does nothing. This is used to prevent exceptions when
    ///     no <see cref="ADrawable" /> exists for a given <see cref="AMenuComponent" />.
    /// </summary>
    public class ADrawableAdapter : ADrawable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draws the <see cref="AMenuComponent" />.
        /// </summary>
        public override void Draw()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Handles the window events for this <see cref="AMenuComponent" />.
        /// </summary>
        /// <param name="args">Event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            // Do nothing.
        }

        /// <summary>
        ///     Calculates the width of this <see cref="AMenuComponent" />.
        /// </summary>
        /// <returns>The width of this <see cref="AMenuComponent" />.</returns>
        public override int Width()
        {
            return 100;
        }

        #endregion
    }
}