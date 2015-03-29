#region

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Utils;
using LeagueSharp.CommonEx.Properties;
using SharpDX;
using SharpDX.Direct3D9;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Root of the User Interface.
    /// </summary>
    public class Root
    {
        private static bool Show;

        static Root()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            var keys = new WindowsKeys(args);
            if (keys.SingleKey == Keys.ShiftKey)
            {
                Show = keys.Msg == WindowsMessages.KEYDOWN;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Show)
            {
                Menu.OnDraw();
            }
        }

        private static void Game_OnUpdate(EventArgs args) {}

        public static void Init()
        {
            Menu.LoadBitmap(Menu.BitmapType.LogoHeader, Resources.skin0_img000);
            Menu.LoadBitmap(Menu.BitmapType.LogoIcon, Resources.skin0_img001);
            Menu.LoadBitmap(Menu.BitmapType.AssembliesIcon, Resources.skin0_img002);
            Menu.LoadBitmap(Menu.BitmapType.SettingsIcon, Resources.skin0_img003);
            Menu.LoadBitmap(Menu.BitmapType.LogoFooter, Resources.skin0_img004);
        }

        #region Menu

        private static class Menu
        {
            /// <summary>
            ///     Menu Sprite
            /// </summary>
            private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

            private static Bitmap LogoBitmap { get; set; }
            private static Texture LogoTexture { get; set; }
            private static float Rotation { set; get; }
            private static Vector2 Scale { get { return new Vector2(1,1); } }

            /// <summary>
            ///     Menu Draw callback
            /// </summary>
            public static void OnDraw()
            {
                Sprite.Begin(SpriteFlags.AlphaBlend);

                SubBox.Begin();
                SubBox.Draw(VerticesSubBox, new ColorBGRA(224, 255, 255, 255 / 2));
                SubBox.End();

                MainBox.Begin();
                MainBox.Draw(VerticesMainBox, new ColorBGRA(0, 0, 0, (byte) (255 / 1.7f)));
                MainBox.End();

                var line = new Line(Drawing.Direct3DDevice) { Antialias = true, GLLines = true, Width = 1f };
                if (LogoBitmap != null && LogoTexture != null)
                {
                    var matrix = Sprite.Transform;
                    var nMatrix = (Matrix.Scaling(Scale.X, Scale.Y, 0)) * Matrix.RotationZ(Rotation) *
                                  Matrix.Translation(Position.X+15f, Position.Y+5f, 0);
                    Sprite.Transform = nMatrix;
                    Sprite.Draw(LogoTexture, new ColorBGRA(255, 255, 255, 255));
                    Sprite.Transform = matrix;

                    line.Begin();
                    line.Draw(
                        new[]
                        {
                            new Vector2(Position.X, Position.Y + LogoBitmap.Height + 10f),
                            new Vector2(Position.X + MainBox.Width + 1f, Position.Y + LogoBitmap.Height + 10f),
                        },
                        new ColorBGRA(255, 255, 255, 255));
                    line.End();
                }

                line = new Line(Drawing.Direct3DDevice) { Antialias = true, GLLines = true, Width = 1f };
                line.Begin();
                line.Draw(
                    new[]
                    {
                        new Vector2(Position.X, Position.Y + Drawing.Height - 200),
                        new Vector2(Position.X + MainBox.Width + 1f, Position.Y + Drawing.Height - 200),
                    },
                    new ColorBGRA(255, 255, 255, 255));
                line.End();
                line.Dispose();

                Constants.LeagueSharpFont.DrawText(
                    Sprite, "\"Accounts are like girls, they come, steal your money and go.\" -iMeh",
                    35, Drawing.Height-190, new ColorBGRA(255, 255, 255, 255));

                Sprite.End();
            }

            /// <summary>
            ///     Menu SubBox Line
            /// </summary>
            private static readonly Line SubBox = new Line(Drawing.Direct3DDevice)
            {
                Antialias = true,
                GLLines = true,
                Width = 8f
            };

            /// <summary>
            ///     Menu MainBox Line
            /// </summary>
            private static readonly Line MainBox = new Line(Drawing.Direct3DDevice)
            {
                Antialias = true,
                GLLines = true,
                Width = 455f
            };

            /// <summary>
            ///     Menu Position
            /// </summary>
            private static readonly Vector2 Position = new Vector2(0, 0);

            /// <summary>
            ///     Sub Box Vertices
            /// </summary>
            private static readonly Vector2[] VerticesSubBox =
            {
                new Vector2(Position.X + (Position.X + MainBox.Width) + SubBox.Width / 2, Position.Y),
                new Vector2(Position.X + (Position.X + MainBox.Width) + SubBox.Width / 2, Position.Y + Drawing.Height)
            };

            /// <summary>
            ///     Main Box Vertices
            /// </summary>
            private static readonly Vector2[] VerticesMainBox =
            {
                new Vector2(Position.X + MainBox.Width / 2, Position.Y),
                new Vector2(Position.X + MainBox.Width / 2, Position.Y + Drawing.Height)
            };

            public static void LoadBitmap(BitmapType bitmapType, Bitmap bitmap)
            {
                switch (bitmapType)
                {
                    case BitmapType.LogoHeader:
                        if (LogoBitmap != null)
                        {
                            LogoBitmap.Dispose();
                            LogoBitmap = null;
                        }

                        LogoBitmap = bitmap;
                        LogoTexture = Texture.FromMemory(
                            Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(LogoBitmap, typeof(byte[])),
                            (int) (LogoBitmap.Width * Scale.X), (int) (LogoBitmap.Height * Scale.Y), 0, Usage.None,
                            Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);

                        break;
                }
            }

            public enum BitmapType
            {
                LogoHeader,
                LogoIcon,
                AssembliesIcon,
                SettingsIcon,
                LogoFooter
            }
        }

        #endregion
    }
}