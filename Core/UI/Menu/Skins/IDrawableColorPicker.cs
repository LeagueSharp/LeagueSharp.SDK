namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    ///     Defines how to draw a ColorPicker
    /// </summary>
    public interface IDrawableColorPicker
    {
        /// <summary>
        ///     Get the preview boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle PreviewBoundaries(MenuColor component);

        /// <summary>
        ///     Get the picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle PickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the red picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle RedPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the green picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle GreenPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the blue picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle BluePickerBoundaries(MenuColor component);

        /// <summary>
        ///     Get the alpha picker boundaries
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>Rectangle</returns>
        Rectangle AlphaPickerBoundaries(MenuColor component);

        /// <summary>
        ///     Draws a MenuColor
        /// </summary>
        /// <param name="component">MenuColor</param>
        void Draw(MenuColor component);

        /// <summary>
        ///     Gets the width of a MenuCOlor
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>int</returns>
        int Width(MenuColor component);

        /// <summary>
        ///     Gets the width of the slider
        /// </summary>
        /// <param name="component">MenuColor</param>
        /// <returns>int</returns>
        int SliderWidth(MenuColor component);
    }
}