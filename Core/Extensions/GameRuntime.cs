// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameRuntime.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   The game runtime extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Extensions
{
    using LeagueSharp.SDK.Core.Wrappers;

    /// <summary>
    ///     The game runtime extensions.
    /// </summary>
    public static class GameRuntime
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="spell">Spell Data Instance</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellDataInst spell, int t = 0)
        {
            return (spell != null)
                   && (spell.Slot != SpellSlot.Unknown && t == 0
                           ? spell.State == SpellState.Ready
                           : (spell.State == SpellState.Ready
                              || (spell.State == SpellState.Cooldown && (spell.CooldownExpires - Game.Time) <= t / 1000f)));
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="spell">The Spell</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this Spell spell, int t = 0)
        {
            return IsReady(spell.Instance, t);
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="slot">The SpellSlot</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellSlot slot, int t = 0)
        {
            var s = GameObjects.Player.Spellbook.GetSpell(slot);
            return s != null && IsReady(s, t);
        }

        #endregion
    }
}