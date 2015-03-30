#region

using System;
using System.IO;
using System.Runtime.CompilerServices;
using LeagueSharp.CommonEx.Core.Enumerations;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Logging class for LeagueSharp.CommonEx, used to log output data into a file and the console.
    /// </summary>
    public class Logging
    {
        /// <summary>
        ///     Write Delegate, used to state the logging data.
        /// </summary>
        /// <param name="logLevel">Level of the log</param>
        /// <param name="message">Message Format</param>
        /// <param name="args">Message Format Arguments</param>
        public delegate void WriteDelegate(LogLevel logLevel, string message, params object[] args);

        /// <summary>
        ///     Execute a logging write through the Write Delegate.
        /// </summary>
        /// <param name="logToFile">Write logging data to file. (Optional)</param>
        /// <param name="printColor">Print to Console with colors. (Optional)</param>
        /// <param name="memberName">Function name (Auto / Optional)</param>
        /// <see cref="WriteDelegate" />
        /// <example>Write()(LogLevel.Debug, "I am a debug, arguments: {0}, {1}.", "arg1", 123);</example>
        /// <returns>Created WriteDelegate</returns>
        public static WriteDelegate Write(bool logToFile = false,
            bool printColor = true,
            [CallerMemberName] string memberName = "")
        {
            return (logLevel, message, args) =>
            {
                string finalMessage;
                try
                {
                    finalMessage = string.Format(message, args);
                }
                catch (Exception)
                {
                    finalMessage = message;
                }
                Write(logLevel, finalMessage, logToFile, printColor, memberName);
            };
        }

        /// <summary>
        ///     Execute a logging write through the Write Delegate.
        /// </summary>
        /// <param name="logLevel">Level of the log</param>
        /// <param name="message">Message Format</param>
        /// <param name="args">Message Format Arguments</param>
        /// <param name="logToFile">Write logging data to file. (Optional)</param>
        /// <param name="printColor">Print to Console with colors. (Optional)</param>
        /// <param name="memberName">Function name (Auto / Optional)</param>
        /// <see cref="WriteDelegate" />
        /// <example>Write(LogLevel.Debug, "I am a debug, arguments: {0}, {1}.", new object[] { "arg1", 123 });</example>
        public static void Write(LogLevel logLevel,
            string message,
            object[] args,
            bool logToFile = false,
            bool printColor = true,
            [CallerMemberName] string memberName = "")
        {
            Write(logLevel, string.Format(message, args), logToFile, printColor, memberName);
        }

        /// <summary>
        ///     Logs information to Console(always), and optionaly logs it to the logging file.
        /// </summary>
        /// <param name="logLevel">Level of the log</param>
        /// <param name="message">Message Format</param>
        /// <param name="logToFile">Write logging data to file.</param>
        /// <param name="printColor">Print to Console with colors.</param>
        /// <param name="memberName">Function name</param>
        private static void Write(LogLevel logLevel, string message, bool logToFile, bool printColor, string memberName)
        {
            var format = string.Format(
                "[{0} - {1}]: ({2}) -> {3}", DateTime.Now.TimeOfDay, logLevel, memberName, message);

            if (printColor)
            {
                var color = Console.ForegroundColor;
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        color = ConsoleColor.White;
                        break;
                    case LogLevel.Error:
                        color = ConsoleColor.Red;
                        break;
                    case LogLevel.Fatal:
                        color = ConsoleColor.Magenta;
                        break;
                    case LogLevel.Info:
                        color = ConsoleColor.Cyan;
                        break;
                    case LogLevel.Trace:
                        color = ConsoleColor.White;
                        break;
                    case LogLevel.Warn:
                        color = ConsoleColor.Yellow;
                        break;
                }
                Console.ForegroundColor = color;
            }

            Console.WriteLine(format);
            Console.ResetColor();

            if (!logToFile && (int) logLevel < 3)
            {
                return;
            }

            try
            {
                if (!Directory.Exists(Constants.LogDirectory))
                {
                    Directory.CreateDirectory(Constants.LogDirectory);
                }

                using (var writer = new StreamWriter(Constants.LogDirectory, true))
                {
                    writer.WriteLine(format);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Logs all exceptions that happen in the current AppDomain.
        /// </summary>
        internal static void LogAllExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (!(args.ExceptionObject is Exception))
                {
                    return;
                }

                Write(true)(LogLevel.Error, ((Exception) args.ExceptionObject).Message);
            };
        }
    }
}