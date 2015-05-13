// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ring.cs" company="LeagueSharp">
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
//   Represents a Ring <see cref="Polygon" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Math.Polygons
{
    using System;

    using LeagueSharp.SDK.Core.Extensions.SharpDX;

    using SharpDX;

    /// <summary>
    ///     Represents a Ring <see cref="Polygon" />
    /// </summary>
    public class Ring : Polygon
    {
        #region Fields

        /// <summary>
        ///     Ring Quality
        /// </summary>
        private readonly int quality;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ring" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="innerRadius">
        ///     Inner Radius
        /// </param>
        /// <param name="outerRadius">
        ///     Outer Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public Ring(Vector3 center, float innerRadius, float outerRadius, int quality = 20)
            : this(center.ToVector2(), innerRadius, outerRadius, quality)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ring" /> class.
        /// </summary>
        /// <param name="center">
        ///     The Center
        /// </param>
        /// <param name="innerRadius">
        ///     Inner Radius
        /// </param>
        /// <param name="outerRadius">
        ///     Outer Radius
        /// </param>
        /// <param name="quality">
        ///     The Quality
        /// </param>
        public Ring(Vector2 center, float innerRadius, float outerRadius, int quality = 20)
        {
            this.Center = center;
            this.InnerRadius = innerRadius;
            this.OuterRadius = outerRadius;
            this.quality = quality;

            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        public Vector2 Center { get; set; }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        public float InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the outer radius.
        /// </summary>
        public float OuterRadius { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the polygon. Call this after you change something.
        /// </summary>
        /// <param name="offset">Added radius</param>
        public void UpdatePolygon(int offset = 0)
        {
            this.Points.Clear();

            var outRadius = (offset + this.InnerRadius + this.OuterRadius) / (float)Math.Cos(2 * Math.PI / this.quality);
            var innerRadius = this.InnerRadius - this.OuterRadius - offset;

            for (var i = 0; i <= this.quality; i++)
            {
                var angle = i * 2 * Math.PI / this.quality;
                var point = new Vector2(
                    this.Center.X - outRadius * (float)Math.Cos(angle), 
                    this.Center.Y - outRadius * (float)Math.Sin(angle));

                this.Points.Add(point);
            }

            for (var i = 0; i <= this.quality; i++)
            {
                var angle = i * 2 * Math.PI / this.quality;
                var point = new Vector2(
                    this.Center.X + innerRadius * (float)Math.Cos(angle), 
                    this.Center.Y - innerRadius * (float)Math.Sin(angle));

                this.Points.Add(point);
            }
        }

        #endregion
    }
}