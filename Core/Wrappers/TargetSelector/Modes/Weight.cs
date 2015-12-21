// <copyright file="Weight.cs" company="LeagueSharp">
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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;

    #endregion

    /// <summary>
    ///     Interface for weight item
    /// </summary>
    public interface IWeightItem
    {
        #region Public Properties

        /// <summary>
        ///     Gets the default weight.
        /// </summary>
        /// <value>
        ///     The default weight.
        /// </value>
        int DefaultWeight { get; }

        /// <summary>
        ///     Gets the display name.
        /// </summary>
        /// <value>
        ///     The display name.
        /// </value>
        string DisplayName { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IWeightItem" /> is inverted.
        /// </summary>
        /// <value>
        ///     <c>true</c> if inverted; otherwise, <c>false</c>.
        /// </value>
        bool Inverted { get; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        float GetValue(Obj_AI_Hero hero);

        #endregion
    }

    /// <summary>
    ///     The weight Mode.
    /// </summary>
    public class Weight : ITargetSelectorMode
    {
        #region Constants

        private const int DefaultPercentage = 100;

        private const int MaxPercentage = 200;

        private const int MaxWeight = 20;

        private const int MinPercentage = 0;

        private const int MinWeight = 0;

        #endregion

        #region Fields

        /// <summary>
        ///     The weight items
        /// </summary>
        private readonly List<WeightItemWrapper> pItems = new List<WeightItemWrapper>();

        private Menu menu;

        private Menu weightsMenu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Weight" /> class.
        /// </summary>
        public Weight()
        {
            var weights =
                Assembly.GetAssembly(typeof(IWeightItem))
                    .GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IWeightItem).IsAssignableFrom(t))
                    .ToList();

            foreach (var weight in weights)
            {
                var instance = DynamicInitializer.NewInstance(weight) as IWeightItem;
                if (instance != null)
                {
                    this.pItems.Add(new WeightItemWrapper(instance));
                }
            }

            this.pItems = this.pItems.OrderBy(p => p.DisplayName).ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName => "Weight";

        /// <summary>
        ///     The items
        /// </summary>
        public ReadOnlyCollection<WeightItemWrapper> Items => this.pItems.AsReadOnly();

        /// <summary>
        ///     The name
        /// </summary>
        public string Name => "weight";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds to menu.
        /// </summary>
        /// <param name="tsMenu">The menu.</param>
        public void AddToMenu(Menu tsMenu)
        {
            this.menu = tsMenu;

            this.weightsMenu = new Menu("weights", "Weights");

            var heroPercentMenu = new Menu("heroPercentage", "Hero Percentage");
            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                heroPercentMenu.Add(
                    new MenuSlider(
                        enemy.ChampionName,
                        enemy.ChampionName,
                        DefaultPercentage,
                        MinPercentage,
                        MaxPercentage));
            }
            this.weightsMenu.Add(heroPercentMenu);

            this.weightsMenu.Add(
                new MenuButton("export", "Export to Clipboard", "Export") { Action = this.ExportSettings });
            this.weightsMenu.Add(
                new MenuButton("import", "Import from Clipboard", "Import") { Action = this.ImportSettings });
            this.weightsMenu.Add(new MenuButton("reset", "Reset to Default", "Reset") { Action = this.ResetSettings });

            foreach (var weight in this.pItems)
            {
                this.weightsMenu.Add(
                    new MenuSlider(
                        weight.Name,
                        weight.DisplayName,
                        Math.Min(MaxWeight, Math.Max(MinWeight, weight.Weight)),
                        MinWeight,
                        MaxWeight));
            }

            this.weightsMenu.MenuValueChanged += (sender, args) =>
                {
                    var slider = sender as MenuSlider;
                    if (slider != null)
                    {
                        foreach (var weight in this.pItems)
                        {
                            if (slider.Name.Equals(weight.Name))
                            {
                                weight.Weight = slider.Value;
                            }
                        }
                    }
                };

            this.menu.Add(this.weightsMenu);

            foreach (var weight in this.pItems)
            {
                weight.Weight = this.weightsMenu[weight.Name].GetValue<MenuSlider>().Value;
            }

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                this.weightsMenu["heroPercentage"][enemy.ChampionName].GetValue<MenuSlider>().Value = DefaultPercentage;
            }
        }

        /// <summary>
        ///     Calculates the weight.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="hero">The hero.</param>
        /// <param name="simulation">if set to <c>true</c> [simulation].</param>
        /// <returns></returns>
        public float Calculate(WeightItemWrapper item, Obj_AI_Hero hero, bool simulation = false)
        {
            var minValue = simulation ? item.SimulationMinValue : item.MinValue;
            var maxValue = simulation ? item.SimulationMaxValue : item.MaxValue;
            if (item.Weight <= MinWeight || maxValue <= 0)
            {
                return MinWeight;
            }
            var minWeight = minValue > 0 ? item.Weight / (maxValue / minValue) : MinWeight;
            var weight = item.Inverted
                             ? item.Weight - item.Weight * item.GetValue(hero) / maxValue + minWeight
                             : item.Weight * item.GetValue(hero) / maxValue;
            return float.IsNaN(weight) || float.IsInfinity(weight)
                       ? MinWeight
                       : Math.Min(MaxWeight, Math.Min(item.Weight, Math.Max(MinWeight, Math.Max(weight, minWeight))));
        }

        /// <summary>
        ///     Deregisters the specified weight.
        /// </summary>
        /// <param name="weight">The weight.</param>
        public void Deregister(IWeightItem weight)
        {
            if (this.pItems.Select(p => p.Item).Contains(weight))
            {
                this.pItems.Remove(this.pItems.FirstOrDefault(w => w.Item.Equals(weight)));
                this.weightsMenu.Remove(this.weightsMenu[weight.Name].GetValue<MenuBool>());
            }
        }

        /// <summary>
        ///     Gets the hero percent.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public int GetHeroPercent(Obj_AI_Hero hero)
        {
            var item = this.weightsMenu["heroPercentage"][hero.ChampionName];
            if (item != null)
            {
                return item.GetValue<MenuSlider>().Value;
            }
            return DefaultPercentage;
        }

        /// <summary>
        ///     Orders the champions.
        /// </summary>
        /// <param name="heroes">The heroes.</param>
        /// <returns></returns>
        public List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes)
        {
            foreach (var item in this.pItems.Where(w => w.Weight > 0))
            {
                this.UpdateMaxMinValue(item, heroes);
            }

            return
                heroes.OrderByDescending(
                    h =>
                    this.pItems.Where(w => w.Weight > 0)
                        .Sum(
                            w =>
                            this.Calculate(w, h) / 100
                            * this.weightsMenu["heroPercentage"][h.ChampionName].GetValue<MenuSlider>().Value)).ToList();
        }

        /// <summary>
        ///     Overwrites the specified old weight.
        /// </summary>
        /// <param name="oldWeight">The old weight.</param>
        /// <param name="newWeight">The new weight.</param>
        public void Overwrite(IWeightItem oldWeight, IWeightItem newWeight)
        {
            var index = this.Items.Select(p => p.Item).ToList().IndexOf(oldWeight);
            if (index >= 0)
            {
                this.pItems[index] = new WeightItemWrapper(newWeight);
                var slider = this.weightsMenu[oldWeight.Name].GetValue<MenuSlider>();
                slider.Name = newWeight.Name;
                slider.DisplayName = newWeight.DisplayName;
                slider.Value = Math.Min(MaxWeight, Math.Max(MinWeight, newWeight.DefaultWeight));
            }
        }

        /// <summary>
        ///     Registers the specified weight.
        /// </summary>
        /// <param name="weight">The weight.</param>
        public void Register(IWeightItem weight)
        {
            if (!this.Items.Any(m => m.Name.Equals(weight.Name)) && !string.IsNullOrEmpty(weight.DisplayName))
            {
                this.pItems.Add(new WeightItemWrapper(weight));
                this.weightsMenu.Add(
                    new MenuSlider(
                        weight.Name,
                        weight.DisplayName,
                        Math.Min(MaxWeight, Math.Max(MinWeight, weight.DefaultWeight)),
                        MinWeight,
                        MaxWeight));
            }
        }

        /// <summary>
        ///     Sets the hero percent.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="percent">The percent.</param>
        public void SetHeroPercent(Obj_AI_Hero hero, int percent)
        {
            var item = this.weightsMenu["heroPercentage"][hero.ChampionName];
            if (item != null)
            {
                item.GetValue<MenuSlider>().Value = Math.Max(MinPercentage, Math.Min(MaxPercentage, percent));
            }
        }

        /// <summary>
        ///     Sets the weight.
        /// </summary>
        /// <param name="weightItem">The weight item.</param>
        /// <param name="weight">The weight.</param>
        public void SetMenuWeight(WeightItemWrapper weightItem, int weight)
        {
            var item = this.weightsMenu[weightItem.Name];
            if (item != null)
            {
                weightItem.Weight = Math.Max(MinWeight, Math.Min(MaxWeight, weight));
                item.GetValue<MenuSlider>().Value = weightItem.Weight;
            }
        }

        /// <summary>
        ///     Updates the maximum minimum value.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="heroes">The heroes.</param>
        /// <param name="simulation">if set to <c>true</c> [simulation].</param>
        public void UpdateMaxMinValue(WeightItemWrapper item, List<Obj_AI_Hero> heroes, bool simulation = false)
        {
            var min = float.MaxValue;
            var max = float.MinValue;
            foreach (var hero in heroes)
            {
                var value = item.GetValue(hero);
                if (value < min)
                {
                    min = value;
                }
                if (value > max)
                {
                    max = value;
                }
            }
            if (!simulation)
            {
                item.MinValue = Math.Min(max, min);
                item.MaxValue = Math.Max(max, min);
            }
            else
            {
                item.SimulationMinValue = Math.Min(max, min);
                item.SimulationMaxValue = Math.Max(max, min);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Exports the settings.
        /// </summary>
        private void ExportSettings()
        {
            Clipboard.SetText(string.Join("|", this.pItems.Select(i => $"{i.Name}:{i.Weight}").ToArray()));
            Game.PrintChat("Weights: Exported to clipboard.");
        }

        /// <summary>
        ///     Imports the settings.
        /// </summary>
        private void ImportSettings()
        {
            var anyApplied = false;
            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();
                var seperated = text.Split('|');
                foreach (var sep in seperated)
                {
                    var splitted = sep.Split(':');
                    if (splitted.Length == 2)
                    {
                        var name = splitted[0];
                        int weight;
                        if (int.TryParse(splitted[1], out weight))
                        {
                            var item = this.pItems.FirstOrDefault(i => i.Name.Equals(name));
                            if (item != null)
                            {
                                this.SetMenuWeight(item, weight);
                                anyApplied = true;
                            }
                        }
                    }
                }
            }

            Game.PrintChat("Weights: " + (anyApplied ? "Imported from clipboard." : "Nothing found in clipboard."));
        }

        /// <summary>
        ///     Resets the settings.
        /// </summary>
        private void ResetSettings()
        {
            foreach (var item in this.pItems)
            {
                this.SetMenuWeight(item, item.DefaultWeight);
            }
            Game.PrintChat("Weights: Reseted to default.");
        }

        #endregion
    }

    /// <summary>
    ///     Wrapper for IWeightItem
    /// </summary>
    public class WeightItemWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeightItemWrapper" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WeightItemWrapper(IWeightItem item)
        {
            this.Item = item;
            this.Weight = item.DefaultWeight;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The default weight
        /// </summary>
        public int DefaultWeight => this.Item.DefaultWeight;

        /// <summary>
        ///     The display name
        /// </summary>
        public string DisplayName => this.Item.DisplayName;

        /// <summary>
        ///     The inverted
        /// </summary>
        public bool Inverted => this.Item.Inverted;

        /// <summary>
        ///     Gets the item.
        /// </summary>
        /// <value>
        ///     The item.
        /// </value>
        public IWeightItem Item { get; internal set; }

        /// <summary>
        ///     Gets the maximum value.
        /// </summary>
        /// <value>
        ///     The maximum value.
        /// </value>
        public float MaxValue { get; internal set; }

        /// <summary>
        ///     Gets the minimum value.
        /// </summary>
        /// <value>
        ///     The minimum value.
        /// </value>
        public float MinValue { get; internal set; }

        /// <summary>
        ///     The name
        /// </summary>
        public string Name => this.Item.Name;

        /// <summary>
        ///     Gets the simulation maximum value.
        /// </summary>
        /// <value>
        ///     The simulation maximum value.
        /// </value>
        public float SimulationMaxValue { get; internal set; }

        /// <summary>
        ///     Gets the simulation minimum value.
        /// </summary>
        /// <value>
        ///     The simulation minimum value.
        /// </value>
        public float SimulationMinValue { get; internal set; }

        /// <summary>
        ///     Gets the weight.
        /// </summary>
        /// <value>
        ///     The weight.
        /// </value>
        public int Weight { get; internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns></returns>
        public float GetValue(Obj_AI_Hero hero) => Math.Max(0, this.Item.GetValue(hero));

        #endregion
    }
}