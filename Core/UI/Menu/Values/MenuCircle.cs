using System;
using System.Drawing;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    /// <summary>
    ///     Menu Circle
    /// </summary>
    [Serializable]
    public struct MenuCircle
    {
        /// <summary>
        ///     Boolean if the Circle is active
        /// </summary>
        public bool Active;

        /// <summary>
        ///     Color of the Circle
        /// </summary>
        public Color Color;

        /// <summary>
        ///     Radius of the Circle
        /// </summary>
        public float Radius;

        /// <summary>
        ///     Creates the Circle
        /// </summary>
        /// <param name="enabled">Boolean if the Circle is active</param>
        /// <param name="color">Color of the Circle</param>
        /// <param name="radius">Radius of the Circle</param>
        public MenuCircle(bool enabled, Color color, float radius = 100)
        {
            Active = enabled;
            Color = color;
            Radius = radius;
        }
    }
}