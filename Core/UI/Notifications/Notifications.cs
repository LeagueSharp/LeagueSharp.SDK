using System.Collections.Concurrent;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    /// </summary>
    public static class Notifications
    {
        /// <summary>
        ///     Safe-thread notifications list.
        /// </summary>
        private static readonly ConcurrentDictionary<string, INotification> NotificationsDictionary =
            new ConcurrentDictionary<string, INotification>();

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static Notifications()
        {
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
                    notificaiton.OnDraw();
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
        public static bool AddNotification(INotification notification)
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
        public static bool RemoveNotification(this INotification notification)
        {
            INotification dumpNotification;
            return NotificationsDictionary.TryRemove(notification.GetGuid(), out dumpNotification);
        }

        /// <summary>
        ///     Attempts to remove the notification from the list using the global unique identification.
        /// </summary>
        /// <param name="notificationGuid">global unique identification</param>
        /// <returns>Process success/failure</returns>
        public static bool RemoveNotification(this string notificationGuid)
        {
            INotification dumpNotification;
            return NotificationsDictionary.TryRemove(notificationGuid, out dumpNotification);
        }

        /// <summary>
        ///     Verifies the notification validation and checks if the notification is on the notifications list.
        /// </summary>
        /// <param name="notification">Notification instance</param>
        /// <returns>Validates the notification and returns success or failure</returns>
        public static bool IsValid(this INotification notification)
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