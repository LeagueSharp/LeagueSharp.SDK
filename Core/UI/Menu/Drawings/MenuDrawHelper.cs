using System;
using LeagueSharp.CommonEx.Core.UI.Values;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace LeagueSharp.CommonEx.Core.UI.Drawings
{
    /// <summary>
    ///     DrawHelper class used to Draw the Menu
    /// </summary>
    public static class MenuDrawHelper
    {
        /// <summary>
        ///     Font used for the Menutext
        /// </summary>
        public static Font Font;

        static MenuDrawHelper()
        {
            Font = new Font(
                LeagueSharp.Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 14,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Antialiased
                });

            LeagueSharp.Drawing.OnPreReset += Drawing_OnPreReset;
            LeagueSharp.Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            if (Font != null)
            {
                Font.Dispose();
                Font = null;
            }
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            Font.OnResetDevice();
        }

        private static void Drawing_OnPreReset(EventArgs args)
        {
            Font.OnLostDevice();
        }

        /// <summary>
        ///     Draws the Menu Box
        /// </summary>
        /// <param name="position">Position of the Box as Vector2</param>
        /// <param name="width">Width of the Box</param>
        /// <param name="height">Height of the Box</param>
        /// <param name="color">Color of the Box</param>
        /// <param name="borderwidth">Borderwidth of the Box</param>
        /// <param name="borderColor">Bordercolor of the Box</param>
        public static void DrawBox(Vector2 position, int width, int height, Color color, int borderwidth,
            Color borderColor)
        {
            LeagueSharp.Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, height, color);

            if (borderwidth > 0)
            {
                LeagueSharp.Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, borderwidth,
                    borderColor);
                LeagueSharp.Drawing.DrawLine(
                    position.X, position.Y + height, position.X + width, position.Y + height, borderwidth, borderColor);
                LeagueSharp.Drawing.DrawLine(position.X, position.Y, position.X, position.Y + height, borderwidth,
                    borderColor);
                LeagueSharp.Drawing.DrawLine(
                    position.X + width, position.Y, position.X + width, position.Y + height, borderwidth, borderColor);
            }
        }

        /// <summary>
        ///     Draws the Item Status
        /// </summary>
        /// <param name="on">Boolean if the Status is on</param>
        /// <param name="position">Position of the Status as Vector2</param>
        /// <param name="item">MenuItem of the Status</param>
        public static void DrawOnOff(bool on, Vector2 position, MenuItem item)
        {
            DrawBox(position, item.Height, item.Height, on ? Color.Green : Color.Red, 1, Color.Black);
            var s = on ? "On" : "Off";
            Font.DrawText(
                null, s,
                new Rectangle(
                    (int)(item.Position.X + item.Width - item.Height), (int)item.Position.Y, item.Height, item.Height),
                FontDrawFlags.VerticalCenter | FontDrawFlags.Center, new ColorBGRA(255, 255, 255, 255));
        }

        /// <summary>
        ///     Draws the Menu Arrow
        /// </summary>
        /// <param name="s">Text of the Arrow</param>
        /// <param name="position">Position of the Arrow as Vector2</param>
        /// <param name="item">MenuItem of the Slider</param>
        /// <param name="color">Color of the Slider</param>
        public static void DrawArrow(string s, Vector2 position, MenuItem item, Color color)
        {
            DrawBox(position, item.Height, item.Height, Color.Blue, 1, color);
            Font.DrawText(
                null, s, new Rectangle((int)(position.X), (int)item.Position.Y, item.Height, item.Height),
                FontDrawFlags.VerticalCenter | FontDrawFlags.Center, new ColorBGRA(255, 255, 255, 255));
        }

        /// <summary>
        ///     Draws the Menu Slider
        /// </summary>
        /// <param name="position">Position of the Slider</param>
        /// <param name="item">MenuItem of the Slider</param>
        /// <param name="width">Width of the Slider</param>
        /// <param name="drawText">Boolean if a Text should be drawn</param>
        public static void DrawSlider(Vector2 position, MenuItem item, int width = -1, bool drawText = true)
        {
            var val = item.GetValue<MenuSlider>();
            DrawSlider(position, item, val.MinValue, val.MaxValue, val.Value, width, drawText);
        }

        /// <summary>
        ///     Draws the Menu Slider
        /// </summary>
        /// <param name="position">Position of the Slider</param>
        /// <param name="item">MenuItem of the Slider</param>
        /// <param name="min">Min Value of the Slider</param>
        /// <param name="max">Max Value of the Slider</param>
        /// <param name="value">
        ///     Value of the Slider
        ///     <param name="width">Width of the Item</param>
        ///     <param name="drawText">Boolean if a Text should be drawn</param>
        public static void DrawSlider(Vector2 position, MenuItem item, int min, int max, int value, int width, bool drawText)
        {
            width = (width > 0 ? width : item.Width);
            var percentage = 100 * (value - min) / (max - min);
            var x = position.X + 3 + (percentage * (width - 3)) / 100;
            LeagueSharp.Drawing.DrawLine(x, position.Y + 2, x, position.Y + item.Height, 2, Color.Yellow);

            if (drawText)
            {
                Font.DrawText(
                    null, value.ToString(),
                    new Rectangle((int)position.X - 5, (int)position.Y, item.Width, item.Height),
                    FontDrawFlags.VerticalCenter | FontDrawFlags.Right, new ColorBGRA(255, 255, 255, 255));
            }
        }
    }
}