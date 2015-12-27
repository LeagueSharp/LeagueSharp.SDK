// <copyright file="Priority.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers.TargetSelector.Modes
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    #endregion

    /// <summary>
    ///     The priority Mode.
    /// </summary>
    public class Priority : ITargetSelectorMode
    {
        #region Constants

        /// <summary>
        ///     The maximum priority
        /// </summary>
        private const int MaxPriority = 5;

        /// <summary>
        ///     The minimum priority
        /// </summary>
        private const int MinPriority = 1;

        #endregion

        #region Fields

        /// <summary>
        ///     The priority categories
        /// </summary>
        private readonly List<PriorityCategory> priorityCategories = new List<PriorityCategory>();

        /// <summary>
        ///     The menu
        /// </summary>
        private Menu menu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Priority" /> class.
        /// </summary>
        public Priority()
        {
            this.priorityCategories.AddRange(
                new List<PriorityCategory>
                    {
                        new PriorityCategory
                            {
                                Champions =
                                    new HashSet<string>
                                        {
                                            "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia",
                                            "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus",
                                            "Katarina", "Kennen", "KogMaw", "Leblanc", "Lucian", "Lux", "Malzahar",
                                            "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                                            "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar",
                                            "Velkoz", "Viktor", "Xerath", "Zed", "Ziggs", "Soraka"
                                        },
                                Value = 4
                            },
                        new PriorityCategory
                            {
                                Champions =
                                    new HashSet<string>
                                        {
                                            "Akali", "Diana", "Ekko", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger",
                                            "Illaoi", "Jayce", "Kassadin", "Kayle", "KhaZix", "Kindred", "Lissandra",
                                            "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo", "Zilean"
                                        },
                                Value = 3
                            },
                        new PriorityCategory
                            {
                                Champions =
                                    new HashSet<string>
                                        {
                                            "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas",
                                            "Irelia", "Jax", "LeeSin", "Maokai", "Morgana", "Nocturne", "Pantheon",
                                            "Poppy", "Rengar", "Rumble", "Ryze", "Swain", "Trundle", "Tryndamere", "Udyr",
                                            "Urgot", "Vi", "XinZhao", "RekSai"
                                        },
                                Value = 2
                            },
                        new PriorityCategory
                            {
                                Champions =
                                    new HashSet<string>
                                        {
                                            "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "ChoGath", "DrMundo",
                                            "Garen", "Gnar", "Hecarim", "Janna", "JarvanIV", "Leona", "Lulu", "Malphite",
                                            "Nami", "Nasus", "Nautilus", "Nunu", "Olaf", "Rammus", "Renekton", "Sejuani",
                                            "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona", "TahmKench",
                                            "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac",
                                            "Zyra"
                                        },
                                Value = 1
                            }
                    });
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName => "Priorities";

        /// <summary>
        ///     The name
        /// </summary>
        public string Name => "priorities";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds to menu.
        /// </summary>
        /// <param name="tsMenu">The ts menu.</param>
        public void AddToMenu(Menu tsMenu)
        {
            this.menu = tsMenu;

            var priorityMenu = new Menu("priority", "Priority");

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                priorityMenu.Add(
                    new MenuSlider(enemy.ChampionName, enemy.ChampionName, MinPriority, MinPriority, MaxPriority));
            }

            priorityMenu.Add(new MenuBool("autoPriority", "Auto Priority"));

            priorityMenu.MenuValueChanged += (sender, args) =>
                {
                    var boolean = sender as MenuBool;
                    if (boolean != null)
                    {
                        if (boolean.Name.Equals("autoPriority") && boolean.Value)
                        {
                            foreach (var enemy in GameObjects.EnemyHeroes)
                            {
                                this.SetPriority(enemy, this.GetDefaultPriority(enemy));
                            }
                        }
                    }
                };

            this.menu.Add(priorityMenu);

            if (priorityMenu["autoPriority"].GetValue<MenuBool>().Value)
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    this.SetPriority(enemy, this.GetDefaultPriority(enemy));
                }
            }
        }

        /// <summary>
        ///     Gets the default priority.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public int GetDefaultPriority(Obj_AI_Hero hero)
        {
            return this.priorityCategories.FirstOrDefault(i => i.Champions.Contains(hero.ChampionName))?.Value
                   ?? MinPriority;
        }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public int GetPriority(Obj_AI_Hero hero)
        {
            return this.menu?["priority"][hero.ChampionName]?.GetValue<MenuSlider>().Value ?? MinPriority;
        }

        /// <summary>
        ///     Orders the champions.
        /// </summary>
        /// <param name="heroes">The heroes.</param>
        /// <returns></returns>
        public List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes)
        {
            return heroes.OrderByDescending(this.GetPriority).ToList();
        }

        /// <summary>
        ///     Sets the priority.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="value">The value.</param>
        public void SetPriority(Obj_AI_Hero hero, int value)
        {
            var item = this.menu?["priority"][hero.ChampionName];
            if (item != null)
            {
                item.GetValue<MenuSlider>().Value = Math.Max(MinPriority, Math.Min(MaxPriority, value));
            }
        }

        #endregion
    }

    /// <summary>
    ///     Category class for Priorities
    /// </summary>
    internal class PriorityCategory
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the champions.
        /// </summary>
        /// <value>
        ///     The champions.
        /// </value>
        public HashSet<string> Champions { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public int Value { get; set; }

        #endregion
    }
}
