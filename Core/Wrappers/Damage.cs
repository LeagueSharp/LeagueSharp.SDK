//#define damageDebug // C#6

#if damageDebug

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Damage.cs" company="LeagueSharp">
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
//   Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Properties;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    using Newtonsoft.Json.Linq;

    using Enumerable = System.Linq.Enumerable;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static class Damage
    {
        #region Static Fields

        /// <summary>
        ///     The damage version files.
        /// </summary>
        private static readonly IDictionary<string, byte[]> DamageFiles = new Dictionary<string, byte[]>
                                                                              { { "5.17", Resources._5_17 } };

        #endregion

        #region Public Properties

        /// <summary>
        ///     The damages dictionary.
        /// </summary>
        public static IDictionary<string, DamageInfo> DamagesDictionary { get; } = new Dictionary<string, DamageInfo>();

        /// <summary>
        ///     The passive dictionary.
        /// </summary>
        public static IDictionary<string, PassiveInfo> PassiveDictionary { get; } =
            new Dictionary<string, PassiveInfo>();

        /// <summary>
        ///     Gets the version.
        /// </summary>
        public static string Version { get; private set; }

        #endregion

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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            DamageType damageType,
            double amount,
            double forceDamageModifier = 0d)
        {
            var damage = 0d;
            switch (damageType)
            {
                case DamageType.Magical:
                    damage = source.CalculateMagicDamage(target, amount, forceDamageModifier);
                    break;
                case DamageType.Physical:
                    damage = source.CalculatePhysicalDamage(target, amount, forceDamageModifier);
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

            if (!includePassive)
            {
                return source.CalculatePhysicalDamage(target, result);
            }

            var k = 1d;
            var reduction = 0d;

            var hero = source as Obj_AI_Hero;
            if (hero != null)
            {
                // Spoils of War
                var minionTarget = target as Obj_AI_Minion;
                if (hero.IsMelee() && minionTarget != null && minionTarget.IsEnemy
                    && minionTarget.Team != GameObjectTeam.Neutral
                    && hero.Buffs.Any(buff => buff.Name == "talentreaperdisplay" && buff.Count > 0))
                {
                    if (
                        GameObjects.Heroes.Any(
                            h =>
                            h.NetworkId != source.NetworkId && h.Team == source.Team
                            && h.Distance(minionTarget.Position) < 1100))
                    {
                        var value = 0;

                        if (Items.HasItem(3302, hero))
                        {
                            value = 200; // Relic Shield
                        }
                        else if (Items.HasItem(3097, hero))
                        {
                            value = 240; // Targon's Brace
                        }
                        else if (Items.HasItem(3401, hero))
                        {
                            value = 400; // Face of the Mountain
                        }

                        return value + hero.TotalAttackDamage;
                    }
                }

                // BotRK
                if (Items.HasItem(3153, hero))
                {
                    var d = 0.08 * target.Health;
                    if (target is Obj_AI_Minion)
                    {
                        d = Math.Min(d, 60);
                    }

                    result += d;
                }

                // Arcane blade
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 132 && m.Points == 1))
                {
                    reduction -= CalculateMagicDamage(hero, target, 0.05 * hero.FlatMagicDamageMod);
                }

                // Champion Passive
                PassiveInfo passiveInfo;
                if (PassiveDictionary.TryGetValue(hero.ChampionName, out passiveInfo))
                {
                    try
                    {
                        result += passiveInfo.GetPassivesSum(hero);
                    }
                    catch (Exception)
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            $"Failure to calculate one or more passives at {hero.ChampionName}.");
                    }
                }
            }

            var targetHero = target as Obj_AI_Hero;
            if (targetHero != null)
            {
                // Ninja tabi
                if (Items.HasItem(3047, targetHero))
                {
                    k *= 0.9d;
                }

                // Nimble Fighter
                if (targetHero.ChampionName == "Fizz")
                {
                    reduction += 4 + (targetHero.Level - 1 / 3) * 2;
                }

                // Block
                // + Reduces incoming damage from champion basic attacks by 1 / 2
                if (hero != null)
                {
                    var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68);
                    if (mastery != null && mastery.Points >= 1)
                    {
                        reduction += 1 * mastery.Points;
                    }
                }
            }

            // This formula is right, work out the math yourself if you don't believe me
            return source.CalculatePhysicalDamage(target, (result - reduction) * k);
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
            int stage = 0)
        {
            if (source == null || !source.IsValid || target == null || !target.IsValid
                || source.Spellbook.GetSpell(spellSlot).Level == 0)
            {
                return 0d;
            }

            DamageInfo value;
            if (DamagesDictionary.TryGetValue(source.ChampionName, out value))
            {
                var func = value.GetSpellFunc(spellSlot, stage);
                if (func != null)
                {
                    try
                    {
                        return func(source, target);
                    }
                    catch (Exception)
                    {
                        // This condition can't happen. Call the police or something.
                        return 0d;
                    }
                }
            }

            return 0d;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="Damage" /> class.
        /// </summary>
        /// <param name="gameVersion">
        ///     The client version.
        /// </param>
        internal static void Initialize(Version gameVersion)
        {
            Load.OnLoad += (sender, args) =>
                {
                    var version = gameVersion.Major + "." + gameVersion.Minor;
                    IDictionary<string, JToken> damageFile = null;

                    byte[] value;
                    if (DamageFiles.TryGetValue(version, out value))
                    {
                        Version = version;
                        damageFile = JObject.Parse(Encoding.Default.GetString(value));
                    }
                    else
                    {
                        var sysVersion = new Version(version);
                        foreach (var file in from file in DamageFiles
                                             let fileVersion = new Version(file.Key)
                                             where
                                                 sysVersion.Major == fileVersion.Major
                                                 && sysVersion.Minor == fileVersion.Minor
                                             select file)
                        {
                            Version = file.Key;
                            damageFile = JObject.Parse(Encoding.Default.GetString(file.Value));
                            break;
                        }

                        if (damageFile == null)
                        {
                            foreach (
                                var file in DamageFiles.Where(file => sysVersion.Major == new Version(file.Key).Major))
                            {
                                Version = file.Key;
                                damageFile = JObject.Parse(Encoding.Default.GetString(file.Value));
                                break;
                            }
                        }

                        if (damageFile == null)
                        {
                            var pair = DamageFiles.FirstOrDefault();
                            if (!string.IsNullOrEmpty(pair.Key))
                            {
                                Version = pair.Key;
                                damageFile = JObject.Parse(Encoding.Default.GetString(pair.Value));
                            }
                        }
                    }

                    if (damageFile == null)
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            "[GameVersion:{0}] No suitable damage library found, unable to load damages.",
                            Variables.GameVersion);
                        return;
                    }

                    CreateDamages(damageFile);
                };
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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculateMagicDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            double forceDamageModifier = 0d)
        {
            var magicResist = target.SpellBlock;

            // Penetration can't reduce magic resist below 0.
            double value;

            if (magicResist < 0)
            {
                value = 2 - 100 / (100 - magicResist);
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
                DamageType.Magical,
                forceDamageModifier) + source.PassiveFlatMod(target);
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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculatePhysicalDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            double forceDamageModifier = 0d)
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
                value = 2 - 100 / (100 - armor);
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
                DamageType.Physical,
                forceDamageModifier) + source.PassiveFlatMod(target);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void CreateDamages(IDictionary<string, JToken> damages)
        {
            var generator = new DamageGenerator();

            foreach (var hero in GameObjects.Heroes)
            {
                JToken championToken;
                if (damages.TryGetValue(hero.ChampionName, out championToken))
                {
                    if (generator.IsClassTaken(hero.ChampionName))
                    {
                        continue;
                    }

                    var @class = generator.AddClass(hero.ChampionName);
                    var championTokenCollection = (IDictionary<string, JToken>)championToken;

                    JToken varToken;
                    if (championTokenCollection.TryGetValue("Vars", out varToken))
                    {
                        var varTokenCollection = (IDictionary<string, JToken>)varToken;
                        foreach (var item in varTokenCollection)
                        {
                            switch (item.Value.Type)
                            {
                                case JTokenType.Array:
                                    var itemArray = (JArray)item.Value;
                                    if (itemArray.Count > 0)
                                    {
                                        if (Regex.IsMatch((string)(itemArray[0]), @"[0-9]*\.[0-9]*"))
                                        {
                                            if (((string)(itemArray[0])).Length <= 9)
                                            {
                                                // float
                                                @class.AddField(
                                                    item.Key,
                                                    itemArray.Select(colItem => (float)colItem).ToArray());
                                            }
                                            else
                                            {
                                                // double
                                                @class.AddField(
                                                    item.Key,
                                                    itemArray.Select(colItem => (double)colItem).ToArray());
                                            }
                                        }
                                        else if (Regex.IsMatch((string)(itemArray[0]), @"^[0-9]*$"))
                                        {
                                            // integer
                                            @class.AddField(
                                                item.Key,
                                                itemArray.Select(colItem => (int)colItem).ToArray());
                                        }
                                    }
                                    break;
                                case JTokenType.Float:
                                    @class.AddField(item.Key, (float)item.Value);
                                    break;
                                case JTokenType.Integer:
                                    @class.AddField(item.Key, (int)item.Value);
                                    break;
                                case JTokenType.String:
                                    @class.AddField(item.Key, (string)item.Value);
                                    break;
                            }
                        }
                    }

                    JToken passiveToken;
                    if (championTokenCollection.TryGetValue("Passives", out passiveToken))
                    {
                        var passiveTokenCollection = (IDictionary<string, JToken>)passiveToken;
                        foreach (var passiveInfo in passiveTokenCollection)
                        {
                            var syslink = generator.AddPassiveLink(hero.ChampionName, passiveInfo.Key);
                            var inlineFunction = "return 0d;";

                            if (passiveInfo.Value.Type == JTokenType.Array && ((JArray)passiveInfo.Value).HasValues)
                            {
                                inlineFunction = ((JArray)passiveInfo.Value).Aggregate(
                                    string.Empty,
                                    (current, item) => current + item);
                            }
                            else
                            {
                                Logging.Write()(
                                    LogLevel.Fatal,
                                    $"Invalid configuration {hero.ChampionName}->PASSIVE->{passiveInfo.Key}.");
                            }

                            @class.AddFunction(
                                $"public static double {syslink.Function}(Obj_AI_Base source)",
                                inlineFunction);
                        }
                    }

                    foreach (var entry in championTokenCollection)
                    {
                        SpellSlot spellSlot;
                        if (Enum.TryParse(entry.Key, out spellSlot))
                        {
                            var spellToken = (IDictionary<string, JToken>)entry.Value;
                            int stages;
                            if (spellToken.ContainsKey("Stages")
                                && int.TryParse(spellToken["Stages"].ToString(), out stages))
                            {
                                for (var i = 0; i < stages; ++i)
                                {
                                    JToken stageToken;
                                    if (spellToken.TryGetValue("Stage" + i, out stageToken))
                                    {
                                        var syslink = generator.AddLink(hero.ChampionName, spellSlot, i);
                                        var inlineFunction = "return 0d;";

                                        if (stageToken.Type == JTokenType.Array && ((JArray)stageToken).HasValues)
                                        {
                                            inlineFunction = ((JArray)stageToken).Aggregate(
                                                string.Empty,
                                                (current, item) => current + (string)item);
                                        }
                                        else
                                        {
                                            Logging.Write()(
                                                LogLevel.Fatal,
                                                $"Invalid configuration {hero.ChampionName}->{spellSlot}->{i}.");
                                        }

                                        @class.AddFunction(
                                            $"public static double {syslink.Function}(Obj_AI_Base source, Obj_AI_Base target)",
                                            inlineFunction);
                                    }
                                    else
                                    {
                                        Logging.Write()(
                                            LogLevel.Fatal,
                                            $"Missing configuration {hero.ChampionName}->{spellSlot}->{i}.");
                                    }
                                }
                            }
                            else
                            {
                                Logging.Write()(
                                    LogLevel.Fatal,
                                    $"Invalid configuration {hero.ChampionName}->{spellSlot} at 'Stages'.");
                            }
                        }
                    }
                }
                else
                {
                    Logging.Write()(LogLevel.Fatal, "No entry for champion {0} in damage file.", hero.ChampionName);
                }
            }

            var assembly = generator.Compile();
            if (assembly != null)
            {
                var links = generator.GetLinks(assembly);

                foreach (var link in links)
                {
                    DamageInfo damageInfo;
                    if (!DamagesDictionary.TryGetValue(link.Key, out damageInfo))
                    {
                        damageInfo = new DamageInfo(link.Key);
                        DamagesDictionary.Add(link.Key, damageInfo);
                    }

                    foreach (var spellSlot in link.Value)
                    {
                        foreach (var stage in spellSlot.Value)
                        {
                            damageInfo.AddSpell(spellSlot.Key, stage.Key, stage.Value);
                        }
                    }
                }

                var passiveLinks = generator.GetPassiveLinks(assembly);
                foreach (var link in passiveLinks)
                {
                    PassiveInfo passiveInfo;
                    if (!PassiveDictionary.TryGetValue(link.Key, out passiveInfo))
                    {
                        passiveInfo = new PassiveInfo(link.Key);
                        PassiveDictionary.Add(link.Key, passiveInfo);
                    }

                    foreach (var passive in link.Value)
                    {
                        passiveInfo.AddPassive(passive.Key, passive.Value);
                    }
                }
            }
            else
            {
                Logging.Write()(LogLevel.Fatal, "Unable to compile damage class.");
            }
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
        /// <param name="forceReduction">
        ///     The forced reduction modifier.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double DamageReductionMod(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            DamageType damageType,
            double forceReduction = 0d)
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

            amount *= 1.0d - forceReduction;

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
                        && target.Health / target.MaxHealth <= 0.05d + 0.15d * mastery.Points)
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

        /// <summary>
        ///     The damage info container.
        /// </summary>
        public struct DamageInfo
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DamageInfo" /> struct.
            /// </summary>
            /// <param name="championName">
            ///     The champion name
            /// </param>
            public DamageInfo(string championName)
            {
                this.ChampionName = championName;
                this.Spells = new Dictionary<SpellSlot, DamageSpellInfo>();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the champion name.
            /// </summary>
            public string ChampionName { get; }

            #endregion

            #region Properties

            private IDictionary<SpellSlot, DamageSpellInfo> Spells { get; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Adds a spell towards the damage info.
            /// </summary>
            /// <param name="spellSlot">
            ///     The spell slot
            /// </param>
            /// <param name="stage">
            ///     The stage
            /// </param>
            /// <param name="func">
            ///     The function
            /// </param>
            public void AddSpell(SpellSlot spellSlot, int stage, Func<Obj_AI_Base, Obj_AI_Base, double> func)
            {
                DamageSpellInfo damageSpellInfo;
                if (this.Spells.TryGetValue(spellSlot, out damageSpellInfo))
                {
                    damageSpellInfo.AddComponent(stage, func);
                }
                else
                {
                    damageSpellInfo = new DamageSpellInfo(this) { Components = { { stage, func } } };
                    this.Spells.Add(spellSlot, damageSpellInfo);
                }
            }

            /// <summary>
            ///     Gets the spell function.
            /// </summary>
            /// <param name="spellSlot">
            ///     The spell slot
            /// </param>
            /// <param name="stage">
            ///     The spell stage
            /// </param>
            /// <returns>
            ///     The delegate created function to execute the calculation.
            /// </returns>
            public Func<Obj_AI_Base, Obj_AI_Base, double> GetSpellFunc(SpellSlot spellSlot, int stage = 0)
            {
                DamageSpellInfo value;
                if (this.Spells.TryGetValue(spellSlot, out value))
                {
                    Func<Obj_AI_Base, Obj_AI_Base, double> func;
                    if (value.Components.TryGetValue(stage, out func))
                    {
                        return func;
                    }
                }

                return null;
            }

            #endregion

            internal struct DamageSpellInfo
            {
                #region Constructors and Destructors

                public DamageSpellInfo(DamageInfo parentInstance)
                {
                    this.ParentInstance = parentInstance;
                    this.Components = new Dictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>();
                }

                #endregion

                #region Public Properties

                public IDictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>> Components { get; }

                public DamageInfo ParentInstance { get; }

                #endregion

                #region Public Methods and Operators

                public void AddComponent(int stage, Func<Obj_AI_Base, Obj_AI_Base, double> func)
                {
                    this.Components.Add(stage, func);
                }

                #endregion
            }
        }

        /// <summary>
        ///     The passive info container.
        /// </summary>
        public struct PassiveInfo
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PassiveInfo" /> struct.
            /// </summary>
            /// <param name="championName">
            ///     The champion name.
            /// </param>
            public PassiveInfo(string championName)
            {
                this.ChampionName = championName;
                this.Passives = new Dictionary<string, Func<Obj_AI_Base, double>>();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the champion name.
            /// </summary>
            public string ChampionName { get; }

            #endregion

            #region Properties

            /// <summary>
            ///     The passive dictionary.
            /// </summary>
            private IDictionary<string, Func<Obj_AI_Base, double>> Passives { get; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Adds passive data towards the champion passive info.
            /// </summary>
            /// <param name="passiveName">
            ///     The passive name
            /// </param>
            /// <param name="passiveFunc">
            ///     The passive function
            /// </param>
            public void AddPassive(string passiveName, Func<Obj_AI_Base, double> passiveFunc)
            {
                this.Passives.Add(passiveName, passiveFunc);
            }

            /// <summary>
            ///     Gets all of the passives from the passive info.
            /// </summary>
            /// <returns>
            ///     The readonly dictionary which contains all of the passives.
            /// </returns>
            public IReadOnlyDictionary<string, Func<Obj_AI_Base, double>> GetPassives()
            {
                return (IReadOnlyDictionary<string, Func<Obj_AI_Base, double>>)this.Passives;
            }

            /// <summary>
            ///     Calculates all of the passive extra damage.
            /// </summary>
            /// <param name="source">
            ///     The source
            /// </param>
            /// <returns>
            ///     The
            ///     <see cref="double" /> sum
            /// </returns>
            public double GetPassivesSum(Obj_AI_Base source)
            {
                return this.Passives.Sum(passive => passive.Value(source));
            }

            #endregion
        }
    }

    internal class DamageGenerator
    {
        #region Constructors and Destructors

        public DamageGenerator()
        {
            this.Code.AppendLine("using System;\nusing System.Linq;\nusing LeagueSharp;\n");
            this.Code.AppendLine("namespace LeagueSharp.Dynamic\n{");

            // This is crap code but it's 5 a.m. and I need to get this working.
            this.AddClass("Damage")
                .AddFunction(
                    "public static double CalculateDamage(Obj_AI_Base source, Obj_AI_Base target, DamageType damageType, double amount, double forceDamageModifier = 0)",
                    "if(source == null || !source.IsValid || target == null || !target.IsValid) { return 0d; } var damage = 0d; switch (damageType) { case DamageType.Magical: damage = CalculateMagicDamage(source, target, amount, forceDamageModifier); break; case DamageType.Physical: damage = CalculatePhysicalDamage(source, target, amount, forceDamageModifier); break; case DamageType.True: damage = amount; break; } return Math.Max(damage, 0d);")
                .AddFunction(
                    "public static double CalculateMixedDamage(Obj_AI_Base source, Obj_AI_Base target, double physicalAmount, double magicalAmount)",
                    "return CalculateMagicDamage(source, target, magicalAmount) + CalculatePhysicalDamage(source, target, physicalAmount);")
                .AddFunction(
                    "private static double CalculateMagicDamage(Obj_AI_Base source, Obj_AI_Base target, double amount, double forceDamageModifier = 0)",
                    "var magicResist = target.SpellBlock; double value; if (magicResist < 0) { value = 2 - 100 / (100 - magicResist); } else if ((magicResist * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod < 0) { value = 1; } else { value = 100 / (100 + (magicResist * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod); } return DamageReductionMod(source, target, PassivePercentMod(source, target, value) * amount, DamageType.Magical, forceDamageModifier) + PassiveFlatMod(source, target);")
                .AddFunction(
                    "private static double CalculatePhysicalDamage(Obj_AI_Base source, Obj_AI_Base target, double amount, double forceDamageModifier = 0)",
                    "double armorPenetrationPercent = source.PercentArmorPenetrationMod; double armorPenetrationFlat = source.FlatArmorPenetrationMod; if (source is Obj_AI_Minion) { armorPenetrationFlat = 0d; armorPenetrationPercent = 1d; } var armor = target.Armor; double value; if (armor < 0) { value = 2 - 100 / (100 - armor); } else if ((armor * armorPenetrationPercent) - armorPenetrationFlat < 0) { value = 1; } else { value = 100 / (100 + (armor * armorPenetrationPercent) - armorPenetrationFlat); } return DamageReductionMod(source, target, PassivePercentMod(source, target, value) * amount, DamageType.Physical, forceDamageModifier) + PassiveFlatMod(source, target);")
                .AddFunction(
                    "private static double DamageReductionMod(Obj_AI_Base source, Obj_AI_Base target, double amount, DamageType damageType, double forceReduction = 0d)",
                    "if (source is Obj_AI_Hero) { if (source.HasBuff(\"Exhaust\")) { amount /= 0.4d; } } var targetHero = target as Obj_AI_Hero; if (targetHero != null) { if (targetHero.InventoryItems.Any(slot => slot.Id == (ItemId)1054)) { amount -= 8; } if (target.HasBuff(\"Ferocious Howl\")) { amount *= 0.3d; } if (target.HasBuff(\"Tantrum\") && damageType == DamageType.Physical) { amount -= new[] { 2, 4, 6, 8, 10 }[target.Spellbook.GetSpell(SpellSlot.E).Level - 1]; } if (target.HasBuff(\"BraumShieldRaise\")) { amount -= amount * new[] { 0.3d, 0.325d, 0.35d, 0.375d, 0.4d }[ target.Spellbook.GetSpell(SpellSlot.E).Level - 1]; } if (target.HasBuff(\"GalioIdolOfDurand\")) { amount *= 0.5d; } if (target.HasBuff(\"GarenW\")) { amount *= 0.7d; } if (target.HasBuff(\"GragasWSelf\")) { amount -= amount * new[] { 0.1d, 0.12d, 0.14d, 0.16d, 0.18d }[ target.Spellbook.GetSpell(SpellSlot.W).Level - 1]; } if (target.HasBuff(\"VoidStone\") && damageType == DamageType.Magical) { amount *= 0.85d; } if (target.HasBuff(\"KatarinaEReduction\")) { amount *= 0.85d; } if (target.HasBuff(\"MaokaiDrainDefense\") && !(source is Obj_AI_Turret)) { amount *= 0.8d; } if (target.HasBuff(\"Meditate\")) { amount -= amount * new[] { 0.5d, 0.55d, 0.6d, 0.65d, 0.7d }[ target.Spellbook.GetSpell(SpellSlot.W).Level - 1] / (source is Obj_AI_Turret ? 2 : 1); } if (target.HasBuff(\"PoppyValiantFighter\") && !(source is Obj_AI_Turret) && amount / target.Health > 0.1d) { amount *= 0.5d; } if (target.HasBuff(\"Shen Shadow Dash\") && source.HasBuff(\"Taunt\") && damageType == DamageType.Physical) { amount *= 0.5d; } if (target.HasBuff(\"YorickUnholySymbiosis\")) { amount -= amount * ObjectManager.Get<AttackableUnit>().Count(g => g.IsEnemy && (g.Name.Equals(\"Clyde\") || g.Name.Equals(\"Inky\") || g.Name.Equals(\"Blinky\"))) * 0.05d; } } amount *= 1.0d - forceReduction; return amount;")
                .AddFunction(
                    "private static double PassivePercentMod(Obj_AI_Base source, Obj_AI_Base target, double amount)",
                    "var hero = source as Obj_AI_Hero; var targetHero = target as Obj_AI_Hero; if (hero != null) { if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1)) { amount *= hero.IsMelee ? 1.02d : 1.015d; } if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 146 && m.Points == 1)) { amount *= 1.03d; } if (targetHero != null) { var mastery = hero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Offense && m.Id == 100); if (mastery != null && mastery.Points >= 1 && target.Health / target.MaxHealth <= 0.05d + 0.15d * mastery.Points) { amount *= 1.05d; } } } if (targetHero != null) { if (targetHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1)) { amount *= targetHero.IsMelee ? 1.01d : 1.015d; } } return amount;")
                .AddFunction(
                    "private static double PassiveFlatMod(Obj_AI_Base source, Obj_AI_Base target)",
                    "var value = 0d; var hero = source as Obj_AI_Hero; var targetHero = target as Obj_AI_Hero; if (hero != null && target is Obj_AI_Minion) { if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 68 && m.Points == 1)) { value += 2d; } } if (source is Obj_AI_Minion && targetHero != null && source.Team == GameObjectTeam.Neutral) { var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68); if (mastery != null && mastery.Points >= 1) { value -= 1 * mastery.Points; } } if (hero != null && targetHero != null) { var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 81); if (mastery != null && mastery.Points == 1) { value -= targetHero.IsMelee ? 2 : 1; } } return value;");
        }

        #endregion

        #region Public Properties

        public List<Syslink> FunctionSyslinks { get; } = new List<Syslink>();

        #endregion

        #region Properties

        private List<ClassExtension> Class { get; } = new List<ClassExtension>();

        private StringBuilder Code { get; } = new StringBuilder();

        #endregion

        #region Public Methods and Operators

        public ClassExtension AddClass(string @class)
        {
            var generatedClass = new ClassExtension(@class);
            this.Class.Add(generatedClass);
            return generatedClass;
        }

        public Syslink AddLink(string championName, SpellSlot spellSlot, int stage)
        {
            var value = new Syslink(championName, spellSlot, stage);
            this.FunctionSyslinks.Add(value);
            return value;
        }

        public Syslink AddPassiveLink(string championName, string passiveName)
        {
            var value = new Syslink(championName, passiveName);
            this.FunctionSyslinks.Add(value);
            return value;
        }

        public Assembly Compile()
        {
            Assembly assembly = null;

            MetadataReference[] references =
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(GameObject).Assembly.Location)
                };

            var compilation = CSharpCompilation.Create(
                "LeagueSharp.Dynamic",
                new[] { CSharpSyntaxTree.ParseText(this.ToString()) },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Magic. Do not touch.
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures =
                        result.Diagnostics.Where(
                            diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Logging.Write()(LogLevel.Fatal, $"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    assembly = Assembly.Load(ms.ToArray());
                }
            }

            return assembly;
        }

        public Dictionary<string, IDictionary<SpellSlot, IDictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>>>
            GetLinks(Assembly assembly)
        {
            var collection =
                new Dictionary<string, IDictionary<SpellSlot, IDictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>>>
                    ();

            // If this comment is removed the library will blow up.
            foreach (var syslink in this.FunctionSyslinks.Where(syslink => string.IsNullOrEmpty(syslink.PassiveName)))
            {
                var type = assembly.GetType($"LeagueSharp.Dynamic.{syslink.ChampionName}");
                if (type == null)
                {
                    // This should never happen.
                    Logging.Write()(
                        LogLevel.Fatal,
                        $"Unable to find the correct type from assembly. (LeagueSharp.Dynamic.{syslink.ChampionName})");
                    continue;
                }

                var method = type.GetMethod(syslink.Function);
                if (method == null)
                {
                    // This should never happen either.
                    Logging.Write()(LogLevel.Fatal, $"Unable to find the method from type. ({syslink.Function})");
                    continue;
                }

                if (collection.ContainsKey(syslink.ChampionName))
                {
                    if (collection[syslink.ChampionName].ContainsKey(syslink.SpellSlot))
                    {
                        collection[syslink.ChampionName][syslink.SpellSlot].Add(
                            syslink.Stage,
                            (Func<Obj_AI_Base, Obj_AI_Base, double>)
                            Delegate.CreateDelegate(typeof(Func<Obj_AI_Base, Obj_AI_Base, double>), method));
                    }
                    else
                    {
                        collection[syslink.ChampionName].Add(
                            syslink.SpellSlot,
                            new Dictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>());
                        collection[syslink.ChampionName][syslink.SpellSlot].Add(
                            syslink.Stage,
                            (Func<Obj_AI_Base, Obj_AI_Base, double>)
                            Delegate.CreateDelegate(typeof(Func<Obj_AI_Base, Obj_AI_Base, double>), method));
                    }
                }
                else
                {
                    collection.Add(
                        syslink.ChampionName,
                        new Dictionary<SpellSlot, IDictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>>());
                    collection[syslink.ChampionName].Add(
                        syslink.SpellSlot,
                        new Dictionary<int, Func<Obj_AI_Base, Obj_AI_Base, double>>());
                    collection[syslink.ChampionName][syslink.SpellSlot].Add(
                        syslink.Stage,
                        (Func<Obj_AI_Base, Obj_AI_Base, double>)
                        Delegate.CreateDelegate(typeof(Func<Obj_AI_Base, Obj_AI_Base, double>), method));
                }
            }

            return collection;
        }

        public Dictionary<string, IDictionary<string, Func<Obj_AI_Base, double>>> GetPassiveLinks(Assembly assembly)
        {
            var collection = new Dictionary<string, IDictionary<string, Func<Obj_AI_Base, double>>>();

            // If this comment is removed the library will blow up.
            foreach (var syslink in this.FunctionSyslinks.Where(syslink => !string.IsNullOrEmpty(syslink.PassiveName)))
            {
                var type = assembly.GetType($"LeagueSharp.Dynamic.{syslink.ChampionName}");
                if (type == null)
                {
                    // This should never happen.
                    Logging.Write()(
                        LogLevel.Fatal,
                        $"Unable to find the correct type from assembly. (LeagueSharp.Dynamic.{syslink.ChampionName})");
                    continue;
                }

                var method = type.GetMethod(syslink.Function);
                if (method == null)
                {
                    // This should never happen either.
                    Logging.Write()(LogLevel.Fatal, $"Unable to find the method from type. ({syslink.Function})");
                    continue;
                }

                if (collection.ContainsKey(syslink.ChampionName))
                {
                    if (!collection[syslink.ChampionName].ContainsKey(syslink.PassiveName))
                    {
                        collection[syslink.ChampionName].Add(
                            syslink.PassiveName,
                            (Func<Obj_AI_Base, double>)
                            Delegate.CreateDelegate(typeof(Func<Obj_AI_Base, double>), method));
                    }
                    else
                    {
                        // Okay, this should never happen, I swear.
                        Logging.Write()(LogLevel.Error, $"Duplicate value '{syslink.PassiveName}'");
                    }
                }
                else
                {
                    collection.Add(syslink.ChampionName, new Dictionary<string, Func<Obj_AI_Base, double>>());
                    collection[syslink.ChampionName].Add(
                        syslink.PassiveName,
                        (Func<Obj_AI_Base, double>)Delegate.CreateDelegate(typeof(Func<Obj_AI_Base, double>), method));
                }
            }

            return collection;
        }

        public bool IsClassTaken(string @class)
        {
            return this.Class.Any(c => c.ClassName.Equals(@class));
        }

        public override string ToString()
        {
            return this.Code + this.Class.Aggregate(string.Empty, (current, @class) => current + @class) + "}";
        }

        #endregion

        public struct Syslink
        {
            #region Constructors and Destructors

            public Syslink(string championName, SpellSlot spellSlot, int stage)
            {
                this.ChampionName = championName;
                this.SpellSlot = spellSlot;
                this.Stage = stage;
                this.PassiveName = string.Empty;
            }

            public Syslink(string championName, string passiveName)
            {
                this.ChampionName = championName;
                this.SpellSlot = SpellSlot.Unknown;
                this.Stage = 0;
                this.PassiveName = passiveName;
            }

            #endregion

            #region Public Properties

            public string ChampionName { get; }

            public string Function
                =>
                    string.IsNullOrEmpty(this.PassiveName)
                        ? this.ChampionName + this.SpellSlot + this.Stage
                        : (this.ChampionName + this.PassiveName).Replace(" ", string.Empty);

            public string PassiveName { get; }

            public SpellSlot SpellSlot { get; }

            public int Stage { get; }

            #endregion
        }

        public class ClassExtension
        {
            #region Constructors and Destructors

            public ClassExtension(string @class)
            {
                this.ClassName = @class;
                this.Code.AppendLine($"        public class {@class}\n        {{");
            }

            #endregion

            #region Public Properties

            public string ClassName { get; }

            public StringBuilder Code { get; } = new StringBuilder();

            public StringBuilder Field { get; } = new StringBuilder();

            public StringBuilder Function { get; } = new StringBuilder();

            #endregion

            #region Public Methods and Operators

            public ClassExtension AddField(string fieldName, object fieldValue)
            {
                var type = fieldValue.GetType();
                var typeString = type.ToString();
                var value = fieldValue;

                if (type == typeof(int[]))
                {
                    value = ((int[])value).Aggregate<int, object>(
                        "new[] {",
                        (current, item) => (current + (item + ", "))) + "}";
                    typeString = "int[]";
                }
                else if (type == typeof(float[]))
                {
                    value = ((float[])value).Aggregate<float, object>(
                        "new[] {",
                        (current, item) => (current + (item + "F, "))) + "}";
                    typeString = "float[]";
                }
                else if (type == typeof(double[]))
                {
                    value = ((double[])value).Aggregate<double, object>(
                        "new[] {",
                        (current, item) => (current + (item + "D, "))) + "}";
                    typeString = "double[]";
                }
                else if (type == typeof(float))
                {
                    value += "F";
                }
                else if (type == typeof(double))
                {
                    value += "D";
                }

                this.Field.AppendLine($"            public static {typeString} {fieldName} = {value};");
                return this;
            }

            public ClassExtension AddFunction(string function, string inlineFunction)
            {
                this.Function.AppendLine(
                    $"            {function}\n            {{\n                {inlineFunction}\n            }}");
                return this;
            }

            public override string ToString()
            {
                return this.Code.ToString() + this.Field + this.Function + "\n        }\n";
            }

            #endregion
        }
    }
}

