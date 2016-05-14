// <copyright file="TickOperation.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDKEx.Utils
{
    using System;

    /// <summary>
    ///     Executes an operation each set amount of ticks.
    /// </summary>
    public sealed class TickOperation : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Contains the next tick value that Action should be executed.
        ///     <seealso cref="Action" />
        /// </summary>
        private int nextTick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TickOperation" /> class.
        ///     Constructor for a new Tick Operation instance, auto-starts by default.
        /// </summary>
        /// <param name="tickDelay">
        ///     A set delay between ticks the action should be executed.
        /// </param>
        /// <param name="action">
        ///     The executed action.
        /// </param>
        /// <param name="runOnce">
        ///     Should executed action be ran(executed) at least once.
        /// </param>
        public TickOperation(int tickDelay, Action action, bool runOnce = false)
        {
            this.Action = action;
            this.TickDelay = tickDelay;

            this.nextTick = runOnce ? Variables.TickCount : Variables.TickCount + tickDelay;
            this.IsRunning = true;
            Game.OnUpdate += this.OnTick;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Executed Action.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is the Tick Operation is currently running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        ///     Gets or sets a delay between ticks that action should be executed.
        /// </summary>
        public int TickDelay { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposal of the Tick Operation.
        /// </summary>
        public void Dispose()
        {
            if (this.IsRunning)
            {
                Game.OnUpdate -= this.OnTick;
            }

            this.Action = null;
            this.TickDelay = 0;
            this.nextTick = 0;
            this.IsRunning = false;
        }

        /// <summary>
        ///     Starts the tick operation.
        /// </summary>
        /// <param name="runOnce">Should executed action be ran(executed) at least once.</param>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Start(bool runOnce = false)
        {
            if (!this.IsRunning)
            {
                Game.OnUpdate += this.OnTick;
            }

            return this;
        }

        /// <summary>
        ///     Stops the tick operation.
        /// </summary>
        /// <returns>Tick Operation instance.</returns>
        public TickOperation Stop()
        {
            if (this.IsRunning)
            {
                Game.OnUpdate -= this.OnTick;
            }

            return this;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Notified function per game tick by Game.OnGameUpdate event.
        ///     Executes the action if met tick requirements.
        /// </summary>
        /// <param name="args"><see cref="System.EventArgs" /> event data</param>
        private void OnTick(EventArgs args)
        {
            if (this.nextTick <= Variables.TickCount)
            {
                this.Action();

                this.nextTick = Variables.TickCount + this.TickDelay;
            }
        }

        #endregion
    }
}