using System;
using System.Windows.Forms;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Events;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
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
        ///     Next Tick on which the Orbwalker should begin to to check once again.
        /// </summary>
        private static int _nextTick;

        /// <summary>
        ///     Returns the last auto attack tick, which is updated when a new auto attack is executed by the player.
        /// </summary>
        private static int _lastAutoAttackTick;

        /// <summary>
        ///     Returns the current Local Player instance.
        /// </summary>
        private static Obj_AI_Hero _player;

        /// <summary>
        ///     Static Constructor.
        /// </summary>
        static Orbwalker()
        {
            ActiveMode = OrbwalkerMode.None;

            Game.OnUpdate += Game_OnUpdate;
            Load.OnLoad += args => { _player = ObjectManager.Player; };

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
                if (WindowsKeys.GetKey(args) == Keys.Space)
                {
                    ActiveMode = OrbwalkerMode.Combo;
                }
            }
            else if (WindowsKeys.GetWindowsMessage(args) == WindowsMessages.KEYUP)
            {
                ActiveMode = OrbwalkerMode.None;
            }
        }

        /// <summary>
        ///     Orbwalker event, used for subscribing to incoming orbwalker events.
        /// </summary>
        public static event Action<OrbwalkerArgs> OnOrbwalk;

        /// <summary>
        ///     On Game Update subscribed event function.
        /// </summary>
        /// <param name="eventArgs">
        ///     <see cref="System.EventArgs" />
        /// </param>
        private static void Game_OnUpdate(EventArgs eventArgs)
        {
            if (ActiveMode == OrbwalkerMode.None || Variables.TickCount < _nextTick)
            {
                return;
            }

            var args = new OrbwalkerArgs { Mode = ActiveMode };
            if (CanAttack())
            {
                var target = GetTarget();
                if (target.IsValidTarget())
                {
                    args.Target = target;
                    args.Position = target.Position;
                }
            }

            if (!args.Target.IsValidTarget())
            {
                var position = GetPosition(Game.CursorPos);
                if (position.IsValid())
                {
                    args.Position = position;
                }
            }

            if (OnOrbwalk != null)
            {
                OnOrbwalk(args);
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

            if (args.Target.IsValidTarget()) {}
            else
            {
                if (args.Position.IsValid())
                {
                    _player.IssueOrder(GameObjectOrder.MoveTo, args.Position);
                }
            }
        }

        #region Functions

        /// <summary>
        ///     Returns the attackable target that came out prioritized.
        /// </summary>
        /// <returns>
        ///     <see cref="AttackableUnit" />
        /// </returns>
        public static AttackableUnit GetTarget()
        {
            return null;
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

        #endregion
    }

    /// <summary>
    ///     Orbwalker data class for <see cref="Orbwalker.OnOrbwalk" />
    /// </summary>
    public sealed class OrbwalkerArgs
    {
        /// <summary>
        ///     Local value to define if should continue with the orbwalker command.
        /// </summary>
        private bool _continue = Orbwalker.BlockOnce;

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
        ///     Continue value, used for blocking the orbwalker command.
        /// </summary>
        public bool Continue
        {
            get { return _continue; }
            set
            {
                Orbwalker.BlockOnce = value;
                _continue = value;
            }
        }
    }
}