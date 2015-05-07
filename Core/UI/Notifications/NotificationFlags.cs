using System;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     Notification runtime flags.
    /// </summary>
    [Flags]
    public enum NotificationFlags
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
        Update = WPM << 2,

        /// <summary>
        ///     Initalization flag.
        /// </summary>
        Initalized = Update << 3,

        /// <summary>
        ///     Idle flag.
        /// </summary>
        Idle = Initalized << 4,

        /// <summary>
        ///     Animation flag.
        /// </summary>
        Animation = Idle << 5
    }
}