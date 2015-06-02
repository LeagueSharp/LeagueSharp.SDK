using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using System.Globalization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// A default implementation of an IDrawableSlider
    /// </summary>
    public class DefaultSlider : DefaultComponent, IDrawableSlider
    {
        /// <summary>
        /// Gets the additional boundaries.
        /// </summary>
        /// <param name="component">MenuSlider</param>
        /// <returns>Rectangle</returns>
        public Rectangle AdditionalBoundries(MenuSlider component)
        {
            return GetContainerRectangle(component.Container);
        }

        /// <summary>
        /// Gets the boundaries
        /// </summary>
        /// <param name="component">MenuSlider</param>
        /// <returns>Rectangle</returns>
        public Rectangle Bounding(MenuSlider component)
        {
            return GetContainerRectangle(component.Container);
        }

        /// <summary>
        /// Draws a MenuSlider
        /// </summary>
        /// <param name="component">MenuSlider</param>
        public void Draw(MenuSlider component)
        {
            var position = component.Container.Position;
            var centeredY =
                                       (int)
                                       GetContainerRectangle(component.Container)
                                           .GetCenteredText(null, component.Container.DisplayName, CenteredFlags.VerticalCenter)
                                           .Y;
            var percent = (component.Value - component.MinValue) / (float)(component.MaxValue - component.MinValue);
            var x = position.X + (percent * component.Container.MenuWidth);

            var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = 2 };
            line.Begin();
            line.Draw(
                new[]
                                           {
                                               new Vector2(x, position.Y + 1),
                                               new Vector2(x, position.Y + DefaultSettings.ContainerHeight)
                                           },
                component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
            line.End();

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Container.DisplayName,
                (int)(position.X + DefaultSettings.ContainerTextOffset),
                centeredY,
                DefaultSettings.TextColor);

            var measureText = DefaultSettings.Font.MeasureText(
                null,
                component.Value.ToString(CultureInfo.InvariantCulture),
                0);
            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Value.ToString(CultureInfo.InvariantCulture),
                (int)(position.X + component.Container.MenuWidth - 5 - measureText.Width),
                centeredY,
                DefaultSettings.TextColor);

            line.Width = DefaultSettings.ContainerHeight;
            line.Begin();
            line.Draw(
                new[]
                                           {
                                               new Vector2(position.X, position.Y + DefaultSettings.ContainerHeight / 2f),
                                               new Vector2(x, position.Y + DefaultSettings.ContainerHeight / 2f)
                                           },
                DefaultSettings.HoverColor);
            line.End();
            line.Dispose();
        }
    }
}
