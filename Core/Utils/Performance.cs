// <copyright file="Performance.cs" company="LeagueSharp">
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
    ///     Performance block class, for block method performance logging.
    /// </summary>
    /// <example>
    ///     using(<c>var</c> performance = new Performance())
    ///     {
    ///     Game.PrintChat("Test");
    ///     <c>var</c> elapsedTicks = performance.GetTickCount();
    ///     Logging.Write()("Game.PrintChat took {0} ticks!", elapsedTicks);
    ///     }
    /// </example>
    public class Performance : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Private, string contains the calling member name.
        /// </summary>
        private readonly string memberName;

        /// <summary>
        ///     Private, final performance type to print once block ends.
        /// </summary>
        private readonly PerformanceType performanceType;

        /// <summary>
        ///     Indicates whether to print on dispose operation.
        /// </summary>
        private readonly bool printDispose;

        /// <summary>
        ///     Private, Stopwatch instance, this will track the time it takes to execute functions inside the block.
        /// </summary>
        private Stopwatch stopwatch;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Performance" /> class.
        /// </summary>
        /// <param name="performanceType">
        ///     The performance Type.
        /// </param>
        /// <param name="printDispose">
        ///     The print on dispose operation.
        /// </param>
        /// <param name="memberName">
        ///     The member Name.
        /// </param>
        public Performance(
            PerformanceType performanceType,
            bool printDispose = true,
            [CallerMemberName] string memberName = "")
        {
            this.memberName = memberName;
            this.printDispose = printDispose;
            this.performanceType = performanceType;
            this.stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Performance" /> class.
        /// </summary>
        ~Performance()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposable requirement, redirects to a safe disposable function.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Returns the milliseconds count from the start of the block.
        /// </summary>
        /// <returns>Milliseconds count from the start of the block.</returns>
        public long GetMilliseconds()
        {
            return this.stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        ///     Returns the tick count from the start of the block.
        /// </summary>
        /// <returns>Tick count from the start of the block.</returns>
        public long GetTickCount()
        {
            return this.stopwatch.ElapsedTicks;
        }

        /// <summary>
        ///     Returns the TimeSpan count data from the start of the block.
        /// </summary>
        /// <returns>TimeSpan count data from the start of the block.</returns>
        public TimeSpan GetTimeSpan()
        {
            return this.stopwatch.Elapsed;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Safe Dispose method.
        /// </summary>
        /// <param name="safe">Is Safe (Is not finalized by GC).</param>
        private void Dispose(bool safe)
        {
            if (!safe)
            {
                return;
            }

            this.stopwatch.Stop();

            if (this.printDispose)
            {
                var format = "{0} has taken {1} elapsed ticks to execute, and was executed successfuly.";
                var argument = this.GetTickCount().ToString();

                switch (this.performanceType)
                {
                    case PerformanceType.Milliseconds:
                        format = "{0} has taken {1} elapsed milliseconds to execute, and was executed successfuly.";
                        argument = this.GetMilliseconds().ToString();
                        break;
                    case PerformanceType.TimeSpan:
                        format = "{0} has taken {1} elapsed time span to execute, and was executed successfuly.";
                        argument = this.GetTimeSpan().ToString("g");
                        break;
                }

                Logging.Write()(LogLevel.Info, format, this.memberName, argument);
            }

            this.stopwatch = null;
        }

        #endregion
    }
}