using System;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

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
        void OnDraw(Vector2 parentPosition);

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
        ///     Sends a global unique identification request to the notification type.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        string GetGuid();
    }
}