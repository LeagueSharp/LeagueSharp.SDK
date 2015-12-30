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
    using System.Text.RegularExpressions;

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

            return Math.Max(damage, 0);
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
                    0);
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
        /// <returns>
        ///     The estimated auto attack damage.
        /// </returns>
        public static double GetAutoAttackDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target)
        {
            double result = source.TotalAttackDamage;
            var damageModifier = 1d;
            var passive = 0d;

            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;
            var targetMinion = target as Obj_AI_Minion;

            if (hero != null)
            {
                if (hero.ChampionName == "Kalista")
                {
                    result *= 0.9;
                }

                if (hero.HasBuff("Serrated"))
                {
                    result += 15;
                }

                // Spoils Of War
                if (hero.IsMelee && targetMinion != null && targetMinion.Team != hero.Team
                    && targetMinion.Team != GameObjectTeam.Neutral && hero.GetBuffCount("TalentReaper") > 0
                    && GameObjects.Heroes.Any(
                        h => h.Team == hero.Team && h.NetworkId != hero.NetworkId && h.Distance(targetMinion) < 1100))
                {
                    return result
                           + (Items.HasItem((int)ItemId.Relic_Shield, hero)
                                  ? 200
                                  : (Items.HasItem((int)ItemId.Targons_Brace, hero) ? 240 : 400));
                }

                if (targetHero != null)
                {
                    // Dorans Shield
                    if (Items.HasItem((int)ItemId.Dorans_Shield, targetHero))
                    {
                        result -= 8;
                    }

                    // Fervor Of Battle
                    var fervorofBattle = hero.GetFerocity(DamageMastery.Ferocity.FervorofBattle);
                    if (fervorofBattle.IsValid())
                    {
                        result += (0.9 + (0.42 * hero.Level)) * hero.GetBuffCount("MasteryOnHitDamageStacker");
                    }
                }

                // RiftHerald P
                if (targetMinion != null && hero.IsRanged && targetMinion.Team == GameObjectTeam.Neutral
                    && Regex.IsMatch(targetMinion.Name, "SRU_RiftHerald"))
                {
                    damageModifier *= 0.65;
                }
            }

            // Ninja Tabi
            if (targetHero != null && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero))
                && !(source is Obj_AI_Turret))
            {
                damageModifier *= 0.9;
            }

            // Bonus Damage (Passive)
            if (hero != null)
            {
                var passiveInfo = hero.GetPassiveDamageInfo(target);
                if (passiveInfo.Override)
                {
                    return passiveInfo.Value + source.PassiveFlatMod(target);
                }

                passive += passiveInfo.Value;
            }

            // This formula is right, work out the math yourself if you don't believe me
            return
                Math.Max(
                    source.CalculatePhysicalDamage(target, result) * damageModifier + passive
                    + source.PassiveFlatMod(target),
                    0);
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
                return 0;
            }

            ChampionDamage value;
            if (!DamageCollection.TryGetValue(source.ChampionName, out value))
            {
                return 0;
            }

            var spellData = value.GetSlot(spellSlot)?.FirstOrDefault(e => e.Stage == stage)?.SpellData;
            if (spellData == null)
            {
                return 0;
            }

            var spellLevel =
                source.Spellbook.GetSpell(spellData.ScaleSlot != SpellSlot.Unknown ? spellData.ScaleSlot : spellSlot)
                    .Level;
            if (spellLevel == 0)
            {
                return 0;
            }

            var baseDamage = 0d;
            var bonusDamage = 0d;

            if (spellData.Damages?.Count > 0)
            {
                if (spellData.DamageType == DamageType.Mixed)
                {
                    var oriDamage = spellData.Damages[Math.Min(spellLevel - 1, spellData.Damages.Count - 1)];
                    baseDamage = source.CalculateMixedDamage(target, oriDamage / 2, oriDamage / 2);
                    if (!string.IsNullOrEmpty(spellData.ScalingBuff))
                    {
                        var buffCount =
                            (spellData.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                                spellData.ScalingBuff);
                        baseDamage = buffCount != 0 ? baseDamage * (buffCount + spellData.ScalingBuffOffset) : 0;
                    }
                }
                else
                {
                    baseDamage = source.CalculateDamage(
                        target,
                        spellData.DamageType,
                        spellData.Damages[Math.Min(spellLevel - 1, spellData.Damages.Count - 1)]);
                    if (!string.IsNullOrEmpty(spellData.ScalingBuff))
                    {
                        var buffCount =
                            (spellData.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                                spellData.ScalingBuff);
                        baseDamage = buffCount != 0 ? baseDamage * (buffCount + spellData.ScalingBuffOffset) : 0;
                    }
                }
            }
            if (spellData.DamagesPerLvl?.Count > 0)
            {
                baseDamage = source.CalculateDamage(
                    target,
                    spellData.DamageType,
                    spellData.Damages[Math.Min(source.Level - 1, spellData.DamagesPerLvl.Count - 1)]);
            }
            if (spellData.BonusDamages?.Count > 0)
            {
                foreach (var bonusDmg in
                    spellData.BonusDamages.Where(i => source.ResolveBonusSpellDamage(target, i, spellLevel - 1) > 0))
                {
                    if (bonusDmg.DamageType == DamageType.Mixed)
                    {
                        var oriDamage = source.ResolveBonusSpellDamage(target, bonusDmg, spellLevel - 1);
                        bonusDamage += source.CalculateMixedDamage(target, oriDamage / 2, oriDamage / 2);
                    }
                    else
                    {
                        bonusDamage += source.CalculateDamage(
                            target,
                            bonusDmg.DamageType,
                            source.ResolveBonusSpellDamage(target, bonusDmg, spellLevel - 1));
                    }
                }
            }

            var totalDamage = baseDamage + bonusDamage;
            var passiveDamage = 0d;

            if (totalDamage > 0)
            {
                if (spellData.ScalePerTargetMissHealth > 0)
                {
                    totalDamage *= (target.MaxHealth - target.Health) / target.MaxHealth
                                   * spellData.ScalePerTargetMissHealth + 1;
                }
                if (target is Obj_AI_Minion && spellData.MaxDamageOnMinion?.Count > 0)
                {
                    totalDamage = Math.Min(
                        totalDamage,
                        spellData.MaxDamageOnMinion[Math.Min(spellLevel - 1, spellData.MaxDamageOnMinion.Count - 1)]);
                }
                if (spellData.SpellEffectType == SpellEffectType.Single && target is Obj_AI_Minion)
                {
                    var savagery = source.GetCunning(DamageMastery.Cunning.Savagery);
                    if (savagery.IsValid())
                    {
                        passiveDamage += savagery.Points;
                    }
                }
            }

            return totalDamage + passiveDamage;
        }

        #endregion

        #region Methods

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
                source.PassivePercentMod(target, value, DamageType.Magical) * amount,
                DamageType.Magical);
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
            double bonusArmorPenetrationMod = source.PercentBonusArmorPenetrationMod;

            // Minions return wrong percent values.
            if (source is Obj_AI_Minion)
            {
                armorPenetrationFlat = 0;
                armorPenetrationPercent = 1;
                bonusArmorPenetrationMod = 1;
            }

            // Turrets passive.
            var turret = source as Obj_AI_Turret;
            if (turret != null)
            {
                armorPenetrationFlat = 0;
                bonusArmorPenetrationMod = 1;

                switch (turret.GetTurretType())
                {
                    case TurretType.TierOne:
                    case TurretType.TierTwo:
                        armorPenetrationPercent = 0.7;
                        break;
                    case TurretType.TierThree:
                    case TurretType.TierFour:
                        armorPenetrationPercent = 0.25;
                        break;
                }
            }

            // Penetration can't reduce armor below 0.
            var armor = target.Armor;
            var bonusArmor = armor - target.CharData.Armor;

            double value;
            if (armor < 0)
            {
                value = 2 - (100 / (100 - armor));
            }
            else if ((armor * armorPenetrationPercent) - (bonusArmor * (1 - bonusArmorPenetrationMod))
                     - armorPenetrationFlat < 0)
            {
                value = 1;
            }
            else
            {
                value = 100
                        / (100 + (armor * armorPenetrationPercent) - (bonusArmor * (1 - bonusArmorPenetrationMod))
                           - armorPenetrationFlat);
            }

            // Take into account the percent passives, flat passives and damage reduction.
            return source.DamageReductionMod(
                target,
                source.PassivePercentMod(target, value, DamageType.Physical) * amount,
                DamageType.Physical);
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
            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;
            if (hero != null)
            {
                // Exhaust
                if (hero.HasBuff("Exhaust"))
                {
                    amount *= 0.6;
                }

                if (targetHero != null)
                {
                    // Phantom Dancer
                    if (hero.HasBuff("itemphantomdancerdebuff")
                        && hero.GetBuff("itemphantomdancerdebuff").Caster.NetworkId == targetHero.NetworkId)
                    {
                        amount *= 0.88;
                    }

                    // Urgot P
                    if (hero.HasBuff("urgotentropypassive"))
                    {
                        amount *= 0.85;
                    }
                }
            }

            if (targetHero != null)
            {
                // Bond Of Stone
                if (hero != null && targetHero.GetResolve(DamageMastery.Resolve.BondofStone).IsValid())
                {
                    amount *=
                        GameObjects.Heroes.Any(
                            x =>
                            x.Team == targetHero.Team && x.NetworkId != targetHero.NetworkId
                            && x.Distance(targetHero) <= 1000)
                            ? 0.94
                            : 0.97;
                }

                // Alistar R
                if (targetHero.HasBuff("Ferocious Howl"))
                {
                    amount *= 0.3;
                }

                // Amumu E
                if (targetHero.HasBuff("Tantrum") && damageType == DamageType.Physical)
                {
                    amount -= new[] { 2, 4, 6, 8, 10 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Braum E
                if (targetHero.HasBuff("BraumShieldRaise"))
                {
                    amount *= 1
                              - new[] { 0.3, 0.325, 0.35, 0.375, 0.4 }[
                                  targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Galio R
                if (targetHero.HasBuff("GalioIdolOfDurand"))
                {
                    amount *= 0.5;
                }

                // Garen W
                if (targetHero.HasBuff("GarenW"))
                {
                    amount *= 0.7;
                }

                // Gragas W
                if (targetHero.HasBuff("GragasWSelf"))
                {
                    amount *= 1
                              - new[] { 0.1, 0.12, 0.14, 0.16, 0.18 }[
                                  targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1];
                }

                // Kassadin P
                if (targetHero.HasBuff("VoidStone") && damageType == DamageType.Magical)
                {
                    amount *= 0.85;
                }

                // Katarina E
                if (targetHero.HasBuff("KatarinaEReduction"))
                {
                    amount *= 0.85;
                }

                // Maokai R
                if (targetHero.HasBuff("MaokaiDrainDefense") && !(source is Obj_AI_Turret))
                {
                    amount *= 0.8;
                }

                // MasterYi W
                if (targetHero.HasBuff("Meditate"))
                {
                    amount *= (1
                               - new[] { 0.5, 0.55, 0.6, 0.65, 0.7 }[
                                   targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1])
                              / (source is Obj_AI_Turret ? 2 : 1);
                }

                // Shen E
                if (targetHero.HasBuff("Shen Shadow Dash") && hero != null && hero.HasBuff("Taunt")
                    && damageType == DamageType.Physical)
                {
                    amount *= 0.5;
                }

                // Yorick P
                if (targetHero.HasBuff("YorickUnholySymbiosis"))
                {
                    amount *= 1
                              - (GameObjects.Minions.Count(
                                  g =>
                                  g.Team == targetHero.Team
                                  && (g.Name.Equals("Clyde") || g.Name.Equals("Inky") || g.Name.Equals("Blinky")
                                      || (g.HasBuff("yorickunholysymbiosis")
                                          && g.GetBuff("yorickunholysymbiosis").Caster.NetworkId == targetHero.NetworkId)))
                                 * 0.05);
                }

                // Urgot R
                if (targetHero.HasBuff("urgotswapdef"))
                {
                    amount *= 1 - new[] { 0.3, 0.4, 0.5 }[targetHero.Spellbook.GetSpell(SpellSlot.R).Level - 1];
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

            if (hero != null && target is Obj_AI_Minion)
            {
                // Savagery
                var savagery = hero.GetCunning(DamageMastery.Cunning.Savagery);
                if (savagery.IsValid())
                {
                    value += savagery.Points;
                }
            }

            // ToughSkin
            if (targetHero != null
                && (hero != null || (source is Obj_AI_Minion && source.Team == GameObjectTeam.Neutral))
                && targetHero.GetResolve(DamageMastery.Resolve.ToughSkin).IsValid())
            {
                value -= 2;
            }

            // Fizz P
            if (targetHero != null && targetHero.ChampionName == "Fizz")
            {
                value -= new[] { 4, 6, 8, 10, 12, 14 }[(targetHero.Level - 1) / 3];
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
        /// <param name="damageType">
        ///     The damage Type.
        /// </param>
        /// <returns>
        ///     The damage after passive percent modifier calculations.
        /// </returns>
        private static double PassivePercentMod(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            DamageType damageType)
        {
            if (source is Obj_AI_Turret)
            {
                var minion = target as Obj_AI_Minion;
                if (minion != null)
                {
                    var minionType = minion.GetMinionType();
                    if (minionType.HasFlag(MinionTypes.Siege) || minionType.HasFlag(MinionTypes.Super))
                    {
                        // Siege minions and super minions receive 70% damage from turrets.
                        amount *= 0.7;
                    }
                    else if (minionType.HasFlag(MinionTypes.Normal))
                    {
                        // Normal minions take 114% more damage from towers.
                        amount *= 1.14285714285714;
                    }
                }
            }

            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;
            if (hero != null)
            {
                // Dragon Buff
                if (hero.GetBuffCount("s5test_dragonslayerbuff") >= 2)
                {
                    var buffCount = hero.GetBuffCount("s5test_dragonslayerbuff");
                    var bonusPercent = 1.15 * (buffCount == 5 ? 2 : 1);
                    if (buffCount >= 2 && target is Obj_AI_Turret)
                    {
                        amount *= bonusPercent;
                    }
                    if (buffCount >= 4 && target is Obj_AI_Minion)
                    {
                        amount *= bonusPercent;
                    }
                }

                // DoubleEdgedSword
                if (hero.GetFerocity(DamageMastery.Ferocity.DoubleEdgedSword).IsValid())
                {
                    amount *= hero.IsMelee ? 1.03 : 1.02;
                }

                if (targetHero != null)
                {
                    // Oppressor
                    if (hero.GetFerocity(DamageMastery.Ferocity.Oppressor).IsValid() && targetHero.IsMoveImpaired())
                    {
                        amount *= 1.025;
                    }

                    // Assassin
                    /*if (hero.GetCunning(DamageMastery.Cunning.Assassin).IsValid()
                        && !GameObjects.Heroes.Any(
                            h => h.Team == hero.Team && h.NetworkId != hero.NetworkId && h.Distance(hero) <= 800))
                    {
                        amount *= 1.015;
                    }*/

                    // Merciless
                    var merciless = hero.GetCunning(DamageMastery.Cunning.Merciless);
                    if (merciless.IsValid() && targetHero.HealthPercent < 40)
                    {
                        amount *= 1 + (merciless.Points / 100);
                    }

                    // Giant Slayer - Lord Dominik's Regards
                    if ((Items.HasItem(3034, hero) || Items.HasItem(3036, hero))
                        && hero.MaxHealth < targetHero.MaxHealth && damageType == DamageType.Physical)
                    {
                        amount *= 1
                                  + Math.Min(targetHero.MaxHealth - hero.MaxHealth, 500) / 50
                                  * (Items.HasItem(3034, hero) ? 0.01 : 0.015);
                    }
                }
            }

            if (targetHero != null)
            {
                // DoubleEdgedSword
                if (targetHero.GetFerocity(DamageMastery.Ferocity.DoubleEdgedSword).IsValid())
                {
                    amount *= targetHero.IsMelee ? 1.015 : 1.02;
                }
            }

            return amount;
        }

        #endregion
    }
}