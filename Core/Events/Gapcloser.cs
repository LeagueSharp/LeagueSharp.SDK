// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Gapcloser.cs" company="LeagueSharp">
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
//   Detection of Gap-closers and fires the OnGapCloser event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using System.Text;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Properties;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using SharpDX;

    /// <summary>
    ///     Detection of Gap-closers and fires the OnGapCloser event.
    /// </summary>
    public class Gapcloser
    {
        #region Static Fields

        /// <summary>
        ///     Gets or sets the active spells.
        /// </summary>
        private static readonly List<GapCloserEventArgs> ActiveSpellsList = new List<GapCloserEventArgs>();

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        private static readonly List<GapCloser> SpellsList = new List<GapCloser>();

        #endregion

        #region Delegates

        /// <summary>
        ///     OnGapCloser Delegate.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">GapCloserEvent Arguments Container</param>
        public delegate void OnGapCloserDelegate(object sender, GapCloserEventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     OnGapCloser Event.
        /// </summary>
        public static event OnGapCloserDelegate OnGapCloser;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the active spells.
        /// </summary>
        public static IEnumerable<GapCloserEventArgs> ActiveSpells
        {
            get
            {
                return ActiveSpellsList;
            }
        }

        /// <summary>
        ///     Gets the spells.
        /// </summary>
        public static IEnumerable<GapCloser> Spells
        {
            get
            {
                return SpellsList;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes static members of the <see cref="Gapcloser" /> class.
        ///     Static Constructor
        /// </summary>
        internal static void Initialize()
        {
            Load.OnLoad += (sender, args) =>
                {
                    var dataFile =
                        (IDictionary<string, JToken>)JObject.Parse(Encoding.Default.GetString(Resources.Gapclosers));
                    var champions = GameObjects.Heroes.Select(champion => champion.ChampionName).ToArray();
                    foreach (var entry in dataFile.Where(entry => champions.Contains(entry.Key)))
                    {
                        LoadGapcloser(entry.Key, entry.Value.ToString());
                    }

                    Game.OnUpdate += OnUpdate;
                    Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                };
        }

        /// <summary>
        ///     Loads the <c>gapcloser</c> data.
        /// </summary>
        /// <param name="championName">
        ///     The champion name
        /// </param>
        /// <param name="value">
        ///     The value
        /// </param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void LoadGapcloser(string championName, string value)
        {
            var gapcloser = JsonConvert.DeserializeObject<GapCloser>(value);
            gapcloser.ChampionName = championName;

            SpellsList.Add(gapcloser);
        }

        /// <summary>
        ///     On Process Spell Cast subscribed event function
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <param name="args">Process Spell Cast Data</param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (SpellsList.All(spell => spell.SpellName != args.SData.Name.ToLower()))
            {
                return;
            }

            var hero = sender as Obj_AI_Hero;
            var player = GameObjects.Player;
            if (hero != null)
            {
                ActiveSpellsList.Add(
                    new GapCloserEventArgs
                        {
                            Start = args.Start, End = args.End, Sender = hero, TickCount = Variables.TickCount, 
                            SkillType =
                                (args.Target != null && args.Target.IsValid)
                                    ? GapcloserType.Targeted
                                    : GapcloserType.Skillshot, 
                            Slot = hero.GetSpellSlot(args.SData.Name), 
                            IsDirectedToPlayer =
                                player.Distance(args.End) < player.Distance(args.Start) || sender.IsFacing(player), 
                            SpellName = args.SData.Name
                        });
            }
        }

        /// <summary>
        ///     On game tick update subscribed event function.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> containing event data
        /// </param>
        private static void OnUpdate(EventArgs args)
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
                OnGapCloser(gapcloser.Sender, gapcloser);
            }
        }

        #endregion

        /// <summary>
        ///     GapCloser Data Container
        /// </summary>
        public struct GapCloser
        {
            #region Fields

            /// <summary>
            ///     Champion Name
            /// </summary>
            public string ChampionName;

            /// <summary>
            ///     Spell Type
            /// </summary>
            public GapcloserType SkillType;

            /// <summary>
            ///     Spell Slot
            /// </summary>
            public SpellSlot Slot;

            /// <summary>
            ///     Spell Name
            /// </summary>
            public string SpellName;

            #endregion
        }

        /// <summary>
        ///     GapCloser Data Container
        /// </summary>
        public class GapCloserEventArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the end.
            /// </summary>
            public Vector3 End { get; set; }

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