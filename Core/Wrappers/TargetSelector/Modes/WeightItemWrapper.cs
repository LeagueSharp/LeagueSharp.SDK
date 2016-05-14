// <copyright file="WeightItemWrapper.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.TSModes
{
    using System;

    /// <summary>
    ///     Wrapper for IWeightItem.
    /// </summary>
    public class WeightItemWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeightItemWrapper" /> class.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        public WeightItemWrapper(IWeightItem item)
        {
            this.Item = item;
            this.Weight = item.DefaultWeight;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the default weight
        /// </summary>
        public int DefaultWeight => this.Item.DefaultWeight;

        /// <summary>
        ///     Gets the display name
        /// </summary>
        public string DisplayName => this.Item.DisplayName;

        /// <summary>
        ///     Gets a value indicating whether the item is inverted.
        /// </summary>
        public bool Inverted => this.Item.Inverted;

        /// <summary>
        ///     Gets the item.
        /// </summary>
        public IWeightItem Item { get; internal set; }

        /// <summary>
        ///     Gets the maximum value.
        /// </summary>
        public float MaxValue { get; internal set; }

        /// <summary>
        ///     Gets the minimum value.
        /// </summary>
        public float MinValue { get; internal set; }

        /// <summary>
        ///     Gets the name
        /// </summary>
        public string Name => this.Item.Name;

        /// <summary>
        ///     Gets the simulation maximum value.
        /// </summary>
        public float SimulationMaxValue { get; internal set; }

        /// <summary>
        ///     Gets the simulation minimum value.
        /// </summary>
        public float SimulationMinValue { get; internal set; }

        /// <summary>
        ///     Gets the weight.
        /// </summary>
        public int Weight { get; internal set; }

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
        public float GetValue(Obj_AI_Hero hero) => Math.Max(0, this.Item.GetValue(hero));

        #endregion
    }
}