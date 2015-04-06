using System;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Math;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    /// <summary>
    ///     Default Menu Render class.
    /// </summary>
    public class MenuRender
    {
        private static readonly Font Font = Constants.LeagueSharpFont;

        /// <summary>
        ///     Drawing Function of the Default Menu Skin.
        /// </summary>
        /// <param name="menuContainer">Menu Information Container</param>
        /// <param name="position">Starting Position</param>
        /// <param name="index">Menu Index</param>
        public static void OnDraw(MenuContainer menuContainer, Vector2 position, int index)
        {
            var centerY =
                DefaultSkin.GetContainerRectangle(position)
                    .GetCenteredText(null, menuContainer.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            #region Text Draw

            Font.DrawText(
                null, menuContainer.DisplayName, (int) (position.X + 15), (int) (centerY), DefaultSettings.TextColor);
            Font.DrawText(
                null, ">>",
                (int) (position.X + DefaultSettings.ContainerWidth - Font.MeasureText(null, ">>", 0).Width - 5f),
                (int) (centerY), DefaultSettings.TextColor);

            #endregion

            #region Components Draw

            if ((index + 1) != MenuInterface.RootMenuComponents.Count)
            {
                var line = new Line(Drawing.Direct3DDevice) { Antialias = true, GLLines = true, Width = 1f };

                line.Begin();
                line.Draw(
                    new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                        new Vector2(
                            position.X + DefaultSettings.ContainerWidth, position.Y + DefaultSettings.ContainerHeight)
                    },
                    Color.White);
                line.End();

                line.Dispose();
            }

            #endregion

            if (menuContainer.Toggled)
            {
                var line = new Line(Drawing.Direct3DDevice)
                {
                    Antialias = true,
                    GLLines = true,
                    Width = DefaultSettings.ContainerWidth
                };

                line.Begin();
                line.Draw(
                    new[]
                    {
                        new Vector2(position.X + DefaultSettings.ContainerWidth / 2, position.Y),
                        new Vector2(
                            position.X + DefaultSettings.ContainerWidth / 2,
                            position.Y + DefaultSettings.ContainerHeight)
                    }, new ColorBGRA(255, 255, 255, 255 / 2));
                line.End();

                line.Dispose();

                for (var i = 0; i < menuContainer.Components.Count; ++i)
                {
                    menuContainer.Components.Values.ToArray()[i].OnDraw(
                        new Vector2(
                            position.X + DefaultSettings.ContainerWidth + 5f,
                            position.Y + i * DefaultSettings.ContainerHeight), i);
                }
            }
        }
    }
}