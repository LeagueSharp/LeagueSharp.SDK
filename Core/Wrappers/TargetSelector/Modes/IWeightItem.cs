// <copyright file="IWeightItem.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.TSModes
{
    /// <summary>
    ///     Interface for weight item
    /// </summary>
    public interface IWeightItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets the default weight.
        /// </summary>
        int DefaultWeight { get; }

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IWeightItem" /> is inverted.
        /// </summary>
        bool Inverted { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="float" /> value.
        /// </returns>
        float GetValue(Obj_AI_Hero hero);

        #endregion
    }
}