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

namespace LeagueSharp.SDK.Core.UI.IMenu
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public class MenuManager
    {
        #region Static Fields

        /// <summary>
        ///     The configuration folder
        /// </summary>
        public static readonly DirectoryInfo ConfigFolder =
            Directory.CreateDirectory(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "LS" + Environment.UserName.GetHashCode().ToString("X"),
                    "MenuConfigSDK"));

        /// <summary>
        ///     The Instance.
        /// </summary>
        public static readonly MenuManager Instance = new MenuManager();

        #endregion

        #region Fields

        /// <summary>
        ///     The delayed draw actions.
        /// </summary>
        private readonly Queue<Action> delayedDrawActions = new Queue<Action>();

        /// <summary>
        ///     The menus list.
        /// </summary>
        private readonly List<Menu> menus = new List<Menu>();

        /// <summary>
        ///     The forced open.
        /// </summary>
        private bool forcedOpen;

        /// <summary>
        ///     Menu visible <c>bool</c>
        /// </summary>
        private bool menuVisible;

        /// <summary>
        ///     The sprite drawn protection.
        /// </summary>
        private bool ppSpriteDrawnProtection;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="MenuManager" /> class from being created.
        /// </summary>
        private MenuManager()
        {
            this.Sprite = new Sprite(Drawing.Direct3DDevice);
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
        ///     Gets or sets a value indicating whether the menu was forced to open.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the menu was forced to open; otherwise, <c>false</c>.
        /// </value>
        public bool ForcedOpen
        {
            get
            {
                return this.forcedOpen;
            }

            set
            {
                this.forcedOpen = value;
                if (this.forcedOpen)
                {
                    this.MenuVisible = true;
                }
            }
        }

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
        ///     Gets the <see cref="Menu" /> with the specified name.
        /// </summary>
        /// <value>
        ///     The <see cref="IMenu.Menu" />.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns>The requested menu</returns>
        public Menu this[string name]
        {
            get
            {
                return this.menus.FirstOrDefault(menu => menu.Name.Equals(name));
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

        /// <summary>
        ///     Draw actions in the specified action will happen after the menu has been drawn.
        /// </summary>
        /// <param name="a">The <see cref="Action" /></param>
        public void DrawDelayed(Action a)
        {
            this.delayedDrawActions.Enqueue(a);
        }

        /// <summary>
        ///     Causes the entire Menu tree to recalculate their widths.
        /// </summary>
        public void ResetWidth()
        {
            foreach (var menu in this.Menus)
            {
                menu.ResetWidth();
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
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            // Do nothing.
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
                if (!this.ppSpriteDrawnProtection)
                {
                    this.Sprite.Begin(SpriteFlags.AlphaBlend);
                    this.ppSpriteDrawnProtection = true;
                }

                ThemeManager.Current.Draw();
                if (this.ppSpriteDrawnProtection)
                {
                    this.Sprite.End();
                    this.ppSpriteDrawnProtection = false;
                }

                if (!this.ppSpriteDrawnProtection)
                {
                    this.Sprite.Begin(SpriteFlags.AlphaBlend);
                    this.ppSpriteDrawnProtection = true;
                }

                while (this.delayedDrawActions.Count > 0)
                {
                    this.delayedDrawActions.Dequeue().Invoke();
                }

                if (this.ppSpriteDrawnProtection)
                {
                    this.Sprite.End();
                    this.ppSpriteDrawnProtection = false;
                }
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
            if (MenuGUI.IsChatOpen)
            {
                return;
            }

            var keys = new WindowsKeys(args);
            if (!this.ForcedOpen)
            {
                if (keys.SingleKey == Keys.ShiftKey || keys.Key == (Keys.Return | Keys.Shift))
                {
                    var keyDown = keys.Msg == WindowsMessages.KEYDOWN;
                    var keyUp = keys.Msg == WindowsMessages.KEYUP || keys.Msg == WindowsMessages.CHAR;

                    if (keyDown)
                    {
                        if (!this.MenuVisible)
                        {
                            this.MenuVisible = true;
                            this.FireOnOpen();
                        }
                    }
                    else if (keyUp)
                    {
                        if (this.MenuVisible)
                        {
                            this.MenuVisible = false;
                            this.FireOnClose();
                        }
                    }
                }
                else if (keys.SingleKey == Keys.CapsLock && keys.Msg == WindowsMessages.KEYDOWN)
                {
                    this.MenuVisible = !this.MenuVisible;
                    if (this.MenuVisible)
                    {
                        this.FireOnOpen();
                    }
                    else
                    {
                        this.FireOnClose();
                    }
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