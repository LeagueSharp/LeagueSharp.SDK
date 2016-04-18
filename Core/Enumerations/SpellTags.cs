// <copyright file="SpellTags.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK
{
    /// <summary>
    ///     Properties that a spell can have
    /// </summary>
    public enum SpellTags
    {
        /// <summary>
        ///     The spell deals damage
        /// </summary>
        Damage,

        /// <summary>
        ///     The spell's effects are AoE
        /// </summary>
        AoE,

        /// <summary>
        ///     The spell applies on-hit effects.
        /// </summary>
        AppliesOnHitEffects,

        /// <summary>
        ///     The spell applies CC
        /// </summary>
        CrowdControl,

        /// <summary>
        ///     The spell applies a shield on the target
        /// </summary>
        Shield,

        /// <summary>
        ///     The spell can heal
        /// </summary>
        Heal,

        /// <summary>
        ///     The spell makes the target enter a stasis state (invulnerable)
        /// </summary>
        Stasis,

        /// <summary>
        ///     The spell leaves a mark than can subsequently be proc'd to deal additional damage
        /// </summary>
        LeavesMark,

        /// <summary>
        ///     The spell can detonate a previously left mark.
        /// </summary>
        CanDetonateMark,

        /// <summary>
        ///     The spell modifies the champion's other spells (nida/jayce/elise ult)
        /// </summary>
        Transformation,

        /// <summary>
        ///     The spell is a dash
        /// </summary>
        Dash,

        /// <summary>
        ///     The spell is a blink
        /// </summary>
        Blink,

        /// <summary>
        ///     The spell teleports the champion
        /// </summary>
        Teleport,

        /// <summary>
        ///     The spell amplifies the damage dealt by attacks or spells
        /// </summary>
        DamageAmplifier,

        /// <summary>
        ///     The spell increases health/armor/mr
        /// </summary>
        DefensiveBuff,

        /// <summary>
        ///     The spell increases the target's movement speed
        /// </summary>
        MovementSpeedAmplifier,

        /// <summary>
        ///     The spell increases the target's Attack Speed
        /// </summary>
        AttackSpeedAmplifier,

        /// <summary>
        ///     The spell increases the target's Attack Range
        /// </summary>
        AttackRangeModifier,

        /// <summary>
        ///     The spell applies a spellshield
        /// </summary>
        SpellShield,

        /// <summary>
        ///     The spell removes all CC from target
        /// </summary>
        RemoveCrowdControl,

        /// <summary>
        ///     The spell grants vision of the target area.
        /// </summary>
        GrantsVision,

        /// <summary>
        ///     The spell can be interrupted
        /// </summary>
        Interruptable
    }
}