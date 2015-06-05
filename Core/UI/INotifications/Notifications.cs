// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Notifications.cs" company="LeagueSharp">
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
//   The notifications main handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.INotifications
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     The notifications main handler.
    /// </summary>
    public class Notifications
    {
        #region Static Fields

        /// <summary>
        ///     The notifications.
        /// </summary>
        public static readonly List<ANotification> NotificationsList = new List<ANotification>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public static Vector2 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a notification towards the list.
        /// </summary>
        /// <param name="notification">
        ///     The notification.
        /// </param>
        public static void Add(ANotification notification)
        {
            if (!NotificationsList.Contains(notification))
            {
                NotificationsList.Add(notification);
            }
        }

        /// <summary>
        ///     Initializes static members of the <see cref="Notifications" /> class.
        /// </summary>
        public static void Initialize()
        {
            Position = new Vector2(Drawing.Width - 5f, 90f);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Game.OnWndProc += OnWndProc;
        }

        /// <summary>
        ///     Removes a notification from the list.
        /// </summary>
        /// <param name="notification">
        ///     The notification.
        /// </param>
        public static void Remove(Notification notification)
        {
            if (NotificationsList.Contains(notification))
            {
                NotificationsList.Remove(notification);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnDraw(EventArgs args)
        {
            var height = Position.Y;
            foreach (var notification in NotificationsList.ToArray())
            {
                notification.OnDraw(new Vector2(Position.X, height));
                height += notification.GetReservedHeight();
            }
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            foreach (var notification in NotificationsList.ToArray())
            {
                notification.OnUpdate();
            }
        }

        /// <summary>
        ///     <c>OnWndProc</c> event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnWndProc(WndEventArgs args)
        {
            var windowsKeys = new WindowsKeys(args);
            var height = Position.Y;
            foreach (var notification in NotificationsList.ToArray())
            {
                notification.OnWndProc(new Vector2(Position.X, height), windowsKeys);
                height += notification.GetReservedHeight();
            }
        }

        #endregion
    }
}