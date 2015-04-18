#region

using System;
using System.Collections.Generic;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Delays actions by a set time.
    /// </summary>
    public class DelayAction
    {
        /// <summary>
        ///     Action List.
        /// </summary>
        public static List<KeyValuePair<Action, int>> ActionList = new List<KeyValuePair<Action, int>>();

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static DelayAction()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        /// <summary>
        ///     OnGameUpdate called event (per game-tick).
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            for (var i = ActionList.Count - 1; i >= 0; i--)
            {
                if (ActionList[i].Value > Variables.TickCount)
                {
                    continue;
                }

                try
                {
                    if (ActionList[i].Key != null)
                    {
                        ActionList[i].Key();
                    }
                }
                catch (Exception)
                {
                    // Ignored exception.
                }

                ActionList.RemoveAt(i);
            }
        }

        /// <summary>
        ///     Add a new delayed action.
        /// </summary>
        /// <param name="time">Delayed Time</param>
        /// <param name="func">Callback Function</param>
        public static void Add(int time, Action func)
        {
            ActionList.Add(new KeyValuePair<Action, int>(func, time + Variables.TickCount));
        }
    }
}