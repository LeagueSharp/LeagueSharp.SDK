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
                                                                                  { "5.13.0.323", Resources._5_13_0_323 }
                                                                              };

        /// <summary>
        ///     The damages dictionary.
        /// </summary>
        private static readonly IDictionary<string, IDictionary<SpellSlot, IDictionary<int, SpellDamage>>> DamagesDictionary = new Dictionary<string, IDictionary<SpellSlot, IDictionary<int, SpellDamage>>>();

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
                    var mastery = targetHero.Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 65);
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
        /// <param name="version">
        ///     The client version.
        /// </param>
        internal static void Initialize(string version)
        {
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
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculateMagicDamage(this Obj_AI_Base source, Obj_AI_Base target, double amount)
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

            var damage = source.DamageReductionMod(
                target,
                source.PassivePercentMod(target, value) * amount,
                DamageType.Magical) + source.PassiveFlatMod(target);

            return damage;
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
                        armorPenetrationPercent = 0.3d;
                        break;
                    case TurretType.TierThree:
                    case TurretType.TierFour:
                        armorPenetrationPercent = 0.7d;
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

            var damage = source.DamageReductionMod(
                target,
                source.PassivePercentMod(target, value) * amount,
                DamageType.Physical) + source.PassiveFlatMod(target);

            // Take into account the percent passives, flat passives and damage reduction.
            return damage;
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
                                    {
                                       SData = {
                                                    Slot = spellSlot 
                                                } 
                                    });
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
                    Console.WriteLine(amount);
                    amount *= 0.3d;
                    Console.WriteLine(amount);
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
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
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
        internal SpellDamage(SpellDamageData sdata)
        {
            this.SData = sdata;
            this.GetDamageFunc = (@base, aiBase) =>
                {
                    if (@base.Spellbook.GetSpell(sdata.Slot).Level == 0)
                    {
                        return 0d;
                    }

                    var spellLevel = @base.Spellbook.GetSpell(sdata.Slot).Level - 1;
                    var damage = 0d;
                    var abilityPowerDamageAmount = 0d;
                    var attackDamageAmount = 0d;

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
                                SpellSlot spellSlot;
                                int intValue;
                                if (sdata.Flags.HasFlag(DamageFlags.SpecialPassiveAlternative)
                                    && ((sdata.AlternativePassive.Length == 1
                                         && @base.HasBuff(sdata.AlternativePassive[0]))
                                        || (sdata.AlternativePassive.Length == 2
                                            && Enum.TryParse(sdata.AlternativePassive[0], out spellSlot)
                                            && int.TryParse(sdata.AlternativePassive[1], out intValue)
                                            && @base.Spellbook.GetSpell(spellSlot).ToggleState == intValue))
                                    && sdata.BaseAlternative != null && sdata.BaseAlternative.Length > 0)
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

                    if (sdata.Flags.HasFlag(DamageFlags.AbilityPower))
                    {
                        var value = @base.TotalMagicalDamage * sdata.AbilityPower
                                    + @base.TotalMagicalDamage * sdata.AbilityPowerMinion;
                        damage += value;
                        abilityPowerDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.AttackDamage))
                    {
                        var value = @base.TotalAttackDamage * sdata.AttackDamage;
                        damage += value;
                        attackDamageAmount += value;
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BonusAttackDamage))
                    {
                        var value = (sdata.Flags.HasFlag(DamageFlags.SpecialDistance)
                                     && @base.Distance(aiBase) > sdata.DistanceOffset)
                                        ? @base.FlatPhysicalDamageMod * sdata.DistanceBonusAttackDamage
                                        : @base.FlatPhysicalDamageMod * sdata.BonusAttackDamage;
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
                        && sdata.BaseAttackDamagePercent != null && sdata.BaseAttackDamagePercent.Length > 0)
                    {
                        damage += @base.TotalAttackDamage
                                  * sdata.BaseAttackDamagePercent[
                                      Math.Min(spellLevel, sdata.BaseAttackDamagePercent.Length - 1)];
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseAbilityPowerPercent)
                        && sdata.BaseAbilityPowerPercent != null && sdata.BaseAbilityPowerPercent.Length > 0)
                    {
                        damage += @base.TotalAttackDamage
                                  * sdata.BaseAbilityPowerPercent[
                                      Math.Min(spellLevel, sdata.BaseAbilityPowerPercent.Length - 1)];
                    }

                    if (sdata.Flags.HasFlag(DamageFlags.BaseBonusAttackDamagePercent)
                        && sdata.BaseAttackDamagePercent != null && sdata.BaseAttackDamagePercent.Length > 0)
                    {
                        damage += @base.TotalAttackDamage
                                  * sdata.BaseBonusAttackDamagePercent[
                                      Math.Min(spellLevel, sdata.BaseBonusAttackDamagePercent.Length - 1)];
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
                        SpellSlot spellSlot;
                        int intValue;
                        var flag = !sdata.Flags.HasFlag(DamageFlags.SpecialPassiveAlternative)
                                   || !((sdata.AlternativePassive.Length == 1
                                         && @base.HasBuff(sdata.AlternativePassive[0]))
                                        || (sdata.AlternativePassive.Length == 2
                                            && Enum.TryParse(sdata.AlternativePassive[0], out spellSlot)
                                            && int.TryParse(sdata.AlternativePassive[1], out intValue)
                                            && @base.Spellbook.GetSpell(spellSlot).ToggleState == intValue));

                        var baseTargetHealth = flag ? sdata.BaseTargetHealth : sdata.BaseAlternativeTargetHealth;
                        var targetHealthBaseAbilityPowerScale = flag
                                                                    ? sdata.TargetHealthBaseAbilityPowerScale
                                                                    : sdata.AlternativeTargetHealthBaseAbilityPowerScale;
                        var targetHealthBaseMinimumDamage = flag
                                                                ? sdata.TargetHealthBaseMinimumDamage
                                                                : sdata.AlternativeTargetHealthBaseMinimumDamage;
                        var minionHealthBaseMaximumDamage = flag
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
                        var baseTargetMissingHealth = flag
                                                          ? sdata.BaseTargetMissingHealth
                                                          : sdata.BaseAlternativeTargetMissingHealth;
                        var targetMissingHealthBaseAbilityPowerScale = flag
                                                                           ? sdata
                                                                                 .TargetMissingHealthBaseAbilityPowerScale
                                                                           : sdata
                                                                                 .AlternativeTargetMissingHealthBaseAbilityPowerScale;
                        var targetMissingHealthBaseMinimumDamage = flag
                                                                       ? sdata.TargetMissingHealthBaseMinimumDamage
                                                                       : sdata
                                                                             .AlternativeTargetMissingHealthBaseMinimumDamage;
                        var minionMissingHealthBaseMaximumDamage = flag
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

                    return sdata.Type == DamageType.Mixed
                               ? @base.CalculateMixedDamage(aiBase, attackDamageAmount, abilityPowerDamageAmount)
                               : @base.CalculateDamage(aiBase, sdata.Type, damage);
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
        ///     Gets the attack damage.
        /// </summary>
        [JsonProperty("AD")]
        public float AttackDamage { get; internal set; }

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
        ///     Gets the max health.
        /// </summary>
        [JsonProperty("maxHealth")]
        public float MaxHealth { get; internal set; }

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
        ///     Gets the slot.
        /// </summary>
        public SpellSlot Slot { get; internal set; }

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
        ///     Gets the type.
        /// </summary>
        [JsonProperty("type")]
        public DamageType Type { get; internal set; }

        #endregion
    }
}