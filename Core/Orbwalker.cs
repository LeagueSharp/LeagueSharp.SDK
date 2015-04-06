using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Math.Prediction;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    ///     Orbwalker class, provides a utility tool for assemblies to implement an orbwalker. An orbwalker is a tool for
    ///     performing an auto attack and moving as quickly as possible afterwards.
    /// </summary>
    public class Orbwalker
    {
        /// <summary>
        ///     Orbwalker Attack Event Delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Orbwalker Attack Event Arguments Container</param>
        public delegate void OnAttackEventDelegate(object sender, AttackArgs e);

        /// <summary>
        ///     Orbwalker Event Delegate.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Orbwalker Event Arguments Container</param>
        public delegate void OnOrbwalkDelegate(object sender, OrbwalkerArgs e);

        /// <summary>
        ///     Next Tick on which the Orbwalker should begin to to check once again.
        /// </summary>
        private static int _nextMovementTick = Variables.TickCount;

        /// <summary>
        ///     Returns the last auto attack tick, which is updated when a new auto attack is executed by the player.
        /// </summary>
        private static int _lastAutoAttackTick = Variables.TickCount;

        /// <summary>
        ///     Returns the current Local Player instance.
        /// </summary>
        private static Obj_AI_Hero _player;

        /// <summary>
        ///     Static Constructor.
        /// </summary>
        static Orbwalker()
        {
            Console.Clear();
            ActiveMode = OrbwalkerMode.None;

            Game.OnUpdate += Game_OnUpdate;
            _player = ObjectManager.Player;

            Game.OnWndProc += Game_OnWndProc;
        }

        /// <summary>
        ///     Returns whether the orbwalker is active.
        /// </summary>
        public static bool Active { get; set; }

        /// <summary>
        ///     Returns the orbwalker's current active mode.
        /// </summary>
        public static OrbwalkerMode ActiveMode { get; set; }

        /// <summary>
        ///     Returns if the player is allowed to attack.
        /// </summary>
        public static bool Attack { get; set; }

        /// <summary>
        ///     Returns if the player is allowed to move.
        /// </summary>
        public static bool Move { get; set; }

        /// <summary>
        ///     Blocks the orbwalker commands once.
        /// </summary>
        internal static bool BlockOnce { get; set; }

        /// <summary>
        ///     No menu, temporary for testing.
        /// </summary>
        /// <param name="args"></param>
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (WindowsKeys.GetWindowsMessage(args) == WindowsMessages.KEYDOWN)
            {
                if (ActiveMode != OrbwalkerMode.Combo && WindowsKeys.GetKey(args) == Keys.Space)
                {
                    ActiveMode = OrbwalkerMode.Combo;
                }
                else if (ActiveMode != OrbwalkerMode.Mixed && WindowsKeys.GetKey(args) == Keys.C)
                {
                    ActiveMode = OrbwalkerMode.Mixed;
                }
                else if (ActiveMode != OrbwalkerMode.LaneClear && WindowsKeys.GetKey(args) == Keys.V)
                {
                    ActiveMode = OrbwalkerMode.LaneClear;
                }
                else if (ActiveMode != OrbwalkerMode.LastHit && WindowsKeys.GetKey(args) == Keys.X)
                {
                    ActiveMode = OrbwalkerMode.LastHit;
                }
                else if (ActiveMode != OrbwalkerMode.LaneFreeze && WindowsKeys.GetKey(args) == Keys.A)
                {
                    ActiveMode = OrbwalkerMode.LaneFreeze;
                }
            }
            else if (WindowsKeys.GetWindowsMessage(args) == WindowsMessages.KEYUP)
            {
                var toggle = false;

                if (ActiveMode == OrbwalkerMode.Combo && WindowsKeys.GetKey(args) == Keys.Space)
                {
                    toggle = true;
                }
                else if (ActiveMode == OrbwalkerMode.Mixed && WindowsKeys.GetKey(args) == Keys.C)
                {
                    toggle = true;
                }
                else if (ActiveMode == OrbwalkerMode.LaneClear && WindowsKeys.GetKey(args) == Keys.V)
                {
                    toggle = true;
                }
                else if (ActiveMode == OrbwalkerMode.LastHit && WindowsKeys.GetKey(args) == Keys.X)
                {
                    toggle = true;
                }
                else if (ActiveMode == OrbwalkerMode.LaneFreeze && WindowsKeys.GetKey(args) == Keys.A)
                {
                    toggle = true;
                }

                if (toggle)
                {
                    ActiveMode = OrbwalkerMode.None;
                }
            }
        }

        /// <summary>
        ///     Orbwalker event, used for subscribing to incoming orbwalker events.
        /// </summary>
        public static event OnOrbwalkDelegate OnOrbwalk;

        /// <summary>
        ///     Orbwalker event of attack, used for notifying on pre, during or after an attack event.
        /// </summary>
        public static event OnAttackEventDelegate OnAttackEvent;

        /// <summary>
        ///     On Game Update subscribed event function.
        /// </summary>
        /// <param name="eventArgs">
        ///     <see cref="System.EventArgs" />
        /// </param>
        private static void Game_OnUpdate(EventArgs eventArgs)
        {
            if (!Active || ActiveMode == OrbwalkerMode.None)
            {
                return;
            }

            if (!_player.IsValid())
            {
                _player = ObjectManager.Player;
                return;
            }

            var args = new OrbwalkerArgs { Mode = ActiveMode, Type = OrbwalkerType.None, Windup = 90 };

            if (CanAttack())
            {
                var target = GetTarget(ActiveMode);
                if (target.IsValidTarget())
                {
                    args.Target = target;
                    args.Position = target.Position;
                    args.Type = OrbwalkerType.AutoAttack;
                }
            }

            if (!args.Target.IsValidTarget() && CanMove(args.Windup) && Variables.TickCount > _nextMovementTick)
            {
                var position = GetPosition(Game.CursorPos);
                if (position.IsValid())
                {
                    args.Position = position;
                    args.Type = OrbwalkerType.Movement;
                }
            }

            if (OnOrbwalk != null)
            {
                OnOrbwalk(MethodBase.GetCurrentMethod().DeclaringType, args);
            }

            Orbwalk(args);
        }

        /// <summary>
        ///     Orbwalk command execution.
        /// </summary>
        private static void Orbwalk(OrbwalkerArgs args)
        {
            if (BlockOnce)
            {
                BlockOnce = false;
                return;
            }
            bool order;

            switch (args.Type)
            {
                case OrbwalkerType.AutoAttack:
                    if (!args.Target.IsValidTarget(_player.GetRealAutoAttackRange()) || !CanAttack())
                    {
                        break;
                    }
                    order = _player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                    if (order)
                    {
                        _lastAutoAttackTick = Variables.TickCount + Game.Ping / 2;
                    }
                    break;
                case OrbwalkerType.Movement:
                    if (!args.Position.IsValid() && !CanMove(args.Windup))
                    {
                        break;
                    }
                    order = _player.IssueOrder(GameObjectOrder.MoveTo, args.Position);
                    if (order)
                    {
                        _nextMovementTick = Variables.TickCount + 200;
                    }
                    break;
            }
        }

        #region Functions

        /// <summary>
        ///     Returns the attackable target that came out prioritized.
        /// </summary>
        /// <returns>
        ///     <see cref="AttackableUnit" />
        /// </returns>
        public static AttackableUnit GetTarget(OrbwalkerMode mode, float distance = -1f)
        {
            Obj_AI_Minion[] minions = null;
            Obj_AI_Minion cannon = null;
            distance = (System.Math.Abs(distance - (-1f)) < float.Epsilon) ? _player.GetRealAutoAttackRange() : distance;

            if (mode == OrbwalkerMode.LaneClear || mode == OrbwalkerMode.LaneFreeze || mode == OrbwalkerMode.LastHit ||
                mode == OrbwalkerMode.Mixed)
            {
                minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(distance)).ToArray();
                cannon = minions.FirstOrDefault(m => m.SkinName.Contains("Siege") || m.SkinName.Contains("Super"));
            }

            if (mode == OrbwalkerMode.Combo)
            {
                var target =
                    ObjectHandler.GetFast<Obj_AI_Hero>().OrderBy(t => t.Distance(_player.Position)).FirstOrDefault();
                if (target.IsValidTarget(_player.GetRealAutoAttackRange()))
                {
                    return target; // TODO: Replace with target selector
                }
            }
            if (mode == OrbwalkerMode.LaneClear || mode == OrbwalkerMode.LastHit || mode == OrbwalkerMode.LaneFreeze ||
                mode == OrbwalkerMode.Mixed)
            {
                // TODO: Add last hit here
            }
            if (mode == OrbwalkerMode.LaneClear)
            {
                if (minions != null)
                {
                    if (!ShouldWait())
                    {
                        if (cannon.IsValidTarget() &&
                            Health.GetHealthPrediction(cannon, (int) (_player.AttackDelay * 1000)) <=
                            _player.TotalAttackDamage)
                        {
                            return cannon;
                        }
                        return minions.OrderBy(m => m.Health).FirstOrDefault();
                    }
                }
            }

            return null;
        }

        private static bool ShouldWait()
        {
            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        m =>
                            m.IsValidTarget() && m.IsEnemy && m.InAutoAttackRange() &&
                            Health.LaneClearHealthPrediction(m, (int) ((_player.AttackDelay * 1000)) * 0) <=
                            _player.TotalAttackDamage);
        }

        /// <summary>
        ///     Returns the position choosen for the cursor position.
        /// </summary>
        /// <param name="cursor">Vector3 of the Cursor</param>
        /// <returns>Vector3 to orbwalk to.</returns>
        public static Vector3 GetPosition(Vector3 cursor)
        {
            return cursor;
        }

        /// <summary>
        ///     Returns if the player's auto-attack is ready.
        /// </summary>
        public static bool CanAttack()
        {
            return (_lastAutoAttackTick <= Variables.TickCount) &&
                   (Variables.TickCount + Game.Ping / 2 + 25 >= _lastAutoAttackTick + _player.AttackDelay * 1000 &&
                    Attack);
        }

        /// <summary>
        ///     Returns true if moving won't cancel the auto-attack and if the champion is able to move.
        /// </summary>
        public static bool CanMove(float extraWindup)
        {
            if (_lastAutoAttackTick <= Variables.TickCount)
            {
                return Move && !_player.CanCancelAutoAttack()
                    ? (Variables.TickCount - _lastAutoAttackTick > 300)
                    : (Variables.TickCount + Game.Ping / 2 >=
                       _lastAutoAttackTick + _player.AttackCastDelay * 1000 + extraWindup);
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    ///     Orbwalker data class for <see cref="Orbwalker.OnOrbwalk" />
    /// </summary>
    public class OrbwalkerArgs : EventArgs
    {
        /// <summary>
        ///     Local value to define if should continue with the orbwalker command.
        /// </summary>
        private bool _process = Orbwalker.BlockOnce;

        /// <summary>
        ///     Orbwalker mode.
        /// </summary>
        public OrbwalkerMode Mode;

        /// <summary>
        ///     Position the Orbwalker is going to target movement.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///     Target of the Orbwalker.
        /// </summary>
        public AttackableUnit Target;

        /// <summary>
        ///     Orbwalker Type
        /// </summary>
        public OrbwalkerType Type;

        /// <summary>
        ///     Orbwalker Windup
        /// </summary>
        public int Windup;

        /// <summary>
        ///     Process value, used for blocking the orbwalker command.
        /// </summary>
        public bool Process
        {
            get { return _process; }
            set
            {
                Orbwalker.BlockOnce = value;
                _process = value;
            }
        }
    }

    /// <summary>
    /// </summary>
    public class AttackArgs : EventArgs {}
}