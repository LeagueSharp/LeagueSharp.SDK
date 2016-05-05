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

namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Data.Enumerations;

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
            bool ignoreCalculation = false,
            bool @override = false)
        {
            var passive = new PassiveDamage
                              {
                                  Condition = condition, Func = func, DamageType = damageType, Override = @override,
                                  IgnoreCalculation = ignoreCalculation
                              };

            if (PassiveDamages.ContainsKey(championName))
            {
                PassiveDamages[championName].Add(passive);
                return;
            }

            PassiveDamages.Add(championName, new List<PassiveDamage> { passive });
        }

        private static void CreatePassives()
        {
            AddPassiveAttack(
                string.Empty,
                (hero, @base) =>
                !new[] { "Ashe", "Corki", "Fiora", "Graves", "Jayce", "Jhin", "Pantheon", "Shaco", "Yasuo" }.Contains(
                    hero.ChampionName) && Math.Abs(hero.Crit - 1) < float.Epsilon,
                DamageType.Physical,
                (hero, @base) =>
                (hero.TotalAttackDamage * (hero.ChampionName == "Kalista" ? 0.9 : 1)) * hero.GetCritMultiplier());
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
                (hero, @base) => hero.HasBuff("Muramana") && hero.ManaPercent >= 20 && @base is Obj_AI_Hero,
                DamageType.Physical,
                (hero, @base) => 0.06 * hero.Mana);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("ItemStatikShankCharge") >= 88,
                DamageType.Magical,
                (hero, @base) =>
                    {
                        var d1 = Items.HasItem(2015, hero) ? 40 : 0;
                        var d2 = Items.HasItem(3087, hero)
                                     ? new[]
                                           {
                                               50, 50, 50, 50, 50, 56, 61, 67, 72, 77, 83, 88, 94, 99, 104, 110, 115, 120
                                           }[hero.Level - 1] * (@base is Obj_AI_Minion ? 2.2 : 1)
                                       * hero.GetCritMultiplier(true)
                                     : 0;
                        var d3 = Items.HasItem(3094, hero)
                                     ? new[]
                                           {
                                               50, 50, 50, 50, 50, 58, 66, 75, 83, 92, 100, 109, 117, 126, 134, 143, 151,
                                               160
                                           }[hero.Level - 1]
                                     : 0;
                        return Math.Max(d1, Math.Max(d2, d3));
                    });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem(3124, hero),
                DamageType.Magical,
                (hero, @base) => 15);
            /*AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("Sheen"),
                DamageType.Physical,
                (hero, @base) => { return 0; });*/
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.IsMelee() && Items.HasItem(3742, hero),
                DamageType.Physical,
                (hero, @base) =>
                    {
                        var count = hero.GetBuffCount("DreadnoughtMomentumBuff");
                        return count > 0 ? Math.Floor(count / 2d) * (count == 100 ? 2 : 1) : 0;
                    });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("s5test_dragonslayerbuff") == 5,
                DamageType.True,
                (hero, @base) => 150 / (@base is Obj_AI_Minion ? 5d : 1));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("Mastery6261") && @base is Obj_AI_Hero,
                DamageType.Magical,
                (hero, @base) => (hero.IsMelee() ? 0.025 : 0.0125) * hero.MaxHealth);

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
                            hero.TotalAttackDamage * (0.1 + (hero.Crit * (1 + hero.CritDamageMultiplier))));
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => hero.HasBuff("asheqattack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q) * hero.GetCritMultiplier(true),
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
                                        dmg = (hero.TotalAttackDamage * 1.5) * hero.GetCritMultiplier(true);
                                    }
                                    else if (@base is Obj_AI_Hero)
                                    {
                                        dmg = hero.TotalAttackDamage
                                              * (0.5 + (hero.Crit * (1 + 0.5 * hero.CritDamageMultiplier)));
                                        if (@base.HasBuff("caitlynyordletrapsight"))
                                        {
                                            dmg +=
                                                new[] { 30, 70, 110, 150, 190 }[
                                                    hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                                + (hero.TotalAttackDamage * 0.7);
                                        }
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
                            true);
                        break;
                    case "Corki":
                        AddPassiveAttack(
                            "Corki",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Mixed,
                            (hero, @base) => hero.TotalAttackDamage * hero.GetCritMultiplier());
                        break;
                    case "Darius":
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => @base is Obj_AI_Hero,
                            DamageType.Physical,
                            (hero, @base) =>
                            ((9 + hero.Level + (hero.FlatPhysicalDamageMod * 0.3))
                             * Math.Min(Math.Max(0, @base.GetBuffCount("dariushemo")) + 1, 5)));
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => hero.HasBuff("DariusNoxianTacticsONH"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
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
                            (hero, @base) => hero.HasBuff("Masochism") && hero.AttackRange >= 150,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Draven":
                        AddPassiveAttack(
                            "Draven",
                            (hero, @base) => hero.HasBuff("DravenSpinning"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
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
                            true);
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => hero.HasBuff("ekkoeattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Elise":
                        AddPassiveAttack(
                            "Elise",
                            (hero, @base) => hero.HasBuff("EliseR"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R, DamageStage.SecondForm),
                            true);
                        break;
                    case "Fiora":
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) =>
                            Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("FioraE")
                            && !hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) => hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (hero.GetCritMultiplier()
                             + new[] { -0.6, -0.45, -0.3, -0.15, 0 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Fizz":
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0 && @base is Obj_AI_Hero,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) * 0.5 / 3,
                            true);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("FizzSeastonePassive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            true);
                        break;
                    case "Gangplank":
                        AddPassiveAttack(
                            "Gangplank",
                            (hero, @base) => hero.HasBuff("gangplankpassiveattack") && @base is Obj_AI_Hero,
                            DamageType.True,
                            (hero, @base) => 20 + (10 * hero.Level) + hero.FlatPhysicalDamageMod);
                        break;
                    case "Garen":
                        AddPassiveAttack(
                            "Garen",
                            (hero, @base) => hero.HasBuff("GarenQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Gnar":
                        AddPassiveAttack(
                            "Gnar",
                            (hero, @base) => @base.GetBuffCount("gnarwproc") == 2,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Gragas":
                        AddPassiveAttack(
                            "Gragas",
                            (hero, @base) => hero.HasBuff("gragaswattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
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
                            true);
                        break;
                    case "Illaoi":
                        AddPassiveAttack(
                            "Illaoi",
                            (hero, @base) => hero.HasBuff("IllaoiW"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W)
                            + hero.GetSpellDamage(@base, SpellSlot.Q)
                            * GameObjects.Minions.Count(
                                i =>
                                i.CharData.BaseSkinName == "illaoiminion" && i.Team == hero.Team
                                && i.Distance(@base) < 800 && (i.IsHPBarRendered || i.HasBuff("illaoir2"))),
                            true);
                        break;
                    case "Irelia":
                        AddPassiveAttack(
                            "Irelia",
                            (hero, @base) => hero.HasBuff("ireliahitenstylecharged"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
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
                            true);
                        break;
                    case "Jayce":
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) =>
                            Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.SecondForm)
                            * hero.GetCritMultiplier(true),
                            true,
                            true);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycepassivemeleeattack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
                            true);
                        break;
                    case "Jhin":
                        AddPassiveAttack(
                            "Jhin",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) =>
                            (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 0.875 : 0.5)
                            * (hero.TotalAttackDamage
                               + Math.Round(
                                   (new[] { 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 40 }[
                                       hero.Level - 1] + (Math.Round((hero.Crit * 100 / 10) * 4))
                                    + (Math.Round(((hero.AttackSpeedMod - 1) * 100 / 10) * 2.5))) / 100
                                   * hero.TotalAttackDamage)));
                        AddPassiveAttack(
                            "Jhin",
                            (hero, @base) => hero.HasBuff("JhinPassiveAttackBuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 1.875 : 1.5)
                            * ((hero.Level < 6 ? 0.15 : (hero.Level < 11 ? 0.2 : 0.25))
                               * (@base.MaxHealth - @base.Health)));
                        break;
                    case "Jinx":
                        AddPassiveAttack(
                            "Jinx",
                            (hero, @base) => hero.HasBuff("JinxQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q) * hero.GetCritMultiplier(true),
                            true);
                        break;
                    case "Kalista":
                        AddPassiveAttack(
                            "Kalista",
                            (hero, @base) => @base.HasBuff("kalistacoopstrikemarkally"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
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
                            true);
                        break;
                    case "Katarina":
                        AddPassiveAttack(
                            "Katarina",
                            (hero, @base) => @base.HasBuff("katarinaqmark"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Detonation),
                            true);
                        break;
                    case "Kayle":
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.E).Level > 0,
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E) * (hero.HasBuff("JudicatorRighteousFury") ? 2 : 1),
                            true);
                        break;
                    case "Kennen":
                        AddPassiveAttack(
                            "Kennen",
                            (hero, @base) => hero.HasBuff("kennendoublestrikelive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
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
                            (hero, @base) => hero.HasBuff("KindredLegendPassive"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var count = hero.GetBuffCount("kindredmarkofthekindredstackcounter");
                                    return count > 0
                                               ? Math.Min(
                                                   (0.125 * count) * @base.Health,
                                                   @base is Obj_AI_Minion ? 75 + (10 * count) : @base.MaxHealth)
                                               : 0;
                                });
                        break;
                    case "KogMaw":
                        AddPassiveAttack(
                            "KogMaw",
                            (hero, @base) => hero.HasBuff("KogMawBioArcaneBarrage"),
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W) * (@base is Obj_AI_Minion ? 1 : 0.55),
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
                            true);
                        break;
                    case "Lucian":
                        AddPassiveAttack(
                            "Lucian",
                            (hero, @base) => hero.HasBuff("lucianpassivebuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            ((@base is Obj_AI_Minion
                                  ? 1
                                  : (hero.Level < 6 ? 0.3 : (hero.Level < 11 ? 0.4 : (hero.Level < 16 ? 0.5 : 0.6))))
                             * hero.TotalAttackDamage) * hero.GetCritMultiplier(true));
                        break;
                    case "Lux":
                        AddPassiveAttack(
                            "Lux",
                            (hero, @base) => @base.HasBuff("LuxIlluminatingFraulein"),
                            DamageType.Magical,
                            (hero, @base) => 10 + (8 * hero.Level) + (0.2 * hero.TotalMagicalDamage));
                        break;
                    case "Malphite":
                        AddPassiveAttack(
                            "Malphite",
                            (hero, @base) => hero.HasBuff("malphitecleave"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) * hero.GetCritMultiplier(true),
                            true);
                        break;
                    case "MasterYi":
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("doublestrike"),
                            DamageType.Physical,
                            (hero, @base) => (0.5 * hero.TotalAttackDamage) * hero.GetCritMultiplier(true));
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("wujustylesuperchargedvisual"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q) * (hero.HasBuff("mordekaisermaceofspades2") ? 2 : 1),
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q) * hero.GetCritMultiplier(true)
                            + Math.Max(0, hero.GetBuffCount("nasusqstacks")),
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
                            (hero, @base) => hero.HasBuff("nautiluspiercinggazeshield") && @base is Obj_AI_Hero,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Nidalee":
                        AddPassiveAttack(
                            "Nidalee",
                            (hero, @base) => hero.HasBuff("Takedown"),
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.SecondForm)
                            * (@base.HasBuff("nidaleepassivehunted") ? 1.33 : 1),
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
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Poppy":
                        AddPassiveAttack(
                            "Poppy",
                            (hero, @base) => hero.HasBuff("PoppyPassiveBuff"),
                            DamageType.Magical,
                            (hero, @base) => 10 + (10 * hero.Level));
                        break;
                    case "RekSai":
                        AddPassiveAttack(
                            "RekSai",
                            (hero, @base) => hero.HasBuff("RekSaiQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Renekton":
                        AddPassiveAttack(
                            "Renekton",
                            (hero, @base) => hero.HasBuff("RenektonPreExecute"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (hero.GetSpellDamage(@base, SpellSlot.W) * (hero.Mana >= 50 ? 1.5 : 1))
                            * hero.GetCritMultiplier(true),
                            true,
                            true);
                        break;
                    case "Rengar":
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqbase"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqemp"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered),
                            true);
                        break;
                    case "Riven":
                        AddPassiveAttack(
                            "Riven",
                            (hero, @base) => hero.HasBuff("rivenpassiveaaboost"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (hero.Level < 3
                                 ? 0.25
                                 : (hero.Level < 6
                                        ? 0.29167
                                        : (hero.Level < 9
                                               ? 0.3333
                                               : (hero.Level < 12
                                                      ? 0.375
                                                      : (hero.Level < 15 ? 0.4167 : (hero.Level < 18 ? 0.4583 : 0.5))))))
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
                            true);
                        break;
                    case "Shaco":
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.IsFacing(@base) && !@base.IsFacing(hero),
                            DamageType.Physical,
                            (hero, @base) => (hero.TotalAttackDamage * 0.2) * hero.GetCritMultiplier(true));
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (hero.GetCritMultiplier()
                             + new[] { -0.6, -0.4, -0.2, 0, 0.2 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Shen":
                        AddPassiveAttack(
                            "Shen",
                            (hero, @base) => hero.HasBuff("shenqbuff"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var dmg = hero.GetSpellDamage(@base, SpellSlot.Q);
                                    if (@base is Obj_AI_Hero && @base.HasBuff("ShenQSlow"))
                                    {
                                        dmg += hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered);
                                    }
                                    return dmg;
                                },
                            true);
                        break;
                    case "Shyvana":
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => hero.HasBuff("ShyvanaDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) =>
                            hero.HasBuff("ShyvanaImmolationAura") || hero.HasBuff("shyvanaimmolatedragon"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) * 0.25,
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => @base.HasBuff("ShyvanaFireballMissile"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Detonation),
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
                            true);
                        break;
                    case "Sona":
                        AddPassiveAttack(
                            "Sona",
                            (hero, @base) => hero.HasBuff("SonaPassiveReady"),
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
                            true);
                        break;
                    case "Talon":
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) =>
                            @base.HasBuffOfType(BuffType.Slow) || @base.HasBuffOfType(BuffType.Stun)
                            || @base.HasBuffOfType(BuffType.Snare) || @base.HasBuffOfType(BuffType.Suppression),
                            DamageType.Physical,
                            (hero, @base) => (hero.TotalAttackDamage * 0.1) * hero.GetCritMultiplier(true));
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) => hero.HasBuff("talonnoxiandiplomacybuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q)
                            + (@base is Obj_AI_Hero ? hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered) : 0),
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E)
                            + (@base is Obj_AI_Hero
                                   ? hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.DamagePerSecond)
                                   : 0),
                            true);
                        break;
                    case "Thresh":
                        AddPassiveAttack(
                            "Thresh",
                            (hero, @base) => hero.Buffs.Any(b => b.Name.Contains("threshqpassive")),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var buff = hero.GetBuffCount("threshpassivesouls");
                                    var dmg = hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Empowered);
                                    dmg /= hero.HasBuff("threshqpassive1")
                                               ? 4
                                               : (hero.HasBuff("threshqpassive2")
                                                      ? 3
                                                      : (hero.HasBuff("threshqpassive3") ? 2 : 1));
                                    if (buff > 0)
                                    {
                                        dmg += hero.CalculateDamage(@base, DamageType.Magical, buff);
                                    }
                                    return dmg;
                                },
                            true);
                        break;
                    case "Tristana":
                        AddPassiveAttack(
                            "Tristana",
                            (hero, @base) => @base.GetBuffCount("tristanaecharge") == 3,
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E)
                            + (hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Buff) / 3 * 4),
                            true);
                        break;
                    case "Trundle":
                        AddPassiveAttack(
                            "Trundle",
                            (hero, @base) => hero.HasBuff("TrundleTrollSmash"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "TwistedFate":
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("cardmasterstackparticle"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
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
                            (hero, @base) => @base is Obj_AI_Hero,
                            DamageType.True,
                            (hero, @base) =>
                            (hero.Level < 5
                                 ? 12
                                 : (hero.Level < 9 ? 18 : (hero.Level < 13 ? 24 : (hero.Level < 17 ? 30 : 36))))
                            * Math.Min(Math.Max(0, @base.GetBuffCount("twitchdeadlyvenom")) + 1, 6));
                        break;
                    case "Udyr":
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("UdyrTigerStance"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("udyrtigerpunch"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.DamagePerSecond)
                            / (@base is Obj_AI_Hero ? 1 : 2),
                            true);
                        break;
                    case "Varus":
                        AddPassiveAttack(
                            "Varus",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Vayne":
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => hero.HasBuff("vaynetumblebonus"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => @base.GetBuffCount("vaynesilvereddebuff") == 2,
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Vi":
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => @base.GetBuffCount("viwproc") == 2,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => hero.HasBuff("ViE"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Viktor":
                        AddPassiveAttack(
                            "Viktor",
                            (hero, @base) => hero.HasBuff("viktorpowertransferreturn"),
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered)
                            + hero.CalculateDamage(
                                @base,
                                DamageType.Magical,
                                hero.TotalAttackDamage * hero.GetCritMultiplier(true)),
                            true,
                            true);
                        break;
                    case "Volibear":
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("VolibearQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("volibearrapplicator"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
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
                            true);
                        break;
                    case "XinZhao":
                        AddPassiveAttack(
                            "XinZhao",
                            (hero, @base) => hero.HasBuff("XenZhaoComboTarget"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
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
                                         && g.GetBuff("yorickunholysymbiosis").Caster.Compare(hero)))))
                            * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Yorick",
                            (hero, @base) => hero.HasBuff("YorickSpectral"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
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
                                    var dmg = (16 + ((level < 7 ? 4 : (level < 13 ? 8 : 12)) * level))
                                              + (hero.TotalMagicalDamage * (level < 7 ? 0.3 : (level < 13 ? 0.4 : 0.5)));
                                    return dmg;
                                });
                        break;
                }
            }
        }

        private static float GetCritMultiplier(this Obj_AI_Hero hero, bool checkCrit = false)
        {
            var crit = Items.HasItem((int)ItemId.Infinity_Edge, hero) ? 1.5f : 1;
            return !checkCrit ? crit : (Math.Abs(hero.Crit - 1) < float.Epsilon ? 1 + crit : 1);
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
        /// <param name="getOverride">
        ///     Get override damage.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static PassiveDamageInfo GetPassiveDamageInfo(
            this Obj_AI_Hero source,
            Obj_AI_Base target,
            bool getOverride = true)
        {
            var @double = 0d;
            var @override = false;
            List<PassiveDamage> value;

            // If you don't understand the code below, you should be flipping burgers instead.
            if (PassiveDamages.TryGetValue(string.Empty, out value))
            {
                // This should fix something that should never happen.
                value =
                    value.Where(
                        i =>
                        (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null
                        && (getOverride || !i.Override)).ToList();
                @double =
                    value.Sum(
                        item =>
                        item.IgnoreCalculation
                            ? item.Func(source, target)
                            : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));
                if (getOverride)
                {
                    @override = value.Any(i => i.Override);
                }
            }

            if (PassiveDamages.TryGetValue(source.ChampionName, out value))
            {
                value =
                    value.Where(
                        i =>
                        (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null
                        && (getOverride || !i.Override)).ToList();
                @double +=
                    value.Sum(
                        item =>
                        item.IgnoreCalculation
                            ? item.Func(source, target)
                            : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));
                if (getOverride && !@override)
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