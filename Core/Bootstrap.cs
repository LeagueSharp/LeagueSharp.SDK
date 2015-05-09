using System.Threading.Tasks;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.UI;
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

            // Log all of the exceptions
            Logging.LogAllExceptions();

            // Create L# menu
            Variables.LeagueSharpMenu = new Menu("LeagueSharp", "LeagueSharp").Attach();

            // Load the orbwalker
            Orbwalker.BootstrapMenu();
        }
    }
}