// <copyright file="Menu.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.SDKEx.UI.Skins;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    /// <summary>
    ///     Menu Value Changed delegate
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The Menu Value Changed Event Data</param>
    public delegate void OnMenuValueChanged(object sender, MenuValueChangedEventArgs e);

    /// <summary>
    ///     Menu User Interface.
    /// </summary>
    public class Menu : AMenuComponent
    {
        #region Fields

        /// <summary>
        ///     Menu Component Sub-Components.
        /// </summary>
        public IDictionary<string, AMenuComponent> Components = new Dictionary<string, AMenuComponent>();

        /// <summary>
        ///     Local toggled indicator.
        /// </summary>
        private bool toggled;

        /// <summary>
        ///     Local visible value.
        /// </summary>
        private bool visible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Menu" /> class.
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">
        ///     Menu Name
        /// </param>
        /// <param name="displayName">
        ///     Menu Display Name
        /// </param>
        /// <param name="root">
        ///     Root component
        /// </param>
        /// <param name="uniqueString">
        ///     Unique string
        /// </param>
        public Menu(string name, string displayName, bool root = false, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Root = root;
            this.AssemblyName = Assembly.GetCallingAssembly().GetName().Name;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a value is changed.
        /// </summary>
        public event OnMenuValueChanged MenuValueChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the path.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public override string Path
        {
            get
            {
                if (this.SharedSettings)
                {
                    return MenuManager.ConfigFolder.CreateSubdirectory("SharedConfig").FullName;
                }

                if (this.Parent == null)
                {
                    return
                        MenuManager.ConfigFolder.CreateSubdirectory(this.AssemblyName)
                            .CreateSubdirectory(this.Name + this.UniqueString)
                            .FullName;
                }

                return
                    Directory.CreateDirectory(System.IO.Path.Combine(this.Parent.Path, this.Name + this.UniqueString))
                        .FullName;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether that the settings are shared.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the settings are shared; otherwise, <c>false</c>.
        /// </value>
        public bool SharedSettings { get; set; }

        /// <summary>
        ///     Returns if the menu has been toggled.
        /// </summary>
        public override sealed bool Toggled
        {
            get
            {
                return this.toggled;
            }

            set
            {
                this.toggled = value;

                // Hide children when untoggled
                foreach (var comp in this.Components)
                {
                    comp.Value.Visible = value;
                    if (!this.toggled)
                    {
                        comp.Value.Toggled = false;
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the menu visibility.
        /// </summary>
        public override sealed bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                this.visible = value;
                if (this.Toggled)
                {
                    foreach (var comp in this.Components)
                    {
                        comp.Value.Visible = value;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Component Sub Object accessibility.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public override AMenuComponent this[string name]
        {
            get
            {
                AMenuComponent value;
                return this.Components.TryGetValue(name, out value) ? value : null;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Add a menu component to this menu.
        /// </summary>
        /// <typeparam name="T">
        ///     <see cref="AMenuComponent" /> type
        /// </typeparam>
        /// <param name="component">
        ///     <see cref="AMenuComponent" /> component
        /// </param>
        /// <returns>
        ///     The <see cref="AMenuComponent" /> instance.
        /// </returns>
        public virtual T Add<T>(T component) where T : AMenuComponent
        {
            if (!this.Components.ContainsKey(component.Name))
            {
                component.Parent = this;
                this.Components[component.Name] = component;

                AMenuComponent comp = this;
                while (comp.Parent != null)
                {
                    comp = comp.Parent;
                }

                if (comp.Root)
                {
                    comp.Load();
                }
            }
            else
            {
                var existingComponent = this.Components[component.Name] as Menu;
                var newComponent = component as Menu;
                if (existingComponent != null && newComponent != null)
                {
                    foreach (var comp in newComponent.Components)
                    {
                        existingComponent.Add(comp.Value);
                    }
                }
                else
                {
                    throw new Exception("This menu already contains a component with the name " + component.Name);
                }
            }

            MenuManager.Instance.ResetWidth();
            return component;
        }

        /// <summary>
        ///     Attaches the menu towards the main menu.
        /// </summary>
        /// <returns>Menu Instance</returns>
        public Menu Attach()
        {
            if (this.Parent == null && this.Root)
            {
                MenuManager.Instance.Add(this);
            }
            else
            {
                throw new Exception("You should not add a Menu that already has a parent or is not a Root component.");
            }

            return this;
        }

        /// <summary>
        ///     Get the value of a child with a certain name.
        /// </summary>
        /// <typeparam name="T">The type of MenuValue of this child.</typeparam>
        /// <param name="name">The name of the child.</param>
        /// <returns>
        ///     The value that is attached to this Child.
        /// </returns>
        /// <exception cref="Exception">Could not find child with name  + name</exception>
        public override T GetValue<T>(string name)
        {
            AMenuComponent value;
            if (this.Components.TryGetValue(name, out value))
            {
                return (T)value;
            }

            throw new Exception("Could not find child with name " + name);
        }

        /// <summary>
        ///     Get the value of this component.
        /// </summary>
        /// <typeparam name="T">The type of MenuValue of this component.</typeparam>
        /// <returns>
        ///     The value that is attached to this component.
        /// </returns>
        /// <exception cref="Exception">Cannot get the Value of a Menu</exception>
        public override T GetValue<T>()
        {
            throw new Exception("Cannot get the Value of a Menu");
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load()
        {
            foreach (var comp in this.Components)
            {
                comp.Value.Load();
            }
        }

        /// <summary>
        ///     Menu Drawing callback.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        public override void OnDraw(Vector2 position)
        {
            this.Position = position;
            this.Handler.Draw();
        }

        /// <summary>
        ///     Menu Update callback.
        /// </summary>
        public override void OnUpdate()
        {
        }

        /// <summary>
        ///     Menu Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            this.Handler.OnWndProc(args);

            // Pass OnWndProc on to children
            foreach (var item in this.Components)
            {
                item.Value.OnWndProc(args);
            }
        }

        /// <summary>
        ///     Removes a menu component from this menu.
        /// </summary>
        /// <param name="component">
        ///     <see cref="AMenuComponent" /> component instance
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Remove(AMenuComponent component)
        {
            if (this.Components.ContainsKey(component.Name))
            {
                component.Save();
                component.Parent = null;
                return this.Components.Remove(component.Name);
            }

            return false;
        }

        /// <summary>
        ///     Resets the width.
        /// </summary>
        public override void ResetWidth()
        {
            base.ResetWidth();
            foreach (var comp in this.Components)
            {
                comp.Value.ResetWidth();
            }
        }

        /// <summary>
        ///     Resets the children of this menu back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            foreach (var comp in this.Components)
            {
                comp.Value.RestoreDefault();
            }
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save()
        {
            foreach (var comp in this.Components)
            {
                comp.Value.Save();
            }
        }

        /// <summary>
        ///     Toggles this menu component.
        /// </summary>
        public void Toggle()
        {
            if (this.Components.Count > 0)
            {
                this.Toggled = !this.Toggled;

                // Toggling siblings logic
                if (this.Parent == null)
                {
                    foreach (var rootComponent in MenuManager.Instance.Menus.Where(c => !c.Equals(this)))
                    {
                        rootComponent.Toggled = false;
                    }
                }
                else
                {
                    foreach (var comp in this.Parent.Components.Where(comp => comp.Value.Name != this.Name))
                    {
                        comp.Value.Toggled = false;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fire the Value Changed event.
        /// </summary>
        /// <param name="sender">
        ///     The sender object.
        /// </param>
        internal void FireEvent(MenuItem sender)
        {
            this.MenuValueChanged?.Invoke(sender, new MenuValueChangedEventArgs(this, sender));
        }

        /// <summary>
        ///     Builds an <see cref="ADrawable" /> for this component.
        /// </summary>
        /// <param name="theme">
        ///     The theme.
        /// </param>
        /// <returns>
        ///     The <see cref="ADrawable" /> instance.
        /// </returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildMenuHandler(this);
        }

        #endregion
    }
}