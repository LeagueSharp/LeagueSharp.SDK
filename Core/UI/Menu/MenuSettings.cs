using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Settings
    /// </summary>
    public class MenuSettings
    {
        /// <summary>
        ///     Static Constructor
        /// </summary>
        static MenuSettings()
        {
            Position = new Vector2(20, 20);
        }

        /// <summary>
        ///     Menu Position
        /// </summary>
        public static Vector2 Position { get; set; }
    }
}