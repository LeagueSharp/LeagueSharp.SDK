// <copyright file="WeightedRandom.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Weighted Random, contains useful extensions for randomizer.
    /// </summary>
    public static class WeightedRandom
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the random that is being used by WeightedRandom.
        /// </summary>
        public static Random Random { get; set; } = new Random(Variables.TickCount);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns a random integer
        /// </summary>
        /// <param name="min">Minimum range</param>
        /// <param name="max">Maximum range</param>
        /// <returns>Random Integer</returns>
        public static int Next(int min, int max)
        {
            var list = new List<int>();
            list.AddRange(Enumerable.Range(min, max));

            var mean = list.Average();
            var stdDev = list.StandardDeviation();

            var v1 = Random.NextDouble();
            var v2 = Random.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(v1)) * Math.Sin(2.0 * Math.PI * v2);
            return (int)(mean + (stdDev * randStdNormal));
        }

        #endregion
    }
}