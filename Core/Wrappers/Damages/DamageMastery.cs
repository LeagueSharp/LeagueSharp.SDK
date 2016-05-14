namespace LeagueSharp.SDKEx
{
    using System.Linq;

    public static partial class Damage
    {
        #region Enums

        public enum Cunning
        {
            Wanderer = 65,

            Savagery = 66,

            RunicAffinity = 81,

            SecretStash = 82,

            Assassin = 83,

            Merciless = 97,

            Meditation = 98,

            Bandit = 114,

            DangerousGame = 115,

            Precision = 129,

            Intelligence = 130,

            StormraidersSurge = 145,

            ThunderlordsDecree = 146,

            WindspeakersBlessing = 147
        }

        public enum Ferocity
        {
            Fury = 65,

            Sorcery = 68,

            DoubleEdgedSword = 81,

            Feast = 82,

            ExposeWeakness = 83,

            Vampirism = 97,

            NaturalTalent = 100,

            BountyHunter = 113,

            Oppressor = 114,

            BatteringBlows = 129,

            PiercingThoughts = 132,

            WarlordsBloodlust = 145,

            FervorofBattle = 146,

            DeathfireTouch = 148
        }

        public enum Resolve
        {
            Recovery = 65,

            Unyielding = 66,

            Explorer = 81,

            ToughSkin = 83,

            RunicArmor = 97,

            VeteransScars = 98,

            Insight = 113,

            Perseverance = 114,

            Swiftness = 129,

            LegendaryGuardian = 130,

            GraspoftheUndying = 145,

            StrengthoftheAges = 146,

            BondofStone = 147
        }

        #endregion

        #region Public Methods and Operators

        public static Mastery GetCunning(this Obj_AI_Hero hero, Cunning cunning)
        {
            return hero.GetMastery(MasteryPage.Cunning, (int)cunning);
        }

        public static Mastery GetFerocity(this Obj_AI_Hero hero, Ferocity ferocity)
        {
            return hero.GetMastery(MasteryPage.Ferocity, (int)ferocity);
        }

        public static Mastery GetResolve(this Obj_AI_Hero hero, Resolve resolve)
        {
            return hero.GetMastery(MasteryPage.Resolve, (int)resolve);
        }

        public static bool IsValid(this Mastery mastery)
        {
            return mastery != null && mastery.Points > 0;
        }

        #endregion

        #region Methods

        internal static bool IsMoveImpaired(this Obj_AI_Base hero)
        {
            return hero.HasBuffOfType(BuffType.Knockback) || hero.HasBuffOfType(BuffType.Knockup)
                   || hero.HasBuffOfType(BuffType.Charm) || hero.HasBuffOfType(BuffType.Slow)
                   || hero.HasBuffOfType(BuffType.Stun) || hero.HasBuffOfType(BuffType.Snare)
                   || hero.HasBuffOfType(BuffType.Flee) || hero.HasBuffOfType(BuffType.Taunt)
                   || hero.HasBuffOfType(BuffType.Suppression);
        }

        private static Mastery GetMastery(this Obj_AI_Hero hero, MasteryPage page, int id)
        {
            return hero.Masteries.FirstOrDefault(m => m.Page == page && m.Id == id);
        }

        #endregion
    }
}