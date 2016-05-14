// <copyright file="CallbackPerformance.cs" company="LeagueSharp">
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
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using LeagueSharp.SDKEx.Enumerations;

    /// <summary>
    ///     Performance class, measures how much time does a function takes to execute.
    /// </summary>
    /// <example>
    ///     Performance.MeasureMilliseconds(() => Console.WriteLine("Measure Milliseconds!"));
    /// </example>
    public class CallbackPerformance
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Measures and returns the elapsed milliseconds the function takes.
        ///     (Caution: This will execute the function in real-time)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Milliseconds the function took (long-units)</returns>
        public static long MeasureMilliseconds(
            Action funcCallback,
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
                    LogLevel.Info,
                    "{0} has taken {1} elapsed milliseconds to execute, and was executed successfuly.",
                    memberName,
                    stopwatch.ElapsedMilliseconds);

                return stopwatch.ElapsedMilliseconds;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error,
                    "{0} had an error during execution and was unable to be measured.",
                    memberName);
                return -1L;
            }
        }

        /// <summary>
        ///     Measures and returns the elapsed ticks the function takes.
        ///     (Caution: This will execute the function in real-time)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Ticks the function took (long-units)</returns>
        public static long MeasureTicks(
            Action funcCallback,
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
                    LogLevel.Info,
                    "{0} has taken {1} elapsed ticks to execute, and was executed successfuly.",
                    memberName,
                    stopwatch.ElapsedTicks);

                return stopwatch.ElapsedTicks;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error,
                    "{0} had an error during execution and was unable to be measured.",
                    memberName);
                return -1L;
            }
        }

        /// <summary>
        ///     Measures and returns the elapsed time span the function takes.
        ///     (Caution: This will execute the function in real-time)
        /// </summary>
        /// <param name="funcCallback">Function to be executed and measured</param>
        /// <param name="iterations">Number of Times to run the callback</param>
        /// <param name="memberName">Member name of the function that called the measurement request.</param>
        /// <returns>Elapsed Time Span the function took (long-units)</returns>
        public static TimeSpan MeasureTimeSpan(
            Action funcCallback,
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
                    LogLevel.Info,
                    "{0} has taken {1} elapsed time span to execute, and was executed successfuly.",
                    memberName,
                    stopwatch.Elapsed);

                return stopwatch.Elapsed;
            }
            catch (Exception)
            {
                Logging.Write()(
                    LogLevel.Error,
                    "{0} had an error during execution and was unable to be measured.",
                    memberName);
                return TimeSpan.Zero;
            }
        }

        #endregion
    }
}