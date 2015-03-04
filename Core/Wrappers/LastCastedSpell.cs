using System.Collections.Generic;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Holds information about the last casted spell a unit did.
    /// </summary>
    public class LastCastedSpellEntry
    {
        /// <summary>
        ///     End time of the cast.
        /// </summary>
        public float EndTime;

        /// <summary>
        ///     Gets if the spell data is valid, and not empty.
        /// </summary>
        public bool IsValid;

        /// <summary>
        ///     The name of the spell last casted.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The <see cref="SpellData" /> of the spell casted.
        /// </summary>
        public SpellData SpellData;

        /// <summary>
        ///     Start time of the cast.
        /// </summary>
        public float StartTime;

        /// <summary>
        ///     Target
        /// </summary>
        public Obj_AI_Base Target;

        /// <summary>
        ///     Internal Constructor for Last Casted Spell Entry.
        /// </summary>
        /// <param name="args">Processed Casted Spell Data</param>
        internal LastCastedSpellEntry(GameObjectProcessSpellCastEventArgs args)
        {
            Name = args.SData.Name;
            Target = (Obj_AI_Base) args.Target;
            StartTime = args.TimeCast;
            EndTime = args.TimeSpellEnd;
            SpellData = args.SData;
            IsValid = true;
        }

        /// <summary>
        ///     Internal Constructor for Last Casted Spell Entry.
        /// </summary>
        internal LastCastedSpellEntry()
        {
            Name = string.Empty;
            Target = null;
            StartTime = 0;
            EndTime = 0;
            SpellData = null;
            IsValid = false;
        }
    }

    /// <summary>
    ///     Extension for getting the last casted spell of an <see cref="Obj_AI_Hero" />
    /// </summary>
    public static class LastCastedSpell
    {
        /// <summary>
        ///     Casted Spells of the champions
        /// </summary>
        internal static readonly Dictionary<int, LastCastedSpellEntry> CastedSpells =
            new Dictionary<int, LastCastedSpellEntry>();

        /// <summary>
        ///     Static constructor
        /// </summary>
        static LastCastedSpell()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        /// <summary>
        ///     Function that is called by the OnProcessSpellCast event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Hero))
            {
                var entry = new LastCastedSpellEntry(args);
                if (!CastedSpells.ContainsKey(sender.NetworkId))
                {
                    CastedSpells.Add(sender.NetworkId, entry);
                    return;
                }

                CastedSpells[sender.NetworkId] = entry;
            }
        }

        /// <summary>
        ///     Gets the <see cref="LastCastedSpellEntry" /> of the unit.
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>
        ///     <see cref="LastCastedSpellEntry" />
        /// </returns>
        public static LastCastedSpellEntry GetLastCastedSpell(this Obj_AI_Hero target)
        {
            LastCastedSpellEntry entry;
            var contains = CastedSpells.TryGetValue(target.NetworkId, out entry);

            return contains ? entry : new LastCastedSpellEntry();
        }
    }
}