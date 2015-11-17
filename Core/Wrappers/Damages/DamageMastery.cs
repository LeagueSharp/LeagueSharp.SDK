namespace LeagueSharp.SDK.Core.Wrappers.Damages
{
    using System.Linq;

    /// <summary>
    /// </summary>
    internal static class DamageMastery
    {
        #region Enums

        internal enum Cunning
        {
            Wanderer = 65,

            Savagery = 66,

            RunicAffinity = 81,

            SecretStash = 82,

            Meditation = 98,

            Merciless = 97,

            Bandit = 114,

            DangerousGame = 115,

            Precision = 129,

            Intelligence = 130,

            StormraiderSurge = 145,

            ThunderlordDecree = 146,

            WindspeakerBlessing = 147
        }

        internal enum Ferocity
        {
            Fury = 65,

            Sorcery = 68,

            DoubleEdgedSword = 81,

            Vampirism = 97,

            NaturalTalent = 100,

            Feast = 82,

            BountyHunter = 113,

            Oppressor = 114,

            BatteringBlows = 129,

            PiercingThoughts = 132,

            WarlordBloodlust = 145,

            FervorofBattle = 146,

            DeathfireTouch = 137
        }

        internal enum Resolve
        {
            Recovery = 65,

            Unyielding = 66,

            Explorer = 81,

            ToughSkin = 82,

            RunicArmor = 97,

            VeteranScars = 98,

            Insight = 113,

            Perseverance = 114,

            Swiftness = 129,

            LegendaryGuardian = 130,

            GraspoftheUndying = 145,

            StrengthoftheAges = 146,

            BondofStone = 147
        }

        #endregion

        #region Methods

        internal static Mastery GetCunning(this Obj_AI_Hero hero, Cunning cunning)
        {
            return hero.GetMastery(MasteryPage.Defense, (int)cunning);
        }

        internal static Mastery GetFerocity(this Obj_AI_Hero hero, Ferocity ferocity)
        {
            return hero.GetMastery(MasteryPage.Offense, (int)ferocity);
        }

        internal static Mastery GetResolve(this Obj_AI_Hero hero, Resolve resolve)
        {
            return hero.GetMastery(MasteryPage.Utility, (int)resolve);
        }

        internal static bool IsMoveImpaired(this Obj_AI_Hero hero)
        {
            return hero.HasBuffOfType(BuffType.Knockback) || hero.HasBuffOfType(BuffType.Knockup)
                   || hero.HasBuffOfType(BuffType.Slow) || hero.HasBuffOfType(BuffType.Stun)
                   || hero.HasBuffOfType(BuffType.Snare) || hero.HasBuffOfType(BuffType.Fear)
                   || hero.HasBuffOfType(BuffType.Taunt) || hero.HasBuffOfType(BuffType.Suppression);
        }

        internal static bool IsValid(this Mastery mastery)
        {
            return mastery != null && mastery.Points > 0;
        }

        private static Mastery GetMastery(this Obj_AI_Hero hero, MasteryPage page, int id)
        {
            return hero.Masteries.FirstOrDefault(m => m.Page == page && m.Id == id);
        }

        #endregion
    }
}