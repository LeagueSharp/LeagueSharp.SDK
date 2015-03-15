using LeagueSharp.CommonEx.Core.Network;

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     Utilities for Packets.
    /// </summary>
    public static class NetworkUtils
    {
        /// <summary>
        ///     Gets the packet id.
        /// </summary>
        /// <param name="args">Packet arguements from event</param>
        /// <returns>The packet ID</returns>
        public static short GetPacketId(this GamePacketEventArgs args)
        {
            return (short) ((args.PacketData[1] << 8) | (args.PacketData[0] << 0));
        }

        /// <summary>
        ///     Gets the packet id.
        /// </summary>
        /// <param name="data">Raw packet data</param>
        /// <returns>The packet ID</returns>
        public static short GetPacketId(this byte[] data)
        {
            return (short) ((data[1] << 8) | (data[0] << 0));
        }

        /// <summary>
        ///     Checks if the packet type matches another packet type.
        /// </summary>
        /// <param name="args">Packet arguements from event</param>
        /// <param name="packet">Packet</param>
        /// <returns>Whether the packets have the same ID</returns>
        public static bool Equals(this GamePacketEventArgs args, Packet packet)
        {
            return args.GetPacketId() == packet.Id;
        }
    }
}