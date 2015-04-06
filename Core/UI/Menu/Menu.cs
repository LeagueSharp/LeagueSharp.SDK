#region

using System.Collections.Generic;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

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
        private readonly IDictionary<string, AMenuComponent> components = new Dictionary<string, AMenuComponent>();

        /// <summary>
        ///     Menu Constructor.
        /// </summary>
        /// <param name="name">Menu Name</param>
        /// <param name="displayName">Menu Display Name</param>
        /// <param name="root">Is Menu Root</param>
        public Menu(string name, string displayName, bool root = false) : base(name, displayName)
        {
            Root = (root) ? this : null;
            Visible = Enabled = true;
            MenuContainer = new MenuContainer { Components = components, DisplayName = displayName, Toggled = Toggled };
        }

        /// <summary>
        ///     Component Sub Object accessability.
        /// </summary>
        /// <param name="name">Child Menu Component name</param>
        /// <returns>Child Menu Component of this component.</returns>
        public override AMenuComponent this[string name]
        {
            get { return components.ContainsKey(name) ? components[name] : null; }
        }

        /// <summary>
        ///     Returns the menu visiblity.
        /// </summary>
        public override sealed bool Visible { get; set; }

        /// <summary>
        ///     Returns if the menu is enabled.
        /// </summary>
        public override sealed bool Enabled { get; set; }

        /// <summary>
        ///     Returns if the menu has been toggled.
        /// </summary>
        public override sealed bool Toggled { get; set; }

        private MenuContainer MenuContainer { get; set; }

        /// <summary>
        ///     Menu Position
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Attaches the menu towards the main menu.
        /// </summary>
        /// <returns>Menu Instance</returns>
        public Menu AttachMenu()
        {
            if (Root == this)
            {
                MenuInterface.RootMenuComponents.Add(this);
                return this;
            }
            return null;
        }

        /// <summary>
        ///     Add a menu component to this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component</param>
        public void Add(AMenuComponent component)
        {
            component.Parent = this;
            components.Add(component.Name, component);
        }

        /// <summary>
        ///     Removes a menu component from this menu.
        /// </summary>
        /// <param name="component"><see cref="AMenuComponent" /> component instance</param>
        public void Remove(AMenuComponent component)
        {
            if (components.ContainsKey(component.Name))
            {
                component.Parent = null;
                components.Remove(component.Name);
            }
        }

        /// <summary>
        ///     Menu Drawing callback.
        /// </summary>
        public override void OnDraw(Vector2 position, int index)
        {
            if (!Position.Equals(position))
            {
                Position = position;
            }

            SkinIndex.Skin[Configuration.GetValidMenuSkin()].OnMenuDraw(MenuContainer, position, index);
        }

        /// <summary>
        ///     Menu Windows Process Messages callback.
        /// </summary>
        /// <param name="args"></param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (UI.Root.MenuVisible && Visible)
            {
                if (args.Cursor.IsUnderRectangle(Position.X, Position.Y, DefaultSettings.ContainerWidth, DefaultSettings.ContainerHeight))
                {
                    if (args.Msg == WindowsMessages.LBUTTONDOWN)
                    {
                        MenuContainer.Toggled = Toggled = !Toggled;
                    }
                }
            }
        }

        /// <summary>
        ///     Menu Update callback.
        /// </summary>
        public override void OnUpdate() {}
    }

    /// <summary>
    ///     Menu Information Container.
    /// </summary>
    public class MenuContainer
    {
        /// <summary>
        ///     Menu Display Name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Returns if the menu was toggled.
        /// </summary>
        public bool Toggled { get; set; }

        /// <summary>
        ///     Menu Components
        /// </summary>
        public IDictionary<string, AMenuComponent> Components { get; set; }
    }
}