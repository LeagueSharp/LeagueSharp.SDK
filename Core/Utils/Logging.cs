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
        ///     Logs information to console(always), and optionaly logs it to a file.
        /// </summary>
        /// <param name="level">The level of the information being loggged</param>
        /// <param name="message">Message</param>
        /// <param name="function">Optional, what function the info is coming from</param>
        /// <param name="logToFile">Optional, writes this data to a file</param>
        /// <param name="printColor">Prints pretty color to the console.</param>
        public static void Write(LogLevel level,
            string message,
            [CallerMemberName] string function = "",
            bool logToFile = false,
            bool printColor = true)
        {
            var text = string.Format("[{0} - {1}]: ({2}) -> {3}", DateTime.Now.TimeOfDay, level, function, message);

            var color = Console.ForegroundColor;
            if (printColor)
            {
                switch (level)
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
            }

            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();

            // Default write to file if is warn or higher
            if (!logToFile && (int) level < 3)
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
                    writer.WriteLine(text);
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