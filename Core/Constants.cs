// <copyright file="Constants.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK
{
    using System;
    using System.IO;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     Constant values of the LeagueSharp development kit.
    /// </summary>
    public static class Constants
    {
        #region Static Fields

        /// <summary>
        ///     LeagueSharp Application Data folder.
        /// </summary>
        public static readonly string LeagueSharpAppData =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LS" + Environment.UserName.GetHashCode().ToString("X"));

        /// <summary>
        ///     LeagueSharp SDK Log directory.
        /// </summary>
        public static readonly string LogDirectory = Path.Combine(LeagueSharpAppData, "Logs", "SDK");

        /// <summary>
        ///     LeagueSharp SDK Session Log file name.
        /// </summary>
        public static readonly string LogFileName = DateTime.Now.ToString("d").Replace('/', '-') + ".log";

        /// <summary>
        ///     LeagueSharp SDK Font.
        /// </summary>
        private static Font leagueSharpFont;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the league sharp font.
        /// </summary>
        public static Font LeagueSharpFont
        {
            get
            {
                if (leagueSharpFont != null && !leagueSharpFont.IsDisposed)
                {
                    return leagueSharpFont;
                }

                return
                    leagueSharpFont =
                    new Font(
                        Drawing.Direct3DDevice,
                        14,
                        0,
                        FontWeight.DoNotCare,
                        0,
                        false,
                        FontCharacterSet.Default,
                        FontPrecision.Default,
                        FontQuality.Antialiased,
                        FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative,
                        "Tahoma");
            }
        }

        #endregion
    }
}