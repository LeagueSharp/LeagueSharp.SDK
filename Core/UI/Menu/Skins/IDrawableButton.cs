namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a Button
    /// </summary>
    public interface IDrawableButton
    {
        /// <summary>
        ///     Calculate the Rectangle that defines the Button
        /// </summary>
        /// <param name="component">MenuButton</param>
        /// <returns>Rectangle</returns>
        Rectangle ButtonBoundaries(MenuButton component);

        /// <summary>
        ///     Draws a MenuButton
        /// </summary>
        /// <param name="component">MenuButton</param>
        void Draw(MenuButton component);

        /// <summary>
        ///     Gets the width of the MenuButton
        /// </summary>
        int Width(MenuButton component);
    }
}