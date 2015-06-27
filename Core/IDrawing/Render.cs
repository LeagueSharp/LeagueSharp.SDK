// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Render.cs" company="LeagueSharp">
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
//   The Render class, main handler for projecting objects onto screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.IDrawing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The Render class, main handler for projecting objects onto screen.
    /// </summary>
    public static class Render
    {
        #region Static Fields

        /// <summary>
        ///     The objects queue to be drawn.
        /// </summary>
        public static readonly List<DrawObject> Objects = new List<DrawObject>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Render" /> class.
        /// </summary>
        static Render()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnBeginScene += OnBeginScene;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnPreReset += OnPreReset;
            Drawing.OnPostReset += OnPostReset;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a <see cref="DrawObject" /> to the render list.
        /// </summary>
        /// <typeparam name="T">
        ///     DrawObject type
        /// </typeparam>
        /// <param name="drawObject">
        ///     The <see cref="DrawObject" />
        /// </param>
        /// <returns>
        ///     The <see cref="DrawObject" />.
        /// </returns>
        public static T Add<T>(this T drawObject) where T : DrawObject
        {
            if (!Objects.Contains(drawObject))
            {
                Objects.Add(drawObject);
                return drawObject;
            }

            return null;
        }

        /// <summary>
        ///     Removes a <see cref="DrawObject" /> from the render list.
        /// </summary>
        /// <param name="drawObject">
        ///     The <see cref="DrawObject" />
        /// </param>
        public static void Remove(this DrawObject drawObject)
        {
            if (Objects.Contains(drawObject))
            {
                Objects.Remove(drawObject);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The OnBeginScene event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnBeginScene(EventArgs args)
        {
            foreach (
                var @object in Objects.Where(o => o.DrawType.HasFlag(DrawType.OnBeginScene)).OrderBy(o => o.OrderIndex))
            {
                @object.Draw(DrawType.OnBeginScene);
            }
        }

        /// <summary>
        ///     The OnDraw event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnDraw(EventArgs args)
        {
            foreach (var @object in Objects.Where(o => o.DrawType.HasFlag(DrawType.OnDraw)).OrderBy(o => o.OrderIndex))
            {
                @object.Draw(DrawType.OnDraw);
            }
        }

        /// <summary>
        ///     The OnEndScene event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnEndScene(EventArgs args)
        {
            foreach (
                var @object in Objects.Where(o => o.DrawType.HasFlag(DrawType.OnEndScene)).OrderBy(o => o.OrderIndex))
            {
                @object.Draw(DrawType.OnEndScene);
            }
        }

        /// <summary>
        ///     The OnPostReset event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnPostReset(EventArgs args)
        {
            foreach (var @object in Objects.OrderBy(o => o.OrderIndex))
            {
                @object.OnPostReset();
            }
        }

        /// <summary>
        ///     The OnPreReset event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnPreReset(EventArgs args)
        {
            foreach (var @object in Objects.OrderBy(o => o.OrderIndex))
            {
                @object.OnPreReset();
            }
        }

        /// <summary>
        ///     The OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            foreach (var @object in Objects.OrderBy(o => o.OrderIndex))
            {
                @object.Update();
            }
        }

        #endregion
    }
}