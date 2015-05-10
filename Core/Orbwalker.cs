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
using Menu = LeagueSharp.CommonEx.Core.UI.Menu;

#endregion

namespace LeagueSharp.CommonEx.Core
{
    using System.Media;
    using System.Threading;

    /// <summary>
    ///     Orbwalker class, provides a utility tool for assemblies to implement an orbwalker. An orbwalker is a tool for
    ///     performing an auto attack and moving as quickly as possible afterwards.
    /// </summary>
    public static class Orbwalker
    {
        /// <summary>
        ///     Returns if we are shooting a missile that isn't visible, yet
        /// </summary>
        private static bool _missleLaunched = true;

        /// <summary>
        ///     Contains a <see cref="Menu"/>
        /// </summary>
        private static Menu _menu;

        /// <summary>
        ///     Contains the current mode (<see cref="OrbwalkerMode"/>)
        /// </summary>
        private static OrbwalkerMode _activeMode = OrbwalkerMode.None;

        /// <summary>
        ///     Contains the time of the last auto attack
        /// </summary>
        private static int _lastAutoAttack;

        /// <summary>
        /// Initializes static members of the <see cref="Orbwalker"/> class.
        /// </summary>
        static Orbwalker()
        {
            BootstrapMenu();

            #region Bind Events

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;

            #endregion
        }

