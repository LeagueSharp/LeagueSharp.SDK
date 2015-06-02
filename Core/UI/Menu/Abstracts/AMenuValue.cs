// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AMenuValue.cs" company="LeagueSharp">
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
//   Abstract build of a Menu Value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Abstracts
{
    using System;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Abstract build of a Menu Value.
    /// </summary>
    [Serializable]
    public abstract class AMenuValue
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Value Container.
        /// </summary>
        public MenuItem Container { get; set; }

        /// <summary>
        ///     Gets the Value Width.
        /// </summary>
        public abstract int Width { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public abstract void Extract(AMenuValue component);

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public abstract void OnDraw();

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        /// Orders the Container to fire an event
        /// </summary>
        protected void FireEvent()
        {
            if (Container != null)
            {
                Container.FireEvent();
            }
        }

        #endregion
    }
}