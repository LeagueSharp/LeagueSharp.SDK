using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Wrappers;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides events for interrupting spells.
    /// </summary>
    public class InterruptableSpell
    {
        /// <summary>
        ///     The danger level of the spell.
        /// </summary>
        public enum DangerLevel
        {
            /// <summary>
            ///     Low danger level
            /// </summary>
            Low,

            /// <summary>
            ///     Medium danger level, should be interrupted
            /// </summary>
            Medium,

            /// <summary>
            ///     High danger level, definitely should be interrupted
            /// </summary>
            High
        }

        /// <summary>
        ///     Static constructor
        /// </summary>
        static InterruptableSpell()
        {
            InterruptableSpells = new ConcurrentDictionary<string, List<InterruptableSpellData>>();
            CastingInterruptableSpell = new ConcurrentDictionary<int, InterruptableSpellData>();

            InitializeSpells();

            ObjectManager.Player.GetLastCastedSpell();

            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        /// <summary>
        ///     Interruptable Spells.
        /// </summary>
        private static ConcurrentDictionary<string, List<InterruptableSpellData>> InterruptableSpells { get; set; }

        /// <summary>
        ///     Casting Interruptable Spells.
        /// </summary>
        private static ConcurrentDictionary<int, InterruptableSpellData> CastingInterruptableSpell { get; set; }

        /// <summary>
        ///     Gets fired when an enemy is casting a spellData that should be interrupted.
        /// </summary>
        public static event Action<InterruptableTargetEventArgs> OnInterruptableTarget;

        /// <summary>
        ///     Initializer for the spells
        /// </summary>
        private static void InitializeSpells()
        {
            #region Spells

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

            #endregion
        }

        /// <summary>
        ///     Registers an interrupterable spell.
        /// </summary>
        /// <param name="champName">Champion Name</param>
        /// <param name="spellData">Spell Data <see cref="InterruptableSpellData" /></param>
        private static void RegisterSpell(string champName, InterruptableSpellData spellData)
        {
            if (!InterruptableSpells.ContainsKey(champName))
            {
                InterruptableSpells.TryAdd(champName, new List<InterruptableSpellData>());
            }

            InterruptableSpells[champName].Add(spellData);
        }

        /// <summary>
        ///     Function called by update tick event.
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            foreach (var hero in
                ObjectHandler.AllHeroes.Where(
                    hero =>
                        CastingInterruptableSpell.ContainsKey(hero.NetworkId) && !hero.Spellbook.IsCastingSpell &&
                        !hero.Spellbook.IsChanneling && !hero.Spellbook.IsCharging))
            {
                InterruptableSpellData value;
                CastingInterruptableSpell.TryRemove(hero.NetworkId, out value);
            }

            if (OnInterruptableTarget == null)
            {
                return;
            }

            foreach (var newArgs in
                ObjectHandler.EnemyHeroes.Select(GetInterruptableTargetData).Where(newArgs => newArgs != null))
            {
                OnInterruptableTarget(newArgs);
            }
        }

        /// <summary>
        ///     Function called by OnProcessSpellCast event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = sender as Obj_AI_Hero;
            if (target == null || CastingInterruptableSpell.ContainsKey(target.NetworkId))
            {
                return;
            }

            if (!InterruptableSpells.ContainsKey(target.ChampionName))
            {
                return;
            }

            var spell = InterruptableSpells[target.ChampionName].Find(
                s =>
                {
                    var firstOrDefault = target.Spellbook.Spells.FirstOrDefault(x => x.SData.Name == args.SData.Name);
                    return firstOrDefault != null && s.Slot == firstOrDefault.Slot;
                });

            if (spell != null)
            {
                CastingInterruptableSpell.TryAdd(target.NetworkId, spell);
            }
        }

        /// <summary>
        ///     Function called by the stop cast event.
        /// </summary>
        /// <param name="sender">Sender spellbook</param>
        /// <param name="args">Spellbok Stop Data</param>
        private static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            var target = sender.Owner as Obj_AI_Hero;

            if (target == null)
            {
                return;
            }

            if (!target.Spellbook.IsCastingSpell && !target.Spellbook.IsChanneling && !target.Spellbook.IsCharging)
            {
                InterruptableSpellData value;
                CastingInterruptableSpell.TryRemove(target.NetworkId, out value);
            }
        }

        /// <summary>
        ///     Checks if the unit is casting a spell that can be interrupted.
        /// </summary>
        /// <param name="target">Unit</param>
        /// <param name="checkMovementInterruption">Checks if moving cancels the spellData.</param>
        /// <returns>If the unit is casting an interruptable spellData.</returns>
        public static bool IsCastingInterruptableSpell(Obj_AI_Hero target, bool checkMovementInterruption = false)
        {
            var data = GetInterruptableTargetData(target);
            return data != null && (!checkMovementInterruption || data.MovementInterrupts);
        }

        /// <summary>
        ///     Gets the <see cref="InterruptableTargetEventArgs" /> for the unit.
        /// </summary>
        /// <param name="target">Unit</param>
        /// <returns>
        ///     <see cref="InterruptableTargetEventArgs" />
        /// </returns>
        public static InterruptableTargetEventArgs GetInterruptableTargetData(Obj_AI_Hero target)
        {
            if (!target.IsValid)
            {
                return null;
            }

            if (CastingInterruptableSpell.ContainsKey(target.NetworkId))
            {
                return new InterruptableTargetEventArgs(
                    target, CastingInterruptableSpell[target.NetworkId].DangerLevel, target.Spellbook.CastEndTime,
                    CastingInterruptableSpell[target.NetworkId].MovementInterrupts);
            }

            return null;
        }

        /// <summary>
        ///     Class that represents the event arguements for <see cref="OnInterruptableTarget" />
        /// </summary>
        public class InterruptableTargetEventArgs
        {
            /// <summary>
            ///     Interruptable Target Data internal constructor
            /// </summary>
            /// <param name="sender">Sender or classifed Target</param>
            /// <param name="dangerLevel">Danger Level</param>
            /// <param name="endTime">Ending time of the spell</param>
            /// <param name="movementInterrupts">Does Movement Interrupts the spell</param>
            internal InterruptableTargetEventArgs(Obj_AI_Hero sender,
                DangerLevel dangerLevel,
                float endTime,
                bool movementInterrupts)
            {
                Sender = sender;
                DangerLevel = dangerLevel;
                EndTime = endTime;
                MovementInterrupts = movementInterrupts;
            }

            /// <summary>
            ///     Gets the <see cref="DangerLevel" /> of the spellData.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Gets the time the spellData ends.
            /// </summary>
            public float EndTime { get; private set; }

            /// <summary>
            ///     Gets whether moving caused the spellData to be interrupted.
            /// </summary>
            public bool MovementInterrupts { get; private set; }

            /// <summary>
            ///     The sender or classifed target of the interruptable spell.
            /// </summary>
            public Obj_AI_Hero Sender { get; private set; }
        }

        /// <summary>
        ///     Interruptable Spell Data
        /// </summary>
        private class InterruptableSpellData
        {
            /// <summary>
            ///     Interruptable Spell Data Constructor.
            /// </summary>
            /// <param name="slot">Spell Slot</param>
            /// <param name="dangerLevel">Danger Level</param>
            /// <param name="movementInterrupts">Does movement interrupt the spell</param>
            public InterruptableSpellData(SpellSlot slot, DangerLevel dangerLevel, bool movementInterrupts = true)
            {
                Slot = slot;
                DangerLevel = dangerLevel;
                MovementInterrupts = movementInterrupts;
            }

            /// <summary>
            ///     Spell Slot.
            /// </summary>
            public SpellSlot Slot { get; private set; }

            /// <summary>
            ///     Spell Danger Level.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Spell Interruptable by Movement by caster.
            /// </summary>
            public bool MovementInterrupts { get; private set; }
        }
    }
}