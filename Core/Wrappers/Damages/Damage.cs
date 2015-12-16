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
    using System.Collections.Generic;
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
        public static double GetAutoAttackDamage(this Obj_AI_Base source, Obj_AI_Base target)
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

            var dictOnHit = new Dictionary<string, SpellSlot>
                                {
                                    { "Evelynn", SpellSlot.E }, { "Ezreal", SpellSlot.Q }, { "Fiora", SpellSlot.Q },
                                    { "Fizz", SpellSlot.Q }, { "Gangplank", SpellSlot.Q }, { "Irelia", SpellSlot.Q },
                                    { "MissFortune", SpellSlot.Q }, { "Warwick", SpellSlot.R }, { "Yasuo", SpellSlot.Q }
                                };
            var isOnHit = dictOnHit.ContainsKey(source.ChampionName) && dictOnHit[source.ChampionName] == spellSlot;

            var dictModifier = new Dictionary<string, Tuple<SpellSlot, DamageStage>>
                                   {
                                       { "Ashe", new Tuple<SpellSlot, DamageStage>(SpellSlot.Q, DamageStage.Default) },
                                       { "Jayce", new Tuple<SpellSlot, DamageStage>(SpellSlot.W, DamageStage.SecondForm) },
                                       {
                                           "Nidalee", new Tuple<SpellSlot, DamageStage>(SpellSlot.Q, DamageStage.SecondForm)
                                       },
                                       { "Renekton", new Tuple<SpellSlot, DamageStage>(SpellSlot.W, DamageStage.Default) },
                                       { "Viktor", new Tuple<SpellSlot, DamageStage>(SpellSlot.Q, DamageStage.Empowered) }
                                   };
            var isModifier = dictModifier.ContainsKey(source.ChampionName)
                             && dictModifier[source.ChampionName].Item1 == spellSlot
                             && dictModifier[source.ChampionName].Item2 == stage;

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
                    if (isOnHit && source.HasBuff("Serrated"))
                    {
                        baseDamage += source.CalculateDamage(target, spellData.DamageType, 15);
                    }
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
                if (spellData.SpellEffectType != SpellEffectType.None && (!isOnHit || source.ChampionName == "Fizz"))
                {
                    var sorcery = source.GetFerocity(DamageMastery.Ferocity.Sorcery);
                    if (sorcery.IsValid())
                    {
                        totalDamage *= 1 + new[] { 0.4, 0.8, 1.2, 1.6, 2 }[sorcery.Points - 1] / 100;
                    }
                }
                if (isOnHit)
                {
                    passiveDamage += source.PassiveFlatMod(target);
                    passiveDamage += source.GetPassiveDamageInfo(target).Value;
                    if (target is Obj_AI_Hero && source.GetFerocity(DamageMastery.Ferocity.FervorofBattle).IsValid())
                    {
                        passiveDamage += source.CalculateDamage(
                            target,
                            DamageType.Physical,
                            (0.9 + (0.42 * source.Level)) * source.GetBuffCount("MasteryOnHitDamageStacker"));
                    }
                }
                switch (source.ChampionName)
                {
                    case "Anivia":
                        if (spellSlot == SpellSlot.E && target.HasBuff("chilled"))
                        {
                            totalDamage *= 2;
                        }
                        break;
                    case "Ashe":
                        if (spellSlot == SpellSlot.Q && Math.Abs(source.Crit - 1) < float.Epsilon)
                        {
                            totalDamage *= 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0);
                        }
                        break;
                    case "Brand":
                        if (spellSlot == SpellSlot.W && target.HasBuff("brandablaze"))
                        {
                            totalDamage *= 1.25;
                        }
                        break;
                    case "Braum":
                        if (spellSlot == SpellSlot.Q)
                        {
                            if (target.GetBuffCount("braummark") == 3)
                            {
                                passiveDamage += source.CalculateDamage(
                                    target,
                                    DamageType.Magical,
                                    32 + (8 * source.Level));
                            }
                            else if (target.HasBuff("braummarkstunreduction"))
                            {
                                passiveDamage += source.CalculateDamage(
                                    target,
                                    DamageType.Magical,
                                    6.4 + (1.6 * source.Level));
                            }
                        }
                        break;
                    case "Cassiopeia":
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W)
                            && target.HasBuff("cassiopeiatwinfangdebuff"))
                        {
                            totalDamage *= 1 + (0.2 * target.GetBuffCount("cassiopeiatwinfangdebuff"));
                        }
                        break;
                    case "Corki":
                        if (spellSlot == SpellSlot.R && source.HasBuff("corkimissilebarragecounterbig"))
                        {
                            totalDamage *= 1.5;
                        }
                        break;
                    case "Darius":
                        if (spellSlot == SpellSlot.R && stage == DamageStage.Default)
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.R, DamageStage.Buff);
                        }
                        break;
                    case "Fiddlesticks":
                        if (spellSlot == SpellSlot.E && target is Obj_AI_Minion)
                        {
                            totalDamage *= 1.5;
                        }
                        break;
                    case "Fizz":
                        if (spellSlot == SpellSlot.Q)
                        {
                            passiveDamage += source.CalculateDamage(
                                target,
                                DamageType.Physical,
                                source.TotalAttackDamage);
                        }
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W || spellSlot == SpellSlot.E)
                            && target.HasBuff("fizzrbonusbuff"))
                        {
                            totalDamage *= 1.2;
                        }
                        break;
                    case "Garen":
                        if (spellSlot == SpellSlot.E)
                        {
                            totalDamage *= source.Level < 4
                                               ? 5
                                               : source.Level < 7
                                                     ? 6
                                                     : source.Level < 10
                                                           ? 7
                                                           : source.Level < 13 ? 8 : source.Level < 16 ? 9 : 10;
                        }
                        break;
                    case "Gnar":
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.E) && stage == DamageStage.Default
                            && target.GetBuffCount("gnarwproc") == 2)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.W);
                        }
                        break;
                    case "Gragas":
                        if (spellSlot == SpellSlot.Q)
                        {
                            if (target is Obj_AI_Minion)
                            {
                                totalDamage *= 0.7;
                            }
                            if (source.HasBuff("GragasQ"))
                            {
                                totalDamage += (totalDamage * 0.25)
                                               * Math.Min(Game.Time - source.GetBuff("GragasQ").StartTime, 2);
                            }
                        }
                        break;
                    case "Hecarim":
                        if (spellSlot == SpellSlot.E && source.HasBuff("hecarimrampspeed"))
                        {
                            totalDamage =
                                Math.Min(
                                    Math.Max(
                                        totalDamage * (Game.Time - source.GetBuff("hecarimrampspeed").StartTime),
                                        totalDamage),
                                    totalDamage * 2);
                        }
                        break;
                    case "Illaoi":
                        if (spellSlot == SpellSlot.W || spellSlot == SpellSlot.E)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.Q)
                                             * GameObjects.Minions.Count(
                                                 i =>
                                                 i.CharData.BaseSkinName == "illaoiminion" && i.Team == source.Team
                                                 && i.Distance(target) < 800
                                                 && (i.IsHPBarRendered || i.HasBuff("illaoir2")));
                        }
                        break;
                    case "Janna":
                        if (spellSlot == SpellSlot.Q && source.HasBuff("HowlingGale"))
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.DamagePerSecond)
                                           * Math.Truncate(Game.Time - source.GetBuff("HowlingGale").StartTime);
                        }
                        break;
                    case "Jax":
                        if (spellSlot == SpellSlot.Q && source.HasBuff("JaxEmpowerTwo"))
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.W);
                        }
                        break;
                    case "Jayce":
                        if (spellSlot == SpellSlot.W && stage == DamageStage.SecondForm
                            && Math.Abs(source.Crit - 1) < float.Epsilon)
                        {
                            totalDamage *= 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0);
                        }
                        break;
                    case "Jinx":
                        if (spellSlot == SpellSlot.R)
                        {
                            totalDamage *= Math.Min(1 + source.Distance(target) / 15 * 0.09, 10);
                            totalDamage += source.CalculateDamage(
                                target,
                                spellData.DamageType,
                                new[] { 0.25, 0.3, 0.35 }[spellLevel - 1] * (target.MaxHealth - target.Health));
                        }
                        break;
                    case "Kalista":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
                        }
                        break;
                    case "Karma":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Default && source.HasBuff("KarmaMantra"))
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Empowered);
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Detonation);
                        }
                        break;
                    case "Kassadin":
                        if (spellSlot == SpellSlot.R && stage == DamageStage.Default)
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.R, DamageStage.Buff);
                        }
                        break;
                    case "Katarina":
                        if ((spellSlot == SpellSlot.W || spellSlot == SpellSlot.E || spellSlot == SpellSlot.R)
                            && target.HasBuff("katarinaqmark"))
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Detonation);
                        }
                        break;
                    case "Kayle":
                        if (spellSlot == SpellSlot.E && source.HasBuff("JudicatorRighteousFury"))
                        {
                            totalDamage *= 2;
                        }
                        break;
                    case "KhaZix":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Default
                            && !new List<Obj_AI_Base>().Concat(GameObjects.Minions.Where(i => i.IsMinion()))
                                    .Concat(GameObjects.Jungle)
                                    .Concat(GameObjects.Heroes)
                                    .Any(
                                        i =>
                                        i.IsValidTarget(425, false, target.ServerPosition)
                                        && i.NetworkId != target.NetworkId
                                        && (i.Team == target.Team || i.Team == GameObjectTeam.Neutral)))
                        {
                            totalDamage *= 1.3;
                            if (source.HasBuff("khazixqevo"))
                            {
                                totalDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Empowered);
                            }
                        }
                        if (spellSlot == SpellSlot.W && target is Obj_AI_Minion)
                        {
                            totalDamage *= 1.2;
                        }
                        break;
                    case "Kindred":
                        if (spellSlot == SpellSlot.W && source.GetBuffCount("kindredmarkofthekindredstackcounter") > 0)
                        {
                            totalDamage += source.CalculateDamage(
                                target,
                                DamageType.Physical,
                                Math.Min(
                                    (0.125 * target.Health) * source.GetBuffCount("kindredmarkofthekindredstackcounter"),
                                    target is Obj_AI_Minion
                                        ? 75 + (10 * source.GetBuffCount("kindredmarkofthekindredstackcounter"))
                                        : target.MaxHealth) * 0.4);
                        }
                        break;
                    case "KogMaw":
                        if (spellSlot == SpellSlot.W && !(target is Obj_AI_Minion))
                        {
                            totalDamage *= 0.55;
                        }
                        if (spellSlot == SpellSlot.R && target.HealthPercent < 50)
                        {
                            totalDamage *= target.HealthPercent < 25 ? 3 : 2;
                        }
                        break;
                    case "LeBlanc":
                        if (((spellSlot == SpellSlot.Q && stage != DamageStage.Detonation) || spellSlot == SpellSlot.W
                             || spellSlot == SpellSlot.E) && target.HasBuff("LeblancChaosOrb"))
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Detonation);
                        }
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W || spellSlot == SpellSlot.E)
                            && stage == DamageStage.Default && target.HasBuff("LeblancChaosOrbM"))
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.R, DamageStage.Detonation);
                        }
                        break;
                    case "Lucian":
                        if (spellSlot == SpellSlot.R)
                        {
                            totalDamage *= new[] { 20, 25, 30 }[spellLevel - 1];
                        }
                        break;
                    case "Lux":
                        if (spellSlot == SpellSlot.R && target.HasBuff("LuxIlluminatingFraulein"))
                        {
                            passiveDamage += source.CalculateDamage(
                                target,
                                DamageType.Magical,
                                10 + (8 * source.Level) + (0.2 * source.TotalMagicalDamage));
                        }
                        break;
                    case "Malphite":
                        if (spellSlot == SpellSlot.W)
                        {
                            totalDamage *= (Math.Abs(source.Crit - 1) < float.Epsilon
                                                ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0)
                                                : 1);
                        }
                        break;
                    case "Maokai":
                        if (spellSlot == SpellSlot.R && source.HasBuff("MaokaiDrain3"))
                        {
                            totalDamage += source.CalculateDamage(
                                target,
                                spellData.DamageType,
                                source.Spellbook.GetSpell(spellSlot).Ammo);
                        }
                        break;
                    case "MasterYi":
                        if (spellSlot == SpellSlot.Q && target is Obj_AI_Minion)
                        {
                            totalDamage += source.CalculateDamage(
                                target,
                                spellData.DamageType,
                                new[] { 75, 100, 125, 150, 175 }[spellLevel - 1]);
                        }
                        break;
                    case "Mordekaiser":
                        if (spellSlot == SpellSlot.Q && source.HasBuff("mordekaisermaceofspades2"))
                        {
                            totalDamage *= 2;
                        }
                        break;
                    case "Nami":
                        if (spellSlot == SpellSlot.W) //Todo
                        {
                            var bounceDmg = 0.85 + (Math.Abs(source.TotalMagicalDamage / 100) * 0.075);
                            if (bounceDmg > 1) {}
                        }
                        break;
                    case "Nasus":
                        if (spellSlot == SpellSlot.Q)
                        {
                            totalDamage *= (Math.Abs(source.Crit - 1) < float.Epsilon
                                                ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0)
                                                : 1);
                            totalDamage += source.GetBuffCount("nasusqstacks");
                        }
                        if (spellSlot == SpellSlot.R)
                        {
                            totalDamage = Math.Min(totalDamage, 240);
                        }
                        break;
                    case "Nidalee":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Default && source.Distance(target) > 525)
                        {
                            totalDamage *= 1 + ((Math.Min(source.Distance(target), 1300) - 525) / 7.75 * 0.02);
                        }
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.SecondForm
                            && target.HasBuff("nidaleepassivehunted"))
                        {
                            totalDamage *= 1.33;
                        }
                        break;
                    case "Nunu":
                        if ((spellSlot == SpellSlot.E || spellSlot == SpellSlot.R) && source.HasBuff("nunuqbufflizard"))
                        {
                            passiveDamage += source.CalculateDamage(target, DamageType.Magical, 0.01 * target.MaxHealth);
                        }
                        break;
                    case "Pantheon":
                        if (spellSlot == SpellSlot.Q
                            && ((target.HealthPercent < 15 && source.Spellbook.GetSpell(SpellSlot.E).Level > 0)
                                || Math.Abs(source.Crit - 1) < float.Epsilon))
                        {
                            totalDamage *= 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0);
                        }
                        if (spellSlot == SpellSlot.E && !(target is Obj_AI_Hero))
                        {
                            totalDamage /= 2;
                        }
                        break;
                    case "Poppy":
                        if (spellSlot == SpellSlot.Q && target is Obj_AI_Minion)
                        {
                            totalDamage *= 0.8;
                        }
                        break;
                    case "RekSai":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            if (source.Mana < 100)
                            {
                                totalDamage *= 1 + (0.01 * source.Mana);
                            }
                            else
                            {
                                totalDamage = source.GetSpellDamage(target, SpellSlot.E, DamageStage.Empowered);
                            }
                        }
                        break;
                    case "Renekton":
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W
                             || (spellSlot == SpellSlot.E && stage == DamageStage.SecondCast)) && source.Mana >= 50)
                        {
                            totalDamage *= 1.5;
                        }
                        break;
                    case "Riven":
                        if (spellSlot == SpellSlot.R)
                        {
                            totalDamage = Math.Min(
                                totalDamage,
                                source.CalculateDamage(
                                    target,
                                    spellData.DamageType,
                                    new[] { 240, 360, 480 }[spellLevel - 1] + (1.8 * source.FlatPhysicalDamageMod)));
                        }
                        break;
                    case "Rumble":
                        if (spellSlot == SpellSlot.Q && !(target is Obj_AI_Hero))
                        {
                            totalDamage /= 2;
                        }
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.E) && source.Mana >= 50)
                        {
                            totalDamage *= 1.5;
                        }
                        break;
                    case "Ryze":
                        if (spellSlot == SpellSlot.E && source.Distance(target) < 200)
                        {
                            totalDamage *= 1.5;
                        }
                        break;
                    case "Shaco":
                        if (spellSlot == SpellSlot.E && source.IsFacing(target) && !target.IsFacing(source))
                        {
                            totalDamage *= 1.2;
                        }
                        break;
                    case "Shyvana":
                        if (spellSlot == SpellSlot.W && target.Team == GameObjectTeam.Neutral)
                        {
                            totalDamage *= 1.2;
                        }
                        break;
                    case "Sion":
                        if (spellSlot == SpellSlot.Q)
                        {
                            if (target.Team == GameObjectTeam.Neutral)
                            {
                                totalDamage *= 1.333;
                            }
                            else if (target is Obj_AI_Hero)
                            {
                                totalDamage *= 1.667;
                            }
                            /*if (source.HasBuff("SionQ"))//Todo
                            {
                                totalDamage *= 1;
                            }*/
                        }
                        /*if (spellSlot == SpellSlot.R && source.HasBuff("SionR"))//Todo
                        {
                            totalDamage *= 1;
                        }*/
                        break;
                    case "Skarner":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Default
                            && source.HasBuff("SkarnerVirulentSlash"))
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Empowered);
                        }
                        break;
                    case "Syndra":
                        if (spellSlot == SpellSlot.Q && target is Obj_AI_Hero && spellLevel == 5)
                        {
                            totalDamage *= 1.15;
                        }
                        if (spellSlot == SpellSlot.R)
                        {
                            totalDamage *= Math.Min(source.Spellbook.GetSpell(spellSlot).Ammo, 7);
                        }
                        break;
                    case "TahmKench":
                        if ((spellSlot == SpellSlot.Q || spellSlot == SpellSlot.W)
                            && source.Spellbook.GetSpell(SpellSlot.R).Level > 0)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.R);
                        }
                        break;
                    case "Talon":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Default && target is Obj_AI_Hero)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.Q, DamageStage.Empowered);
                        }
                        break;
                    case "Teemo":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.E, DamageStage.DamagePerSecond);
                        }
                        if (spellSlot == SpellSlot.E && stage == DamageStage.DamagePerSecond && !(target is Obj_AI_Hero))
                        {
                            totalDamage /= 4;
                        }
                        break;
                    case "Thresh":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Empowered
                            && source.Buffs.Any(b => b.Name.Contains("threshqpassive")))
                        {
                            totalDamage /= source.HasBuff("threshqpassive1")
                                               ? 4
                                               : source.HasBuff("threshqpassive2")
                                                     ? 3
                                                     : source.HasBuff("threshqpassive3") ? 2 : 1;
                            totalDamage += source.CalculateDamage(
                                target,
                                DamageType.Magical,
                                source.GetBuffCount("threshpassivesouls"));
                        }
                        break;
                    case "Tristana":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            var buffDmg = source.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
                            totalDamage += target.GetBuffCount("tristanaecharge") == 3 ? buffDmg / 3 * 4 : buffDmg;
                        }
                        if ((spellSlot == SpellSlot.W || spellSlot == SpellSlot.R)
                            && target.GetBuffCount("tristanaecharge") == 3)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.E);
                        }
                        break;
                    case "Twitch":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
                        }
                        break;
                    case "Varus":
                        if (spellSlot == SpellSlot.Q || spellSlot == SpellSlot.E || spellSlot == SpellSlot.R)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.W, DamageStage.Buff);
                        }
                        /*if (spellSlot == SpellSlot.Q && source.HasBuff("VarusQ")) //Todo
                        {
                            totalDamage *= 1;
                        }*/
                        break;
                    case "Vayne":
                        if (spellSlot == SpellSlot.E && target.GetBuffCount("vaynesilvereddebuff") == 2)
                        {
                            passiveDamage += source.GetSpellDamage(target, SpellSlot.W);
                        }
                        break;
                    case "Velkoz":
                        if (spellSlot >= SpellSlot.Q && spellSlot <= SpellSlot.R
                            && target.GetBuffCount("velkozresearchstack") == 2)
                        {
                            passiveDamage += source.CalculateDamage(target, DamageType.True, 25 + (10 * source.Level));
                        }
                        break;
                    case "Vi":
                        if (spellSlot == SpellSlot.Q)
                        {
                            if (target is Obj_AI_Hero)
                            {
                                totalDamage *= 1.333;
                            }
                            if (target.GetBuffCount("viwproc") == 2)
                            {
                                passiveDamage += source.GetSpellDamage(target, SpellSlot.W);
                            }
                            /*if (source.HasBuff("ViQ"))//Todo
                            {
                                totalDamage *= 1;
                            }*/
                        }
                        break;
                    case "Viktor":
                        if (spellSlot == SpellSlot.Q && stage == DamageStage.Empowered)
                        {
                            totalDamage += source.CalculateDamage(
                                target,
                                DamageType.Magical,
                                source.TotalAttackDamage
                                * (Math.Abs(source.Crit - 1) < float.Epsilon
                                       ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.5 : 0)
                                       : 1));
                        }
                        break;
                    case "Vladimir":
                        if (spellSlot == SpellSlot.E && stage == DamageStage.Default)
                        {
                            totalDamage += source.GetSpellDamage(target, SpellSlot.E, DamageStage.Buff);
                        }
                        if (spellSlot == SpellSlot.R && target.HasBuff("vladimirhemoplaguedebuff"))
                        {
                            totalDamage *= 0.88;
                        }
                        break;
                    case "Yasuo":
                        if (spellSlot == SpellSlot.Q && Math.Abs(source.Crit - 1) < float.Epsilon)
                        {
                            totalDamage += source.CalculateDamage(
                                target,
                                spellData.DamageType,
                                (Items.HasItem((int)ItemId.Infinity_Edge, source) ? 0.875 : 0.5)
                                * source.TotalAttackDamage);
                        }
                        break;
                    case "Ziggs":
                        if (spellSlot == SpellSlot.R && target is Obj_AI_Minion && target.Team != GameObjectTeam.Neutral)
                        {
                            totalDamage *= 2;
                        }
                        break;
                }
                if (isModifier || (source.ChampionName == "TwistedFate" && spellSlot == SpellSlot.W)
                    || (isOnHit && source.ChampionName != "Fizz"))
                {
                    var targetHero = target as Obj_AI_Hero;
                    if (targetHero != null
                        && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero)))
                    {
                        totalDamage *= 0.9;
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

                // Vlad R
                if (targetHero.HasBuff("vladimirhemoplaguedebuff"))
                {
                    amount *= 1.12;
                }
            }

            return amount;
        }

        #endregion
    }
}