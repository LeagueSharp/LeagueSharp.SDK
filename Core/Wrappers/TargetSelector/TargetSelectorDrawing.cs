// <copyright file="TargetSelectorDrawing.cs" company="LeagueSharp">
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

    using LeagueSharp.SDKEx.TSModes;
    using LeagueSharp.SDKEx.UI;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     Drawings for TargetSelector
    /// </summary>
    internal class TargetSelectorDrawing
    {
        #region Fields

        /// <summary>
        ///     The menu
        /// </summary>
        private readonly Menu menu = new Menu("drawing", "Drawing");

        /// <summary>
        ///     The mode instance.
        /// </summary>
        private readonly TargetSelectorMode mode;

        /// <summary>
        ///     The selected instance.
        /// </summary>
        private readonly TargetSelectorSelected selected;

        /// <summary>
        ///     The weight
        /// </summary>
        private readonly Weight weight;

        /// <summary>
        ///     The weight best target
        /// </summary>
        private Obj_AI_Hero weightBestTarget;

        /// <summary>
        ///     The weight best target last tick count
        /// </summary>
        private int weightBestTargetLastTickCount;

        /// <summary>
        ///     The weight targets
        /// </summary>
        private List<Tuple<Obj_AI_Hero, float>> weightTargets = new List<Tuple<Obj_AI_Hero, float>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelectorDrawing" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        /// <param name="selected">
        ///     The selected.
        /// </param>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        public TargetSelectorDrawing(Menu menu, TargetSelectorSelected selected, TargetSelectorMode mode)
        {
            this.selected = selected;
            this.mode = mode;
            this.weight = this.mode.Entries.FirstOrDefault(e => e.Name.Equals("weight")) as Weight;

            var selectedMenu = new Menu("selected", "Selected");
            selectedMenu.Add(new MenuColor("color", "Color", new ColorBGRA(255, 0, 0, 255)));
            selectedMenu.Add(new MenuSlider("radius", "Radius", 35));
            selectedMenu.Add(new MenuBool("enabled", "Enabled", true));

            if (this.weight != null)
            {
                var weightMenu = new Menu("weight", "Weight");

                var bestTarget = new Menu("bestTarget", "Best Target");
                bestTarget.Add(new MenuColor("color", "Color", new ColorBGRA(0, 255, 0, 255)));
                bestTarget.Add(new MenuSlider("radius", "Radius", 55));
                bestTarget.Add(new MenuBool("enabled", "Enabled", true));

                weightMenu.Add(bestTarget);

                weightMenu.Add(new MenuSlider("range", "Range", 1500, 500, 3000));
                weightMenu.Add(new MenuBool("simple", "Simple"));

                this.menu.Add(weightMenu);
            }

            this.menu.Add(selectedMenu);

            menu.Add(this.menu);

            Drawing.OnDraw += this.OnDrawingDraw;

            if (this.weight != null)
            {
                Game.OnUpdate += this.OnGameUpdate;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <see cref="E:DrawingDraw" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnDrawingDraw(EventArgs args)
        {
            if (this.menu["selected"]["enabled"].GetValue<MenuBool>().Value)
            {
                if (this.selected.Focus && this.selected.Target.IsValidTarget()
                    && this.selected.Target.Position.IsOnScreen())
                {
                    Render.Circle.DrawCircle(
                        this.selected.Target.Position,
                        this.selected.Target.BoundingRadius
                        + this.menu["selected"]["radius"].GetValue<MenuSlider>().Value,
                        this.menu["selected"]["color"].GetValue<MenuColor>().Color.ToSystemColor());
                }
            }

            if (this.weight != null && this.mode.Current.Equals(this.weight))
            {
                if (this.menu["weight"]["simple"].GetValue<MenuBool>().Value)
                {
                    foreach (var target in
                        this.weightTargets.Where(t => t.Item1.IsValidTarget() && t.Item1.Position.IsOnScreen()))
                    {
                        Drawing.DrawText(
                            target.Item1.HPBarPosition.X + 55f,
                            target.Item1.HPBarPosition.Y - 20f,
                            Color.White,
                            target.Item2.ToString("0.0").Replace(",", "."));
                    }
                }

                if (this.menu["weight"]["bestTarget"]["enabled"].GetValue<MenuBool>().Value
                    && this.weightBestTarget.IsValidTarget() && this.weightBestTarget.Position.IsOnScreen())
                {
                    Render.Circle.DrawCircle(
                        this.weightBestTarget.Position,
                        this.weightBestTarget.BoundingRadius
                        + this.menu["weight"]["bestTarget"]["radius"].GetValue<MenuSlider>().Value,
                        this.menu["weight"]["bestTarget"]["color"].GetValue<MenuColor>().Color.ToSystemColor());
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnGameUpdate(EventArgs args)
        {
            if (this.weight == null || !this.mode.Current.Equals(this.weight))
            {
                return;
            }

            if (!this.menu["weight"]["simple"].GetValue<MenuBool>().Value
                && !this.menu["weight"]["bestTarget"]["enabled"].GetValue<MenuBool>().Value)
            {
                return;
            }

            var weightRange = this.menu["weight"]["range"].GetValue<MenuSlider>().Value;
            var enemies = GameObjects.EnemyHeroes.Where(e => e.IsValidTarget(weightRange)).ToList();
            foreach (var w in this.weight.Items)
            {
                this.weight.UpdateMaxMinValue(w, enemies, true);
            }

            this.weightTargets = (from enemy in enemies
                                  let w =
                                      this.weight.Items.Where(w => w.Weight > 0)
                                      .Sum(w => this.weight.Calculate(w, enemy, true))
                                  let hp = this.weight.GetHeroPercent(enemy)
                                  let sw = hp > 0 ? w / 100 * hp : 0
                                  select new Tuple<Obj_AI_Hero, float>(enemy, sw)).OrderByDescending(w => w.Item2)
                .ToList();

            var bestTarget = this.weightTargets.Any() ? this.weightTargets.Select(e => e.Item1).FirstOrDefault() : null;
            if (!bestTarget.Compare(this.weightBestTarget)
                && Variables.TickCount - this.weightBestTargetLastTickCount >= 1000)
            {
                this.weightBestTarget = bestTarget;
                this.weightBestTargetLastTickCount = Variables.TickCount;
            }
        }

        #endregion
    }
}