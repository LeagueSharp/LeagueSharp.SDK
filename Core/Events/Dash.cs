// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dash.cs" company="LeagueSharp">
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
//   Dash class, contains the OnDash event for tracking for Dash events of a champion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.SDK.Core.Extensions.SharpDX;

    using SharpDX;

    /// <summary>
    ///     Dash class, contains the OnDash event for tracking for Dash events of a champion.
    /// </summary>
    public static class Dash
    {
        #region Static Fields

        /// <summary>
        ///     DetectedDashes list.
        /// </summary>
        private static readonly Dictionary<int, DashArgs> DetectedDashes = new Dictionary<int, DashArgs>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Dash" /> class.
        /// </summary>
        static Dash()
        {
            Obj_AI_Base.OnNewPath += ObjAiHeroOnOnNewPath;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     OnDash Delegate.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">Dash Arguments Container</param>
        public delegate void OnDashDelegate(object sender, DashArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     OnDash Event.
        /// </summary>
        public static event OnDashDelegate OnDash;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the speed of the dashing unit if it is dashing.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        ///     The <see cref="DashArgs" />.
        /// </returns>
        public static DashArgs GetDashInfo(this Obj_AI_Base unit)
        {
            DashArgs value;
            return DetectedDashes.TryGetValue(unit.NetworkId, out value) ? value : new DashArgs();
        }

        /// <summary>
        ///     Returns true if the unit is dashing.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsDashing(this Obj_AI_Base unit)
        {
            DashArgs value;
            if (DetectedDashes.TryGetValue(unit.NetworkId, out value))
            {
                return value.EndTick > Variables.TickCount;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     New Path subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">New Path event data</param>
        private static void ObjAiHeroOnOnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero != null && hero.IsValid && args.IsDash)
            {
                if (!DetectedDashes.ContainsKey(hero.NetworkId))
                {
                    DetectedDashes.Add(hero.NetworkId, new DashArgs());
                }

                var path = new List<Vector2> { hero.ServerPosition.ToVector2() };
                path.AddRange(args.Path.ToList().ToVector2());

                DetectedDashes[hero.NetworkId] = new DashArgs
                                                     {
                                                         StartTick = Variables.TickCount - Game.Ping / 2, 
                                                         Speed = args.Speed, StartPos = hero.ServerPosition.ToVector2(), 
                                                         Unit = sender, Path = path, EndPos = path.Last(), 
                                                         EndTick =
                                                             Variables.TickCount - Game.Ping / 2
                                                             + ((int)
                                                                (1000
                                                                 * (DetectedDashes[hero.NetworkId].EndPos.Distance(
                                                                     DetectedDashes[hero.NetworkId].StartPos)
                                                                    / DetectedDashes[hero.NetworkId].Speed))), 
                                                         Duration =
                                                             DetectedDashes[hero.NetworkId].EndTick
                                                             - DetectedDashes[hero.NetworkId].StartTick
                                                     };

                if (OnDash != null)
                {
                    OnDash(MethodBase.GetCurrentMethod().DeclaringType, DetectedDashes[hero.NetworkId]);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Dash event data.
        /// </summary>
        public class DashArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the dash duration.
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            ///     Gets or sets the end position.
            /// </summary>
            public Vector2 EndPos { get; set; }

            /// <summary>
            ///     Gets or sets the end tick.
            /// </summary>
            public int EndTick { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether is blink.
            /// </summary>
            public bool IsBlink { get; set; }

            /// <summary>
            ///     Gets or sets the path.
            /// </summary>
            public List<Vector2> Path { get; set; }

            /// <summary>
            ///     Gets or sets the speed.
            /// </summary>
            public float Speed { get; set; }

            /// <summary>
            ///     Gets or sets the start position.
            /// </summary>
            public Vector2 StartPos { get; set; }

            /// <summary>
            ///     Gets or sets the start tick.
            /// </summary>
            public int StartTick { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            public Obj_AI_Base Unit { get; set; }

            #endregion
        }
    }
}