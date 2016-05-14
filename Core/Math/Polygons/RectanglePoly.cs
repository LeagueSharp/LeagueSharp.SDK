// <copyright file="RectanglePoly.cs" company="LeagueSharp">
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
    using SharpDX;

    /// <summary>
    ///     Represents a Rectangle <see cref="Polygon" />
    /// </summary>
    public class RectanglePoly : Polygon
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectanglePoly" /> class.
        /// </summary>
        /// <param name="start">
        ///     The Start
        /// </param>
        /// <param name="end">
        ///     The End
        /// </param>
        /// <param name="width">
        ///     The Width
        /// </param>
        public RectanglePoly(Vector3 start, Vector3 end, float width)
            : this(start.ToVector2(), end.ToVector2(), width)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectanglePoly" /> class.
        /// </summary>
        /// <param name="start">
        ///     The Start
        /// </param>
        /// <param name="end">
        ///     The End
        /// </param>
        /// <param name="width">
        ///     The Width
        /// </param>
        public RectanglePoly(Vector2 start, Vector2 end, float width)
        {
            this.Start = start;
            this.End = end;
            this.Width = width;

            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the direction of the Rectangle(Does not need update)
        /// </summary>
        public Vector2 Direction => (this.End - this.Start).Normalized();

        /// <summary>
        ///     Gets or sets the end.
        /// </summary>
        public Vector2 End { get; set; }

        /// <summary>
        ///     Gets a perpendicular direction of the Rectangle(Does not need an update)
        /// </summary>
        public Vector2 Perpendicular => this.Direction.Perpendicular();

        /// <summary>
        ///     Gets or sets the start.
        /// </summary>
        public Vector2 Start { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        public float Width { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the Polygon. Call this after changing something.
        /// </summary>
        /// <param name="offset">Extra width</param>
        /// <param name="overrideWidth">New width to use, overriding the set one.</param>
        public void UpdatePolygon(int offset = 0, float overrideWidth = -1)
        {
            this.Points.Clear();
            this.Points.Add(
                this.Start + ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)
                - (offset * this.Direction));
            this.Points.Add(
                this.Start - ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)
                - (offset * this.Direction));
            this.Points.Add(
                this.End - ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)
                + (offset * this.Direction));
            this.Points.Add(
                this.End + ((overrideWidth > 0 ? overrideWidth : this.Width + offset) * this.Perpendicular)
                + (offset * this.Direction));
        }

        #endregion
    }
}