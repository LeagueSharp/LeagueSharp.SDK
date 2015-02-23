#region

using System;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.Events
{
    public class OnGameLoad
    {
        /// <summary>
        ///     Static constructor.
        /// </summary>
        static OnGameLoad()
        {
            if (Game.Mode == GameMode.Running)
            {
                DelayAction.Add(0, () => Game_OnGameStart(new EventArgs()));
            }
            else
            {
                Game.OnGameStart += Game_OnGameStart;
            }
        }

        /// <summary>
        ///     OnGameLoad is getting called when you get ingame (doesn't matter if started or restarted while game is already
        ///     running) and when reloading an assembly.
        /// </summary>
        public static event Action<EventArgs> EOnGameLoad;

        private static void Game_OnGameStart(EventArgs args)
        {
            if (EOnGameLoad != null)
            {
                EOnGameLoad(new EventArgs());
            }
        }
    }
}