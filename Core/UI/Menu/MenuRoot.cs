namespace LeagueSharp.CommonEx.Core.UI
{
    public class MenuRoot : Menu
    {
        public MenuRoot(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString) {}

        /// <summary>
        ///     Attaches the menu towards the main menu.
        /// </summary>
        /// <returns>Menu Instance</returns>
        public Menu Attach()
        {
            MenuManager.Instance.Add(this);
            return this;
        }
    }
}