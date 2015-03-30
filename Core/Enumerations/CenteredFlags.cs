using System;

namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     CenteredText Drawing Flags
    /// </summary>
    [Flags]
    public enum CenteredFlags
    {
        /// <summary>
        ///     None Flag
        /// </summary>
        None = 0,

        /// <summary>
        ///     Center Horizontally Left.
        /// </summary>
        HorizontalLeft = 1 << 0,

        /// <summary>
        ///     Center Horizontally.
        /// </summary>
        HorizontalCenter = 1 << 1,

        /// <summary>
        ///     Center Horizontally Right.
        /// </summary>
        HorizontalRight = 1 << 2,

        /// <summary>
        ///     Center Vertically Up.
        /// </summary>
        VerticalUp = 1 << 3,

        /// <summary>
        ///     Center Vertically.
        /// </summary>
        VerticalCenter = 1 << 4,

        /// <summary>
        ///     Center Vertically Down.
        /// </summary>
        VerticalDown = 1 << 5
    }
}