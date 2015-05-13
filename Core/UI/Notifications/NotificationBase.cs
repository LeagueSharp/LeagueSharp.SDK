// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationBase.cs" company="LeagueSharp">
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
//   Notification base abstract class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Notifications
{
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Notification base abstract class.
    /// </summary>
    public abstract class NotificationBase : INotification
    {
        #region Fields

        /// <summary>
        ///     Notification Height.
        /// </summary>
        private float height = 25f;

        /// <summary>
        ///     Notification Width.
        /// </summary>
        private float width = 190f;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Notification flags.
        /// </summary>
        public NotificationFlags Flags { get; set; }

        /// <summary>
        ///     Gets or sets the Notification Height.
        /// </summary>
        public float Height
        {
            get
            {
                return this.height;
            }

            set
            {
                this.height = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Notification Position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets the Notification Width.
        /// </summary>
        public float Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clone function which clones the notification information and creates a new one, however position and GUID are
        ///     re-generated.
        /// </summary>
        /// <returns>Object in the type of <see cref="NotificationBase" /> class</returns>
        public abstract object Clone();

        /// <summary>
        ///     Disposal by user-call.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        ///     Retrieves the global unique identification.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        public abstract string GetGuid();

        /// <summary>
        ///     On drawing event callback.
        /// </summary>
        /// <param name="parentPosition">Base Position</param>
        public abstract void OnDraw(Vector2 parentPosition);

        /// <summary>
        ///     On update event callback.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        ///     On windows process messages callback.
        /// </summary>
        /// <param name="keys">Converted WindowsKeys</param>
        public abstract void OnWndProc(WindowsKeys keys);

        #endregion
    }
}