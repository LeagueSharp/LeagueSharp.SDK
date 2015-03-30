#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;
using SharpDX.Direct3D9;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public static class MenuInterface
    {
        /// <summary>
        ///     Color of the border of the main box.
        /// </summary>
        public static ColorBGRA BorderBoxColor = new ColorBGRA(255, 255, 255, 255 / 10);

        /// <summary>
        ///     Color of the main box.
        /// </summary>
        public static ColorBGRA MainBoxColor = new ColorBGRA(0, 0, 0, 255 / 2);

        /// <summary>
        ///     Static Constructor.
        /// </summary>
        static MenuInterface()
        {
            MenuSkins.Load(0, Sprite, Position, out Sprites);
        }

        /// <summary>
        ///     Menu Position.
        /// </summary>
        public static Vector2 Position { get; set; }

        /// <summary>
        ///     Menu Draw Callback.
        /// </summary>
        public static void OnDraw()
        {
            Sprite.Begin(SpriteFlags.AlphaBlend);

            #region Draw Border/Menu

            MenuBorder.Begin();
            MenuBorder.Draw(VerticesBorderBox, BorderBoxColor);
            MenuBorder.End();

            MainMenu.Begin();
            MainMenu.Draw(VerticesMainBox, MainBoxColor);
            MainMenu.End();

            #endregion

            foreach (var sprite in Sprites.Where(s => !s.Key.Contains("Special")))
            {
                sprite.Value.Draw(Sprite);
            }

            #region Draw Seperate Lines

            if (Sprites.ContainsKey("Header_Logo"))
            {
                Line.Begin();
                Line.Draw(MenuSkins.GetSeperatorVertices(Sprites["Header_Logo"]), Color.White);
                Line.End();
            }
            if (Sprites.ContainsKey("Footer_Logo"))
            {
                var position = MenuSkins.GetSeperatorVertices(Sprites["Footer_Logo"], true, true, 0f);
                Line.Begin();
                Line.Draw(position, Color.White);
                Line.End();

                QuoteContainer.Draw(position);
            }

            #endregion

            Sprite.End();
        }

        /// <summary>
        ///     Menu Windows Process Messages Callback.
        /// </summary>
        /// <param name="keys">Windows Keys</param>
        public static void OnWndProc(WindowsKeys keys)
        {
            Sprites.Any(s => s.Value.TryGlow(keys.Cursor));
        }

        #region Quote

        /// <summary>
        ///     Quote Container.
        /// </summary>
        public static class QuoteContainer
        {
            /// <summary>
            ///     Quotes.
            /// </summary>
            private static readonly string[] Quotes =
            {
                "\"There are no strangers here; Only friends you haven't yet met.\"",
                "\"Cheater's just a fancy word for winner.\"", "\"Never lost a fair game... or played one.\"",
                "\"Hola mi nombre es Degrec and I use cracked scripts.\"", "\"auth tomorrow.\"",
                "\"Accounts are like girls, they come, steal your money and go.\"",
                "\"2000 dollar as btc and still not down.\"",
                "\"@iMeh I tried to open your image in Photoshop and it deinstalled itself.\"",
                "\"I'm not doing the strip tease anymore it keeps getting leaked\""
            };

            /// <summary>
            ///     Quote Authors.
            /// </summary>
            private static readonly string[] QuotesAuthors =
            {
                "William Butler Yeats", "Twisted Fate", "Twisted Fate",
                "Degrec", "Joduskame 2013", "iMeh", "{F3VD2VG}", "superei", "Artud"
            };

            /// <summary>
            ///     Random Pointer.
            /// </summary>
            private static readonly Random Random = new Random();

            /// <summary>
            ///     Current Quote.
            /// </summary>
            public static string Quote { get; set; }

            /// <summary>
            ///     Current Author.
            /// </summary>
            public static string Author { get; set; }

            /// <summary>
            ///     Selects randomly a quote.
            /// </summary>
            public static void Next()
            {
                var index = Random.Next(0, Quotes.Length);

                Quote = Quotes[index];
                Author = (index >= QuotesAuthors.Length) ? " - ???" : " - " + QuotesAuthors[index];
            }

            /// <summary>
            ///     Draws the quote onto the screen.
            /// </summary>
            /// <param name="position">Position</param>
            public static void Draw(Vector2[] position)
            {
                if (!Author.Contains("{"))
                {
                    var x =
                        new Rectangle((int) Position.X, (int) Position.Y, (int) MainMenu.Width, Drawing.Height)
                            .GetCenter(
                                Sprite, Constants.LeagueSharpFont.MeasureText(Sprite, Quote, 0),
                                CenteredFlags.HorizontalCenter).X;
                    Constants.LeagueSharpFont.DrawText(
                        Sprite, Quote, (int) x, (int) (position[0].Y - 40), new ColorBGRA(255, 255, 255, 255));


                    x =
                        new Rectangle((int) Position.X, (int) Position.Y, (int) MainMenu.Width, Drawing.Height)
                            .GetCenter(
                                Sprite, Constants.LeagueSharpFont.MeasureText(Sprite, Author, 0),
                                CenteredFlags.HorizontalCenter).X;
                    Constants.LeagueSharpFont.DrawText(
                        Sprite, Author, (int) x, (int) (position[0].Y - 20), new ColorBGRA(255, 255, 255, 255));
                }
                else
                {
                    var drawSprite =
                        Sprites.Find(e => e.Key.Equals("Special_" + Author.Substring(4, Author.Length - 5))).Value;
                    if (drawSprite != null)
                    {
                        var x =
                            new Rectangle((int) Position.X, (int) Position.Y, (int) MainMenu.Width, Drawing.Height)
                                .GetCenter(
                                    Sprite, Constants.LeagueSharpFont.MeasureText(Sprite, Quote, 0),
                                    CenteredFlags.HorizontalCenter).X;
                        Constants.LeagueSharpFont.DrawText(
                            Sprite, Quote, (int) x, (int) (position[0].Y - drawSprite.Height - 35),
                            new ColorBGRA(255, 255, 255, 255));

                        x =
                            new Rectangle((int) Position.X, (int) Position.Y, (int) MainMenu.Width, Drawing.Height)
                                .GetCenter(
                                    Sprite, Constants.LeagueSharpFont.MeasureText(Sprite, Author, 0),
                                    CenteredFlags.HorizontalCenter).X;
                        Constants.LeagueSharpFont.DrawText(
                            Sprite, " - ", (int) x, (int) (position[0].Y - drawSprite.Height),
                            new ColorBGRA(255, 255, 255, 255));

                        drawSprite.Draw(Sprite, (int) position[0].Y - drawSprite.Height - 15);
                    }
                }
            }
        }

        #endregion

        #region Sprites

        /// <summary>
        ///     Sprites
        /// </summary>
        private static readonly IDictionary<string, MenuSprite> Sprites;

        /// <summary>
        ///     Sprite
        /// </summary>
        private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

        #endregion

        #region Draw Border/Menu Settings

        /// <summary>
        ///     General line for drawing.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice)
        {
            Antialias = true,
            GLLines = true,
            Width = 1f
        };

        /// <summary>
        ///     Menu Border Line
        /// </summary>
        private static readonly Line MenuBorder = new Line(Drawing.Direct3DDevice)
        {
            Antialias = true,
            GLLines = true,
            Width = 10f
        };

        /// <summary>
        ///     Main Menu Line
        /// </summary>
        public static readonly Line MainMenu = new Line(Drawing.Direct3DDevice)
        {
            Antialias = true,
            GLLines = true,
            Width = 421f
        };

        /// <summary>
        ///     Border Box Vertices
        /// </summary>
        private static readonly Vector2[] VerticesBorderBox =
        {
            new Vector2(Position.X + (Position.X + MainMenu.Width) + MenuBorder.Width / 2, Position.Y),
            new Vector2(Position.X + (Position.X + MainMenu.Width) + MenuBorder.Width / 2, Position.Y + Drawing.Height)
        };

        /// <summary>
        ///     Main Menu Vertices
        /// </summary>
        private static readonly Vector2[] VerticesMainBox =
        {
            new Vector2(Position.X + MainMenu.Width / 2, Position.Y),
            new Vector2(Position.X + MainMenu.Width / 2, Position.Y + Drawing.Height)
        };

        #endregion
    }
}