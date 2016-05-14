// <copyright file="RadioMenu.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI
{
    using System;

    /// <summary>
    ///     RadioMenu Item.
    /// </summary>
    public class RadioMenu : Menu
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RadioMenu" /> class.
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">
        ///     Menu Name
        /// </param>
        /// <param name="displayName">
        ///     Menu Display Name
        /// </param>
        /// <param name="root">
        ///     Root component
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string
        /// </param>
        public RadioMenu(string name, string displayName, bool root = false, string uniqueString = "")
            : base(name, displayName, root, uniqueString)
        {
            this.MenuValueChanged += this.RadioMenuValueChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Radio Menu when a value is changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void RadioMenuValueChanged(object sender, MenuValueChangedEventArgs args)
        {
            try
            {
                var menuBool = args.MenuItem as MenuBool;

                if (menuBool != null && menuBool.Value)
                {
                    foreach (var comp in this.Components)
                    {
                        var child = comp.Value as MenuBool;
                        if (child != null && child.Name != menuBool.Name)
                        {
                            child.Value = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion
    }
}