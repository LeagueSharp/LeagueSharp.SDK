namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    /// Defines how to draw a keybind.
    /// </summary>
    public interface IDrawableKeyBind
    {
        /// <summary>
        /// Draws a MenuKeyBind
        /// </summary>
        /// <param name="component">MenuKeyBind</param>
        void Draw(MenuKeyBind component);

        /// <summary>
        /// Gets the On/Off boundaries
        /// </summary>
        /// <param name="component">MenuKeyBind</param>
        /// <returns>Rectangle</returns>
        Rectangle ButtonBoundaries(MenuKeyBind component);

        /// <summary>
        /// Gets the keybind boundaries
        /// </summary>
        /// <param name="component">MenuKeyBind</param>
        /// <returns>Rectangle</returns>
        Rectangle KeyBindBoundaries(MenuKeyBind component);

        /// <summary>
        /// Gets the width of the MenuKeyBind
        /// </summary>
        /// <param name="keyBind">MenuKeyBind</param>
        /// <returns>int</returns>
        int Width(MenuKeyBind keyBind);
    }
}