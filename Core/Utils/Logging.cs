#region

using System;
using System.IO;
using System.Runtime.CompilerServices;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Logging class for LeagueSharp.CommonEx, used to log output data into a file.
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
        /// <returns></returns>
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
                if (!Directory.Exists(Constants.LogDir))
                {
                    Directory.CreateDirectory(Constants.LogDir);
                }

                var path = Path.Combine(Constants.LeagueSharpDirectory, "Logs", Constants.LogFileName);

                using (var writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(format);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }

    /// <summary>
    ///     The level of the information being loggged
    /// </summary>
    public enum LogLevel
    {
        Debug = 2,
        Error = 5,
        Fatal = 6,
        Info = 1,
        Trace = 3,
        Warn = 4
    }
}