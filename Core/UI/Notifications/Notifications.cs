using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     Notifications class.
    /// </summary>
    public static class Notifications
    {
        /// <summary>
        ///     Notifications list.
        /// </summary>
        public static readonly List<NotificationBase> NotificationsList = new List<NotificationBase>();

        /// <summary>
        ///     Notifications array.
        /// </summary>
        private static NotificationBase[] _notifications;

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static Notifications()
        {
            Position = new Vector2(Drawing.Width - 250, 10);
            Game.OnUpdate += args =>
            {
                foreach (var notificaiton in _notifications)
                {
                    notificaiton.OnUpdate();
                }
            };
            Drawing.OnDraw += args =>
            {
                foreach (var notificaiton in _notifications)
                {
                    notificaiton.OnDraw(Position);
                }
            };
            Game.OnWndProc += args =>
            {
                foreach (var notification in _notifications)
                {
                    notification.OnWndProc(new WindowsKeys(args));
                }
            };
        }

        /// <summary>
        ///     Notifications starting position.
        /// </summary>
        public static Vector2 Position { get; set; }

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
        ///     Updates all of the notifications runtime values.
        /// </summary>
        public static void UpdateNotifications()
        {
            if (!NotificationsList.Any())
            {
                return;
            }

            _notifications = NotificationsList.ToArray();
            _notifications[0].Position = Position;
            for (var i = 1; i < _notifications.Length; ++i)
            {
                _notifications[i].Position = new Vector2(
                    _notifications[i - 1].Position.X,
                    _notifications[i - 1].Position.Y + _notifications[i - 1].Height + 10f);
            }
        }

        /// <summary>
        ///     Attempts to create an add a notification to the list.
        /// </summary>
        /// <param name="string">Display String</param>
        /// <param name="duration">Duration</param>
        /// <param name="dispose">Auto Disposal</param>
        /// <returns>Process success/failure</returns>
        public static Notification AddNotification(string @string, int duration = -1, bool dispose = true)
        {
            var notification = new Notification(@string, duration, dispose);
            return AddNotification(notification) ? notification : null;
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
        ///     Verifies the notification validation and checks if the notification is on the notifications list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Validates the notification and returns success or failure</returns>
        public static bool IsValid(this NotificationBase notification)
        {
            return notification != null && NotificationsList.Contains(notification);
        }
    }
}