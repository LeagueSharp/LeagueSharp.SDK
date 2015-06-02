namespace LeagueSharp.SDK.Core.UI.Skins
{
    using LeagueSharp.SDK.Core.UI.Values;

    /// <summary>
    /// Defines how to draw a separator.
    /// </summary>
    public interface IDrawableSeparator
    {
        /// <summary>
        /// Draw a MenuSeparator
        /// </summary>
        /// <param name="component">MenuSeparator</param>
        void Draw(MenuSeparator component);
    }
}