// <copyright file="DamageLibrary.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Properties

        /// <summary>
        ///     Gets the Damage Collection.
        /// </summary>
        private static IDictionary<string, ChampionDamage> DamageCollection { get; } =
            new Dictionary<string, ChampionDamage>();

        #endregion

        #region Methods

        /// <summary>
        ///     Creates the damage collection.
        /// </summary>
        /// <param name="damages">
        ///     The converted <see cref="byte" />s of damages into a dictionary collection.
        /// </param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void CreateDamages(IDictionary<string, JToken> damages)
        {
            foreach (var champion in GameObjects.Heroes.Select(h => h.ChampionName))
            {
                JToken value;
                if (damages.TryGetValue(champion, out value))
                {
                    DamageCollection.Add(champion, JsonConvert.DeserializeObject<ChampionDamage>(value.ToString()));
                }
            }
        }

        /// <summary>
        ///     Gets the passive raw damage summary.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double GetPassiveDamageSum(this Obj_AI_Hero source)
        {
            return 0d;
        }

        #endregion
    }
}