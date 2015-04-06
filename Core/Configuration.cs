using LeagueSharp.CommonEx.Core.UI.Skins;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     LeagueSharp CommonEx Configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        ///     The skin of the menu.
        /// </summary>
        public static int MenuSkin = 0;

        /// <summary>
        ///     Returns a valid menu skin id.
        /// </summary>
        /// <returns>Returns a valid menu skin id.</returns>
        public static int GetValidMenuSkin()
        {
            return MenuSkin < SkinIndex.Skin.Length ? MenuSkin : 0;
        }
    }
}