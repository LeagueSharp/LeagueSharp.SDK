using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of a <see cref="ADrawable{MenuBool}"/>
    /// </summary>
    public class DefaultBool : ADrawable<MenuBool>
    {
        /// <summary>
        /// Creates a default handler responsible for <see cref="MenuBool"/>.
        /// </summary>
        /// <param name="component"></param>
        public DefaultBool(MenuBool component)
            : base(component) {}

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the Rectangle that defines the on/off button
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuBool component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)component.Position.Y,
                    MenuSettings.ContainerHeight,
                    MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Draws a <see cref="MenuBool" />
        /// </summary>
        public override void Draw()
        {
            var centerY =
                (int)
                DefaultUtilities.GetContainerRectangle(Component)
                    .GetCenteredText(null, MenuSettings.Font, Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.DisplayName,
                (int)(Component.Position.X + MenuSettings.ContainerTextOffset),
                centerY,
                MenuSettings.TextColor);

            var line = new Line(Drawing.Direct3DDevice)
            {
                Antialias = false,
                GLLines = true,
                Width = MenuSettings.ContainerHeight
            };
            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            (Component.Position.X + Component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            Component.Position.Y + 1), 
                        new Vector2(
                            (Component.Position.X + Component.MenuWidth
                             - MenuSettings.ContainerHeight) + MenuSettings.ContainerHeight / 2f, 
                            Component.Position.Y + MenuSettings.ContainerHeight)
                    },
                Component.Value ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
            line.End();
            line.Dispose();

            var centerX =
                (int)
                new Rectangle(
                    (int)
                    (Component.Position.X + Component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)Component.Position.Y,
                    MenuSettings.ContainerHeight,
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null, MenuSettings.Font,
                        Component.Value ? "ON" : "OFF",
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                Component.Value ? "ON" : "OFF",
                centerX,
                centerY,
                MenuSettings.TextColor);
        }

        #endregion

        /// <summary>
        /// Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!Component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                var rect = ButtonBoundaries(Component);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    Component.Value = !Component.Value;
                    Component.FireEvent();
                }
            }
        }

        /// <summary>
        /// Calculates the Width of a MenuBool
        /// </summary>
        /// <returns>width</returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(Component) + MenuSettings.ContainerHeight;
        }

        /// <summary>
        /// Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            //do nothing
        }
    }
}
