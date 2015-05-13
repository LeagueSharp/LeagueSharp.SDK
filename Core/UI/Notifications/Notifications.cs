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
//   Notifications class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.UI.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Notifications class.
    /// </summary>
    public static class Notifications
    {
        #region Static Fields

        /// <summary>
        ///     Notifications list.
        /// </summary>
        public static readonly List<NotificationBase> NotificationsList = new List<NotificationBase>();

        /// <summary>
        ///     Notifications array.
        /// </summary>
        private static NotificationBase[] notifications;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Notifications"/> class. 
        ///     Static constructor.
        /// </summary>
        static Notifications()
        {
            Position = new Vector2(Drawing.Width - 250, 10);
            Game.OnUpdate += args =>
                {
                    foreach (var notificaiton in notifications)
                    {
                        notificaiton.OnUpdate();
                    }
                };
            Drawing.OnDraw += args =>
                {
                    foreach (var notificaiton in notifications)
                    {
                        notificaiton.OnDraw(Position);
                    }
                };
            Game.OnWndProc += args =>
                {
                    foreach (var notification in notifications)
                    {
                        notification.OnWndProc(new WindowsKeys(args));
                    }
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Notifications starting position.
        /// </summary>
        public static Vector2 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Attempts to add a notification to the list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Process success/failure</returns>
        public static bool AddNotification(NotificationBase notification)
        {
            if (notification != null && !NotificationsList.Contains(notification))
            {
                NotificationsList.Add(notification);
                UpdateNotifications();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempts to create an add a notification to the list.
        /// </summary>
        /// <param name="string">Display String</param>
        /// <param name="duration">The Duration</param>
        /// <param name="dispose">Auto Disposal</param>
        /// <returns>Process success/failure</returns>
        public static Notification AddNotification(string @string, int duration = -1, bool dispose = true)
        {
            var notification = new Notification(@string, duration, dispose);
            return AddNotification(notification) ? notification : null;
        }

        /// <summary>
        ///     Verifies the notification validation and checks if the notification is on the notifications list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Validates the notification and returns success or failure</returns>
        public static bool IsValid(this NotificationBase notification)
        {
            return notification != null && NotificationsList.Contains(notification);
        }

        /// <summary>
        ///     Attempts to remove the notification from the list.
        /// </summary>
        /// <param name="notification">Notifications instance</param>
        /// <returns>Process success/failure</returns>
        public static bool RemoveNotification(this NotificationBase notification)
        {
            if (NotificationsList.Contains(notification))
            {
                NotificationsList.Remove(notification);
                UpdateNotifications();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Updates all of the notifications runtime values.
        /// </summary>
        public static void UpdateNotifications()
        {
            if (!NotificationsList.Any())
            {
                return;
            }

            notifications = NotificationsList.ToArray();
            notifications[0].Position = Position;
            for (var i = 1; i < notifications.Length; ++i)
            {
                notifications[i].Position = new Vector2(
                    notifications[i - 1].Position.X, 
                    notifications[i - 1].Position.Y + notifications[i - 1].Height + 10f);
            }
        }

        #endregion
    }
}