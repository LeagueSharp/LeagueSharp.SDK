using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Holds information about the last casted spell a unit did.
    /// </summary>
    public class LastCastedSpellEntry
    {
        /// <summary>
        ///     The name of the spell last casted.
        /// </summary>
        public string Name;

        /// <summary>
        ///     Target
        /// </summary>
        public Obj_AI_Base Target;

        /// <summary>
        ///     Start time of the cast.
        /// </summary>
        public float StartTime;

        /// <summary>
        ///     End time of the cast.
        /// </summary>
        public float EndTime;

        /// <summary>
        ///     The <see cref="SpellData"/> of the spell casted.
        /// </summary>
        public SpellData SpellData;

        /// <summary>
        ///     Gets if the spell data is valid, and not empty.
        /// </summary>
        public bool IsValid;

        internal LastCastedSpellEntry(GameObjectProcessSpellCastEventArgs args)
        {
            Name = args.SData.Name;
            Target = (Obj_AI_Base) args.Target;
            StartTime = args.TimeCast;
            EndTime = args.TimeSpellEnd;
            SpellData = args.SData;
            IsValid = true;
        }

        internal LastCastedSpellEntry()
        {
            Name = "";
            Target = null;
            StartTime = 0;
            EndTime = 0;
            SpellData = null;
            IsValid = false;
        }
    }

    /// <summary>
    ///     Extension for getting the last casted spell of an <see cref="Obj_AI_Hero"/>
    /// </summary>
    public static class LastCastedSpell
    {
        internal static readonly Dictionary<int, LastCastedSpellEntry> CastedSpells =
            new Dictionary<int, LastCastedSpellEntry>();

        static LastCastedSpell()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is Obj_AI_Hero))
            {
                return;
            }

            var entry = new LastCastedSpellEntry(args);
            CastedSpells[sender.NetworkId] = entry;
        }

        /// <summary>
        ///     Gets the <see cref="LastCastedSpellEntry"/> of the unit.
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns><see cref="LastCastedSpellEntry"/></returns>
        public static LastCastedSpellEntry GetLastCastedSpell(this Obj_AI_Hero target)
        {
            LastCastedSpellEntry entry;
            var contains = CastedSpells.TryGetValue(target.NetworkId, out entry);

            return contains ? entry : new LastCastedSpellEntry();
        }
        
    }
}
