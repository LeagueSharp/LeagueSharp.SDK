// <copyright file="InterruptableSpell.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    /// <summary>
    ///     Provides events for interrupting spells.
    /// </summary>
    public static partial class Events
    {
        #region Static Fields

        /// <summary>
        ///     Gets the global interruptable spells.
        /// </summary>
        public static readonly IReadOnlyList<InterruptableSpellDataEntry> GlobalInterruptableSpells =
            Data.Get<InterruptableSpellData>().GlobalInterruptableSpells;

        /// <summary>
        ///     Gets the interruptable spells.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, List<InterruptableSpellDataEntry>> InterruptableSpells =
            Data.Get<InterruptableSpellData>().InterruptableSpells;

        /// <summary>
        ///     Gets the casting interruptable spells.
        /// </summary>
        private static readonly Dictionary<int, InterruptableSpellDataEntry> CastingInterruptableSpellDictionary =
            new Dictionary<int, InterruptableSpellDataEntry>();

        #endregion

        #region Public Events

        /// <summary>
        ///     Gets fired when an enemy is casting a spellData that should be interrupted.
        /// </summary>
        public static event EventHandler<InterruptableTargetEventArgs> OnInterruptableTarget;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the casting interruptible spell dictionary.
        /// </summary>
        public static IReadOnlyDictionary<int, InterruptableSpellDataEntry> CastingInterruptableSpell
            => CastingInterruptableSpellDictionary;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="InterruptableTargetEventArgs" /> for the unit.
        /// </summary>
        /// <param name="target">
        ///     The unit
        /// </param>
        /// <returns>
        ///     <see cref="InterruptableTargetEventArgs" />
        /// </returns>
        public static InterruptableTargetEventArgs GetInterruptableTargetData(Obj_AI_Hero target)
        {
            if (target == null || !target.IsValid)
            {
                return null;
            }

            InterruptableSpellDataEntry value;
            return CastingInterruptableSpellDictionary.TryGetValue(target.NetworkId, out value)
                       ? new InterruptableTargetEventArgs(
                             target,
                             value.DangerLevel,
                             target.Spellbook.CastEndTime,
                             value.MovementInterrupts)
                       : null;
        }

        /// <summary>
        ///     Checks if the unit is casting a spell that can be interrupted.
        /// </summary>
        /// <param name="target">
        ///     The unit
        /// </param>
        /// <param name="checkMovementInterruption">
        ///     Checks if moving cancels the spellData.
        /// </param>
        /// <returns>
        ///     If the unit is casting an interrupt-able spellData.
        /// </returns>
        public static bool IsCastingInterruptableSpell(this Obj_AI_Hero target, bool checkMovementInterruption = false)
        {
            var data = GetInterruptableTargetData(target);
            return data != null && (!checkMovementInterruption || data.MovementInterrupts);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Function called by update tick event.
        /// </summary>
        private static void EventInterruptableSpell()
        {
            foreach (var hero in
                GameObjects.Heroes.Where(
                    hero =>
                    CastingInterruptableSpellDictionary.ContainsKey(hero.NetworkId) && !hero.Spellbook.IsCastingSpell
                    && !hero.Spellbook.IsChanneling && !hero.Spellbook.IsCharging))
            {
                CastingInterruptableSpellDictionary.Remove(hero.NetworkId);
            }

            if (OnInterruptableTarget == null)
            {
                return;
            }

            foreach (var newArgs in
                GameObjects.EnemyHeroes.Select(GetInterruptableTargetData).Where(newArgs => newArgs != null))
            {
                OnInterruptableTarget(MethodBase.GetCurrentMethod().DeclaringType, newArgs);
            }
        }

        /// <summary>
        ///     Function called by OnProcessSpellCast event
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     Processed Spell Cast Data
        /// </param>
        private static void EventInterruptableSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = sender as Obj_AI_Hero;

            if (target == null || CastingInterruptableSpellDictionary.ContainsKey(target.NetworkId))
            {
                return;
            }

            var globalInterruptSpell = GlobalInterruptableSpells.FirstOrDefault(s => s.Name.Equals(args.SData.Name));

            if (globalInterruptSpell != null)
            {
                CastingInterruptableSpellDictionary.Add(target.NetworkId, globalInterruptSpell);
                return;
            }

            if (!InterruptableSpells.ContainsKey(target.ChampionName))
            {
                return;
            }

            var spell = InterruptableSpells[target.ChampionName].Find(
                s =>
                    {
                        var firstOrDefault = target.Spellbook.Spells.FirstOrDefault(
                            x => x.SData.Name == args.SData.Name);
                        return firstOrDefault != null && s.Slot == firstOrDefault.Slot;
                    });

            if (spell != null)
            {
                CastingInterruptableSpellDictionary.Add(target.NetworkId, spell);
            }
        }

        /// <summary>
        ///     Function called by the stop cast event.
        /// </summary>
        /// <param name="sender">
        ///     Sender Spell-book
        /// </param>
        private static void EventInterruptableSpell(Spellbook sender)
        {
            var target = sender.Owner as Obj_AI_Hero;

            if (target == null)
            {
                return;
            }

            if (!target.Spellbook.IsCastingSpell && !target.Spellbook.IsChanneling && !target.Spellbook.IsCharging)
            {
                CastingInterruptableSpellDictionary.Remove(target.NetworkId);
            }
        }

        #endregion

        /// <summary>
        ///     Class that represents the event arguments for <see cref="OnInterruptableTarget" />
        /// </summary>
        public class InterruptableTargetEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableTargetEventArgs" /> class.
            /// </summary>
            /// <param name="sender">
            ///     Sender or classified Target
            /// </param>
            /// <param name="dangerLevel">
            ///     Danger Level
            /// </param>
            /// <param name="endTime">
            ///     Ending time of the spell
            /// </param>
            /// <param name="movementInterrupts">
            ///     Does Movement Interrupts the spell
            /// </param>
            internal InterruptableTargetEventArgs(
                Obj_AI_Hero sender,
                DangerLevel dangerLevel,
                float endTime,
                bool movementInterrupts)
            {
                this.Sender = sender;
                this.DangerLevel = dangerLevel;
                this.EndTime = endTime;
                this.MovementInterrupts = movementInterrupts;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the <see cref="DangerLevel" /> of the spellData.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Gets the time the spellData ends.
            /// </summary>
            public float EndTime { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether movement interrupts.
            /// </summary>
            public bool MovementInterrupts { get; }

            /// <summary>
            ///     Gets the sender.
            /// </summary>
            public Obj_AI_Hero Sender { get; }

            #endregion
        }
    }
}