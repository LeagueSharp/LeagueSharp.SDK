using System;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     Notification runtime flags.
    /// </summary>
    [Flags]
    public enum NotificationFlags : byte
    {
        /// <summary>
        ///     Drawing runtime flag.
        /// </summary>
        Draw = 1 << 0,

        /// <summary>
        ///     Windows Process Messages flag.
        /// </summary>
        WPM = Draw << 1,

        /// <summary>
        ///     Update flag.
        /// </summary>
        Update = WPM << 2
    }
}