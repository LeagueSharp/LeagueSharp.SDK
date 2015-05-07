using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     Notification base abstract class.
    /// </summary>
    public abstract class NotificationBase : INotification
    {
        /// <summary>
        ///     Notification flags.
        /// </summary>
        public NotificationFlags Flags { get; set; }

        /// <summary>
        ///     Clone function which clones the notification information and creates a new one, however position and guid are
        ///     re-generated.
        /// </summary>
        /// <returns>Object in the type of <see cref="NotificationBase" /> class</returns>
        public abstract object Clone();

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
        /// <param name="keys">Converted windowskeys</param>
        public abstract void OnWndProc(WindowsKeys keys);

        /// <summary>
        ///     Sets the notification list position.
        /// </summary>
        /// <param name="position">New position</param>
        public abstract void SetPosition(int position);

        /// <summary>
        ///     Retrieves the notification list position.
        /// </summary>
        /// <returns>Notification list position</returns>
        public abstract int GetPosition();

        /// <summary>
        ///     Retrieves the global unique identification.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        public abstract string GetGuid();

        /// <summary>
        ///     Disposal by usercall.
        /// </summary>
        public abstract void Dispose();
    }
}