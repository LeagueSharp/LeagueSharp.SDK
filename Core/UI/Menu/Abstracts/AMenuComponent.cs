#region

using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Abstracts
{
    /// <summary>
    ///     Abstract build of a menu component.
    /// </summary>
    public abstract class AMenuComponent : DynamicObject
    {
        private int _menuWidthCached;

        /// <summary>
        ///     Abstract Constructor
        /// </summary>
        /// <param name="name">Menu Name</param>
        /// <param name="displayName">Menu Display Name</param>
        protected AMenuComponent(string name, string displayName, string uniqueString)
        {
            UniqueString = uniqueString;
            AssemblyName = Assembly.GetCallingAssembly().GetName().Name;
            Name = name;
            DisplayName = displayName;
        }

        public string AssemblyName { get; private set; }

        public string UniqueString { get; set; }

        /// <summary>
        ///     Menu Component Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Menu Component Display Name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Parent Menu Component.
        /// </summary>
        public Menu Parent { get; set; }

        /// <summary>
        ///     Component Sub Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public abstract AMenuComponent this[string name] { get; }

        /// <summary>
        ///     Component Visibilty Flag.
        /// </summary>
        public abstract bool Visible { get; set; }

        /// <summary>
        ///     Component Toggled Flag.
        /// </summary>
        public abstract bool Toggled { get; set; }

        /// <summary>
        ///     Component Position
        /// </summary>
        public abstract Vector2 Position { get; set; }

        public abstract string Path { get; }

        public abstract int Width { get; }

        public int MenuWidth
        {
            get
            {
                if (_menuWidthCached == 0)
                {
                    if (Parent != null)
                    {
                        _menuWidthCached = Parent.Components.Max(comp => comp.Value.Width);
                    }
                    else
                    {
                        _menuWidthCached = MenuInterface.Instance.Menus.Max(menu => menu.Width);
                    }
                }
                return _menuWidthCached;
            }
            set { _menuWidthCached = value; }
        }

        public void ResetWidth()
        {
            if (Parent != null)
            {
                foreach (var comp in Parent.Components)
                {
                    comp.Value.MenuWidth = 0;
                }
            }
            else
            {
                foreach (Menu menu in MenuInterface.Instance.Menus)
                {
                    menu.MenuWidth = 0;
                }
            }
        }

        /// <summary>
        ///     Component Drawing callback.
        /// </summary>
        public abstract void OnDraw(Vector2 position, int index);

        /// <summary>
        ///     Component Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnWndProc(WindowsKeys args);

        /// <summary>
        ///     Component Update callback.
        /// </summary>
        public abstract void OnUpdate();

        public abstract void Save();

        public abstract void Load();


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
        ///     Dynamic Object Member Resolver.
        /// </summary>
        /// <param name="binder">Member Binder</param>
        /// <param name="result">Object Result</param>
        /// <returns>Whether was found</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                AMenuComponent comp = this[binder.Name];
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
    }
}