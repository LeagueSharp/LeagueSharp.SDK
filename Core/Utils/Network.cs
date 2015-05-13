// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Network.cs" company="LeagueSharp">
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
//   Utilities for Packets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using LeagueSharp.SDK.Core.Network;

    /// <summary>
    ///     Utilities for Packets.
    /// </summary>
    public static class NetworkUtils
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Checks if the packet type matches another packet type.
        /// </summary>
        /// <param name="args">Packet arguments from event</param>
        /// <param name="packet">The Packet</param>
        /// <returns>Whether the packets have the same ID</returns>
        public static bool Equals(this GamePacketEventArgs args, Packet packet)
        {
            return args.GetPacketId() == packet.Id;
        }

        /// <summary>
        ///     Gets the packet id.
        /// </summary>
        /// <param name="args">Packet arguments from event</param>
        /// <returns>The packet ID</returns>
        public static short GetPacketId(this GamePacketEventArgs args)
        {
            return (short)((args.PacketData[1] << 8) | (args.PacketData[0] << 0));
        }

        /// <summary>
        ///     Gets the packet id.
        /// </summary>
        /// <param name="data">Raw packet data</param>
        /// <returns>The packet ID</returns>
        public static short GetPacketId(this byte[] data)
        {
            return (short)((data[1] << 8) | (data[0] << 0));
        }

        #endregion
    }
}