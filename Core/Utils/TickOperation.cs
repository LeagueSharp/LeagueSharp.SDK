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
        private long nextTick;

        /// <summary>
        ///     Constructor for a new Tick Operation instance.
        /// </summary>
        /// <param name="tickDelay">A set delay between ticks the action should be executed.</param>
        /// <param name="action">The executed action.</param>
        /// <param name="runOnce">Should executed action be ran(executed) at least once.</param>
        public TickOperation(int tickDelay, Action action, bool runOnce = false)
        {
            Action = action;
            TickDelay = tickDelay;

            nextTick = Utils.TickCount + tickDelay;

            Start(runOnce);
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
        ///     Disposal of the Tick Operation.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Game.OnGameUpdate -= OnTick;
            }
            catch (Exception)
            {
                // Ignored Exception
            }
            Action = null;
            TickDelay = 0;
            nextTick = 0L;
        }

        /// <summary>
        ///     Finalization of the Tick Operation.
        /// </summary>
        ~TickOperation()
        {
            try
            {
                Game.OnGameUpdate -= OnTick;
            }
            catch (Exception)
            {
                // Ignored Exception
            }
        }

        /// <summary>
        ///     Starts the tick operation.
        /// </summary>
        /// <param name="runOnce">Should executed action be ran(executed) at least once.</param>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Start(bool runOnce = false)
        {
            Game.OnGameUpdate += OnTick;
            return this;
        }

        /// <summary>
        ///     Notified function per game tick by Game.OnGameUpdate event.
        ///     Executes the action if met tick requirements.
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private void OnTick(EventArgs args)
        {
            if (nextTick <= Utils.TickCount)
            {
                Action();
                nextTick = Utils.TickCount + TickDelay;
            }
        }

        /// <summary>
        ///     Stops the tick operation.
        /// </summary>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Stop()
        {
            try
            {
                Game.OnGameUpdate -= OnTick;
            }
            catch (Exception)
            {
                // Ignored Exception
            }
            return this;
        }
    }
}