#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Extensions;

#endregion

namespace LeagueSharp.CommonEx.Core.Math.Prediction
{
    /// <summary>
    ///     Health Prediction class for prediction of health of units.
    /// </summary>
    public class Health
    {
        /// <summary>
        ///     List of Active Attacks.
        /// </summary>
        private static readonly Dictionary<int, PredictedDamage> ActiveAttacks = new Dictionary<int, PredictedDamage>();

        /// <summary>
        ///     Last Tick Update
        /// </summary>
        private static int _lastTick;

        /// <summary>
        ///     Static Construcor
        /// </summary>
        static Health()
        {
            Obj_AI_Base.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnStopCast += SpellbookOnStopCast;
        }

        /// <summary>
        ///     Game Tick which is called by the game update event.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" />
        /// </param>
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Variables.TickCount - _lastTick <= 60 * 1000)
            {
                return;
            }
            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.TickCount - 60000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));
            _lastTick = Variables.TickCount;
        }

        /// <summary>
        ///     Spellbook On casting stop subscribed event function.
        /// </summary>
        /// <param name="spellbook">
        ///     <see cref="Spellbook" />
        /// </param>
        /// <param name="args">Spellbook Stop Cast Data</param>
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

        /// <summary>
        ///     Process Spell Cast subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void ObjAiBaseOnOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(3000, false) || sender.Team != ObjectManager.Player.Team || sender is Obj_AI_Hero ||
                !args.SData.ConsideredAsAutoAttack || !(args.Target is Obj_AI_Base))
            {
                return;
            }

            var target = (Obj_AI_Base) args.Target;
            ActiveAttacks.Remove(sender.NetworkId);

            var attackData = new PredictedDamage(
                sender, target, Variables.TickCount - Game.Ping / 2, sender.AttackCastDelay * 1000,
                sender.AttackDelay * 1000 - (sender is Obj_AI_Turret ? 70 : 0),
                sender.CombatType == GameObjectCombatType.Melee ? int.MaxValue : (int) args.SData.MissileSpeed,
                sender.TotalAttackDamage);
            ActiveAttacks.Add(sender.NetworkId, attackData);
        }

        /// <summary>
        ///     Returns the unit health after a set time milliseconds.
        /// </summary>
        public static float GetHealthPrediction(Obj_AI_Base unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var attackDamage = 0f;
                if (attack.Source.IsValidTarget(float.MaxValue, false) &&
                    attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var landTime = attack.StartTick + attack.Delay +
                                   1000 * unit.Distance(attack.Source) / attack.ProjectileSpeed + delay;

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
        ///     Returns the unit health after time milliseconds assuming that the past auto-attacks are periodic.
        /// </summary>
        public static float LaneClearHealthPrediction(Obj_AI_Base unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var n = 0;
                if (Variables.TickCount - 100 <= attack.StartTick + attack.AnimationTime &&
                    attack.Target.IsValidTarget(float.MaxValue, false) &&
                    attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.TickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Variables.TickCount &&
                            (fromT + attack.Delay + unit.Distance(attack.Source) / attack.ProjectileSpeed < toT))
                        {
                            n++;
                        }
                        fromT += (int) attack.AnimationTime;
                    }
                }
                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Predicted Damage Container
        /// </summary>
        private class PredictedDamage
        {
            /// <summary>
            ///     Animation Time
            /// </summary>
            public readonly float AnimationTime;

            /// <summary>
            ///     Damage
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
            ///     Source
            /// </summary>
            public readonly Obj_AI_Base Source;

            /// <summary>
            ///     Start Tick
            /// </summary>
            public readonly int StartTick;

            /// <summary>
            ///     Target
            /// </summary>
            public readonly Obj_AI_Base Target;

            /// <summary>
            ///     Predicted Damage Constructor
            /// </summary>
            /// <param name="source">Damage Source</param>
            /// <param name="target">Damage Target</param>
            /// <param name="startTick">Starting Game Tick</param>
            /// <param name="delay">Delay of damage impact</param>
            /// <param name="animationTime">Animation time</param>
            /// <param name="projectileSpeed">Projectile Speed</param>
            /// <param name="damage">Damage</param>
            public PredictedDamage(Obj_AI_Base source,
                Obj_AI_Base target,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                Source = source;
                Target = target;
                StartTick = startTick;
                Delay = delay;
                ProjectileSpeed = projectileSpeed;
                Damage = damage;
                AnimationTime = animationTime;
            }
        }
    }
}