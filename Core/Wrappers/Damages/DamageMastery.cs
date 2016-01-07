namespace LeagueSharp.SDK.Core.Wrappers.Damages
{
    using System.Linq;

    /// <summary>
    ///     The damage mastery enum.
    /// </summary>
    public static class DamageMastery
    {
        #region Enums

        /// <summary>
        ///     The cunning enum.
        /// </summary>
        public enum Cunning
        {
            /// <summary>
            ///     The wanderer
            /// </summary>
            Wanderer = 65,

            /// <summary>
            ///     The savagery
            /// </summary>
            Savagery = 66,

            /// <summary>
            ///     The runic affinity
            /// </summary>
            RunicAffinity = 81,

            /// <summary>
            ///     The secret stash
            /// </summary>
            SecretStash = 82,

            /// <summary>
            ///     The merciless
            /// </summary>
            Merciless = 97,

            /// <summary>
            ///     The meditation
            /// </summary>
            Meditation = 98,

            /// <summary>
            ///     The bandit
            /// </summary>
            Bandit = 114,

            /// <summary>
            ///     The dangerous game
            /// </summary>
            DangerousGame = 115,

            /// <summary>
            ///     The precision
            /// </summary>
            Precision = 129,

            /// <summary>
            ///     The intelligence
            /// </summary>
            Intelligence = 130,

            /// <summary>
            ///     The stormraiders surge
            /// </summary>
            StormraidersSurge = 145,

            /// <summary>
            ///     The thunderlords decree
            /// </summary>
            ThunderlordsDecree = 146,

            /// <summary>
            ///     The windspeakers blessing
            /// </summary>
            WindspeakersBlessing = 147
        }

        /// <summary>
        ///     The ferocity enum.
        /// </summary>
        public enum Ferocity
        {
            /// <summary>
            ///     The fury
            /// </summary>
            Fury = 65,

            /// <summary>
            ///     The sorcery
            /// </summary>
            Sorcery = 68,

            /// <summary>
            ///     The double edged sword
            /// </summary>
            DoubleEdgedSword = 81,

            /// <summary>
            ///     The feast
            /// </summary>
            Feast = 82,

            /// <summary>
            ///     The vampirism
            /// </summary>
            Vampirism = 97,

            /// <summary>
            ///     The natural talent
            /// </summary>
            NaturalTalent = 100,

            /// <summary>
            ///     The bounty hunter
            /// </summary>
            BountyHunter = 113,

            /// <summary>
            ///     The oppressor
            /// </summary>
            Oppressor = 114,

            /// <summary>
            ///     The battering blows
            /// </summary>
            BatteringBlows = 129,

            /// <summary>
            ///     The piercing thoughts
            /// </summary>
            PiercingThoughts = 132,

            /// <summary>
            ///     The warlords bloodlust
            /// </summary>
            WarlordsBloodlust = 145,

            /// <summary>
            ///     The fervorof battle
            /// </summary>
            FervorofBattle = 146,

            /// <summary>
            ///     The deathfire touch
            /// </summary>
            DeathfireTouch = 137
        }

        /// <summary>
        ///     The resolve enum.
        /// </summary>
        public enum Resolve
        {
            /// <summary>
            ///     The recovery
            /// </summary>
            Recovery = 65,

            /// <summary>
            ///     The unyielding
            /// </summary>
            Unyielding = 66,

            /// <summary>
            ///     The explorer
            /// </summary>
            Explorer = 81,

            /// <summary>
            ///     The tough skin
            /// </summary>
            ToughSkin = 82,

            /// <summary>
            ///     The runic armor
            /// </summary>
            RunicArmor = 97,

            /// <summary>
            ///     The veterans scars
            /// </summary>
            VeteransScars = 98,

            /// <summary>
            ///     The insight
            /// </summary>
            Insight = 113,

            /// <summary>
            ///     The perseverance
            /// </summary>
            Perseverance = 114,

            /// <summary>
            ///     The swiftness
            /// </summary>
            Swiftness = 129,

            /// <summary>
            ///     The legendary guardian
            /// </summary>
            LegendaryGuardian = 130,

            /// <summary>
            ///     The graspofthe undying
            /// </summary>
            GraspoftheUndying = 145,

            /// <summary>
            ///     The strengthofthe ages
            /// </summary>
            StrengthoftheAges = 146,

            /// <summary>
            ///     The bondof stone
            /// </summary>
            BondofStone = 147
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the cunning.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="cunning">The cunning.</param>
        /// <returns></returns>
        public static Mastery GetCunning(this Obj_AI_Hero hero, Cunning cunning)
        {
            return hero.GetMastery(MasteryPage.Cunning, (int)cunning);
        }

        /// <summary>
        ///     Gets the ferocity.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="ferocity">The ferocity.</param>
        /// <returns></returns>
        public static Mastery GetFerocity(this Obj_AI_Hero hero, Ferocity ferocity)
        {
            return hero.GetMastery(MasteryPage.Ferocity, (int)ferocity);
        }

        /// <summary>
        ///     Gets the resolve.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="resolve">The resolve.</param>
        /// <returns></returns>
        public static Mastery GetResolve(this Obj_AI_Hero hero, Resolve resolve)
        {
            return hero.GetMastery(MasteryPage.Resolve, (int)resolve);
        }

        /// <summary>
        ///     Returns true if mastery is valid.
        /// </summary>
        /// <param name="mastery">The mastery.</param>
        /// <returns></returns>
        public static bool IsValid(this Mastery mastery)
        {
            return mastery != null && mastery.Points > 0;
        }

        #endregion

        #region Methods

        internal static bool IsMoveImpaired(this Obj_AI_Hero hero)
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