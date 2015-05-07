using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeagueSharp.CommonEx.Core.Signals
{
    /// <summary>
    ///     Manages Signals.
    /// </summary>
    internal static class SignalManager
    {
        private static readonly List<Signal> Signals = new List<Signal>();

        static SignalManager()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

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
        /// Removes a signal.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public static void RemoveSignal(Signal signal)
        {
            if (Signals.Contains(signal))
            {
                Signals.Remove(signal);
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            foreach (var signal in Signals.Where(x => x.Enabled))
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
    }
}