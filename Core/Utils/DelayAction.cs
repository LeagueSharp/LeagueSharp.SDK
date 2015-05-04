#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        public static List<DelayActionItem> ActionList = new List<DelayActionItem>();

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
            //  Remove all actions that are cancelled.
            ActionList.Where(x => x.Token.IsCancellationRequested).ToList().ForEach(x => ActionList.Remove(x));

            for (var i = ActionList.Count - 1; i >= 0; i--)
            {
                var action = ActionList[i];

                if (action.Time > Variables.TickCount)
                {
                    continue;
                }

                try
                {
                    if (action.Function != null)
                    {
                        action.Function();
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
        ///     Adds a new delayed action.
        /// </summary>
        /// <param name="time">Delayed Time</param>
        /// <param name="func">Callback Function</param>
        public static void Add(int time, Action func)
        {
            ActionList.Add(new DelayActionItem(time, func, new CancellationToken(false)));
        }

        /// <summary>
        ///     Adds a new delayed action, casting the time to an integer.
        /// </summary>
        /// <param name="time">The time(in miliseconds) to call the function.</param>
        /// <param name="func">The function to call once the <paramref name="time" /> has expired.</param>
        public static void Add(float time, Action func)
        {
            ActionList.Add(new DelayActionItem((int) time, func, new CancellationToken(false)));
        }

        /// <summary>
        ///     Adds a new delayed action with a cancelation token. Use the <see cref="CancellationTokenSource" /> class for
        ///     tokens.
        /// </summary>
        /// <param name="time">The time(in miliseconds) to call the function.</param>
        /// <param name="func">The function to call once the <paramref name="time" /> has expired.</param>
        /// <param name="token">The cancelation token.</param>
        public static void Add(int time, Action func, CancellationToken token)
        {
            ActionList.Add(new DelayActionItem(time, func, token));
        }

        /// <summary>
        ///     Adds a new delayed action with a cancelation token. Use the <see cref="CancellationTokenSource" /> class for
        ///     tokens.
        /// </summary>
        /// <param name="time">The time(in miliseconds) to call the function. (Gets casted into an integer)</param>
        /// <param name="func">The function to call once the <paramref name="time" /> has expired.</param>
        /// <param name="token">The cancelation token.</param>
        public static void Add(float time, Action func, CancellationToken token)
        {
            ActionList.Add(new DelayActionItem((int) time, func, token));
        }

        /// <summary>
        ///     Adds a new delayed action.
        /// </summary>
        /// <param name="item">The <see cref="DelayActionItem" /> to add.</param>
        public static void Add(DelayActionItem item)
        {
            ActionList.Add(item);
        }
    }


    /// <summary>
    ///     Class that contains all of the needed information for delaying an action.
    /// </summary>
    public class DelayActionItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DelayActionItem" /> class.
        /// </summary>
        /// <param name="time">The time(in miliseconds) to call the function..</param>
        /// <param name="func">The function to call once the <paramref name="time" /> has expired.</param>
        /// <param name="token">The cancelation token.</param>
        public DelayActionItem(int time, Action func, CancellationToken token)
        {
            Time = time;
            Function = func;
            Token = token;
        }

        /// <summary>
        ///     Gets or sets the time.
        /// </summary>
        /// <value>
        ///     The time.
        /// </value>
        public int Time { get; set; }

        /// <summary>
        ///     Gets or sets the function.
        /// </summary>
        /// <value>
        ///     The function.
        /// </value>
        public Action Function { get; set; }

        /// <summary>
        ///     Gets or sets the cancelation token.
        /// </summary>
        /// <value>
        ///     The cancelation token.
        /// </value>
        /// <example>
        ///     <see cref="CancellationTokenSource" />
        /// </example>
        public CancellationToken Token { get; set; }
    }
}