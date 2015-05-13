// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Spells.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Spells class, contains all current game champion spells.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Wrappers;

    /// <summary>
    ///     Spells class, contains all current game champion spells.
    /// </summary>
    public static class Spells
    {
        #region Static Fields

        /// <summary>
        ///     Saved Spell List
        /// </summary>
        private static readonly Dictionary<Obj_AI_Hero, SpellDataWrapper[]> SpellsList =
            new Dictionary<Obj_AI_Hero, SpellDataWrapper[]>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Spells" /> class.
        /// </summary>
        static Spells()
        {
            Load.OnLoad += (sender, e) =>
                {
                    foreach (var obj in ObjectHandler.GetFast<Obj_AI_Hero>())
                    {
                        SpellsList.Add(
                            obj, 
                            new[]
                                {
                                    new SpellDataWrapper(obj, SpellSlot.Q), new SpellDataWrapper(obj, SpellSlot.W), 
                                    new SpellDataWrapper(obj, SpellSlot.E), new SpellDataWrapper(obj, SpellSlot.R)
                                });
                    }
                };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns a specific spell from the specific unit
        /// </summary>
        /// <param name="hero">The unit</param>
        /// <param name="slot">The SpellSlot</param>
        /// <returns>SpellData Wrapper</returns>
        public static SpellDataWrapper GetSpell(this Obj_AI_Hero hero, SpellSlot slot)
        {
            SpellDataWrapper[] value;
            if (SpellsList.TryGetValue(hero, out value))
            {
                foreach (var container in value.Where(container => container.Slot == slot))
                {
                    return container;
                }
            }

            return new SpellDataWrapper(hero, slot);
        }

        /// <summary>
        ///     Returns the spells from the specific unit
        /// </summary>
        /// <param name="hero">The unit</param>
        /// <returns>SpellData Wrapper Array</returns>
        public static SpellDataWrapper[] GetSpells(this Obj_AI_Hero hero)
        {
            SpellDataWrapper[] value;
            return SpellsList.TryGetValue(hero, out value) ? value : null;
        }

        #endregion
    }
}