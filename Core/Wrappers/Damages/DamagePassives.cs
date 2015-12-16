// <copyright file="DamagePassives.cs" company="LeagueSharp">
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

    using Extensions;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Static Fields

        private static readonly IDictionary<string, List<PassiveDamage>> PassiveDamages =
            new Dictionary<string, List<PassiveDamage>>();

        #endregion

        #region Methods

        private static void AddPassiveAttack(
            string championName,
            Func<Obj_AI_Hero, Obj_AI_Base, bool> condition,
            DamageType damageType,
            Func<Obj_AI_Hero, Obj_AI_Base, double> func,
            bool @override = false,
            bool ignoreCalculation = false)
        {
            var passive = new PassiveDamage
                              {
                                  Condition = condition, Func = func, DamageType = damageType, Override = @override,
                                  IgnoreCalculation = ignoreCalculation
                              };

            List<PassiveDamage> value;
            if (PassiveDamages.TryGetValue(championName, out value))
            {
                value.Add(passive);
                return;
            }

            PassiveDamages.Add(championName, new List<PassiveDamage> { passive });
        }

        private static void CreatePassives()
        {
            AddPassiveAttack(
                string.Empty,
                (hero, @base) =>
                !new[] { "Ashe", "Corki", "Fiora", "Graves", "Jayce", "Pantheon", "Shaco", "Yasuo" }.Contains(
                    hero.ChampionName) && Math.Abs(hero.Crit - 1) < float.Epsilon,
                DamageType.Physical,
                (hero, @base) =>
                (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)) * hero.TotalAttackDamage
                * (hero.ChampionName == "Kalista" ? 0.9 : 1));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Blade_of_the_Ruined_King, hero),
                DamageType.Physical,
                (hero, @base) =>
                    {
                        var d = Math.Max(0.06 * @base.Health, 10);
                        return @base is Obj_AI_Minion ? Math.Min(d, 60) : d;
                    });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Nashors_Tooth, hero),
                DamageType.Magical,
                (hero, @base) => 15 + (0.15 * hero.TotalMagicalDamage));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Recurve_Bow, hero),
                DamageType.Physical,
                (hero, @base) => 15);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Runaans_Hurricane_Ranged_Only, hero),
                DamageType.Physical,
                (hero, @base) => 15);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem(3748, hero),
                DamageType.Physical,
                (hero, @base) =>
                hero.HasBuff("itemtitanichydracleavebuff") ? 40 + (0.1 * hero.MaxHealth) : 5 + (0.01 * hero.MaxHealth));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Wits_End, hero),
                DamageType.Magical,
                (hero, @base) => 40);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("Muramana") && hero.ManaPercent >= 3,
                DamageType.Physical,
                (hero, @base) => 0.06 * hero.Mana);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("ItemStatikShankCharge") == 100,
                DamageType.Magical,
                (hero, @base) =>
                    {
                        var d1 = Items.HasItem(2015, hero) ? 30 : 0;
                        var d2 = Items.HasItem(3087, hero)
                                     ? (30 + (hero.Level - 1) * 70 / 17) * (@base is Obj_AI_Minion ? 1.75 : 1)
                                       * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                              ? (2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0))
                                              : 1)
                                     : 0;
                        var d3 = Items.HasItem(3094, hero) ? 50 + (hero.Level - 1) * 150 / 17 : 0;
                        return Math.Max(d1, Math.Max(d2, d3));
                    });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("rageblade") == 8,
                DamageType.Magical,
                (hero, @base) => 20 + (0.15 * hero.FlatPhysicalDamageMod) + (0.075 * hero.TotalMagicalDamage));
            /*AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("Sheen"),
                DamageType.Physical,
                (hero, @base) => { return 0; });*/
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.IsMelee && hero.GetBuffCount("DreadnoughtMomentumBuff") > 0,
                DamageType.Physical,
                (hero, @base) =>
                hero.GetBuffCount("DreadnoughtMomentumBuff") / 2
                * (hero.GetBuffCount("DreadnoughtMomentumBuff") == 100 ? 2 : 1));

            var excluded = new List<string>();
            foreach (var name in GameObjects.Heroes.Select(h => h.ChampionName).Where(name => !excluded.Contains(name)))
            {
                excluded.Add(name);

                // This is O(scary), but seems quick enough in practice.
                switch (name)
                {
                    case "Aatrox":
                        AddPassiveAttack(
                            "Aatrox",
                            (hero, @base) => hero.HasBuff("aatroxwpower") && hero.HasBuff("aatroxwonhpowerbuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Akali":
                        AddPassiveAttack(
                            "Akali",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            (0.06 + (Math.Abs(hero.TotalMagicalDamage / 100) * 0.16667)) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Akali",
                            (hero, @base) => @base.HasBuff("AkaliMota"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Detonation),
                            false,
                            true);
                        break;
                    case "Alistar":
                        AddPassiveAttack(
                            "Alistar",
                            (hero, @base) => hero.HasBuff("alistartrample"),
                            DamageType.Magical,
                            (hero, @base) =>
                            (6 + hero.Level + (0.1 * hero.TotalMagicalDamage)) * (@base is Obj_AI_Minion ? 2 : 1));
                        break;
                    case "Ashe":
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => @base.HasBuff("ashepassiveslow"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.TotalAttackDamage * (0.1 + (hero.FlatCritChanceMod * (1 + hero.FlatCritDamageMod))));
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => hero.HasBuff("asheqattack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true,
                            true);
                        break;
                    case "Bard":
                        AddPassiveAttack(
                            "Bard",
                            (hero, @base) => hero.GetBuffCount("bardpspiritammocount") > 0,
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var curChime = hero.GetBuffCount("bardpdisplaychimecount");
                                    var meepDmg =
                                        new[]
                                            {
                                                30, 55, 80, 110, 140, 175, 210, 245, 280, 315, 345, 375, 400, 425, 445,
                                                465
                                            }[Math.Min(curChime / 10, 15)];
                                    return meepDmg + (curChime > 150 ? Math.Truncate((curChime - 150) / 5d) * 20 : 0)
                                           + 0.3 * hero.TotalMagicalDamage;
                                });
                        break;
                    case "Blitzcrank":
                        AddPassiveAttack(
                            "Blitzcrank",
                            (hero, @base) => hero.HasBuff("PowerFist"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Braum":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.GetBuffCount("braummark") == 3,
                            DamageType.Magical,
                            (hero, @base) => 32 + (8 * ((Obj_AI_Hero)@base.GetBuff("braummark").Caster).Level));
                        AddPassiveAttack(
                            "Braum",
                            (hero, @base) => @base.HasBuff("braummarkstunreduction"),
                            DamageType.Magical,
                            (hero, @base) => 6.4 + (1.6 * hero.Level));
                        break;
                    case "Caitlyn":
                        AddPassiveAttack(
                            "Caitlyn",
                            (hero, @base) => hero.HasBuff("caitlynheadshot"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var dmg = 0d;
                                    if (@base is Obj_AI_Minion)
                                    {
                                        dmg = (hero.TotalAttackDamage * 1.5)
                                              * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                                     ? 1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                                     : 1);
                                    }
                                    else if (@base is Obj_AI_Hero)
                                    {
                                        dmg = hero.TotalAttackDamage
                                              * (0.5 + (hero.FlatCritChanceMod * (1 + 0.5 * hero.FlatCritDamageMod)));
                                        /*if (@base.HasBuff("")) //Todo Trap Buff
                                        {
                                            dmg *= 1 + hero.Spellbook.GetSpell(SpellSlot.W).Level / 10;
                                        }*/
                                    }
                                    return dmg;
                                });
                        break;
                    case "ChoGath":
                        AddPassiveAttack(
                            "ChoGath",
                            (hero, @base) => hero.HasBuff("VorpalSpikes"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Corki":
                        AddPassiveAttack(
                            "Corki",
                            (hero, @base) => true,
                            DamageType.Mixed,
                            (hero, @base) =>
                                {
                                    var oriDmg = hero.TotalAttackDamage * 1.1;
                                    var dmg = hero.CalculateMixedDamage(@base, oriDmg / 2, oriDmg / 2)
                                              * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                                     ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                                     : 1);
                                    var targetHero = @base as Obj_AI_Hero;
                                    if (targetHero != null
                                        && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero)))
                                    {
                                        dmg *= 0.9;
                                    }
                                    return dmg;
                                },
                            true,
                            true);
                        break;
                    case "Darius":
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => true,
                            DamageType.Physical,
                            (hero, @base) =>
                            ((9 + hero.Level + (hero.FlatPhysicalDamageMod * 0.3))
                             * Math.Min(@base.GetBuffCount("dariushemo") + 1, 5)) * (@base is Obj_AI_Minion ? 0.25 : 1));
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => hero.HasBuff("DariusNoxianTacticsONH"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Diana":
                        AddPassiveAttack(
                            "Diana",
                            (hero, @base) => hero.HasBuff("dianaarcready"),
                            DamageType.Magical,
                            (hero, @base) =>
                            15
                            + ((hero.Level < 6
                                    ? 5
                                    : (hero.Level < 11 ? 10 : (hero.Level < 14 ? 15 : (hero.Level < 16 ? 20 : 25))))
                               * hero.Level) + (hero.TotalMagicalDamage * 0.8));
                        break;
                    case "DrMundo":
                        AddPassiveAttack(
                            "DrMundo",
                            (hero, @base) => hero.HasBuff("Masochism"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Draven":
                        AddPassiveAttack(
                            "Draven",
                            (hero, @base) => hero.HasBuff("DravenSpinning"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Ekko":
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => @base.GetBuffCount("ekkostacks") == 2,
                            DamageType.Magical,
                            (hero, @base) => 10 + (10 * hero.Level) + (hero.TotalMagicalDamage * 0.8));
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0 && @base.HealthPercent < 30,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => hero.HasBuff("ekkoeattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Elise":
                        AddPassiveAttack(
                            "Elise",
                            (hero, @base) => hero.HasBuff("EliseR"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R, DamageStage.SecondForm),
                            false,
                            true);
                        break;
                    case "Fiora":
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) =>
                            Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("FioraE")
                            && !hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) => hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                             + new[] { -0.6, -0.45, -0.3, -0.15, 0 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Fizz":
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) * 0.5 / 3,
                            false,
                            true);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("FizzSeastonePassive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            false,
                            true);
                        break;
                    case "Gangplank":
                        AddPassiveAttack(
                            "Gangplank",
                            (hero, @base) => hero.HasBuff("gangplankpassiveattack"),
                            DamageType.True,
                            (hero, @base) =>
                            ((10 + (5 * hero.Level)) + (hero.FlatPhysicalDamageMod * 0.6))
                            * (@base is Obj_AI_Turret ? 1 : 2));
                        break;
                    case "Garen":
                        AddPassiveAttack(
                            "Garen",
                            (hero, @base) => hero.HasBuff("GarenQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Gnar":
                        AddPassiveAttack(
                            "Gnar",
                            (hero, @base) => @base.GetBuffCount("gnarwproc") == 2,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Gragas":
                        AddPassiveAttack(
                            "Gragas",
                            (hero, @base) => hero.HasBuff("gragaswattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    /*case "Graves":
                        AddPassiveAttack(
                            "Graves",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var mainDmg =
                                        new[]
                                            {
                                               .75,.76,.78,.79,.81,.83,.84,.86,.88,.9,.92,.95,
                                               .97,.99, 1.02, 1.04, 1.07, 1.1
                                            }[hero.Level - 1];
                                    var bonusDmg = mainDmg / 3;
                                    return (mainDmg * hero.TotalAttackDamage + bonusDmg)
                                           * (@base is Obj_AI_Turret ? 0.25 : 1);
                                },
                            true);
                        break;*/
                    case "Hecarim":
                        AddPassiveAttack(
                            "Hecarim",
                            (hero, @base) => hero.HasBuff("hecarimrampspeed"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Illaoi":
                        AddPassiveAttack(
                            "Illaoi",
                            (hero, @base) => hero.HasBuff("IllaoiW"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Irelia":
                        AddPassiveAttack(
                            "Irelia",
                            (hero, @base) => hero.HasBuff("ireliahitenstylecharged"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "JarvanIV":
                        AddPassiveAttack(
                            "JarvanIV",
                            (hero, @base) => !@base.HasBuff("jarvanivmartialcadencecheck"),
                            DamageType.Physical,
                            (hero, @base) => Math.Min(@base.Health * 0.1, 400));
                        break;
                    case "Jax":
                        AddPassiveAttack(
                            "Jax",
                            (hero, @base) => hero.HasBuff("JaxEmpowerTwo"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Jayce":
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) =>
                            Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.SecondForm),
                            true,
                            true);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycepassivemeleeattack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
                            false,
                            true);
                        break;
                    case "Jinx":
                        AddPassiveAttack(
                            "Jinx",
                            (hero, @base) => hero.HasBuff("JinxQ"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q)
                            * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                   ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                   : 1),
                            false,
                            true);
                        break;
                    case "Kalista":
                        AddPassiveAttack(
                            "Kalista",
                            (hero, @base) => @base.HasBuff("kalistacoopstrikemarkally"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) =>
                            @base.HasBuff("kalistacoopstrikemarkbuff") && hero.HasBuff("kalistacoopstrikeally"),
                            DamageType.Magical,
                            (hero, @base) =>
                            ((Obj_AI_Hero)@base.GetBuff("kalistacoopstrikemarkbuff").Caster).GetSpellDamage(
                                @base,
                                SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Kassadin":
                        AddPassiveAttack(
                            "Kassadin",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(
                                @base,
                                SpellSlot.W,
                                hero.HasBuff("NetherBlade") ? DamageStage.Empowered : DamageStage.Default),
                            false,
                            true);
                        break;
                    case "Katarina":
                        AddPassiveAttack(
                            "Katarina",
                            (hero, @base) => @base.HasBuff("katarinaqmark"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Detonation),
                            false,
                            true);
                        break;
                    case "Kayle":
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.E).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Kennen":
                        AddPassiveAttack(
                            "Kennen",
                            (hero, @base) => hero.HasBuff("kennendoublestrikelive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "KhaZix":
                        AddPassiveAttack(
                            "KhaZix",
                            (hero, @base) => hero.HasBuff("khazixpdamage") && @base is Obj_AI_Hero,
                            DamageType.Magical,
                            (hero, @base) =>
                            10
                            + ((hero.Level < 6 ? 5 : (hero.Level < 11 ? 10 : (hero.Level < 14 ? 15 : 20))) * hero.Level)
                            + (0.5 * hero.TotalMagicalDamage));
                        break;
                    case "Kindred":
                        AddPassiveAttack(
                            "Kindred",
                            (hero, @base) => hero.GetBuffCount("kindredmarkofthekindredstackcounter") > 0,
                            DamageType.Physical,
                            (hero, @base) =>
                            Math.Min(
                                (0.125 * @base.Health) * hero.GetBuffCount("kindredmarkofthekindredstackcounter"),
                                @base is Obj_AI_Minion
                                    ? 75 + (10 * hero.GetBuffCount("kindredmarkofthekindredstackcounter"))
                                    : @base.MaxHealth));
                        break;
                    case "KogMaw":
                        AddPassiveAttack(
                            "KogMaw",
                            (hero, @base) => hero.HasBuff("KogMawBioArcaneBarrage"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Leona":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) =>
                            @base.HasBuff("leonasunlight")
                            && @base.GetBuff("leonasunlight").Caster.NetworkId != hero.NetworkId,
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var lvl = ((Obj_AI_Hero)@base.GetBuff("leonasunlight").Caster).Level - 1;
                                    if ((lvl / 2) % 1 > 0)
                                    {
                                        lvl -= 1;
                                    }
                                    return 20 + (15 * lvl / 2);
                                });
                        AddPassiveAttack(
                            "Leona",
                            (hero, @base) => hero.HasBuff("LeonaShieldOfDaybreak"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Lucian":
                        AddPassiveAttack(
                            "Lucian",
                            (hero, @base) => hero.HasBuff("lucianpassivebuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (@base is Obj_AI_Minion
                                 ? 1
                                 : (hero.Level < 6 ? 0.3 : (hero.Level < 11 ? 0.4 : (hero.Level < 16 ? 0.5 : 0.6))))
                            * hero.TotalAttackDamage
                            * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                   ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                   : 1));
                        break;
                    case "Lux":
                        AddPassiveAttack(
                            "Lux",
                            (hero, @base) => hero.HasBuff("LuxIlluminatingFraulein"),
                            DamageType.Magical,
                            (hero, @base) => 10 + (8 * hero.Level) + (0.2 * hero.TotalMagicalDamage));
                        break;
                    case "Malphite":
                        AddPassiveAttack(
                            "Malphite",
                            (hero, @base) => hero.HasBuff("malphitecleave"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "MasterYi":
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("doublestrike"),
                            DamageType.Physical,
                            (hero, @base) =>
                            0.5 * hero.TotalAttackDamage
                            * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                   ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                   : 1));
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("wujustylesuperchargedvisual"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    /*case "MissFortune":
                        AddPassiveAttack(
                            "MissFortune",
                            (hero, @base) => !@base.HasBuff(""),
                            DamageType.Physical,
                            (hero, @base) =>
                            (0.6 + (hero.Level - 1) * 0.4 / 17) * hero.TotalAttackDamage
                            / (@base is Obj_AI_Hero ? 1 : 2));
                        break;*/
                    case "Mordekaiser":
                        AddPassiveAttack(
                            "Mordekaiser",
                            (hero, @base) => hero.Buffs.Any(b => b.Name.Contains("mordekaisermaceofspades")),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Nami":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("NamiE"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var caster = (Obj_AI_Hero)hero.GetBuff("NamiE").Caster;
                                    return
                                        new[] { 25, 40, 55, 70, 85 }[caster.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                                        + (0.2 * caster.TotalMagicalDamage);
                                });
                        break;
                    case "Nasus":
                        AddPassiveAttack(
                            "Nasus",
                            (hero, @base) => hero.HasBuff("NasusQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Nautilus":
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => !@base.HasBuff("nautiluspassivecheck"),
                            DamageType.Physical,
                            (hero, @base) => 2 + (6 * hero.Level));
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => hero.HasBuff("nautiluspiercinggazeshield"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) / (@base is Obj_AI_Hero ? 1 : 2),
                            false,
                            true);
                        break;
                    case "Nidalee":
                        AddPassiveAttack(
                            "Nidalee",
                            (hero, @base) => hero.HasBuff("Takedown"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.SecondForm),
                            true,
                            true);
                        break;
                    case "Nocturne":
                        AddPassiveAttack(
                            "Nocturne",
                            (hero, @base) => hero.HasBuff("nocturneumbrablades"),
                            DamageType.Physical,
                            (hero, @base) => 0.2 * hero.TotalAttackDamage);
                        break;
                    case "Nunu":
                        AddPassiveAttack(
                            "Nunu",
                            (hero, @base) => hero.HasBuff("nunuqbufflizard"),
                            DamageType.Magical,
                            (hero, @base) => 0.01 * hero.MaxHealth);
                        break;
                    case "Orianna":
                        AddPassiveAttack(
                            "Orianna",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var d = (hero.Level < 4
                                                 ? 10
                                                 : (hero.Level < 7
                                                        ? 18
                                                        : (hero.Level < 10
                                                               ? 26
                                                               : (hero.Level < 13 ? 34 : (hero.Level < 16 ? 42 : 50)))))
                                            + (0.15 * hero.TotalMagicalDamage);
                                    return d;
                                });
                        break;
                    case "Pantheon":
                        AddPassiveAttack(
                            "Pantheon",
                            (hero, @base) =>
                            (@base.HealthPercent < 15 && hero.Spellbook.GetSpell(SpellSlot.E).Level > 0)
                            || Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)) * hero.TotalAttackDamage);
                        break;
                    case "Poppy":
                        AddPassiveAttack(
                            "Poppy",
                            (hero, @base) => hero.HasBuff("PoppyPassiveBuff"),
                            DamageType.Magical,
                            (hero, @base) => 10 + (10 * hero.Level));
                        break;
                    case "Quinn":
                        AddPassiveAttack(
                            "Quinn",
                            (hero, @base) => @base.HasBuff("quinnw"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var dmg = hero.CalculateDamage(
                                        @base,
                                        DamageType.Physical,
                                        10 + (5 * hero.Level) + (1.14 + (0.02 * hero.Level)) * hero.TotalAttackDamage);
                                    var targetHero = @base as Obj_AI_Hero;
                                    if (targetHero != null
                                        && new[] { 3047, 1316, 1318, 1315, 1317 }.Any(i => Items.HasItem(i, targetHero)))
                                    {
                                        dmg *= 0.9;
                                    }
                                    return dmg;
                                },
                            true,
                            true);
                        break;
                    case "RekSai":
                        AddPassiveAttack(
                            "RekSai",
                            (hero, @base) => hero.HasBuff("RekSaiQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Renekton":
                        AddPassiveAttack(
                            "Renekton",
                            (hero, @base) => hero.HasBuff("RenektonPreExecute"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true,
                            true);
                        break;
                    case "Rengar":
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqbase"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqemp"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered),
                            false,
                            true);
                        break;
                    case "Riven":
                        AddPassiveAttack(
                            "Riven",
                            (hero, @base) => hero.HasBuff("rivenpassiveaaboost"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (hero.Level < 3
                                 ? 0.2
                                 : (hero.Level < 6
                                        ? 0.25
                                        : (hero.Level < 9
                                               ? 0.3
                                               : (hero.Level < 12
                                                      ? 0.35
                                                      : (hero.Level < 15 ? 0.4 : (hero.Level < 18 ? 0.45 : 0.5))))))
                            * hero.TotalAttackDamage);
                        break;
                    case "Rumble":
                        AddPassiveAttack(
                            "Rumble",
                            (hero, @base) => hero.HasBuff("rumbleoverheat"),
                            DamageType.Magical,
                            (hero, @base) => 20 + (5 * hero.Level) + (0.3 * hero.TotalMagicalDamage));
                        break;
                    case "Sejuani":
                        AddPassiveAttack(
                            "Sejuani",
                            (hero, @base) => hero.HasBuff("sejuaninorthernwindsenrage"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Shaco":
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.IsFacing(@base) && !@base.IsFacing(hero),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.TotalAttackDamage * 0.2
                            * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                   ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                   : 1));
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                             + new[] { -0.6, -0.4, -0.2, 0, 0.2 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Shen":
                        AddPassiveAttack(
                            "Shen",
                            (hero, @base) => hero.HasBuff("shenwayoftheninjaaura"),
                            DamageType.Magical,
                            (hero, @base) => 4 + (4 * hero.Level) + (hero.BonusHealth * 0.1));
                        break;
                    case "Shyvana":
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => hero.HasBuff("ShyvanaDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) =>
                            hero.HasBuff("ShyvanaImmolationAura") || hero.HasBuff("shyvanaimmolatedragon"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) * 0.25,
                            false,
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => @base.HasBuff("ShyvanaFireballMissile"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Detonation),
                            false,
                            true);
                        break;
                    case "Sion":
                        AddPassiveAttack(
                            "Sion",
                            (hero, @base) => hero.HasBuff("sionpassivezombie"),
                            DamageType.Physical,
                            (hero, @base) =>
                            Math.Min(0.1 * @base.MaxHealth, @base is Obj_AI_Minion ? 75 : @base.MaxHealth));
                        break;
                    case "Skarner":
                        AddPassiveAttack(
                            "Skarner",
                            (hero, @base) => @base.HasBuff("skarnerpassivebuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Detonation),
                            false,
                            true);
                        break;
                    case "Sona":
                        AddPassiveAttack(
                            "Sona",
                            (hero, @base) => hero.HasBuff("sonapassiveattack"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var level = hero.Level;
                                    return (6
                                            + ((level < 4
                                                    ? 7
                                                    : (level < 6 ? 8 : (level < 7 ? 9 : (level < 15 ? 10 : 15))))
                                               * level)) + (0.2 * hero.TotalMagicalDamage);
                                });
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("SonaQProcAttacker"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var caster = (Obj_AI_Hero)hero.GetBuff("SonaQProcAttacker").Caster;
                                    return
                                        new[] { 20, 30, 40, 50, 60 }[caster.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                                        + (0.2 * caster.TotalMagicalDamage)
                                        + new[] { 0, 10, 20, 30 }[caster.Spellbook.GetSpell(SpellSlot.R).Level];
                                });
                        break;
                    case "TahmKench":
                        AddPassiveAttack(
                            "TahmKench",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.R).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
                            false,
                            true);
                        break;
                    case "Talon":
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) =>
                            @base.HasBuffOfType(BuffType.Slow) || @base.HasBuffOfType(BuffType.Stun)
                            || @base.HasBuffOfType(BuffType.Snare) || @base.HasBuffOfType(BuffType.Suppression),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.TotalAttackDamage * 0.1
                            * (Math.Abs(hero.Crit - 1) < float.Epsilon
                                   ? 2 + (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.5 : 0)
                                   : 1));
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) => hero.HasBuff("talonnoxiandiplomacybuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Taric":
                        AddPassiveAttack(
                            "Taric",
                            (hero, @base) => hero.HasBuff("taricgemcraftbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.Armor * 0.2);
                        break;
                    case "Teemo":
                        AddPassiveAttack(
                            "Teemo",
                            (hero, @base) => hero.HasBuff("ToxicShot"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Thresh":
                        AddPassiveAttack(
                            "Thresh",
                            (hero, @base) => hero.Buffs.Any(b => b.Name.Contains("threshqpassive")),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Empowered),
                            false,
                            true);
                        break;
                    case "Tristana":
                        AddPassiveAttack(
                            "Tristana",
                            (hero, @base) => @base.GetBuffCount("tristanaecharge") == 3,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Trundle":
                        AddPassiveAttack(
                            "Trundle",
                            (hero, @base) => hero.HasBuff("TrundleTrollSmash"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "TwistedFate":
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("cardmasterstackparticle"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("bluecardpreattack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("redcardpreattack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Detonation),
                            true,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("goldcardpreattack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            true,
                            true);
                        break;
                    case "Twitch":
                        AddPassiveAttack(
                            "Twitch",
                            (hero, @base) => true,
                            DamageType.True,
                            (hero, @base) =>
                            (hero.Level < 5
                                 ? 12
                                 : (hero.Level < 9 ? 18 : (hero.Level < 13 ? 24 : (hero.Level < 17 ? 30 : 36))))
                            * Math.Min(@base.GetBuffCount("twitchdeadlyvenom") + 1, 6) / (@base is Obj_AI_Hero ? 1 : 6d));
                        break;
                    case "Udyr":
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("UdyrTigerStance"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("udyrtigerpunch"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.DamagePerSecond)
                            / (@base is Obj_AI_Hero ? 1 : 2),
                            false,
                            true);
                        break;
                    case "Varus":
                        AddPassiveAttack(
                            "Varus",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Vayne":
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => hero.HasBuff("vaynetumblebonus"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => @base.GetBuffCount("vaynesilvereddebuff") == 2,
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Vi":
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => @base.GetBuffCount("viwproc") == 2,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => hero.HasBuff("ViE"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "Viktor":
                        AddPassiveAttack(
                            "Viktor",
                            (hero, @base) => hero.HasBuff("viktorpowertransferreturn"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered),
                            true,
                            true);
                        break;
                    case "Volibear":
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("VolibearQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("volibearrapplicator"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
                            false,
                            true);
                        break;
                    case "Warwick":
                        AddPassiveAttack(
                            "Warwick",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => 2.5 + (hero.Level < 10 ? 0.5 : 1) * hero.Level);
                        break;
                    case "MonkeyKing":
                        AddPassiveAttack(
                            "MonkeyKing",
                            (hero, @base) => hero.HasBuff("MonkeyKingDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "XinZhao":
                        AddPassiveAttack(
                            "XinZhao",
                            (hero, @base) => hero.HasBuff("XenZhaoComboTarget"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Yasuo":
                        AddPassiveAttack(
                            "Yasuo",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) =>
                            (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 1.25 : 0.8) * hero.TotalAttackDamage);
                        break;
                    case "Yorick":
                        AddPassiveAttack(
                            "Yorick",
                            (hero, @base) => hero.HasBuff("YorickUnholySymbiosis"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (0.05
                             * GameObjects.Minions.Count(
                                 g =>
                                 g.Team == hero.Team
                                 && (g.Name.Equals("Clyde") || g.Name.Equals("Inky") || g.Name.Equals("Blinky")
                                     || (g.HasBuff("yorickunholysymbiosis")
                                         && g.GetBuff("yorickunholysymbiosis").Caster.NetworkId == hero.NetworkId))))
                            * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Yorick",
                            (hero, @base) => hero.HasBuff("YorickSpectral"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Zed":
                        AddPassiveAttack(
                            "Zed",
                            (hero, @base) => @base.HealthPercent < 50 && !@base.HasBuff("ZedPassiveCD"),
                            DamageType.Magical,
                            (hero, @base) => (hero.Level < 7 ? 0.06 : (hero.Level < 17 ? 0.08 : 0.1)) * @base.MaxHealth);
                        break;
                    case "Ziggs":
                        AddPassiveAttack(
                            "Ziggs",
                            (hero, @base) => hero.HasBuff("ziggsshortfuse"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var level = hero.Level;
                                    var amount = (16 + (level < 7 ? 4 : (level < 13 ? 8 : 12)) * level)
                                                 + (hero.TotalMagicalDamage
                                                    * (level < 7 ? 0.25 : (level < 13 ? 0.3 : 0.35)));
                                    return @base is Obj_AI_Turret ? amount * 2 : amount;
                                });
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets the passive raw damage summary.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static PassiveDamageInfo GetPassiveDamageInfo(this Obj_AI_Hero source, Obj_AI_Base target)
        {
            var @double = 0d;
            var @override = false;
            List<PassiveDamage> value;

            // If you don't understand the code below, you should be flipping burgers instead.
            if (PassiveDamages.TryGetValue(string.Empty, out value))
            {
                // This should fix something that should never happen.
                @double +=
                    value.Where(i => (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null)
                        .Sum(
                            item =>
                            item.IgnoreCalculation
                                ? item.Func(source, target)
                                : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));

                @override = value.Any(i => i.Override);
            }

            if (PassiveDamages.TryGetValue(source.ChampionName, out value))
            {
                @double +=
                    value.Where(i => (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null)
                        .Sum(
                            item =>
                            item.IgnoreCalculation
                                ? item.Func(source, target)
                                : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));

                if (!@override)
                {
                    @override = value.Any(i => i.Override);
                }
            }

            return new PassiveDamageInfo { Value = @double, Override = @override };
        }

        #endregion

        private struct PassiveDamage
        {
            #region Public Properties

            public Func<Obj_AI_Hero, Obj_AI_Base, bool> Condition { get; set; }

            public DamageType DamageType { get; set; }

            public Func<Obj_AI_Hero, Obj_AI_Base, double> Func { get; set; }

            public bool IgnoreCalculation { get; set; }

            public bool Override { get; set; }

            #endregion
        }

        private struct PassiveDamageInfo
        {
            #region Public Properties

            public bool Override { get; set; }

            public double Value { get; set; }

            #endregion
        }
    }
}