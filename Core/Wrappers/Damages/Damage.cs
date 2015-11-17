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

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;

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
                    if (hero.IsMelee && targetMinion != null && targetMinion.IsEnemy
                        && targetMinion.Team != GameObjectTeam.Neutral && hero.GetBuffCount("TalentReaper") > 0
                        && GameObjects.AllyHeroes.Any(
                            h => h.NetworkId != hero.NetworkId && h.Distance(targetMinion) < 1100))
                    {
                        return hero.TotalAttackDamage
                               + (Items.HasItem((int)ItemId.Relic_Shield, hero)
                                      ? 200
                                      : (Items.HasItem((int)ItemId.Targons_Brace, hero) ? 240 : 400));
                    }

                    // BotRK
                    if (Items.HasItem((int)ItemId.Blade_of_the_Ruined_King, hero))
                    {
                        var d = 0.06 * target.Health;
                        result += targetMinion != null ? Math.Min(d, 60) : d;
                    }

                    if (hero.IsMelee && hero.GetBuffCount("DreadnoughtMomentumBuff") > 0)
                    {
                        result += hero.GetBuffCount("DreadnoughtMomentumBuff") / 2
                                  * (hero.GetBuffCount("DreadnoughtMomentumBuff") == 100 ? 2 : 1);
                    }

                    if (hero.GetBuffCount("kagesluckypickdisplay") > 0
                        && (targetHero != null || target is Obj_AI_Turret))
                    {
                        result += Items.HasItem((int)ItemId.Spellthiefs_Edge, hero) ? 10 : 15;
                    }

                    // Serrated Dirk
                    /*if (hero.HasBuff(""))
                    {
                        result += 15;
                    }*/

                    if (targetHero != null)
                    {
                        // Fervorofbattle:
                        // + STACKTIVATE Your basic attacks and spells give you stacks of Fervor for 5 seconds, stacking 10 times. Each stack of Fervor adds 1-8 bonus physical damage to your basic attacks against champions, based on your level.
                        var fervorofBattle = hero.GetFerocity(DamageMastery.Ferocity.FervorofBattle);
                        if (fervorofBattle.IsValid())
                        {
                            result += (1 + (hero.Level - 1) * 7 / 17) * hero.GetBuffCount("MasteryOnHitDamageStacker");
                        }

                        // Phantom Dancer:
                        // + Reduces damage dealt to attacker (if he's attack you) by 12%
                        if (hero.HasBuff("itemphantomdancerdebuff")
                            && hero.GetBuff("itemphantomdancerdebuff").Caster.NetworkId == targetHero.NetworkId)
                        {
                            damageModifier *= 0.88d;
                        }

                        if ((Items.HasItem(3034, hero) || Items.HasItem(3036, hero))
                            && hero.MaxHealth < targetHero.MaxHealth)
                        {
                            result *= Math.Min(targetHero.MaxHealth - hero.MaxHealth, 500) / 50
                                      * (Items.HasItem(3034, hero) ? 0.01 : 0.015) + 1;
                        }
                    }
                }

                if (targetHero != null)
                {
                    // Ninja tabi
                    if (Items.HasItem((int)ItemId.Ninja_Tabi, targetHero))
                    {
                        damageModifier *= 0.9d;
                    }

                    // Nimble Fighter
                    if (targetHero.ChampionName == "Fizz")
                    {
                        reduction += 4 + ((targetHero.Level - (1 / 3)) * 2);
                    }

                    // Tough Skin
                    // + Take 2 less damage from champion and monster basic attacks
                    if ((source is Obj_AI_Minion || hero != null)
                        && targetHero.GetResolve(DamageMastery.Resolve.ToughSkin).IsValid())
                    {
                        reduction += 2;
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

            ChampionDamage value;
            if (!DamageCollection.TryGetValue(source.ChampionName, out value))
            {
                return 0d;
            }

            var spellData = value.GetSlot(spellSlot)?.FirstOrDefault(e => e.Stage == stage)?.SpellData;
            if (spellData == null)
            {
                return 0d;
            }

            var spellLevel =
                source.Spellbook.GetSpell(spellData.ScaleSlot != SpellSlot.Unknown ? spellData.ScaleSlot : spellSlot)
                    .Level;
            if (spellLevel == 0)
            {
                return 0d;
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
                        baseDamage = buffCount != 0 ? baseDamage * (buffCount + spellData.ScalingBuffOffset) : 0d;
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
                        baseDamage = buffCount != 0 ? baseDamage * (buffCount + spellData.ScalingBuffOffset) : 0d;
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
            }

            return totalDamage;
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
                if (target is Obj_AI_Minion)
                {
                    amount *= 1.25;
                    if (target.GetMinionType() == MinionTypes.Siege)
                    {
                        amount *= 0.7;
                    }
                    return amount;
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
                    amount *= 0.6d;
                }
            }

            var targetHero = target as Obj_AI_Hero;
            if (targetHero != null)
            {
                // Masteries:

                // BondofStone
                // + DAMAGE REDUCTION 4 %, doubling to 8 % when near at least one allied champion
                // + IN THIS TOGETHER 8 % of the damage that the nearest allied champion would take is dealt to you instead.This can't bring you below 15% health.
                if (targetHero.GetResolve(DamageMastery.Resolve.BondofStone).IsValid())
                {
                    amount *=
                        GameObjects.EnemyHeroes.Any(
                            x => x.NetworkId != targetHero.NetworkId && x.Distance(targetHero) <= 500)
                            ? 0.92d
                            : 0.96d;
                }

                // Items:

                // Doran's Shield
                // + Blocks 8 damage from single target attacks and spells from champions.
                if (Items.HasItem((int)ItemId.Dorans_Shield, targetHero))
                {
                    amount -= 8;
                }

                // Passives:

                // Unbreakable Will
                // + Alistar removes all crowd control effects from himself, then gains additional attack damage and takes 70% reduced physical and magic damage for 7 seconds.
                if (targetHero.HasBuff("Ferocious Howl"))
                {
                    amount *= 0.3d;
                }

                // Tantrum
                // + Amumu takes reduced physical damage from basic attacks and abilities.
                if (targetHero.HasBuff("Tantrum") && damageType == DamageType.Physical)
                {
                    amount -= new[] { 2, 4, 6, 8, 10 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Unbreakable
                // + Grants Braum 30% / 32.5% / 35% / 37.5% / 40% damage reduction from oncoming sources (excluding true damage and towers) for 3 / 3.25 / 3.5 / 3.75 / 4 seconds.
                // + The damage reduction is increased to 100% for the first source of champion damage that would be reduced.
                if (targetHero.HasBuff("BraumShieldRaise"))
                {
                    amount *= 1
                              - new[] { 0.3d, 0.325d, 0.35d, 0.375d, 0.4d }[
                                  targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }

                // Idol of Durand
                // + Galio becomes a statue and channels for 2 seconds, Taunt icon taunting nearby foes and reducing incoming physical and magic damage by 50%.
                if (targetHero.HasBuff("GalioIdolOfDurand"))
                {
                    amount *= 0.5d;
                }

                // Courage
                // + Garen gains a defensive shield for a few seconds, reducing incoming damage by 30% and granting 30% crowd control reduction for the duration.
                if (targetHero.HasBuff("GarenW"))
                {
                    amount *= 0.7d;
                }

                // Drunken Rage
                // + Gragas takes a long swig from his barrel, disabling his ability to cast or attack for 1 second and then receives 10% / 12% / 14% / 16% / 18% reduced damage for 3 seconds.
                if (targetHero.HasBuff("GragasWSelf"))
                {
                    amount *= 1
                              - new[] { 0.1d, 0.12d, 0.14d, 0.16d, 0.18d }[
                                  targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1];
                }

                // Void Stone
                // + Kassadin reduces all magic damage taken by 15%.
                if (targetHero.HasBuff("VoidStone") && damageType == DamageType.Magical)
                {
                    amount *= 0.85d;
                }

                // Shunpo
                // + Katarina teleports to target unit and gains 15% damage reduction for 1.5 seconds. If the target is an enemy, the target takes magic damage.
                if (targetHero.HasBuff("KatarinaEReduction"))
                {
                    amount *= 0.85d;
                }

                // Vengeful Maelstrom
                // + Maokai creates a magical vortex around himself, protecting him and allied champions by reducing damage from non-turret sources by 20% for a maximum of 10 seconds.
                if (targetHero.HasBuff("MaokaiDrainDefense") && !(source is Obj_AI_Turret))
                {
                    amount *= 0.8d;
                }

                // Meditate
                // + Master Yi channels for up to 4 seconds, restoring health each second. This healing is increased by 1% for every 1% of his missing health. Meditate also resets the autoattack timer.
                // + While channeling, Master Yi reduces incoming damage (halved against turrets).
                if (targetHero.HasBuff("Meditate"))
                {
                    amount *= (1
                               - new[] { 0.5d, 0.55d, 0.6d, 0.65d, 0.7d }[
                                   targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1])
                              / (source is Obj_AI_Turret ? 2 : 1);
                }

                // Valiant Fighter
                // + Poppy reduces all damage that exceeds 10% of her current health by 50%.
                if (targetHero.HasBuff("PoppyValiantFighter") && !(source is Obj_AI_Turret)
                    && amount / targetHero.Health > 0.1d)
                {
                    amount *= 0.5d;
                }

                // Shadow Dash
                // + Shen reduces all physical damage by 50% from taunted enemies.
                if (targetHero.HasBuff("Shen Shadow Dash") && source.HasBuff("Taunt")
                    && damageType == DamageType.Physical)
                {
                    amount *= 0.5d;
                }

                // Unholy Covenant
                // + Yorick grants him 5% damage reduction for each ghoul currently summoned.
                if (targetHero.HasBuff("YorickUnholySymbiosis"))
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

            // SAVAGERY:
            // + BONUS DAMAGE TO MINIONS AND MONSTERS 1/2/3/4/5 on single target spells and basic attacks
            if (hero != null && target is Obj_AI_Minion)
            {
                var savagery = hero.GetCunning(DamageMastery.Cunning.Savagery);
                if (savagery.IsValid())
                {
                    value += new[] { 1, 2, 3, 4, 5 }[savagery.Points - 1];
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
                // DoubleEdgedSword:
                // + MELEE Deal an additional 3 % damage, but receive an additional 1.5 % damage
                // + RANGED Deal an additional 2 % damage, but receive an additional 2 % damage
                if (hero.GetFerocity(DamageMastery.Ferocity.DoubleEdgedSword).IsValid())
                {
                    amount *= hero.IsMelee ? 1.03d : 1.02d;
                }

                if (targetHero != null)
                {
                    // Oppressor:
                    // + KICK 'EM WHEN THEY'RE DOWN You deal 2.5% increased damage to targets with impaired movement (slows, stuns, taunts, etc)
                    if (hero.GetFerocity(DamageMastery.Ferocity.Oppressor).IsValid() && targetHero.IsMoveImpaired())
                    {
                        amount *= 1.025;
                    }

                    // Merciless:
                    // + DAMAGE AMPLIFICATION 1 / 2 / 3 / 4 / 5 % increased damage to champions below 40 % health
                    var merciless = hero.GetCunning(DamageMastery.Cunning.Merciless);
                    if (merciless.IsValid() && targetHero.HealthPercent < 40)
                    {
                        amount *= 1 + new[] { 1, 2, 3, 4, 5 }[merciless.Points - 1] / 100;
                    }
                }
            }

            if (targetHero != null)
            {
                // DoubleEdgedSword:
                // + MELEE Deal an additional 3 % damage, but receive an additional 1.5 % damage
                // + RANGED Deal an additional 2 % damage, but receive an additional 2 % damage
                if (targetHero.GetFerocity(DamageMastery.Ferocity.DoubleEdgedSword).IsValid())
                {
                    amount *= targetHero.IsMelee ? 1.015d : 1.02d;
                }
            }

            return amount;
        }

        #endregion
    }
}