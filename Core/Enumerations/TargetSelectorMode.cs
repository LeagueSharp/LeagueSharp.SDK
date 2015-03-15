namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     Enum that defines the priority in which the target selector should organize targets.
    /// </summary>
    public enum TargetSelectorMode
    {
        /// <summary>
        ///     Focuses targets based on how many auto attacks it takes to kill the units.
        /// </summary>
        LessAttacksToKill,

        /// <summary>
        ///     Focuses targets based on the amount of AP they have.
        /// </summary>
        MostAbilityPower,

        /// <summary>
        ///     Focuses targets based on the amount of AD they have.
        /// </summary>
        MostAttackDamage,

        /// <summary>
        ///     Focuses targets based on the distance between the player and target.
        /// </summary>
        Closest,

        /// <summary>
        ///     Focuses targets base on the distance between the target and the mouse.
        /// </summary>
        NearMouse,

        /// <summary>
        ///     Focuses targets based on the amount skills needed to use to kill the target.
        /// </summary>
        LessCastPriority,

        /// <summary>
        ///     Focuses targets by their class.
        /// </summary>
        AutoPriority,

        /// <summary>
        ///     Focuses targets by their health.
        /// </summary>
        LeastHealth
    }
}