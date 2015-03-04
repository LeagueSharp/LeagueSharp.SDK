namespace LeagueSharp.CommonEx.Core.Network
{
    /// <summary>
    ///     Utilities for Packets.
    /// </summary>
    public static class NetworkUtils
    {
        /// <summary>
        ///     Gets the header of the packet.
        /// </summary>
        /// <param name="args">Packet arguements from event</param>
        /// <returns>The header</returns>
        public static short GetHeader(this GamePacketEventArgs args)
        {
            return args.PacketData[0];
        }

        /// <summary>
        ///     Checks if the packet type matches another packet type.
        /// </summary>
        /// <param name="args">Packet arguements from event</param>
        /// <param name="header">Packet Header</param>
        /// <returns>Whether the packets have the same time</returns>
        public static bool Equals(this GamePacketEventArgs args, Packet.C2S header)
        {
            return args.GetHeader() == (byte) header;
        }
    }
}