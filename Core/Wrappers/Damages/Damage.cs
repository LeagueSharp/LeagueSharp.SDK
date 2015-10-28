// <copyright file="Damage.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers.Damages
{
    using System;
    using System.Linq;

    using Enumerations;
    using Extensions;
    using Utils;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the calculated damage based on the given damage type onto the target from source.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="damageType">
        ///     The damage type
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            DamageType damageType,
            double amount)
        {
            var damage = 0d;
            switch (damageType)
            {
                case DamageType.Magical:
                    damage = source.CalculateMagicDamage(target, amount);
                    break;
                case DamageType.Physical:
                    damage = source.CalculatePhysicalDamage(target, amount);
                    break;
                case DamageType.True:
                    damage = amount;
                    break;
            }

            return Math.Max(damage, 0d);
        }

        /// <summary>
        ///     Gets the calculated mixed damage onto the target from source.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="physicalAmount">
        ///     The physical amount
        /// </param>
        /// <param name="magicalAmount">
        ///     The magical amount
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateMixedDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double physicalAmount,
            double magicalAmount)
        {
            return
                Math.Max(
                    source.CalculatePhysicalDamage(target, physicalAmount)
                    + source.CalculateMagicDamage(target, magicalAmount),
                    0d);
        }

        /// <summary>
        ///     Gets the source auto attack damage on the target.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="includePassive">
        ///     Indicates whether to include passive effects.
        /// </param>
        /// <returns>
        ///     The estimated auto attack damage.
        /// </returns>
        public static double GetAutoAttackDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            bool includePassive = false)
        {
            double result = source.TotalAttackDamage;
            var damageModifier = 1d;
            var reduction = 0d;
            var passive = 0d;

            if (includePassive)
            {
                var hero = source as Obj_AI_Hero;
                var targetHero = target as Obj_AI_Hero;
                var targetMinion = target as Obj_AI_Minion;

                if (hero != null)
                {
                    // Spoils of War
                    if (hero.IsMelee() && targetMinion != null && targetMinion.IsEnemy
                        && targetMinion.Team != GameObjectTeam.Neutral && hero.GetBuffCount("talentreaperdisplay") > 0)
                    {
                        if (
                            GameObjects.AllyHeroes.Any(
                                h => h.NetworkId != source.NetworkId && h.Distance(targetMinion.Position) < 1100))
                        {
                            var value = 200; // Relic Shield - 3302

                            if (!Items.HasItem(3302, hero))
                            {
                                // 240 = Tragon's Brace (3097)
                                // 400 = Face of the Mountain (3401)
                                value = Items.HasItem(3097, hero) ? 240 : 400;
                            }

                            return value + hero.TotalAttackDamage;
                        }
                    }

                    // BotRK
                    if (Items.HasItem(3153, hero))
                    {
                        var d = 0.08 * target.Health;
                        result += targetMinion != null ? Math.Min(d, 60) : d;
                    }

                    // Arcane blade
                    if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 132 && m.Points == 1))
                    {
                        reduction -= CalculateMagicDamage(hero, target, 0.05 * hero.FlatMagicDamageMod);
                    }
                }

                if (targetHero != null)
                {
                    // Ninja tabi
                    if (Items.HasItem(3047, targetHero))
                    {
                        damageModifier *= 0.9d;
                    }

                    // Nimble Fighter
                    if (targetHero.ChampionName == "Fizz")
                    {
                        reduction += 4 + ((targetHero.Level - (1 / 3)) * 2);
                    }

                    // Block
                    // + Reduces incoming damage from champion basic attacks by 1 / 2
                    if (hero != null)
                    {
                        var mastery =
                            targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68);
                        if (mastery != null && mastery.Points >= 1)
                        {
                            reduction += 1 * mastery.Points;
                        }
                    }
                }

                // Bonus Damage (Passive)
                if (hero != null)
                {
                    var passiveInfo = hero.GetPassiveDamageInfo(target);
                    if (passiveInfo.Override)
                    {
                        return source.CalculatePhysicalDamage(target, -reduction * damageModifier) + passiveInfo.Value;
                    }

                    passive += passiveInfo.Value;
                }
            }

            // This formula is right, work out the math yourself if you don't believe me
            return source.CalculatePhysicalDamage(target, (result - reduction) * damageModifier) + passive;
        }

        /// <summary>
        ///     Get the spell damage value.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="spellSlot">
        ///     The spell slot
        /// </param>
        /// <param name="stage">
        ///     The stage
        /// </param>
        /// <returns>
        ///     The <see cref="double" /> value of damage.
        /// </returns>
        public static double GetSpellDamage(
            this Obj_AI_Hero source,
            Obj_AI_Base target,
            SpellSlot spellSlot,
            DamageStage stage = DamageStage.Default)
        {
            if (source == null || !source.IsValid || target == null || !target.IsValid)
            {
                return 0d;
            }

            var spellLevel = source.Spellbook.GetSpell(spellSlot).Level;
            if (spellLevel == 0)
            {
                return 0d;
            }

            ChampionDamage value;
            if (DamageCollection.TryGetValue(source.ChampionName, out value))
            {
                var baseDamage = 0d;
                var bonusDamage = 0d;

                var spellData = value.GetSlot(spellSlot)?.FirstOrDefault(e => e.Stage == stage)?.SpellData;
                if (spellData?.Damages?.Count > 0)
                {
                    baseDamage = source.CalculateDamage(
                        target,
                        spellData.DamageType,
                        spellData.Damages[Math.Min(spellLevel - 1, spellData.Damages.Count)]);

                    if (!string.IsNullOrEmpty(spellData.ScalingBuff))
                    {
                        var buffCount =
                            (spellData.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                                spellData.ScalingBuff);

                        if (buffCount != 0)
                        {
                            baseDamage *= buffCount + spellData.ScalingBuffOffset;
                        }
                    }
                }

                if (spellData?.BonusDamages?.Count > 0)
                {
                    bonusDamage =
                        spellData.BonusDamages.Sum(
                            instance =>
                            source.CalculateDamage(
                                target,
                                instance.DamageType,
                                source.ResolveBonusSpellDamage(target, instance, spellLevel - 1)));
                }

                return baseDamage + bonusDamage;
            }

            return 0d;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the magic damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculateMagicDamage(this Obj_AI_Base source, Obj_AI_Base target, double amount)
        {
            // Penetration can't reduce magic resist below 0.
            var magicResist = target.SpellBlock;

            double value;

            if (magicResist < 0)
            {
                value = 2 - (100 / (100 - magicResist));
            }
            else if ((magicResist * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod < 0)
            {
                value = 1;
            }
            else
            {
                value = 100 / (100 + (magicResist * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod);
            }

            return source.DamageReductionMod(
                target,
                source.PassivePercentMod(target, value) * amount,
                DamageType.Magical) + source.PassiveFlatMod(target);
        }

        /// <summary>
        ///     Calculates the physical damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount of damage
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculatePhysicalDamage(this Obj_AI_Base source, Obj_AI_Base target, double amount)
        {
            double armorPenetrationPercent = source.PercentArmorPenetrationMod;
            double armorPenetrationFlat = source.FlatArmorPenetrationMod;

            // Minions return wrong percent values.
            if (source is Obj_AI_Minion)
            {
                armorPenetrationFlat = 0d;
                armorPenetrationPercent = 1d;
            }

            // Turrets passive.
            var turret = source as Obj_AI_Turret;
            if (turret != null)
            {
                armorPenetrationFlat = 0d;

                var tier = turret.GetTurretType();
                switch (tier)
                {
                    case TurretType.TierOne:
                    case TurretType.TierTwo:
                        armorPenetrationPercent = 0.7d;
                        break;
                    case TurretType.TierThree:
                    case TurretType.TierFour:
                        armorPenetrationPercent = 0.3d;
                        break;
                }
            }

            // Penetration can't reduce armor below 0.
            var armor = target.Armor;

            double value;
            if (armor < 0)
            {
                value = 2 - (100 / (100 - armor));
            }
            else if ((armor * armorPenetrationPercent) - armorPenetrationFlat < 0)
            {
                value = 1;
            }
            else
            {
                value = 100 / (100 + (armor * armorPenetrationPercent) - armorPenetrationFlat);
            }

            // Take into account the percent passives, flat passives and damage reduction.
            return source.DamageReductionMod(
                target,
                source.PassivePercentMod(target, value) * amount,
                DamageType.Physical) + source.PassiveFlatMod(target);
        }

        /// <summary>
        ///     Calculates the physical damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount of damage
        /// </param>
        /// <param name="ignoreArmorPercent">
        ///     The amount of armor to ignore.
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        internal static double CalculatePhysicalDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            double ignoreArmorPercent)
        {
            return source.CalculatePhysicalDamage(target, amount) * ignoreArmorPercent;
        }

        /// <summary>
        ///     Applies damage reduction mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     damage reduction passives.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="amount">
        ///     The amount.
        /// </param>
        /// <param name="damageType">
        ///     The damage Type.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double DamageReductionMod(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            DamageType damageType)
        {
            if (source is Obj_AI_Hero)
            {
                // Summoners:

                // Exhaust:
                // + Exhausts target enemy champion, reducing their Movement Speed and Attack Speed by 30%, their Armor and Magic Resist by 10, and their damage dealt by 40% for 2.5 seconds.
                if (source.HasBuff("Exhaust"))
                {
                    amount /= 0.4d;
                }
            }

            var targetHero = target as Obj_AI_Hero;
            if (targetHero != null)
            {
                // Items:

                // Doran's Shield
                // + Blocks 8 damage from single target attacks and spells from champions.
                if (Items.HasItem(1054, targetHero))
                {
                    amount -= 8;
                }

                // Passives:

                // Unbreakable Will
                // + Alistar removes all crowd control effects from himself, then gains additional attack damage and takes 70% reduced physical and magic damage for 7 seconds.
                if (target.HasBuff("Ferocious Howl"))
                {
                    amount *= 0.3d;
                }

                // Tantrum
                // + Amumu takes reduced physical damage from basic attacks and abilities.
                if (target.HasBuff("Tantrum") && damageType == DamageType.Physical)
                {
                    amount -= new[] { 2, 4, 6, 8, 10 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Unbreakable
                // + Grants Braum 30% / 32.5% / 35% / 37.5% / 40% damage reduction from oncoming sources (excluding true damage and towers) for 3 / 3.25 / 3.5 / 3.75 / 4 seconds.
                // + The damage reduction is increased to 100% for the first source of champion damage that would be reduced.
                if (target.HasBuff("BraumShieldRaise"))
                {
                    amount -= amount
                              * new[] { 0.3d, 0.325d, 0.35d, 0.375d, 0.4d }[
                                  target.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Idol of Durand
                // + Galio becomes a statue and channels for 2 seconds, Taunt icon taunting nearby foes and reducing incoming physical and magic damage by 50%.
                if (target.HasBuff("GalioIdolOfDurand"))
                {
                    amount *= 0.5d;
                }

                // Courage
                // + Garen gains a defensive shield for a few seconds, reducing incoming damage by 30% and granting 30% crowd control reduction for the duration.
                if (target.HasBuff("GarenW"))
                {
                    amount *= 0.7d;
                }

                // Drunken Rage
                // + Gragas takes a long swig from his barrel, disabling his ability to cast or attack for 1 second and then receives 10% / 12% / 14% / 16% / 18% reduced damage for 3 seconds.
                if (target.HasBuff("GragasWSelf"))
                {
                    amount -= amount
                              * new[] { 0.1d, 0.12d, 0.14d, 0.16d, 0.18d }[
                                  target.Spellbook.GetSpell(SpellSlot.W).Level - 1];
                }

                // Void Stone
                // + Kassadin reduces all magic damage taken by 15%.
                if (target.HasBuff("VoidStone") && damageType == DamageType.Magical)
                {
                    amount *= 0.85d;
                }

                // Shunpo
                // + Katarina teleports to target unit and gains 15% damage reduction for 1.5 seconds. If the target is an enemy, the target takes magic damage.
                if (target.HasBuff("KatarinaEReduction"))
                {
                    amount *= 0.85d;
                }

                // Vengeful Maelstrom
                // + Maokai creates a magical vortex around himself, protecting him and allied champions by reducing damage from non-turret sources by 20% for a maximum of 10 seconds.
                if (target.HasBuff("MaokaiDrainDefense") && !(source is Obj_AI_Turret))
                {
                    amount *= 0.8d;
                }

                // Meditate
                // + Master Yi channels for up to 4 seconds, restoring health each second. This healing is increased by 1% for every 1% of his missing health. Meditate also resets the autoattack timer.
                // + While channeling, Master Yi reduces incoming damage (halved against turrets).
                if (target.HasBuff("Meditate"))
                {
                    amount -= amount
                              * new[] { 0.5d, 0.55d, 0.6d, 0.65d, 0.7d }[
                                  target.Spellbook.GetSpell(SpellSlot.W).Level - 1] / (source is Obj_AI_Turret ? 2 : 1);
                }

                // Valiant Fighter
                // + Poppy reduces all damage that exceeds 10% of her current health by 50%.
                if (target.HasBuff("PoppyValiantFighter") && !(source is Obj_AI_Turret) && amount / target.Health > 0.1d)
                {
                    amount *= 0.5d;
                }

                // Shadow Dash
                // + Shen reduces all physical damage by 50% from taunted enemies.
                if (target.HasBuff("Shen Shadow Dash") && source.HasBuff("Taunt") && damageType == DamageType.Physical)
                {
                    amount *= 0.5d;
                }

                // Unholy Covenant
                // + Yorick grants him 5% damage reduction for each ghoul currently summoned.
                if (target.HasBuff("YorickUnholySymbiosis"))
                {
                    amount -= amount
                              * GameObjects.AttackableUnits.Count(
                                  g =>
                                  g.IsEnemy
                                  && (g.Name.Equals("Clyde") || g.Name.Equals("Inky") || g.Name.Equals("Blinky")))
                              * 0.05d;
                }
            }

            return amount;
        }

        /// <summary>
        ///     Apples passive percent mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     passive flat effects.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The damage after passive flat modifier calculations.
        /// </returns>
        private static double PassiveFlatMod(this GameObject source, Obj_AI_Base target)
        {
            var value = 0d;
            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;

            // Offensive masteries:

            // Butcher
            // + Basic attacks and single target abilities do 2 bonus damage to minions and monsters. 
            if (hero != null && target is Obj_AI_Minion)
            {
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 68 && m.Points == 1))
                {
                    value += 2d;
                }
            }

            // Defensive masteries:

            // Tough Skin
            // + Reduces damage taken from neutral monsters by 1 / 2
            if (source is Obj_AI_Minion && targetHero != null && source.Team == GameObjectTeam.Neutral)
            {
                var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68);
                if (mastery != null && mastery.Points >= 1)
                {
                    value -= 1 * mastery.Points;
                }
            }

            // Unyielding
            // + Melee - Reduces all incoming damage from champions by 2
            // + Ranged - Reduces all incoming damage from champions by 1
            if (hero != null && targetHero != null)
            {
                var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 81);
                if (mastery != null && mastery.Points == 1)
                {
                    value -= targetHero.IsMelee() ? 2 : 1;
                }
            }

            return value;
        }

        /// <summary>
        ///     Apples passive percent mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     passive percent effects.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <returns>
        ///     The damage after passive percent modifier calculations.
        /// </returns>
        private static double PassivePercentMod(this Obj_AI_Base source, Obj_AI_Base target, double amount)
        {
            if (source is Obj_AI_Turret)
            {
                var minionType = target.GetMinionType();

                if (minionType.HasFlag(MinionTypes.Siege) || minionType.HasFlag(MinionTypes.Super))
                {
                    // Siege minions and super minions receive 70% damage from turrets.
                    amount *= 0.7d;
                }
                else if (minionType.HasFlag(MinionTypes.Normal))
                {
                    // Normal minions take 114% more damage from towers.
                    amount *= 1.14285714285714d;
                }
            }

            // Masteries:
            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;
            if (hero != null)
            {
                // Offensive masteries:

                // Double-Edged Sword:
                // + Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
                // + Ranged champions: You deal and take 1.5% increased damage from all sources. 
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
                {
                    amount *= hero.IsMelee() ? 1.02d : 1.015d;
                }

                // Havoc:
                // + Increases damage by 3%.
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 146 && m.Points == 1))
                {
                    amount *= 1.03d;
                }

                // Executioner
                // + Increases damage dealt to champions below 20 / 35 / 50% by 5%. 
                if (targetHero != null)
                {
                    var mastery = hero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Offense && m.Id == 100);
                    if (mastery != null && mastery.Points >= 1
                        && target.Health / target.MaxHealth <= 0.05d + (0.15d * mastery.Points))
                    {
                        amount *= 1.05d;
                    }
                }
            }

            if (targetHero != null)
            {
                // Defensive masteries:

                // Double edge sword:
                // + Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
                // + Ranged champions: You deal and take 1.5% increased damage from all sources.
                if (targetHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
                {
                    amount *= targetHero.IsMelee() ? 1.01d : 1.015d;
                }
            }

            return amount;
        }

        #endregion
    }
}