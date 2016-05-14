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

namespace LeagueSharp.SDK
{
    using System;
    using System.Linq;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

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
                case DamageType.Mixed:
                    damage = source.CalculateMixedDamage(target, damage / 2, damage / 2);
                    break;
                case DamageType.True:
                    damage = Math.Max(Math.Floor(amount), 0);
                    break;
            }

            return damage;
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
            return source.CalculatePhysicalDamage(target, physicalAmount)
                   + source.CalculateMagicDamage(target, magicalAmount);
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
        public static double GetAutoAttackDamage(this Obj_AI_Base source, Obj_AI_Base target)
        {
            double dmgPhysical = source.TotalAttackDamage, dmgMagical = 0, dmgPassive = 0;
            var dmgReduce = 1d;

            var hero = source as Obj_AI_Hero;
            var targetHero = target as Obj_AI_Hero;

            var isMixDmg = false;

            if (hero != null)
            {
                // Spoils Of War
                if (hero.IsMelee() && target is Obj_AI_Minion && target.Team != GameObjectTeam.Neutral
                    && hero.GetBuffCount("TalentReaper") > 0)
                {
                    var eyeEquinox = Items.HasItem(2303, hero);
                    if (
                        GameObjects.Heroes.Any(
                            h => h.Team == hero.Team && !h.Compare(hero) && h.Distance(hero) < (eyeEquinox ? 850 : 1050)))
                    {
                        var spoilwarDmg = 200;
                        if (Items.HasItem((int)ItemId.Targons_Brace, hero))
                        {
                            spoilwarDmg = 240;
                        }
                        else if (Items.HasItem((int)ItemId.Face_of_the_Mountain, hero) || eyeEquinox)
                        {
                            spoilwarDmg = 400;
                        }
                        if (target.Health < spoilwarDmg)
                        {
                            return float.MaxValue;
                        }
                    }
                }

                // Bonus Damage (Passive)
                var passiveInfo = hero.GetPassiveDamageInfo(target);
                dmgPassive += passiveInfo.Value;
                if (passiveInfo.Override)
                {
                    return dmgPassive;
                }

                switch (hero.ChampionName)
                {
                    case "Kalista":
                        dmgPhysical *= 0.9;
                        break;
                    case "Jhin":
                        dmgPhysical +=
                            Math.Round(
                                (new[] { 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40 }[
                                    hero.Level - 1] + (Math.Round((hero.Crit * 100 / 10) * 4))
                                 + (Math.Round(((hero.AttackSpeedMod - 1) * 100 / 10) * 2.5))) / 100 * dmgPhysical);
                        break;
                    case "Corki":
                        dmgPhysical /= 2;
                        dmgMagical = dmgPhysical;
                        isMixDmg = true;
                        break;
                    case "Quinn":
                        if (target.HasBuff("quinnw"))
                        {
                            dmgPhysical += 10 + (5 * hero.Level) + (0.14 + (0.02 * hero.Level)) * hero.TotalAttackDamage;
                        }
                        break;
                }

                // Serrated Dirk
                if (hero.HasBuff("Serrated"))
                {
                    if (!isMixDmg)
                    {
                        dmgPhysical += 15;
                    }
                    else
                    {
                        dmgPhysical += 7.5;
                        dmgMagical += 7.5;
                    }
                }

                if (targetHero != null)
                {
                    // Dorans Shield
                    if (Items.HasItem((int)ItemId.Dorans_Shield, targetHero))
                    {
                        var subDmg = (dmgPhysical + dmgMagical) - 8;
                        dmgPhysical = !isMixDmg ? subDmg : (dmgMagical = subDmg / 2);
                    }

                    // Fervor Of Battle
                    if (hero.GetFerocity(Ferocity.FervorofBattle).IsValid())
                    {
                        var fervorBuffCount = hero.GetBuffCount("MasteryOnHitDamageStacker");
                        if (fervorBuffCount > 0)
                        {
                            dmgPassive += hero.CalculatePhysicalDamage(
                                target,
                                (0.13 + (0.77 * hero.Level)) * fervorBuffCount);
                        }
                    }
                }
                else if (target is Obj_AI_Minion)
                {
                    // Savagery
                    var savagery = hero.GetCunning(Cunning.Savagery);
                    if (savagery.IsValid())
                    {
                        dmgPhysical += savagery.Points;
                    }

                    // RiftHerald P
                    if (!hero.IsMelee() && target.Team == GameObjectTeam.Neutral && target.Name == "SRU_RiftHerald")
                    {
                        dmgReduce *= 0.65;
                    }
                }
            }

            // Ninja Tabi
            if (targetHero != null && !(source is Obj_AI_Turret)
                && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero)))
            {
                dmgReduce *= 0.9;
            }

            dmgPhysical = source.CalculatePhysicalDamage(target, dmgPhysical);
            dmgMagical = source.CalculateMagicDamage(target, dmgMagical);

            // Fizz P
            if (targetHero != null && targetHero.ChampionName == "Fizz")
            {
                dmgPhysical -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
            }

            // This formula is right, work out the math yourself if you don't believe me
            return
                Math.Max(
                    Math.Floor((dmgPhysical + dmgMagical) * dmgReduce + source.PassiveFlatMod(target) + dmgPassive),
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

            bool alreadyAdd1 = false, alreadyAdd2 = false;
            var targetHero = target as Obj_AI_Hero;
            var targetMinion = target as Obj_AI_Minion;

            double dmgBase = 0, dmgBonus = 0, dmgPassive = 0;
            var dmgReduce = 1d;

            if (spellData.DamagesPerLvl?.Count > 0)
            {
                dmgBase = spellData.DamagesPerLvl[Math.Min(source.Level - 1, spellData.DamagesPerLvl.Count - 1)];
            }
            else if (spellData.Damages?.Count > 0)
            {
                dmgBase = spellData.Damages[Math.Min(spellLevel - 1, spellData.Damages.Count - 1)];
                if (!string.IsNullOrEmpty(spellData.ScalingBuff))
                {
                    var buffCount =
                        (spellData.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                            spellData.ScalingBuff);
                    dmgBase = buffCount > 0 ? dmgBase * (buffCount + spellData.ScalingBuffOffset) : 0;
                }
            }
            if (dmgBase > 0)
            {
                if (targetMinion != null && spellData.BonusDamageOnMinion?.Count > 0)
                {
                    dmgBase +=
                        spellData.BonusDamageOnMinion[Math.Min(spellLevel - 1, spellData.BonusDamageOnMinion.Count - 1)];
                }
                if (spellData.IsApplyOnHit || spellData.IsModifiedDamage
                    || spellData.SpellEffectType == SpellEffectType.Single)
                {
                    if (source.HasBuff("Serrated"))
                    {
                        dmgBase += 15;
                    }
                    if (targetHero != null)
                    {
                        if (!spellData.IsApplyOnHit && Items.HasItem((int)ItemId.Dorans_Shield, targetHero))
                        {
                            dmgBase -= 8;
                        }
                    }
                    else if (targetMinion != null)
                    {
                        var savagery = source.GetCunning(Cunning.Savagery);
                        if (savagery.IsValid())
                        {
                            dmgBase += savagery.Points;
                        }
                    }
                    alreadyAdd1 = true;
                }
                dmgBase = source.CalculateDamage(target, spellData.DamageType, dmgBase);
                if (spellData.IsModifiedDamage && spellData.DamageType == DamageType.Physical && targetHero != null
                    && targetHero.ChampionName == "Fizz")
                {
                    dmgBase -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
                    alreadyAdd2 = true;
                }
            }
            if (spellData.BonusDamages?.Count > 0)
            {
                foreach (var bonusDmg in spellData.BonusDamages)
                {
                    var dmg = source.ResolveBonusSpellDamage(target, bonusDmg, spellLevel - 1);
                    if (dmg <= 0)
                    {
                        continue;
                    }
                    if (!alreadyAdd1
                        && (spellData.IsModifiedDamage || spellData.SpellEffectType == SpellEffectType.Single))
                    {
                        if (source.HasBuff("Serrated"))
                        {
                            dmg += 15;
                        }
                        if (targetHero != null)
                        {
                            if (Items.HasItem((int)ItemId.Dorans_Shield, targetHero))
                            {
                                dmg -= 8;
                            }
                        }
                        else if (targetMinion == null)
                        {
                            var savagery = source.GetCunning(Cunning.Savagery);
                            if (savagery.IsValid())
                            {
                                dmg += savagery.Points;
                            }
                        }
                        alreadyAdd1 = true;
                    }
                    dmgBonus += source.CalculateDamage(target, bonusDmg.DamageType, dmg);
                    if (!alreadyAdd2 && spellData.IsModifiedDamage && bonusDmg.DamageType == DamageType.Physical
                        && targetHero != null && targetHero.ChampionName == "Fizz")
                    {
                        dmgBonus -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
                        alreadyAdd2 = true;
                    }
                }
            }

            var totalDamage = dmgBase + dmgBonus;

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
                if (spellData.IsApplyOnHit || spellData.IsModifiedDamage)
                {
                    dmgPassive += source.GetPassiveDamageInfo(target, false).Value;
                    if (targetHero != null)
                    {
                        if (spellData.IsModifiedDamage
                            && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero)))
                        {
                            dmgReduce *= 0.9;
                        }
                        if (source.GetFerocity(Ferocity.FervorofBattle).IsValid())
                        {
                            var fervorBuffCount = source.GetBuffCount("MasteryOnHitDamageStacker");
                            if (fervorBuffCount > 0)
                            {
                                dmgPassive += source.CalculateDamage(
                                    target,
                                    DamageType.Physical,
                                    (0.13 + (0.77 * source.Level)) * fervorBuffCount);
                            }
                        }
                    }
                }
            }

            return
                Math.Max(
                    Math.Floor(
                        totalDamage * dmgReduce
                        + (spellData.IsApplyOnHit || spellData.IsModifiedDamage ? source.PassiveFlatMod(target) : 0)
                        + dmgPassive),
                    0);
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
            if (amount <= 0)
            {
                return 0;
            }

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

            return
                Math.Max(
                    Math.Floor(
                        source.DamageReductionMod(
                            target,
                            source.PassivePercentMod(target, value, DamageType.Magical) * amount,
                            DamageType.Magical)),
                    0);
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
            if (amount <= 0)
            {
                return 0;
            }

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
            return
                Math.Max(
                    Math.Floor(
                        source.DamageReductionMod(
                            target,
                            source.PassivePercentMod(target, value, DamageType.Physical) * amount,
                            DamageType.Physical)),
                    0);
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
            var targetHero = target as Obj_AI_Hero;
            
            if (source is Obj_AI_Hero)
            {
                // Exhaust
                if (source.HasBuff("SummonerExhaust"))
                {
                    amount *= 0.6;
                }

                // Urgot P
                if (source.HasBuff("urgotentropypassive"))
                {
                    amount *= 0.85;
                }

                if (targetHero != null)
                {
                    // Bond Of Stone
                    var bondofstoneBuffCount = targetHero.GetBuffCount("MasteryWardenOfTheDawn");
                    if (bondofstoneBuffCount > 0)
                    {
                        amount *= 1 - (0.06 * bondofstoneBuffCount);
                    }

                    // Phantom Dancer
                    var phantomdancerBuff = source.GetBuff("itemphantomdancerdebuff");
                    if (phantomdancerBuff != null && phantomdancerBuff.Caster.Compare(targetHero))
                    {
                        amount *= 0.88;
                    }
                }
            }

            if (targetHero != null)
            {
                // Bond Of Stone
                if (targetHero.GetResolve(Resolve.BondofStone).IsValid())
                {
                    amount *= 0.96;
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
                    amount *= 1
                              - new[] { 0.5, 0.55, 0.6, 0.65, 0.7 }[targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1
                                    ] / (source is Obj_AI_Turret ? 2 : 1);
                }

                // Urgot R
                if (targetHero.HasBuff("urgotswapdef"))
                {
                    amount *= 1 - new[] { 0.3, 0.4, 0.5 }[targetHero.Spellbook.GetSpell(SpellSlot.R).Level - 1];
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
                                          && g.GetBuff("yorickunholysymbiosis").Caster.Compare(targetHero)))) * 0.05);
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
            var targetHero = target as Obj_AI_Hero;

            // ToughSkin
            if (targetHero != null && targetHero.GetResolve(Resolve.ToughSkin).IsValid()
                && (source is Obj_AI_Hero || (source is Obj_AI_Minion && source.Team == GameObjectTeam.Neutral)))
            {
                value -= 2;
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
                    if (minionType.HasFlag(MinionTypes.Siege))
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
                // DoubleEdgedSword
                if (damageType != DamageType.True && hero.GetFerocity(Ferocity.DoubleEdgedSword).IsValid())
                {
                    amount *= hero.IsMelee() ? 1.03 : 1.02;
                }

                // Oppressor
                if (hero.GetFerocity(Ferocity.Oppressor).IsValid() && target.IsMoveImpaired())
                {
                    amount *= 1.025;
                }

                if (targetHero != null)
                {
                    // Expose Weakness
                    var exposeweakBuff = targetHero.GetBuff("ExposeWeaknessDebuff");
                    if (exposeweakBuff != null && hero.Team == exposeweakBuff.Caster.Team
                        && !exposeweakBuff.Caster.Compare(hero))
                    {
                        amount *= 1.03;
                    }

                    // Assassin
                    if (hero.GetCunning(Cunning.Assassin).IsValid()
                        && !GameObjects.Heroes.Any(
                            h => h.Team == hero.Team && !h.Compare(hero) && h.Distance(hero) < 800))
                    {
                        amount *= 1.02;
                    }

                    // Merciless
                    var merciless = hero.GetCunning(Cunning.Merciless);
                    if (merciless.IsValid() && targetHero.HealthPercent < 40)
                    {
                        amount *= 1 + (merciless.Points / 100);
                    }

                    // Giant Slayer - Lord Dominik's Regards
                    if ((Items.HasItem(3036, hero) || Items.HasItem(3034, hero))
                        && hero.MaxHealth < targetHero.MaxHealth && damageType == DamageType.Physical)
                    {
                        amount *= 1
                                  + Math.Min(targetHero.MaxHealth - hero.MaxHealth, 500) / 50
                                  * (Items.HasItem(3036, hero) ? 0.015 : 0.01);
                    }
                }
            }

            // DoubleEdgedSword
            if (targetHero != null && damageType != DamageType.True
                && targetHero.GetFerocity(Ferocity.DoubleEdgedSword).IsValid())
            {
                amount *= targetHero.IsMelee() ? 1.015 : 1.02;
            }

            return amount;
        }

        #endregion
    }
}