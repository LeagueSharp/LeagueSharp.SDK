// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TargetSelector.cs" company="LeagueSharp">
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
//   Target Selector, manageable utility to return the best candidate target based on chosen settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     Target Selector, manageable utility to return the best candidate target based on chosen settings.
    /// </summary>
    public static class TargetSelector
    {
        #region Static Fields

        /// <summary>
        ///     Champions that should be prioritized first. (1)
        /// </summary>
        public static readonly string[] HighestPriority =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Brand", "Caitlyn", 
                "Cassiopeia", "Corki", "Draven", "Ezreal", "Graves", "Jinx", 
                "Kalista", "Karma", "Karthus", "Katarina", "Kennen", 
                "KogMaw", "Leblanc", "Lucian", "Lux", "Malzahar", "MasterYi", 
                "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", 
                "Talon", "Teemo", "Tristana", "TwistedFate", "Twitch", 
                "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath", 
                "Zed", "Ziggs"
            };

        /// <summary>
        ///     Champions that should be prioritized fourth(last). (4)
        /// </summary>
        public static readonly string[] LowestPriority =
            {
                "Alistar", "Amumu", "Blitzcrank", "Braum", "Cho'Gath", 
                "Dr. Mundo", "Garen", "Gnar", "Hecarim", "Janna", "Jarvan IV", 
                "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", 
                "Nunu", "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", 
                "Shyvana", "Singed", "Sion", "Skarner", "Sona", "Soraka", 
                "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", 
                "Yorick", "Zac", "Zyra"
            };

        /// <summary>
        ///     Champions that should be prioritized second. (2)
        /// </summary>
        public static readonly string[] MedHighPriority =
            {
                "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz", 
                "Heimerdinger", "Jayce", "Kassadin", "Kayle", "Kha'Zix", 
                "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", 
                "Vladimir", "Yasuo", "Zilean"
            };

        /// <summary>
        ///     Champions that should be prioritized third (3)
        /// </summary>
        public static readonly string[] MedLowPriority =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", 
                "Gragas", "Irelia", "Jax", "Lee Sin", "Maokai", "Morgana", 
                "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", 
                "Swain", "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", 
                "XinZhao", "RekSai"
            };

        /// <summary>
        ///     The menu handle.
        /// </summary>
        private static Menu menu;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the mode.
        /// </summary>
        public static TargetSelectorMode Mode { get; private set; }

        /// <summary>
        ///     Gets the selected target.
        /// </summary>
        public static Obj_AI_Hero SelectedTarget
        {
            get
            {
                return Hud.SelectedUnit as Obj_AI_Hero;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the best candidate target.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damage">
        ///     The damage type.
        /// </param>
        /// <param name="ignoredChamps">
        ///     The ignored champions list.
        /// </param>
        /// <param name="rangeCheckFrom">
        ///     The range check from.
        /// </param>
        /// <returns>
        ///     The <see cref="Obj_AI_Hero" /> target.
        /// </returns>
        public static Obj_AI_Hero GetTarget(
            float range, 
            DamageType damage = DamageType.Physical, 
            IEnumerable<Obj_AI_Hero> ignoredChamps = null, 
            Vector3? rangeCheckFrom = null)
        {
            if (menu["focusTarget"].GetValue<MenuBool>().Value
                && SelectedTarget.IsValidTarget(range < 0 ? SelectedTarget.GetRealAutoAttackRange() : range))
            {
                return SelectedTarget;
            }

            var targets =
                GameObjects.EnemyHeroes.Where(t => t.IsValidTarget(range < 0 ? t.GetRealAutoAttackRange() : range))
                    .ToArray();

            if (!targets.Any())
            {
                return null;
            }

            if (ignoredChamps != null)
            {
                targets = targets.Where(t => ignoredChamps.All(i => t.NetworkId == i.NetworkId)).ToArray();
            }

            var excludedTargets = targets.Where(t => t.IsInvulnerable(damage));

            return targets.Any() ? GetTarget(targets, rangeCheckFrom) : GetTarget(excludedTargets, rangeCheckFrom);
        }

        /// <summary>
        ///     Gets the best candidate target.
        /// </summary>
        /// <param name="targets">
        ///     The targets.
        /// </param>
        /// <param name="rangeCheckFrom">
        ///     The range check from.
        /// </param>
        /// <returns>
        ///     The <see cref="Obj_AI_Hero" /> target.
        /// </returns>
        public static Obj_AI_Hero GetTarget(IEnumerable<Obj_AI_Hero> targets, Vector3? rangeCheckFrom = null)
        {
            switch (Mode)
            {
                case TargetSelectorMode.LessAttacksToKill:
                    return targets.MinOrDefault(
                        t =>
                            {
                                var attackDamage = GameObjects.Player.GetAutoAttackDamage(t, true);
                                var damage = t.Health / attackDamage > 0 ? attackDamage : 1;
                                try
                                {
                                    return damage * menu["ts" + t.ChampionName].GetValue<MenuSlider>().Value;
                                }
                                catch (Exception)
                                {
                                    return damage;
                                }
                            });
                case TargetSelectorMode.MostAbilityPower:
                    return targets.MaxOrDefault(t => t.TotalMagicalDamage);
                case TargetSelectorMode.MostAttackDamage:
                    return targets.MaxOrDefault(t => t.TotalAttackDamage);
                case TargetSelectorMode.Closest:
                    return
                        targets.MinOrDefault(
                            hero => (rangeCheckFrom ?? GameObjects.Player.ServerPosition).DistanceSquared(hero.Position));
                case TargetSelectorMode.NearMouse:
                    return targets.Find(t => t.DistanceSquared(Game.CursorPos) < 22500);
                case TargetSelectorMode.LessCastPriority:
                    break;
                case TargetSelectorMode.AutoPriority:
                    break;
                case TargetSelectorMode.LeastHealth:
                    return targets.MinOrDefault(t => t.Health);
            }

            return null;
        }

        /// <summary>
        ///     Gets the best candidate target.
        /// </summary>
        /// <param name="spell">The Spell instance</param>
        /// <param name="champsToIgnore">Champions to ignore.</param>
        /// <returns>
        ///     The <see cref="Obj_AI_Hero" /> target.
        /// </returns>
        public static Obj_AI_Hero GetTargetNoCollision(Spell spell, IEnumerable<Obj_AI_Hero> champsToIgnore)
        {
            return null; // TODO: this.
        }

        /// <summary>
        ///     Checks whether the target is invulnerable.
        /// </summary>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="damageType">
        ///     The damage type
        /// </param>
        /// <param name="ignoreShields">
        ///     The ignore shields
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool IsInvulnerable(this Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
        {
            // Tryndamere's Undying Rage (R)
            if (!damageType.Equals(DamageType.True) && target.HasBuff("Undying Rage") && target.Health <= 2f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes the <c>TargetSelector</c>, starting from the menu.
        /// </summary>
        /// <param name="rootMenu">
        ///     The parent menu
        /// </param>
        internal static void Initialize(Menu rootMenu)
        {
            Load.OnLoad += (sender, args) =>
                {
                    menu = new Menu("targetselector", "Target Selector");

                    if (GameObjects.EnemyHeroes.Any())
                    {
                        foreach (var enemy in GameObjects.EnemyHeroes)
                        {
                            var priority = HighestPriority.Any(t => t == enemy.ChampionName)
                                               ? 1
                                               : MedHighPriority.Any(t => t == enemy.ChampionName)
                                                     ? 2
                                                     : MedLowPriority.Any(t => t == enemy.ChampionName) ? 3 : 4;
                            menu.Add(new MenuSlider("ts" + enemy.ChampionName, enemy.ChampionName,priority,0,5));
                        }

                        menu.Add(new MenuSeparator("separatorOther","Other Settings"));
                    }

                    menu.Add(new MenuBool("focusTarget", "Focus Selected Target"));
                    menu.Add(new MenuBool("drawTarget", "Draw Target", true));
                    menu.Add(new MenuSeparator("separatorMode", "Mode Selection"));
                    menu.Add(new MenuList<TargetSelectorMode>("mode", "Mode") { Index = 7 });

                    rootMenu.Add(menu);

                    menu.MenuValueChanged += (objSender, objArgs) =>
                        {
                            var list = objSender as MenuList<TargetSelectorMode>;
                            if (list != null)
                            {
                                Mode = list.SelectedValue;
                            }
                        };

                    Mode = menu["mode"].GetValue<MenuList<TargetSelectorMode>>().SelectedValue;

                    Drawing.OnDraw += eventArgs =>
                        {
                            if (menu["drawTarget"].GetValue<MenuBool>().Value)
                            {
                                var target = GetTarget(-1f);
                                if (target.IsValidTarget())
                                {
                                    Drawing.DrawCircle(target.Position, target.BoundingRadius, Color.Red);
                                }
                            }
                        };
                };
        }

        #endregion
    }
}