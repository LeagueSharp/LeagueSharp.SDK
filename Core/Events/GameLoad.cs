#region

using System;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides an event for when the game starts.
    /// </summary>
    public class GameLoad
    {
        /// <summary>
        ///     Static constructor.
        /// </summary>
        static GameLoad()
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
        public static event Action<EventArgs> OnGameLoad;

        /// <summary>
        ///     Internal event that is called when the game starts or is already running (when you get ingame).
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnGameStart(EventArgs args)
        {
            if (OnGameLoad != null)
            {
                OnGameLoad(new EventArgs());
            }
        }
    }
}