// <copyright file="SpellDatabase.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers.Spells.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     The spell database.
    /// </summary>
    [ResourceImport]
    public static class Database
    {
        #region Static Fields

        /// <summary>
        ///     A list of all the entries in the SpellDatabase.
        /// </summary>
        [ResourceImport("Data.Database.json")]
        public static List<DatabaseEntry> Spells = new List<DatabaseEntry>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Queries a search through the spell collection, collecting the values with the predicate function.
        /// </summary>
        /// <param name="predicate">
        ///     The predicate function.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> collection of <see cref="DatabaseEntry" />.
        /// </returns>
        public static IEnumerable<DatabaseEntry> Get(Func<DatabaseEntry, bool> predicate = null)
        {
            return predicate == null ? Spells : Spells.Where(predicate);
        }

        /// <summary>
        ///     Queries a search through the spell collection by missile name.
        /// </summary>
        /// <param name="missileSpellName">The missile spell name.</param>
        /// <returns>
        ///     The <see cref="DatabaseEntry" />
        /// </returns>
        public static DatabaseEntry GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            return
                Spells.FirstOrDefault(
                    spellData =>
                    (spellData.MissileSpellName?.ToLower() == missileSpellName)
                    || spellData.ExtraMissileNames.Contains(missileSpellName));
        }

        /// <summary>
        ///     Queries a search through the spell collection by spell name.
        /// </summary>
        /// <param name="spellName">The spell name.</param>
        /// <returns>
        ///     The <see cref="DatabaseEntry" />
        /// </returns>
        public static DatabaseEntry GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            return
                Spells.FirstOrDefault(
                    spellData =>
                    spellData.SpellName.ToLower() == spellName || spellData.ExtraSpellNames.Contains(spellName));
        }

        #endregion
    }
}