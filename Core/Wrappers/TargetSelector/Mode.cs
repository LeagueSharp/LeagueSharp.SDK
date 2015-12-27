// <copyright file="Mode.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.Core.Wrappers.TargetSelector
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    #endregion

    /// <summary>
    ///     Interface for modes
    /// </summary>
    public interface ITargetSelectorMode
    {
        #region Public Properties

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        string DisplayName { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds to menu.
        /// </summary>
        /// <param name="menu">The menu.</param>
        void AddToMenu(Menu menu);

        /// <summary>
        ///     Orders the champions.
        /// </summary>
        /// <param name="heroes">The heroes.</param>
        /// <returns></returns>
        List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes);

        #endregion
    }

    /// <summary>
    ///     The mode for targetselector
    /// </summary>
    public class Mode
    {
        #region Fields

        private readonly Menu menu;

        private readonly List<ITargetSelectorMode> pEntries = new List<ITargetSelectorMode>();

        private ITargetSelectorMode current;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mode" /> class.
        /// </summary>
        /// <param name="menu">The menu.</param>
        public Mode(Menu menu)
        {
            this.menu = menu;
            var modes =
                Assembly.GetAssembly(typeof(ITargetSelectorMode))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ITargetSelectorMode).IsAssignableFrom(t))
                    .ToList();

            foreach (var mode in modes)
            {
                var instance = DynamicInitializer.NewInstance(mode) as ITargetSelectorMode;
                if (instance != null)
                {
                    this.pEntries.Add(instance);
                    instance.AddToMenu(this.menu);
                }
            }

            this.pEntries = this.pEntries.OrderBy(p => p.DisplayName).ToList();

            this.menu.Add(new MenuList<string>("mode", "Mode", this.pEntries.Select(e => e.DisplayName)));

            this.menu.MenuValueChanged += (sender, args) =>
                {
                    var stringList = sender as MenuList<string>;
                    if (stringList != null)
                    {
                        if (stringList.Name.Equals("mode"))
                        {
                            this.current = this.GeModeBySelectedIndex(stringList.Index);
                        }
                    }
                };

            this.current = this.GeModeBySelectedIndex(this.menu["mode"].GetValue<MenuList<string>>().Index);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The<see cref="OnChange" /> event delegate.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public delegate void OnChangeDelegate(object sender, ITargetSelectorMode e);

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when [Current] [on change].
        /// </summary>
        public event OnChangeDelegate OnChange;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the current.
        /// </summary>
        /// <value>
        ///     The current.
        /// </value>
        public ITargetSelectorMode Current
        {
            get
            {
                return this.current;
            }
            set
            {
                if (this.pEntries.Contains(value))
                {
                    if (this.current != value)
                    {
                        this.OnChange?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, value);
                    }
                    this.current = value;
                    this.menu["mode"].GetValue<MenuList<string>>().Index = this.pEntries.IndexOf(this.Current);
                }
            }
        }

        /// <summary>
        ///     The entries
        /// </summary>
        public ReadOnlyCollection<ITargetSelectorMode> Entries => this.pEntries.AsReadOnly();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Deregisters the specified Mode.
        /// </summary>
        /// <param name="mode">The Mode.</param>
        public void Deregister(ITargetSelectorMode mode)
        {
            if (this.pEntries.Contains(mode))
            {
                this.pEntries.Remove(mode);
                if (!this.pEntries.Contains(this.Current))
                {
                    this.Current = this.pEntries.FirstOrDefault();
                }
                this.UpdateMenu();
            }
        }

        /// <summary>
        ///     Orders the champions.
        /// </summary>
        /// <param name="heroes">The heroes.</param>
        /// <returns></returns>
        public List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes)
        {
            try
            {
                return this.Current.OrderChampions(heroes);
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, ex);
            }
            return new List<Obj_AI_Hero>();
        }

        /// <summary>
        ///     Overwrites the specified old Mode.
        /// </summary>
        /// <param name="oldMode">The old Mode.</param>
        /// <param name="newMode">The new Mode.</param>
        public void Overwrite(ITargetSelectorMode oldMode, ITargetSelectorMode newMode)
        {
            var index = this.pEntries.IndexOf(oldMode);
            if (index >= 0)
            {
                this.pEntries[index] = newMode;
                this.UpdateMenu();
            }
        }

        /// <summary>
        ///     Registers the specified Mode.
        /// </summary>
        /// <param name="mode">The Mode.</param>
        public void Register(ITargetSelectorMode mode)
        {
            if (!this.pEntries.Any(m => m.Name.Equals(mode.Name)) && !string.IsNullOrEmpty(mode.DisplayName))
            {
                this.pEntries.Add(mode);
                this.UpdateMenu();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Ges the index of the mode by selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private ITargetSelectorMode GeModeBySelectedIndex(int index)
        {
            return index < this.pEntries.Count && index >= 0 ? this.pEntries[index] : null;
        }

        /// <summary>
        ///     Updates the menu.
        /// </summary>
        private void UpdateMenu()
        {
            this.menu["mode"].GetValue<MenuList<string>>().Values = this.pEntries.Select(e => e.DisplayName).ToArray();
        }

        #endregion
    }
}