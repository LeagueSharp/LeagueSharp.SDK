namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;

    /// <summary>
    /// Implements IDrawableSeperator as a default skin.
    /// </summary>
    public class DefaultSeparator : DefaultComponent, IDrawableSeparator
    {
        /// <summary>
        /// Draw a MenuSeparator
        /// </summary>
        /// <param name="component">MenuSeparator</param>
        public void Draw(MenuSeparator component)
        {
            Vector2 centerY = GetContainerRectangle(component.Container)
                .GetCenteredText(
                    null,
                    component.Container.DisplayName,
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Container.DisplayName,
                (int)centerY.X,
                (int)centerY.Y,
                DefaultSettings.TextColor);
        }
    }
}