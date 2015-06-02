using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// A default implementation of IDrawableButton
    /// </summary>
    public class DefaultButton : DefaultComponent, IDrawableButton
    {
        const int TextGap = 5;
        private readonly ColorBGRA buttonColor = new ColorBGRA(100, 100, 100, 255);
        private readonly ColorBGRA buttonHoverColor = new ColorBGRA(170, 170, 170, 200);

        /// <summary>
        ///     Calculate the Rectangle that defines the Button
        /// </summary>
        /// <param name="component">MenuButton</param>
        /// <returns>Rectangle</returns>
        public Rectangle ButtonBoundaries(MenuButton component)
        {
            var buttonTextWidth =
                DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0)
                    .Width;
            return
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth
                     - (2 * TextGap)),
                    (int)component.Container.Position.Y,
                    (2 * TextGap) + buttonTextWidth,
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a MenuButton
        /// </summary>
        /// <param name="component">MenuButton</param>
        public void Draw(MenuButton component)
        {
            var rectangleName = GetContainerRectangle(component.Container)
                                       .GetCenteredText(null, component.Container.DisplayName, CenteredFlags.VerticalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Container.DisplayName,
                (int)(component.Container.Position.X + DefaultSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                DefaultSettings.TextColor);

            var buttonTextWidth = DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, component.ButtonText, 0).Width;

            var line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true, Width = DefaultSettings.ContainerHeight };
            line.Begin();
            line.Draw(
                new[]
                                           {
                                               new Vector2(
                                                   component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth - (2 * TextGap),
                                                   component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)),
                                               new Vector2(
                                                   component.Container.Position.X + component.Container.MenuWidth,
                                                   component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)),
                                           },
                DefaultSettings.HoverColor);
            line.End();
            line.Width = DefaultSettings.ContainerHeight - 5;
            line.Begin();
            line.Draw(
                new[]
                                           {
                                               new Vector2(
                                                   component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth - (2 * TextGap) + 2,
                                                   component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)),
                                               new Vector2(
                                                   component.Container.Position.X + component.Container.MenuWidth - 2,
                                                   component.Container.Position.Y + (DefaultSettings.ContainerHeight / 2f)),
                                           },
                component.Hovering ? buttonHoverColor : buttonColor);
            line.End();
            line.Dispose();

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.ButtonText,
                (int)(component.Container.Position.X + component.Container.MenuWidth - buttonTextWidth - TextGap),
                (int)rectangleName.Y,
                DefaultSettings.TextColor);
        }

        /// <summary>
        ///     Gets the width of the MenuButton
        /// </summary>
        public int Width(MenuButton menuButton)
        {
            return (2 * TextGap)
                   + DefaultSettings.Font.MeasureText(MenuManager.Instance.Sprite, menuButton.ButtonText, 0).Width;
        }
    }
}
