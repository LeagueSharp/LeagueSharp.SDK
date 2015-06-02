// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueChangedEventArgs.cs" company="LeagueSharp">
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
//   Arguments for the OnValueChanged event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu
{
    using System;

    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;

    /// <summary>
    ///     Arguments for the OnValueChanged event.
    /// </summary>
    /// <typeparam name="T">
    ///     <see cref="AMenuValue" /> type
    /// </typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
        where T : AMenuValue
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueChangedEventArgs{T}" /> class.
        ///     Initializes a new instance of the OnValueChangedEventArgs class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public ValueChangedEventArgs(T value)
        {
            this.Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the new Value.
        /// </summary>
        public T Value { get; private set; }

        #endregion
    }
}