using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Wrappers;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides events for interrupting spells.
    /// </summary>
    public class Interrupter
    {
        /// <summary>
        ///     Delegate for <see cref="OnInterruptableTarget" />
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Hero" /> that is casting the interruptable spell.</param>
        /// <param name="args">Arguements detailing the spell</param>
        public delegate void InterruptableTargetDelegate(Obj_AI_Hero sender, InterruptableTargetEventArgs args);

        /// <summary>
        ///     The dang
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

        static Interrupter()
        {
            // Initialize Properties
            InterruptableSpells = new Dictionary<string, List<InterruptableSpell>>();
            CastingInterruptableSpell = new Dictionary<int, InterruptableSpell>();

            InitializeSpells();

            // Trigger LastCastedSpell
            ObjectManager.Player.GetLastCastedSpell();

            // Listen to required events
            Game.OnGameUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        private static Dictionary<string, List<InterruptableSpell>> InterruptableSpells { get; set; }
        private static Dictionary<int, InterruptableSpell> CastingInterruptableSpell { get; set; }

        /// <summary>
        ///     Gets fired when an enemy is casting a spell that should be interrupted.
        /// </summary>
        public static event InterruptableTargetDelegate OnInterruptableTarget;

        private static void InitializeSpells()
        {
            #region Spells

            RegisterSpell("Caitlyn", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("FiddleSticks", new InterruptableSpell(SpellSlot.W, DangerLevel.Medium));
            RegisterSpell("FiddleSticks", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Galio", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Janna", new InterruptableSpell(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("Karthus", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Katarina", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Lucian", new InterruptableSpell(SpellSlot.R, DangerLevel.High, false));
            RegisterSpell("Malzahar", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("MasterYi", new InterruptableSpell(SpellSlot.W, DangerLevel.Low));
            RegisterSpell("MissFortune", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Nunu", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Pantheon", new InterruptableSpell(SpellSlot.E, DangerLevel.Low));
            RegisterSpell("Pantheon", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("RekSai", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Sion", new InterruptableSpell(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("Shen", new InterruptableSpell(SpellSlot.R, DangerLevel.Low));
            RegisterSpell("TwistedFate", new InterruptableSpell(SpellSlot.R, DangerLevel.Medium));
            RegisterSpell("Urgot", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Velkoz", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Warwick", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Xerath", new InterruptableSpell(SpellSlot.R, DangerLevel.High));
            RegisterSpell("Varus", new InterruptableSpell(SpellSlot.Q, DangerLevel.Low, false));

            #endregion
        }

        private static void RegisterSpell(string champName, InterruptableSpell spell)
        {
            if (!InterruptableSpells.ContainsKey(champName))
            {
                InterruptableSpells.Add(champName, new List<InterruptableSpell>());
            }

            InterruptableSpells[champName].Add(spell);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Remove heros that have finished casting their interruptable spell
            foreach (var hero in
                ObjectHandler.AllHeroes.Where(
                    hero =>
                        CastingInterruptableSpell.ContainsKey(hero.NetworkId) && !hero.Spellbook.IsCastingSpell &&
                        !hero.Spellbook.IsChanneling && !hero.Spellbook.IsCharging))
            {
                CastingInterruptableSpell.Remove(hero.NetworkId);
            }

            // Trigger OnInterruptableTarget event if needed
            if (OnInterruptableTarget == null)
            {
                return;
            }

            foreach (var enemy in ObjectHandler.Enemies)
            {
                var newArgs = GetInterruptableTargetData(enemy);
                if (newArgs != null)
                {
                    OnInterruptableTarget(enemy, newArgs);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = sender as Obj_AI_Hero;
            if (target == null || CastingInterruptableSpell.ContainsKey(target.NetworkId))
            {
                return;
            }

            // Check if the target is known to have interruptable spells
            if (!InterruptableSpells.ContainsKey(target.ChampionName))
            {
                return;
            }

            // Get the interruptable spell
            var spell = InterruptableSpells[target.ChampionName].Find(
                s =>
                {
                    var firstOrDefault = target.Spellbook.Spells.FirstOrDefault(x => x.SData.Name == args.SData.Name);
                    return firstOrDefault != null && s.Slot == firstOrDefault.Slot;
                });

            if (spell != null)
            {
                // Mark champ as casting interruptable spell
                CastingInterruptableSpell.Add(target.NetworkId, spell);
            }
        }

        private static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            var target = sender.Owner as Obj_AI_Hero;

            if (target == null)
            {
                return;
            }

            // Check if the spell itself stopped casting (interrupted)
            if (!target.Spellbook.IsCastingSpell && !target.Spellbook.IsChanneling && !target.Spellbook.IsCharging)
            {
                CastingInterruptableSpell.Remove(target.NetworkId);
            }
        }

        /// <summary>
        ///     Checks if the unit is casting a spell that can be interrupted.
        /// </summary>
        /// <param name="target">Unit</param>
        /// <param name="checkMovementInterruption">Checks if moving cancels the spell.</param>
        /// <returns>If the unit is casting an interruptable spell.</returns>
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
                // Return the args with spell end time
                return new InterruptableTargetEventArgs(
                    CastingInterruptableSpell[target.NetworkId].DangerLevel, target.Spellbook.CastEndTime,
                    CastingInterruptableSpell[target.NetworkId].MovementInterrupts);
            }

            return null;
        }

        /// <summary>
        ///     Class that represents the event arguements for <see cref="OnInterruptableTarget" />
        /// </summary>
        public class InterruptableTargetEventArgs
        {
            internal InterruptableTargetEventArgs(DangerLevel dangerLevel, float endTime, bool movementInterrupts)
            {
                DangerLevel = dangerLevel;
                EndTime = endTime;
                MovementInterrupts = movementInterrupts;
            }

            /// <summary>
            ///     Gets the <see cref="DangerLevel" /> of the spell.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Gets the time the spell ends.
            /// </summary>
            public float EndTime { get; private set; }

            /// <summary>
            ///     Gets whether moving caused the spell to be interrupted.
            /// </summary>
            public bool MovementInterrupts { get; private set; }
        }

        private class InterruptableSpell
        {
            public InterruptableSpell(SpellSlot slot, DangerLevel dangerLevel, bool movementInterrupts = true)
            {
                Slot = slot;
                DangerLevel = dangerLevel;
                MovementInterrupts = movementInterrupts;
            }

            public SpellSlot Slot { get; private set; }
            public DangerLevel DangerLevel { get; private set; }
            public bool MovementInterrupts { get; private set; }
        }
    }
}