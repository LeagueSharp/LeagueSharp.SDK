#region

using System;

#endregion

namespace LeagueSharp.CommonEx.Core.Enumerations
{
    /// <summary>
    ///     Collisonable Objects Flags
    /// </summary>
    [Flags]
    public enum CollisionableObjects
    {
        /// <summary>
        ///     Minion Collisionable Flag
        /// </summary>
        Minions,

        /// <summary>
        ///     Hero Collisionable Flag
        /// </summary>
        Heroes,

        /// <summary>
        ///     Yasuo-Wall Collisionable Flag
        /// </summary>
        YasuoWall,

        /// <summary>
        ///     Wall Collisionable Flag
        /// </summary>
        Walls
    }
}