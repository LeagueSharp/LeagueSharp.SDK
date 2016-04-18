// <copyright file="LastCastPacketSentEntry.cs" company="LeagueSharp">
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
    ///     The last cast packet sent entry.
    /// </summary>
    public class LastCastPacketSentEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastPacketSentEntry" /> class.
        /// </summary>
        /// <param name="slot">
        ///     The slot
        /// </param>
        /// <param name="tick">
        ///     The tick
        /// </param>
        /// <param name="targetNetworkId">
        ///     The target network id
        /// </param>
        public LastCastPacketSentEntry(SpellSlot slot, int tick, int targetNetworkId)
        {
            this.Slot = slot;
            this.Tick = tick;
            this.TargetNetworkId = targetNetworkId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the slot.
        /// </summary>
        public SpellSlot Slot { get; private set; }

        /// <summary>
        ///     Gets the target network id.
        /// </summary>
        public int TargetNetworkId { get; private set; }

        /// <summary>
        ///     Gets the tick.
        /// </summary>
        public int Tick { get; private set; }

        #endregion
    }
}