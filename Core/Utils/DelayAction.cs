#region

using System;
using System.Collections.Generic;
using System.Threading;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Signals;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Delays actions by a set time.
    /// </summary>
    public class DelayAction
    {
        /// <summary>
        ///     Adds a new delayed action.
        /// </summary>
        /// <param name="time">Delayed Time</param>
        /// <param name="func">Callback Function</param>
        public static void Add(int time, Action func)
        {
            Add(new DelayActionItem(time, func, new CancellationToken(false)));
        }

        /// <summary>
        ///     Adds a new delayed action, casting the time to an integer.
        /// </summary>
        /// <param name="time">The time(in miliseconds) to call the function.</param>
        /// <param name="func">The function to call once the <paramref name="time" /> has expired.</param>
        public static void Add(float time, Action func)
        {
            Add(new DelayActionItem((int) time, func, new CancellationToken(false)));
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
            Add(new DelayActionItem(time, func, token));
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
            Add(new DelayActionItem((int) time, func, token));
        }

        /// <summary>
        ///     Adds a new delayed action.
        /// </summary>
        /// <param name="item">The <see cref="DelayActionItem" /> to add.</param>
        public static void Add(DelayActionItem item)
        {
            Signal.Create(delegate(object sender, Signal.RaisedArgs args)
            {
                var delayActionItem = (DelayActionItem) args.Signal.Properties["DelayActionItem"];

                if (delayActionItem.Token.IsCancellationRequested)
                {
                    return;
                }

                delayActionItem.Function();
            }, delegate(Signal signal)
            {
                var delayActionItem = (DelayActionItem) signal.Properties["DelayActionItem"];
                return Variables.TickCount >= delayActionItem.Time;
            }, default(DateTimeOffset), new Dictionary<string, object> {{ "DelayActionItem", item}});
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
            Time = time + Variables.TickCount;
            Function = func;
            Token = token;
        }

        /// <summary>
        ///     Gets or sets the time the function will be executed at.
        /// </summary>
        /// <value>
        ///     The time the function will be executed at.
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