using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public class MenuManager
    {
        /// <summary>
        /// The configuration folder
        /// </summary>
        public static DirectoryInfo ConfigFolder =
            Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "LS" + Environment.UserName.GetHashCode().ToString("X"), "MenuConfigEx"));

        private static readonly MenuManager instance = new MenuManager();

        private readonly List<Menu> _menus = new List<Menu>();

        private readonly Vector2 _position = new Vector2(30, 30);
        private bool _menuVisible;

        private MenuManager()
        {
            MenuVisible = true;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => SaveSettings();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => SaveSettings();
        }

        /// <summary>
        /// Gets the <see cref="Menu"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="Menu"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Menu this[string name]
        {
            get { return _menus.First(menu => menu.Name.Equals(name)); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the menu is visible; otherwise, <c>false</c>.
        /// </value>
        public bool MenuVisible
        {
            get { return _menuVisible || ForcedOpen; }
            set { _menuVisible = value; }
        }

        /// <summary>
        /// Gets the menus.
        /// </summary>
        /// <value>
        /// The menus.
        /// </value>
        public List<Menu> Menus
        {
            get { return _menus; }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static MenuManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu was forced to open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the menu was forced to open; otherwise, <c>false</c>.
        /// </value>
        public bool ForcedOpen { get; set; }

        private void SaveSettings()
        {
            foreach (Menu menu in _menus)
            {
                try
                {
                    menu.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Occurs when the menu is opened.
        /// </summary>
        public event EventHandler OnOpen;

        /// <summary>
        /// Fires the on open.
        /// </summary>
        protected virtual void FireOnOpen()
        {
            EventHandler handler = OnOpen;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the menu is closed.
        /// </summary>
        public event EventHandler OnClose;

        /// <summary>
        /// Fires the on close.
        /// </summary>
        protected virtual void FireOnClose()
        {
            EventHandler handler = OnClose;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            var keys = new WindowsKeys(args);
            if (keys.SingleKey == Keys.ShiftKey)
            {
                bool value = keys.Msg == WindowsMessages.KEYDOWN;

                MenuVisible = value;

                if (value)
                {
                    FireOnOpen();
                }
                else
                {
                    FireOnClose();
                }
            }
            else if (keys.SingleKey == Keys.CapsLock && keys.Msg == WindowsMessages.KEYDOWN)
            {
                MenuVisible = !MenuVisible;
            }

            foreach (Menu component in _menus)
            {
                component.OnWndProc(keys);
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            //nothing
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (MenuVisible)
            {
                ThemeManager.Current.OnDraw(_position);
            }
        }

        /// <summary>
        /// Adds the specified menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public void Add(Menu menu)
        {
            if (!_menus.Contains(menu))
            {
                _menus.Add(menu);
                try
                {
                    menu.Load();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}