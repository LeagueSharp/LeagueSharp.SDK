using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Events;
using LeagueSharp.CommonEx.Core.Wrappers;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Spells class, contains all current game champion spells.
    /// </summary>
    public static class Spells
    {
        /// <summary>
        ///     Saved Spell List
        /// </summary>
        private static readonly Dictionary<Obj_AI_Hero, SpellDataWrapper[]> SpellsList =
            new Dictionary<Obj_AI_Hero, SpellDataWrapper[]>();

        /// <summary>
        ///     Static Constructor
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

        /// <summary>
        ///     Returns the spells from the specific unit
        /// </summary>
        /// <param name="hero">Unit</param>
        /// <returns>SpellData Wrapper Array</returns>
        public static SpellDataWrapper[] GetSpells(this Obj_AI_Hero hero)
        {
            return (SpellsList.ContainsKey(hero)) ? SpellsList[hero] : null;
        }

        /// <summary>
        ///     Returns a specific spell from the specific unit
        /// </summary>
        /// <param name="hero">Unit</param>
        /// <param name="slot">SpellSlot</param>
        /// <returns>SpellData Wrapper</returns>
        public static SpellDataWrapper GetSpell(this Obj_AI_Hero hero, SpellSlot slot)
        {
            if (SpellsList.ContainsKey(hero))
            {
                foreach (var container in SpellsList[hero].Where(container => container.Slot == slot))
                {
                    return container;
                }
            }
            return new SpellDataWrapper(hero, slot);
        }
    }
}