#region

using System;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Managers;
using LeagueSharp.CommonEx.Core.Math.Prediction;
using LeagueSharp.CommonEx.Core.UI;
using LeagueSharp.CommonEx.Core.UI.Values;
using LeagueSharp.CommonEx.Core.Utils;
using LeagueSharp.CommonEx.Core.Wrappers;
using SharpDX;
using Cursor = LeagueSharp.CommonEx.Core.Utils.Cursor;
using Menu = LeagueSharp.CommonEx.Core.UI.Menu;

#endregion

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Orbwalker class, provides a utility tool for assemblies to implement an orbwalker. An orbwalker is a tool for
    ///     performing an auto attack and moving as quickly as possible afterwards.
    /// </summary>
    public class Orbwalker
    {
        /// <summary>
        ///     Returns if we are currently trying to lane freeze
        /// </summary>
        private static bool _activeFreeze;

        /// <summary>
        ///     Returns if we are shooting a missile that isn't visible, yet
        /// </summary>
        private static bool _missleLaunched = true;

        /// <summary>
        ///     Contains a <see cref="Menu"/>
        /// </summary>
        private readonly Menu _menu;

        /// <summary>
        ///     Contains the current mode (<see cref="OrbwalkerMode"/>)
        /// </summary>
        private OrbwalkerMode _activeMode = OrbwalkerMode.None;

        /// <summary>
        /// Initializes static members of the <see cref="Orbwalker"/> class.
        /// </summary>
        static Orbwalker()
        {
            #region Bind Events

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;

            #endregion
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Orbwalker" /> class.
        /// </summary>
        public Orbwalker()
        {
            #region Menu

            _menu = new Menu("CommonEx.Orbwalker", "Orbwalker");

            var drawingMenu = new Menu("Drawings", "Drawings");
            drawingMenu.Add(new MenuItem<MenuBool>("HoldZone", "Draw Hold Zone") {Value = new MenuBool()});
            drawingMenu.Add(new MenuItem<MenuBool>("AttackedMinion", "Draw Attacked Minion")
            {
                Value = new MenuBool(true) // thats redundant :S
            });
            drawingMenu.Add(new MenuItem<MenuBool>("AutoAttackRange", "Draw AA Range") {Value = new MenuBool(true)});
            _menu.Add(drawingMenu);

            _menu.Add(new MenuItem<MenuKeyBind>("Combo", "Combo")
            {
                Value = new MenuKeyBind(Keys.Space, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("Harass", "Harass") {Value = new MenuKeyBind(Keys.C, KeyBindType.Press)});
            _menu.Add(new MenuItem<MenuKeyBind>("WaveClear", "Wave Clear")
            {
                Value = new MenuKeyBind(Keys.V, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("LastHit", "Last Hit")
            {
                Value = new MenuKeyBind(Keys.X, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("Freeze", "Freeze")
            {
                Value = new MenuKeyBind(Keys.F1, KeyBindType.Toggle)
            });

            Variables.LeagueSharpMenu.Add(_menu);
          
            _menu["Combo"].GetValue<MenuKeyBind>().ValueChanged +=
                (sender, args) =>
                {
                    if (args.GetValue<MenuKeyBind>().Active)
                    {
                        _activeMode = OrbwalkerMode.Combo;
                    }
                };
            _menu["Harass"].GetValue<MenuKeyBind>().ValueChanged +=
                (sender, args) =>
                {
                    if (args.GetValue<MenuKeyBind>().Active)
                    {
                        _activeMode = OrbwalkerMode.Mixed;
                    }
                };
            _menu["WaveClear"].GetValue<MenuKeyBind>().ValueChanged +=
                (sender, args) =>
                {
                    if (args.GetValue<MenuKeyBind>().Active)
                    {
                        _activeMode = OrbwalkerMode.LaneClear;
                    }
                };
            _menu["LastHit"].GetValue<MenuKeyBind>().ValueChanged +=
                (sender, args) =>
                {
                    if (args.GetValue<MenuKeyBind>().Active)
                    {
                        _activeMode = OrbwalkerMode.LastHit;
                    }
                };
            _menu["Freeze"].GetValue<MenuKeyBind>().ValueChanged +=
                (sender, args) =>
                {
                   _activeFreeze = args.GetValue<MenuKeyBind>().Active;
                };

            #endregion
        }

        internal void BootstrapMenu()
        {
            
        }

        #region Minion Stuff

        /// <summary>
        ///     Returns the best killable minion (if any)
        /// </summary>
        /// <param name="types"><see cref="MinionTypes" /> as filter</param>
        /// <param name="predictionType"><see cref="HealthPredictionType" /> to support both lane clear/last hit</param>
        /// <param name="adModifier">A modifier value for the lane freeze, passed as <see cref="float" /></param>
        /// <returns></returns>
        private Obj_AI_Base GetKillableMinion(MinionTypes types = MinionTypes.All,
            HealthPredictionType predictionType = HealthPredictionType.Default,
            double adModifier = 1.0)
        {
            return
                MinionManager.GetMinions(
                    ObjectHandler.Player.ServerPosition, ObjectHandler.Player.GetRealAutoAttackRange() + 65, types)
                    .Where(CanGetOrbwalked)
                    .Select(
                        minion =>
                            new
                            {
                                minion,
                                predictedHealth =
                                    Health.GetPrediction(minion, (int) minion.GetTimeToHit(), 20, predictionType)
                            })
                    .Where(@t => @t.predictedHealth > 0 &&
                                 @t.predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(@t.minion)*adModifier)
                    .Select(@t => @t.minion).FirstOrDefault();
        }

        /// <summary>
        ///     Returns if a minion is killable soon and we should wait with our attack
        /// </summary>
        /// <returns></returns>
        private bool ShouldWait()
        {
            return
                MinionManager.GetMinions(
                    ObjectHandler.Player.ServerPosition, ObjectHandler.Player.GetRealAutoAttackRange() + 65)
                    .Where(CanGetOrbwalked)
                    .Select(minion => new
                    {
                        minion,
                        predictedHealth = Health.GetPrediction(
                            minion, (int) (ObjectHandler.Player.AttackDelay*1.3), 20, HealthPredictionType.Simulated)
                    })
                    .Select(
                        @t =>
                            @t.predictedHealth > 0 &&
                            @t.predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(@t.minion))
                    .FirstOrDefault();
        }

        #endregion

        #region Public API

        /// <summary>
        ///     Wrapper function to attack a target
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
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
        ///     Returns if our hero is currently able to perform the orbwalking process on another <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="target">Type of <see cref="AttackableUnit" /></param>
        /// <returns></returns>
        public static bool CanGetOrbwalked(AttackableUnit target)
        {
            return target != null && target.IsValidTarget() && target.InAutoAttackRange();
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
        ///     Wrapper function to move to an extended position
        /// </summary>
        /// <param name="position"><see cref="Vector3" /> position</param>
        /// <param name="distance">Extend distance from the given position</param>
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

        /// <summary>
        /// </summary>
        /// <param name="target"></param>
        private void PerformModeCombo(AttackableUnit target = null)
        {
            if (CanAttack() && CanGetOrbwalked(target))
            {
                Attack(target);
            }
        }

        /// <summary>
        ///     Internal function for the lane clear logic
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> Optional target (usually <see cref="Obj_AI_Hero" />)</param>
        private void PerformModeLaneClear(AttackableUnit target = null)
        {
            if (CanGetOrbwalked(target) && !ShouldWait())
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

        /// <summary>
        ///     Internal function for the lane freeze logic
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> Optional target (usually <see cref="Obj_AI_Hero" />)</param>
        private void PerformModeLaneFreeze(AttackableUnit target = null)
        {
            if (CanGetOrbwalked(target) && !ShouldWait())
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

        /// <summary>
        ///     Internal function for the last hit logic
        /// </summary>
        private void PerformModeLastHit()
        {
            var minion = GetKillableMinion();

            if (minion != null)
            {
                Attack(minion);
            }
        }

        /// <summary>
        ///     Internal function for the mixed mode logic
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> Optional target (usually <see cref="Obj_AI_Hero" />)</param>
        private void PerformModeMixed(AttackableUnit target = null)
        {
            if (CanGetOrbwalked(target) && !ShouldWait())
            {
                Attack(target);
                return;
            }

            PerformModeLastHit();
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        ///     Game.OnUpdate Event
        /// </summary>
        /// <param name="args"><see cref="EventArgs" /> args</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            /*
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
             */
        }

        /// <summary>
        ///     Game.OnCreate Event to detect missiles
        /// </summary>
        /// <param name="sender"><see cref="GameObject" /> sender</param>
        /// <param name="args"><see cref="EventArgs" /> args</param>
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


        /// <summary>
        ///     Obj_AI_Base.OnProcessSpellCast Event to detect auto attacks
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args"><see cref="GameObjectProcessSpellCastEventArgs" /> args</param>
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && AutoAttack.IsAutoAttack(args.SData.Name))
            {
                _missleLaunched = false;
            }
        }

        /// <summary>
        ///     Spellbook.OnStopCast Event to detect canceld auto attacks
        /// </summary>
        /// <param name="sender"><see cref="Spellbook" /> sender</param>
        /// <param name="args"><see cref="SpellbookStopCastEventArgs" /> args</param>
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