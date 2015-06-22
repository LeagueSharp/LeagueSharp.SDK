// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Health.cs" company="LeagueSharp">
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
//   Health Prediction class for prediction of health of units.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Math.Prediction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;

    /// <summary>
    ///     Health Prediction class for prediction of health of units.
    /// </summary>
    public class Health
    {
        #region Static Fields

        /// <summary>
        ///     List of Active Attacks.
        /// </summary>
        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();

        /// <summary>
        ///     Last Tick Update
        /// </summary>
        private static int lastTick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Health" /> class.
        /// </summary>
        static Health()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnStopCast += SpellbookOnStopCast;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the health prediction, either default or simulated
        /// </summary>
        /// <param name="unit"><see cref="Obj_AI_Base" /> unit</param>
        /// <param name="time">The time in milliseconds</param>
        /// <param name="delay">An optional delay</param>
        /// <param name="type"><see cref="HealthPredictionType" /> type</param>
        /// <returns>The <see cref="float" /></returns>
        public static float GetPrediction(
            Obj_AI_Base unit, 
            int time, 
            int delay = 70, 
            HealthPredictionType type = HealthPredictionType.Default)
        {
            return type == HealthPredictionType.Simulated
                       ? GetPredictionSimulated(unit, time)
                       : GetPredictionDefault(unit, time, delay);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Game Tick which is called by the game update event.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Variables.TickCount - lastTick <= 60 * 1000)
            {
                return;
            }

            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.TickCount - 60000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));
            lastTick = Variables.TickCount;
        }

        /// <summary>
        ///     Calculates the default prediction of the unit.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="time">
        ///     The time
        /// </param>
        /// <param name="delay">
        ///     The delay
        /// </param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        private static float GetPredictionDefault(Obj_AI_Base unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var attackDamage = 0f;
                if (attack.Source.IsValidTarget(float.MaxValue, false)
                    && attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000 * unit.Distance(attack.Source) / attack.ProjectileSpeed + delay;

                    if (Variables.TickCount < landTime - delay && landTime < Variables.TickCount + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Calculates the simulated prediction of the unit.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="time">
        ///     The time
        /// </param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        private static float GetPredictionSimulated(Obj_AI_Base unit, int time)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var n = 0;
                if (Variables.TickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Target.IsValidTarget(float.MaxValue, false)
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.TickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Variables.TickCount
                            && (fromT + attack.Delay + unit.Distance(attack.Source) / attack.ProjectileSpeed < toT))
                        {
                            n++;
                        }

                        fromT += (int)attack.AnimationTime;
                    }
                }

                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Process Spell Cast subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(3000, false) || sender.Team != GameObjects.Player.Team || sender is Obj_AI_Hero
                || !args.SData.ConsideredAsAutoAttack || !(args.Target is Obj_AI_Base))
            {
                return;
            }

            var target = (Obj_AI_Base)args.Target;
            ActiveAttacks.Remove(sender.NetworkId);

            var attackData = new PredictedDamage(
                sender, 
                target, 
                Variables.TickCount - Game.Ping / 2, 
                sender.AttackCastDelay * 1000, 
                sender.AttackDelay * 1000 - (sender is Obj_AI_Turret ? 70 : 0), 
                sender.CombatType == GameObjectCombatType.Melee ? int.MaxValue : (int)args.SData.MissileSpeed, 
                sender.TotalAttackDamage);

            ActiveAttacks.Add(sender.NetworkId, attackData);
        }

        /// <summary>
        ///     Spell-book on casting stop subscribed event function.
        /// </summary>
        /// <param name="spellbook">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">Spell-book Stop Cast Data</param>
        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && args.DestroyMissile && args.StopAnimation)
            {
                if (ActiveAttacks.ContainsKey(spellbook.Owner.NetworkId))
                {
                    ActiveAttacks.Remove(spellbook.Owner.NetworkId);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Predicted Damage Container
        /// </summary>
        private class PredictedDamage
        {
            #region Fields

            /// <summary>
            ///     Animation Time
            /// </summary>
            public readonly float AnimationTime;

            /// <summary>
            ///     The Damage
            /// </summary>
            public readonly float Damage;

            /// <summary>
            ///     Delay before damage impact
            /// </summary>
            public readonly float Delay;

            /// <summary>
            ///     Projectile Speed
            /// </summary>
            public readonly int ProjectileSpeed;

            /// <summary>
            ///     The Source
            /// </summary>
            public readonly Obj_AI_Base Source;

            /// <summary>
            ///     Start Tick
            /// </summary>
            public readonly int StartTick;

            /// <summary>
            ///     The Target
            /// </summary>
            public readonly Obj_AI_Base Target;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PredictedDamage" /> class.
            /// </summary>
            /// <param name="source">
            ///     Damage Source
            /// </param>
            /// <param name="target">
            ///     Damage Target
            /// </param>
            /// <param name="startTick">
            ///     Starting Game Tick
            /// </param>
            /// <param name="delay">
            ///     Delay of damage impact
            /// </param>
            /// <param name="animationTime">
            ///     Animation time
            /// </param>
            /// <param name="projectileSpeed">
            ///     Projectile Speed
            /// </param>
            /// <param name="damage">
            ///     The Damage
            /// </param>
            public PredictedDamage(
                Obj_AI_Base source, 
                Obj_AI_Base target, 
                int startTick, 
                float delay, 
                float animationTime, 
                int projectileSpeed, 
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTick = startTick;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }

            #endregion
        }
    }
}