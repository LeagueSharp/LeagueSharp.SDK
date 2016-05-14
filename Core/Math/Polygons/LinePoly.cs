// <copyright file="LinePoly.cs" company="LeagueSharp">
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
    ///     Represents a Line <see cref="Polygon" />
    /// </summary>
    public class LinePoly : Polygon
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinePoly" /> class.
        /// </summary>
        /// <param name="start">
        ///     The Start
        /// </param>
        /// <param name="end">
        ///     The End
        /// </param>
        /// <param name="length">
        ///     Length of line(will be automatically set if -1)
        /// </param>
        public LinePoly(Vector3 start, Vector3 end, float length = -1)
            : this(start.ToVector2(), end.ToVector2(), length)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinePoly" /> class.
        /// </summary>
        /// <param name="start">
        ///     The Start
        /// </param>
        /// <param name="end">
        ///     The End
        /// </param>
        /// <param name="length">
        ///     Length of line(will be automatically set if -1)
        /// </param>
        public LinePoly(Vector2 start, Vector2 end, float length = -1)
        {
            this.LineStart = start;
            this.LineEnd = end;

            if (length > 0)
            {
                this.Length = length;
            }

            this.UpdatePolygon();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the length of the Line. (Does not have to be updated)
        /// </summary>
        public float Length
        {
            get
            {
                return this.LineStart.Distance(this.LineEnd);
            }

            set
            {
                this.LineEnd = ((this.LineEnd - this.LineStart).Normalized() * value) + this.LineStart;
            }
        }

        /// <summary>
        ///     Gets or sets the End of the Line.
        /// </summary>
        public Vector2 LineEnd { get; set; }

        /// <summary>
        ///     Gets or sets the Start of the Line.
        /// </summary>
        public Vector2 LineStart { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates the polygon. Use this after changing something.
        /// </summary>
        public void UpdatePolygon()
        {
            this.Points.Clear();
            this.Points.Add(this.LineStart);
            this.Points.Add(this.LineEnd);
        }

        #endregion
    }
}