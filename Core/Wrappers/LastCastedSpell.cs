// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastCastedSpell.cs" company="LeagueSharp">
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
//   Holds information about the last casted spell a unit did.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.Wrappers
{
    using System.Collections.Generic;

    /// <summary>
    ///     Holds information about the last casted spell a unit did.
    /// </summary>
    public class LastCastedSpellEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastedSpellEntry" /> class.
        /// </summary>
        /// <param name="args">
        ///     Processed Casted Spell Data
        /// </param>
        internal LastCastedSpellEntry(GameObjectProcessSpellCastEventArgs args)
        {
            this.Name = args.SData.Name;
            this.Target = args.Target as Obj_AI_Base;
            this.StartTime = Variables.TickCount;
            this.EndTime = Variables.TickCount + args.SData.SpellCastTime;
            this.SpellData = args.SData;
            this.IsValid = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastedSpellEntry" /> class.
        ///     Internal Constructor for Last Casted Spell Entry.
        /// </summary>
        internal LastCastedSpellEntry()
        {
            this.Name = string.Empty;
            this.Target = null;
            this.StartTime = 0;
            this.EndTime = 0;
            this.SpellData = null;
            this.IsValid = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the end time of the cast.
        /// </summary>
        public float EndTime { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is the spell is valid and not empty.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        ///     Gets or sets the name of the spell last casted.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="SpellData" /> of the spell casted.
        /// </summary>
        public SpellData SpellData { get; set; }

        /// <summary>
        ///     Gets or sets the Start time of the cast.
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the Target
        /// </summary>
        public Obj_AI_Base Target { get; set; }

        #endregion
    }

    /// <summary>
    ///     The last cast packet sent entry.
    /// </summary>
    public class LastCastPacketSentEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastPacketSentEntry" /> class.
        /// </summary>
        /// <param name="slot">
        ///     The slot
        /// </param>
        /// <param name="tick">
        ///     The tick
        /// </param>
        /// <param name="targetNetworkId">
        ///     The target network id
        /// </param>
        public LastCastPacketSentEntry(SpellSlot slot, int tick, int targetNetworkId)
        {
            this.Slot = slot;
            this.Tick = tick;
            this.TargetNetworkId = targetNetworkId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the slot.
        /// </summary>
        public SpellSlot Slot { get; private set; }

        /// <summary>
        ///     Gets the target network id.
        /// </summary>
        public int TargetNetworkId { get; private set; }

        /// <summary>
        ///     Gets the tick.
        /// </summary>
        public int Tick { get; private set; }

        #endregion
    }

    /// <summary>
    ///     Extension for getting the last casted spell of an <see cref="Obj_AI_Hero" />
    /// </summary>
    public static class LastCastedSpell
    {
        #region Static Fields

        /// <summary>
        ///     Casted Spells of the champions
        /// </summary>
        internal static readonly Dictionary<int, LastCastedSpellEntry> CastedSpells =
            new Dictionary<int, LastCastedSpellEntry>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="LastCastedSpell" /> class.
        ///     Static constructor
        /// </summary>
        static LastCastedSpell()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Spellbook.OnCastSpell += OnCastSpell;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the last cast packet sent.
        /// </summary>
        public static LastCastPacketSentEntry LastCastPacketSent { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="LastCastedSpellEntry" /> of the unit.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>
        ///     <see cref="LastCastedSpellEntry" />
        /// </returns>
        public static LastCastedSpellEntry GetLastCastedSpell(this Obj_AI_Hero target)
        {
            LastCastedSpellEntry entry;
            var contains = CastedSpells.TryGetValue(target.NetworkId, out entry);

            return contains ? entry : new LastCastedSpellEntry();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Function that is called by the OnProcessSpellCast event.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero != null)
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
        ///     OnCastSpell event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                LastCastPacketSent = new LastCastPacketSentEntry(
                    args.Slot,
                    Variables.TickCount,
                    (args.Target is Obj_AI_Base) ? args.Target.NetworkId : 0);
            }
        }

        #endregion
    }
}