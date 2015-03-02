using System;
using System.Linq;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides events for OnStealth
    /// </summary>
    public class Stealth
    {
        /// <summary>
        ///     Delegate for the <see cref="OnStealth" /> event.
        /// </summary>
        /// <param name="sender"></param>
        public delegate void OnStealthDelegate(Obj_AI_Hero sender);

        static Stealth()
        {
            Game.OnGameUpdate += GameOnOnGameUpdate;
        }

        /// <summary>
        ///     Gets fired when any hero is invisible.
        /// </summary>
        public static event OnStealthDelegate OnStealth;

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            foreach (var hero in ObjectHandler.AllHeroes.Where(hero => hero.HasBuffOfType(BuffType.Invisibility)))
            {
                FireOnStealth(hero);
            }
        }

        private static void FireOnStealth(Obj_AI_Hero sender)
        {
            var handler = OnStealth;

            if (handler != null)
            {
                handler(sender);
            }
        }
    }
}