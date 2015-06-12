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

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Utils;

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
        ///     The ally shops list.
        /// </summary>
        private static readonly List<Obj_Shop> AllyShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The ally spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> AllySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The ally turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> AllyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The ally wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> AllyWardsList = new List<Obj_AI_Minion>();

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
        ///     The enemy shops list.
        /// </summary>
        private static readonly List<Obj_Shop> EnemyShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The enemy spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> EnemySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The enemy turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> EnemyTurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The enemy wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> EnemyWardsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The heroes list.
        /// </summary>
        private static readonly List<Obj_AI_Hero> HeroesList = new List<Obj_AI_Hero>();

        /// <summary>
        ///     The jungle large list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleLargeList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle legendary list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleLegendaryList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The jungle small list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleSmallList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The minions list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> MinionsList = new List<Obj_AI_Minion>();

        /// <summary>
        ///     The shops list.
        /// </summary>
        private static readonly List<Obj_Shop> ShopsList = new List<Obj_Shop>();

        /// <summary>
        ///     The spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> SpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The turrets list.
        /// </summary>
        private static readonly List<Obj_AI_Turret> TurretsList = new List<Obj_AI_Turret>();

        /// <summary>
        ///     The wards list.
        /// </summary>
        private static readonly List<Obj_AI_Minion> WardsList = new List<Obj_AI_Minion>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the ally.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Ally
        {
            get
            {
                return AllyList;
            }
        }

        /// <summary>
        ///     Gets the ally heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> AllyHeroes
        {
            get
            {
                return AllyHeroesList;
            }
        }

        /// <summary>
        ///     Gets the ally minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyMinions
        {
            get
            {
                return AllyMinionsList;
            }
        }

        /// <summary>
        ///     Gets the ally shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> AllyShops
        {
            get
            {
                return AllyShopsList;
            }
        }

        /// <summary>
        ///     Gets the ally spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> AllySpawnPoints
        {
            get
            {
                return AllySpawnPointsList;
            }
        }

        /// <summary>
        ///     Gets the ally turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> AllyTurrets
        {
            get
            {
                return AllyTurretsList;
            }
        }

        /// <summary>
        ///     Gets the ally wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> AllyWards
        {
            get
            {
                return AllyWardsList;
            }
        }

        /// <summary>
        ///     Gets the enemy.
        /// </summary>
        public static IEnumerable<Obj_AI_Base> Enemy
        {
            get
            {
                return EnemyList;
            }
        }

        /// <summary>
        ///     Gets the enemy heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> EnemyHeroes
        {
            get
            {
                return EnemyHeroesList;
            }
        }

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyMinions
        {
            get
            {
                return EnemyMinionsList;
            }
        }

        /// <summary>
        ///     Gets the enemy shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> EnemyShops
        {
            get
            {
                return EnemyShopsList;
            }
        }

        /// <summary>
        ///     Gets the enemy spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> EnemySpawnPoints
        {
            get
            {
                return EnemySpawnPointsList;
            }
        }

        /// <summary>
        ///     Gets the enemy turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> EnemyTurrets
        {
            get
            {
                return EnemyTurretsList;
            }
        }

        /// <summary>
        ///     Gets the enemy wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> EnemyWards
        {
            get
            {
                return EnemyWardsList;
            }
        }

        /// <summary>
        ///     Gets the heroes.
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> Heroes
        {
            get
            {
                return HeroesList;
            }
        }

        /// <summary>
        ///     Gets the jungle.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Jungle
        {
            get
            {
                return JungleList;
            }
        }

        /// <summary>
        ///     Gets the jungle large.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleLarge
        {
            get
            {
                return JungleLargeList;
            }
        }

        /// <summary>
        ///     Gets the jungle legendary.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleLegendary
        {
            get
            {
                return JungleLegendaryList;
            }
        }

        /// <summary>
        ///     Gets the jungle small.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> JungleSmall
        {
            get
            {
                return JungleSmallList;
            }
        }

        /// <summary>
        ///     Gets the minions.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Minions
        {
            get
            {
                return MinionsList;
            }
        }

        /// <summary>
        ///     Gets the shops.
        /// </summary>
        public static IEnumerable<Obj_Shop> Shops
        {
            get
            {
                return ShopsList;
            }
        }

        /// <summary>
        ///     Gets the spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> SpawnPoints
        {
            get
            {
                return SpawnPointsList;
            }
        }

        /// <summary>
        ///     Gets the turrets.
        /// </summary>
        public static IEnumerable<Obj_AI_Turret> Turrets
        {
            get
            {
                return TurretsList;
            }
        }

        /// <summary>
        ///     Gets the wards.
        /// </summary>
        public static IEnumerable<Obj_AI_Minion> Wards
        {
            get
            {
                return WardsList;
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
                    MinionsList.AddRange(
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                o =>
                                o.Team != GameObjectTeam.Neutral && !o.Name.Contains("ward")
                                && !o.Name.Contains("trinket")));
                    TurretsList.AddRange(ObjectManager.Get<Obj_AI_Turret>());
                    JungleList.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(o => o.Team == GameObjectTeam.Neutral));
                    WardsList.AddRange(
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(o => o.Name.Contains("ward") || o.Name.Contains("trinket")));
                    ShopsList.AddRange(ObjectManager.Get<Obj_Shop>());
                    SpawnPointsList.AddRange(ObjectManager.Get<Obj_SpawnPoint>());

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

                    JungleSmallList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Small));
                    JungleLargeList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Large));
                    JungleLegendaryList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Legendary));

                    AllyWardsList.AddRange(WardsList.Where(o => o.IsAlly));
                    EnemyWardsList.AddRange(WardsList.Where(o => o.IsEnemy));

                    AllyShopsList.AddRange(ShopsList.Where(o => o.IsAlly));
                    EnemyShopsList.AddRange(ShopsList.Where(o => o.IsEnemy));

                    AllySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsAlly));
                    EnemySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsEnemy));
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
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (!minion.Name.Contains("ward") || minion.Name.Contains("trinket"))
                    {
                        MinionsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyMinionsList.Add(minion);
                        }
                        else
                        {
                            AllyMinionsList.Add(minion);
                        }
                    }
                    else
                    {
                        WardsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyWardsList.Add(minion);
                        }
                        else
                        {
                            AllyWardsList.Add(minion);
                        }
                    }

                    if (minion.IsEnemy)
                    {
                        EnemyList.Add(minion);
                    }
                    else
                    {
                        AllyList.Add(minion);
                    }
                }
                else
                {
                    switch (minion.GetJungleType())
                    {
                        case JungleType.Small:
                            JungleSmallList.Add(minion);
                            break;
                        case JungleType.Large:
                            JungleLargeList.Add(minion);
                            break;
                        case JungleType.Legendary:
                            JungleLegendaryList.Add(minion);
                            break;
                    }

                    JungleList.Add(minion);
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

            var shop = sender as Obj_Shop;
            if (shop != null)
            {
                ShopsList.Add(shop);
                if (shop.IsAlly)
                {
                    AllyShopsList.Add(shop);
                }
                else
                {
                    EnemyShopsList.Add(shop);
                }
            }

            var spawnPoint = sender as Obj_SpawnPoint;
            if (spawnPoint != null)
            {
                SpawnPointsList.Add(spawnPoint);
                if (spawnPoint.IsAlly)
                {
                    AllySpawnPointsList.Add(spawnPoint);
                }
                else
                {
                    EnemySpawnPointsList.Add(spawnPoint);
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