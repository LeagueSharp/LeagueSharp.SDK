// <copyright file="Invulnerable.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using LeagueSharp.SDKEx.Enumerations;

    /// <summary>
    ///     Invulnerable utility class
    /// </summary>
    public class Invulnerable
    {
        #region Static Fields

        /// <summary>
        ///     The invulnerable entries
        /// </summary>
        private static readonly List<InvulnerableEntry> PEntries = new List<InvulnerableEntry>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes the <see cref="Invulnerable" /> class.
        /// </summary>
        static Invulnerable()
        {
            PEntries.AddRange(
                new List<InvulnerableEntry>
                    {
                        new InvulnerableEntry("FerociousHowl")
                            {
                                ChampionName = "Alistar",
                                CheckFunction =
                                    (target, type) =>
                                    GameObjects.Player.CountEnemyHeroesInRange(
                                        GameObjects.Player.GetRealAutoAttackRange()) > 1
                            },
                        new InvulnerableEntry("Meditate")
                            {
                                ChampionName = "MasterYi",
                                CheckFunction =
                                    (target, type) =>
                                    GameObjects.Player.CountEnemyHeroesInRange(
                                        GameObjects.Player.GetRealAutoAttackRange()) > 1
                            },
                        new InvulnerableEntry("UndyingRage")
                            {
                                ChampionName = "Tryndamere", MinHealthPercent = 1,
                                CheckFunction = (target, type) => target.HealthPercent <= 5
                            },
                        new InvulnerableEntry("JudicatorIntervention") { IsShield = true },
                        new InvulnerableEntry("fizztrickslamsounddummy") { ChampionName = "Fizz" },
                        new InvulnerableEntry("VladimirSanguinePool") { ChampionName = "Vladimir" },
                        new InvulnerableEntry("FioraW") { ChampionName = "Fiora" },
                        new InvulnerableEntry("JaxCounterStrike")
                            { ChampionName = "Jax", DamageType = DamageType.Physical },
                        new InvulnerableEntry("BlackShield") { IsShield = true, DamageType = DamageType.Magical },
                        new InvulnerableEntry("BansheesVeil") { IsShield = true, DamageType = DamageType.Magical },
                        new InvulnerableEntry("SivirE") { ChampionName = "Sivir", IsShield = true },
                        new InvulnerableEntry("NocturneShroudofDarkness") { ChampionName = "Nocturne", IsShield = true },
                        new InvulnerableEntry("KindredrNoDeathBuff")
                            {
                                ChampionName = "Kindred", MinHealthPercent = 10,
                                CheckFunction = (target, type) => target.HealthPercent <= 10
                            }
                    });
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The invulnerable entries
        /// </summary>
        public static ReadOnlyCollection<InvulnerableEntry> Entries => PEntries.AsReadOnly();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Checks if the specified target is invulnerable.
        /// </summary>
        /// <param name="hero">The target.</param>
        /// <param name="damageType">Type of the damage.</param>
        /// <param name="ignoreShields">if set to <c>true</c> [ignore shields].</param>
        /// <param name="damage">The damage.</param>
        /// <returns></returns>
        public static bool Check(
            Obj_AI_Hero hero,
            DamageType damageType = DamageType.True,
            bool ignoreShields = true,
            float damage = -1f)
        {
            if (hero.HasBuffOfType(BuffType.Invulnerability) || hero.IsInvulnerable)
            {
                return true;
            }
            foreach (var entry in Entries)
            {
                if (entry.ChampionName == null || entry.ChampionName == hero.ChampionName)
                {
                    if (entry.DamageType == null || entry.DamageType == damageType)
                    {
                        if (hero.HasBuff(entry.BuffName))
                        {
                            if (!ignoreShields || !entry.IsShield)
                            {
                                if (entry.CheckFunction == null || ExecuteCheckFunction(entry, hero, damageType))
                                {
                                    if (damage <= 0 || entry.MinHealthPercent <= 0
                                        || (hero.Health - damage) / hero.MaxHealth * 100 < entry.MinHealthPercent)
                                    {
                                        return true;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     Deregisters the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public static void Deregister(InvulnerableEntry entry)
        {
            if (PEntries.Any(i => i.BuffName.Equals(entry.BuffName)))
            {
                PEntries.Remove(entry);
            }
        }

        /// <summary>
        ///     Gets the item.
        /// </summary>
        /// <param name="buffName">Name of the buff.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public static InvulnerableEntry GetItem(
            string buffName,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return PEntries.FirstOrDefault(w => w.BuffName.Equals(buffName, stringComparison));
        }

        /// <summary>
        ///     Registers the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public static void Register(InvulnerableEntry entry)
        {
            if (!string.IsNullOrEmpty(entry.BuffName) && !PEntries.Any(i => i.BuffName.Equals(entry.BuffName)))
            {
                PEntries.Add(entry);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the check function.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="hero">The target.</param>
        /// <param name="damageType">Type of the damage.</param>
        /// <returns></returns>
        private static bool ExecuteCheckFunction(InvulnerableEntry entry, Obj_AI_Hero hero, DamageType damageType)
        {
            try
            {
                return entry != null && entry.CheckFunction(hero, damageType);
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, ex);
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     Entry for <see cref="Invulnerable" /> class.
    /// </summary>
    public class InvulnerableEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvulnerableEntry" /> class.
        /// </summary>
        /// <param name="buffName">Name of the buff.</param>
        public InvulnerableEntry(string buffName)
        {
            this.BuffName = buffName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name of the buff.
        /// </summary>
        /// <value>
        ///     The name of the buff.
        /// </value>
        public string BuffName { get; }

        /// <summary>
        ///     Gets or sets the champion name.
        /// </summary>
        /// <value>
        ///     The champion name.
        /// </value>
        public string ChampionName { get; set; }

        /// <summary>
        ///     Gets or sets the check function.
        /// </summary>
        /// <value>
        ///     The check function.
        /// </value>
        public Func<Obj_AI_Base, DamageType, bool> CheckFunction { get; set; }

        /// <summary>
        ///     Gets or sets the type of the damage.
        /// </summary>
        /// <value>
        ///     The type of the damage.
        /// </value>
        public DamageType? DamageType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this is a shield.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this is a shield; otherwise, <c>false</c>.
        /// </value>
        public bool IsShield { get; set; }

        /// <summary>
        ///     Gets or sets the minimum health percent.
        /// </summary>
        /// <value>
        ///     The minimum health percent.
        /// </value>
        public int MinHealthPercent { get; set; }

        #endregion
    }
}