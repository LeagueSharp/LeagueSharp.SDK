// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameObjects.cs" company="LeagueSharp">
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
//   A static (stack) class which contains a sort-of cached versions of the important game objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Events;

    /// <summary>
    ///     A static (stack) class which contains a sort-of cached versions of the important game objects.
    /// </summary>
    public static class GameObjects
    {
        #region Static Fields

        /// <summary>
        ///     The ally heroes list.
        /// </summary>
        private static readonly List<Obj_AI_Hero> AllyHeroesList = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The ally list.
        /// </summary>
        private static readonly List<Obj_AI_Base> AllyList = new List<Obj_AI_Base>();

        /// <summary>
        ///     The ally minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> AllyMinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The ally turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> AllyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The enemy heroes list.
        /// </summary>
        private static readonly List<Obj_AI_Hero> EnemyHeroesList = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The enemy list.
        /// </summary>
        private static readonly List<Obj_AI_Base> EnemyList = new List<Obj_AI_Base>();

        /// <summary>
        ///     The enemy minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> EnemyMinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The enemy turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> EnemyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The heroes list.
        /// </summary>
        private static readonly List<Obj_AI_Hero> HeroesList = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> MinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> TurretsList = new List<Obj_AI_Turret>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the ally.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Ally
        {
            get
            {
                return AllyList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the ally heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllyHeroes
        {
            get
            {
                return AllyHeroesList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the ally minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyMinions
        {
            get
            {
                return AllyMinionsList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the ally turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> AllyTurrets
        {
            get
            {
                return AllyTurretsList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the enemy.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Enemy
        {
            get
            {
                return EnemyList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the enemy heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> EnemyHeroes
        {
            get
            {
                return EnemyHeroesList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyMinions
        {
            get
            {
                return EnemyMinionsList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the enemy turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> EnemyTurrets
        {
            get
            {
                return EnemyTurretsList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> Heroes
        {
            get
            {
                return HeroesList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Minions
        {
            get
            {
                return MinionsList.Where(o => o.IsValid).ToArray();
            }
        }

        /// <summary>
        ///     Gets the turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> Turrets
        {
            get
            {
                return TurretsList.Where(o => o.IsValid).ToArray();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize method.
        /// </summary>
        internal static void Initialize()
        {
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;

            Load.OnLoad += (sender, args) =>
                {
                    HeroesList.AddRange(ObjectManager.Get<Obj_AI_Hero>());
                    MinionsList.AddRange(ObjectManager.Get<Obj_AI_Minion>());
                    TurretsList.AddRange(ObjectManager.Get<Obj_AI_Turret>());

                    EnemyHeroesList.AddRange(HeroesList.Where(o => o.IsEnemy));
                    EnemyMinionsList.AddRange(MinionsList.Where(o => o.IsEnemy));
                    EnemyTurretsList.AddRange(TurretsList.Where(o => o.IsEnemy));
                    EnemyList.AddRange(
                        EnemyHeroesList.Cast<Obj_AI_Base>().Concat(EnemyMinionsList).Concat(EnemyTurretsList));

                    AllyHeroesList.AddRange(HeroesList.Where(o => o.IsAlly));
                    AllyMinionsList.AddRange(MinionsList.Where(o => o.IsAlly));
                    AllyTurretsList.AddRange(TurretsList.Where(o => o.IsAlly));
                    AllyList.AddRange(
                        AllyHeroesList.Cast<Obj_AI_Base>().Concat(AllyMinionsList).Concat(AllyTurretsList));
                };
        }

        /// <summary>
        ///     OnCreate event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero != null)
            {
                HeroesList.Add(hero);
                if (hero.IsEnemy)
                {
                    EnemyHeroesList.Add(hero);
                    EnemyList.Add(hero);
                }
                else
                {
                    AllyHeroesList.Add(hero);
                    AllyList.Add(hero);
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                MinionsList.Add(minion);
                if (minion.IsEnemy)
                {
                    EnemyMinionsList.Add(minion);
                    EnemyList.Add(minion);
                }
                else
                {
                    AllyMinionsList.Add(minion);
                    AllyList.Add(minion);
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                TurretsList.Add(turret);
                if (turret.IsEnemy)
                {
                    EnemyTurretsList.Add(turret);
                    EnemyList.Add(turret);
                }
                else
                {
                    AllyTurretsList.Add(turret);
                    AllyList.Add(turret);
                }
            }
        }

        /// <summary>
        ///     OnDelete event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            var hero = sender as Obj_AI_Hero;
            if (hero != null)
            {
                HeroesList.Remove(hero);
                if (hero.IsEnemy)
                {
                    EnemyHeroesList.Remove(hero);
                }
                else
                {
                    AllyHeroesList.Remove(hero);
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                MinionsList.Remove(minion);
                if (minion.IsEnemy)
                {
                    EnemyMinionsList.Remove(minion);
                }
                else
                {
                    AllyMinionsList.Remove(minion);
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                TurretsList.Remove(turret);
                if (turret.IsEnemy)
                {
                    EnemyTurretsList.Remove(turret);
                }
                else
                {
                    AllyTurretsList.Remove(turret);
                }
            }
        }

        #endregion
    }
}