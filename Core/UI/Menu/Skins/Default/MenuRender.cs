using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    /// <summary>
    ///     Default Menu Render class.
    /// </summary>
    public class MenuRender
    {
        private static readonly Font Font = DefaultSettings.Font;

        /// <summary>
        ///     Drawing Function of the Default Menu Skin.
        /// </summary>
        /// <param name="menuComponent">Menu</param>
        /// <param name="position">Starting Position</param>
        /// <param name="index">Menu Index</param>
        public static void OnDraw(AMenuComponent menuComponent, Vector2 position, int index)
        {
            #region Text Draw

            var centerY =
                (int)
                    (DefaultSkin.GetContainerRectangle(position)
                        .GetCenteredText(null, menuComponent.DisplayName, CenteredFlags.VerticalCenter)
                        .Y);

            Font.DrawText(
                null, menuComponent.DisplayName, (int) (position.X + DefaultSettings.ContainerTextOffset), centerY,
                DefaultSettings.TextColor);

            Font.DrawText(
                null, ">>",
                (int)
                    (position.X + DefaultSettings.ContainerWidth - DefaultSettings.ContainerTextMarkWidth +
                     DefaultSettings.ContainerTextMarkOffset), centerY, DefaultSettings.TextColor);

            #endregion

            #region Components Draw

            if ((index + 1) < MenuInterface.RootMenuComponents.Count)
            {
                DefaultSettings.ContainerSeperatorLine.Begin();
                DefaultSettings.ContainerSeperatorLine.Draw(
                    new[]
                    {
                        new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight),
                        new Vector2(
                            position.X + DefaultSettings.ContainerWidth, position.Y + DefaultSettings.ContainerHeight)
                    },
                    DefaultSettings.ContainerSeperatorColor);
                DefaultSettings.ContainerSeperatorLine.End();
            }

            #endregion

            if (menuComponent.Toggled)
            {
                #region Selection Mark

                DefaultSettings.ContainerLine.Begin();
                DefaultSettings.ContainerLine.Draw(
                    new[]
                    {
                        new Vector2(position.X + DefaultSettings.ContainerWidth / 2, position.Y),
                        new Vector2(
                            position.X + DefaultSettings.ContainerWidth / 2,
                            position.Y + DefaultSettings.ContainerHeight)
                    }, DefaultSettings.ContainerSelectedColor);
                DefaultSettings.ContainerLine.End();

                #endregion

                var height = DefaultSettings.ContainerHeight * menuComponent.Components.Count;
                var width = DefaultSettings.ContainerWidth;

                DefaultSettings.ContainerLine.Begin();
                DefaultSettings.ContainerLine.Draw(
                    new[]
                    {
                        new Vector2((position.X + DefaultSettings.ContainerWidth) + width / 2, position.Y),
                        new Vector2((position.X + DefaultSettings.ContainerWidth) + width / 2, position.Y + height)
                    },
                    DefaultSettings.RootContainerColor);
                DefaultSettings.ContainerLine.End();

                for (var i = 0; i < menuComponent.Components.Count; ++i)
                {
                    if (menuComponent.Components.Values.ToList()[i] != null)
                    {
                        menuComponent.Components.Values.ToList()[i].OnDraw(
                            new Vector2(
                                position.X + DefaultSettings.ContainerWidth,
                                position.Y + i * DefaultSettings.ContainerHeight), i);
                    }
                }
            }
        }
    }
}