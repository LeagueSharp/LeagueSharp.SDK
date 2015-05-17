// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuManager.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Menu Interface class, used to control the menu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public class MenuManager
    {
        #region Static Fields

        /// <summary>
        ///     The Instance.
        /// </summary>
        public static readonly MenuManager Instance = new MenuManager();

        /// <summary>
        ///     The configuration folder
        /// </summary>
        private static DirectoryInfo configFolder =
            Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    "LS" + Environment.UserName.GetHashCode().ToString("X"), 
                    "MenuConfigEx"));

        #endregion

        #region Fields

        /// <summary>
        ///     The menus list.
        /// </summary>
        private readonly List<Menu> menus = new List<Menu>();

        /// <summary>
        ///     The default menu zero-position.
        /// </summary>
        private readonly Vector2 position = new Vector2(30, 30);

        /// <summary>
        ///     Menu visible <c>bool</c>
        /// </summary>
        private bool menuVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="MenuManager" /> class from being created.
        /// </summary>
        private MenuManager()
        {
            this.Sprite = new Sprite(Drawing.Direct3DDevice);
            this.MenuVisible = true;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnEndScene += this.Drawing_OnDraw;
            Game.OnWndProc += this.Game_OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => this.SaveSettings();
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => this.SaveSettings();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when the menu is closed.
        /// </summary>
        public event EventHandler OnClose;

        /// <summary>
        ///     Occurs when the menu is opened.
        /// </summary>
        public event EventHandler OnOpen;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the configuration folder
        /// </summary>
        public static DirectoryInfo ConfigFolder
        {
            get
            {
                return configFolder;
            }

            set
            {
                configFolder = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the menu was forced to open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the menu was forced to open; otherwise, <c>false</c>.
        /// </value>
        public bool ForcedOpen { get; set; }

        /// <summary>
        ///     Gets the menus.
        /// </summary>
        /// <value>
        ///     The menus.
        /// </value>
        public List<Menu> Menus
        {
            get
            {
                return this.menus;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the menu is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the menu is visible; otherwise, <c>false</c>.
        /// </value>
        public bool MenuVisible
        {
            get
            {
                return this.menuVisible;
            }

            set
            {
                this.menuVisible = value;
                foreach (var menu in this.menus)
                {
                    menu.Visible = value;
                }
            }
        }

        /// <summary>
        ///     Gets The Sprite used to draw the components of the menu on.
        /// </summary>
        public Sprite Sprite { get; private set; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Gets the <see cref="UI.Menu" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="UI.Menu" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>The requested menu</returns>
        public Menu this[string name]
        {
            get
            {
                return this.menus.First(menu => menu.Name.Equals(name));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds the specified menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public void Add(Menu menu)
        {
            if (!this.menus.Contains(menu))
            {
                this.menus.Add(menu);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fires the on close.
        /// </summary>
        protected virtual void FireOnClose()
        {
            var handler = this.OnClose;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Fires the on open.
        /// </summary>
        protected virtual void FireOnOpen()
        {
            var handler = this.OnOpen;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     On update event.
        /// </summary>
        /// <param name="args">
        ///     Event data
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            // nothing
        }

        /// <summary>
        ///     On Draw event.
        /// </summary>
        /// <param name="args">
        ///     Event data
        /// </param>
        private void Drawing_OnDraw(EventArgs args)
        {
            if (this.MenuVisible)
            {
                ThemeManager.Current.OnDraw(this.position);
            }
        }

        /// <summary>
        ///     On Window Process Message event.
        /// </summary>
        /// <param name="args">
        ///     Event data
        /// </param>
        private void Game_OnWndProc(WndEventArgs args)
        {
            var keys = new WindowsKeys(args);
            if (!this.ForcedOpen)
            {
                if (keys.SingleKey == Keys.ShiftKey)
                {
                    var keyDown = keys.Msg == WindowsMessages.KEYDOWN;
                    var keyUp = keys.Msg == WindowsMessages.KEYUP;

                    if (keyDown)
                    {
                        this.MenuVisible = true;
                        this.FireOnOpen();
                    }
                    else if (keyUp)
                    {
                        this.MenuVisible = false;
                        this.FireOnClose();
                    }
                }
                else if (keys.SingleKey == Keys.CapsLock && keys.Msg == WindowsMessages.KEYDOWN)
                {
                    this.MenuVisible = !this.MenuVisible;
                }
            }

            foreach (var component in this.menus)
            {
                component.OnWndProc(keys);
            }
        }

        /// <summary>
        ///     Save settings method
        /// </summary>
        private void SaveSettings()
        {
            foreach (var menu in this.menus)
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

        #endregion
    }
}