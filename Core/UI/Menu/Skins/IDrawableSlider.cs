namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    /// Defines how to draw a slider
    /// </summary>
    public interface IDrawableSlider
    {
        /// <summary>
        /// Gets the additional boundaries.
        /// </summary>
        /// <param name="component">MenuSlider</param>
        /// <returns>Rectangle</returns>
        Rectangle AdditionalBoundries(MenuSlider component);

        
        /// <summary>
        /// Gets the boundaries
        /// </summary>
        /// <param name="component">MenuSlider</param>
        /// <returns>Rectangle</returns>
        Rectangle Bounding(MenuSlider component);

        
        /// <summary>
        /// Draws a MenuSlider
        /// </summary>
        /// <param name="component">MenuSlider</param>
        void Draw(MenuSlider component);
    }
}