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

    using LeagueSharp.SDK.Core.Extensions;

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
                (hero, @base) => hero.TotalAttackDamage);

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
                            (hero, @base) =>
                            hero.HasBuff("AatroxWPower") && hero.HasBuff("AatroxWONHPowerBuff")
                            && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 60, 95, 130, 165, 200 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + hero.FlatPhysicalDamageMod);
                        break;
                    case "Akali":
                        AddPassiveAttack(
                            "Akali",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            (.06f + (Math.Abs(hero.TotalMagicalDamage / 6) * .01f)) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Akali",
                            (hero, @base) => @base.HasBuff("AkaliMota"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 45, 70, 95, 120, 145 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (hero.TotalMagicalDamage * .5d));
                        break;
                    case "Alistar":
                        AddPassiveAttack(
                            "Alistar",
                            (hero, @base) => hero.HasBuff("alistartrample"),
                            DamageType.Magical,
                            (hero, @base) =>
                            (6d + hero.Level + (.1d * hero.TotalMagicalDamage)) * (@base is Obj_AI_Minion ? 2 : 1));
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
                            (hero, @base) => hero.HasBuff("asheqbuff") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 1.15f, 1.2f, 1.25f, 1.3f, 1.35f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            * hero.TotalAttackDamage);
                        break;
                    case "Blitzcrank":
                        AddPassiveAttack(
                            "Blitzcrank",
                            (hero, @base) => hero.HasBuff("PowerFist"),
                            DamageType.Physical,
                            (hero, @base) => hero.TotalAttackDamage);
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
                            @base is Obj_AI_Minion
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
                            (hero, @base) =>
                            new[] { 20, 35, 50, 65, 80 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (hero.TotalMagicalDamage * .3f));
                        break;
                    case "Corki":
                        AddPassiveAttack(
                            "Corki",
                            (hero, @base) => !(@base is Obj_AI_Turret),
                            DamageType.True,
                            (hero, @base) => hero.TotalAttackDamage * .1f);
                        break;
                    case "Darius":
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            (9 + hero.Level + (hero.FlatPhysicalDamageMod * .3f))
                            * Math.Max(@base.GetBuffCount("dariushemo"), 1));
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => hero.HasBuff("DariusNoxianTacticsONH"),
                            DamageType.Physical,
                            (hero, @base) => .4f * hero.TotalAttackDamage);
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
                    case "Draven":
                        AddPassiveAttack(
                            "Draven",
                            (hero, @base) => hero.HasBuff("DravenSpinning") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { .45f, .55f, .65f, .75f, .85f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            * hero.TotalAttackDamage);
                        break;
                    case "Ekko":
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => @base.GetBuffCount("ekkostacks") == 2,
                            DamageType.Magical,
                            (hero, @base) => 10 + (10 * hero.Level) + (hero.TotalMagicalDamage * .8f));
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
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0 && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            (new[] { 20, 30, 40, 50, 60 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                             + (.45f * hero.TotalMagicalDamage)
                             + (new[] { .04f, .05f, .06f, .07f, .08f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                * (@base.MaxHealth - @base.Health))) * (@base.HasBuff("fizzrbonusbuff") ? 1.2f : 1)
                            * .5f / 3);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("FizzSeastonePassive") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            (new[] { 10, 15, 20, 25, 30 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                             + (.3f * hero.TotalMagicalDamage)) * (@base.HasBuff("fizzrbonusbuff") ? 1.2f : 1));
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
                            (hero, @base) =>
                            new[] { 30, 55, 80, 105, 130 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (.4f * hero.TotalAttackDamage));
                        break;
                    case "Gnar":
                        AddPassiveAttack(
                            "Gnar",
                            (hero, @base) => @base.GetBuffCount("gnarwproc") == 2,
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var index = hero.Spellbook.GetSpell(SpellSlot.W).Level - 1;
                                    return new[] { 10, 20, 30, 40, 50 }[index]
                                           + Math.Min(
                                               (new[] { .06f, .08f, .1f, .12f, .14f }[index] * @base.MaxHealth),
                                               new[] { 100, 150, 200, 250, 300 }[index]) + hero.TotalMagicalDamage;
                                });
                        break;
                    case "Gragas":
                        AddPassiveAttack(
                            "Gragas",
                            (hero, @base) => hero.HasBuff("gragaswattackbuff") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 20, 50, 80, 110, 140 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (.3f * hero.TotalMagicalDamage) + (.008f * @base.MaxHealth));
                        break;
                    case "Hecarim":
                        AddPassiveAttack(
                            "Hecarim",
                            (hero, @base) => hero.HasBuff("hecarimrampspeed"),
                            DamageType.Physical,
                            (hero, @base) =>
                            (new[] { 40, 75, 110, 145, 180 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                             * (Game.Time - hero.GetBuff("hecarimrampspeed").StartTime))
                            + (hero.FlatPhysicalDamageMod * .5f));
                        break;
                    case "Irelia":
                        AddPassiveAttack(
                            "Irelia",
                            (hero, @base) => hero.HasBuff("ireliahitenstylecharged") && !(@base is Obj_AI_Turret),
                            DamageType.True,
                            (hero, @base) =>
                            new[] { 15, 30, 45, 60, 75 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]);
                        break;
                    case "JarvanIV":
                        AddPassiveAttack(
                            "JarvanIV",
                            (hero, @base) => !@base.HasBuff("jarvanivmartialcadencecheck") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) => Math.Min(@base.Health * .1f, 400));
                        break;
                    case "Jax":
                        AddPassiveAttack(
                            "Jax",
                            (hero, @base) => hero.HasBuff("JaxEmpowerTwo") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 40, 75, 110, 145, 180 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (.6f * hero.TotalMagicalDamage));
                        break;
                    case "Jayce":
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("jaycehypercharge"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { .7f, .78f, .86f, .94f, 1.02f, 1.1f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            * hero.TotalAttackDamage,
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
                            (hero, @base) => .1f * hero.TotalAttackDamage);
                        break;
                    case "Kalista":
                        AddPassiveAttack(
                            "Kalista",
                            (hero, @base) => true,
                            DamageType.Physical,
                            (hero, @base) => .9f * hero.TotalAttackDamage,
                            true);
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.HasBuff("") && @base.GetBuff("").Caster.NetworkId != hero.NetworkId,
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Min(
                                new[] { .1f, .125f, .15f, .175f, .2f }[
                                    ((Obj_AI_Hero)@base.GetBuff("").Caster).Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                * @base.MaxHealth,
                                @base is Obj_AI_Minion
                                    ? new[] { 75, 125, 150, 175, 200 }[
                                        ((Obj_AI_Hero)@base.GetBuff("").Caster).Spellbook.GetSpell(SpellSlot.W).Level
                                        - 1]
                                    : @base.MaxHealth));
                        break;
                    case "Kassadin":
                        AddPassiveAttack(
                            "Kassadin",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => 20 + (.1f * hero.TotalMagicalDamage));
                        AddPassiveAttack(
                            "Kassadin",
                            (hero, @base) => hero.HasBuff("NetherBlade") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 20, 45, 70, 95, 120 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (.6f * hero.TotalMagicalDamage));
                        break;
                    case "Katarina":
                        AddPassiveAttack(
                            "Katarina",
                            (hero, @base) => @base.HasBuff("katarinaqmark"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 15, 30, 45, 60, 75 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (.15f * hero.TotalMagicalDamage));
                        break;
                    case "Kayle":
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.E).Level > 0 && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 10, 15, 20, 25, 30 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (.15f * hero.TotalMagicalDamage));
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.HasBuff("JudicatorRighteousFury"),
                            DamageType.Magical,
                            (hero, @base) =>
                            (new[] { 20, 30, 40, 50, 60 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                             + (.3f * hero.TotalMagicalDamage))
                            + (new[] { 20, 25, 30, 35, 40 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                               * hero.TotalAttackDamage));
                        break;
                    case "Kennen":
                        AddPassiveAttack(
                            "Kennen",
                            (hero, @base) => hero.HasBuff("kennendoublestrikelive") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { .4f, .5f, .6f, .7f, .8f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            * hero.TotalAttackDamage);
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
                    case "KogMaw":
                        AddPassiveAttack(
                            "KogMaw",
                            (hero, @base) => hero.HasBuff("KogMawBioArcaneBarrage") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Min(
                                (new[] { .02f, .03f, .04f, .05f, .06f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                 + (Math.Abs(hero.TotalMagicalDamage / 100) * .01f)) * @base.MaxHealth,
                                @base is Obj_AI_Minion ? 100 : @base.MaxHealth));
                        break;
                    case "Leona":
                        /*AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.HasBuff("") && @base.GetBuff("").Caster.NetworkId != hero.NetworkId,
                            DamageType.Magical,
                            (hero, @base) =>
                            hero.Level < 3
                                ? 20
                                : hero.Level < 5
                                      ? 35
                                      : hero.Level < 7
                                            ? 50
                                            : hero.Level < 9
                                                  ? 65
                                                  : hero.Level < 11
                                                        ? 80
                                                        : hero.Level < 13
                                                              ? 95
                                                              : hero.Level < 15 ? 110 : hero.Level < 17 ? 125 : 140);*/
                        AddPassiveAttack(
                            "Leona",
                            (hero, @base) => hero.HasBuff("LeonaShieldOfDaybreak"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 40, 70, 100, 130, 160 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (.3f * hero.TotalMagicalDamage));
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
                    case "Malphite":
                        AddPassiveAttack(
                            "Malphite",
                            (hero, @base) => hero.HasBuff("malphitecleave") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 15, 30, 45, 60, 75 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (.1f * hero.TotalMagicalDamage) + (.1f * hero.Armor));
                        break;
                    case "MasterYi":
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("doublestrike"),
                            DamageType.Physical,
                            (hero, @base) => .5f * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("wujustylesuperchargedvisual") && !(@base is Obj_AI_Turret),
                            DamageType.True,
                            (hero, @base) =>
                            new[] { 10, 15, 20, 25, 30 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (new[] { .1f, .125f, .15f, .175f, .2f }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                               * hero.TotalAttackDamage));
                        break;
                    case "MissFortune":
                        AddPassiveAttack(
                            "MissFortune",
                            (hero, @base) => !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Max(@base.GetBuffCount("missfortunepassivestack"), 1) * (.6 * hero.TotalAttackDamage));
                        break;
                    case "Mordekaiser":
                        AddPassiveAttack(
                            "Mordekaiser",
                            (hero, @base) =>
                            hero.Buffs.Any(b => b.Name.Contains("mordekaisermaceofspades")) && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                                {
                                    var d = new[] { 4, 8, 12, 16, 20 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                                            + (new[] { .25f, .263f, .275f, .288f, .3f }[
                                                hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1] * hero.TotalAttackDamage)
                                            + (.2f * hero.TotalMagicalDamage);
                                    return hero.HasBuff("mordekaisermaceofspades15")
                                               ? d * 2
                                               : hero.HasBuff("mordekaisermaceofspades2") ? d * 4 : d;
                                });
                        break;
                    case "Nami":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("NamiE") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 25, 40, 55, 70, 85 }[
                                ((Obj_AI_Hero)hero.GetBuff("NamiE").Caster).Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (.2f * ((Obj_AI_Hero)hero.GetBuff("NamiE").Caster).TotalMagicalDamage));
                        break;
                    case "Nasus":
                        AddPassiveAttack(
                            "Nasus",
                            (hero, @base) => hero.HasBuff("NasusQ"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 50, 70, 90, 110 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + Math.Max(hero.GetBuffCount("nasusqstacks"), 1));
                        break;
                    case "Nautilus":
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => !@base.HasBuff("nautiluspassivecheck") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) => 2 + (6 * hero.Level));
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => hero.HasBuff("nautiluspiercinggazeshield") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 30, 40, 50, 60, 70 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (hero.TotalMagicalDamage * .4f));
                        break;
                    case "Nidalee":
                        AddPassiveAttack(
                            "Nidalee",
                            (hero, @base) => hero.HasBuff("Takedown"),
                            DamageType.Magical,
                            (hero, @base) =>
                            (new[] { 4, 20, 50, 90 }[hero.Spellbook.GetSpell(SpellSlot.R).Level - 1]
                             + (.75f * hero.TotalAttackDamage) + (.36f * hero.TotalMagicalDamage))
                            * (@base.HasBuff("nidaleepassivehunted") ? 1.33f : 1)
                            * (!(@base is Obj_AI_Turret)
                                   ? ((@base.MaxHealth - @base.Health) / @base.MaxHealth * 1.5) + 1
                                   : 1),
                            true);
                        break;
                    case "Nocturne":
                        AddPassiveAttack(
                            "Nocturne",
                            (hero, @base) => hero.HasBuff("nocturneumbrablades") && !(@base is Obj_AI_Turret),
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
                                           + (Variables.Orbwalker != null
                                              && Variables.Orbwalker.LastTarget.Compare(@base)
                                                  ? d * .2f * Math.Max(hero.GetBuffCount("orianapowerdaggerdisplay"), 1)
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
                            (hero, @base) => hero.HasBuff("PoppyDevastatingBlow") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 20, 40, 60, 80, 100 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + Math.Min(
                                .08 * @base.MaxHealth,
                                new[] { 55, 110, 165, 220, 275 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1])
                            + hero.TotalAttackDamage + (.6 * hero.TotalMagicalDamage));
                        break;
                    case "Quinn":
                        AddPassiveAttack(
                            "Quinn",
                            (hero, @base) => @base.HasBuff("QuinnW"),
                            DamageType.Physical,
                            (hero, @base) =>
                            15 + ((hero.Level <= 14 ? 10 : 15) * hero.Level) + (.5f * hero.FlatPhysicalDamageMod));
                        break;
                    case "RekSai":
                        AddPassiveAttack(
                            "RekSai",
                            (hero, @base) => hero.HasBuff("RekSaiQ") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 15, 25, 35, 45, 55 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (.2f * hero.FlatPhysicalDamageMod));
                        break;
                    case "Renekton":
                        AddPassiveAttack(
                            "Renekton",
                            (hero, @base) => hero.HasBuff("RenektonPreExecute") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            hero.HasBuff("renektonrageready")
                                ? new[] { 15, 45, 75, 105, 135 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                  + (2.25f * hero.TotalAttackDamage)
                                : new[] { 10, 30, 50, 70, 90 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                  + (1.5f * hero.TotalAttackDamage),
                            true);
                        break;
                    case "Rengar":
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqbase") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 60, 90, 120, 150 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (new[] { 0, .05f, .1f, .15f, .2f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                               * hero.TotalAttackDamage));
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("rengarqemp") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            15 + ((hero.Level <= 9 ? 15 : 10) * hero.Level) + (.5f * hero.TotalAttackDamage));
                        break;
                    case "Riven":
                        AddPassiveAttack(
                            "Riven",
                            (hero, @base) => hero.HasBuff("rivenpassiveaaboost") && !(@base is Obj_AI_Turret),
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
                            (hero, @base) => hero.HasBuff("rumbleoverheat") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) => 20 + (5 * hero.Level) + (.3f * hero.TotalMagicalDamage));
                        break;
                    case "Sejuani":
                        AddPassiveAttack(
                            "Sejuani",
                            (hero, @base) => hero.HasBuff("sejuaninorthernwindsenrage") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            (new[] { .04f, .045f, .05f, .055f, .06f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                             + (Math.Abs(hero.TotalMagicalDamage / 100) * .03f)) * @base.MaxHealth);
                        break;
                    case "Shaco":
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.IsFacing(@base) && !@base.IsFacing(hero) && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) => hero.TotalAttackDamage * .2f);
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.HasBuff("Deceive") && !(@base is Obj_AI_Turret),
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
                            (hero, @base) =>
                            4 + (4 * hero.Level) + ((hero.MaxHealth - (570.8 + (85 * hero.Level))) * .1f));
                        break;
                    case "Shyvana":
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => hero.HasBuff("ShyvanaDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { .8f, .85f, .9f, .95f, 1f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => @base.HasBuff("ShyvanaFireballMissile"),
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Min(@base.MaxHealth * .025f, @base is Obj_AI_Hero ? @base.MaxHealth : 100));
                        break;
                    case "Sion":
                        AddPassiveAttack(
                            "Sion",
                            (hero, @base) => hero.HasBuff("sionpassivezombie"),
                            DamageType.Physical,
                            (hero, @base) =>
                            Math.Min(.1f * @base.MaxHealth, @base is Obj_AI_Minion ? 75 : @base.MaxHealth));
                        break;
                    case "Sivir":
                        AddPassiveAttack(
                            "Sivir",
                            (hero, @base) => hero.HasBuff("sivirwmarker"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { .5f, .55f, .6f, .65f, .7f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            * hero.TotalAttackDamage);
                        break;
                    case "Skarner":
                        AddPassiveAttack(
                            "Skarner",
                            (hero, @base) => @base.HasBuff("skarnerpassivebuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 25, 35, 45, 55, 65 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]);
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
                        break;
                    case "TahmKench":
                        AddPassiveAttack(
                            "TahmKench",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.R).Level > 0,
                            DamageType.Magical,
                            (hero, @base) =>
                            20
                            + (new[] { .04f, .06f, 08f }[hero.Spellbook.GetSpell(SpellSlot.R).Level - 1]
                               * (hero.MaxHealth - (610 + (95 * hero.Level)))));
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
                            new[] { 30, 60, 90, 120, 150 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (hero.FlatPhysicalDamageMod * .3f));
                        break;
                    case "Taric":
                        AddPassiveAttack(
                            "Taric",
                            (hero, @base) => hero.HasBuff("taricgemcraftbuff") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) => hero.Armor * .2f);
                        break;
                    case "Teemo":
                        AddPassiveAttack(
                            "Teemo",
                            (hero, @base) => hero.HasBuff("ToxicShot"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 10, 20, 30, 40, 50 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (hero.TotalMagicalDamage * .3f));
                        break;
                    case "Thresh":
                        AddPassiveAttack(
                            "Thresh",
                            (hero, @base) =>
                            hero.Buffs.Any(b => b.Name.Contains("threshqpassive")) && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            Math.Max(hero.GetBuffCount("threshpassivesouls"), 0)
                            + ((new[] { .8f, 1.1f, 1.4f, 1.7f, 2f }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                                / (hero.HasBuff("threshqpassive1")
                                       ? 4
                                       : hero.HasBuff("threshqpassive2") ? 3 : hero.HasBuff("threshqpassive3") ? 2 : 1))
                               * hero.TotalAttackDamage));
                        break;
                    case "Trundle":
                        AddPassiveAttack(
                            "Trundle",
                            (hero, @base) => hero.HasBuff("TrundleTrollSmash"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var level = hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1;
                                    return new[] { 20, 40, 60, 80, 100 }[level]
                                           + (new[] { 0f, .05f, .1f, .15f, .2f }[level] * hero.TotalAttackDamage);
                                });
                        break;
                    case "TwistedFate":
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("cardmasterstackparticle") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 55, 80, 105, 130, 155 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (hero.TotalMagicalDamage * .5f));
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("bluecardpreattack"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 40, 60, 80, 100, 120 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + hero.TotalAttackDamage + (hero.TotalMagicalDamage * .5f),
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("redcardpreattack"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 30, 45, 60, 75, 90 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + hero.TotalAttackDamage + (hero.TotalMagicalDamage * .5f),
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("goldcardpreattack"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 15, 22.5, 30, 37.5, 45 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + hero.TotalAttackDamage + (hero.TotalMagicalDamage * .5f),
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
                            * Math.Max(@base.GetBuffCount("twitchdeadlyvenom"), 1));
                        break;
                    case "Udyr":
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("UdyrTigerStance"),
                            DamageType.Physical,
                            (hero, @base) => hero.TotalAttackDamage * .15f);
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.HasBuff("udyrtigerpunch"),
                            DamageType.Physical,
                            (hero, @base) =>
                                {
                                    var index = hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1;
                                    return new[] { 30, 80, 130, 180, 230 }[index]
                                           + (new[] { 1.2f, 1.3f, 1.4f, 1.5f, 1.6f }[index] * hero.TotalAttackDamage);
                                });
                        break;
                    case "Varus":
                        AddPassiveAttack(
                            "Varus",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 10, 14, 18, 22, 26 }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                            + (hero.TotalMagicalDamage * .25f));
                        break;
                    case "Vayne":
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => hero.HasBuff("vaynetumblebonus") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { .3f, .35f, .4f, .45f, .5f }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => @base.GetBuffCount("vaynesilvereddebuff") == 2,
                            DamageType.True,
                            (hero, @base) =>
                                {
                                    var index = hero.Spellbook.GetSpell(SpellSlot.W).Level - 1;
                                    return
                                        Math.Min(
                                            new[] { 20, 30, 40, 50, 60 }[index]
                                            + (new[] { .04f, .05f, .06f, .07f, .08f }[index] * @base.MaxHealth),
                                            @base is Obj_AI_Minion ? 200 : @base.MaxHealth);
                                });
                        break;
                    case "Vi":
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => @base.GetBuffCount("viwproc") == 2,
                            DamageType.Physical,
                            (hero, @base) =>
                            Math.Min(
                                (new[] { .04f, .055f, .07f, .085f, .1f }[hero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                 + (Math.Abs(hero.FlatPhysicalDamageMod / 35) * .01f)) * @hero.MaxHealth,
                                @base is Obj_AI_Minion ? 300 : @hero.MaxHealth));
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => hero.HasBuff("ViE"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 5, 20, 35, 50, 65 }[hero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                            + (hero.TotalAttackDamage * .15f) + (hero.TotalMagicalDamage * .7f));
                        break;
                    case "Viktor":
                        AddPassiveAttack(
                            "Viktor",
                            (hero, @base) => hero.HasBuff("viktorpowertransferreturn") && !(@base is Obj_AI_Turret),
                            DamageType.Magical,
                            (hero, @base) =>
                            (15 + (hero.Level < 10 ? 5 : hero.Level < 13 ? 10 : 20) * hero.Level)
                            + hero.TotalAttackDamage + (hero.TotalMagicalDamage * .5f),
                            true);
                        break;
                    case "Volibear":
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("VolibearQ"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 60, 90, 120, 150 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]);
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("volibearrapplicator"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 75, 115, 155 }[hero.Spellbook.GetSpell(SpellSlot.R).Level - 1]
                            + (hero.TotalMagicalDamage * .3f));
                        break;
                    case "Warwick":
                        AddPassiveAttack(
                            "Warwick",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => 2.5f + (hero.Level < 10 ? .5f : 1f) * hero.Level);
                        break;
                    case "Wukong":
                        AddPassiveAttack(
                            "Wukong",
                            (hero, @base) => hero.HasBuff("MonkeyKingDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 60, 90, 120, 150 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (hero.TotalAttackDamage * .1f));
                        break;
                    case "XinZhao":
                        AddPassiveAttack(
                            "XinZhao",
                            (hero, @base) => hero.HasBuff("XenZhaoComboTarget"),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 15, 30, 45, 60, 75 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (hero.TotalAttackDamage * .2f));
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
                            (hero, @base) => hero.HasBuff("YorickSpectral") && !(@base is Obj_AI_Turret),
                            DamageType.Physical,
                            (hero, @base) =>
                            new[] { 30, 60, 90, 120, 150 }[hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1]
                            + (hero.TotalAttackDamage * .2f));
                        break;
                    case "Zed":
                        AddPassiveAttack(
                            "Zed",
                            (hero, @base) =>
                            @base.HealthPercent < 50f && !@base.HasBuff("ZedPassiveCD") && !(@base is Obj_AI_Turret),
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