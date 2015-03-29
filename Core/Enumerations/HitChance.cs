namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     Skillshot HitChance
    /// </summary>
    public enum HitChance
    {
        /// <summary>
        ///     Target is immobile, skillshot will hit.
        /// </summary>
        Immobile = 8,

        /// <summary>
        ///     Target is dashing to a known location, skillshot will hit.
        /// </summary>
        Dashing = 7,

        /// <summary>
        ///     Very High Prediction output, skillshot will probably hit.
        /// </summary>
        VeryHigh = 6,

        /// <summary>
        ///     High Prediction output, skillshot will probably hit.
        /// </summary>
        High = 5,

        /// <summary>
        ///     Medium Prediction output, accuracy considered low.
        /// </summary>
        Medium = 4,

        /// <summary>
        ///     Low Prediction output, accuracy considered low.
        /// </summary>
        Low = 3,

        /// <summary>
        ///     Impossible Hit.
        /// </summary>
        Impossible = 2,

        /// <summary>
        ///     Skillshot is out of range.
        /// </summary>
        OutOfRange = 1,

        /// <summary>
        ///     Collision before hit onto target.
        /// </summary>
        Collision = 0,

        /// <summary>
        ///     No HitChance.
        /// </summary>
        None = -1
    }
}