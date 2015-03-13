#region

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Performance class, measures how much time does a function takes to execute.
    /// </summary>
    /// <example>
    ///     Performance.MeasureMilliseconds(() => Game.PrintChat("Measure Milliseconds!"));
    /// </example>
    public class CallbackPerformance
    {
        /// <summary>
        ///     Measures and returns the elapsed ticks the function takes.
        ///     (Caution: This will execute the function in realtime)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Ticks the function took (long-units)</returns>
        public static long MeasureTicks(Action funcCallback,
            int iterations = 1,
            [CallerMemberName] string memberName = "")
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                for (var i = 0; i < iterations; ++i)
                {
                    funcCallback();
                }

                stopwatch.Stop();

                Logging.Write()(
                    LogLevel.Info, "{0} has taken {1} elapsed ticks to execute, and was executed successfuly.",
                    memberName, stopwatch.ElapsedTicks);

                return stopwatch.ElapsedTicks;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error, "{0} had an error during execution and was unable to be measured.", memberName);
                return -1L;
            }
        }

        /// <summary>
        ///     Measures and returns the elapsed milliseconds the function takes.
        ///     (Caution: This will execute the function in realtime)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Milliseconds the function took (long-units)</returns>
        public static long MeasureMilliseconds(Action funcCallback,
            int iterations = 1,
            [CallerMemberName] string memberName = "")
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                for (var i = 0; i < iterations; ++i)
                {
                    funcCallback();
                }

                stopwatch.Stop();

                Logging.Write()(
                    LogLevel.Info, "{0} has taken {1} elapsed milliseconds to execute, and was executed successfuly.",
                    memberName, stopwatch.ElapsedMilliseconds);

                return stopwatch.ElapsedMilliseconds;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error, "{0} had an error during execution and was unable to be measured.", memberName);
                return -1L;
            }
        }

        /// <summary>
        ///     Measures and returns the elapsed time span the function takes.
        ///     (Caution: This will execute the function in realtime)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Time Span the function took (long-units)</returns>
        public static TimeSpan MeasureTimeSpan(Action funcCallback,
            int iterations = 1,
            [CallerMemberName] string memberName = "")
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                for (var i = 0; i < iterations; ++i)
                {
                    funcCallback();
                }

                stopwatch.Stop();

                Logging.Write()(
                    LogLevel.Info, "{0} has taken {1} elapsed time span to execute, and was executed successfuly.",
                    memberName, stopwatch.Elapsed);

                return stopwatch.Elapsed;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error, "{0} had an error during execution and was unable to be measured.", memberName);
                return TimeSpan.Zero;
            }
        }
    }
}