// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Packet.cs" company="LeagueSharp">
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
//   Class of known packets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Network
{
    using System.Collections.Generic;

    /// <summary>
    ///     Class of known packets.
    /// </summary>
    public class Packet
    {
        #region Static Fields

        /// <summary>
        ///     Sent on buying an item
        /// </summary>
        public static readonly Packet BuyItem = new Packet(0x2B, PacketChannel.C2S);

        /// <summary>
        ///     Sent on moving camera or zooming.
        /// </summary>
        public static readonly Packet Camera = new Packet(0xC5, PacketChannel.C2S, PacketProtocolFlags.NoFlags);

        /// <summary>
        ///     Sent on a spell cast
        /// </summary>
        public static readonly Packet CastSpell = new Packet(0xE9, PacketChannel.C2S);

        /// <summary>
        ///     Sent on a chat message(in the game, not client)
        /// </summary>
        public static readonly Packet Chat = new Packet(0x68, PacketChannel.Communication);

        /// <summary>
        ///     Sent on an emote
        /// </summary>
        public static readonly Packet Emote = new Packet(0x0A, PacketChannel.C2S);

        /// <summary>
        ///     Sent on heartbeat
        /// </summary>
        public static readonly Packet HeartBeat = new Packet(0x124, PacketChannel.GamePlay);

        /// <summary>
        ///     Sent on using objects(Clicking thresh lantern)
        /// </summary>
        public static readonly Packet InteractObject = new Packet(0x10B, PacketChannel.C2S);

        /// <summary>
        ///     Sent on an issue order(move to, hold position)
        /// </summary>
        public static readonly Packet IssueOrder = new Packet(0xD2, PacketChannel.C2S);

        /// <summary>
        ///     Sent on leveling spell
        /// </summary>
        public static readonly Packet LevelSpell = new Packet(0xC9, PacketChannel.C2S);

        /// <summary>
        ///     Sent on locking camera.
        /// </summary>
        public static readonly Packet LockCamera = new Packet(0xC9, PacketChannel.C2S);

        /// <summary>
        ///     Sent on pausing games(tournament mode, custom games)
        /// </summary>
        public static readonly Packet PauseGame = new Packet(0x10E, PacketChannel.C2S);

        /// <summary>
        ///     Sent on game ping. (Broken)
        /// </summary>
        public static readonly Packet Ping = new Packet(0xD3, PacketChannel.C2S);

        /// <summary>
        ///     Sent on when game resumes from pause
        /// </summary>
        public static readonly Packet ResumeGame = new Packet(0xAD, PacketChannel.C2S);

        /// <summary>
        ///     Sent when opening score board.
        /// </summary>
        public static readonly Packet ScoreScreen = new Packet(0x75, PacketChannel.C2S);

        /// <summary>
        ///     Sent on selling an item.
        /// </summary>
        public static readonly Packet SellItem = new Packet(0x51, PacketChannel.C2S);

        /// <summary>
        ///     Sent on left clicking target.
        /// </summary>
        public static readonly Packet SetTarget = new Packet(0x101, PacketChannel.C2S);

        /// <summary>
        ///     Sent on pressing the undo button in the shop.
        /// </summary>
        public static readonly Packet UndoBuy = new Packet(0x109, PacketChannel.C2S);

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Packet" /> class.
        ///     Creates a <see cref="Packet" /> with the packet data.
        /// </summary>
        /// <param name="id">
        ///     Packet Id
        /// </param>
        /// <param name="channel">
        ///     Packet Channel
        /// </param>
        /// <param name="flags">
        ///     Packet Flags
        /// </param>
        public Packet(short id, PacketChannel channel, PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            this.Id = id;
            this.Channel = channel;
            this.Flags = flags;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Collection of all known packets.
        /// </summary>
        public static PacketWrapper PacketList
        {
            get
            {
                return new PacketWrapper
                           {
                               BuyItem, CastSpell, Chat, Emote, HeartBeat, IssueOrder, LevelSpell, LockCamera, PauseGame, 
                               Ping, ResumeGame, ScoreScreen, SetTarget, UndoBuy, InteractObject
                           };
            }
        }

        /// <summary>
        ///     Gets or sets the channel of the packet.
        /// </summary>
        public PacketChannel Channel { get; set; }

        /// <summary>
        ///     Gets or sets the flags of the packet.
        /// </summary>
        public PacketProtocolFlags Flags { get; set; }

        /// <summary>
        ///     Gets or sets the packet id.
        /// </summary>
        public short Id { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with the packet data.
        /// </summary>
        /// <returns>
        ///     The <see cref="GamePacket" />.
        /// </returns>
        public GamePacket GetGamePacket()
        {
            return new GamePacket(this);
        }

        #endregion
    }

    /// <summary>
    ///     Methods to manipulate collection of packets.
    /// </summary>
    public class PacketWrapper : List<Packet>
    {
        #region Public Properties

        /// <summary>
        ///     Gets the Client to Server packets.
        /// </summary>
        public List<Packet> C2S
        {
            get
            {
                return this.FindAll(p => p.Channel.Equals(PacketChannel.C2S));
            }
        }

        /// <summary>
        ///     Gets the Communication channel packets.
        /// </summary>
        public List<Packet> Communication
        {
            get
            {
                return this.FindAll(p => p.Channel.Equals(PacketChannel.Communication));
            }
        }

        /// <summary>
        ///     Gets the GamePlay channel packets.
        /// </summary>
        public List<Packet> GamePlay
        {
            get
            {
                return this.FindAll(p => p.Channel.Equals(PacketChannel.GamePlay));
            }
        }

        /// <summary>
        ///     Gets the Packets with no flags.
        /// </summary>
        public List<Packet> NoFlags
        {
            get
            {
                return this.FindAll(p => p.Flags.Equals(PacketProtocolFlags.NoFlags));
            }
        }

        /// <summary>
        ///     Gets the Packets with reliable flags.
        /// </summary>
        public List<Packet> Reliable
        {
            get
            {
                return this.FindAll(p => p.Flags.Equals(PacketProtocolFlags.Reliable));
            }
        }

        /// <summary>
        ///     Gets the Server to Client packets.
        /// </summary>
        public List<Packet> S2C
        {
            get
            {
                return this.FindAll(p => p.Channel.Equals(PacketChannel.S2C));
            }
        }

        /// <summary>
        ///     Gets the Packets with un-sequenced flags.
        /// </summary>
        public List<Packet> Unsequenced
        {
            get
            {
                return this.FindAll(p => p.Flags.Equals(PacketProtocolFlags.Unsequenced));
            }
        }

        #endregion
    }
}