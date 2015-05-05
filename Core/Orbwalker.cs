#region

using System;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Managers;
using LeagueSharp.CommonEx.Core.Math.Prediction;
using LeagueSharp.CommonEx.Core.Utils;
using LeagueSharp.CommonEx.Core.Wrappers;
using Microsoft.Win32.SafeHandles;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Orbwalker class, provides a utility tool for assemblies to implement an orbwalker. An orbwalker is a tool for
    ///     performing an auto attack and moving as quickly as possible afterwards.
    /// </summary>
    public class Orbwalker
    {
        private static bool _missleLaunched = true;

        static Orbwalker()
        {
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        #region Minion Stuff

        private Obj_AI_Base GetKillableMinion(MinionTypes types = MinionTypes.All,
            HealthPredictionType predictionType = HealthPredictionType.Default,
            double adModifier = 1.0)
        {
            return
                (from minion in
                    MinionManager.GetMinions(
                        ObjectHandler.Player.ServerPosition, ObjectHandler.Player.GetRealAutoAttackRange() + 65, types)
                    where CanOrbwalk(minion)
                    let predictedHealth =
                        Health.GetPrediction(minion, (int) minion.GetTimeToHit(), 20, HealthPredictionType.Simulated)
                    where
                        predictedHealth > 0 &&
                        predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(minion) * adModifier
                    select minion).FirstOrDefault();
        }

        private bool ShouldWait()
        {
            return
                (from minion in
                    MinionManager.GetMinions(
                        ObjectHandler.Player.ServerPosition, ObjectHandler.Player.GetRealAutoAttackRange() + 65)
                    where CanOrbwalk(minion)
                    let predictedHealth =
                        Health.GetPrediction(
                            minion, (int) (ObjectHandler.Player.AttackDelay * 1.3), 20, HealthPredictionType.Simulated)
                    select predictedHealth > 0 && predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(minion))
                    .FirstOrDefault();
        }

        #endregion

        #region Public API

        private void Attack(AttackableUnit target)
        {
            ObjectHandler.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
        }

        /// <summary>
        ///     Returns if our hero is currently able to perform an auto attack
        /// </summary>
        /// <returns></returns>
        public bool CanAttack()
        {
            return false;
        }

        /// <summary>
        ///     Returns if our hero is currently able to move and not attacking
        /// </summary>
        /// <returns></returns>
        public bool CanMove()
        {
            return false;
        }

        /// <summary>
        ///     Returns if our hero is currently able to perform the orbwalking process on another <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="target">Type of <see cref="AttackableUnit" /></param>
        /// <returns></returns>
        public bool CanOrbwalk(AttackableUnit target)
        {
            return target != null && target.IsValidTarget() && target.InAutoAttackRange();
        }

        private void MoveTo(Vector3 position, float distance = 500)
        {
            ObjectHandler.Player.IssueOrder(
                GameObjectOrder.MoveTo, ObjectHandler.Player.Position.Extend(position, distance));
        }

        /// <summary>
        ///     Performs the orbwalking process on another <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="mode"><see cref="OrbwalkerMode" /> mode</param>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
        public void PerformMode(OrbwalkerMode mode, AttackableUnit target = null)
        {
            if (CanAttack())
            {
                switch (mode)
                {
                    case OrbwalkerMode.Combo:
                        PerformModeCombo(target);
                        return;

                    case OrbwalkerMode.LaneClear:
                        PerformModeLaneClear(target);
                        return;

                    case OrbwalkerMode.LaneFreeze:
                        PerformModeLaneFreeze();
                        return;

                    case OrbwalkerMode.LastHit:
                        PerformModeLastHit();
                        return;

                    case OrbwalkerMode.Mixed:
                        PerformModeMixed();
                        return;
                }
            }

            if (mode != OrbwalkerMode.None && CanMove())
            {
                MoveTo(Cursor.Position.ToVector3());
            }
        }

        /// <summary>
        ///     Resets the auto attack timer, which results in an instant attack again (if possible) or stuttering (if used wrong)
        /// </summary>
        public void ResetSwingTimer()
        {
            //
        }

        #region Orbwalker Modes

        private void PerformModeCombo(AttackableUnit target = null)
        {
            if (CanAttack() && CanOrbwalk(target))
            {
                Attack(target);
            }
        }

        private void PerformModeLaneClear(AttackableUnit target = null)
        {
            if (CanOrbwalk(target) && !ShouldWait())
            {
                Attack(target);
                return;
            }

            var minion = GetKillableMinion(MinionTypes.All, HealthPredictionType.Simulated);

            if (minion != null)
            {
                Attack(minion);
            }
        }

        private void PerformModeLaneFreeze(AttackableUnit target = null)
        {
            if (CanOrbwalk(target) && !ShouldWait())
            {
                Attack(target);
                return;
            }

            var minion = GetKillableMinion(MinionTypes.All, HealthPredictionType.Default, 0.5);

            if (minion != null)
            {
                Attack(minion);
            }
        }

        private void PerformModeLastHit()
        {
            var minion = GetKillableMinion();

            if (minion != null)
            {
                Attack(minion);
            }
        }

        private void PerformModeMixed(AttackableUnit target = null)
        {
            if (CanOrbwalk(target) && !ShouldWait())
            {
                Attack(target);
                return;
            }

            var minion = GetKillableMinion();

            if (minion != null)
            {
                Attack(minion);
            }
        }

        #endregion

        #endregion

        #region Events

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!_missleLaunched)
            {
                foreach (var hero in
                    ObjectHandler.EnemyHeroes.Where(
                        hero => hero.IsValidTarget(ObjectHandler.Player.GetRealAutoAttackRange())))
                {
                    ObjectHandler.Player.IssueOrder(GameObjectOrder.AttackUnit, hero);
                }
            }

            if (_missleLaunched)
            {
                ObjectHandler.Player.IssueOrder(GameObjectOrder.MoveTo, Cursor.Position.ToVector3());
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid)
            {
                var missile = (Obj_SpellMissile) sender;

                if (missile.SpellCaster.IsMe && AutoAttack.IsAutoAttack(missile.SData.Name))
                {
                    _missleLaunched = true;
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && AutoAttack.IsAutoAttack(args.SData.Name))
            {
                _missleLaunched = false;
            }
        }

        private static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            if (sender.Owner.IsValid && sender.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                _missleLaunched = false;
            }
        }

        #endregion
    }
}