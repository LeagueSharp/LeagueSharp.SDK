#region

using System;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides an event for when the game starts.
    /// </summary>
    public class Load
    {
        /// <summary>
        ///     Static constructor.
        /// </summary>
        static Load()
        {
            if (Game.Mode == GameMode.Running)
            {
                DelayAction.Add(0, () => Game_OnStart(new EventArgs()));
            }
            else
            {
                Game.OnStart += Game_OnStart;
            }
        }

        /// <summary>
        ///     OnLoad is getting called when you get ingame (doesn't matter if started or restarted while game is already
        ///     running) and when reloading an assembly.
        /// </summary>
        public static event Action<EventArgs> OnLoad;

        /// <summary>
        ///     Internal event that is called when the game starts or is already running (when you get ingame).
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnStart(EventArgs args)
        {
            if (OnLoad != null)
            {
                OnLoad(new EventArgs());
            }
        }
    }
}