namespace LeagueSharp.CommonEx.Core.Network
{
    internal static class Utils
    {
        public static byte GetHeader(this GamePacketEventArgs args)
        {
            return args.PacketData[0];
        }

        public static bool Equals(this GamePacketEventArgs args, Packet.C2S header)
        {
            return args.GetHeader() == (byte) header;
        }
    }
}