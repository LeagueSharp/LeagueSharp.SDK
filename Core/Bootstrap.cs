using System.Threading.Tasks;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI;
using LeagueSharp.CommonEx.Core.UI.Values;
using LeagueSharp.CommonEx.Core.Utils;
using LeagueSharp.CommonEx.Core.Wrappers;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Bootstrap is an init pointer for the AppDomainManager to initialize the library correctly once loaded in game.
    /// </summary>
    public class Bootstrap
    {
        /// <summary>
        ///     External attachment handle for the AppDomainManager
        /// </summary>
        public static void Init()
        {
            // Load the Damage class async.
            Task.Factory.StartNew(Damage.LoadDamage)
                .ContinueWith(task => Logging.Write()(LogLevel.Info, "Damage loaded!"));

            // Load the menu
            var menu = new Menu("menutest", "Menu Test", true).AttachMenu();
            menu.Add(new MenuItem<MenuBool>("booltest", "Bool Test #1"));
            var menu2 = new Menu("menutest2", "Menu Test #2", true).AttachMenu();
            Root.Init();

            // Log all of the exceptions
            Logging.LogAllExceptions();
        }
    }
}