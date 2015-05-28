// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AMenuComponent.cs" company="LeagueSharp">
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
//   Abstract build of a menu component.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Abstracts
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Abstract build of a menu component.
    /// </summary>
    public abstract class AMenuComponent : DynamicObject
    {
        #region Fields

        /// <summary>
        ///     Local menu width.
        /// </summary>
        private int menuWidthCached;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMenuComponent" /> class.
        ///     Abstract Constructor
        /// </summary>
        /// <param name="name">
        ///     Menu Name
        /// </param>
        /// <param name="displayName">
        ///     Menu Display Name
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string (ID)
        /// </param>
        protected AMenuComponent(string name, string displayName, string uniqueString)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(displayName))
            {
                throw new Exception("Please enter a valid name.\nName: " + name + "\nDisplayName: " + displayName);
            }
            this.AssemblyName = Assembly.GetEntryAssembly().GetName().Name;
            this.UniqueString = uniqueString;
            this.Name = name;
            this.DisplayName = displayName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        ///     The name of the assembly.
        /// </value>
        public string AssemblyName { get; protected set; }

        /// <summary>
        ///     Gets or sets the Menu Component Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the width of the menu.
        /// </summary>
        /// <value>
        ///     The width of the menu.
        /// </value>
        public int MenuWidth
        {
            get
            {
                if (this.menuWidthCached == 0)
                {
                    this.menuWidthCached = this.Parent != null
                                               ? this.Parent.Components.Max(comp => comp.Value.Width)
                                               : MenuManager.Instance.Menus.Max(menu => menu.Width);
                }

                return this.menuWidthCached;
            }

            set
            {
                this.menuWidthCached = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Menu Component Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Parent Menu Component.
        /// </summary>
        public Menu Parent { get; set; }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public abstract string Path { get; }

        /// <summary>
        ///     Gets or sets the Component Position
        /// </summary>
        public abstract Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether component is on the uppermost level.
        /// </summary>
        public bool Root { get; protected set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the component is toggled.
        /// </summary>
        public abstract bool Toggled { get; set; }

        /// <summary>
        ///     Gets or sets the unique string.
        /// </summary>
        /// <value>
        ///     The unique string.
        /// </value>
        public string UniqueString { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the component is visible.
        /// </summary>
        public abstract bool Visible { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public abstract int Width { get; }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Component Sub Object accessibility.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public abstract AMenuComponent this[string name] { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the value of a child with a certain name.
        /// </summary>
        /// <typeparam name="T">The type of MenuValue of this child.</typeparam>
        /// <param name="name">The name of the child.</param>
        /// <returns>The value that is attached to this Child.</returns>
        public abstract T GetValue<T>(string name) where T : AMenuValue;

        /// <summary>
        ///     Get the value of this component.
        /// </summary>
        /// <typeparam name="T">The type of MenuValue of this component.</typeparam>
        /// <returns>The value that is attached to this component.</returns>
        public abstract T GetValue<T>() where T : AMenuValue;

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public abstract void Load();

        /// <summary>
        ///     Component Drawing callback.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        public abstract void OnDraw(Vector2 position, int index);

        /// <summary>
        ///     Component Update callback.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        ///     Component Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        ///     Resets the width.
        /// </summary>
        public void ResetWidth()
        {
            if (this.Parent != null)
            {
                foreach (var comp in this.Parent.Components)
                {
                    comp.Value.MenuWidth = 0;
                }
            }
            else
            {
                foreach (var menu in MenuManager.Instance.Menus)
                {
                    menu.MenuWidth = 0;
                }
            }
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public abstract void Save();

        /// <summary>
        ///     Dynamic Object Member Resolver.
        /// </summary>
        /// <param name="binder">Member Binder</param>
        /// <param name="result">Object Result</param>
        /// <returns>Whether was found</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                var comp = this[binder.Name];
                var item = comp as MenuItem;
                result = item != null ? item.ValueAsObject : comp;
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        #endregion
    }
}