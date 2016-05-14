// <copyright file="Health.cs" company="LeagueSharp">
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

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Utils;

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
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnObjAiBaseProcessSpellCast;
            Spellbook.OnStopCast += OnSpellbookStopCast;
            GameObject.OnDelete += OnGameObjectDelete;
            Obj_AI_Base.OnDoCast += OnObjAiBaseDoCast;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Return the Attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static Obj_AI_Base GetAggroTurret(Obj_AI_Minion minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
            return activeTurret?.Source;
        }

        /// <summary>
        ///     Returns the health prediction, either default or simulated
        /// </summary>
        /// <param name="unit"><see cref="Obj_AI_Base" /> unit</param>
        /// <param name="time">The time in milliseconds</param>
        /// <param name="delay">An optional delay</param>
        /// <param name="type"><see cref="HealthPredictionType" /> type</param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
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

        /// <summary>
        ///     Determines whether the specified minion has minion aggro.
        /// </summary>
        /// <param name="minion">The minion.</param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool HasMinionAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is Obj_AI_Minion && m.Target.Compare(minion));
        }

        /// <summary>
        ///     Determines whether the specified minion has turret aggro.
        /// </summary>
        /// <param name="minion">The minion</param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool HasTurretAggro(Obj_AI_Minion minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
        }

        /// <summary>
        ///     Return the starttick of the attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static int TurretAggroStartTick(Obj_AI_Minion minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is Obj_AI_Turret && m.Target.Compare(minion));
            return activeTurret?.StartTick ?? 0;
        }

        #endregion

        #region Methods

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

            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit) && !i.Processed))
            {
                var attackDamage = 0f;

                if (attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000
                                   * (attack.Source.IsMelee
                                          ? 0
                                          : Math.Max(unit.Distance(attack.Source) - attack.Source.BoundingRadius, 0)
                                            / attack.ProjectileSpeed) + delay;

                    if (landTime < Variables.TickCount + time)
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

            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit)))
            {
                var n = 0;

                if (Variables.TickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.TickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Variables.TickCount
                            && fromT + attack.Delay / 1000
                            + (attack.Source.IsMelee
                                   ? 0
                                   : Math.Max(unit.Distance(attack.Source) - attack.Source.BoundingRadius, 0)
                                     / attack.ProjectileSpeed) < toT)
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
        ///     GameObject on delete subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="GameObject" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            var aiBase = sender as Obj_AI_Base;

            if (aiBase != null)
            {
                var objNetworkId = aiBase.NetworkId;

                if (ActiveAttacks.ContainsKey(objNetworkId))
                {
                    ActiveAttacks.Remove(objNetworkId);
                    return;
                }

                foreach (var activeAttack in ActiveAttacks.Values.Where(i => i.Target.Compare(aiBase)))
                {
                    ActiveAttacks.Remove(activeAttack.Source.NetworkId);
                }
                return;
            }

            var missile = sender as MissileClient;

            if (missile?.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     Game Tick which is called by the game update event.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameUpdate(EventArgs args)
        {
            if (Variables.TickCount - lastTick <= 1000)
            {
                return;
            }

            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.TickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));

            lastTick = Variables.TickCount;
        }

        /// <summary>
        ///     Obj_AI_Base on DoCast subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Obj_AI_Base" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="GameObjectProcessSpellCastEventArgs" /> event data
        /// </param>
        private static void OnObjAiBaseDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMelee)
            {
                var casterNetworkId = sender.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     Process Spell Cast subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(2000, false) || !AutoAttack.IsAutoAttack(args.SData.Name) || !sender.IsAlly)
            {
                return;
            }

            if (!(sender is Obj_AI_Minion) && !(sender is Obj_AI_Turret))
            {
                return;
            }

            var target = args.Target as Obj_AI_Minion;

            if (target == null)
            {
                return;
            }

            ActiveAttacks.Remove(sender.NetworkId);
            ActiveAttacks.Add(
                sender.NetworkId,
                new PredictedDamage(
                    sender,
                    target,
                    Variables.TickCount - (Game.Ping / 2),
                    sender.AttackCastDelay * 1000,
                    (sender.AttackDelay * 1000) - (sender is Obj_AI_Turret ? 70 : 0),
                    sender.IsMelee ? int.MaxValue : (int)args.SData.MissileSpeed,
                    (float)sender.GetAutoAttackDamage(target)));
        }

        /// <summary>
        ///     Spell-book on casting stop subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">Spell-book Stop Cast Data</param>
        private static void OnSpellbookStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            if (sender.Owner.IsValid && args.StopAnimation && args.DestroyMissile)
            {
                var casterNetworkId = sender.Owner.NetworkId;

                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks.Remove(casterNetworkId);
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

            #region Public Properties

            public bool Processed { get; set; }

            #endregion
        }
    }
}