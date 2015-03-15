namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     Cast States
    /// </summary>
    public enum CastStates
    {
        /// <summary>
        ///     Spell Successfully Casted
        /// </summary>
        SuccessfullyCasted,

        /// <summary>
        ///     Spell Not Ready
        /// </summary>
        NotReady,

        /// <summary>
        ///     Spell Not Casted
        /// </summary>
        NotCasted,

        /// <summary>
        ///     Spell Out of Range
        /// </summary>
        OutOfRange,

        /// <summary>
        ///     Spell Collision
        /// </summary>
        Collision,

        /// <summary>
        ///     Spell Not Enough Targets
        /// </summary>
        NotEnoughTargets,

        /// <summary>
        ///     Spell Low Hit Chance
        /// </summary>
        LowHitChance
    }
}