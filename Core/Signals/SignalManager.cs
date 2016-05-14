// <copyright file="SignalManager.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Signals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Manages Signals.
    /// </summary>
    internal static class SignalManager
    {
        #region Static Fields

        /// <summary>
        ///     Signals list.
        /// </summary>
        private static readonly List<Signal> Signals = new List<Signal>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="SignalManager" /> class.
        /// </summary>
        static SignalManager()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public static void AddSignal(Signal signal)
        {
            if (!Signals.Contains(signal))
            {
                Signals.Add(signal);
            }
        }

        /// <summary>
        ///     Removes a signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public static void RemoveSignal(Signal signal)
        {
            if (Signals.Contains(signal))
            {
                Signals.Remove(signal);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Game Update tick subscribe.
        /// </summary>
        /// <param name="args">
        ///     Event data
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var signal in Signals.Where(x => x.Enabled).ToList())
            {
                if (signal.SignalWaver(signal))
                {
                    signal.TriggerSignal(MethodBase.GetCurrentMethod().Name, "Signal was waved.");
                    Signals.Remove(signal);
                }

                if (signal.Expired && signal.CalledExpired == false)
                {
                    signal.TriggerOnExipired(MethodBase.GetCurrentMethod().Name);
                    signal.CalledExpired = true;
                }
            }
        }

        #endregion
    }
}