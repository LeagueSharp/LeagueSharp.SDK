#region

using System.Collections.Generic;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu User Interface.
    /// </summary>
    public class Menu : AMenuComponent
    {
        /// <summary>
        ///     Menu Children.
        /// </summary>
        private readonly IDictionary<string, AMenuComponent> children = new Dictionary<string, AMenuComponent>();

        /// <summary>
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">Menu Name</param>
        /// <param name="displayName">Menu Display Name</param>
        /// <param name="root">Is Menu Root</param>
        public Menu(string name, string displayName, bool root = false) : base(name, displayName)
        {
            Root = (root) ? this : null;
        }

        /// <summary>
        ///     Component Sub Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public override AMenuComponent this[string name]
        {
            get { return children.ContainsKey(name) ? children[name] : null; }
        }

        /// <summary>
        ///     Returns the menu visiblity.
        /// </summary>
        public override bool Visible { get; set; }

        /// <summary>
        ///     Returns if the menu is enabled.
        /// </summary>
        public override bool Enabled { get; set; }

        /// <summary>
        ///     Attaches the menu towards the main menu.
        /// </summary>
        /// <returns>Menu Instance</returns>
        public Menu AttachMenu()
        {
            return this;
        }

        /// <summary>
        ///     Add a menu component to this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component</param>
        public void Add(AMenuComponent component)
        {
            component.Parent = this;
            children.Add(component.Name, component);
        }

        /// <summary>
        ///     Removes a menu component from this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component instance</param>
        public void Remove(AMenuComponent component)
        {
            if (children.ContainsKey(component.Name))
            {
                component.Parent = null;
                children.Remove(component.Name);
            }
        }

        /// <summary>
        ///     Menu Drawing callback.
        /// </summary>
        public override void OnDraw() {}

        /// <summary>
        ///     Menu Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args) {}

        /// <summary>
        ///     Menu Update callback.
        /// </summary>
        public override void OnUpdate() {}
    }
}