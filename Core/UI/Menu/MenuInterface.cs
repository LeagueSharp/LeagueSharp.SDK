using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Interface class, used to control the menu.
    /// </summary>
    public static class MenuInterface
    {
        /// <summary>
        ///     Root Menu(Components), contains the list of the menu that are attached to the root.
        /// </summary>
        public static readonly List<AMenuComponent> RootMenuComponents = new List<AMenuComponent>();

        /// <summary>
        ///     Sends a drawing request towards the menu, happens on an OnDraw present.
        /// </summary>
        public static void OnDraw()
        {
            SkinIndex.Skin[Configuration.GetValidMenuSkin()].OnDraw(MenuSettings.Position);
        }

        /// <summary>
        ///     Sends a windows process message towards the menu.
        /// </summary>
        /// <param name="keys"></param>
        public static void OnWndProc(WindowsKeys keys)
        {
            foreach (var component in RootMenuComponents)
            {
                component.OnWndProc(keys);
            }
        }

        /// <summary>
        ///     Event is fired when the menu container gets opened.
        /// </summary>
        public static void OnMenuOpen(AMenuComponent component)
        {
            if (component != null)
            {
                foreach (var rootComponent in RootMenuComponents.Where(c => !c.Equals(component)))
                {
                    rootComponent.Toggled = false;
                }
            }
        }

        /// <summary>
        ///     Event is fired when the menu container gets closed.
        /// </summary>
        public static void OnMenuClose(AMenuComponent component) {}
    }
}