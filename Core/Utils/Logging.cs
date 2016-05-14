// <copyright file="Logging.cs" company="LeagueSharp">
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
    using System.IO;
    using System.Runtime.CompilerServices;

    using LeagueSharp.SDKEx.Enumerations;

    /// <summary>
    ///     Logging class for LeagueSharp.SDKEx, used to log output data into a file and the console.
    /// </summary>
    public class Logging
    {
        #region Delegates

        /// <summary>
        ///     Write Delegate, used to state the logging data.
        /// </summary>
        /// <param name="logLevel">
        ///     Level of the log
        /// </param>
        /// <param name="value">
        ///     Value or Message Format
        /// </param>
        /// <param name="args">
        ///     Message Format Arguments
        /// </param>
        public delegate void WriteDelegate(LogLevel logLevel, object value, params object[] args);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Execute a logging write through the Write Delegate.
        /// </summary>
        /// <param name="logToFile">
        ///     Write logging data to file. (Optional)
        /// </param>
        /// <param name="printColor">
        ///     Print to Console with colors. (Optional)
        /// </param>
        /// <param name="memberName">
        ///     Function name (Auto / Optional)
        /// </param>
        /// <see cref="WriteDelegate" />
        /// <example>Write()(LogLevel.Debug, "I am a debug, arguments: {0}, {1}.", "argument1", 123);</example>
        /// <returns>Created WriteDelegate</returns>
        public static WriteDelegate Write(
            bool logToFile = false,
            bool printColor = true,
            [CallerMemberName] string memberName = "")
        {
            return (logLevel, value, args) =>
                {
                    var finalMessage = value;
                    if (args.Length > 0)
                    {
                        try
                        {
                            finalMessage = string.Format(value.ToString(), args);
                        }
                        catch (Exception)
                        {
                            // Ignored.
                        }
                    }

                    Write(logLevel, finalMessage.ToString(), logToFile, printColor, memberName);
                };
        }

        /// <summary>
        ///     Execute a logging write through the Write Delegate.
        /// </summary>
        /// <param name="logLevel">
        ///     Level of the log
        /// </param>
        /// <param name="value">
        ///     Value or Message Format
        /// </param>
        /// <param name="args">
        ///     Message Format Arguments
        /// </param>
        /// <param name="logToFile">
        ///     Write logging data to file. (Optional)
        /// </param>
        /// <param name="printColor">
        ///     Print to Console with colors. (Optional)
        /// </param>
        /// <param name="memberName">
        ///     Function name (Auto / Optional)
        /// </param>
        /// <see cref="WriteDelegate" />
        /// <example>Write(LogLevel.Debug, "I am a debug, arguments: {0}, {1}.", new object[] { "argument1", 123 });</example>
        public static void Write(
            LogLevel logLevel,
            object value,
            object[] args,
            bool logToFile = false,
            bool printColor = true,
            [CallerMemberName] string memberName = "")
        {
            var finalMessage = value;
            if (args.Length > 0)
            {
                try
                {
                    finalMessage = string.Format(value.ToString(), args);
                }
                catch (Exception)
                {
                    // Ignored.
                }
            }

            Write(logLevel, finalMessage.ToString(), logToFile, printColor, memberName);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Logs all exceptions that happen in the current AppDomain.
        /// </summary>
        internal static void LogAllExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var exception = args.ExceptionObject as Exception;
                    if (exception != null)
                    {
                        Write(true)(LogLevel.Error, exception.Message);
                    }
                };
        }

        /// <summary>
        ///     Logs information to Console(always), and optionally logs it to the logging file.
        /// </summary>
        /// <param name="logLevel">
        ///     Level of the log
        /// </param>
        /// <param name="message">
        ///     Message Format
        /// </param>
        /// <param name="logToFile">
        ///     Write logging data to file.
        /// </param>
        /// <param name="printColor">
        ///     Print to Console with colors.
        /// </param>
        /// <param name="memberName">
        ///     Function name
        /// </param>
        private static void Write(LogLevel logLevel, string message, bool logToFile, bool printColor, string memberName)
        {
            var format = $"[{DateTime.Now.TimeOfDay} - {logLevel}]: ({memberName}) -> {message}";

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

            if (!logToFile && (int)logLevel < 3)
            {
                return;
            }

            try
            {
                if (!Directory.Exists(Constants.LogDirectory))
                {
                    Directory.CreateDirectory(Constants.LogDirectory);
                }

                using (var writer = new StreamWriter(Path.Combine(Constants.LogDirectory, Constants.LogFileName), true))
                {
                    writer.WriteLine(format);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        #endregion
    }
}