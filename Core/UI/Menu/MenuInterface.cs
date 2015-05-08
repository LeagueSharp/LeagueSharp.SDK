using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public class MenuInterface
    {
        public static DirectoryInfo ConfigFolder =
            Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "LS" + Environment.UserName.GetHashCode().ToString("X"), "MenuConfigEx"));

        private static readonly MenuInterface instance = new MenuInterface();

        private readonly List<Menu> _menus = new List<Menu>();

        private bool _menuVisible;

        private MenuInterface()
        {
            MenuVisible = true;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainDomainUnload;
        }

        public bool MenuVisible
        {
            get { return _menuVisible || ForcedOpen; }
            set { _menuVisible = value; }
        }

        public List<Menu> Menus
        {
            get { return _menus; }
        }

        public static MenuInterface Instance
        {
            get { return instance; }
        }

        public bool ForcedOpen { get; set; }

        private void CurrentDomainDomainUnload(object sender, EventArgs args)
        {
            Console.WriteLine("UNLOADING");
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

        public event EventHandler OnOpen;

        protected virtual void FireOnOpen()
        {
            EventHandler handler = OnOpen;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnClose;

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
                ThemeManager.Current.OnDraw(MenuSettings.Position);
            }
        }

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