namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     The team the minion is on.
    /// </summary>
    public enum MinionTeam
    {
        /// <summary>
        ///     Neutral(Jungle minions)
        /// </summary>
        Neutral,

        /// <summary>
        ///     Ally minions
        /// </summary>
        Ally,

        /// <summary>
        ///     Enemy minions
        /// </summary>
        Enemy,

        /// <summary>
        ///     Minions that are not an ally.
        /// </summary>
        NotAlly,

        /// <summary>
        ///     Minions that are not an ally for the the enemy.
        /// </summary>
        NotAllyForEnemy,

        /// <summary>
        ///     Any team.
        /// </summary>
        All
    }
}