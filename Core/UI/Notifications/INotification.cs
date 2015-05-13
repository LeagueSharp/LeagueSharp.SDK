// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotification.cs" company="LeagueSharp">
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
//   Notification Interface to communicate between a notification type and the notifications manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Notifications
{
    using System;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Notification Interface to communicate between a notification type and the notifications manager.
    /// </summary>
    public interface INotification : IDisposable, ICloneable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sends a global unique identification request to the notification type.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        string GetGuid();

        /// <summary>
        ///     Send an drawing request to the callback.
        /// </summary>
        /// <param name="parentPosition">
        ///     The parent Position.
        /// </param>
        void OnDraw(Vector2 parentPosition);

        /// <summary>
        ///     Send an update request to the callback.
        /// </summary>
        void OnUpdate();

        /// <summary>
        ///     Send an windows process message to the callback with formatted keys.
        /// </summary>
        /// <param name="keys">Formatted Keys</param>
        void OnWndProc(WindowsKeys keys);

        #endregion
    }
}