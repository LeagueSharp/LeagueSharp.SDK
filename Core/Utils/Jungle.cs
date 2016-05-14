// <copyright file="Jungle.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Utils
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     The jungle utility class, provides utils for jungle related items.
    /// </summary>
    public static class Jungle
    {
        #region Static Fields

        /// <summary>
        ///     The large name regex list.
        /// </summary>
        private static readonly string[] LargeNameRegex =
            {
                "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
                "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
                "SRU_Krug[0-9]{1,}"
            };

        /// <summary>
        ///     The legendary name regex list.
        /// </summary>
        private static readonly string[] LegendaryNameRegex = { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        /// <summary>
        ///     The small name regex list.
        /// </summary>
        private static readonly string[] SmallNameRegex = { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the minion jungle type.
        /// </summary>
        /// <param name="minion">
        ///     The minion
        /// </param>
        /// <returns>
        ///     The <see cref="JungleType" />
        /// </returns>
        public static JungleType GetJungleType(this Obj_AI_Minion minion)
        {
            if (SmallNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Small;
            }

            if (LargeNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Large;
            }

            if (LegendaryNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Legendary;
            }

            return JungleType.Unknown;
        }

        /// <summary>
        ///     Indicates whether the object is a jungle buff carrier.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsJungleBuff(this Obj_AI_Minion minion)
        {
            var @base = minion.CharData.BaseSkinName;
            return @base.Equals("SRU_Blue") || @base.Equals("SRU_Red");
        }

        #endregion
    }
}