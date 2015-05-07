using System;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     Notification Interface to comminucate between a notification type and the notifications manager.
    /// </summary>
    public interface INotification : IDisposable, ICloneable
    {
        /// <summary>
        ///     Send an drawing request to the callback.
        /// </summary>
        void OnDraw();

        /// <summary>
        ///     Send an update request to the callback.
        /// </summary>
        void OnUpdate();

        /// <summary>
        ///     Send an windows process message to the callback with formatted keys.
        /// </summary>
        /// <param name="keys">Formated Keys</param>
        void OnWndProc(WindowsKeys keys);

        /// <summary>
        ///     Send a new position to be placed to the notification.
        /// </summary>
        /// <param name="position">New position</param>
        void SetPosition(int position);

        /// <summary>
        ///     Send a notification position request to the notification type.
        /// </summary>
        /// <returns>Notification postion in list</returns>
        int GetPosition();

        /// <summary>
        ///     Sends a global unique identification request to the notification type.
        /// </summary>
        /// <returns></returns>
        string GetGuid();
    }
}