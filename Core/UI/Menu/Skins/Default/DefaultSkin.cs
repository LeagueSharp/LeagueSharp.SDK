using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    /// <summary>
    ///     Default skin.
    /// </summary>
    public class DefaultSkin
    {
        /// <summary>
        ///     Drawing Function of the Default Skin.
        /// </summary>
        /// <param name="position">Starting Position</param>
        public static void OnDraw(Vector2 position)
        {
            var height = DefaultSettings.ContainerHeight * MenuInterface.RootMenuComponents.Count;
            var width = DefaultSettings.ContainerWidth;

            DefaultSettings.ContainerLine.Begin();
            DefaultSettings.ContainerLine.Draw(
                new[]
                {
                    new Vector2(position.X + width / 2, position.Y),
                    new Vector2(position.X + width / 2, position.Y + height)
                }, DefaultSettings.RootContainerColor);
            DefaultSettings.ContainerLine.End();

            for (var i = 0; i < MenuInterface.RootMenuComponents.Count; ++i)
            {
                MenuInterface.RootMenuComponents[i].OnDraw(
                    new Vector2(position.X, position.Y + i * DefaultSettings.ContainerHeight), i);
            }
        }

        /// <summary>
        ///     Returns the container rectangle.
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <returns>Container Rectangle</returns>
        public static Rectangle GetContainerRectangle(Vector2 position)
        {
            return new Rectangle(
                (int) position.X, (int) position.Y, (int) DefaultSettings.ContainerWidth,
                (int) DefaultSettings.ContainerHeight);
        }
    }
}