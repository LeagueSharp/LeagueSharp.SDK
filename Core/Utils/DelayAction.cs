#region

using System;
using System.Collections.Generic;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    public class DelayAction
    {
        /// <summary>
        ///     Callback delegate.
        /// </summary>
        public delegate void Callback();

        /// <summary>
        ///     Action List.
        /// </summary>
        public static List<Action> ActionList = new List<Action>();

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static DelayAction()
        {
            Game.OnGameUpdate += GameOnOnGameUpdate;
        }

        /// <summary>
        ///     OnGameUpdate called event (per game-tick).
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void GameOnOnGameUpdate(EventArgs args)
        {
            for (var i = ActionList.Count - 1; i >= 0; i--)
            {
                if (ActionList[i].Time <= Utils.TickCount)
                {
                    try
                    {
                        if (ActionList[i].CallbackObject != null)
                        {
                            ActionList[i].CallbackObject();
                        }
                    }
                    catch (Exception)
                    {
                        // Ignored exception.
                    }

                    ActionList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        ///     Add a new delayed action.
        /// </summary>
        /// <param name="time">Delayed Time</param>
        /// <param name="func">Callback Function</param>
        public static void Add(int time, Callback func)
        {
            var action = new Action(time, func);
            ActionList.Add(action);
        }

        /// <summary>
        ///     Action Struct.
        /// </summary>
        public struct Action
        {
            /// <summary>
            ///     Callback Object.
            /// </summary>
            public Callback CallbackObject;

            /// <summary>
            ///     Time.
            /// </summary>
            public int Time;

            /// <summary>
            ///     Action Constructor.
            /// </summary>
            /// <param name="time">Time (int)</param>
            /// <param name="callback">Callback</param>
            public Action(int time, Callback callback)
            {
                Time = time + Utils.TickCount;
                CallbackObject = callback;
            }
        }
    }
}