// <copyright file="CirclePoly.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Polygons
{
    using System;

    using SharpDX;

    /// <summary>
    ///     Represents a Circle <see cref="Polygon" />
    /// </summary>
    public class CirclePoly : Polygon
    {
        #region Fields

        /// <summary>
        ///     Circle Quality
        /// </summary>
        private readonly int quality;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CirclePoly" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="radius">
        ///     The Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public CirclePoly(Vector3 center, float radius, int quality = 20)
            : this(center.ToVector2(), radius, quality)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CirclePoly" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="radius">
        ///     The Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public CirclePoly(Vector2 center, float radius, int quality = 20)
        {
            this.Center = center;
            this.Radius = radius;
            this.quality = quality;

            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Center of the Circle.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        ///     Gets or sets the Radius of Circle.
        /// </summary>
        public float Radius { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the Polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Extra radius</param>
        /// <param name="overrideWidth">New width to use, overriding the set one.</param>
        public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
        {
            this.Points.Clear();

            var outRadius = overrideWidth > 0
                                ? overrideWidth
                                : (offset + this.Radius) / (float)Math.Cos(2 * Math.PI / this.quality);

            for (var i = 1; i <= this.quality; i++)
            {
                var angle = i * 2 * Math.PI / this.quality;
                var point = new Vector2(
                    this.Center.X + (outRadius * (float)Math.Cos(angle)),
                    this.Center.Y + (outRadius * (float)Math.Sin(angle)));

                this.Points.Add(point);
            }
        }

        #endregion
    }
}