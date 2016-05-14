// <copyright file="MenuValueChangedEventArgs.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.UI
{
    using System;

    /// <summary>
    ///     Arguments for the OnValueChanged event.
    /// </summary>
    public class MenuValueChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuValueChangedEventArgs" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="menuItem">The MenuItem that changed value</param>
        public MenuValueChangedEventArgs(Menu menu, MenuItem menuItem)
        {
            this.Menu = menu;
            this.MenuItem = menuItem;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the menu that contains the changed MenuItem.
        /// </summary>
        public Menu Menu { get; private set; }

        /// <summary>
        ///     Gets the MenuItem that changed value.
        /// </summary>
        public MenuItem MenuItem { get; private set; }

        #endregion
    }
}