using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Utils;
using Newtonsoft.Json.Linq;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Calculates damage.
    /// </summary>
    public static class Damage
    {
        /// <summary>
        ///     Delegate that gets the damage of the spell.
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="level">The level of the spell.</param>
        /// <returns>Damage of the spell.</returns>
        internal delegate double SpellDamageDelegate(Obj_AI_Base source, Obj_AI_Base target, int level);

        /// <summary>
        ///     Class that represents Passive Damages.
        /// </summary>
        internal class PassiveDamage
        {
            /// <summary>
            ///     Delegate that gets the damage.
            /// </summary>
            /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
            /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
            /// <returns>Damage</returns>
            public delegate float GetDamageD(Obj_AI_Hero source, Obj_AI_Base target);

            /// <summary>
            ///     Delegate that gets if the passive is active.
            /// </summary>
            /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
            /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
            /// <returns>If the passive is active.</returns>
            public delegate bool IsActiveD(Obj_AI_Hero source, Obj_AI_Base target);

            /// <summary>
            ///     Champion Name
            /// </summary>
            public string ChampionName = "";

            /// <summary>
            ///     <see cref="GetDamageD"/>
            /// </summary>
            public GetDamageD GetDamage;

            /// <summary>
            ///     <see cref="IsActiveD"/>
            /// </summary>
            public IsActiveD IsActive;
        }

        /// <summary>
        ///     Class that represents the damage a spell does.
        /// </summary>
        internal class DamageSpell
        {
            /// <summary>
            ///     <see cref="SpellDamageDelegate"/>
            /// </summary>
            public SpellDamageDelegate Damage;

            /// <summary>
            ///     The type of damage this spell deals.
            /// </summary>
            public DamageType DamageType;

            /// <summary>
            ///     The slot of the spell.
            /// </summary>
            public SpellSlot Slot;

            /// <summary>
            ///     The "stage" of the spell.
            /// </summary>
            public int Stage;
        }

        /// <summary>
        ///     Enum that contains items that deal damage.
        /// </summary>
        public enum DamageItems
        {
            /// <summary>
            ///     Hextech Gunblade
            /// </summary>
            HextechGunblade,

            /// <summary>
            ///     Blade of the Ruined King
            /// </summary>
            BladeOfTheRuinedKing,

            /// <summary>
            ///     Bilgewater Cutlass
            /// </summary>
            BilgewaterCutlass,

            /// <summary>
            ///     Tiamat
            /// </summary>
            Tiamat,

            /// <summary>
            ///     Ravenous Hydra
            /// </summary>
            RavenousHydra,

            /// <summary>
            ///     Black Fire Torch
            /// </summary>
            BlackFireTorch,

            /// <summary>
            ///     Odyn's Veil
            /// </summary>
            OdynsVeil,

            /// <summary>
            ///     Frost Queen's Claim
            /// </summary>
            FrostQueenClaim,

            /// <summary>
            ///     Liandry's Torment
            /// </summary>
            LiandrysTorment
        }

        /// <summary>
        ///     Enum that contains summoner spells that deal damage
        /// </summary>
        public enum SummonerSpell
        {
            /// <summary>
            ///     Ignite.
            /// </summary>
            Ignite,

            /// <summary>
            ///     Smite. Detects which smite the source has.
            /// </summary>
            Smite
        }

        /// <summary>
        ///     The DDragon JSON file.
        /// </summary>
        internal static JObject SpellDamageFile;

        /// <summary>
        ///     The JSON file that contains attack passives.
        /// </summary>
        internal static JObject AttackPassiveFile;

        static Damage()
        {
            
        }

        /// <summary>
        ///     Gets the damage of the spell against a target.
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="slot">Slot</param>
        /// <param name="stage">The "stage" of the spell.</param>
        /// <returns>The damage of the spell against a target.</returns>
        public static double GetSpellDamage(this Obj_AI_Hero source, Obj_AI_Base target, SpellSlot slot, int stage = 0)
        {
            // Recreate Dictionary from the JObject
            var spells = SpellDamageFile.ToObject<Dictionary<string, List<DamageSpell>>>();
            
            // Get the damage spell
            var damageSpell = spells[source.ChampionName].FirstOrDefault(x => x.Slot == slot && x.Stage == stage);

            if (damageSpell == null)
            {
                throw new ArgumentException("Could not find a spell with the specified slot and stage.");
            }

            return damageSpell.Damage(source, target, source.Spellbook.GetSpell(slot).Level);
        }

        /// <summary>
        ///     Gets the Auto Attack damage,
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="includePassive">Includes passives, such as Botrk, and masteries.</param>
        /// <returns>The calculated Auto Attack damage.</returns>
        public static double GetAutoAttackDamage(this Obj_AI_Base source,
            Obj_AI_Base target,
            bool includePassive = false)
        {
            // TODO: Include Attack Passives
            return CalculatePhysicalDamage(source, target, source.TotalAttackDamage) + 2;
        }

        /// <summary>
        ///     Gets the damage of an Item.
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="damageItem">Item</param>
        /// <returns>Damage of the item.</returns>
        public static double GetItemDamage(this Obj_AI_Hero source, Obj_AI_Base target, DamageItems damageItem)
        {
            switch (damageItem)
            {
                case DamageItems.BilgewaterCutlass:
                    return source.CalculateDamage(target, DamageType.Magical, 100);
                case DamageItems.BlackFireTorch:
                    return source.CalculateDamage(target, DamageType.Magical, target.MaxHealth * 0.2);
                case DamageItems.BladeOfTheRuinedKing:
                    return source.CalculateDamage(target, DamageType.Physcial, target.MaxHealth * 0.1);
                case DamageItems.FrostQueenClaim:
                    return source.CalculateDamage(target, DamageType.Magical, 50 + 5 * source.Level);
                case DamageItems.HextechGunblade:
                    return source.CalculateDamage(target, DamageType.Magical, 150 + 0.4 * source.FlatMagicDamageMod);
                case DamageItems.RavenousHydra:
                    return source.CalculateDamage(
                        target, DamageType.Physcial, source.BaseAttackDamage + source.FlatPhysicalDamageMod);
                case DamageItems.OdynsVeil:
                    return source.CalculateDamage(target, DamageType.Magical, 200);
                case DamageItems.Tiamat:
                    return source.CalculateDamage(
                        target, DamageType.Physcial, source.BaseAttackDamage + source.FlatPhysicalDamageMod);
                case DamageItems.LiandrysTorment:
                    var d = target.Health * .2f * 3f;
                    return (target.CanMove || target.HasBuffOfType(BuffType.Slow)) ? d : d * 2;
            }
            return 0d;
        }

        /// <summary>
        ///     Gets the summoner spell damage.
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="spell">
        ///     <see cref="SummonerSpell" />
        /// </param>
        /// <returns>Summoner spell damage</returns>
        public static double GetSummonerSpellDamage(this Obj_AI_Hero source, Obj_AI_Base target, SummonerSpell spell)
        {
            switch (spell)
            {
                case SummonerSpell.Ignite:
                    return 50 + 20 * source.Level - (target.HPRegenRate / 5 * 3);

                case SummonerSpell.Smite:
                    if (!(target is Obj_AI_Hero))
                    {
                        return
                            new double[]
                            {
                                390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000
                            }[source.Level - 1];
                    }

                    if (source.Spellbook.Spells.Any(x => x.Name == "s5_summonersmiteplayerganker"))
                    {
                        return 8 * source.Level + 20;
                    }

                    if (source.Spellbook.Spells.Any(x => x.Name == "s5_summonersmiteduel"))
                    {
                        return 6 * source.Level + 54;
                    }
                    break;
            }

            return 0d;
        }

        /// <summary>
        ///     Calculates the "real" damage done to a target.
        /// </summary>
        /// <param name="source"><see cref="Obj_AI_Base" /> where the damage is coming from.</param>
        /// <param name="target"><see cref="Obj_AI_Base" /> where the damage is going to.</param>
        /// <param name="damage">The amount of damage</param>
        /// <param name="damageType">Damage Type</param>
        /// <returns>The real damage done after calculations.</returns>
        public static double CalculateDamage(this Obj_AI_Base source,
            Obj_AI_Base target,
            DamageType damageType,
            double damage)
        {
            switch (damageType)
            {
                case DamageType.Physcial:
                    return CalculatePhysicalDamage(source, target, damage);
                case DamageType.Magical:
                    return CalculateMagicDamage(source, target, damage);
                case DamageType.TrueDamage:
                    return damage;
            }
            return 0d;
        }

        /// <summary>
        ///     Calculates Magical Damage to a target.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="target">Target</param>
        /// <param name="amount">Amount of damage</param>
        /// <returns>The "Real" Magical damage done to a target.</returns>
        private static double CalculateMagicDamage(Obj_AI_Base source, Obj_AI_Base target, double amount)
        {
            var magicResist = target.SpellBlock;

            //Penetration cant reduce magic resist below 0
            double k;
            if (magicResist < 0)
            {
                k = 2 - 100 / (100 - magicResist);
            }
            else if ((target.SpellBlock * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod < 0)
            {
                k = 1;
            }
            else
            {
                k = 100 /
                    (100 + (target.SpellBlock * source.PercentMagicPenetrationMod) - source.FlatMagicPenetrationMod);
            }

            //Take into account the percent passives
            k = PassivePercentMod(source, target, k);

            k = k * (1 - target.PercentMagicReduction) * (1 + target.PercentMagicDamageMod);

            return k * amount;
        }

        /// <summary>
        ///     Calculates Physical Damage to a target.
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="target">Target</param>
        /// <param name="amount">Amount of damage</param>
        /// <returns>The "Real" physical damage done to a target</returns>
        private static double CalculatePhysicalDamage(Obj_AI_Base source, Obj_AI_Base target, double amount)
        {
            double armorPenPercent = source.PercentArmorPenetrationMod;
            double armorPenFlat = source.FlatArmorPenetrationMod;

            //Minions return wrong percent values.
            if (source is Obj_AI_Minion)
            {
                armorPenFlat = 0;
                armorPenPercent = 1;
            }

            //Turrets passive.
            if (source is Obj_AI_Turret)
            {
                armorPenFlat = 0;
                armorPenPercent = 0.7f; //Penetrating Bullets passive.
            }

            //Penetration cant reduce armor below 0
            var armor = target.Armor;

            double k;
            if (armor < 0)
            {
                k = 2 - 100 / (100 - armor);
            }
            else if ((target.Armor * armorPenPercent) - armorPenFlat < 0)
            {
                k = 1;
            }
            else
            {
                k = 100 / (100 + (target.Armor * armorPenPercent) - armorPenFlat);
            }

            //Take into account the percent passives
            k = PassivePercentMod(source, target, k);

            return k * amount + PassiveFlatMod(source, target);
        }

        private static double PassiveFlatMod(Obj_AI_Base source, Obj_AI_Base target)
        {
            double d = 0;

            //Offensive masteries:

            //Butcher
            //  Basic attacks and single target abilities do 2 bonus damage to minions and monsters. 
            var hero = source as Obj_AI_Hero;
            if (hero != null && target is Obj_AI_Minion)
            {
                if (hero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
                {
                    d = d + 2;
                }
            }

            //Defensive masteries:

            //Block
            //Reduces incoming damage from champion basic attacks by 1 / 2
            if (source is Obj_AI_Hero && target is Obj_AI_Hero)
            {
                var mastery =
                    ((Obj_AI_Hero) target).Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 65);
                if (mastery != null && mastery.Points >= 1)
                {
                    d = d - 1 * mastery.Points;
                }
            }

            //Tough Skin
            //Reduces damage taken from neutral monsters by 1 / 2
            if (source is Obj_AI_Minion && target is Obj_AI_Hero && source.Team == GameObjectTeam.Neutral)
            {
                var mastery =
                    ((Obj_AI_Hero) target).Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 68);
                if (mastery != null && mastery.Points >= 1)
                {
                    d = d - 1 * mastery.Points;
                }
            }

            //Unyielding
            //Melee - Reduces all incoming damage from champions by 2
            //Ranged - Reduces all incoming damage from champions by 1
            if (source is Obj_AI_Hero && target is Obj_AI_Hero)
            {
                var mastery =
                    ((Obj_AI_Hero) target).Masteries.FirstOrDefault(m => m.Page == MasteryPage.Defense && m.Id == 81);
                if (mastery == null || mastery.Points != 1)
                {
                    return d;
                }

                if (source.IsMelee())
                {
                    d = d - 2;
                }
                else
                {
                    d = d - 1;
                }
            }

            return d;
        }

        private static double PassivePercentMod(Obj_AI_Base source, Obj_AI_Base target, double k)
        {
            var siegeMinionList = new List<string> { "Red_Minion_MechCannon", "Blue_Minion_MechCannon" };

            var normalMinionList = new List<string>
            {
                "Red_Minion_Wizard",
                "Blue_Minion_Wizard",
                "Red_Minion_Basic",
                "Blue_Minion_Basic"
            };

            //Minions and towers passives:
            if (source is Obj_AI_Turret)
            {
                //Siege minions receive 70% damage from turrets
                if (siegeMinionList.Contains(target.BaseSkinName))
                {
                    k = 0.7d * k;
                }

                //Normal minions take 114% more damage from towers.
                else if (normalMinionList.Contains(target.BaseSkinName))
                {
                    k = (1 / 0.875) * k;
                }

                // Turrets deal 105% damage to champions for the first attack.
                else if (target is Obj_AI_Hero)
                {
                    k = 1.05 * k;
                }
            }

            var hero = source as Obj_AI_Hero;
            if (hero != null)
            {
                var sourceAsHero = hero;

                //Double edge sword:
                //  Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
                //  Ranged champions: You deal and take 1.5% increased damage from all sources. 
                if (sourceAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
                {
                    if (sourceAsHero.CombatType == GameObjectCombatType.Melee)
                    {
                        k = k * 1.02d;
                    }
                    else
                    {
                        k = k * 1.015d;
                    }
                }

                //Havoc:
                //  Increases damage by 3%. 
                if (sourceAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 146 && m.Points == 1))
                {
                    k = k * 1.03d;
                }

                //Executioner
                //  Increases damage dealt to champions below 20 / 35 / 50% by 5%. 
                if (target is Obj_AI_Hero)
                {
                    var mastery =
                        (sourceAsHero).Masteries.FirstOrDefault(m => m.Page == MasteryPage.Offense && m.Id == 100);
                    if (mastery != null && mastery.Points >= 1 &&
                        target.Health / target.MaxHealth <= 0.05d + 0.15d * mastery.Points)
                    {
                        k = k * 1.05;
                    }
                }
            }


            if (!(target is Obj_AI_Hero))
            {
                return k;
            }

            var targetAsHero = (Obj_AI_Hero) target;

            //Double edge sword:
            //     Melee champions: You deal 2% increase damage from all sources, but take 1% increase damage from all sources.
            //     Ranged champions: You deal and take 1.5% increased damage from all sources. 
            if (!targetAsHero.Masteries.Any(m => m.Page == MasteryPage.Offense && m.Id == 65 && m.Points == 1))
            {
                return k;
            }

            if (target.CombatType == GameObjectCombatType.Melee)
            {
                k = k * 1.01d;
            }
            else
            {
                k = k * 1.015d;
            }

            return k;
        }


        /// <summary>
        ///     Converts the damages to JSON.
        /// </summary>
        internal static void LoadDamage()
        {
            var attackPassives = new List<PassiveDamage>();
            var spells = new Dictionary<string, List<DamageSpell>>();

            LoadAttackPassives(ref attackPassives);
            LoadSpells(ref spells);

            // Convert lists and spells into JObject's for damage
            AttackPassiveFile = JObject.FromObject(attackPassives);
            SpellDamageFile = JObject.FromObject(spells);
        }

        #region Spells
        private static void LoadSpells(ref Dictionary<string, List<DamageSpell>> spells)
        {
            spells.Add(
                "Aatrox", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 110, 145, 180, 215 }[level] + 0.6 * source.FlatMagicDamageMod +
                                0.6 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 300, 400 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Ahri", new List<DamageSpell>
                {
                    //Normal Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //Q Return
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //W => Magic Damage to the Same Target
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 64, 104, 144, 184, 224 }[level] + 0.64 * source.FlatMagicDamageMod
                    },
                    //W => 1 FF
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //R, per dash
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Akali", new List<DamageSpell>
                {
                    //Q Initial
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 35, 55, 75, 95, 115 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //Q Detonation
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 45, 70, 95, 120, 145 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 55, 80, 105, 130 }[level] + 0.3 * source.FlatMagicDamageMod +
                                0.6 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 175, 250 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Alistar", new List<DamageSpell>
                {
                    //Q Initial
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 110, 165, 220, 275 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Amumu", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //W - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 8, 12, 16, 20, 24 }[level] +
                                (new[] { 0.01, 0.015, 0.02, 0.025, 0.03 }[level] +
                                 0.01 * source.FlatMagicDamageMod / 100) * target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 100, 125, 150, 175 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Anivia", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //Q - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] * 2 + 1 * source.FlatMagicDamageMod
                    },
                    //E - not chilled targets - TODO
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 85, 115, 145, 175 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160 }[level] + 0.25 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Annie", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 115, 150, 185, 220 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.85 * source.FlatMagicDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 210, 335, 460 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Ashe", new List<DamageSpell>
                {
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 50, 60, 70, 80 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 425, 600 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R - Min
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 250, 425, 600 }[level] + 1 * source.FlatMagicDamageMod) / 2
                    },
                });

            spells.Add(
                "Azir", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 85, 105, 125, 145 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W - Soldier auto attacks
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                45 +
                                new double[]
                                { 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120, 130, 140, 150, 160, 170 }[
                                    ((Obj_AI_Hero) source).Level - 1] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 225, 300 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Blitzcrank", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 300 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage = (source, target, level) => 1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 375, 500 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Bard", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.50 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Brand", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.65 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 120, 165, 210, 255 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.55 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Braum", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.025 * source.MaxHealth
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Caitlyn", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 60, 100, 140, 180 }[level] +
                                1.3 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 475, 700 }[level] + 2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Cassiopeia", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 115, 155, 195, 235 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 15, 20, 25, 30 }[level] + 0.1 * source.FlatMagicDamageMod
                    },
                    //E 
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 80, 105, 130, 155 }[level] + 0.55 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "ChoGath", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 305 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 125, 175, 225, 275 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 35, 50, 65, 80 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new double[] { 300, 475, 650 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Corki", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.5 * source.FlatMagicDamageMod +
                                0.5 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 32, 44, 56, 68 }[level] + 0.4 * source.FlatPhysicalDamageMod
                    },
                    //R - Normal missile
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 180, 260 }[level] + 0.3 * source.FlatMagicDamageMod +
                                new double[] { 20, 30, 40 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - Big missile
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 270, 390 }[level] + 0.45 * source.FlatMagicDamageMod +
                                new double[] { 30, 40, 60 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                });

            spells.Add(
                "Darius", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.7 * source.FlatPhysicalDamageMod
                    },
                    //Q - Blade
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new[] { 105, 157.5, 210, 262.5, 315 }[level] + 1.05 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 40, 60, 80, 100 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R 
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new double[] { 160, 250, 340 }[level] + 0.75 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Diana", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 22, 34, 46, 58, 70 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 160, 220 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "DrMundo", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) =>
                        {
                            if (target.IsMinion)
                            {
                                return System.Math.Min(
                                    new double[] { 300, 400, 500, 600, 700 }[level],
                                    new double[] { 15, 18, 21, 23, 25 }[level] / 100 * target.Health);
                            }
                            return System.Math.Max(
                                new double[] { 80, 130, 180, 230, 280 }[level],
                                new double[] { 15, 18, 21, 23, 25 }[level] / 100 * target.Health);
                        }
                    },
                    //W - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 35, 50, 65, 80, 95 }[level] + 0.2 * source.FlatMagicDamageMod
                    }
                });

            spells.Add(
                "Draven", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 45, 55, 65, 75, 85 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.5 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 175, 275, 375 }[level] + 1.1 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Elise", new List<DamageSpell>
                {
                    //Q - Human
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 75, 110, 145, 180 }[level] +
                                (0.08 + 0.03 / 100 * source.FlatMagicDamageMod) * target.Health
                    },
                    //Q - Spider
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] +
                                (0.08 + 0.03 / 100 * source.FlatMagicDamageMod) * (target.MaxHealth - target.Health)
                    },
                    //W - Human
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 125, 175, 225, 275 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Evelynn", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 45, 60, 75, 90 }[level] +
                                new double[] { 35, 40, 45, 50, 55 }[level] / 100 * source.FlatMagicDamageMod +
                                new double[] { 50, 55, 60, 65, 70 }[level] / 100 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 1 * source.FlatMagicDamageMod +
                                1 * source.FlatPhysicalDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new[] { 0.15, 0.20, 0.25 }[level] + 0.01 / 100 * source.FlatMagicDamageMod) *
                                target.Health
                    },
                });

            spells.Add(
                "Ezreal", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 35, 55, 75, 95, 115 }[level] + 0.4 * source.FlatMagicDamageMod +
                                1.1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 125, 175, 225, 275 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 350, 500, 650 }[level] + 0.9 * source.FlatMagicDamageMod +
                                1 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Fiddlesticks", new List<DamageSpell>
                {
                    //W - Per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //E - Per bounce
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 85, 105, 125, 145 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //R - Per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 125, 225, 325 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Fiora", new List<DamageSpell>
                {
                    //Q - per dash
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                    //W 
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R - Max damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 325, 663, 1001 }[level] + 2.34 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Fizz", new List<DamageSpell>
                {
                    //Q - AA excluded.
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 25, 40, 55, 70 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //W - Per attack
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 20, 30, 40, 50 }[level] +
                                new double[] { 4, 5, 6, 7, 8 }[level] / 100 * (target.MaxHealth - target.Health)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 120, 170, 220, 270 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 325, 450 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Galio", new List<DamageSpell>
                {
                    //Q 
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 300 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 308, 462, 616 }[level] + 0.84 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "GangPlank", new List<DamageSpell>
                {
                    //Q 
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 45, 70, 95, 120 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - per cannonball
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 120, 165 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Garen", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 55, 80, 105, 130 }[level] +
                                1.4 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E 
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 45, 70, 95, 120 }[level] +
                                new double[] { 70, 80, 90, 100, 110 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - Max damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 175, 350, 525 }[level] +
                                new[] { 28.57, 33.33, 40 }[level] / 100 * (target.MaxHealth - target.Health)
                    },
                });

            spells.Add(
                "Gnar", new List<DamageSpell>
                {
                    //Q - mini
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 5, 35, 65, 95, 125 }[level] +
                                1.15 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //Q - big
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 5, 45, 85, 125, 165 }[level] +
                                1.2 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - mini
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 20, 30, 40, 50 }[level] + 1 * source.FlatMagicDamageMod +
                                new double[] { 6, 8, 10, 12, 14 }[level] / 100 * target.MaxHealth
                    },
                    //W - big
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 25, 45, 65, 85, 105 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E - mini
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 60, 100, 140, 180 }[level] + source.MaxHealth * 0.06
                    },
                    //E - big
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 60, 100, 140, 180 }[level] + source.MaxHealth * 0.06
                    },
                    //R - Max damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 300, 400 }[level] + 0.5 * source.FlatMagicDamageMod +
                                0.2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Gragas", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 50, 80, 110, 140 }[level] +
                                new double[] { 8, 9, 10, 11, 12 }[level] / 100 * target.MaxHealth +
                                0.3 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 300, 400 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Graves", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.8 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R - Max damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 400, 550 }[level] + 1.5 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Hecarim", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 30, 40, 50, 60 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 75, 110, 145, 180 }[level] + 0.5 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Heimerdinger", new List<DamageSpell>
                {
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 135, 180, 225 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                0.45 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 200, 250 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Irelia", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 50, 80, 110, 140 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.TrueDamage,
                        Damage = (source, target, level) => new double[] { 15, 30, 45, 60, 75 }[level]
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240  }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R - per blade
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160 }[level] + 0.5 * source.FlatMagicDamageMod +
                                0.6 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Janna", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 85, 110, 135, 160 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 115, 170, 225, 280 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "JarvanIV", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 1.2 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 325, 450 }[level] + 1.5 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Jax", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 1 * source.FlatPhysicalDamageMod +
                                0.6 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 75, 110, 145, 180 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] + 0.5 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 160, 220 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Jayce", new List<DamageSpell>
                {
                    //Q 
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 115, 170, 225, 280 }[level] + 1.2 * source.FlatPhysicalDamageMod
                    },
                    //Q - Melee
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 65, 110, 155, 200 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //W - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new[] { 25, 42.5, 60, 77.5, 95 }[level] + 0.25 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 8, 11, 14, 17, 20 }[level] / 100) * target.MaxHealth +
                                1 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Jinx", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) => 0.1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 60, 110, 160, 210 }[level] +
                                1.4 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 300 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R - Min
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 125, 175, 225 }[level] +
                                new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                                0.5 * source.FlatPhysicalDamageMod
                    },
                    //R - Max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 350, 450 }[level] +
                                new double[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                                1 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Karma", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 125, 170, 215, 260 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //Q - mantra
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 125, 170, 215, 260 }[level] +
                                new double[] { 25, 75, 125, 175 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                0.9 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W - mantra
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] +
                                new double[] { 75, 150, 225, 300 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                1.2 * source.FlatMagicDamageMod
                    },
                    //E - mantra
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 140, 220, 300 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Karthus", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 40, 60, 80, 100, 120 }[level] + 0.3 * source.FlatMagicDamageMod) * 2
                    },
                    //Q - Multi-target
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 60, 80, 100, 120 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 50, 70, 90, 110 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 400, 550 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Kassadin", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 95, 120, 145, 170 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W - pasive
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) => 20 + 0.1 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 105, 130, 155, 180 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //R - Base
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) => new double[] { 80, 100, 120 }[level] + 0.02 * source.MaxMana
                    },
                    //R - Per Stack
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) => new double[] { 40, 50, 60 }[level] + 0.01 * source.MaxMana
                    },
                });

            spells.Add(
                "Katarina", new List<DamageSpell>
                {
                    //Q - dagger
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 85, 110, 135, 160 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //Q - mark
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 30, 45, 60, 75 }[level] + 0.15 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 75, 110, 145, 180 }[level] + 0.6 * source.FlatPhysicalDamageMod +
                                0.25 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 85, 110, 135, 160 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R - per dagger
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 350, 550, 750 }[level] + 3.75 * source.FlatPhysicalDamageMod +
                                 2.5 * source.FlatMagicDamageMod) / 10
                    },
                    //R - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 400, 575, 750 }[level] + 3.75 * source.FlatPhysicalDamageMod +
                                2.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Kayle", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 1 * source.FlatPhysicalDamageMod +
                                0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 30, 40, 50, 60 }[level] + 0.25 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Kennen", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 115, 155, 195, 235 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                    //W - Passive
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 50, 60, 70, 80 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - Active
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 95, 125, 155, 185 }[level] + 0.55 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 85, 125, 165, 205, 245 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 145, 210 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "KhaZix", new List<DamageSpell>
                {
                    //Q - Normal target - UnEvolved
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 95, 120, 145, 170 }[level] + 1.2 * source.FlatPhysicalDamageMod
                    },
                    //Q - Isolated target - UnEvolved
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new[] { 91, 123.5, 156, 188.5, 221 }[level] + 1.56 * source.FlatPhysicalDamageMod
                    },
                    //Q - Normal target - Evolved
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 2,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 95, 120, 145, 170 }[level] + 2.24 * source.FlatPhysicalDamageMod +
                                10 * ((Obj_AI_Hero) source).Level
                    },
                    //Q - Isolated target - Evolved
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 3,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new[] { 91, 123.5, 156, 188.5, 221 }[level] + 2.6 * source.FlatPhysicalDamageMod +
                                10 * ((Obj_AI_Hero) source).Level
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 110, 140, 170, 200 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 100, 135, 170, 205 }[level] + 0.2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "KogMaw", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 2, 3, 4, 5, 6 }[level] / 100 + 0.01 / 100 * source.FlatMagicDamageMod) *
                                target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160 }[level] * 2 + 0.5 * source.FlatPhysicalDamageMod +
                                0.3 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Kalista", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 70, 130, 190, 250 }[level] + source.BaseAttackDamage +
                                source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 12, 14, 16, 18, 20 }[level] / 100) * target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage = (source, target, level) =>
                        {
                            var buff =
                                target.Buffs.Find(
                                    b =>
                                        b.Caster.NetworkId == source.NetworkId && b.IsValid &&
                                        b.DisplayName == "KalistaExpungeMarker");
                            if (buff != null)
                            {
                                return (new double[] { 20, 30, 40, 50, 60 }[level] +
                                        0.6 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)) +
                                       // Base damage of E
                                       ((buff.Count - 1) *
                                        (new double[] { 10, 14, 19, 25, 32 }[level] + // Base damage per spear
                                         new[] { 0.2, 0.225, 0.25, 0.275, 0.3 }[level] *
                                         (source.BaseAttackDamage + source.FlatPhysicalDamageMod)));
                                    // Damage multiplier per spear
                            }
                            return 0;
                        }
                    },
                });

            spells.Add(
                "LeBlanc", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 80, 105, 130, 155 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //Q . explosion
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 80, 105, 130, 155 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 85, 125, 165, 205, 245 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "LeeSin", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 80, 110, 140, 170 }[level] + 0.9 * source.FlatPhysicalDamageMod
                    },
                    //Q - 2nd
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 80, 110, 140, 170 }[level] + 0.9 * source.FlatPhysicalDamageMod +
                                0.08 * (target.MaxHealth - target.Health)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 400, 600 }[level] + 2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Leona", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 70, 100, 130, 160 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Lissandra", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 100, 130, 160, 190 }[level] + 0.65 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Lucian", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 110, 140, 170, 200 }[level] +
                                new double[] { 60, 75, 90, 105, 120 }[level] / 100 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.9 * source.FlatMagicDamageMod
                    },
                    //R - per shot
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 50, 60 }[level] + 0.1 * source.FlatMagicDamageMod +
                                0.25 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Lulu", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 125, 170, 215, 260 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 110, 140, 170, 200 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Lux", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 300, 400, 500 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Malphite", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 120, 170, 220, 270 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 38, 46, 54, 62 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.3 * source.Armor +
                                0.2 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 300, 400 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Malzahar", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 300 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 4, 5, 6, 7, 8 }[level] / 100 + 0.01 / 100 * source.FlatMagicDamageMod) *
                                target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 140, 200, 260, 320 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 400, 550 }[level] + 1.3 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Maokai", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 9, 10, 11, 12, 13 }[level] / 100 +
                                 0.03 / 100 * source.FlatMagicDamageMod) * target.MaxHealth
                    },
                    //E - impact
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 60, 80, 100, 120 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E - explosion
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 150, 200 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "MasterYi", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 25, 60, 95, 130, 165 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.6 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new[] { 10, 12.5, 15, 17.5, 20 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                new double[] { 10, 15, 20, 25, 30 }[level]
                    },
                });

            spells.Add(
                "MissFortune", new List<DamageSpell>
                {
                    //Q - First target
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 35, 50, 65, 80 }[level] + 0.35 * source.FlatMagicDamageMod +
                                0.85 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //Q - Second target
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 70, 100, 130, 160 }[level] + 0.5 * source.FlatMagicDamageMod +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) => 0.06 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 90, 145, 200, 255, 310 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R - per wave
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 125 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "MonkeyKing", new List<DamageSpell> //AKA wukong
                {
                    //Q - bonus
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] +
                                0.1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.8 * source.FlatPhysicalDamageMod
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 110, 200 }[level] +
                                1.1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                });

            spells.Add(
                "Mordekaiser", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 110, 140, 170, 200 }[level] + 1 * source.FlatPhysicalDamageMod +
                                0.4 * source.FlatMagicDamageMod
                    },
                    //W - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 24, 38, 52, 66, 80 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 24, 29, 34 }[level] / 100 + 0.04 / 100 * source.FlatMagicDamageMod) *
                                target.MaxHealth
                    },
                });

            spells.Add(
                "Morgana", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 135, 190, 245, 300 }[level] + 0.9 * source.FlatMagicDamageMod
                    },
                    //W - per tick
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 12, 19, 26, 33, 40 }[level] + 0.11 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 225, 300 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Nami", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 130, 185, 240, 295 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 25, 40, 55, 70, 85 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Nasus", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (from buff in ObjectManager.Player.Buffs
                                    where buff.DisplayName == "NasusQStacks"
                                    select buff.Count).FirstOrDefault() + new double[] { 30, 50, 70, 90, 110 }[level]
                    },
                    //E - Initial
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 95, 135, 175, 215 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 11, 19, 27, 35, 43 }[level] + 0.12 * source.FlatMagicDamageMod
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 3, 4, 5 }[level] / 100 + 0.01 / 100 * source.FlatMagicDamageMod) *
                                target.MaxHealth
                    },
                });

            spells.Add(
                "Nautilus", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 40, 50, 60, 70 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R - main target
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 325, 450 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R - missile
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 125, 175, 225 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Nidalee", new List<DamageSpell>
                {
                    //Q - human - min * 3 = max
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //Q - cat
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 4, 20, 50, 90 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                 0.36 * source.FlatMagicDamageMod +
                                 0.75 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)) *
                                ((target.MaxHealth - target.Health) / target.MaxHealth * 1.5 + 1)
                    },
                    //W - human
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 80, 120, 160, 200 }[level] +
                                0.2 * source.FlatMagicDamageMod
                    },
                    //W - cat
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 100, 150, 200 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                0.3 * source.FlatMagicDamageMod
                    },
                    //E - cat
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 130, 190, 250 }[source.Spellbook.GetSpell(SpellSlot.R).Level - 1] +
                                0.45 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Nocturne", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.75 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 100, 150, 200, 250 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 1.2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Nunu", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.TrueDamage,
                        Damage = (source, target, level) => new double[] { 400, 550, 700, 850, 1000 }[level]
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 85, 130, 175, 225, 275 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R - Max Damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 625, 875, 1125 }[level] + 2.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Olaf", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] +
                                0.4 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                });

            spells.Add(
                "Orianna", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 225, 300 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Pantheon", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 65, 105, 145, 185, 225 }[level] + 1.4 * source.FlatPhysicalDamageMod) *
                                ((target.Health / target.MaxHealth < 0.15) ? 2 : 1)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //E - per strike
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 13, 23, 33, 43, 53 }[level] + 0.6 * source.FlatPhysicalDamageMod) *
                                ((target is Obj_AI_Hero) ? 2 : 1)
                    },
                    //R - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 400, 700, 1000 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R - min
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 400, 700, 1000 }[level] + 1 * source.FlatMagicDamageMod) * 0.5
                    },
                });

            spells.Add(
                "Poppy", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                System.Math.Min(
                                    new double[] { 75, 150, 225, 300, 375 }[level],
                                    new double[] { 20, 40, 60, 80, 100 }[level] + 0.08 * target.MaxHealth +
                                    1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                    0.6 * source.FlatMagicDamageMod)
                    },
                    //E - without colliding
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E - with colliding
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] +
                                new double[] { 75, 125, 175, 225, 275 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Quinn", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.65 * source.FlatPhysicalDamageMod +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 70, 100, 130, 160 }[level] + 0.2 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 100, 150, 200 }[level] + 0.5 * source.FlatPhysicalDamageMod) *
                                ((target.MaxHealth - target.Health) / target.MaxHealth + 1)
                    },
                });

            spells.Add(
                "Rammus", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 100, 150, 200, 250, 300 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) => new double[] { 15, 25, 35, 45, 55 }[level] + 0.1 * source.Armor
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 130, 195 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Renekton", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.8 * source.FlatPhysicalDamageMod
                    },
                    //Q - empowered
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 60, 90, 120, 150, 180 }[level] + 0.8 * source.FlatPhysicalDamageMod) *
                                1.5
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 30, 50, 70, 90 }[level] +
                                1.5 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - empowered
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 10, 30, 50, 70, 90 }[level] +
                                 1.5 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)) * 1.5
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] + 0.9 * source.FlatPhysicalDamageMod
                    },
                    //E - empowered
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 30, 60, 90, 120, 150 }[level] + 0.9 * source.FlatPhysicalDamageMod) *
                                1.5
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 120 }[level] + 0.1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Rengar", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] +
                                new double[] { 0, 5, 10, 15, 20 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //Q - Extra
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] +
                                (new double[] { 100, 105, 110, 115, 120 }[level] / 100 - 1) *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 80, 110, 140, 170 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 100, 150, 200, 250 }[level] + 0.7 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Riven", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 30, 50, 70, 90 }[level] +
                                ((source.BaseAttackDamage + source.FlatPhysicalDamageMod) / 100) *
                                new double[] { 40, 45, 50, 55, 60 }[level]
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 80, 110, 140, 170 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 80, 120, 160 }[level] + 0.6 * source.FlatPhysicalDamageMod) *
                                ((target.MaxHealth - target.Health) / target.MaxHealth * 2.67 + 1)
                    },
                });

            spells.Add(
                "Rumble", new List<DamageSpell>
                {
                    //Q - total  damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 135, 195, 255, 315 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //Q - Danger Zone total damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new[] { 112.5, 202.5, 292.5, 382.5, 472.5 }[level] + 1.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 45, 70, 95, 120, 145 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E - Danger Zone
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new[] { 67.5, 105, 142.5, 180, 217.5 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 130, 185, 240 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R - Total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 650, 925, 1200 }[level] + 1.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Ryze", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 75, 95, 115, 135 }[level] + 0.4 * source.FlatMagicDamageMod +
                                0.065 * source.MaxMana
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.6 * source.FlatMagicDamageMod +
                                0.045 * source.MaxMana
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 70, 90, 110, 130 }[level] + 0.35 * source.FlatMagicDamageMod +
                                0.01 * source.MaxMana
                    },
                });

            spells.Add(
                "Sejuani", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) => new double[] { 80, 125, 170, 215, 260 }[level]
                    },
                    //W - AA  damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new[] { 4, 5.5, 7, 8.5, 10 }[level] + (source.FlatMagicDamageMod / 100)) / 100 *
                                target.MaxHealth
                    },
                    //W - Aoe per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new[] { 10, 17.5, 25, 32.5, 40 }[level] +
                                (new double[] { 4, 6, 8, 10, 12 }[level] / 100) * source.MaxHealth +
                                0.15 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Shaco", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 140, 160, 180, 200, 220 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - per attack
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 35, 50, 65, 80, 95 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 90, 130, 170, 210 }[level] + 1 * source.FlatPhysicalDamageMod +
                                1 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 300, 450, 600 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Shen", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 85, 120, 155, 190 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Shyvana", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 85, 90, 95, 100 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 35, 50, 65, 80 }[level] + 0.2 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 100, 140, 180, 220 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 175, 300, 425 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Singed", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 22, 34, 46, 58, 70 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 65, 80, 95, 110 }[level] + 0.75 * source.FlatMagicDamageMod + new[] { 4, 5.5, 7, 8.5, 10 }[level] / 100 * target.MaxHealth
                    },
                });

            spells.Add(
                "Sion", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 40, 60, 80, 100 }[level] +
                                0.6 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 120, 180, 240, 300 }[level] +
                                1.8 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.4 * source.FlatMagicDamageMod +
                                new double[] { 10, 11, 12, 13, 14 }[level] / 100 * target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 70, 105, 140, 175, 210 }[level] + 0.4 * source.FlatMagicDamageMod) * 1.5
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 300, 450 }[level] + 0.4 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 150, 300, 450 }[level] + 0.4 * source.FlatPhysicalDamageMod) * 2
                    },
                });

            spells.Add(
                "Sivir", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 25, 45, 65, 85, 105 }[level] +
                                new double[] { 70, 80, 90, 100, 110 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //W - bounce
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 55, 60, 65, 70 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                });

            spells.Add(
                "Skarner", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 30, 40, 50, 60 }[level] + 0.4 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 75, 110, 145, 180 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 100, 150, 200 }[level] + 0.5 * source.FlatMagicDamageMod) * 2
                    },
                });

            spells.Add(
                "Sona", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 80, 120, 160, 200 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Soraka", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Swain", new List<DamageSpell>
                {
                    //Q - per second
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 25, 40, 55, 70, 85 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 115, 155, 195, 235 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //R - per draven
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 70, 90 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Syndra", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 50, 95, 140, 185, 230 }[level] + 0.6 * source.FlatMagicDamageMod) *
                                ((level == 5 && target is Obj_AI_Hero) ? 1.15 : 1)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R - min damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 270, 405, 540 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R - per sphere
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 90, 135, 180 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Talon", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] + 0.3 * source.FlatPhysicalDamageMod +
                                ((target is Obj_AI_Hero)
                                    ? (new double[] { 10, 20, 30, 40, 50 }[level] + 1 * source.FlatPhysicalDamageMod)
                                    : 0)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 55, 80, 105, 130 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 120, 170, 220 }[level] + 0.75 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Taric", new List<DamageSpell>
                {
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 80, 120, 160, 200 }[level] + 0.2 * source.Armor
                    },
                    //E - min damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 70, 100, 130, 160 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Teemo", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 125, 170, 215, 260 }[level] + 0.8 * source.FlatMagicDamageMod
                    },
                    //E - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 34, 68, 102, 136, 170 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E - onhit
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 20, 30, 40, 50 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R - total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 325, 450 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Thresh", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //E - Active
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 95, 125, 155, 185 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 400, 550 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Tristana", new List<DamageSpell>
                {
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 105, 130, 155, 180 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //E - base damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 70, 80, 90, 100 }[level] +
                                new[] { 0.5, 0.65, 0.8, 0.95, 1.10 }[level] * source.FlatPhysicalDamageMod +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 300, 400, 500 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Trundle", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 40, 60, 80, 100 }[level] +
                                new[] { 0, 0.5, 0.1, 0.15, 0.2 }[level] *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - Total
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 20, 24, 28 }[level] / 100 + 0.02 * source.FlatMagicDamageMod / 100) *
                                target.MaxHealth
                    },
                });

            spells.Add(
                "Tryndamere", new List<DamageSpell>
                {
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 100, 130, 160, 190 }[level] + 1.2 * source.FlatPhysicalDamageMod +
                                1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "TwistedFate", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 110, 160, 210, 260 }[level] + 0.65 * source.FlatMagicDamageMod
                    },
                    //W - Blue
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 60, 80, 100, 120 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //W - Red
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 45, 60, 75, 90 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //W - Yellow
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 2,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new[] { 15, 22.5, 30, 37.5, 45 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.5 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 80, 105, 130, 155 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Twitch", new List<DamageSpell>
                {
                    //E - current stacks
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (from buff in target.Buffs
                                    where buff.DisplayName.ToLower() == "twitchdeadlyvenom"
                                    select buff.Count).FirstOrDefault() *
                                (new double[] { 15, 20, 25, 30, 35 }[level] + 0.2 * source.FlatMagicDamageMod +
                                 0.25 * source.FlatPhysicalDamageMod) + new double[] { 20, 35, 50, 65, 80 }[level]
                    },
                    //E - per stack
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 20, 25, 30, 35 }[level] + 0.2 * source.FlatMagicDamageMod +
                                0.25 * source.FlatPhysicalDamageMod + new double[] { 20, 35, 50, 65, 80 }[level]
                    },
                });

            spells.Add(
                "Udyr", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 80, 130, 180, 230 }[level] +
                                (new double[] { 120, 130, 140, 150, 160 }[level] / 100) *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //R - per wave
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 25, 35, 45, 55 }[level] + 0.25 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Urgot", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 40, 70, 100, 130 }[level] +
                                0.85 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 130, 185, 240, 295 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Varus", new List<DamageSpell>
                {
                    //Q - min
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 47, 83, 120, 157 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //Q - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        Stage = 1,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 70, 125, 180, 235 }[level] +
                                +1.6 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W - on hit
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 10, 14, 18, 22, 26 }[level] + 0.25 * source.FlatMagicDamageMod
                    },
                    //W - per stack
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                (new[] { 2, 2.75, 3.5, 4.25, 5 }[level] / 100 + 0.02 * source.FlatMagicDamageMod / 100) *
                                target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 65, 100, 135, 170, 205 }[level] + 0.6 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Vayne", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 35, 40, 45, 50 }[level] / 100 *
                                (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.TrueDamage,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 30, 40, 50, 60 }[level] +
                                (new double[] { 4, 5, 6, 7, 8 }[level] / 100) * target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 45, 80, 115, 150, 185 }[level] + 0.5 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Veigar", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 125, 170, 215, 260 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 120, 170, 220, 270, 320 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 375, 500 }[level] + 0.8 * target.FlatMagicDamageMod +
                                1.0 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Velkoz", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W - Max
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 50, 70, 90, 110 }[level] +
                                new double[] { 45, 75, 105, 135, 165 }[level] + 0.625 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 100, 130, 160, 190 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 500, 700, 900 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Vi", new List<DamageSpell>
                {
                    //Q - min
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 50, 75, 100, 125, 150 }[level] + 0.8 * source.FlatPhysicalDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new[] { 4, 5.5, 7, 8.5, 10 }[level] / 100 + 0.01 * source.FlatPhysicalDamageMod / 35) *
                                target.MaxHealth
                    },
                    //E - extra
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 5, 20, 35, 50, 65 }[level] +
                                1.15 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod) +
                                0.7 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 300, 450 }[level] + 1.4 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Viktor", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 60, 80, 100, 120 }[level] + 0.2 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 115, 160, 205, 250 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 98, 161, 224, 287, 350 }[level] + 0.98 * source.FlatMagicDamageMod
                    },
                    //R - summon damage
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.55 * source.FlatMagicDamageMod
                    },
                    //R - per bolt
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        Stage = 1,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 30, 45 }[level] + 0.1 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Vladimir", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 90, 125, 160, 195, 230 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //W - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage = (source, target, level) => new double[] { 80, 135, 190, 245, 300 }[level]
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 85, 110, 135, 160 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Volibear", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage = (source, target, level) => new double[] { 30, 60, 90, 120, 150 }[level]
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                (new double[] { 80, 125, 170, 215, 260 }[level]) *
                                ((target.MaxHealth - target.Health) / target.MaxHealth + 1)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 105, 150, 195, 240 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R - per bolt
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 115, 155 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Warwick", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                System.Math.Max(
                                    new double[] { 75, 125, 175, 225, 275 }[level],
                                    new double[] { 8, 10, 12, 14, 16 }[level] / 100 * target.MaxHealth) +
                                1 * source.FlatMagicDamageMod
                    },
                    //R - max
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 150, 250, 350 }[level] + 2 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Xerath", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 120, 160, 200, 240 }[level] + 0.75 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 110, 140, 170, 200 }[level] + 0.45 * source.FlatMagicDamageMod
                    },
                    //R - per charge
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 190, 245, 300 }[level] + 0.43 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "XinZhao", new List<DamageSpell>
                {
                    //Q - per attack
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 15, 30, 45, 60, 75 }[level] +
                                0.2 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 175, 275 }[level] + 1 * source.FlatPhysicalDamageMod +
                                0.15 * target.Health
                    },
                });

            spells.Add(
                "Yasuo", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 20, 40, 60, 80, 100 }[level] +
                                1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //E - min
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 90, 110, 130, 150 }[level] + 0.6 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 200, 300, 400 }[level] + 1.5 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Yorick", new List<DamageSpell>
                {
                    //Q - extra
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 30, 60, 90, 120, 150 }[level] +
                                1.2 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 1 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 55, 85, 115, 145, 175 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                });

            spells.Add(
                "Zac", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 110, 150, 190, 230 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 55, 70, 85, 100 }[level] +
                                (new double[] { 4, 5, 6, 7, 8 }[level] / 100 + 0.02 * source.FlatMagicDamageMod / 100) *
                                target.MaxHealth
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 80, 130, 180, 230, 280 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                    //R - per bounce
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 140, 210, 280 }[level] + 0.4 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Zed", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 115, 155, 195, 235 }[level] + 1 * source.FlatPhysicalDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Physcial,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 90, 120, 150, 180 }[level] + 0.8 * source.FlatPhysicalDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Physcial,
                        Damage = (source, target, level) => 1 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)
                    },
                });

            spells.Add(
                "Ziggs", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 75, 120, 165, 210, 255 }[level] + 0.65 * source.FlatMagicDamageMod
                    },
                    //W
                    new DamageSpell
                    {
                        Slot = SpellSlot.W,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.35 * source.FlatMagicDamageMod
                    },
                    //E - per mine
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 40, 65, 90, 115, 140 }[level] + 0.3 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 250, 375, 500 }[level] + 0.9 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Zilean", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 90, 145, 200, 260, 320 }[level] + 0.9 * source.FlatMagicDamageMod
                    },
                });

            spells.Add(
                "Zyra", new List<DamageSpell>
                {
                    //Q
                    new DamageSpell
                    {
                        Slot = SpellSlot.Q,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 70, 105, 140, 175, 210 }[level] + 0.65 * source.FlatMagicDamageMod
                    },
                    //E
                    new DamageSpell
                    {
                        Slot = SpellSlot.E,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 60, 95, 130, 165, 200 }[level] + 0.5 * source.FlatMagicDamageMod
                    },
                    //R
                    new DamageSpell
                    {
                        Slot = SpellSlot.R,
                        DamageType = DamageType.Magical,
                        Damage =
                            (source, target, level) =>
                                new double[] { 180, 265, 350 }[level] + 0.7 * source.FlatMagicDamageMod
                    },
                });
        }
        #endregion

        #region PassiveDamages
        private static void LoadAttackPassives(ref List<PassiveDamage> attackPassives)
        {
            PassiveDamage p;



            #region Aatrox

            p = new PassiveDamage
            {
                ChampionName = "Aatrox",
                IsActive = (source, target) => (source.HasBuff("AatroxWPower") && source.HasBuff("AatroxWONHPowerBuff")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.W)),
            };

            attackPassives.Add(p);

            #endregion

            #region Alistar

            p = new PassiveDamage
            {
                ChampionName = "Alistar",
                IsActive = (source, target) => (source.HasBuff("Trample")),
                GetDamage =
                    (source, target) =>
                        ((float)
                            source.CalculateDamage(
                                target, DamageType.Magical, 6d + source.Level + (0.1d * source.FlatMagicDamageMod))),
            };
            attackPassives.Add(p);

            #endregion

            #region Caitlyn

            p = new PassiveDamage
            {
                ChampionName = "Caitlyn",
                IsActive = (source, target) => (source.HasBuff("CaitlynHeadshotReady")),
                GetDamage =
                    (source, target) =>
                        ((float)
                            source.CalculateDamage(
                                target, DamageType.Physcial,
                                1.5d * (source.BaseAttackDamage + source.FlatPhysicalDamageMod))),
            };
            attackPassives.Add(p);

            #endregion

            #region Draven

            p = new PassiveDamage
            {
                ChampionName = "Draven",
                IsActive = (source, target) => (source.HasBuff("dravenspinning")),
                GetDamage =
                    (source, target) =>
                        ((float)
                            source.CalculateDamage(
                                target, DamageType.Physcial,
                                0.45d * (source.BaseAttackDamage + source.FlatPhysicalDamageMod))),
            };
            attackPassives.Add(p);

            #endregion

            #region Corki

            p = new PassiveDamage
            {
                ChampionName = "Corki",
                IsActive = (source, target) => (source.HasBuff("RapidReload")),
                GetDamage =
                    (source, target) => ((float)0.1d * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)),
            };
            attackPassives.Add(p);

            #endregion

            #region Gnar

            p = new PassiveDamage
            {
                ChampionName = "Gnar",
                IsActive =
                    (source, target) =>
                        (from buff in target.Buffs where buff.DisplayName == "GnarWProc" select buff.Count)
                            .FirstOrDefault() == 2,
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.W)),
            };
            attackPassives.Add(p);

            #endregion

            #region Jinx

            p = new PassiveDamage
            {
                ChampionName = "Jinx",
                IsActive = (source, target) => (source.HasBuff("JinxQ")),
                GetDamage =
                    (source, target) =>
                        ((float)
                            source.CalculateDamage(
                                target, DamageType.Physcial,
                                0.1d * (source.BaseAttackDamage + source.FlatPhysicalDamageMod))),
            };
            attackPassives.Add(p);

            #endregion

            #region Katarina

            p = new PassiveDamage
            {
                ChampionName = "Katarina",
                IsActive = (source, target) => (target.HasBuff("KataQMark1")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.Q, 1)),
            };
            attackPassives.Add(p);

            #endregion

            #region KogMaw

            p = new PassiveDamage
            {
                ChampionName = "KogMaw",
                IsActive = (source, target) => (source.HasBuff("KogMawBioArcaneBarrage")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.W)),
            };
            attackPassives.Add(p);

            #endregion

            #region MissFortune

            p = new PassiveDamage
            {
                ChampionName = "MissFortune",
                IsActive = (source, target) => (source.HasBuff("MissFortunePassive")),
                GetDamage =
                    (source, target) =>
                        (float)
                            source.CalculateDamage(
                                target, DamageType.Magical,
                                (float)0.06 * (source.BaseAttackDamage + source.FlatPhysicalDamageMod)),
            };
            attackPassives.Add(p);

            #endregion

            #region Nasus

            p = new PassiveDamage
            {
                ChampionName = "Nasus",
                IsActive = (source, target) => (source.HasBuff("SiphoningStrike")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.Q)),
            };
            attackPassives.Add(p);

            #endregion

            #region Orianna

            p = new PassiveDamage
            {
                ChampionName = "Orianna",
                IsActive = (source, target) => (source.HasBuff("OrianaSpellSword")),
                GetDamage =
                    (source, target) =>
                        (float)
                            source.CalculateDamage(
                                target, DamageType.Magical,
                                (float)0.15 * source.FlatMagicDamageMod +
                                new float[] { 10, 10, 10, 18, 18, 18, 26, 26, 26, 34, 34, 34, 42, 42, 42, 50, 50, 50 }[
                                    source.Level - 1]),
            };
            attackPassives.Add(p);

            #endregion

            #region Teemo

            p = new PassiveDamage
            {
                ChampionName = "Teemo",
                IsActive = (source, target) => (source.HasBuff("Toxic Attack")),
                GetDamage =
                    (source, target) =>
                        ((float)
                            source.CalculateDamage(
                                target, DamageType.Magical,
                                source.Spellbook.GetSpell(SpellSlot.E).Level * 10 + source.FlatMagicDamageMod * 0.3)),
            };
            attackPassives.Add(p);

            #endregion

            #region TwistedFate

            p = new PassiveDamage
            {
                ChampionName = "TwistedFate",
                IsActive = (source, target) => (source.HasBuff("Pick A Card Blue")),
                GetDamage =
                    (source, target) =>
                        (float)source.GetSpellDamage(target, SpellSlot.W) -
                        (float)
                            source.CalculateDamage(
                                target, DamageType.Physcial, (source.BaseAttackDamage + source.FlatPhysicalDamageMod)),
            };
            attackPassives.Add(p);

            p = new PassiveDamage
            {
                ChampionName = "TwistedFate",
                IsActive = (source, target) => (source.HasBuff("CardMasterStackParticle")),
                GetDamage = (source, target) => (float)source.GetSpellDamage(target, SpellSlot.E),
            };
            attackPassives.Add(p);

            #endregion

            #region Varus

            p = new PassiveDamage
            {
                ChampionName = "Varus",
                IsActive = (source, target) => (source.HasBuff("VarusW")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.W)),
            };
            attackPassives.Add(p);

            #endregion

            #region Vayne

            p = new PassiveDamage
            {
                ChampionName = "Vayne",
                IsActive = (source, target) => (source.HasBuff("VayneTumble")),
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.Q)),
            };
            attackPassives.Add(p);

            p = new PassiveDamage
            {
                ChampionName = "Vayne",
                IsActive =
                    (source, target) =>
                        (from buff in target.Buffs where buff.DisplayName == "VayneSilverDebuff" select buff.Count)
                            .FirstOrDefault() == 2,
                GetDamage = (source, target) => ((float)source.GetSpellDamage(target, SpellSlot.W)),
            };
            attackPassives.Add(p);

            #endregion

            #region Ziggs

            p = new PassiveDamage
            {
                ChampionName = "Ziggs",
                IsActive = (source, target) => (source.HasBuff("ziggsShortFuse")),
                GetDamage =
                    (source, target) =>
                        (float)
                            source.CalculateDamage(
                                target, DamageType.Magical,
                                (float)0.25d * source.FlatMagicDamageMod +
                                new float[] { 20, 24, 28, 32, 36, 40, 48, 56, 64, 72, 80, 88, 100, 112, 124, 136, 148, 160 }[
                                    source.Level - 1]),
            };
            attackPassives.Add(p);

            #endregion
        }
        #endregion
    }
}