using System.Collections.Concurrent;
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
        ///     Safe-thread notifications list.
        /// </summary>
        public static readonly ConcurrentDictionary<string, NotificationBase> NotificationsDictionary =
            new ConcurrentDictionary<string, NotificationBase>();

        /// <summary>
        ///     Notifications starting position.
        /// </summary>
        public static Vector2 Position { get; set; }

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static Notifications()
        {
            Position = new Vector2(Drawing.Width - 250, 10);
            Game.OnUpdate += args =>
            {
                foreach (var notificaiton in NotificationsDictionary.Values)
                {
                    notificaiton.OnUpdate();
                }
            };
            Drawing.OnDraw += args =>
            {
                foreach (var notificaiton in NotificationsDictionary.Values)
                {
                    notificaiton.OnDraw(Position);
                }
            };
            Game.OnWndProc += args =>
            {
                foreach (var notification in NotificationsDictionary.Values)
                {
                    notification.OnWndProc(new WindowsKeys(args));
                }
            };
        }

        /// <summary>
        ///     Attempts to add a notification to the list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Process success/failure</returns>
        public static bool AddNotification(NotificationBase notification)
        {
            return (notification != null) && !NotificationsDictionary.ContainsKey(notification.GetGuid()) &&
                   NotificationsDictionary.TryAdd(notification.GetGuid(), notification);
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
            NotificationsDictionary.TryAdd(notification.GetGuid(), notification);
            return notification;
        }

        /// <summary>
        ///     Attempts to remove the notification from the list.
        /// </summary>
        /// <param name="notification">Notifications instance</param>
        /// <returns>Process success/failure</returns>
        public static bool RemoveNotification(this NotificationBase notification)
        {
            NotificationBase dumpNotification;
            return NotificationsDictionary.TryRemove(notification.GetGuid(), out dumpNotification);
        }

        /// <summary>
        ///     Attempts to remove the notification from the list using the global unique identification.
        /// </summary>
        /// <param name="notificationGuid">global unique identification</param>
        /// <returns>Process success/failure</returns>
        public static bool RemoveNotification(this string notificationGuid)
        {
            NotificationBase dumpNotification;
            return NotificationsDictionary.TryRemove(notificationGuid, out dumpNotification);
        }

        /// <summary>
        ///     Verifies the notification validation and checks if the notification is on the notifications list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Validates the notification and returns success or failure</returns>
        public static bool IsValid(this NotificationBase notification)
        {
            return notification != null && NotificationsDictionary.ContainsKey(notification.GetGuid());
        }

        /// <summary>
        ///     Verifies the notification validation and checks if the notification is on the notifications list through the global
        ///     unique identification.
        /// </summary>
        /// <param name="notificationGuid">global unique identification</param>
        /// <returns>Validates the notification and returns success or failure</returns>
        public static bool IsValidNotification(this string notificationGuid)
        {
            return NotificationsDictionary.ContainsKey(notificationGuid);
        }
    }
}