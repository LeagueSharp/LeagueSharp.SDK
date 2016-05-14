// <copyright file="SectorPoly.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Polygons
{
    using System;

    using SharpDX;

    /// <summary>
    ///     Represents a Sector <see cref="Polygon" />
    /// </summary>
    public class SectorPoly : Polygon
    {
        #region Fields

        /// <summary>
        ///     Local quality.
        /// </summary>
        private readonly int quality;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorPoly" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="direction">
        ///     The Direction
        /// </param>
        /// <param name="angle">
        ///     The Angle
        /// </param>
        /// <param name="radius">
        ///     The Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public SectorPoly(Vector3 center, Vector3 direction, float angle, float radius, int quality = 20)
            : this(center.ToVector2(), direction.ToVector2(), angle, radius, quality)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SectorPoly" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="endPosition">
        ///     The end position
        /// </param>
        /// <param name="angle">
        ///     The Angle
        /// </param>
        /// <param name="radius">
        ///     The Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public SectorPoly(Vector2 center, Vector2 endPosition, float angle, float radius, int quality = 20)
        {
            this.Center = center;
            this.Direction = (endPosition - center).Normalized();
            this.Angle = angle;
            this.Radius = radius;
            this.quality = quality;

            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the angle.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        ///     Gets or sets the center.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        ///     Gets or sets the direction.
        /// </summary>
        public Vector2 Direction { get; set; }

        /// <summary>
        ///     Gets or sets the radius.
        /// </summary>
        public float Radius { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Added radius</param>
        public void UpdatePolygon(int offset = 0)
        {
            this.Points.Clear();

            var outRadius = (this.Radius + offset) / (float)Math.Cos(2 * Math.PI / this.quality);
            this.Points.Add(this.Center);
            var side1 = this.Direction.Rotated(-this.Angle * 0.5f);

            for (var i = 0; i <= this.quality; i++)
            {
                var cDirection = side1.Rotated(i * this.Angle / this.quality).Normalized();
                this.Points.Add(
                    new Vector2(this.Center.X + (outRadius * cDirection.X), this.Center.Y + (outRadius * cDirection.Y)));
            }
        }

        #endregion
    }
}