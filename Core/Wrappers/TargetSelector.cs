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
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;

    using Color = System.Drawing.Color;

    /// <summary>
    ///     Target Selector, manageable utility to return the best candidate target based on chosen settings.
    /// </summary>
    public static class TargetSelector
    {
        #region Static Fields

        private static Obj_AI_Hero focusedHero;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Menu showed when you hold SHIFT.
        ///     You have to use it to get values setted on menu.
        /// </summary>
        public static Menu Menu { get; private set; }

        /// <summary>
        ///     The current mode of the TargetSelector
        /// </summary>
        public static TargetSelectorMode Mode
        {
            get
            {
                return (TargetSelectorMode)Menu.GetValue<MenuList>("tsMode").SelectedValueAsObject;
            }
        }

        #endregion

        #region Properties

        private static Obj_AI_Hero SelectedHero
        {
            get
            {
                return Hud.SelectedUnit as Obj_AI_Hero;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Use this to get the selected champion.
        ///     Range: Auto Attack
        /// </summary>
        /// <returns>
        ///     Selected champion.
        ///     It will return null if the champ isn't on range or can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetSelectedTarget()
        {
            CheckInitialized();

            return GetSelectedTarget(GetPlayerAutoAttackRange());
        }

        /// <summary>
        ///     Use this to get the selected champion.
        ///     Range: Specified by the "range" parameter
        /// </summary>
        /// <param name="range">
        ///     The range that the selected target will be searched
        /// </param>
        /// <returns>
        ///     Selected champion.
        ///     It will return null if the champ isn't on then specified range or can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetSelectedTarget(float range)
        {
            CheckInitialized();

            return GetSelectedTarget(GetPlayerPosition(), range);
        }

        /// <summary>
        ///     Use this to get the selected champion.
        ///     Range: Specified by the "range" parameter
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetSelectedTarget(float range) if you wanna check from your own position.
        /// </param>
        /// <param name="range">
        ///     The range that the selected target will be searched
        /// </param>
        /// <returns>
        ///     Selected champion.
        ///     It will return null if the champ isn't on then specified range or can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetSelectedTarget(Vector3 rangeCheckFrom, float range)
        {
            CheckInitialized();

            if (SelectedHero == null || !SelectedHero.IsValid || !SelectedHero.IsVisible || !SelectedHero.IsTargetable
                || SelectedHero.IsDead || SelectedHero.IsInvulnerable(DamageType.True) || SelectedHero.IsAlly)
            {
                return null;
            }

            return SelectedHero.Distance(rangeCheckFrom) <= range ? SelectedHero : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="range"></param>
        /// <param name="damageType"></param>
        /// <param name="ignoredChamps"></param>
        /// <param name="rangeCheckFrom"></param>
        /// <returns></returns>
        [Obsolete("Use another overload of GetTarget")]
        public static Obj_AI_Hero GetTarget(
            float? range,
            DamageType damageType = DamageType.Physical,
            IEnumerable<Obj_AI_Hero> ignoredChamps = null,
            Vector3? rangeCheckFrom = null)
        {
            CheckInitialized();

            var checkRange = range ?? GetPlayerAutoAttackRange();
            var from = rangeCheckFrom ?? GetPlayerPosition();

            return GetSelectedTarget(from, checkRange) ?? GetTarget(from, checkRange, ignoredChamps);
        }

        /// <summary>
        ///     Get the best target at you auto attack range.
        /// </summary>
        /// <returns>
        ///     Best target to auto attack.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget()
        {
            CheckInitialized();

            return GetTarget(GetPlayerAutoAttackRange());
        }

        /// <summary>
        ///     Get the best target at a specific range.
        /// </summary>
        /// <param name="range">
        ///     The range that it will search for targets.
        /// </param>
        /// <returns>
        ///     Best target on range.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(float range)
        {
            CheckInitialized();

            return GetTarget(range, new List<Obj_AI_Hero>());
        }

        /// <summary>
        ///     Get the best target to cast a spell.
        /// </summary>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <returns>
        ///     Best target to cast a spell.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Spell spell)
        {
            CheckInitialized();

            return GetTarget(GetPlayerPosition(), spell);
        }

        /// <summary>
        ///     Get the best target to auto attack into a specific range checked from another location,
        ///     different of the current champion position.
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTarget() if you wanna check from your own position.
        /// </param>
        /// <returns>
        ///     Best target on range.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Vector3 rangeCheckFrom)
        {
            CheckInitialized();

            return GetTarget(rangeCheckFrom, GetPlayerAutoAttackRange());
        }

        /// <summary>
        ///     Get the best target to cast a spell ignoring some champions
        /// </summary>
        /// The spell you wanna search the best target.
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore
        /// </param>
        /// <returns>
        ///     Best target to cast a spell.
        ///     It will be null if there is no available champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Spell spell, IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            return GetTarget(GetPlayerPosition(), spell, ignoredChamps);
        }

        /// <summary>
        ///     Get the best target to cast a spell checked from another location different of the current champion position.
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTarget(Spell) if you wanna check from your own position.
        /// </param>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <returns>
        ///     Best target to cast a spell.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Vector3 rangeCheckFrom, Spell spell)
        {
            CheckInitialized();

            return GetTarget(rangeCheckFrom, spell, new List<Obj_AI_Hero>());
        }

        /// <summary>
        ///     Get the best target into a specific range checked from another location, different of the current champion
        ///     position.
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTarget(Spell) if you wanna check from your own position.
        /// </param>
        /// <param name="range">
        ///     The range that it will search for targets.
        /// </param>
        /// <returns>
        ///     Best target on range.
        ///     It will be null if there is no champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Vector3 rangeCheckFrom, float range)
        {
            CheckInitialized();

            return GetTarget(rangeCheckFrom, range, new List<Obj_AI_Hero>());
        }

        /// <summary>
        ///     Get the best target into a specific range ignoring some champions
        /// </summary>
        /// <param name="range">
        ///     The range that it will search for targets.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore
        /// </param>
        /// <returns>
        ///     Best target on range.
        ///     It will be null if there is no available champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(float range, IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            return GetTarget(GetPlayerPosition(), range, ignoredChamps);
        }

        /// <summary>
        ///     Get the best target to cast a spell checking from a different position of your current and ignoring some champions.
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTarget(Spell, IgnoredChamps) if you wanna check from your own position.
        /// </param>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore
        /// </param>
        /// <returns>
        ///     The best target to cast a spell.
        ///     It will be null if there is no available champions to cast or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Vector3 rangeCheckFrom, Spell spell, IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            if (Menu.GetValue<MenuBool>("tsFocusSelected").Value)
            {
                var selected = GetSelectedTarget(rangeCheckFrom, spell.Range);

                if (selected != null)
                {
                    focusedHero = selected;
                    return selected;
                }
            }

            var possibleTargets = GetTargetsOnRange(rangeCheckFrom, spell.Range, ignoredChamps);

            focusedHero = GetBestTarget(rangeCheckFrom, possibleTargets, spell);

            return focusedHero;
        }

        /// <summary>
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTarget(Range, IgnoredChamps) if you wanna check from your own position.
        /// </param>
        /// <param name="range">
        ///     The range that it will search for targets.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore
        /// </param>
        /// <returns>
        ///     Best target on range.
        ///     It will be null if there is no available champions on range or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTarget(Vector3 rangeCheckFrom, float range, IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            if (Menu.GetValue<MenuBool>("tsFocusSelected").Value)
            {
                var selected = GetSelectedTarget(rangeCheckFrom, range);

                if (selected != null)
                {
                    focusedHero = selected;
                    return selected;
                }
            }

            var possibleTargets = GetTargetsOnRange(rangeCheckFrom, range, ignoredChamps);

            focusedHero = GetBestTarget(rangeCheckFrom, possibleTargets);

            return focusedHero;
        }

        /// <summary>
        ///     Get the best target to cast a spell checking collision
        /// </summary>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <returns>
        ///     Best target to spell.
        ///     It will be null if there is no champions to spell or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTargetNoCollision(Spell spell)
        {
            CheckInitialized();

            return GetTargetNoCollision(GetPlayerPosition(), spell);
        }

        /// <summary>
        ///     Get the best target to cast a spell from another location different of your current and checking collision.
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTargetNoCollision(Spell) if you wanna check from your own position.
        /// </param>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <returns>
        ///     Best target to spell.
        ///     It will be null if there is no champions to spell or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTargetNoCollision(Vector3 rangeCheckFrom, Spell spell)
        {
            CheckInitialized();

            return GetTargetNoCollision(rangeCheckFrom, spell, new List<Obj_AI_Hero>());
        }

        /// <summary>
        ///     Get the best target to cast a spell ignoring some champions.
        /// </summary>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore.
        /// </param>
        /// <returns>
        ///     Best target to spell.
        ///     It will be null if there is no champions to spell or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTargetNoCollision(Spell spell, IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            return GetTargetNoCollision(GetPlayerPosition(), spell, ignoredChamps);
        }

        /// <summary>
        ///     Get the best target to cast a spell checked from another location, different of your current position
        ///     and ignoring some champions
        /// </summary>
        /// <param name="rangeCheckFrom">
        ///     From where it will searched.
        ///     Use GetTargetNoCollision(Spell, IgnoredChamps) if you wanna check from your own position.
        /// </param>
        /// <param name="spell">
        ///     The spell you wanna search for the best target.
        /// </param>
        /// <param name="ignoredChamps">
        ///     List of champions you want ignore.
        /// </param>
        /// <returns>
        ///     Best target to spell.
        ///     It will be null if there is no champions to spell or if they can't be targetable.
        /// </returns>
        public static Obj_AI_Hero GetTargetNoCollision(
            Vector3 rangeCheckFrom,
            Spell spell,
            IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            CheckInitialized();

            var possibleTargets =
                GetTargetsOnRange(rangeCheckFrom, spell.Range, new List<Obj_AI_Hero>())
                    .Where(target => spell.GetPrediction(target).Hitchance != HitChance.Collision);

            focusedHero = GetBestTarget(rangeCheckFrom, possibleTargets, spell);

            return focusedHero;
        }

        /// <summary>
        ///     Put the target selector options on Menu
        /// </summary>
        /// <param name="mainMenu">
        ///     Parent menu.
        ///     if null the menu will shown as root menu.
        /// </param>
        internal static Menu Initialize(Menu mainMenu = null)
        {
            if (Menu == null)
            {
                LoadMenu(mainMenu);

                if (Menu["tsDraws"].GetValue<MenuBool>("tsDrawSelectedTarget").Value)
                {
                    Drawing.OnDraw += Drawing_OnDraw_SelectedTarget;
                }

                if (Menu["tsDraws"].GetValue<MenuBool>("tsDrawFocusedTarget").Value)
                {
                    Drawing.OnDraw += Drawing_OnDraw_FocusedTarget;
                }
            }

            return Menu;
        }

        #endregion

        #region Methods

        private static void CheckInitialized(string message = "")
        {
            if (Menu == null)
            {
                var msg = message != ""
                              ? message
                              : "\n\n>>>>> TargetSelector not initialized. Use Bootstrap.Init(string[] args) <<<<<\n";
                throw new TargetSelectorNotInitializedException(msg);
            }
        }

        private static void Drawing_OnDraw_FocusedTarget(EventArgs args)
        {
            if (focusedHero != null)
            {
                Drawing.DrawCircle(focusedHero.Position, 90f, Color.Chartreuse);
            }
        }

        private static void Drawing_OnDraw_SelectedTarget(EventArgs args)
        {
            if (SelectedHero != null)
            {
                Drawing.DrawCircle(SelectedHero.Position, 150f, Color.DarkRed);
            }
        }

        private static Obj_AI_Hero GetBestTarget(
            Vector3 rangeCheckFrom,
            IEnumerable<Obj_AI_Hero> possibleTargets,
            Spell spell = null)
        {
            if (!possibleTargets.Any())
            {
                return null;
            }

            switch (Mode)
            {
                case TargetSelectorMode.LeastHealth:
                    return GetLowLifeTarget(possibleTargets, spell);
                case TargetSelectorMode.Closest:
                    return GetClosestTarget(rangeCheckFrom, possibleTargets, spell);
                case TargetSelectorMode.LessAttacksToKill:
                    return GetLessAttackToKillTarget(possibleTargets, spell);
                case TargetSelectorMode.MostAbilityPower:
                    return GetMostAbilityPowerTarget(possibleTargets, spell);
                case TargetSelectorMode.MostAttackDamage:
                    return GetMostAttackDamageTarget(possibleTargets, spell);
                case TargetSelectorMode.NearMouse:
                    return GetNearMouseTarget(possibleTargets, spell);
                default:
                    return null;
            }
        }

        private static Obj_AI_Hero GetClosestTarget(
            Vector3 rangeCheckFrom,
            IEnumerable<Obj_AI_Hero> possibleTargets,
            Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenBy(champ => champ.Distance(rangeCheckFrom))
                    .First();
        }

        private static Obj_AI_Hero GetLessAttackToKillTarget(IEnumerable<Obj_AI_Hero> possibleTargets, Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenBy(champ => champ.Health / ObjectManager.Player.GetAutoAttackDamage(champ))
                    .First();
        }

        private static Obj_AI_Hero GetLowLifeTarget(IEnumerable<Obj_AI_Hero> possibleTargets, Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenBy(champ => champ.Health)
                    .First();
        }

        private static Obj_AI_Hero GetMostAbilityPowerTarget(IEnumerable<Obj_AI_Hero> possibleTargets, Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenByDescending(champ => champ.TotalMagicalDamage)
                    .ThenBy(champ => champ.Health / ObjectManager.Player.GetAutoAttackDamage(champ))
                    .First();
        }

        private static Obj_AI_Hero GetMostAttackDamageTarget(IEnumerable<Obj_AI_Hero> possibleTargets, Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenByDescending(champ => champ.TotalAttackDamage)
                    .ThenBy(champ => champ.Health / ObjectManager.Player.GetAutoAttackDamage(champ))
                    .First();
        }

        private static Obj_AI_Hero GetNearMouseTarget(IEnumerable<Obj_AI_Hero> possibleTargets, Spell spell)
        {
            return
                possibleTargets.OrderByDescending(champ => GetPriority(champ, spell))
                    .ThenBy(champ => champ.Distance(Game.CursorPos))
                    .First();
        }

        private static float GetPlayerAutoAttackRange()
        {
            return ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
        }

        private static Vector3 GetPlayerPosition()
        {
            return ObjectManager.Player.Position;
        }

        private static int GetPriority(Obj_AI_Hero champ, Spell spell)
        {
            var damageDealt = (spell == null)
                                  ? ObjectManager.Player.GetAutoAttackDamage(champ)
                                  : ObjectManager.Player.GetSpellDamage(champ, spell.Slot);

            return GetPriority(champ, damageDealt);
        }

        private static byte GetPriority(Obj_AI_Hero champ, double damageDealt)
        {
            if (damageDealt > champ.Health)
            {
                return byte.MaxValue;
            }
            else if (
                Menu["tsMaxPriorityMenu"].GetValue<MenuBool>(
                    string.Format("tsPriority{0}", champ.ChampionName.ToLowerInvariant())).Value)
            {
                return byte.MaxValue - 1;
            }
            else
            {
                return 1;
            }
        }

        private static IEnumerable<Obj_AI_Hero> GetTargetsOnRange(
            Vector3 rangeCheckFrom,
            float range,
            IEnumerable<Obj_AI_Hero> ignoredChamps)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Except(ignoredChamps)
                    .Where(
                        champ =>
                        champ.IsEnemy && champ.Distance(rangeCheckFrom) <= range && champ.IsValid && champ.IsVisible
                        && champ.IsTargetable && !champ.IsInvulnerable(DamageType.True) && !champ.IsDead)
                    .OrderBy(champ => champ.Health / ObjectManager.Player.GetAutoAttackDamage(champ));
        }

        private static void LoadMenu(Menu mainMenu)
        {
            Menu = new Menu("tsmenu", "Target Selector", mainMenu == null);

            Menu.Add(
                new MenuList<TargetSelectorMode>("tsMode", "Mode")
                    { SelectedValue = TargetSelectorMode.LessAttacksToKill });

            var maxPriorityMenu = new Menu("tsMaxPriorityMenu", "Max priority");

            foreach (
                var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(champ => champ.IsEnemy).OrderBy(champ => champ.Name)
                )
            {
                var menuName = string.Format("tsPriority{0}", enemy.ChampionName.ToLowerInvariant());
                var menuDisplay = enemy.ChampionName;
                maxPriorityMenu.Add(new MenuBool(menuName, menuDisplay, false));
            }

            Menu.Add(maxPriorityMenu);

            Menu.Add(new MenuBool("tsFocusSelected", "Focus selected target", true));

            var targetSelectorDrawMenu = new Menu("tsDraws", "Draws");

            var tsDrawSelectedTargetMenuBool = new MenuBool("tsDrawSelectedTarget", "Selected target", true);
            tsDrawSelectedTargetMenuBool.ValueChanged += delegate(object sender, EventArgs args)
                {
                    if (tsDrawSelectedTargetMenuBool.Value)
                    {
                        Drawing.OnDraw += Drawing_OnDraw_SelectedTarget;
                    }
                    else
                    {
                        Drawing.OnDraw -= Drawing_OnDraw_SelectedTarget;
                    }
                };
            targetSelectorDrawMenu.Add(tsDrawSelectedTargetMenuBool);

            var tsDrawFocusedTargetMenuBool = new MenuBool("tsDrawFocusedTarget", "FocusedTarget", true);
            tsDrawFocusedTargetMenuBool.ValueChanged += delegate(object sender, EventArgs args)
                {
                    if (tsDrawSelectedTargetMenuBool.Value)
                    {
                        Drawing.OnDraw += Drawing_OnDraw_FocusedTarget;
                    }
                    else
                    {
                        Drawing.OnDraw -= Drawing_OnDraw_FocusedTarget;
                    }
                };
            targetSelectorDrawMenu.Add(tsDrawFocusedTargetMenuBool);

            Menu.Add(targetSelectorDrawMenu);

            if (mainMenu != null)
            {
                mainMenu.Add(Menu);
            }
            else
            {
                Menu.Attach();
            }
        }

        #endregion
    }

    internal class TargetSelectorNotInitializedException : Exception
    {
        #region Constructors and Destructors

        internal TargetSelectorNotInitializedException(string message)
            : base(message)
        {
        }

        #endregion
    }
}