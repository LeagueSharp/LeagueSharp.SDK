using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Dash class, contains the OnDash event for tracking for Dash events of a champion.
    /// </summary>
    public static class Dash
    {
        /// <summary>
        ///     DetectedDashes list.
        /// </summary>
        private static readonly Dictionary<int, DashArgs> DetectedDashes = new Dictionary<int, DashArgs>();

        /// <summary>
        ///     Static Constructor.
        /// </summary>
        static Dash()
        {
            Obj_AI_Base.OnNewPath += ObjAiHeroOnOnNewPath;
        }

        /// <summary>
        ///     OnDashDelegate
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Dash Arguments Container</param>
        public delegate void OnDashDelegate(object sender, DashArgs e);

        /// <summary>
        ///     OnDash Event
        /// </summary>
        public static event OnDashDelegate OnDash;

        /// <summary>
        ///     New Path subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args"></param>
        private static void ObjAiHeroOnOnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender is Obj_AI_Hero && ((Obj_AI_Hero) (sender)).IsValid && args.IsDash)
            {
                if (!DetectedDashes.ContainsKey(sender.NetworkId))
                {
                    DetectedDashes.Add(sender.NetworkId, new DashArgs());
                }
                var path = new List<Vector2> { sender.ServerPosition.ToVector2() };
                path.AddRange(args.Path.ToList().ToVector2());

                DetectedDashes[sender.NetworkId] = new DashArgs
                {
                    StartTick = Variables.TickCount - Game.Ping / 2,
                    Speed = args.Speed,
                    StartPos = sender.ServerPosition.ToVector2(),
                    Unit = sender,
                    Path = path,
                    EndPos = path.Last(),
                    EndTick =
                        Variables.TickCount - Game.Ping / 2 +
                        ((int)
                            ((1000 *
                              (DetectedDashes[sender.NetworkId].EndPos.Distance(
                                  DetectedDashes[sender.NetworkId].StartPos) / DetectedDashes[sender.NetworkId].Speed)))),
                    Duration = DetectedDashes[sender.NetworkId].EndTick - DetectedDashes[sender.NetworkId].StartTick
                };

                if (OnDash != null)
                {
                    OnDash(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, DetectedDashes[sender.NetworkId]);
                }
            }
        }

        /// <summary>
        ///     Returns true if the unit is dashing.
        /// </summary>
        public static bool IsDashing(this Obj_AI_Base unit)
        {
            if (DetectedDashes.ContainsKey(unit.NetworkId))
            {
                return DetectedDashes[unit.NetworkId].EndTick > Variables.TickCount;
            }
            return false;
        }

        /// <summary>
        ///     Gets the speed of the dashing unit if it is dashing.
        /// </summary>
        public static DashArgs GetDashInfo(this Obj_AI_Base unit)
        {
            return DetectedDashes.ContainsKey(unit.NetworkId) ? DetectedDashes[unit.NetworkId] : new DashArgs();
        }

        /// <summary>
        ///     Dash Container
        /// </summary>
        public class DashArgs : EventArgs
        {
            /// <summary>
            ///     Dash Duration
            /// </summary>
            public int Duration;

            /// <summary>
            ///     Dash Ending Position
            /// </summary>
            public Vector2 EndPos;

            /// <summary>
            ///     Dash End Tick
            /// </summary>
            public int EndTick;

            /// <summary>
            ///     Is a Blinking (Game Mechanic) Dash.
            /// </summary>
            public bool IsBlink;

            /// <summary>
            ///     Dash Path
            /// </summary>
            public List<Vector2> Path;

            /// <summary>
            ///     Dash Speed
            /// </summary>
            public float Speed;

            /// <summary>
            ///     Dash Starting Position
            /// </summary>
            public Vector2 StartPos;

            /// <summary>
            ///     Dash Starting Tick
            /// </summary>
            public int StartTick;

            /// <summary>
            ///     Dash Unit
            /// </summary>
            public Obj_AI_Base Unit;
        }
    }
}