        internal static void BootstrapMenu()
        {
            #region Menu Structure

            _menu = new Menu("CommonEx.Orbwalker", "Orbwalker");

            var drawingMenu = new Menu("Drawings", "Drawings");
            drawingMenu.Add(new MenuItem<MenuBool>("HoldZone", "Draw Hold Zone") { Value = new MenuBool() });
            drawingMenu.Add(new MenuItem<MenuBool>("AttackedMinion", "Draw Attacked Minion")
            {
                Value = new MenuBool(true) // thats redundant :S
            });
            drawingMenu.Add(new MenuItem<MenuBool>("AutoAttackRange", "Draw AA Range") { Value = new MenuBool(true) });
            _menu.Add(drawingMenu);

            _menu.Add(new MenuItem<MenuKeyBind>("Combo", "Combo")
            {
                Value = new MenuKeyBind(Keys.Space, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("Harass", "Harass") { Value = new MenuKeyBind(Keys.C, KeyBindType.Press) });
                        _menu.Add(new MenuItem<MenuKeyBind>("LastHit", "Last Hit")
            {
                Value = new MenuKeyBind(Keys.X, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("WaveClear", "Wave Clear")
            {
                Value = new MenuKeyBind(Keys.V, KeyBindType.Press)
            });
            _menu.Add(new MenuItem<MenuKeyBind>("Freeze", "Freeze")
            {
                Value = new MenuKeyBind(Keys.F1, KeyBindType.Toggle)
            });

            #endregion

            Variables.LeagueSharpMenu.Add(_menu);

            #region Menu Events

            _menu.MenuValueChanged += delegate(object sender, OnMenuValueChangedEventArgs args)
            {
                if (args.Menu["Combo"].GetValue<MenuKeyBind>().Active)
                {
                    _activeMode = OrbwalkerMode.Combo;
                }

                if (args.Menu["Harass"].GetValue<MenuKeyBind>().Active)
                {
                    _activeMode = OrbwalkerMode.Mixed;
                }

                if (args.Menu["LastHit"].GetValue<MenuKeyBind>().Active)
                {
                    _activeMode = OrbwalkerMode.LastHit;
                }

                if (args.Menu["WaveClear"].GetValue<MenuKeyBind>().Active)
                {
                    _activeMode = OrbwalkerMode.LaneClear;
                }
            };

            #endregion

        }

        #region Minion Stuff

        /// <summary>
        ///     Returns the best killable minion (if any)
        /// </summary>
        /// <param name="types"><see cref="MinionTypes" /> as filter</param>
        /// <param name="predictionType"><see cref="HealthPredictionType" /> to support both lane clear/last hit</param>
        /// <param name="adModifier">A modifier value for the lane freeze, passed as <see cref="float" /></param>
        /// <returns></returns>
        private static Obj_AI_Base GetKillableMinion(MinionTypes types = MinionTypes.All,
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
        private static bool ShouldWait()
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
        private static void Attack(AttackableUnit target, bool setAttackTime = false)
        {
            ObjectHandler.Player.IssueOrder(GameObjectOrder.AttackUnit, target);

            if (setAttackTime)
            {
                _lastAutoAttack = Variables.TickCount - (Game.Ping / 2);
            }
        }

        /// <summary>
        ///     Returns if our hero is currently able to perform an auto attack
        /// </summary>
        /// <returns></returns>
        public static bool CanAttack()
        {
            if (_lastAutoAttack <= Variables.TickCount)
            {
                return Variables.TickCount + (Game.Ping / 2) >= _lastAutoAttack + (ObjectHandler.Player.AttackDelay * 1000);
            }

            return false;
        }

        /// <summary>
        ///     Returns if our hero is currently able to perform the orbwalking process on another <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="target">Type of <see cref="AttackableUnit" /></param>
        /// <returns></returns>
        public static bool CanGetOrbwalked(this AttackableUnit target)
        {
            return target != null && target.IsValidTarget() && target.InAutoAttackRange();
        }

        /// <summary>
        ///     Returns if our hero is currently able to move and not attacking
        /// </summary>
        /// <returns></returns>
        public static bool CanMove()
        {
            if (_lastAutoAttack <= Variables.TickCount && !_missleLaunched)
            {
                return ObjectManager.Player.CanCancelAutoAttack()
                    ? Variables.TickCount + (Game.Ping / 2) >= _lastAutoAttack + (ObjectManager.Player.AttackCastDelay * 1000) + 20
                    : Variables.TickCount - _lastAutoAttack > 250;
            }

            return false;
        }

        /// <summary>
        ///     Wrapper function to move to an extended position
        /// </summary>
        /// <param name="position"><see cref="Vector3" /> position</param>
        /// <param name="distance">Extend distance from the given position</param>
        private static void MoveTo(Vector3 position, float distance = 500)
        {
            ObjectHandler.Player.IssueOrder(
                GameObjectOrder.MoveTo, ObjectHandler.Player.Position.Extend(position, distance));
        }

        /// <summary>
        ///     Performs the orbwalking process on another <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="mode"><see cref="OrbwalkerMode" /> mode</param>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
        public static void PerformMode(OrbwalkerMode mode, AttackableUnit target = null)
        {
            if (CanAttack())
            {
                switch (mode)
                {
                    case OrbwalkerMode.Combo:
                        PerformModeCombo(target);
                        break;

                    case OrbwalkerMode.LaneClear:
                        PerformModeLaneClear(target);
                        break;

                    case OrbwalkerMode.LaneFreeze:
                        PerformModeLaneFreeze();
                        break;

                    case OrbwalkerMode.LastHit:
                        PerformModeLastHit();
                        break;

                    case OrbwalkerMode.Mixed:
                        PerformModeMixed();
                        break;
                }
            }

            if (mode != OrbwalkerMode.None && CanMove())
            {
                MoveTo(Game.CursorPos);
            }
        }

        /// <summary>
        ///     Resets the auto attack timer, which results in an instant attack again (if possible) or stuttering (if used wrong)
        /// </summary>
        public static void ResetSwingTimer()
        {
            _lastAutoAttack = 0;
        }

        #region Orbwalker Modes

        /// <summary>
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> target (usually <see cref="Obj_AI_Hero" />)</param>
        private static void PerformModeCombo(AttackableUnit target = null)
        {
            if (CanAttack() && target.CanGetOrbwalked())
            {
                Attack(target);
            }
        }

        /// <summary>
        ///     Internal function for the lane clear logic
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> Optional target (usually <see cref="Obj_AI_Hero" />)</param>
        private static void PerformModeLaneClear(AttackableUnit target = null)
        {
            if (target.CanGetOrbwalked() && !ShouldWait())
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
        private static void PerformModeLaneFreeze(AttackableUnit target = null)
        {
            if (target.CanGetOrbwalked() && !ShouldWait())
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
        private static void PerformModeLastHit()
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
        private static void PerformModeMixed(AttackableUnit target = null)
        {
            if (target.CanGetOrbwalked() && !ShouldWait())
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
            PerformMode(OrbwalkerMode.Combo, ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(hero => !hero.IsMe && hero.IsEnemy));
        }

        /// <summary>
        ///     Game.OnCreate Event to detect missiles
        /// </summary>
        /// <param name="sender"><see cref="GameObject" /> sender</param>
        /// <param name="args"><see cref="EventArgs" /> args</param>
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.Type == GameObjectType.MissileClient)
            {
                var missile = sender as MissileClient;

                if (missile != null && missile.SpellCaster.IsMe && AutoAttack.IsAutoAttack(missile.SData.Name))
                {
                    _missleLaunched = false;
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
            if (sender.IsMe)
            {
                var spellName = args.SData.Name;

                if (AutoAttack.IsAutoAttack(spellName))
                {
                    _lastAutoAttack = Variables.TickCount - (Game.Ping / 2);
                }

                if (AutoAttack.IsAutoAttackReset(spellName))
                {
                    DelayAction.Add(250, ResetSwingTimer);
                }

                if (AutoAttack.IsAutoAttack(spellName))
                {
                    _missleLaunched = true;
                }
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
                //_missleLaunched = false;
            }
        }

        #endregion
    }
}