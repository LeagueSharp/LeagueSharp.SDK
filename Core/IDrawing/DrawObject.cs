// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawObject.cs" company="LeagueSharp">
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
//   The DrawObject class, contains information required to construct a specific
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.IDrawing
{
    using System;
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     The DrawObject class, contains information required to construct a specific
    /// </summary>
    public abstract class DrawObject
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawObject" /> class.
        /// </summary>
        protected DrawObject()
        {
            this.DrawType = DrawType.OnDraw;
            this.IsVisible = true;

            var highestIndex = Render.Objects.OrderByDescending(o => o.OrderIndex).FirstOrDefault();
            this.OrderIndex = highestIndex != null ? highestIndex.OrderIndex + 1 : 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DrawObject" /> class.
        /// </summary>
        /// <param name="position">
        ///     The position
        /// </param>
        protected DrawObject(Vector2 position)
            : this()
        {
            this.Position = position;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The main message delegate.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="e">
        ///     The event data
        /// </param>
        public delegate void MessageDelegate(object sender, EventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     The OnDraw event.
        /// </summary>
        public event MessageDelegate OnDraw;

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        public event MessageDelegate OnUpdate;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Gets or sets the draw type.
        /// </summary>
        public DrawType DrawType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the element is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        ///     Gets or sets the stack order of an element.
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public Vector2 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The drawing event call.
        /// </summary>
        /// <param name="drawType">
        ///     The draw Type.
        /// </param>
        public abstract void Draw(DrawType drawType);

        /// <summary>
        ///     The OnLostDevice event.
        /// </summary>
        public abstract void OnPostReset();

        /// <summary>
        ///     The OnPreReset event.
        /// </summary>
        public abstract void OnPreReset();

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        public abstract void Update();

        #endregion

        #region Methods

        /// <summary>
        ///     Call <see cref="OnDraw" /> event.
        /// </summary>
        protected virtual void CallOnDraw()
        {
            var handler = this.OnDraw;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Call <see cref="OnUpdate" /> event.
        /// </summary>
        protected virtual void CallOnUpdate()
        {
            var handler = this.OnUpdate;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}