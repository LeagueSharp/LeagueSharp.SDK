// <copyright file="Orbwalker.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using Enumerations;
    using Extensions;
    using Extensions.SharpDX;
    using Math.Prediction;
    using UI.IMenu.Values;
    using Utils;
    using Wrappers;
    using Wrappers.Damages;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Menu = UI.IMenu.Menu;

    /// <summary>
    ///     The <c>orbwalker</c> system.
    /// </summary>
    public class Orbwalker
    {
        #region Static Fields

        /// <summary>
        ///     The attack time tracking list.
        /// </summary>
        private static readonly IDictionary<float, OrbwalkerActionArgs> AfterAttackTime =
            new Dictionary<float, OrbwalkerActionArgs>();

        /// <summary>
        ///     The <c>orbwalker</c> menu.
        /// </summary>
        private static readonly Menu Menu = new Menu("orbwalker", "Orbwalker");

        private static readonly string[] SpecialMinions =
            {
                "zyrathornplant", "zyragraspingplant", "heimertyellow",
                "heimertblue", "malzaharvoidling", "yorickdecayedghoul",
                "yorickravenousghoul", "yorickspectralghoul", "shacobox"
            };

        /// <summary>
        ///     Value indicating whether the <see cref="Orbwalker" /> is enabled.
        /// </summary>
        private static bool enabled;

        #endregion

        #region Delegates

        /// <summary>
        ///     The<see cref="OnAction" /> event delegate.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        public delegate void OnActionDelegate(object sender, OrbwalkerActionArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     The OnAction event.
        /// </summary>
        public static event OnActionDelegate OnAction;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the active mode.
        /// </summary>
        public static OrbwalkerMode ActiveMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether attack.
        /// </summary>
        public static bool Attack { get; set; }

        /// <summary>
        ///     Gets a value indicating whether can attack.
        /// </summary>
        public static bool CanAttack
            =>
                Variables.TickCount + (Game.Ping / 2) + 25
                >= LastAutoAttackTick + (GameObjects.Player.AttackDelay * 1000) && Attack;

        /// <summary>
        ///     Gets a value indicating whether can move.
        /// </summary>
        public static bool CanMove
        {
            get
            {
                if (!Movement)
                {
                    return false;
                }

                if (MissileLaunched && Menu["advanced"]["miscMissile"].GetValue<MenuBool>().Value)
                {
                    return true;
                }

                return !GameObjects.Player.CanCancelAutoAttack()
                       || (Variables.TickCount + (Game.Ping / 2)
                           >= LastAutoAttackTick + (GameObjects.Player.AttackCastDelay * 1000)
                           + Menu["advanced"]["miscExtraWindup"].GetValue<MenuSlider>().Value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="Orbwalker" /> is enabled.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
                if (Menu != null)
                {
                    Menu["enableOption"].GetValue<MenuBool>().Value = value;
                }
            }
        }

        /// <summary>
        ///     Gets the last auto attack tick.
        /// </summary>
        public static int LastAutoAttackTick { get; private set; }

        /// <summary>
        ///     Gets the last minion.
        /// </summary>
        public static Obj_AI_Minion LastMinion { get; private set; }

        /// <summary>
        ///     Gets the last movement order tick.
        /// </summary>
        public static int LastMovementOrderTick { get; internal set; }

        /// <summary>
        ///     Gets the last target.
        /// </summary>
        public static AttackableUnit LastTarget { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether movement.
        /// </summary>
        public static bool Movement { get; set; }

        /// <summary>
        ///     Gets or sets the <c>orbwalk</c> position.
        /// </summary>
        public static Vector3 OrbwalkPosition { get; set; }

        /// <summary>
        ///     Gets or sets the <c>orbwalk</c> target.
        /// </summary>
        public static AttackableUnit OrbwalkTarget { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether missile launched.
        /// </summary>
        private static bool MissileLaunched { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the best target candidate.
        /// </summary>
        /// <param name="modeArg">
        ///     The requested mode
        /// </param>
        /// <returns>
        ///     The <see cref="AttackableUnit" />.
        /// </returns>
        public static AttackableUnit GetTarget(OrbwalkerMode? modeArg)
        {
            var mode = modeArg ?? ActiveMode;

            if (mode == OrbwalkerMode.Orbwalk || mode == OrbwalkerMode.None
                || (mode == OrbwalkerMode.Hybrid && !Menu["advanced"]["miscPriorizeFarm"].GetValue<MenuBool>().Value))
            {
                var target = TargetSelector.GetTarget();
                if (target != null)
                {
                    return target;
                }
            }

            if (mode == OrbwalkerMode.LaneClear || mode == OrbwalkerMode.Hybrid || mode == OrbwalkerMode.LastHit)
            {
                foreach (var minion in
                    GameObjects.EnemyMinions.Where(
                        m =>
                        m.IsValidTarget(m.GetRealAutoAttackRange())
                        && m.Health < 2 * GameObjects.Player.TotalAttackDamage).OrderByDescending(m => m.MaxHealth))
                {
                    var time =
                        (int)
                        ((GameObjects.Player.AttackCastDelay * 1000)
                         + (GameObjects.Player.Distance(minion) / GameObjects.Player.GetProjectileSpeed() * 1000)
                         + (Game.Ping / 2f));
                    var healthPrediction = Health.GetPrediction(minion, time, 100);

                    if (healthPrediction <= 0)
                    {
                        InvokeAction(
                            new OrbwalkerActionArgs
                                {
                                    Position = minion.Position, Target = minion, Process = true,
                                    Type = OrbwalkerType.NonKillableMinion
                                });
                    }

                    if (healthPrediction > 0 && healthPrediction <= GameObjects.Player.GetAutoAttackDamage(minion))
                    {
                        return minion;
                    }
                }
            }

            if (mode == OrbwalkerMode.LaneClear)
            {
                foreach (var turret in GameObjects.EnemyTurrets.Where(t => t.IsValidTarget(t.GetRealAutoAttackRange())))
                {
                    return turret;
                }

                foreach (
                    var inhibitor in GameObjects.EnemyInhibitors.Where(i => i.IsValidTarget(i.GetRealAutoAttackRange()))
                    )
                {
                    return inhibitor;
                }

                if (GameObjects.EnemyNexus.IsValidTarget(GameObjects.EnemyNexus.GetRealAutoAttackRange()))
                {
                    return GameObjects.EnemyNexus;
                }
            }

            if (mode != OrbwalkerMode.LastHit)
            {
                var target = TargetSelector.GetTarget();
                if (target != null)
                {
                    return target;
                }
            }

            if (ActiveMode == OrbwalkerMode.LaneClear)
            {
                var shouldWait =
                    GameObjects.EnemyMinions.Any(
                        m =>
                        m.IsValidTarget(m.GetRealAutoAttackRange())
                        && Health.GetPrediction(m, (int)((GameObjects.Player.AttackDelay * 1000) * 2f), 100)
                        <= GameObjects.Player.GetAutoAttackDamage(m));
                if (!shouldWait)
                {
                    // H-28G, Sumon Voidling, Jack In The Box, (Clyde, Inky, Blinky), Plant
                    foreach (var specialMinion in
                        GameObjects.EnemyMinions.Where(
                            m =>
                            m.IsValidTarget(m.GetRealAutoAttackRange())
                            && SpecialMinions.Any(s => s.Equals(m.CharData.BaseSkinName))))
                    {
                        return specialMinion;
                    }

                    // Jungle mob.
                    var mob =
                        (GameObjects.JungleLegendary.FirstOrDefault(j => j.IsValidTarget(j.GetRealAutoAttackRange()))
                         ?? GameObjects.JungleSmall.FirstOrDefault(
                             j =>
                             j.IsValidTarget(j.GetRealAutoAttackRange()) && j.Name.Contains("Mini")
                             && j.Name.Contains("SRU_Razorbeak"))
                         ?? GameObjects.JungleLarge.FirstOrDefault(j => j.IsValidTarget(j.GetRealAutoAttackRange())))
                        ?? GameObjects.JungleSmall.FirstOrDefault(j => j.IsValidTarget(j.GetRealAutoAttackRange()));
                    if (mob != null)
                    {
                        return mob;
                    }

                    // Sentinel
                    foreach (var sentinel in
                        GameObjects.EnemyMinions.Where(
                            m =>
                            m.IsValidTarget(m.GetRealAutoAttackRange()) && m.CharData.BaseSkinName == "kalistaspawn"))
                    {
                        return sentinel;
                    }

                    // Last Minion
                    if (LastMinion.IsValidTarget(LastMinion.GetRealAutoAttackRange()))
                    {
                        var predHealth = Health.GetPrediction(
                            LastMinion,
                            (int)((GameObjects.Player.AttackDelay * 1000) * 2f),
                            100);
                        if (predHealth >= 2 * GameObjects.Player.GetAutoAttackDamage(LastMinion)
                            || System.Math.Abs(predHealth - LastMinion.Health) < float.Epsilon)
                        {
                            return LastMinion;
                        }
                    }

                    // Minion
                    var minion = (from m in
                                      GameObjects.EnemyMinions.Where(m => m.IsValidTarget(m.GetRealAutoAttackRange()))
                                  let predictedHealth =
                                      Health.GetPrediction(m, (int)((GameObjects.Player.AttackDelay * 1000) * 2f), 100)
                                  where
                                      predictedHealth >= 2 * GameObjects.Player.GetAutoAttackDamage(m)
                                      || System.Math.Abs(predictedHealth - m.Health) < float.Epsilon
                                  select m).MaxOrDefault(m => m.Health);
                    if (minion != null)
                    {
                        return LastMinion = minion;
                    }

                    // Elise Spiderlings
                    return
                        GameObjects.EnemyMinions.FirstOrDefault(
                            m =>
                            m.IsValidTarget(m.GetRealAutoAttackRange()) && m.CharData.BaseSkinName == "elisespiderling");
                }
            }

            return null;
        }

        /// <summary>
        ///     Orders a move onto the player, with the <c>orbwalker</c> parameters.
        /// </summary>
        /// <param name="position">
        ///     The position to <c>orbwalk</c> to
        /// </param>
        /// <param name="overrideTimer">
        ///     Indicates whether to override the timer.
        /// </param>
        public static void MoveOrder(Vector3 position, bool overrideTimer = false)
        {
            if (Variables.TickCount - LastMovementOrderTick
                < Menu["advanced"]["movementDelay"].GetValue<MenuSlider>().Value && !overrideTimer)
            {
                return;
            }

            LastMovementOrderTick = Variables.TickCount;

            if (position.Distance(GameObjects.Player.Position)
                > GameObjects.Player.BoundingRadius + Menu["advanced"]["movementExtraHold"].GetValue<MenuSlider>().Value)
            {
                if (position.Distance(GameObjects.Player.Position)
                    > Menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>().Value)
                {
                    var menuItem = Menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>();

                    var randomDistance = new Random(Variables.TickCount).Next(0, 50);
                    position = menuItem.Value - randomDistance <= GameObjects.Player.BoundingRadius
                                   ? GameObjects.Player.Position.Extend(
                                       position,
                                       GameObjects.Player.BoundingRadius + randomDistance)
                                   : GameObjects.Player.Position.Extend(position, menuItem.Value - randomDistance);
                }

                if (Menu["advanced"]["movementScramble"].GetValue<MenuBool>().Value)
                {
                    var random = new Random(Variables.TickCount);
                    var angle = 2D * System.Math.PI * random.NextDouble();
                    var radius = GameObjects.Player.Distance(Game.CursorPos) < 360
                                     ? 0F
                                     : GameObjects.Player.BoundingRadius / 2f;
                    var x = (float)(position.X + (radius * System.Math.Cos(angle)));
                    var y = (float)(position.Y + (radius * System.Math.Sin(angle)));
                    position = new Vector3(x, y, NavMesh.GetHeightForPosition(x, y));
                }

                var eventArgs = new OrbwalkerActionArgs
                                    { Position = position, Process = true, Type = OrbwalkerType.Movement };
                InvokeAction(eventArgs);

                if (eventArgs.Process)
                {
                    GameObjects.Player.IssueOrder(GameObjectOrder.MoveTo, eventArgs.Position);
                }
            }
        }

        /// <summary>
        ///     <c>Orbwalk</c> command, attempting to attack or move.
        /// </summary>
        /// <param name="target">
        ///     The target of choice
        /// </param>
        /// <param name="position">
        ///     The position of choice
        /// </param>
        public static void Orbwalk(AttackableUnit target = null, Vector3? position = null)
        {
            if (CanAttack)
            {
                var gTarget = target ?? GetTarget(ActiveMode);
                if (gTarget.IsValidTarget())
                {
                    var eventArgs = new OrbwalkerActionArgs
                                        {
                                            Target = gTarget, Position = gTarget.Position, Process = true,
                                            Type = OrbwalkerType.BeforeAttack
                                        };
                    InvokeAction(eventArgs);

                    if (eventArgs.Process)
                    {
                        if (GameObjects.Player.CanCancelAutoAttack())
                        {
                            LastAutoAttackTick = Variables.TickCount + Game.Ping + 250
                                                 - (int)(GameObjects.Player.AttackCastDelay * 1000);
                            MissileLaunched = false;
                        }

                        GameObjects.Player.IssueOrder(GameObjectOrder.AttackUnit, gTarget);
                        LastTarget = gTarget;
                        return;
                    }
                }
            }

            if (CanMove)
            {
                MoveOrder(position.HasValue && position.Value.IsValid() ? position.Value : Game.CursorPos);
            }
        }

        /// <summary>
        ///     Resets the auto attack timer, <see cref="LastAutoAttackTick" />.
        /// </summary>
        public static void ResetAutoAttackTimer()
        {
            LastAutoAttackTick = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize method of the <c>orbwalker</c>.
        /// </summary>
        /// <param name="menu">
        ///     The LeagueSharp menu
        /// </param>
        internal static void Initialize(Menu menu)
        {
            var drawing = new Menu("drawings", "Drawings");
            drawing.Add(new MenuBool("drawAARange", "Draw Auto-Attack Range", true));
            drawing.Add(new MenuBool("drawKillableMinion", "Draw Killable Minion"));
            drawing.Add(new MenuBool("drawKillableMinionFade", "Enable Killable Minion Fade Effect"));
            Menu.Add(drawing);

            var advanced = new Menu("advanced", "Advanced");
            advanced.Add(new MenuSeparator("separatorMovement", "Movement"));
            advanced.Add(
                new MenuSlider(
                    "movementDelay",
                    "Delay between Movement",
                    new Random(Variables.TickCount).Next(80, 121),
                    0,
                    500));
            advanced.Add(new MenuBool("movementScramble", "Randomize movement location", true));
            advanced.Add(new MenuSlider("movementExtraHold", "Extra Hold Position", 25, 0, 250));
            advanced.Add(
                new MenuSlider(
                    "movementMaximumDistance",
                    "Maximum Movement Distance",
                    new Random().Next(500, 1201),
                    350,
                    1200));
            advanced.Add(new MenuSeparator("separatorMisc", "Miscellaneous"));
            advanced.Add(new MenuSlider("miscExtraWindup", "Extra Windup", 80, 0, 200));
            advanced.Add(new MenuSlider("miscFarmDelay", "Farm Delay", 0, 0, 200));
            advanced.Add(new MenuBool("miscPriorizeFarm", "Priorize farm over harass", true));
            advanced.Add(new MenuBool("miscMissile", "Use Missile Checks", true));
            advanced.Add(new MenuSeparator("separatorOther", "Other"));
            advanced.Add(
                new MenuButton("resetAll", "Settings", "Reset All Settings")
                    {
                        Action = () =>
                            {
                                Menu.RestoreDefault();
                                Menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>().Value = new Random().Next(
                                    500,
                                    1201);
                                Menu["advanced"]["movementDelay"].GetValue<MenuSlider>().Value = new Random().Next(80, 121);
                            }
                    });
            Menu.Add(advanced);
            Menu.Add(new MenuSeparator("separatorKeys", "Key Bindings"));
            Menu.Add(new MenuBool("enableOption", "Enable Orbwalker", true));
            Menu.Add(new MenuKeyBind("lasthitKey", "Farm", Keys.X, KeyBindType.Press));
            Menu.Add(new MenuKeyBind("laneclearKey", "Lane Clear", Keys.V, KeyBindType.Press));
            Menu.Add(new MenuKeyBind("hybridKey", "Hybrid", Keys.C, KeyBindType.Press));
            Menu.Add(new MenuKeyBind("orbwalkKey", "Orbwalk", Keys.Space, KeyBindType.Press));

            Menu.MenuValueChanged += (sender, args) =>
                {
                    var keyBind = sender as MenuKeyBind;
                    if (keyBind != null)
                    {
                        var modeName = keyBind.Name.Substring(0, keyBind.Name.IndexOf("Key", StringComparison.Ordinal));
                        OrbwalkerMode mode;
                        ActiveMode = Enum.TryParse(modeName, true, out mode)
                                         ? keyBind.Active
                                               ? mode
                                               : mode == ActiveMode
                                                     ? Menu["lasthitKey"].GetValue<MenuKeyBind>().Active
                                                           ? OrbwalkerMode.LastHit
                                                           : Menu["laneclearKey"].GetValue<MenuKeyBind>().Active
                                                                 ? OrbwalkerMode.LaneClear
                                                                 : Menu["hybridKey"].GetValue<MenuKeyBind>().Active
                                                                       ? OrbwalkerMode.Hybrid
                                                                       : Menu["orbwalkKey"].GetValue<MenuKeyBind>()
                                                                             .Active
                                                                             ? OrbwalkerMode.Orbwalk
                                                                             : OrbwalkerMode.None
                                                     : ActiveMode
                                         : ActiveMode;
                    }

                    var boolean = sender as MenuBool;
                    if (boolean != null)
                    {
                        if (boolean.Name.Equals("enableOption"))
                        {
                            enabled = boolean.Value;
                        }
                    }
                };

            menu.Add(Menu);

            Movement = Attack = true;
            enabled = Menu["enableOption"].GetValue<MenuBool>().Value;

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += OnCreate;
        }

        /// <summary>
        ///     The <see cref="OnAction" /> invocator.
        /// </summary>
        /// <param name="e">
        ///     The event data.
        /// </param>
        protected static void InvokeAction(OrbwalkerActionArgs e)
        {
            OnAction?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, e);
        }

        /// <summary>
        ///     OnCreate event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster.IsMe && AutoAttack.IsAutoAttack(missile.SData.Name))
            {
                MissileLaunched = true;
            }
        }

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnDraw(EventArgs args)
        {
            if (GameObjects.Player == null || !GameObjects.Player.IsValid || !Enabled)
            {
                return;
            }

            if (Menu["drawings"]["drawAARange"].GetValue<MenuBool>().Value)
            {
                if (GameObjects.Player.Position.IsValid())
                {
                    Drawing.DrawCircle(
                        GameObjects.Player.Position,
                        GameObjects.Player.GetRealAutoAttackRange(),
                        Color.Blue);
                }
            }

            if (Menu["drawings"]["drawKillableMinion"].GetValue<MenuBool>().Value)
            {
                if (Menu["drawings"]["drawKillableMinionFade"].GetValue<MenuBool>().Value)
                {
                    var minions =
                        GameObjects.EnemyMinions.Where(
                            m => m.IsValidTarget(1200F) && m.Health < GameObjects.Player.GetAutoAttackDamage(m) * 2);
                    foreach (var minion in minions)
                    {
                        var value = 255 - (minion.Health * 2);
                        value = value > 255 ? 255 : value < 0 ? 0 : value;

                        Drawing.DrawCircle(
                            minion.Position,
                            minion.BoundingRadius * 2f,
                            Color.FromArgb(255, 0, 255, (byte)(255 - value)));
                    }
                }
                else
                {
                    var minions =
                        GameObjects.EnemyMinions.Where(
                            m => m.IsValidTarget(1200F) && m.Health < GameObjects.Player.GetAutoAttackDamage(m));
                    foreach (var minion in minions)
                    {
                        Drawing.DrawCircle(minion.Position, minion.BoundingRadius * 2f, Color.FromArgb(255, 0, 255, 0));
                    }
                }
            }
        }

        /// <summary>
        ///     OnProcessSpellCast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            if (sender.IsMe)
            {
                var spellName = args.SData.Name;
                var target = args.Target as AttackableUnit;

                if (target != null && target.IsValid && AutoAttack.IsAutoAttack(spellName))
                {
                    LastAutoAttackTick = Variables.TickCount - (Game.Ping / 2);
                    MissileLaunched = false;

                    if (!target.Compare(LastTarget))
                    {
                        InvokeAction(new OrbwalkerActionArgs { Target = target, Type = OrbwalkerType.TargetSwitch });

                        LastTarget = target;
                    }

                    var time = Variables.TickCount + (sender.AttackCastDelay * 1000) + 40;
                    if (!AfterAttackTime.ContainsKey(time))
                    {
                        AfterAttackTime.Add(
                            time,
                            new OrbwalkerActionArgs { Target = target, Type = OrbwalkerType.AfterAttack });
                    }

                    InvokeAction(
                        new OrbwalkerActionArgs { Target = target, Sender = sender, Type = OrbwalkerType.OnAttack });
                }

                if (AutoAttack.IsAutoAttackReset(spellName))
                {
                    ResetAutoAttackTimer();
                }
            }
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            if (!Enabled)
            {
                return;
            }

            if (ActiveMode != OrbwalkerMode.None)
            {
                Orbwalk(OrbwalkTarget, OrbwalkPosition);
            }

            foreach (var item in AfterAttackTime.ToArray().Where(item => Variables.TickCount - item.Key >= 0))
            {
                InvokeAction(item.Value);
                AfterAttackTime.Remove(item);
            }
        }

        #endregion

        /// <summary>
        ///     The <c>orbwalker</c> action event data.
        /// </summary>
        public class OrbwalkerActionArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the position.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether process.
            /// </summary>
            public bool Process { get; set; }

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            public Obj_AI_Base Sender { get; set; }

            /// <summary>
            ///     Gets or sets the target.
            /// </summary>
            public AttackableUnit Target { get; set; }

            /// <summary>
            ///     Gets the type.
            /// </summary>
            public OrbwalkerType Type { get; internal set; }

            #endregion
        }
    }
}