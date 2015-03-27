#region

using System;
using System.Collections.Concurrent;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;
using SharpDX.Direct3D9;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     User Interface Root class
    /// </summary>
    public class Root
    {
        /// <summary>
        ///     Menu List Holder
        /// </summary>
        private static readonly ConcurrentDictionary<string, AMenuComponent> MenuList =
            new ConcurrentDictionary<string, AMenuComponent>();

        /// <summary>
        ///     Menu State
        /// </summary>
        private static MenuState _state;

        /// <summary>
        ///     Static Constructor
        /// </summary>
        static Root()
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;

            _state = MenuState.HomePage;
        }

        /// <summary>
        ///     Windows Process Messages callback
        /// </summary>
        /// <param name="args">
        ///     <see cref="WndEventArgs" />
        /// </param>
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (_state == MenuState.Assemblies)
            {
                foreach (var menu in MenuList)
                {
                    menu.Value.OnWndProc(new WindowsKeys(args));
                }
            }
            else if (_state == MenuState.HomePage) {}
        }

        /// <summary>
        ///     Drawing callback
        /// </summary>
        /// <param name="args">
        ///     <see cref="EventArgs" />
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            Menu.OnDraw();

            if (_state == MenuState.Assemblies)
            {
                foreach (var menu in MenuList)
                {
                    menu.Value.OnDraw();
                }
            }
        }

        /// <summary>
        ///     Update callback
        /// </summary>
        /// <param name="args">
        ///     <see cref="EventArgs" />
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (_state == MenuState.Assemblies)
            {
                foreach (var menu in MenuList)
                {
                    menu.Value.OnUpdate();
                }
            }
        }

        /// <summary>
        ///     Adds a menu component.
        /// </summary>
        /// <param name="component">Menu Component</param>
        /// <returns>If the action was successful</returns>
        public static bool Add(AMenuComponent component)
        {
            return !MenuList.ContainsKey(component.Id) && component.Parent == null && component.Root != null &&
                   MenuList.TryAdd(component.Id, component);
        }

        /// <summary>
        ///     Removes a menu component.
        /// </summary>
        /// <param name="component">Menu Component</param>
        /// <returns>If the action was successful</returns>
        public static bool Remove(AMenuComponent component)
        {
            return MenuList.ContainsKey(component.Id) && MenuList.TryRemove(component.Id, out component);
        }

        #region Menu Setup

        /// <summary>
        ///     Menu Setup container
        /// </summary>
        private static class Menu
        {
            /// <summary>
            ///     Menu Sprite
            /// </summary>
            private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

            /// <summary>
            ///     Menu SubBox Color
            /// </summary>
            private static readonly ColorBGRA ColorSubBox = new ColorBGRA(0, 0, 0, 127.5f);

            /// <summary>
            ///     Menu MainBox Color
            /// </summary>
            private static readonly ColorBGRA ColorMainBox = new ColorBGRA(0, 0, 0, 63.75f);

            /// <summary>
            ///     Menu Draw callback
            /// </summary>
            public static void OnDraw()
            {
                Sprite.Begin();

                SubBox.Begin();
                SubBox.Draw(VerticesSubBox, ColorSubBox);
                SubBox.End();

                MainBox.Begin();
                MainBox.Draw(VerticesMainBox, ColorMainBox);
                MainBox.End();

                Sprite.End();
            }

            /// <summary>
            ///     Menu SubBox Line
            /// </summary>
            private static readonly Line SubBox = new Line(Drawing.Direct3DDevice)
            {
                Antialias = true,
                GLLines = true,
                Width = 350f
            };

            /// <summary>
            ///     Menu MainBox Line
            /// </summary>
            private static readonly Line MainBox = new Line(Drawing.Direct3DDevice)
            {
                Antialias = true,
                GLLines = true,
                Width = 325f
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
                new Vector2(Position.X + SubBox.Width / 2, Position.Y),
                new Vector2(Position.X + SubBox.Width / 2, Position.Y + Drawing.Height)
            };

            /// <summary>
            ///     Main Box Vertices
            /// </summary>
            private static readonly Vector2[] VerticesMainBox =
            {
                new Vector2(Position.X + MainBox.Width / 2, Position.Y),
                new Vector2(Position.X + MainBox.Width / 2, Position.Y + Drawing.Height)
            };
        }

        #endregion
    }
}