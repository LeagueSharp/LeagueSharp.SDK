namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     The level of the information being loggged
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     Debug Information
        /// </summary>
        Debug = 2,

        /// <summary>
        ///     An error occured somewhere in the code(exception)
        /// </summary>
        Error = 5,

        /// <summary>
        ///     An error occured and the program is unable to proceed
        /// </summary>
        Fatal = 6,

        /// <summary>
        ///     General information
        /// </summary>
        Info = 1,

        /// <summary>
        ///     Current location of the program
        /// </summary>
        Trace = 3,

        /// <summary>
        ///     Warning
        /// </summary>
        Warn = 4
    }
}