#region

using System;
using System.Reflection;
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
        ///     OnLoad Delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        public delegate void OnLoadDelegate(object sender, EventArgs e);

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
        public static event OnLoadDelegate OnLoad;

        /// <summary>
        ///     Internal event that is called when the game starts or is already running (when you get ingame).
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnStart(EventArgs args)
        {
            if (OnLoad != null)
            {
                OnLoad(MethodBase.GetCurrentMethod().DeclaringType, new EventArgs());
            }
        }
    }
}