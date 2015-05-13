// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Orbwalker.cs" company="LeagueSharp">
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
//   <c>Orbwalker</c> class, provides a utility tool for assemblies to implement an <c>orbwalker</c>.
//   <c>An orbwalker</c> is a tool for
//   performing an auto attack and moving as quickly as possible afterwards.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Managers;
    using LeagueSharp.SDK.Core.Math.Prediction;
    using LeagueSharp.SDK.Core.UI;
    using LeagueSharp.SDK.Core.UI.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;

    using SharpDX;

    using Menu = LeagueSharp.SDK.Core.UI.Menu;

    /// <summary>
    ///     <c>Orbwalker</c> class, provides a utility tool for assemblies to implement an <c>orbwalker</c>.
    ///     <c>An orbwalker</c> is a tool for
    ///     performing an auto attack and moving as quickly as possible afterwards.
    /// </summary>
    public static class Orbwalker
    {
        #region Static Fields

        /// <summary>
        ///     Contains the current mode (<see cref="OrbwalkerMode" />)
        /// </summary>
        private static OrbwalkerMode activeMode = OrbwalkerMode.None;

        /// <summary>
        ///     Contains the time of the last auto attack
        /// </summary>
        private static int lastAutoAttack;

        /// <summary>
        ///     Contains a <see cref="SDK.Core.UI.Menu" />
        /// </summary>
        private static Menu menu;

        /// <summary>
        ///     Returns if we are shooting a missile that isn't visible, yet
        /// </summary>
        private static bool missleLaunched = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Orbwalker" /> class.
        /// </summary>
        static Orbwalker()
        {
            BootstrapMenu();

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns if our hero is currently able to perform an auto attack
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        public static bool CanAttack()
        {
            if (lastAutoAttack <= Variables.TickCount)
            {
                return Variables.TickCount + (Game.Ping / 2)
                       >= lastAutoAttack + (ObjectHandler.Player.AttackDelay * 1000);
            }

            return false;
        }

        /// <summary>
        ///     Returns if our hero is currently able to perform the <c>orbwalking</c> process on another
        ///     <see cref="AttackableUnit" />
        /// </summary>
        /// <param name="target">Type of <see cref="AttackableUnit" /></param>
        /// <returns>The <see cref="bool" /></returns>
        public static bool CanGetOrbwalked(this AttackableUnit target)
        {
            return target != null && target.IsValidTarget() && target.InAutoAttackRange();
        }

        /// <summary>
        ///     Returns if our hero is currently able to move and not attacking
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        public static bool CanMove()
        {
            if (lastAutoAttack <= Variables.TickCount && !missleLaunched)
            {
                return ObjectManager.Player.CanCancelAutoAttack()
                           ? Variables.TickCount + (Game.Ping / 2)
                             >= lastAutoAttack + (ObjectManager.Player.AttackCastDelay * 1000) + 20
                           : Variables.TickCount - lastAutoAttack > 250;
            }

            return false;
        }

        /// <summary>
        ///     Performs the <c>orbwalking</c> process on another <see cref="AttackableUnit" />
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
            lastAutoAttack = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     TODO The bootstrap menu.
        /// </summary>
        internal static void BootstrapMenu()
        {
            menu = new Menu("CommonEx.Orbwalker", "Orbwalker");

            var drawingMenu = new Menu("Drawings", "Drawings");
            drawingMenu.Add(new MenuItem<MenuBool>("HoldZone", "Draw Hold Zone") { Value = new MenuBool() });
            drawingMenu.Add(
                new MenuItem<MenuBool>("AttackedMinion", "Draw Attacked Minion")
                    {
                        Value = new MenuBool(true) // thats redundant :S
                    });
            drawingMenu.Add(new MenuItem<MenuBool>("AutoAttackRange", "Draw AA Range") { Value = new MenuBool(true) });
            menu.Add(drawingMenu);

            menu.Add(
                new MenuItem<MenuKeyBind>("Combo", "Combo") { Value = new MenuKeyBind(Keys.Space, KeyBindType.Press) });
            menu.Add(
                new MenuItem<MenuKeyBind>("Harass", "Harass") { Value = new MenuKeyBind(Keys.C, KeyBindType.Press) });
            menu.Add(
                new MenuItem<MenuKeyBind>("LastHit", "Last Hit") { Value = new MenuKeyBind(Keys.X, KeyBindType.Press) });
            menu.Add(
                new MenuItem<MenuKeyBind>("WaveClear", "Wave Clear")
                    {
                       Value = new MenuKeyBind(Keys.V, KeyBindType.Press) 
                    });
            menu.Add(
                new MenuItem<MenuKeyBind>("Freeze", "Freeze") { Value = new MenuKeyBind(Keys.F1, KeyBindType.Toggle) });

            Variables.LeagueSharpMenu.Add(menu);

            menu.MenuValueChanged += delegate(object sender, OnMenuValueChangedEventArgs args)
                {
                    if (args.Menu["Combo"].GetValue<MenuKeyBind>().Active)
                    {
                        activeMode = OrbwalkerMode.Combo;
                    }

                    if (args.Menu["Harass"].GetValue<MenuKeyBind>().Active)
                    {
                        activeMode = OrbwalkerMode.Mixed;
                    }

                    if (args.Menu["LastHit"].GetValue<MenuKeyBind>().Active)
                    {
                        activeMode = OrbwalkerMode.LastHit;
                    }

                    if (args.Menu["WaveClear"].GetValue<MenuKeyBind>().Active)
                    {
                        activeMode = OrbwalkerMode.LaneClear;
                    }
                };
        }

        /// <summary>
        ///     Wrapper function to attack a target
        /// </summary>
        /// <param name="target">
        ///     <see cref="AttackableUnit" /> target
        /// </param>
        /// <param name="setAttackTime">
        ///     The set Attack Time.
        /// </param>
        private static void Attack(GameObject target, bool setAttackTime = false)
        {
            ObjectHandler.Player.IssueOrder(GameObjectOrder.AttackUnit, target);

            if (setAttackTime)
            {
                lastAutoAttack = Variables.TickCount - (Game.Ping / 2);
            }
        }

        /// <summary>
        ///     Game.OnUpdate Event
        /// </summary>
        /// <param name="args"><see cref="EventArgs" /> args</param>
        private static void Game_OnUpdate(EventArgs args)
        {
            PerformMode(
                OrbwalkerMode.Combo, 
                ObjectManager.Get<Obj_AI_Hero>().FirstOrDefault(hero => !hero.IsMe && hero.IsEnemy));
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
                    missleLaunched = false;
                }
            }
        }

        /// <summary>
        ///     Returns the best killable minion (if any)
        /// </summary>
        /// <param name="types"><see cref="MinionTypes" /> as filter</param>
        /// <param name="predictionType"><see cref="HealthPredictionType" /> to support both lane clear/last hit</param>
        /// <param name="adModifier">A modifier value for the lane freeze, passed as <see cref="float" /></param>
        /// <returns>The kill-able minion as a <see cref="Obj_AI_Base" /> type</returns>
        private static Obj_AI_Base GetKillableMinion(
            MinionTypes types = MinionTypes.All, 
            HealthPredictionType predictionType = HealthPredictionType.Default, 
            double adModifier = 1.0)
        {
            return
                MinionManager.GetMinions(
                    ObjectHandler.Player.ServerPosition, 
                    ObjectHandler.Player.GetRealAutoAttackRange() + 65, 
                    types)
                    .Where(CanGetOrbwalked)
                    .Select(
                        minion =>
                        new
                            {
                                minion, 
                                predictedHealth =
                            Health.GetPrediction(minion, (int)minion.GetTimeToHit(), 20, predictionType)
                            })
                    .Where(
                        @t =>
                        @t.predictedHealth > 0
                        && @t.predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(@t.minion) * adModifier)
                    .Select(@t => @t.minion)
                    .FirstOrDefault();
        }

        /// <summary>
        ///     Wrapper function to move to an extended position
        /// </summary>
        /// <param name="position"><see cref="Vector3" /> position</param>
        /// <param name="distance">Extend distance from the given position</param>
        private static void MoveTo(Vector3 position, float distance = 500)
        {
            ObjectHandler.Player.IssueOrder(
                GameObjectOrder.MoveTo, 
                ObjectHandler.Player.Position.Extend(position, distance));
        }

        /// <summary>
        ///     <c>Obj_AI_Base.OnProcessSpellCast</c> Event to detect auto attacks
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args"><see cref="GameObjectProcessSpellCastEventArgs" /> args</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var spellName = args.SData.Name;

                if (AutoAttack.IsAutoAttack(spellName))
                {
                    lastAutoAttack = Variables.TickCount - (Game.Ping / 2);
                }

                if (AutoAttack.IsAutoAttackReset(spellName))
                {
                    DelayAction.Add(250, ResetSwingTimer);
                }

                if (AutoAttack.IsAutoAttack(spellName))
                {
                    missleLaunched = true;
                }
            }
        }

        /// <summary>
        ///     Commands to preform the combo mode.
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

        /// <summary>
        ///     Returns if a minion is killable soon and we should wait with our attack
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        private static bool ShouldWait()
        {
            return
                MinionManager.GetMinions(
                    ObjectHandler.Player.ServerPosition, 
                    ObjectHandler.Player.GetRealAutoAttackRange() + 65)
                    .Where(CanGetOrbwalked)
                    .Select(
                        minion =>
                        new
                            {
                                minion, 
                                predictedHealth =
                            Health.GetPrediction(
                                minion, 
                                (int)(ObjectHandler.Player.AttackDelay * 1.3), 
                                20, 
                                HealthPredictionType.Simulated)
                            })
                    .Select(
                        @t =>
                        @t.predictedHealth > 0
                        && @t.predictedHealth <= ObjectManager.Player.GetAutoAttackDamage(@t.minion))
                    .FirstOrDefault();
        }

        /// <summary>
        ///     <c>Spellbook.OnStopCast</c> Event to detect canceled auto attacks
        /// </summary>
        /// <param name="sender"><see cref="Spellbook" /> sender</param>
        /// <param name="args"><see cref="SpellbookStopCastEventArgs" /> args</param>
        private static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            if (sender.Owner.IsValid && sender.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                // _missleLaunched = false;
            }
        }

        #endregion
    }
}