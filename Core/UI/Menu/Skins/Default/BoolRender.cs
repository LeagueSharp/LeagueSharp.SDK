using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Values;
using SharpDX;
using SharpDX.Direct3D9;

namespace LeagueSharp.CommonEx.Core.UI.Skins.Default
{
    /// <summary>
    ///     Default Bool Render class.
    /// </summary>
    public class BoolRender
    {
        /// <summary>
        ///     Drawing Function of the Default Boolean Skin.
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="position">Starting Position</param>
        /// <param name="index"></param>
        public static void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            #region Text Draw

            var centerY =
                (int)
                    (DefaultSkin.GetContainerRectangle(position)
                        .GetCenteredText(null, component.DisplayName, CenteredFlags.VerticalCenter)
                        .Y);

            DefaultSettings.Font.DrawText(
                null, component.DisplayName, (int) (position.X + DefaultSettings.ContainerTextOffset), centerY,
                DefaultSettings.TextColor);

            #endregion

            #region Components Draw

            if ((index + 1) < component.Parent.Components.Count)
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

            #region Boolean Box Draw

            var line = new Line(Drawing.Direct3DDevice)
            {
                Antialias = false,
                GLLines = true,
                Width = DefaultSettings.ContainerHeight
            };
            line.Begin();
            line.Draw(
                new[]
                {
                    new Vector2(
                        (position.X + DefaultSettings.ContainerWidth - DefaultSettings.ContainerHeight) +
                        DefaultSettings.ContainerHeight / 2, position.Y),
                    new Vector2(
                        (position.X + DefaultSettings.ContainerWidth - DefaultSettings.ContainerHeight) +
                        DefaultSettings.ContainerHeight / 2, position.Y + DefaultSettings.ContainerHeight)
                },
                ((MenuItem<MenuBool>) component).Value.Value
                    ? new ColorBGRA(0, 100, 0, 255)
                    : new ColorBGRA(255, 0, 0, 255));
            line.End();
            line.Dispose();

            #region Text Draw

            var centerX =
                (int)
                    (new Rectangle(
                        (int) (position.X + DefaultSettings.ContainerWidth - DefaultSettings.ContainerHeight),
                        (int) (position.Y), (int) DefaultSettings.ContainerHeight, (int) DefaultSettings.ContainerHeight)
                        .GetCenteredText(
                            null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF",
                            CenteredFlags.HorizontalCenter).X);
            DefaultSettings.Font.DrawText(
                null, ((MenuItem<MenuBool>) component).Value.Value ? "ON" : "OFF", centerX, centerY,
                DefaultSettings.TextColor);

            #endregion

            #endregion
        }

        /// <summary>
        ///     Returns the Boolean Container Rectangle Boundaries.
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>Returns the Boolean Container Rectangle Boundaries</returns>
        public static Rectangle GetBooleanContainerRectangle(Vector2 position)
        {
            return new Rectangle(
                (int) (position.X + DefaultSettings.ContainerWidth - DefaultSettings.ContainerHeight), (int) position.Y,
                (int) DefaultSettings.ContainerHeight, (int) DefaultSettings.ContainerHeight);
        }
    }
}