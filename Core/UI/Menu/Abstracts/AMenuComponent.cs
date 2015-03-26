#region

using System;
using System.Dynamic;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI.Abstracts
{
    /// <summary>
    ///     Abstract build of a menu component.
    /// </summary>
    public abstract class AMenuComponent : DynamicObject
    {
        /// <summary>
        ///     Local menu root component.
        /// </summary>
        private AMenuComponent _root;

        /// <summary>
        ///     Abstract Constructor
        /// </summary>
        /// <param name="name">Menu Name</param>
        /// <param name="displayName">Menu Display Name</param>
        protected AMenuComponent(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

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
        public AMenuComponent Parent { get; set; }

        /// <summary>
        ///     Root Menu Component Resolver.
        /// </summary>
        public AMenuComponent Root
        {
            get
            {
                if (_root == null && Parent != null)
                {
                    return Parent.Root;
                }
                return _root;
            }
            set { _root = value; }
        }

        /// <summary>
        ///     Component Dynamic Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public abstract AMenuComponent this[string name] { get; }

        /// <summary>
        ///     Component Visibilty.
        /// </summary>
        public abstract bool Visible { get; set; }

        /// <summary>
        ///     Component Enable Flag.
        /// </summary>
        public abstract bool Enabled { get; set; }

        /// <summary>
        ///     Component Drawing callback.
        /// </summary>
        public abstract void OnDraw();

        /// <summary>
        ///     Component Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public abstract void OnWndProc(WindowsKeys args);

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
    }
}