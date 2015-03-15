using System;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Executes an operation each set amount of ticks.
    /// </summary>
    public class TickOperation : IDisposable
    {
        /// <summary>
        ///     Contains the next tick value that Action should be executed.
        ///     <seealso cref="Action" />
        /// </summary>
        private int nextTick;

        /// <summary>
        ///     Constructor for a new Tick Operation instance, auto-starts by default.
        /// </summary>
        /// <param name="tickDelay">A set delay between ticks the action should be executed.</param>
        /// <param name="action">The executed action.</param>
        /// <param name="runOnce">Should executed action be ran(executed) at least once.</param>
        public TickOperation(int tickDelay, Action action, bool runOnce = false)
        {
            Action = action;
            TickDelay = tickDelay;

            nextTick = (runOnce) ? Variables.TickCount : Variables.TickCount + tickDelay;
            IsRunning = true;
            Game.OnUpdate += OnTick;
        }

        /// <summary>
        ///     Executed Action.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        ///     Set delay between ticks that action should be executed.
        /// </summary>
        public int TickDelay { get; set; }

        /// <summary>
        ///     Returns if the Tick Operation is currently running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        ///     Disposal of the Tick Operation.
        /// </summary>
        public void Dispose()
        {
            if (IsRunning)
            {
                Game.OnUpdate -= OnTick;
            }
            Action = null;
            TickDelay = 0;
            nextTick = 0;
            IsRunning = false;
        }

        /// <summary>
        ///     Starts the tick operation.
        /// </summary>
        /// <param name="runOnce">Should executed action be ran(executed) at least once.</param>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Start(bool runOnce = false)
        {
            if (!IsRunning)
            {
                Game.OnUpdate += OnTick;
            }
            return this;
        }

        /// <summary>
        ///     Notified function per game tick by Game.OnGameUpdate event.
        ///     Executes the action if met tick requirements.
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private void OnTick(EventArgs args)
        {
            if (nextTick <= Variables.TickCount)
            {
                Action();

                nextTick = Variables.TickCount + TickDelay;
            }
        }

        /// <summary>
        ///     Stops the tick operation.
        /// </summary>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Stop()
        {
            if (IsRunning)
            {
                Game.OnUpdate -= OnTick;
            }
            return this;
        }
    }
}