#else

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Damage.cs" company="LeagueSharp">
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
//   Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Permissions;
    using System.Text;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Properties;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static class Damage
    {
        #region Static Fields

        /// <summary>
        ///     The damage version files.
        /// </summary>
        private static readonly IDictionary<string, byte[]> DamageFiles = new Dictionary<string, byte[]>
                                                                              {
                                                                                  { "5.15.0.333", Resources._5_15_0_333 }
                                                                              };

        /// <summary>
        ///     The damages dictionary.
        /// </summary>
        private static readonly IDictionary<string, IDictionary<SpellSlot, IDictionary<int, SpellDamage>>>
            DamagesDictionary = new Dictionary<string, IDictionary<SpellSlot, IDictionary<int, SpellDamage>>>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the version.
        /// </summary>
        public static string Version { get; private set; }

        #endregion

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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            DamageType damageType,
            double amount,
            double forceDamageModifier = 0d)
        {
            var damage = 0d;
            switch (damageType)
            {
                case DamageType.Magical:
                    damage = source.CalculateMagicDamage(target, amount, forceDamageModifier);
                    break;
                case DamageType.Physical:
                    damage = source.CalculatePhysicalDamage(target, amount, forceDamageModifier);
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

            if (!includePassive)
            {
                return source.CalculatePhysicalDamage(target, result);
            }

            var k = 1d;
            var reduction = 0d;

            var hero = source as Obj_AI_Hero;
            if (hero != null)
            {
                // Spoils of War
                var minionTarget = target as Obj_AI_Minion;
                if (hero.IsMelee() && minionTarget != null && minionTarget.IsEnemy
                    && minionTarget.Team != GameObjectTeam.Neutral
                    && hero.Buffs.Any(buff => buff.Name == "talentreaperdisplay" && buff.Count > 0))
                {
                    if (
                        GameObjects.Heroes.Any(
                            h =>
                            h.NetworkId != source.NetworkId && h.Team == source.Team
                            && h.Distance(minionTarget.Position) < 1100))
                    {
                        var value = 0;

                        if (Items.HasItem(3302, hero))
                        {
                            value = 200; // Relic Shield
                        }
                        else if (Items.HasItem(3097, hero))
                        {
                            value = 240; // Targon's Brace
                        }
                        else if (Items.HasItem(3401, hero))
                        {
                            value = 400; // Face of the Mountain
                        }

                        return value + hero.TotalAttackDamage;
                    }
                }

                // BotRK
                if (Items.HasItem(3153, hero))
                {
                    var d = 0.08 * target.Health;
                    if (target is Obj_AI_Minion)
                    {
                        d = Math.Min(d, 60);
                    }

                    result += d;
                }

                // Arcane blade
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 132 && m.Points == 1))
                {
                    reduction -= CalculateMagicDamage(hero, target, 0.05 * hero.FlatMagicDamageMod);
                }
            }

            var targetHero = target as Obj_AI_Hero;
            if (targetHero != null)
            {
                // Ninja tabi
                if (Items.HasItem(3047, targetHero))
                {
                    k *= 0.9d;
                }

                // Nimble Fighter
                if (targetHero.ChampionName == "Fizz")
                {
                    int f;

                    if (targetHero.Level >= 1 && targetHero.Level < 4)
                    {
                        f = 4;
                    }
                    else if (targetHero.Level >= 4 && targetHero.Level < 7)
                    {
                        f = 6;
                    }
                    else if (targetHero.Level >= 7 && targetHero.Level < 10)
                    {
                        f = 8;
                    }
                    else if (targetHero.Level >= 10 && targetHero.Level < 13)
                    {
                        f = 10;
                    }
                    else if (targetHero.Level >= 13 && targetHero.Level < 16)
                    {
                        f = 12;
                    }
                    else
                    {
                        f = 14;
                    }

                    reduction += f;
                }

                // Block
                // + Reduces incoming damage from champion basic attacks by 1 / 2
                if (hero != null)
                {
                    var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68);
                    if (mastery != null && mastery.Points >= 1)
                    {
                        reduction += 1 * mastery.Points;
                    }
                }
            }

            return source.CalculatePhysicalDamage(target, (result - reduction) * k);
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
            int stage = 0)
        {
            var instance = source.GetSpellDamageInstance(spellSlot, stage);
            return instance != null ? instance.GetDamage(source, target) : 0d;
        }

        /// <summary>
        ///     Gets the spell damage instance.
        /// </summary>
        /// <param name="champion">
        ///     The champion
        /// </param>
        /// <param name="spellSlot">
        ///     The spell slot
        /// </param>
        /// <param name="stage">
        ///     The stage
        /// </param>
        /// <returns>
        ///     <see cref="SpellDamage" /> instance.
        /// </returns>
        public static SpellDamage GetSpellDamageInstance(this Obj_AI_Hero champion, SpellSlot spellSlot, int stage = 0)
        {
            return GetSpellDamageInstance(champion.ChampionName, spellSlot, stage);
        }

        /// <summary>
        ///     Gets the spell damage instance.
        /// </summary>
        /// <param name="champion">
        ///     The champion name
        /// </param>
        /// <param name="spellSlot">
        ///     The spell slot
        /// </param>
        /// <param name="stage">
        ///     The stage
        /// </param>
        /// <returns>
        ///     <see cref="SpellDamage" /> instance.
        /// </returns>
        public static SpellDamage GetSpellDamageInstance(string champion, SpellSlot spellSlot, int stage = 0)
        {
            IDictionary<SpellSlot, IDictionary<int, SpellDamage>> championCollection;
            if (DamagesDictionary.TryGetValue(champion, out championCollection) && championCollection != null)
            {
                IDictionary<int, SpellDamage> spellCollection;
                if (championCollection.TryGetValue(spellSlot, out spellCollection) && spellCollection != null)
                {
                    SpellDamage spellDamage;
                    if (spellCollection.TryGetValue(stage, out spellDamage) && spellDamage != null)
                    {
                        return spellDamage;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="Damage" /> class.
        /// </summary>
        /// <param name="gameVersion">
        ///     The client version.
        /// </param>
        internal static void Initialize(Version gameVersion)
        {
            var version = gameVersion.ToString();
            Load.OnLoad += (sender, args) =>
                {
                    IDictionary<string, JToken> damageFile = null;

                    byte[] value;
                    if (DamageFiles.TryGetValue(version, out value))
                    {
                        Version = version;
                        damageFile = JObject.Parse(Encoding.Default.GetString(value));
                    }
                    else
                    {
                        var sysVersion = new Version(version);
                        foreach (var file in from file in DamageFiles
                                             let fileVersion = new Version(file.Key)
                                             where
                                                 sysVersion.Major == fileVersion.Major
                                                 && sysVersion.Minor == fileVersion.Minor
                                             select file)
                        {
                            Version = file.Key;
                            damageFile = JObject.Parse(Encoding.Default.GetString(file.Value));
                            break;
                        }

                        if (damageFile == null)
                        {
                            foreach (
                                var file in DamageFiles.Where(file => sysVersion.Major == new Version(file.Key).Major))
                            {
                                Version = file.Key;
                                damageFile = JObject.Parse(Encoding.Default.GetString(file.Value));
                                break;
                            }
                        }

                        if (damageFile == null)
                        {
                            var pair = DamageFiles.FirstOrDefault();
                            if (!string.IsNullOrEmpty(pair.Key))
                            {
                                Version = pair.Key;
                                damageFile = JObject.Parse(Encoding.Default.GetString(pair.Value));
                            }
                        }
                    }

                    if (damageFile == null)
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            "[GameVersion:{0}] No suitable damage library found, unable to load damages.",
                            Variables.GameVersion);
                        return;
                    }

                    JToken damagesToken;
                    if (damageFile.TryGetValue("Damages", out damagesToken))
                    {
                        foreach (var champion in (IDictionary<string, JToken>)damagesToken)
                        {
                            CreateSpells(champion.Key, (IDictionary<string, JToken>)champion.Value);
                        }
                    }
                    else
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            "[{0}] No suitable damage category exists in damage library.",
                            Version);
                    }
                };
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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculateMagicDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            double forceDamageModifier = 0d)
        {
            var magicResist = target.SpellBlock;

            // Penetration can't reduce magic resist below 0.
            double value;

            if (magicResist < 0)
            {
                value = 2 - 100 / (100 - magicResist);
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
                DamageType.Magical,
                forceDamageModifier) + source.PassiveFlatMod(target);
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
        /// <param name="forceDamageModifier">
        ///     The forced damage modifier
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculatePhysicalDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            double forceDamageModifier = 0d)
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
                value = 2 - 100 / (100 - armor);
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
                DamageType.Physical,
                forceDamageModifier) + source.PassiveFlatMod(target);
        }

        /// <summary>
        ///     Creates the given loaded spells for the selected champion given if the champion exists in game.
        /// </summary>
        /// <param name="champion">
        ///     The champion name.
        /// </param>
        /// <param name="spells">
        ///     The spells collection.
        /// </param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void CreateSpells(string champion, IDictionary<string, JToken> spells)
        {
            var gameChampionExists = GameObjects.Heroes.Any(h => h.ChampionName == champion);
            if (!gameChampionExists)
            {
                return;
            }

            var spellDamages = new Dictionary<SpellSlot, IDictionary<int, SpellDamage>>();

            foreach (var spell in spells)
            {
                SpellSlot spellSlot;
                if (Enum.TryParse(spell.Key, out spellSlot))
                {
                    var spellData = (IDictionary<string, JToken>)spell.Value;
                    if (!spellData.ContainsKey("Stages"))
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            "[{0}->{1}->{2}] Invalid configuration for spell.",
                            Version,
                            champion,
                            spellSlot);
                        continue;
                    }

                    int stages;
                    if (int.TryParse(spellData["Stages"].ToString(), out stages))
                    {
                        var stagesInfo = new Dictionary<int, SpellDamage>();

                        for (var stage = 0; stage < stages; ++stage)
                        {
                            var stageData = spellData["Stage" + stage].ToString();
                            stagesInfo.Add(
                                stage,
                                new SpellDamage(JsonConvert.DeserializeObject<SpellDamageData>(stageData))
                                    { SData = { Slot = spellSlot } });
                        }

                        spellDamages.Add(spellSlot, stagesInfo);
                    }
                    else
                    {
                        Logging.Write()(
                            LogLevel.Fatal,
                            "[{0}->{1}->{2}] Invalid configuration for spell.",
                            Version,
                            champion,
                            spellSlot);
                    }
                }
            }

            DamagesDictionary.Add(champion, spellDamages);
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
        /// <param name="forceReduction">
        ///     The forced reduction modifier.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double DamageReductionMod(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            double amount,
            DamageType damageType,
            double forceReduction = 0d)
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

            amount *= 1.0d - forceReduction;

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
                        && target.Health / target.MaxHealth <= 0.05d + 0.15d * mastery.Points)
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

    /// <summary>
    ///     The spell damage information class container.
    /// </summary>
    public class SpellDamage
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellDamage" /> class.
        /// </summary>
        /// <param name="sdata">
        ///     The spell damage data.
        /// </param>
        [SuppressMessage("ReSharper", "FunctionComplexityOverflow",
            Justification = "Reviewed. Suppression is OK here and does not hurt function nor performance.")]
        internal SpellDamage(SpellDamageData sdata)
        {
            this.SData = sdata;
            this.GetDamageFunc = (@base, aiBase) =>
                {
                    if (@base.Spellbook.GetSpell(sdata.Slot).Level == 0)
                    {
                        return 0d;
                    }

                    var spellLevel = sdata.Flags.HasFlag(DamageFlags.LevelScale)
                                         ? @base.Spellbook.GetSpell(sdata.LevelScale).Level - 1
                                         : @base.Spellbook.GetSpell(sdata.Slot).Level - 1;

                    var damage = 0d;
                    var abilityPowerDamageAmount = 0d;
                    var attackDamageAmount = 0d;

                    SpellSlot spellSlot;
                    int intValue;
                    var alternativePassive = sdata.Flags.HasFlag(DamageFlags.SpecialPassiveAlternative)
                                             && ((sdata.AlternativePassive.Length == 1
                                                  && @base.HasBuff(sdata.AlternativePassive[0]))
                                                 || (sdata.AlternativePassive.Length == 2
                                                     && Enum.TryParse(sdata.AlternativePassive[0], out spellSlot)
                                                     && int.TryParse(sdata.AlternativePassive[1], out intValue)
                                                     && @base.Spellbook.GetSpell(spellSlot).ToggleState == intValue));

                    if (sdata.Base != null && sdata.Base.Length > 0)
                    {
                        if (sdata.Flags.HasFlag(DamageFlags.SpecialDistance)
                            && @base.Distance(aiBase) > sdata.DistanceOffset)
                        {
                            damage += sdata.BaseDistance[Math.Min(spellLevel, sdata.BaseDistance.Length - 1)];
                        }
                        else
                        {
                            if (sdata.BaseMinion != null && sdata.BaseMinion.Length > 0 && aiBase is Obj_AI_Minion)
                            {
                                damage += sdata.BaseMinion[Math.Min(spellLevel, sdata.BaseMinion.Length - 1)];
                            }
                            else
                            {
                                if (alternativePassive)
                                {
                                    damage +=
                                        sdata.BaseAlternative[Math.Min(spellLevel, sdata.BaseAlternative.Length - 1)];
                                }
                                else
                                {
                                    damage += sdata.Base[Math.Min(spellLevel, sdata.Base.Length - 1)];
                                }
                            }
                        }
                    }

                    if (sdata.TargetBase != null && sdata.TargetBase.Length > 0 && aiBase is Obj_AI_Hero)
                    {
                        damage += sdata.TargetBase[Math.Min(spellLevel, sdata.TargetBase.Length - 1)];
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.AbilityPower))
                    {
                        var value = @base.TotalMagicalDamage
                                    * (aiBase is Obj_AI_Minion && sdata.AbilityPowerMinion > float.Epsilon
                                           ? sdata.AbilityPowerMinion
                                           : sdata.AbilityPower);

                        if (alternativePassive)
                        {
                            value = @base.TotalMagicalDamage * sdata.AlternativeAbilityPower;
                        }

                        damage += value;
                        abilityPowerDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.AttackDamage))
                    {
                        var value = @base.TotalAttackDamage
                                    * (aiBase is Obj_AI_Minion && sdata.AttackDamageMinion > float.Epsilon
                                           ? sdata.AttackDamageMinion
                                           : sdata.AttackDamage);

                        if (alternativePassive)
                        {
                            value = @base.TotalAttackDamage * sdata.AlternativeAttackDamage;
                        }

                        damage += value;
                        attackDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BonusAttackDamage))
                    {
                        var value = (sdata.Flags.HasFlag(DamageFlags.SpecialDistance)
                                     && @base.Distance(aiBase) > sdata.DistanceOffset)
                                        ? @base.FlatPhysicalDamageMod * sdata.DistanceBonusAttackDamage
                                        : @base.FlatPhysicalDamageMod * sdata.BonusAttackDamage;

                        if (alternativePassive)
                        {
                            value = @base.FlatPhysicalDamageMod * sdata.AlternativeBonusAttackDamage;
                        }

                        damage += value;
                        attackDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.EnemyMaxHealth) && sdata.EnemyMaxHealthBase != null
                        && sdata.EnemyMaxHealthBase.Length > 0)
                    {
                        damage += aiBase.MaxHealth
                                  * sdata.EnemyMaxHealthBase[Math.Min(spellLevel, sdata.EnemyMaxHealthBase.Length - 1)];
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.AbilityPowerEnemyMaxHealth))
                    {
                        damage += @base.TotalMagicalDamage * sdata.AbilityPowerEnemyMaxHealth;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseAttackDamagePercent)
                        && ((sdata.BaseAttackDamagePercent != null && sdata.BaseAttackDamagePercent.Length > 0)
                            || (sdata.BaseMinionAttackDamagePercent != null
                                && sdata.BaseMinionAttackDamagePercent.Length > 0)))
                    {
                        var value = @base.TotalAttackDamage
                                    * (aiBase is Obj_AI_Minion && sdata.BaseMinionAttackDamagePercent != null
                                       && sdata.BaseMinionAttackDamagePercent.Length > 0
                                           ? sdata.BaseMinionAttackDamagePercent[
                                               Math.Min(spellLevel, sdata.BaseMinionAttackDamagePercent.Length - 1)]
                                           : sdata.BaseAttackDamagePercent[
                                               Math.Min(spellLevel, sdata.BaseAttackDamagePercent.Length - 1)]);
                        damage += value;
                        attackDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseAbilityPowerPercent)
                        && sdata.BaseAbilityPowerPercent != null && sdata.BaseAbilityPowerPercent.Length > 0)
                    {
                        var value = @base.TotalMagicalDamage
                                    * sdata.BaseAbilityPowerPercent[
                                        Math.Min(spellLevel, sdata.BaseAbilityPowerPercent.Length - 1)];
                        damage += value;
                        abilityPowerDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseBonusAttackDamagePercent)
                        && sdata.BaseBonusAttackDamagePercent != null && sdata.BaseBonusAttackDamagePercent.Length > 0)
                    {
                        var value = @base.TotalAttackDamage
                                    * sdata.BaseBonusAttackDamagePercent[
                                        Math.Min(spellLevel, sdata.BaseBonusAttackDamagePercent.Length - 1)];
                        damage += value;
                        attackDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseChampionLevel) && sdata.BaseChampionLevel != null
                        && sdata.BaseChampionLevel.Length > 0)
                    {
                        damage += sdata.BaseChampionLevel[Math.Min(@base.Level, sdata.BaseChampionLevel.Length - 1)];
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.MaxHealth))
                    {
                        damage += @base.MaxHealth * sdata.MaxHealth;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.TargetHealth))
                    {
                        var baseTargetHealth = alternativePassive
                                                   ? sdata.BaseTargetHealth
                                                   : sdata.BaseAlternativeTargetHealth;
                        var targetHealthBaseAbilityPowerScale = alternativePassive
                                                                    ? sdata.TargetHealthBaseAbilityPowerScale
                                                                    : sdata.AlternativeTargetHealthBaseAbilityPowerScale;
                        var targetHealthBaseMinimumDamage = alternativePassive
                                                                ? sdata.TargetHealthBaseMinimumDamage
                                                                : sdata.AlternativeTargetHealthBaseMinimumDamage;
                        var minionHealthBaseMaximumDamage = alternativePassive
                                                                ? sdata.MinionHealthBaseMaximumDamage
                                                                : sdata.AlternativeMinionHealthBaseMaximumDamage;

                        var flagDamage = 0f;

                        if (baseTargetHealth != null && baseTargetHealth.Length > 0)
                        {
                            flagDamage += aiBase.Health
                                          * baseTargetHealth[Math.Min(spellLevel, baseTargetHealth.Length - 1)];
                        }

                        if (targetHealthBaseAbilityPowerScale != null && targetHealthBaseAbilityPowerScale.Length > 0)
                        {
                            float value;
                            switch (targetHealthBaseAbilityPowerScale.Length)
                            {
                                case 1:
                                    value = aiBase.Health * targetHealthBaseAbilityPowerScale[0];
                                    flagDamage += value;
                                    abilityPowerDamageAmount += value;
                                    break;
                                case 2:
                                    if (@base.TotalMagicalDamage / targetHealthBaseAbilityPowerScale[1] >= 1f)
                                    {
                                        value = aiBase.Health
                                                * ((@base.TotalMagicalDamage / targetHealthBaseAbilityPowerScale[1])
                                                   * targetHealthBaseAbilityPowerScale[0]);
                                        flagDamage += value;
                                        abilityPowerDamageAmount += value;
                                    }

                                    break;
                            }
                        }

                        if (targetHealthBaseMinimumDamage != null && targetHealthBaseMinimumDamage.Length > 0)
                        {
                            flagDamage =
                                Math.Max(
                                    targetHealthBaseMinimumDamage[
                                        Math.Min(spellLevel, targetHealthBaseMinimumDamage.Length - 1)],
                                    flagDamage);
                        }

                        if (minionHealthBaseMaximumDamage != null && minionHealthBaseMaximumDamage.Length > 0
                            && aiBase is Obj_AI_Minion)
                        {
                            flagDamage =
                                Math.Min(
                                    minionHealthBaseMaximumDamage[
                                        Math.Min(spellLevel, minionHealthBaseMaximumDamage.Length - 1)],
                                    flagDamage);
                        }

                        damage += flagDamage;

                        flagDamage = 0f;
                        var baseTargetMissingHealth = alternativePassive
                                                          ? sdata.BaseTargetMissingHealth
                                                          : sdata.BaseAlternativeTargetMissingHealth;
                        var targetMissingHealthBaseAbilityPowerScale = alternativePassive
                                                                           ? sdata
                                                                                 .TargetMissingHealthBaseAbilityPowerScale
                                                                           : sdata
                                                                                 .AlternativeTargetMissingHealthBaseAbilityPowerScale;
                        var targetMissingHealthBaseMinimumDamage = alternativePassive
                                                                       ? sdata.TargetMissingHealthBaseMinimumDamage
                                                                       : sdata
                                                                             .AlternativeTargetMissingHealthBaseMinimumDamage;
                        var minionMissingHealthBaseMaximumDamage = alternativePassive
                                                                       ? sdata.MinionMissingHealthBaseMaximumDamage
                                                                       : sdata
                                                                             .AlternativeMinionMissingHealthBaseMaximumDamage;
                        var missingHealth = aiBase.MaxHealth - aiBase.Health;

                        if (baseTargetMissingHealth != null && baseTargetMissingHealth.Length > 0)
                        {
                            flagDamage += missingHealth
                                          * baseTargetMissingHealth[
                                              Math.Min(spellLevel, baseTargetMissingHealth.Length - 1)];
                        }

                        if (targetMissingHealthBaseAbilityPowerScale != null
                            && targetMissingHealthBaseAbilityPowerScale.Length > 0)
                        {
                            float value;
                            switch (targetMissingHealthBaseAbilityPowerScale.Length)
                            {
                                case 1:
                                    value = missingHealth * targetMissingHealthBaseAbilityPowerScale[0];
                                    flagDamage += value;
                                    abilityPowerDamageAmount += value;
                                    break;
                                case 2:
                                    if (@base.TotalMagicalDamage / targetMissingHealthBaseAbilityPowerScale[1] >= 1f)
                                    {
                                        value = missingHealth
                                                * ((@base.TotalMagicalDamage
                                                    / targetMissingHealthBaseAbilityPowerScale[1])
                                                   * targetMissingHealthBaseAbilityPowerScale[0]);
                                        flagDamage += value;
                                        abilityPowerDamageAmount += value;
                                    }

                                    break;
                            }
                        }

                        if (targetMissingHealthBaseMinimumDamage != null
                            && targetMissingHealthBaseMinimumDamage.Length > 0)
                        {
                            flagDamage =
                                Math.Max(
                                    targetMissingHealthBaseMinimumDamage[
                                        Math.Min(spellLevel, targetMissingHealthBaseMinimumDamage.Length - 1)],
                                    flagDamage);
                        }

                        if (minionMissingHealthBaseMaximumDamage != null
                            && minionMissingHealthBaseMaximumDamage.Length > 0 && aiBase is Obj_AI_Minion)
                        {
                            flagDamage =
                                Math.Min(
                                    minionMissingHealthBaseMaximumDamage[
                                        Math.Min(spellLevel, minionMissingHealthBaseMaximumDamage.Length - 1)],
                                    flagDamage);
                        }

                        damage += flagDamage;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.SpecialPassive))
                    {
                        if (sdata.TargetPassiveIdentifier != null && sdata.TargetPassiveIdentifier.Length > 0)
                        {
                            var passiveName = sdata.TargetPassiveIdentifier[0];
                            int rawTempValue;
                            var passiveCountPrefix = sdata.TargetPassiveIdentifier.Length > 1
                                                     && int.TryParse(sdata.TargetPassiveIdentifier[1], out rawTempValue)
                                                         ? rawTempValue
                                                         : 0;

                            var enemyPassiveCount = aiBase.GetBuffCount(passiveName) - passiveCountPrefix;
                            if (enemyPassiveCount > 0)
                            {
                                if (sdata.BaseTargetPassive != null && sdata.BaseTargetPassive.Length > 0)
                                {
                                    damage += enemyPassiveCount
                                              * sdata.BaseTargetPassive[
                                                  Math.Min(spellLevel, sdata.BaseTargetPassive.Length - 1)];
                                }

                                if (sdata.BaseTargetPassiveAttackDamage != null
                                    && sdata.BaseTargetPassiveAttackDamage.Length > 0)
                                {
                                    damage += enemyPassiveCount
                                              * sdata.BaseTargetPassiveAttackDamage[
                                                  Math.Min(spellLevel, sdata.BaseTargetPassiveAttackDamage.Length - 1)]
                                              * @base.TotalAttackDamage;
                                }
                            }
                        }

                        if (sdata.PassiveIdentifier != null && sdata.PassiveIdentifier.Length > 0)
                        {
                            var passiveName = sdata.PassiveIdentifier[0];
                            int rawTempValue;
                            var passiveCountPrefix = sdata.PassiveIdentifier.Length > 1
                                                     && int.TryParse(sdata.PassiveIdentifier[1], out rawTempValue)
                                                         ? rawTempValue
                                                         : 0;

                            var passiveCount = @base.GetBuffCount(passiveName) - passiveCountPrefix;
                            var maxMana = sdata.PassiveIdentifierMaxMana + @base.MaxMana;

                            damage += passiveCount * maxMana;
                        }
                    }

                    var trueDamage = 0d;
                    if (alternativePassive)
                    {
                        trueDamage += damage * sdata.AlternativeTrueDamage;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.MaxMana))
                    {
                        damage += @base.MaxMana * sdata.MaxMana;
                    }

                    var forceIgnoreArmor = -sdata.TargetArmorIgnore;

                    return (sdata.Type == DamageType.Mixed
                                ? @base.CalculateMixedDamage(aiBase, attackDamageAmount, abilityPowerDamageAmount)
                                : @base.CalculateDamage(aiBase, sdata.Type, damage, forceIgnoreArmor)) + trueDamage;
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the spell damage data.
        /// </summary>
        public SpellDamageData SData { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the damage calculation function.
        /// </summary>
        private Func<Obj_AI_Hero, Obj_AI_Base, double> GetDamageFunc { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the spell damage.
        /// </summary>
        /// <param name="source">
        ///     The Source
        /// </param>
        /// <param name="target">
        ///     The Target
        /// </param>
        /// <returns>
        ///     The damage in <see cref="double" /> value.
        /// </returns>
        public double GetDamage(Obj_AI_Hero source, Obj_AI_Base target)
        {
            return (target == null || !target.IsValid) ? 0d : this.GetDamageFunc(source, target);
        }

        #endregion
    }

    /// <summary>
    ///     The spell damage data.
    /// </summary>
    public class SpellDamageData
    {
        #region Public Properties

        /// <summary>
        ///     Gets the ability power.
        /// </summary>
        [JsonProperty("AP")]
        public float AbilityPower { get; internal set; }

        /// <summary>
        ///     Gets the ability power enemy max health.
        /// </summary>
        [JsonProperty("enemyMaxHealthAP")]
        public float AbilityPowerEnemyMaxHealth { get; internal set; }

        /// <summary>
        ///     Gets the ability power minion.
        /// </summary>
        [JsonProperty("APMinion")]
        public float AbilityPowerMinion { get; internal set; }

        /// <summary>
        ///     Gets the alternative ability power.
        /// </summary>
        [JsonProperty("AlternativeAP")]
        public float AlternativeAbilityPower { get; internal set; }

        /// <summary>
        ///     Gets the alternative attack damage.
        /// </summary>
        [JsonProperty("AlternativeAD")]
        public float AlternativeAttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative bonus attack damage.
        /// </summary>
        [JsonProperty("AlternativeBonusAD")]
        public float AlternativeBonusAttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative minion health base maximum damage.
        /// </summary>
        [JsonProperty("minionAlternativeHealthBaseMax")]
        public float[] AlternativeMinionHealthBaseMaximumDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative minion missing health base maximum damage.
        /// </summary>
        [JsonProperty("minionAlternativeMissingHealthBaseMax")]
        public float[] AlternativeMinionMissingHealthBaseMaximumDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative passive.
        /// </summary>
        [JsonProperty("baseAlternativePassiveIdentifier")]
        public string[] AlternativePassive { get; internal set; }

        /// <summary>
        ///     Gets the alternative target health base ability power scale.
        /// </summary>
        [JsonProperty("targetAlternativeHealthBaseAPScale")]
        public float[] AlternativeTargetHealthBaseAbilityPowerScale { get; internal set; }

        /// <summary>
        ///     Gets the alternative target health base minimum damage.
        /// </summary>
        [JsonProperty("targetAlternativeHealthBaseMin")]
        public float[] AlternativeTargetHealthBaseMinimumDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative target missing health base ability power scale.
        /// </summary>
        [JsonProperty("targetAlternativeMissingHealthBaseAPScale")]
        public float[] AlternativeTargetMissingHealthBaseAbilityPowerScale { get; internal set; }

        /// <summary>
        ///     Gets the alternative target missing health base minimum damage.
        /// </summary>
        [JsonProperty("targetAlternativeMissingHealthBaseMin")]
        public float[] AlternativeTargetMissingHealthBaseMinimumDamage { get; internal set; }

        /// <summary>
        ///     Gets the alternative true damage.
        /// </summary>
        [JsonProperty("AlternativeTrueDamage")]
        public float AlternativeTrueDamage { get; internal set; }

        /// <summary>
        ///     Gets the attack damage.
        /// </summary>
        [JsonProperty("AD")]
        public float AttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the ability power minion.
        /// </summary>
        [JsonProperty("ADMinion")]
        public float AttackDamageMinion { get; internal set; }

        /// <summary>
        ///     Gets the base.
        /// </summary>
        [JsonProperty("base")]
        public float[] Base { get; internal set; }

        /// <summary>
        ///     Gets the base ability power percent.
        /// </summary>
        [JsonProperty("baseAPPercent")]
        public float[] BaseAbilityPowerPercent { get; internal set; }

        /// <summary>
        ///     Gets the base alternative.
        /// </summary>
        [JsonProperty("baseAlternative")]
        public float[] BaseAlternative { get; internal set; }

        /// <summary>
        ///     Gets the base target health.
        /// </summary>
        [JsonProperty("targetAlternativeHealthBase")]
        public float[] BaseAlternativeTargetHealth { get; internal set; }

        /// <summary>
        ///     Gets the base target health.
        /// </summary>
        [JsonProperty("targetAlternativeMissingHealthBase")]
        public float[] BaseAlternativeTargetMissingHealth { get; internal set; }

        /// <summary>
        ///     Gets the base attack damage percent.
        /// </summary>
        [JsonProperty("baseADPercent")]
        public float[] BaseAttackDamagePercent { get; internal set; }

        /// <summary>
        ///     Gets the base bonus attack damage percent.
        /// </summary>
        [JsonProperty("baseBonusADPercent")]
        public float[] BaseBonusAttackDamagePercent { get; internal set; }

        /// <summary>
        ///     Gets the base champion level.
        /// </summary>
        [JsonProperty("baseChampLevel")]
        public float[] BaseChampionLevel { get; internal set; }

        /// <summary>
        ///     Gets the base distance.
        /// </summary>
        [JsonProperty("distanceBase")]
        public float[] BaseDistance { get; internal set; }

        /// <summary>
        ///     Gets the base minion.
        /// </summary>
        [JsonProperty("baseMinion")]
        public float[] BaseMinion { get; internal set; }

        /// <summary>
        ///     Gets the base minion attack damage percent.
        /// </summary>
        [JsonProperty("baseADPercentMinion")]
        public float[] BaseMinionAttackDamagePercent { get; internal set; }

        /// <summary>
        ///     Gets the base target health.
        /// </summary>
        [JsonProperty("targetHealthBase")]
        public float[] BaseTargetHealth { get; internal set; }

        /// <summary>
        ///     Gets the base target health.
        /// </summary>
        [JsonProperty("targetMissingHealthBase")]
        public float[] BaseTargetMissingHealth { get; internal set; }

        /// <summary>
        ///     Gets the base target passive.
        /// </summary>
        [JsonProperty("targetPassiveBase")]
        public float[] BaseTargetPassive { get; internal set; }

        /// <summary>
        ///     Gets the base target passive attack damage.
        /// </summary>
        [JsonProperty("targetPassiveADBase")]
        public float[] BaseTargetPassiveAttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the bonus attack damage.
        /// </summary>
        [JsonProperty("bonusAD")]
        public float BonusAttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the distance bonus attack damage.
        /// </summary>
        [JsonProperty("distanceBonusAD")]
        public float DistanceBonusAttackDamage { get; internal set; }

        /// <summary>
        ///     Gets the distance offset.
        /// </summary>
        [JsonProperty("distanceOffset")]
        public float DistanceOffset { get; internal set; }

        /// <summary>
        ///     Gets the enemy max health base.
        /// </summary>
        [JsonProperty("enemyMaxHealthBase")]
        public float[] EnemyMaxHealthBase { get; internal set; }

        /// <summary>
        ///     Gets the flags.
        /// </summary>
        [JsonProperty("flags")]
        public DamageFlags Flags { get; internal set; }

        /// <summary>
        ///     Gets the level scale.
        /// </summary>
        [JsonProperty("baseLevelScale")]
        public SpellSlot LevelScale { get; internal set; }

        /// <summary>
        ///     Gets the max health.
        /// </summary>
        [JsonProperty("maxHealth")]
        public float MaxHealth { get; internal set; }

        /// <summary>
        ///     Gets the max <c>mana</c>.
        /// </summary>
        [JsonProperty("maxMana")]
        public float MaxMana { get; internal set; }

        /// <summary>
        ///     Gets the minion health base maximum damage.
        /// </summary>
        [JsonProperty("minionHealthBaseMax")]
        public float[] MinionHealthBaseMaximumDamage { get; internal set; }

        /// <summary>
        ///     Gets the minion missing health base maximum damage.
        /// </summary>
        [JsonProperty("minionMissingHealthBaseMax")]
        public float[] MinionMissingHealthBaseMaximumDamage { get; internal set; }

        /// <summary>
        ///     Gets the passive identifier.
        /// </summary>
        [JsonProperty("passiveIdentifier")]
        public string[] PassiveIdentifier { get; internal set; }

        /// <summary>
        ///     Gets the passive identifier max <c>mana</c>.
        /// </summary>
        [JsonProperty("passiveIdentifierMaxMana")]
        public float PassiveIdentifierMaxMana { get; internal set; }

        /// <summary>
        ///     Gets the slot.
        /// </summary>
        public SpellSlot Slot { get; internal set; }

        /// <summary>
        ///     Gets or sets the target armor ignore.
        /// </summary>
        [JsonProperty("targetArmorIgnore")]
        public float TargetArmorIgnore { get; set; }

        /// <summary>
        ///     Gets or sets the target base.
        /// </summary>
        [JsonProperty("targetBase")]
        public float[] TargetBase { get; set; }

        /// <summary>
        ///     Gets the target health base ability power scale.
        /// </summary>
        [JsonProperty("targetHealthBaseAPScale")]
        public float[] TargetHealthBaseAbilityPowerScale { get; internal set; }

        /// <summary>
        ///     Gets the target health base minimum damage.
        /// </summary>
        [JsonProperty("targetHealthBaseMin")]
        public float[] TargetHealthBaseMinimumDamage { get; internal set; }

        /// <summary>
        ///     Gets the target missing health base ability power scale.
        /// </summary>
        [JsonProperty("targetMissingHealthBaseAPScale")]
        public float[] TargetMissingHealthBaseAbilityPowerScale { get; internal set; }

        /// <summary>
        ///     Gets the target missing health base minimum damage.
        /// </summary>
        [JsonProperty("targetMissingHealthBaseMin")]
        public float[] TargetMissingHealthBaseMinimumDamage { get; internal set; }

        /// <summary>
        ///     Gets the target passive identifier.
        /// </summary>
        [JsonProperty("targetPassiveIdentifier")]
        public string[] TargetPassiveIdentifier { get; internal set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        [JsonProperty("type")]
        public DamageType Type { get; internal set; }

        #endregion
    }
}

#endif