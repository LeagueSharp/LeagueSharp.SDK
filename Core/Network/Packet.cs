using System.Collections.Generic;

namespace LeagueSharp.CommonEx.Core.Network
{
    /// <summary>
    ///     Class of known packets.
    /// </summary>
    public class Packet
    {
        /// <summary>
        ///     Sent on buying an item
        /// </summary>
        public static Packet BuyItem = new Packet(0xD7, PacketChannel.C2S);

        /// <summary>
        ///     Sent on moving camera or zooming.
        /// </summary>
        public static Packet Camera = new Packet(0x10, PacketChannel.C2S, PacketProtocolFlags.NoFlags);

        /// <summary>
        ///     Sent on a spell cast
        /// </summary>
        public static Packet CastSpell = new Packet(0xE4, PacketChannel.C2S);

        /// <summary>
        ///     Sent on a chat message(in the game, not client)
        /// </summary>
        public static Packet Chat = new Packet(0x68, PacketChannel.Communication);

        /// <summary>
        ///     Sent on an emote
        /// </summary>
        public static Packet Emote = new Packet(0x2C, PacketChannel.C2S);

        /// <summary>
        ///     Sent on heartbeat
        /// </summary>
        public static Packet HeartBeat = new Packet(0x4F, PacketChannel.GamePlay);

        /// <summary>
        ///     Sent on using objects(Clicking thresh lantern)
        /// </summary>
        public static Packet UseObject = new Packet(0x90, PacketChannel.C2S);

        /// <summary>
        ///     Sent on an issue order(move to, hold position)
        /// </summary>
        public static Packet IssueOrder = new Packet(0xB5, PacketChannel.C2S);

        /// <summary>
        ///     Sent on leveling spell
        /// </summary>
        public static Packet LevelSpell = new Packet(0x9A, PacketChannel.C2S);

        /// <summary>
        ///     Sent on locking camera.
        /// </summary>
        public static Packet LockCamera = new Packet(0xFD, PacketChannel.C2S);

        /// <summary>
        ///     Sent on pausing games(tournament mode, custom games)
        /// </summary>
        public static Packet PauseGame = new Packet(0x97, PacketChannel.C2S);

        /// <summary>
        ///     Sent on game ping
        /// </summary>
        public static Packet Ping = new Packet(0x122, PacketChannel.C2S);

        /// <summary>
        ///     Sent on when game resumes from pause
        /// </summary>
        public static Packet ResumeGame = new Packet(0x19, PacketChannel.C2S);

        /// <summary>
        ///     Sent when opening score board.
        /// </summary>
        public static Packet ScoreScreen = new Packet(0x96, PacketChannel.C2S);

        /// <summary>
        ///     Sent on selling an item.
        /// </summary>
        public static Packet SellItem = new Packet(0x32, PacketChannel.C2S);

        /// <summary>
        ///     Sent on left clicking target.
        /// </summary>
        public static Packet SetTarget = new Packet(0xA9, PacketChannel.C2S);

        /// <summary>
        ///     Sent on pressing the undo button in the shop.
        /// </summary>
        public static Packet UndoBuy = new Packet(0x1C, PacketChannel.C2S);

        /// <summary>
        ///     The channel of the packet.
        /// </summary>
        public PacketChannel Channel;

        /// <summary>
        ///     The flags of the packet.
        /// </summary>
        public PacketProtocolFlags Flags;

        /// <summary>
        ///     The packet id.
        /// </summary>
        public short Id;

        /// <summary>
        ///     Creates a <see cref="Packet" /> with the packet data.
        /// </summary>
        /// <param name="id">Packet Id</param>
        /// <param name="channel">Packet Channel</param>
        /// <param name="flags">Packet Flags</param>
        public Packet(short id, PacketChannel channel, PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            Id = id;
            Channel = channel;
            Flags = flags;
        }

        /// <summary>
        ///     Collection of all known packets.
        /// </summary>
        public static PacketWrapper PacketList
        {
            get
            {
                return new PacketWrapper
                {
                    BuyItem,
                    CastSpell,
                    Chat,
                    Emote,
                    HeartBeat,
                    IssueOrder,
                    LevelSpell,
                    LockCamera,
                    PauseGame,
                    Ping,
                    ResumeGame,
                    ScoreScreen,
                    SetTarget,
                    UndoBuy,
                    UseObject
                };
            }
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with the packet data.
        /// </summary>
        public GamePacket GetGamePacket()
        {
            return new GamePacket(this);
        }
    }

    /// <summary>
    ///     Methods to manipulate collection of packets.
    /// </summary>
    public class PacketWrapper : List<Packet>
    {
        /// <summary>
        ///     Client to Server packets.
        /// </summary>
        public List<Packet> C2S
        {
            get { return FindAll(p => p.Channel.Equals(PacketChannel.C2S)); }
        }

        /// <summary>
        ///     Server to Client packets.
        /// </summary>
        public List<Packet> S2C
        {
            get { return FindAll(p => p.Channel.Equals(PacketChannel.S2C)); }
        }

        /// <summary>
        ///     GamePlay channel packets.
        /// </summary>
        public List<Packet> GamePlay
        {
            get { return FindAll(p => p.Channel.Equals(PacketChannel.GamePlay)); }
        }

        /// <summary>
        ///     Communication channel packets.
        /// </summary>
        public List<Packet> Communication
        {
            get { return FindAll(p => p.Channel.Equals(PacketChannel.Communication)); }
        }

        /// <summary>
        ///     Packets with reliable flags.
        /// </summary>
        public List<Packet> Reliable
        {
            get { return FindAll(p => p.Flags.Equals(PacketProtocolFlags.Reliable)); }
        }

        /// <summary>
        ///     Packets with no flags.
        /// </summary>
        public List<Packet> NoFlags
        {
            get { return FindAll(p => p.Flags.Equals(PacketProtocolFlags.NoFlags)); }
        }

        /// <summary>
        ///     Packets with unsequenced flags.
        /// </summary>
        public List<Packet> Unsequenced
        {
            get { return FindAll(p => p.Flags.Equals(PacketProtocolFlags.Unsequenced)); }
        }
    }
}