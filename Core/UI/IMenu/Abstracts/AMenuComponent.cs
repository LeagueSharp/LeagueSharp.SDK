// <copyright file="AMenuComponent.cs" company="LeagueSharp">
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
    using System.Dynamic;
    using System.Linq;

    using LeagueSharp.SDKEx.UI.Skins;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    /// <summary>
    ///     Abstract build of a menu component.
    /// </summary>
    public abstract class AMenuComponent : DynamicObject
    {
        #region Fields

        /// <summary>
        ///     The default theme handler.
        /// </summary>
        private readonly ADrawable defaultThemeHandler = new ADrawableAdapter();

        /// <summary>
        ///     The current theme.
        /// </summary>
        private ITheme currentTheme;

        /// <summary>
        ///     Local menu width.
        /// </summary>
        private int menuWidthCached;

        /// <summary>
        ///     True if MenuWidth should be recalculated.
        /// </summary>
        private bool resetWidth = true;

        /// <summary>
        ///     The theme handler.
        /// </summary>
        private ADrawable themeHandler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMenuComponent" /> class.
        /// </summary>
        internal AMenuComponent()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AMenuComponent" /> class.
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
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(displayName))
            {
                throw new Exception("Please enter a valid name.\nName: " + name + "\nDisplayName: " + displayName);
            }

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
                if (this.resetWidth)
                {
                    this.menuWidthCached = this.Parent?.Components.Max(comp => comp.Value.Width)
                                           ?? MenuManager.Instance.Menus.Max(menu => menu.Width);
                    this.resetWidth = false;
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
        public Vector2 Position { get; set; }

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

        #region Properties

        /// <summary>
        ///     Gets the current handler for this AMenuComponent. If it is null it will ask the current theme to build a new one.
        /// </summary>
        protected ADrawable Handler
        {
            get
            {
                if (this.themeHandler != null && this.currentTheme != ThemeManager.Current)
                {
                    this.themeHandler.Dispose();
                    this.themeHandler = null;
                }

                if (this.themeHandler == null || this.currentTheme != ThemeManager.Current)
                {
                    this.currentTheme = ThemeManager.Current;
                    this.themeHandler = this.BuildHandler(ThemeManager.Current);
                    if (this.themeHandler == null)
                    {
                        this.themeHandler = this.defaultThemeHandler;
                        Console.WriteLine(@"No ADrawable handler exists for the component of type " + this.GetType());
                    }
                }

                return this.themeHandler;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     Component Sub Object accessibility.
        /// </summary>
        /// <param name="name">
        ///     Child Menu Component name
        /// </param>
        /// <returns>Child Menu Component of this component.</returns>
        public abstract AMenuComponent this[string name] { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     AMenuComponent conversion operator to a <see cref="bool" />.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static implicit operator bool(AMenuComponent component)
        {
            return component.GetValue<MenuBool>().Value;
        }

        /// <summary>
        ///     AMenuComponent conversion operator to a <see cref="ColorBGRA" />.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        /// <returns>
        ///     The <see cref="ColorBGRA" />.
        /// </returns>
        public static implicit operator ColorBGRA(AMenuComponent component)
        {
            return component.GetValue<MenuColor>().Color;
        }

        /// <summary>
        ///     AMenuComponent conversion operator to a <see cref="Color" />.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        /// <returns>
        ///     The <see cref="System.Drawing.Color" />.
        /// </returns>
        public static implicit operator System.Drawing.Color(AMenuComponent component)
        {
            var color = component.GetValue<MenuColor>().Color;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        ///     AMenuComponent conversion operator to a <see cref="int" />.
        /// </summary>
        /// <param name="component">
        ///     The component
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static implicit operator int(AMenuComponent component)
        {
            return component.GetValue<MenuSlider>().Value;
        }

        /// <summary>
        ///     Get the value of a child with a certain name.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of MenuValue of this child.
        /// </typeparam>
        /// <param name="name">The name of the child.</param>
        /// <returns>The value that is attached to this Child.</returns>
        public abstract T GetValue<T>(string name) where T : MenuItem;

        /// <summary>
        ///     Get the value of this component.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of MenuValue of this component.
        /// </typeparam>
        /// <returns>The value that is attached to this component.</returns>
        public abstract T GetValue<T>() where T : MenuItem;

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
        public abstract void OnDraw(Vector2 position);

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
        public virtual void ResetWidth()
        {
            this.resetWidth = true;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public abstract void RestoreDefault();

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public abstract void Save();

        #endregion

        #region Methods

        /// <summary>
        ///     Builds an <see cref="ADrawable" /> for this component.
        /// </summary>
        /// <param name="theme">
        ///     The theme.
        /// </param>
        /// <returns>
        ///     The <see cref="ADrawable" /> instance.
        /// </returns>
        protected virtual ADrawable BuildHandler(ITheme theme)
        {
            return null;
        }

        #endregion
    }
}