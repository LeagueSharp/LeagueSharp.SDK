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
//   <c>Orbwalker</c> part that contains the internal system functionality of the <c>orbwalker</c>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;

    /// <summary>
    ///     <c>Orbwalker</c> part that contains the internal system functionality of the <c>orbwalker</c>.
    /// </summary>
    public partial class Orbwalker
    {
        #region Static Fields

        /// <summary>
        ///     The local attack value.
        /// </summary>
        private static bool attack = true;

        /// <summary>
        ///     The local last auto attack tick value.
        /// </summary>
        private static int lastAutoAttackTick;

        /// <summary>
        ///     The local last movement order tick value.
        /// </summary>
        private static int lastMovementOrderTick = Variables.TickCount;

        /// <summary>
        ///     The local movement value.
        /// </summary>
        private static bool movement = true;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the active mode.
        /// </summary>
        public static OrbwalkerMode ActiveMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether attack.
        /// </summary>
        public static bool Attack
        {
            get
            {
                return attack
                       && Variables.TickCount + (Game.Ping / 2)
                       >= lastAutoAttackTick + (Player.AttackDelay * 1000)
                       + menu["advanced"]["miscExtraWindup"].GetValue<MenuSlider>().Value;
            }

            set
            {
                attack = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether movement.
        /// </summary>
        public static bool Movement
        {
            get
            {
                return movement && !InterruptableSpell.IsCastingInterruptableSpell(Player, true)
                       && (!IsMissileLaunched
                           || (Player.CanCancelAutoAttack()
                                   ? Variables.TickCount + (Game.Ping / 2)
                                     >= lastAutoAttackTick + (Player.AttackCastDelay * 1000) + 20
                                   : Variables.TickCount - lastAutoAttackTick > 250));
            }

            set
            {
                movement = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether is missile launched.
        /// </summary>
        private static bool IsMissileLaunched { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        private static Obj_AI_Hero Player
        {
            get
            {
                return GameObjects.Player;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     On create event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile != null && missile.SpellCaster.IsMe && AutoAttack.IsAutoAttack(missile.SData.Name))
            {
                IsMissileLaunched = false;
            }
        }

        /// <summary>
        ///     On draw event.
        /// </summary>
        /// <param name="e">
        ///     The event data
        /// </param>
        private static void OnDraw(EventArgs e)
        {
            if (Player == null || !Player.IsValid)
            {
                return;
            }

            if (menu["drawings"]["drawAARange"].GetValue<MenuBool>().Value)
            {
                if (Player.Position.IsValid())
                {
                    Drawing.DrawCircle(Player.Position, Player.GetRealAutoAttackRange(), Color.Blue);
                }
            }

            if (menu["drawings"]["drawKillableMinion"].GetValue<MenuBool>().Value)
            {
                if (menu["drawings"]["drawKillableMinionFade"].GetValue<MenuBool>().Value)
                {
                    var minions =
                        GameObjects.EnemyMinions.Where(
                            m => m.IsValidTarget(1200F) && m.Health < Player.GetAutoAttackDamage(m, true) * 2);
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
                            m => m.IsValidTarget(1200F) && m.Health < Player.GetAutoAttackDamage(m, true));
                    foreach (var minion in minions)
                    {
                        Drawing.DrawCircle(minion.Position, minion.BoundingRadius * 2f, Color.FromArgb(255, 0, 255, 0));
                    }
                }
            }
        }

        /// <summary>
        ///     On process spell cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                var spellName = args.SData.Name;

                if (AutoAttack.IsAutoAttack(spellName))
                {
                    lastAutoAttackTick = Variables.TickCount - (Game.Ping / 2);
                }

                if (AutoAttack.IsAutoAttackReset(spellName))
                {
                    DelayAction.Add(250, () => { lastAutoAttackTick = 0; });
                }
            }
        }

        /// <summary>
        ///     On game update event.
        /// </summary>
        /// <param name="e">
        ///     The event data
        /// </param>
        private static void OnUpdate(EventArgs e)
        {
            if (menu == null || ActiveMode == OrbwalkerMode.None)
            {
                return;
            }

            if (Attack)
            {
                Preform(GetTarget(ActiveMode));
            }

            if (Movement
                && Variables.TickCount - lastMovementOrderTick
                > menu["advanced"]["movementDelay"].GetValue<MenuSlider>().Value)
            {
                MoveOrder(Game.CursorPos.SetZ());
            }
        }

        #endregion
    }

    /// <summary>
    ///     <c>Orbwalker</c> part that contains the internal modes functionality of the <c>orbwalker</c>.
    /// </summary>
    public partial class Orbwalker
    {
        #region Methods

        /// <summary>
        ///     Preforms the <c>orbwalker</c> action.
        /// </summary>
        /// <param name="target">
        ///     A target to attack.
        /// </param>
        private static void Preform(AttackableUnit target)
        {
            if (target.IsValidTarget()
                && Player.DistanceSquared(target) <= System.Math.Pow(target.GetRealAutoAttackRange(), 2))
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        #endregion
    }

    /// <summary>
    ///     <c>Orbwalker</c> part that contains the internal menu functionality of the <c>orbwalker</c>.
    /// </summary>
    public partial class Orbwalker
    {
        #region Static Fields

        /// <summary>
        ///     The menu handle.
        /// </summary>
        private static Menu menu;

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the <c>Orbwalker</c>, starting from the menu.
        /// </summary>
        /// <param name="rootMenu">
        ///     The parent menu.
        /// </param>
        internal static void Initialize(Menu rootMenu)
        {
            menu = new Menu("orbwalker", "Orbwalker");

            menu.Add(new Menu("resetSpells", "Reset Spells"));
            menu.Add(new Menu("items", "Items"));

            var drawing = new Menu("drawings", "Drawings");
            drawing.Add(new MenuBool("drawAARange", "Draw Auto-Attack Range", true));
            drawing.Add(new MenuBool("drawTargetAARange", "Draw Target Auto-Attack Range"));
            drawing.Add(new MenuBool("drawKillableMinion", "Draw Killable Minion"));
            drawing.Add(new MenuBool("drawKillableMinionFade", "Enable Killable Minion Fade Effect"));
            menu.Add(drawing);

            var advanced = new Menu("advanced", "Advanced");
            advanced.Add(new MenuSeparator("separatorMovement", "Movement"));
            advanced.Add(new MenuSlider("movementDelay", "Delay between Movement", new Random(Variables.TickCount).Next(200, 301), 0, 2500));
            advanced.Add(new MenuBool("movementScramble", "Randomize movement location", true));
            advanced.Add(new MenuSlider("movementExtraHold", "Extra Hold Position", 25, 0, 250));
            advanced.Add(new MenuSlider("movementMaximumDistance", "Maximum Movement Distance", new Random().Next(500, 1201), 0, 1200));
            advanced.Add(new MenuSeparator("separatorMisc", "Miscellaneous"));
            advanced.Add(new MenuSlider("miscExtraWindup", "Extra Windup", 80, 0, 200));
            advanced.Add(new MenuSlider("miscFarmDelay", "Farm Delay", 0, 0, 200));
            advanced.Add(new MenuSeparator("separatorOther", "Other"));
            advanced.Add(
                new MenuButton("resetAll", "Settings", "Reset All Settings")
                    {
                       Action = ResetSettings 
                    });
            menu.Add(advanced);

            menu.Add(new MenuSeparator("separatorKeys", "Key Bindings"));
            menu.Add(new MenuKeyBind("lasthitKey", "Farm", Keys.X, KeyBindType.Press));
            menu.Add(new MenuKeyBind("laneclearKey", "Lane Clear", Keys.V, KeyBindType.Press));
            menu.Add(new MenuKeyBind("hybridKey", "Hybrid", Keys.C, KeyBindType.Press));
            menu.Add(new MenuKeyBind("orbwalkKey", "Orbwalk", Keys.Space, KeyBindType.Press));

            menu.MenuValueChanged += (sender, args) =>
                {
                    var keyBind = sender as MenuKeyBind;
                    if (keyBind != null)
                    {
                        var modeName = keyBind.Name.Substring(0, keyBind.Name.IndexOf("Key", StringComparison.Ordinal));
                        OrbwalkerMode mode;
                        if (Enum.TryParse(modeName, true, out mode))
                        {
                            if (keyBind.Active)
                            {
                                ActiveMode = mode;
                            }
                            else
                            {
                                if (mode == ActiveMode)
                                {
                                    ActiveMode = OrbwalkerMode.None;
                                }
                            }
                        }
                    }
                };

            rootMenu.Add(menu);

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        /// <summary>
        ///     Resets the <c>orbwalker</c> settings.
        /// </summary>
        private static void ResetSettings()
        {
            menu["advanced"]["movementDelay"].GetValue<MenuSlider>().Value = new Random(Variables.TickCount).Next(
                200, 
                301);
            menu["advanced"]["movementScramble"].GetValue<MenuBool>().Value = true;
            menu["advanced"]["movementExtraHold"].GetValue<MenuSlider>().Value = 25;
            menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>().Value = new Random().Next(500, 1201);
            menu["advanced"]["miscExtraWindup"].GetValue<MenuSlider>().Value = 80;
            menu["advanced"]["miscFarmDelay"].GetValue<MenuSlider>().Value = 0;
            menu["lasthitKey"].GetValue<MenuKeyBind>().Key = Keys.X;
            menu["laneclearKey"].GetValue<MenuKeyBind>().Key = Keys.V;
            menu["hybridKey"].GetValue<MenuKeyBind>().Key = Keys.C;
            menu["orbwalkKey"].GetValue<MenuKeyBind>().Key = Keys.Space;
        }

        #endregion
    }

    /// <summary>
    ///     <c>Orbwalker</c> part that contains the external API functionality of the <c>orbwalker</c>.
    /// </summary>
    public partial class Orbwalker
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns a target based on the <c>OrbwalkerMode</c>.
        /// </summary>
        /// <param name="mode">
        ///     The <c>OrbwalkerMode</c>
        /// </param>
        /// <returns>
        ///     The target in a <c>AttackableUnit</c> instance
        /// </returns>
        public static AttackableUnit GetTarget(OrbwalkerMode mode = OrbwalkerMode.None)
        {
            switch (mode)
            {
                case OrbwalkerMode.None:
                case OrbwalkerMode.Orbwalk:
                    {
                        return TargetSelector.GetTarget(-1f);
                    }

                default:
                    {
                        break;
                    }
            }

            return null;
        }

        /// <summary>
        ///     Orders a move onto the player, with the <c>orbwalker</c> parameters.
        /// </summary>
        /// <param name="position">
        ///     The position to <c>orbwalk</c> to.
        /// </param>
        public static void MoveOrder(Vector3 position)
        {
            if (position.Distance(Player.Position)
                > Player.BoundingRadius + menu["advanced"]["movementExtraHold"].GetValue<MenuSlider>().Value)
            {
                if (position.Distance(Player.Position)
                    > menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>().Value)
                {
                    position = Player.Position.Extend(
                        position, 
                        menu["advanced"]["movementMaximumDistance"].GetValue<MenuSlider>().Value);
                }

                if (menu["advanced"]["movementScramble"].GetValue<MenuBool>().Value)
                {
                    var random = new Random(Variables.TickCount);
                    var angle = 2D * System.Math.PI * random.NextDouble();
                    var radius = Player.Distance(Game.CursorPos) < 360 ? 0F : Player.BoundingRadius * 2F;
                    position =
                        new Vector3(
                            (float)(position.X + radius * System.Math.Cos(angle)), 
                            (float)(position.Y + radius * System.Math.Sin(angle)), 
                            0f).SetZ();
                }

                if (Player.IssueOrder(GameObjectOrder.MoveTo, position))
                {
                    lastMovementOrderTick = Variables.TickCount;
                }
            }
        }

        #endregion
    }
}