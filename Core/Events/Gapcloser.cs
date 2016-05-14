// <copyright file="Gapcloser.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using LeagueSharp.Data;
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;

    using SharpDX;

    /// <summary>
    ///     Detection of Gap-closers and fires the OnGapCloser event.
    /// </summary>
    public static partial class Events
    {
        #region Static Fields

        /// <summary>
        ///     Gets or sets the active spells.
        /// </summary>
        private static readonly List<GapCloserEventArgs> ActiveSpellsList = new List<GapCloserEventArgs>();

        /// <summary>
        ///     Gets the spells.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, GapcloserDataEntry> SpellsList =
            Data.Get<GapcloserData>().SpellsList;

        #endregion

        #region Public Events

        /// <summary>
        ///     OnGapCloser Event.
        /// </summary>
        public static event EventHandler<GapCloserEventArgs> OnGapCloser;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the active spells.
        /// </summary>
        public static IEnumerable<GapCloserEventArgs> ActiveSpells => ActiveSpellsList;

        /// <summary>
        ///     Gets the spells.
        /// </summary>
        public static IEnumerable<GapcloserDataEntry> GapCloserSpells => SpellsList.Values;

        #endregion

        #region Methods

        /// <summary>
        ///     On Process Spell Cast subscribed event function
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Process Spell Cast Data</param>
        private static void EventGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (SpellsList.Values.All(spell => spell.SpellName != args.SData.Name.ToLower()))
            {
                return;
            }

            var hero = sender as Obj_AI_Hero;

            if (hero == null || hero.IsAlly)
            {
                return;
            }

            ActiveSpellsList.Add(
                new GapCloserEventArgs
                    {
                        Start = args.Start, End = args.End, Target = args.Target,
                        Sender = hero, TickCount = Variables.TickCount,
                        SkillType =
                            (args.Target != null && args.Target.IsValid)
                                ? GapcloserType.Targeted
                                : GapcloserType.Skillshot,
                        Slot = args.Slot,
                        IsDirectedToPlayer = (hero.Distance(ObjectManager.Player) < 1500 || args.End.Distance(ObjectManager.Player.Position) < 800) &&
                            ((args.Target != null && args.Target.IsValid && args.Target.IsMe)
                            || args.End.DistanceToPlayer() < args.Start.DistanceToPlayer()
                            || hero.IsFacing(GameObjects.Player)),
                        SpellName = args.SData.Name
                    });
        }

        /// <summary>
        ///     On game tick update subscribed event function.
        /// </summary>
        private static void EventGapcloser()
        {
            ActiveSpellsList.RemoveAll(entry => Variables.TickCount > entry.TickCount + 900);

            if (OnGapCloser == null)
            {
                return;
            }

            foreach (var gapcloser in
                ActiveSpellsList.Where(gapcloser => gapcloser.Sender.IsValidTarget())
                    .Where(
                        gapcloser =>
                        gapcloser.SkillType == GapcloserType.Targeted
                        || (gapcloser.SkillType == GapcloserType.Skillshot
                            && GameObjects.Player.DistanceSquared(gapcloser.Sender) < 250000)))
            {
                OnGapCloser(MethodBase.GetCurrentMethod().DeclaringType, gapcloser);
            }
        }

        #endregion

        /// <summary>
        ///     GapCloser Data Container
        /// </summary>
        public class GapCloserEventArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the position at which the enemy will be upon spell completion.
            /// </summary>
            public Vector3 End { get; set; }
            
            /// <summary>
            ///     Gets or sets the target of the gapcloser spell. It can be null!
            public GameObject Target { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether is directed to player.
            /// </summary>
            public bool IsDirectedToPlayer { get; set; }

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            public Obj_AI_Hero Sender { get; set; }

            /// <summary>
            ///     Gets or sets the skill type.
            /// </summary>
            public GapcloserType SkillType { get; set; }

            /// <summary>
            ///     Gets or sets the slot.
            /// </summary>
            public SpellSlot Slot { get; set; }

            /// <summary>
            ///     Gets or sets the spell name.
            /// </summary>
            public string SpellName { get; set; }

            /// <summary>
            ///     Gets or sets the start.
            /// </summary>
            public Vector3 Start { get; set; }

            /// <summary>
            ///     Gets or sets the tick count.
            /// </summary>
            public int TickCount { get; set; }

            #endregion
        }
    }
}
