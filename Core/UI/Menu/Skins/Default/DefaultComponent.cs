namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    /// Provides of series of methods used in the Default theme.
    /// </summary>
    public class DefaultComponent
    {
        

        /// <summary>
        ///     Gets the container rectangle.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        /// <returns>
        ///     <see cref="Rectangle" /> with information.
        /// </returns>
        protected static Rectangle GetContainerRectangle(AMenuComponent component)
        {
            return new Rectangle((int)component.Position.X, (int)component.Position.Y, component.MenuWidth, DefaultSettings.ContainerHeight);
        }


        /// <summary>
        ///     Calculates the width of text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The <see cref="int" /></returns>
        public int CalcWidthText(string text)
        {
            return DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, text, 0).Width;
        }
    }
}
