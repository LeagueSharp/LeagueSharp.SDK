#region

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     Performance block class, for block method performance logging.
    /// </summary>
    /// <example>
    ///     using(var performance = new Performance())
    ///     {
    ///         Game.PrintChat("Test");
    ///         var elapsedTicks = performance.GetTickCount();
    ///         Logging.Write()("Game.PrintChat took {0} ticks!", elapsedTicks);
    ///     }
    /// </example>
    public class Performance : IDisposable
    {
        /// <summary>
        ///     Private, string contains the calling member name.
        /// </summary>
        private readonly string memberName;

        /// <summary>
        ///     Private, final performance type to print once block ends.
        /// </summary>
        private readonly PerformanceType performanceType;

        /// <summary>
        ///     Private, Stopwatch instance, this will track the time it takes to execute functions inside the block.
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        ///     Performance Constructor, starting a new Stopwatch.
        /// </summary>
        public Performance(PerformanceType performanceType, [CallerMemberName] string memberName = "")
        {
            this.memberName = memberName;
            this.performanceType = performanceType;
            stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        ///     Disposable requirement, redirects to a safe disposable function.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Returns the tick count from the start of the block.
        /// </summary>
        /// <returns>Tick count from the start of the block.</returns>
        public long GetTickCount()
        {
            return stopwatch.ElapsedTicks;
        }

        /// <summary>
        ///     Returns the milliseconds count from the start of the block.
        /// </summary>
        /// <returns>Milliseconds count from the start of the block.</returns>
        public long GetMilliseconds()
        {
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        ///     Returns the TimeSpan count data from the start of the block.
        /// </summary>
        /// <returns>TimeSpan count data from the start of the block.</returns>
        public TimeSpan GetTimeSpan()
        {
            return stopwatch.Elapsed;
        }

        /// <summary>
        ///     Finalization Dispose.
        /// </summary>
        ~Performance()
        {
            Dispose(false);
        }

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

            stopwatch.Stop();

            var format = "{0} has taken {1} elapsed ticks to execute, and was executed successfuly.";
            var argument = GetTickCount().ToString();

            switch (performanceType)
            {
                case PerformanceType.Milliseconds:
                    format = "{0} has taken {1} elapsed milliseconds to execute, and was executed successfuly.";
                    argument = GetMilliseconds().ToString();
                    break;
                case PerformanceType.TimeSpan:
                    format = "{0} has taken {1} elapsed time span to execute, and was executed successfuly.";
                    argument = GetTimeSpan().ToString("g");
                    break;
            }

            Logging.Write()(LogLevel.Info, format, memberName, argument);
            stopwatch = null;
        }
    }
}