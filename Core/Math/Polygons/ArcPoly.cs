// <copyright file="ArcPoly.cs" company="LeagueSharp">
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
    ///     Represents an Arc <see cref="Polygon" />
    /// </summary>
    public class ArcPoly : Polygon
    {
        #region Fields

        /// <summary>
        ///     Arc Quality
        /// </summary>
        private readonly int quality;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArcPoly" /> class, after converting the points to 2D.
        /// </summary>
        /// <param name="start">
        ///     Start of the Arc
        /// </param>
        /// <param name="direction">
        ///     Direction of the Arc
        /// </param>
        /// <param name="angle">
        ///     Angle of the Arc
        /// </param>
        /// <param name="radius">
        ///     Radius of the Arc
        /// </param>
        /// <param name="quality">
        ///     Quality of the Arc
        /// </param>
        public ArcPoly(Vector3 start, Vector3 direction, float angle, float radius, int quality = 20)
            : this(start.ToVector2(), direction.ToVector2(), angle, radius, quality)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArcPoly" /> class.
        /// </summary>
        /// <param name="start">
        ///     Start of the Arc
        /// </param>
        /// <param name="end">
        ///     End of the Arc
        /// </param>
        /// <param name="angle">
        ///     Angle of the Arc
        /// </param>
        /// <param name="radius">
        ///     Radius of the Arc
        /// </param>
        /// <param name="quality">
        ///     Quality of the Arc
        /// </param>
        public ArcPoly(Vector2 start, Vector2 end, float angle, float radius, int quality = 20)
        {
            this.StartPos = start;
            this.EndPos = (end - start).Normalized();
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
        ///     Gets or sets the end position.
        /// </summary>
        public Vector2 EndPos { get; set; }

        /// <summary>
        ///     Gets or sets the radius.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        ///     Gets or sets the start position.
        /// </summary>
        public Vector2 StartPos { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the Arc. Use this after changing something.
        /// </summary>
        /// <param name="offset">Radius extra offset</param>
        public void UpdatePolygon(int offset = 0)
        {
            this.Points.Clear();

            var outRadius = (this.Radius + offset) / (float)Math.Cos(2 * Math.PI / this.quality);
            var side1 = this.EndPos.Rotated(-this.Angle * 0.5f);

            for (var i = 0; i <= this.quality; i++)
            {
                var cDirection = side1.Rotated(i * this.Angle / this.quality).Normalized();
                this.Points.Add(
                    new Vector2(
                        this.StartPos.X + (outRadius * cDirection.X),
                        this.StartPos.Y + (outRadius * cDirection.Y)));
            }
        }

        #endregion
    }
}