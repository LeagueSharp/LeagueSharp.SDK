// <copyright file="Aggro.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.TSModes.Weights
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    ///     Aggro tracking
    /// </summary>
    public static class Aggro
    {
        #region Static Fields

        private static readonly Dictionary<int, AggroEntry> PEntries = new Dictionary<int, AggroEntry>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Aggro" /> class.
        /// </summary>
        static Aggro()
        {
            Obj_AI_Base.OnAggro += OnObjAiBaseAggro;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the entries
        /// </summary>
        public static ReadOnlyDictionary<int, AggroEntry> Entries => new ReadOnlyDictionary<int, AggroEntry>(PEntries);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the sender items.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> of <see cref="AggroEntry" />.
        /// </returns>
        public static IEnumerable<AggroEntry> GetSenderItems(Obj_AI_Base sender)
        {
            return PEntries.Where(i => i.Key.Equals(sender.NetworkId)).Select(i => i.Value);
        }

        /// <summary>
        ///     Gets the sender target item.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="AggroEntry" />.
        /// </returns>
        public static AggroEntry GetSenderTargetItem(Obj_AI_Base sender, Obj_AI_Base target)
        {
            return GetSenderItems(sender).FirstOrDefault(entry => entry.Target.Compare(target));
        }

        /// <summary>
        ///     Gets the target items.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> of <see cref="AggroEntry" />.
        /// </returns>
        public static IEnumerable<AggroEntry> GetTargetItems(Obj_AI_Base target)
        {
            return PEntries.Where(i => i.Value.Target.Compare(target)).Select(i => i.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when aggro is changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="LeagueSharp.GameObjectAggroEventArgs" /> instance containing the event data.
        /// </param>
        private static void OnObjAiBaseAggro(Obj_AI_Base sender, GameObjectAggroEventArgs args)
        {
            if (!sender.IsEnemy)
            {
                return;
            }

            var hero = sender as Obj_AI_Hero;
            var target = GameObjects.EnemyHeroes.FirstOrDefault(h => h.NetworkId == args.NetworkId);

            if (hero != null && target != null)
            {
                AggroEntry aggro;
                if (PEntries.TryGetValue(hero.NetworkId, out aggro))
                {
                    aggro.Target = target;
                }
                else
                {
                    PEntries[target.NetworkId] = new AggroEntry(hero, target);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Entry for the class Aggro
    /// </summary>
    public class AggroEntry
    {
        #region Fields

        /// <summary>
        ///     The sender.
        /// </summary>
        private Obj_AI_Hero sender;

        /// <summary>
        ///     The target.
        /// </summary>
        private Obj_AI_Hero target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggroEntry" /> class.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        public AggroEntry(Obj_AI_Hero sender, Obj_AI_Hero hero)
        {
            this.Sender = sender;
            this.Target = hero;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the sender.
        /// </summary>
        public Obj_AI_Hero Sender
        {
            get
            {
                return this.sender;
            }

            set
            {
                this.sender = value;
                this.TickCount = Variables.TickCount;
            }
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public Obj_AI_Hero Target
        {
            get
            {
                return this.target;
            }

            set
            {
                this.target = value;
                this.TickCount = Variables.TickCount;
            }
        }

        /// <summary>
        ///     Gets the tick count.
        /// </summary>
        public int TickCount { get; private set; }

        #endregion
    }
}