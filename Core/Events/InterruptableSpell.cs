// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterruptableSpell.cs" company="LeagueSharp">
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
//   Provides events for interrupting spells.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     Provides events for interrupting spells.
    /// </summary>
    public class InterruptableSpell
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="InterruptableSpell" /> class.
        /// </summary>
        static InterruptableSpell()
        {
            InterruptableSpellsDictionary = new Dictionary<string, List<InterruptableSpellData>>();
            CastingInterruptableSpellDictionary = new Dictionary<int, InterruptableSpellData>();

            InitializeSpells();

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Interrupt-able Target Delegate.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Interrupt-able Target Event Arguments Container</param>
        public delegate void OnInterruptableTargetDelegate(object sender, InterruptableTargetEventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     Gets fired when an enemy is casting a spellData that should be interrupted.
        /// </summary>
        public static event OnInterruptableTargetDelegate OnInterruptableTarget;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the casting interruptible spell dictionary.
        /// </summary>
        public static IReadOnlyDictionary<int, InterruptableSpellData> CastingInterruptableSpell
        {
            get
            {
                return CastingInterruptableSpellDictionary;
            }
        }

        /// <summary>
        ///     Gets the interruptible spells dictionary.
        /// </summary>
        public static IReadOnlyDictionary<string, List<InterruptableSpellData>> InterruptableSpells
        {
            get
            {
                return InterruptableSpellsDictionary;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the casting interrupt-able spell.
        /// </summary>
        private static Dictionary<int, InterruptableSpellData> CastingInterruptableSpellDictionary { get; set; }

        /// <summary>
        ///     Gets or sets the interrupt-able spells.
        /// </summary>
        private static Dictionary<string, List<InterruptableSpellData>> InterruptableSpellsDictionary { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="InterruptableTargetEventArgs" /> for the unit.
        /// </summary>
        /// <param name="target">The unit</param>
        /// <returns>
        ///     <see cref="InterruptableTargetEventArgs" />
        /// </returns>
        public static InterruptableTargetEventArgs GetInterruptableTargetData(Obj_AI_Hero target)
        {
            if (target == null || !target.IsValid)
            {
                return null;
            }

            InterruptableSpellData value;
            if (CastingInterruptableSpellDictionary.TryGetValue(target.NetworkId, out value))
            {
                return new InterruptableTargetEventArgs(
                    target, 
                    value.DangerLevel, 
                    target.Spellbook.CastEndTime, 
                    value.MovementInterrupts);
            }

            return null;
        }

        /// <summary>
        ///     Checks if the unit is casting a spell that can be interrupted.
        /// </summary>
        /// <param name="target">The unit</param>
        /// <param name="checkMovementInterruption">Checks if moving cancels the spellData.</param>
        /// <returns>If the unit is casting an interrupt-able spellData.</returns>
        public static bool IsCastingInterruptableSpell(Obj_AI_Hero target, bool checkMovementInterruption = false)
        {
            var data = GetInterruptableTargetData(target);
            return data != null && (!checkMovementInterruption || data.MovementInterrupts);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Function called by update tick event.
        /// </summary>
        /// <param name="args"><see cref="System.EventArgs" /> event data</param>
        private static void Game_OnGameUpdate(EventArgs args)
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
                OnInterruptableTarget(newArgs.Sender, newArgs);
            }
        }

        /// <summary>
        ///     Initializer for the spells
        /// </summary>
        private static void InitializeSpells()
        {
            RegisterSpell("Caitlyn", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("FiddleSticks", new InterruptableSpellData(SpellSlot.W, DangerLevel.Medium));
            RegisterSpell("FiddleSticks", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Galio", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Janna", new InterruptableSpellData(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("Karthus", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Katarina", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Lucian", new InterruptableSpellData(SpellSlot.R, DangerLevel.High, false));
            RegisterSpell("Malzahar", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("MasterYi", new InterruptableSpellData(SpellSlot.W, DangerLevel.Low));
            RegisterSpell("MissFortune", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Nunu", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Pantheon", new InterruptableSpellData(SpellSlot.E, DangerLevel.Low));
            RegisterSpell("Pantheon", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("RekSai", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Sion", new InterruptableSpellData(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("Shen", new InterruptableSpellData(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("TwistedFate", new InterruptableSpellData(SpellSlot.R, DangerLevel.Medium));
            RegisterSpell("Urgot", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Velkoz", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Warwick", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Xerath", new InterruptableSpellData(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Varus", new InterruptableSpellData(SpellSlot.Q, DangerLevel.Low, false));
        }

        /// <summary>
        ///     Function called by OnProcessSpellCast event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = sender as Obj_AI_Hero;
            if (target == null || CastingInterruptableSpellDictionary.ContainsKey(target.NetworkId))
            {
                return;
            }

            if (!InterruptableSpellsDictionary.ContainsKey(target.ChampionName))
            {
                return;
            }

            var spell = InterruptableSpellsDictionary[target.ChampionName].Find(
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
        ///     Registers an interrupter-able spell.
        /// </summary>
        /// <param name="champName">Champion Name</param>
        /// <param name="spellData">Spell Data <see cref="InterruptableSpellData" /></param>
        private static void RegisterSpell(string champName, InterruptableSpellData spellData)
        {
            if (!InterruptableSpellsDictionary.ContainsKey(champName))
            {
                InterruptableSpellsDictionary.Add(champName, new List<InterruptableSpellData>());
            }

            InterruptableSpellsDictionary[champName].Add(spellData);
        }

        /// <summary>
        ///     Function called by the stop cast event.
        /// </summary>
        /// <param name="sender">Sender Spell-book</param>
        /// <param name="args">Spell-book Stop Data</param>
        private static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
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
        ///     Interrupt-able Spell Data
        /// </summary>
        public class InterruptableSpellData
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableSpellData" /> class.
            /// </summary>
            /// <param name="slot">
            ///     Spell Slot
            /// </param>
            /// <param name="dangerLevel">
            ///     Danger Level
            /// </param>
            /// <param name="movementInterrupts">
            ///     Does movement interrupt the spell
            /// </param>
            public InterruptableSpellData(SpellSlot slot, DangerLevel dangerLevel, bool movementInterrupts = true)
            {
                this.Slot = slot;
                this.DangerLevel = dangerLevel;
                this.MovementInterrupts = movementInterrupts;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the danger level.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether movement interrupts.
            /// </summary>
            public bool MovementInterrupts { get; private set; }

            /// <summary>
            ///     Gets the slot.
            /// </summary>
            public SpellSlot Slot { get; private set; }

            #endregion
        }

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
            public bool MovementInterrupts { get; private set; }

            /// <summary>
            ///     Gets the sender.
            /// </summary>
            public Obj_AI_Hero Sender { get; private set; }

            #endregion
        }
    }
}