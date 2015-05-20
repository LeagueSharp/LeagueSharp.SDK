// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuFactory.cs" company="LeagueSharp">
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
//   Menu Value Factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     Menu Value Factory.
    /// </summary>
    public class MenuFactory
    {
        #region Static Fields

        /// <summary>
        ///     Default Menu Values.
        /// </summary>
        private static readonly IDictionary<Type, Func<AMenuValue>> DefaultMenuValueByType =
            new Dictionary<Type, Func<AMenuValue>>
                {
                    { typeof(MenuBool), () => new MenuBool() }, { typeof(MenuKeyBind), () => new MenuKeyBind() }, 
                    { typeof(MenuSlider), () => new MenuSlider() }, { typeof(MenuInputText), () => new MenuInputText() }, 
                    { typeof(MenuSeparator), () => new MenuSeparator() },
                    { typeof(MenuColor), () => new MenuColor(Color.Green) }
                };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a new value out of the given type.
        /// </summary>
        /// <typeparam name="T">
        ///     <see cref="AMenuValue" /> type
        /// </typeparam>
        /// <returns>Created given type</returns>
        public static T Create<T>() where T : AMenuValue
        {
            if (DefaultMenuValueByType.ContainsKey(typeof(T)))
            {
                return (T)DefaultMenuValueByType[typeof(T)].Invoke();
            }

            return default(T);
        }

        /// <summary>
        ///     Stores the new factory values into the menu factory container.
        /// </summary>
        /// <typeparam name="T"><see cref="AMenuValue" /> to contain into the factory value container</typeparam>
        /// <param name="value">Standard build for the constructor</param>
        /// <returns>The <see cref="bool" /></returns>
        public static bool CreateFactory<T>(AMenuValue value) where T : AMenuValue
        {
            if (!DefaultMenuValueByType.ContainsKey(typeof(T)))
            {
                DefaultMenuValueByType.Add(typeof(T), () => value);
                return true;
            }

            return false;
        }

        #endregion
    }
}