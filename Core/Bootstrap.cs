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
            Orbwalker.Active = Orbwalker.Attack = Orbwalker.Move = true;
        }
    }
}