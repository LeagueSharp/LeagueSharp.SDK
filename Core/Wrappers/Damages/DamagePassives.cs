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

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;

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
                (hero.ChampionName != "Fiora" || hero.ChampionName != "Shaco" || hero.ChampionName != "Yasuo")
                && Math.Abs(hero.Crit - 1) < float.Epsilon,
                DamageType.Physical,
                (hero, @base) => (1f + (Items.HasItem(3031, hero) ? .5f : 0f)) * hero.TotalAttackDamage);

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
                            (hero, @base) => hero.HasBuff("AatroxWPower") && hero.HasBuff("AatroxWONHPowerBuff"),
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
                            (.06 + (Math.Abs(hero.TotalMagicalDamage / 100) * .16667)) * hero.TotalAttackDamage);
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
                            (6 + hero.Level + (.1 * hero.TotalMagicalDamage)) * (@base is Obj_AI_Minion ? 2 : 1));
                        break;
                    case "Ashe":
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => @base.HasBuff("ashepassiveslow"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.TotalAttackDamage * (1 + .1f + (hero.FlatCritChanceMod * (1 + hero.FlatCritDamageMod))));
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => hero.HasBuff("asheqbuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
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
                            (hero, @base) => @base.GetBuffCount("braummarkcounter") == 3,
                            DamageType.Magical,
                            (hero, @base) => 32 + (8 * ((Obj_AI_Hero)@base.GetBuff("braummarkcounter").Caster).Level));
                        AddPassiveAttack(
                            "Braum",
                            (hero, @base) => @base.HasBuff("braummarkstunreduction"),
                            DamageType.Magical,
                            (hero, @base) => (32 + (8 * hero.Level)) * .2f);
                        break;
                    case "Caitlyn":
                        AddPassiveAttack(
                            "Caitlyn",
                            (hero, @base) => hero.HasBuff("caitlynheadshot"),
                            DamageType.Physical,
                            (hero, @base) =>
                            @base is Obj_AI_Minion && @base.GetMinionType() != MinionTypes.Ward
                                ? hero.CalculateDamage(@base, DamageType.Physical, hero.TotalAttackDamage * 1.5f)
                                : @base is Obj_AI_Hero
                                      ? hero.CalculatePhysicalDamage(@base, hero.TotalAttackDamage * .5f, .5f)
                                      : 0d,
                            false,
                            true);
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
                            DamageType.True,
                            (hero, @base) => hero.TotalAttackDamage * .1f);
                        break;
                    case "Darius":
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => true,
                            DamageType.Physical,
                            (hero, @base) =>
                            (9 + hero.Level + (hero.FlatPhysicalDamageMod * .3f))
                            * Math.Min(@base.GetBuffCount("dariushemo") + 1, 5) / (@base is Obj_AI_Hero ? 1 : 5));
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
                                    : hero.Level < 11 ? 10 : hero.Level < 14 ? 15 : hero.Level < 16 ? 20 : 25)
                               * hero.Level) + (hero.TotalMagicalDamage * .8f));
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
                            (hero, @base) => 10 + (10 * hero.Level) + (hero.TotalMagicalDamage * .8f));
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
                            (hero, @base) => hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1f + (Items.HasItem(3031, hero) ? .5f : 0f)
                             + new[] { -.6f, -.45f, -.3f, -.15f, 0 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Fizz":
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W) * (@base.HasBuff("fizzrbonusbuff") ? 1.2f : 1) * .5f
                            / 3,
                            false,
                            true);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("FizzSeastonePassive"),
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered)
                            * (@base.HasBuff("fizzrbonusbuff") ? 1.2f : 1),
                            false,
                            true);
                        break;
                    case "Gangplank":
                        AddPassiveAttack(
                            "Gangplank",
                            (hero, @base) => hero.HasBuff("gangplankpassiveattack"),
                            DamageType.True,
                            (hero, @base) =>
                            ((10 + (5 * hero.Level)) + (hero.FlatPhysicalDamageMod * .6f))
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
                    case "Hecarim":
                        AddPassiveAttack(
                            "Hecarim",
                            (hero, @base) => hero.HasBuff("hecarimrampspeed"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var d = hero.GetSpellDamage(@base, SpellSlot.E);
                                    return
                                        Math.Min(
                                            Math.Max(d * (Game.Time - hero.GetBuff("hecarimrampspeed").StartTime), d),
                                            hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Empowered));
                                },
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
                            (hero, @base) => Math.Min(@base.Health * .1f, 400));
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
                            (hero, @base) => hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true,
                            true);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycepassivemeleeattack"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 20, 60, 100, 140 }[hero.Spellbook.GetSpell(SpellSlot.R).Level - 1]
                            + (.4f * hero.TotalMagicalDamage));
                        break;
                    case "Jinx":
                        AddPassiveAttack(
                            "Jinx",
                            (hero, @base) => hero.HasBuff("JinxQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Kalista":
                        AddPassiveAttack(
                            "Kalista",
                            (hero, @base) => true,
                            DamageType.Physical,
                            (hero, @base) => .9f * hero.TotalAttackDamage,
                            true);
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
                                {
                                    var spellW =
                                        ((Obj_AI_Hero)@base.GetBuff("kalistacoopstrikemarkbuff").Caster).Spellbook
                                            .GetSpell(SpellSlot.W).Level - 1;
                                    return Math.Min(
                                        new[] { 75, 125, 150, 175, 200 }[spellW],
                                        hero.CalculateMagicDamage(
                                            @base,
                                            new[] { 0.1, 0.125, 0.15, 0.175, 0.2 }[spellW] * @base.MaxHealth));
                                },
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E) * (hero.HasBuff("JudicatorRighteousFury") ? 2 : 1),
                            false,
                            true);
                        break;
                    case "Kennen":
                        AddPassiveAttack(
                            "Kennen",
                            (hero, @base) => hero.HasBuff("kennendoublestrikelive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            false,
                            true);
                        break;
                    case "KhaZix":
                        AddPassiveAttack(
                            "KhaZix",
                            (hero, @base) => hero.HasBuff("khazixpdamage") && @base is Obj_AI_Hero,
                            DamageType.Magical,
                            (hero, @base) =>
                            10 + ((hero.Level < 6 ? 5 : hero.Level < 11 ? 10 : hero.Level < 14 ? 15 : 20) * hero.Level)
                            + (.5f * hero.TotalMagicalDamage));
                        break;
                    case "Kindred":
                        AddPassiveAttack(
                            "Kindred",
                            (hero, @base) => hero.GetBuffCount("kindredmarkofthekindredstackcounter") > 0,
                            DamageType.Physical,
                            (hero, @base) =>
                            (.125f * @base.Health) * hero.GetBuffCount("kindredmarkofthekindredstackcounter"));
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
                                    var lvl = ((Obj_AI_Hero)@base.GetBuff("leonasunlight").Caster).Level;
                                    return lvl < 3
                                               ? 20
                                               : lvl < 5
                                                     ? 35
                                                     : lvl < 7
                                                           ? 50
                                                           : lvl < 9
                                                                 ? 65
                                                                 : lvl < 11
                                                                       ? 80
                                                                       : lvl < 13
                                                                             ? 95
                                                                             : lvl < 15 ? 110 : lvl < 17 ? 125 : 140;
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
                                 : hero.Level < 5 ? .3f : hero.Level < 11 ? .4f : hero.Level < 16 ? .5f : .6f)
                            * hero.TotalAttackDamage);
                        break;
                    case "Lux":
                        AddPassiveAttack(
                            "Lux",
                            (hero, @base) => hero.HasBuff("LuxIlluminatingFraulein"),
                            DamageType.Magical,
                            (hero, @base) => 10 + (8 * hero.Level) + (.2f * hero.TotalMagicalDamage));
                        break;
                    case "MasterYi":
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("doublestrike"),
                            DamageType.Physical,
                            (hero, @base) => .5f * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("wujustylesuperchargedvisual"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            false,
                            true);
                        break;
                    case "MissFortune":
                        AddPassiveAttack(
                            "MissFortune",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Min(
                                @base.GetBuffCount("missfortunepassivestack") + 1,
                                5 + hero.Spellbook.GetSpell(SpellSlot.R).Level)
                            * hero.GetSpellDamage(@base, SpellSlot.W),
                            false,
                            true);
                        break;
                    case "Mordekaiser":
                        AddPassiveAttack(
                            "Mordekaiser",
                            (hero, @base) => hero.Buffs.Any(b => b.Name.Contains("mordekaisermaceofspades")),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var d = hero.GetSpellDamage(@base, SpellSlot.Q);
                                    return hero.HasBuff("mordekaisermaceofspades15")
                                               ? d * 2
                                               : hero.HasBuff("mordekaisermaceofspades2") ? d * 4 : d;
                                },
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
                                        + (.2f * caster.TotalMagicalDamage);
                                });
                        break;
                    case "Nasus":
                        AddPassiveAttack(
                            "Nasus",
                            (hero, @base) => hero.HasBuff("NasusQ"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 50, 70, 90, 110 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + hero.GetBuffCount("nasusqstacks"));
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.SecondForm)
                            * (@base.HasBuff("nidaleepassivehunted") ? 1.33f : 1),
                            true,
                            true);
                        break;
                    case "Nocturne":
                        AddPassiveAttack(
                            "Nocturne",
                            (hero, @base) => hero.HasBuff("nocturneumbrablades"),
                            DamageType.Physical,
                            (hero, @base) => 1.2f * hero.TotalAttackDamage);
                        break;
                    case "Nunu":
                        AddPassiveAttack(
                            "Nunu",
                            (hero, @base) => hero.HasBuff("nunuqbufflizard"),
                            DamageType.Magical,
                            (hero, @base) => .01 * hero.MaxHealth);
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
                                                 : hero.Level < 7
                                                       ? 18
                                                       : hero.Level < 10
                                                             ? 26
                                                             : hero.Level < 13 ? 34 : hero.Level < 16 ? 42 : 50)
                                            + (.15f * hero.TotalMagicalDamage);
                                    return d
                                           + (Orbwalker.LastTarget.Compare(@base)
                                                  ? d * .2f
                                                    * Math.Min(hero.GetBuffCount("orianapowerdaggerdisplay") + 1, 2)
                                                  : 0);
                                });
                        break;
                    case "Pantheon":
                        AddPassiveAttack(
                            "Pantheon",
                            (hero, @base) => @base.HealthPercent < 15 && hero.Spellbook.GetSpell(SpellSlot.E).Level > 0,
                            DamageType.Physical,
                            (hero, @base) => (1f + (Items.HasItem(3031, hero) ? .5f : 0f)) * hero.TotalAttackDamage);
                        break;
                    case "Poppy":
                        AddPassiveAttack(
                            "Poppy",
                            (hero, @base) => hero.HasBuff("PoppyDevastatingBlow"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            false,
                            true);
                        break;
                    case "Quinn":
                        AddPassiveAttack(
                            "Quinn",
                            (hero, @base) => @base.HasBuff("QuinnW"),
                            DamageType.Physical,
                            (hero, @base) =>
                            15 + ((hero.Level <= 14 ? 10 : 15) * hero.Level) + (.5f * hero.FlatPhysicalDamageMod));
                        break;
                    case "Rammus":
                        AddPassiveAttack(
                            "Rammus",
                            (hero, @base) => true,
                            DamageType.Physical,
                            (hero, @base) => .25f * hero.Armor);
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
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.W) * (hero.HasBuff("renektonrageready") ? 1.5 : 1),
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
                                 ? .2f
                                 : hero.Level < 6
                                       ? .25f
                                       : hero.Level < 9
                                             ? .3f
                                             : hero.Level < 12
                                                   ? .35f
                                                   : hero.Level < 15 ? .4f : hero.Level < 18 ? .45f : .5f)
                            * hero.TotalAttackDamage);
                        break;
                    case "Rumble":
                        AddPassiveAttack(
                            "Rumble",
                            (hero, @base) => hero.HasBuff("rumbleoverheat"),
                            DamageType.Magical,
                            (hero, @base) => 20 + (5 * hero.Level) + (.3f * hero.TotalMagicalDamage));
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
                            (hero, @base) => hero.IsFacing(@base) && !@base.IsFacing(hero),
                            DamageType.Physical,
                            (hero, @base) => hero.TotalAttackDamage * .2f);
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (1f + (Items.HasItem(3031, hero) ? .5f : 0f)
                             + new[] { -.6f, -.4f, -.2f, 0, .2f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1])
                            * hero.TotalAttackDamage);
                        break;
                    case "Shen":
                        AddPassiveAttack(
                            "Shen",
                            (hero, @base) => hero.HasBuff("shenwayoftheninjaaura"),
                            DamageType.Magical,
                            (hero, @base) => 4 + (4 * hero.Level) + (hero.BonusHealth * .1f));
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
                            Math.Min(.1f * @base.MaxHealth, @base is Obj_AI_Minion ? 75 : @base.MaxHealth));
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
                                            + (level < 4 ? 7 : level < 6 ? 8 : level < 7 ? 9 : level < 15 ? 10 : 15)
                                            * level) + (.2f * hero.TotalMagicalDamage);
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
                                        + (.2f * caster.TotalMagicalDamage)
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
                            (hero, @base) => hero.TotalAttackDamage * .1f);
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) => hero.HasBuff("talonnoxiandiplomacybuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.Q)
                            + (@base is Obj_AI_Hero
                                   ? hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.DamagePerSecond)
                                   : 0),
                            false,
                            true);
                        break;
                    case "Taric":
                        AddPassiveAttack(
                            "Taric",
                            (hero, @base) => hero.HasBuff("taricgemcraftbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.Armor * .2f);
                        break;
                    case "Teemo":
                        AddPassiveAttack(
                            "Teemo",
                            (hero, @base) => hero.HasBuff("ToxicShot"),
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E)
                            + hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.DamagePerSecond)
                            / (@base is Obj_AI_Hero ? 1 : 4),
                            false,
                            true);
                        break;
                    case "Thresh":
                        AddPassiveAttack(
                            "Thresh",
                            (hero, @base) =>
                            hero.Buffs.Any(b => b.Name.Contains("threshqpassive"))
                            && @base.GetMinionType() != MinionTypes.Ward,
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.GetBuffCount("threshpassivesouls")
                            + ((new[] { .8f, 1.1f, 1.4f, 1.7f, 2f }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                                / (hero.HasBuff("threshqpassive1")
                                       ? 4
                                       : hero.HasBuff("threshqpassive2") ? 3 : hero.HasBuff("threshqpassive3") ? 2 : 1))
                               * hero.TotalAttackDamage));
                        break;
                    case "Tristana":
                        AddPassiveAttack(
                            "Tristana",
                            (hero, @base) => @base.GetBuffCount("tristanaecharge") == 4,
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.GetSpellDamage(@base, SpellSlot.E)
                            + hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Buff),
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
                                 : hero.Level < 9 ? 18 : hero.Level < 13 ? 24 : hero.Level < 17 ? 30 : 36)
                            * Math.Min(@base.GetBuffCount("twitchdeadlyvenom") + 1, 6) / (@base is Obj_AI_Hero ? 1 : 6f));
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
                            (hero, @base) => 2.5f + (hero.Level < 10 ? .5f : 1f) * hero.Level);
                        break;
                    case "MonkeyKing":
                        AddPassiveAttack(
                            "MonkeyKing",
                            (hero, @base) =>
                            hero.HasBuff("MonkeyKingDoubleAttack") && @base.GetMinionType() != MinionTypes.Ward,
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
                            (hero, @base) => (Items.HasItem(3031, hero) ? 2.25f : 1.8f) * hero.TotalAttackDamage);
                        break;
                    case "Yorick":
                        AddPassiveAttack(
                            "Yorick",
                            (hero, @base) => hero.GetBuffCount("yorickunholysymbiosis") > 0,
                            DamageType.Physical,
                            (hero, @base) =>
                            (.05f * hero.GetBuffCount("yorickunholysymbiosis")) * hero.TotalAttackDamage);
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
                            (hero, @base) => @base.HealthPercent < 50f && !@base.HasBuff("ZedPassiveCD"),
                            DamageType.Magical,
                            (hero, @base) => (hero.Level < 7 ? .06f : hero.Level < 17 ? .08f : .1f) * @base.MaxHealth);
                        break;
                    case "Ziggs":
                        AddPassiveAttack(
                            "Ziggs",
                            (hero, @base) => hero.HasBuff("ziggsshortfuse"),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var level = hero.Level;
                                    var amount = (16 + (level < 7 ? 4 : level < 13 ? 8 : 12) * level)
                                                 + (hero.TotalMagicalDamage
                                                    * (level < 7 ? .25f : level < 13 ? .3f : 35f));
                                    return @base is Obj_AI_Turret ? amount * 1.5f : amount;
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