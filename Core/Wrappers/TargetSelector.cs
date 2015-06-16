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
//   Gets the best target.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;

    /// <summary>
    ///     Gets the best target.
    /// </summary>
    public class TargetSelector
    {
        #region Static Fields

        /// <summary>
        ///     The current mode the TS is using.
        /// </summary>
        private static TargetSelectorMode mode = TargetSelectorMode.AutoPriority;

        /// <summary>
        ///     Champions that should be prioritized first. (1)
        /// </summary>
        private static readonly string[] HighestPriority =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Brand", "Caitlyn", 
                "Cassiopeia", "Corki", "Draven", "Ezreal", "Graves", "Jinx", 
                "Kalista", "Karma", "Karthus", "Katarina", "Kennen", 
                "KogMaw", "Leblanc", "Lucian", "Lux", "Malzahar", 
                "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", 
                "Syndra", "Talon", "Teemo", "Tristana", "TwistedFate", 
                "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", 
                "Xerath", "Zed", "Ziggs"
            };

        /// <summary>
        ///     Champions that should be prioritized fourth(last). (4)
        /// </summary>
        private static readonly string[] LowestPriority =
            {
                "Alistar", "Amumu", "Blitzcrank", "Braum", "Cho'Gath", 
                "Dr. Mundo", "Garen", "Gnar", "Hecarim", "Janna", 
                "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", 
                "Nautilus", "Nunu", "Olaf", "Rammus", "Renekton", "Sejuani", 
                "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona", 
                "Soraka", "Taric", "Thresh", "Volibear", "Warwick", 
                "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

        /// <summary>
        ///     Champions that should be prioritized second. (2)
        /// </summary>
        private static readonly string[] MedHighPriority =
            {
                "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz", 
                "Heimerdinger", "Jayce", "Kassadin", "Kayle", "Kha'Zix", 
                "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", 
                "Vladimir", "Yasuo", "Zilean"
            };

        /// <summary>
        ///     Champions that should be prioritized third (3)
        /// </summary>
        private static readonly string[] MedLowPriority =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", 
                "Gragas", "Irelia", "Jax", "Lee Sin", "Maokai", "Morgana", 
                "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", 
                "Swain", "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", 
                "XinZhao", "RekSai"
            };

        /// <summary>
        ///     The menu
        /// </summary>
        private static readonly Menu Menu;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="TargetSelector" /> class.
        /// </summary>
        static TargetSelector()
        {
            // Create Menu
            Menu = new Menu("TargetSelector", "Target Selector", true);

            // Create Priority Menu
            var priorityMenu = new Menu("Priorities", "Priorities");
            foreach (var hero in ObjectHandler.EnemyHeroes.Select(x => x.ChampionName))
            {
                priorityMenu.Add(
                    new MenuItem<MenuSlider>(hero, hero) { Value = new MenuSlider(GetPriorityFromDatabase(hero), 1, 4) });
            }

            var autoArrange = new MenuItem<MenuBool>("AutoArrange", "Auto Arrange Priorities")
                                  {
                                     Value = new MenuBool(true) 
                                  };

            autoArrange.ValueChanged += delegate(object sender, ValueChangedEventArgs<MenuBool> args)
                {
                    if (!args.Value.Value)
                    {
                        return;
                    }

                    foreach (var hero in ObjectHandler.EnemyHeroes.Select(x => x.ChampionName))
                    {
                        Menu["Priorities"][hero].GetValue<MenuSlider>().Value = GetPriorityFromDatabase(hero);
                    }
                };

            priorityMenu.Add(autoArrange);

            // Add info
            priorityMenu.Add(
                new MenuItem<MenuSeparator>("Info", "1 = Highest, 4 = Lowest Priority") { Value = new MenuSeparator() });

            Menu.Add(new MenuItem<MenuBool>("FocusSelected", "Focus Selected Target") { Value = new MenuBool(true) });
            Menu.Add(
                new MenuItem<MenuBool>("AttackFocusedTarget", "Only Attack Selected Target") { Value = new MenuBool() });

            var targetingMode = new MenuItem<MenuList<TargetSelectorMode>>("TargetingMode", "Targeting Mode")
                                    {
                                       Value = new MenuList<TargetSelectorMode>() 
                                    };

            targetingMode.ValueChanged +=
                delegate(object sender, ValueChangedEventArgs<MenuList<TargetSelectorMode>> args)
                    {
                        mode = args.Value.SelectedValue;
                    };

            Menu.Add(targetingMode);

            // Add target selector menu to LeagueSharp menu
            Variables.LeagueSharpMenu.Add(Menu);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the selected target.
        /// </summary>
        /// <value>
        ///     The selected target.
        /// </value>
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
        ///     Gets the priority of the champion from the menu.
        /// </summary>
        /// <param name="champName">Name of the champ.</param>
        /// <returns>A value between 1 and 4 representing the priority of the target.</returns>
        public static int GetPriority(string champName)
        {
            return Menu["Priorities"][champName].GetValue<MenuSlider>().Value;
        }

        /// <summary>
        ///     Gets the priority of the champion. (1 being the highest, 4 being the lowest)
        /// </summary>
        /// <param name="champName">Champion Name</param>
        /// <returns>
        ///     Number representing the priority. (1 being the highest, 4 being the lowest)
        /// </returns>
        public static int GetPriorityFromDatabase(string champName)
        {
            if (HighestPriority.Contains(champName))
            {
                return 1;
            }

            if (MedHighPriority.Contains(champName))
            {
                return 2;
            }

            if (MedLowPriority.Contains(champName))
            {
                return 3;
            }

            return LowestPriority.Contains(champName) ? 4 : 1;
        }

        /// <summary>
        ///     Gets a target based on range, damage type, from, and ignores any specified champions.
        /// </summary>
        /// <param name="range">The range to get the enemies in.</param>
        /// <param name="damageType">Damage Type</param>
        /// <param name="from">Position to get enemies around, defaults to player's position.</param>
        /// <param name="ignoredChampions">Any champions to ignore.</param>
        /// <returns>The target</returns>
        /// <exception cref="ArgumentNullException">The LINQ predicate or source is null</exception>
        public static Obj_AI_Hero GetTarget(
            float range, 
            DamageType damageType = DamageType.True, 
            Vector3 from = new Vector3(), 
            IEnumerable<Obj_AI_Hero> ignoredChampions = null)
        {
            if (!from.IsValid())
            {
                from = ObjectManager.Player.ServerPosition;
            }

            if (ignoredChampions == null)
            {
                ignoredChampions = new List<Obj_AI_Hero>();
            }

            // Filter out champions that we are ignoring
            var enemyChamps =
                ObjectHandler.EnemyHeroes.Where(x => x.IsValidTarget(range, true, from))
                    .Where(x => ignoredChampions.Any(y => y.NetworkId != x.NetworkId))
                    .Where(x => !IsInvulnerable(x, damageType));

            if (Menu["FocusSelected"].GetValue<MenuBool>().Value && SelectedTarget != null)
            {
                return SelectedTarget;
            }

            return GetChampionByMode(enemyChamps, damageType);
        }

        /// <summary>
        ///     Gets a target that that can be hit by the spell.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="ignoredChampions">Champions that should be ignored.</param>
        /// <returns>Best target</returns>
        /// <exception cref="ArgumentNullException">The LINQ predicate or source is null.</exception>
        public static Obj_AI_Hero GetTargetNoCollision(Spell spell, IEnumerable<Obj_AI_Hero> ignoredChampions = null)
        {
            if (ignoredChampions == null)
            {
                ignoredChampions = new List<Obj_AI_Hero>();
            }

            var enemyChamps =
                ObjectHandler.EnemyHeroes.Where(x => x.IsValidTarget(spell.Range, true, spell.From))
                    .Where(x => ignoredChampions.Any(y => y.NetworkId != x.NetworkId))
                    .Where(x => spell.GetPrediction(x).Hitchance != HitChance.Collision)
                    .Where(x => !IsInvulnerable(x, spell.DamageType));

            if (Menu["FocusSelected"].GetValue<MenuBool>().Value && SelectedTarget != null)
            {
                return SelectedTarget;
            }

            return GetChampionByMode(enemyChamps, spell.DamageType);
        }

        /// <summary>
        ///     Gets whether or not a target is invulnerable to a spell.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <param name="damageType">Damage Type.</param>
        /// <param name="ignoreShields">Ignore shields(Morgana E, Banshees, etc)</param>
        /// <returns>Whether or not a target is invulnerable to a spell.</returns>
        public static bool IsInvulnerable(Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
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

            if (ignoreShields)
            {
                return false;
            }

            // Morgana's Black Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BlackShield"))
            {
                return true;
            }

            // Banshee's Veil (PASSIVE)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return true;
            }

            // Sivir's Spell Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return true;
            }

            // Nocturne's Shroud of Darkness (W)
            return damageType.Equals(DamageType.Magical) && target.HasBuff("ShroudofDarkness");
        }

        /// <summary>
        ///     Sets the <see cref="TargetSelectorMode" /> of the target selector.
        /// </summary>
        /// <param name="targetSelectorMode">The Mode</param>
        public static void SetPriorityMode(TargetSelectorMode targetSelectorMode)
        {
            mode = targetSelectorMode;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a champion out of a list, and picks the best one based on the mode.
        /// </summary>
        /// <param name="heroes">List of heroes.</param>
        /// <param name="damageType">The type of damage to get.</param>
        /// <returns>The best hero.</returns>
        private static Obj_AI_Hero GetChampionByMode(IEnumerable<Obj_AI_Hero> heroes, DamageType damageType)
        {
            // TODO use Damage.cs
            switch (mode)
            {
                case TargetSelectorMode.LessAttacksToKill:
                    return heroes.MinOrDefault(x => x.Health / ObjectManager.Player.TotalAttackDamage);

                case TargetSelectorMode.MostAbilityPower:
                    return heroes.MaxOrDefault(x => x.TotalMagicalDamage);

                case TargetSelectorMode.MostAttackDamage:
                    return heroes.MaxOrDefault(x => x.TotalAttackDamage);

                case TargetSelectorMode.Closest:
                    return heroes.MinOrDefault(x => x.Distance(ObjectManager.Player));

                case TargetSelectorMode.NearMouse:
                    return heroes.MinOrDefault(x => x.Distance(Game.CursorPos));

                case TargetSelectorMode.LessCastPriority:
                    return heroes.MinOrDefault(x => x.Health / ObjectManager.Player.TotalMagicalDamage);

                case TargetSelectorMode.AutoPriority:
                    return heroes.MinOrDefault(x => GetPriority(x.ChampionName));

                case TargetSelectorMode.LeastHealth:
                    return heroes.MinOrDefault(x => x.Health);
            }

            return null;
        }

        #endregion
    }
}