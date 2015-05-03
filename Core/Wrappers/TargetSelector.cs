using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Gets a best target.
    /// </summary>
    //TODO: Implement damage and menu.
    public class TargetSelector
    {
        /// <summary>
        ///     Champions that should be prioritzed fourth(last). (4)
        /// </summary>
        private static readonly string[] LowestPriority =
        {
            "Alistar", "Amumu", "Blitzcrank", "Braum", "Cho'Gath",
            "Dr. Mundo", "Garen", "Gnar", "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus",
            "Nautilus", "Nunu", "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner",
            "Sona", "Soraka", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
        };

        /// <summary>
        ///     Champions that should be prioritized third (3)
        /// </summary>
        private static readonly string[] MedLowPriority =
        {
            "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank",
            "Gragas", "Irelia", "Jax", "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar",
            "Rumble", "Ryze", "Swain", "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao", "RekSai"
        };

        /// <summary>
        ///     Champions that should be prioritized second. (2)
        /// </summary>
        private static readonly string[] MedHighPriority =
        {
            "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz",
            "Heimerdinger", "Jayce", "Kassadin", "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven",
            "Shaco", "Vladimir", "Yasuo", "Zilean"
        };

        /// <summary>
        ///     Champions taht should be prioritized first. (1)
        /// </summary>
        private static readonly string[] HighestPriority =
        {
            "Ahri", "Anivia", "Annie", "Ashe", "Brand", "Caitlyn",
            "Cassiopeia", "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina",
            "Kennen", "KogMaw", "LeBlanc", "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn",
            "Sivir", "Syndra", "Talon", "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar",
            "VelKoz", "Viktor", "Xerath", "Zed", "Ziggs"
        };

        /// <summary>
        ///     The current mode the TS is using.
        /// </summary>
        private static TargetSelectorMode _mode = TargetSelectorMode.AutoPriority;

        /// <summary>
        ///     Gets the priority of the champion. (1 being the highest, 4 being the lowest)
        /// </summary>
        /// <param name="champName">Champion Name</param>
        /// <returns>Number representing the priority. (1 being the highest, 4 being the lowest)</returns>
        public static int GetPriority(string champName)
        {
            if (HighestPriority.Contains(champName))
            {
                return 1;
            }

            if (MedHighPriority.Contains(champName))
            {
                return 2;
            }

            if (MedLowPriority.Contains(champName))
            {
                return 3;
            }

            return LowestPriority.Contains(champName) ? 4 : 1;
        }

        /// <summary>
        ///     Sets the <see cref="TargetSelectorMode" /> of the target selector.
        /// </summary>
        /// <param name="mode">Mode</param>
        public static void SetPriorityMode(TargetSelectorMode mode)
        {
            //TODO: Replace this with menu code.
            _mode = mode;
        }

        /// <summary>
        ///     Gets a target based on range, damage type, from, and ignores any specified champions.
        /// </summary>
        /// <param name="range">The range to get the enemies in.</param>
        /// <param name="damageType">Damage Type</param>
        /// <param name="from">Position to get enemies around, defaults to player's position.</param>
        /// <param name="ignoredChampions">Any champions to ignore.</param>
        /// <returns></returns>
        public static Obj_AI_Hero GetTarget(float range,
            DamageType damageType = DamageType.True,
            Vector3 from = new Vector3(),
            IEnumerable<Obj_AI_Hero> ignoredChampions = null)
        {
            if (!from.IsValid())
            {
                from = ObjectManager.Player.ServerPosition;
            }

            if (ignoredChampions == null)
            {
                ignoredChampions = new List<Obj_AI_Hero>();
            }

            // Filter out champions that we are ignoring
            var enemyChamps =
                ObjectHandler.EnemyHeroes.Where(x => x.IsValidTarget(range, true, from))
                    .Where(x => ignoredChampions.Any(y => y.NetworkId != x.NetworkId)).Where(x => !IsInvulnerable(x, damageType));

            return GetChampionByMode(enemyChamps, damageType);
        }

        /// <summary>
        ///     Gets a champion out of a list, and picks the best one based on the mode.
        /// </summary>
        /// <param name="heroes">List of heroes.</param>
        /// <param name="damageType">The type of damage to get.</param>
        /// <returns>The best hero.</returns>
        private static Obj_AI_Hero GetChampionByMode(IEnumerable<Obj_AI_Hero> heroes, DamageType damageType)
        {
            switch (_mode)
            {
                // TODO: Use Damage.cs
                case TargetSelectorMode.LessAttacksToKill:
                    return heroes.MinOrDefault(x => x.Health / ObjectManager.Player.TotalAttackDamage);

                case TargetSelectorMode.MostAbilityPower:
                    return heroes.MaxOrDefault(x => x.TotalMagicalDamage);

                case TargetSelectorMode.MostAttackDamage:
                    return heroes.MaxOrDefault(x => x.TotalAttackDamage);

                case TargetSelectorMode.Closest:
                    return heroes.MinOrDefault(x => x.Distance(ObjectManager.Player));

                case TargetSelectorMode.NearMouse:
                    return heroes.MinOrDefault(x => x.Distance(Game.CursorPos));

                case TargetSelectorMode.LessCastPriority:
                    return heroes.MinOrDefault(x => x.Health / ObjectManager.Player.TotalMagicalDamage);

                case TargetSelectorMode.AutoPriority:
                    return heroes.MinOrDefault(x => GetPriority(x.ChampionName));

                case TargetSelectorMode.LeastHealth:
                    return heroes.MinOrDefault(x => x.Health);
            }

            return null;
        }

        /// <summary>
        ///     Gets whether or not a target is invulnerable to a spell.
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="damageType">Damage Type.</param>
        /// <param name="ignoreShields">Ignore shields(Morgana E, Banshees, etc)</param>
        /// <returns>Whether or not a target is invulnerable to a spell.</returns>
        public static bool IsInvulnerable(Obj_AI_Base target, DamageType damageType, bool ignoreShields = true)
        {
            // Tryndamere's Undying Rage (R)
            if (!damageType.Equals(DamageType.True) && target.HasBuff("Undying Rage") && target.Health <= 2f)
            {
                return true;
            }

            // Kayle's Intervention (R)
            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (ignoreShields)
            {
                return false;
            }

            // Morgana's Black Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BlackShield"))
            {
                return true;
            }

            // Banshee's Veil (PASSIVE)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return true;
            }

            // Sivir's Spell Shield (E)
            if (damageType.Equals(DamageType.Magical) && target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return true;
            }

            // Nocturne's Shroud of Darkness (W)
            return damageType.Equals(DamageType.Magical) && target.HasBuff("ShroudofDarkness");
        }

        /// <summary>
        ///     Gets a target that that can be hit by the spell. 
        /// </summary>
        /// <param name="spell">Spell</param>
        /// <param name="ignoredChampions">Champions that should be ignored.</param>
        /// <returns>Best target</returns>
        public static Obj_AI_Hero GetTargetNoCollision(Spell spell, IEnumerable<Obj_AI_Hero> ignoredChampions = null)
        {
            if (ignoredChampions == null)
            {
                ignoredChampions = new List<Obj_AI_Hero>();
            }

            var enemyChamps =
                ObjectHandler.EnemyHeroes.Where(x => x.IsValidTarget(spell.Range, true, spell.From))
                    .Where(x => ignoredChampions.Any(y => y.NetworkId != x.NetworkId))
                    .Where(x => spell.GetPrediction(x).Hitchance != HitChance.Collision)
                    .Where(x => !IsInvulnerable(x, spell.DamageType));

            return GetChampionByMode(enemyChamps, spell.DamageType);
        }
    }
}