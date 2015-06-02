namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a On/Off button
    /// </summary>
    public interface IDrawableBool
    {
        /// <summary>
        ///     Draws a MenuBool
        /// </summary>
        /// <param name="component">MenuBool</param>
        void Draw(MenuBool component);

        /// <summary>
        ///     Calculates the On/Off button Rectangle
        /// </summary>
        /// <param name="component">MenuBool</param>
        /// <returns>Rectangle</returns>
        Rectangle ButtonBoundaries(MenuBool component);
    }
}