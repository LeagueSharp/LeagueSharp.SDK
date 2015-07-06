// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GamePacket.cs" company="LeagueSharp">
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
//   This class makes easier to handle packets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.Core.Network
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     This class makes easier to handle packets.
    /// </summary>
    public sealed class GamePacket : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Game Packet Data
        /// </summary>
        private readonly GamePacketData packetData;

        /// <summary>
        ///     Local protocol that packet is sent on.
        /// </summary>
        private PacketProtocolFlags flags = PacketProtocolFlags.Reliable;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacket" /> class.
        /// </summary>
        /// <param name="data">
        ///     Bytes of the packet
        /// </param>
        public GamePacket(byte[] data)
        {
            this.packetData = new GamePacketData(data);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacket" /> class.
        /// </summary>
        /// <param name="args">
        ///     GamePacket event data
        /// </param>
        public GamePacket(GamePacketEventArgs args)
        {
            this.packetData = new GamePacketData(args);
            this.Channel = args.Channel;
            this.Flags = args.ProtocolFlag;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacket" /> class.
        /// </summary>
        /// <param name="packet">
        ///     The Packet
        /// </param>
        public GamePacket(Packet packet)
        {
            this.packetData = new GamePacketData(packet.Id);
            this.Channel = packet.Channel;
            this.Flags = packet.Flags;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacket" /> class.
        /// </summary>
        /// <param name="id">
        ///     Id of the packet
        /// </param>
        /// <param name="channel">
        ///     The Channel
        /// </param>
        /// <param name="flags">
        ///     Protocol to send packet
        /// </param>
        public GamePacket(short id, PacketChannel channel, PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            this.packetData = new GamePacketData(id);
            this.Channel = channel;
            this.Flags = flags;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the channel this packet is sent on.
        /// </summary>
        public PacketChannel Channel { get; set; }

        /// <summary>
        ///     Gets or sets the protocol that packet is sent on.
        /// </summary>
        public PacketProtocolFlags Flags
        {
            get
            {
                return this.flags;
            }

            set
            {
                this.flags = value;
            }
        }

        /// <summary>
        ///     Gets the raw packet bytes
        /// </summary>
        /// <returns>Bytes of the packet</returns>
        public byte[] PacketData
        {
            get
            {
                return this.packetData.PacketData;
            }
        }

        /// <summary>
        ///     Gets the Header of the packet(The first byte)
        /// </summary>
        public short PacketId
        {
            get
            {
                return this.packetData.GetPacketId();
            }
        }

        /// <summary>
        ///     Gets or sets the position of the reader.
        /// </summary>
        public long Position
        {
            get
            {
                return this.Reader.BaseStream.Position;
            }

            set
            {
                if (value >= 0L)
                {
                    this.Reader.BaseStream.Position = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to block sending and processing the packet.
        /// </summary>
        public bool Process { get; set; }

        /// <summary>
        ///     Gets the packet size.
        /// </summary>
        public long Size
        {
            get
            {
                return this.Reader.BaseStream.Length;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the reader.
        /// </summary>
        private BinaryReader Reader
        {
            get
            {
                return this.packetData.Reader;
            }
        }

        /// <summary>
        ///     Gets the writer.
        /// </summary>
        private BinaryWriter Writer
        {
            get
            {
                return this.packetData.Writer;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Dispose of the GamePacket.
        /// </summary>
        public void Dispose()
        {
            if (this.packetData != null)
            {
                this.packetData.Dispose();
            }

            if (this.Reader != null)
            {
                this.Reader.Dispose();
            }

            if (this.Writer != null)
            {
                this.Writer.Dispose();
            }
        }

        /// <summary>
        ///     Dumps the packet.
        /// </summary>
        /// <param name="simple">
        ///     Dumps only the raw data if true.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string DumpPacket(bool simple = false)
        {
            var bytes = string.Concat(this.PacketData.Select(b => b.ToString("X2") + " "));

            if (simple)
            {
                return bytes;
            }

            bytes = "Data: " + bytes;
            var channel = "Channel: " + this.Channel;
            var packetFlags = "Flags: " + this.Flags;

            return bytes + "\n" + channel + "\n" + packetFlags + "\n";
        }

        /// <summary>
        ///     Receives the packet.
        /// </summary>
        /// <param name="channel">Channel to process the packet on</param>
        public void ProcessPacket(PacketChannel channel = PacketChannel.S2C)
        {
            if (this.Process)
            {
                Game.ProcessPacket(this.PacketData, channel);
            }
        }

        /// <summary>
        ///     Reads a byte from the packet and increases the position by 1.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read byte</returns>
        public byte ReadByte(long position = -1)
        {
            this.Position = position;
            return this.Reader.ReadBytes(1)[0];
        }

        /// <summary>
        ///     Reads and returns a float.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read float</returns>
        public float ReadFloat(long position = -1)
        {
            this.Position = position;
            return BitConverter.ToSingle(this.Reader.ReadBytes(4), 0);
        }

        /// <summary>
        ///     Reads and returns an integer.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read integer</returns>
        public int ReadInteger(long position = -1)
        {
            this.Position = position;
            return BitConverter.ToInt32(this.Reader.ReadBytes(4), 0);
        }

        /// <summary>
        ///     Reads and returns a double byte.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read short</returns>
        public short ReadShort(long position = -1)
        {
            this.Position = position;
            return BitConverter.ToInt16(this.Reader.ReadBytes(2), 0);
        }

        /// <summary>
        ///     Reads and returns a string.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read string</returns>
        public string ReadString(long position = -1)
        {
            this.Position = position;
            var sb = new StringBuilder();

            for (var i = this.Position; i < this.Size; i++)
            {
                var num = this.ReadByte();

                if (num == 0)
                {
                    return sb.ToString();
                }

                sb.Append(Convert.ToChar(num));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Saves the packet dump to a file
        /// </summary>
        /// <param name="filePath">Path to save the file to</param>
        public void SaveToFile(string filePath)
        {
            File.AppendAllText(filePath, this.DumpPacket());
        }

        /// <summary>
        ///     Sends the packet
        /// </summary>
        /// <param name="channel">Channel to send the packet on</param>
        /// <param name="packetFlags">Protocol to send to packet on</param>
        public void SendPacket(
            PacketChannel channel = PacketChannel.C2S, 
            PacketProtocolFlags packetFlags = PacketProtocolFlags.Reliable)
        {
            if (this.Process)
            {
                Game.SendPacket(this.PacketData, channel, packetFlags);
            }
        }

        /// <summary>
        ///     Writes a byte.
        /// </summary>
        /// <param name="byte">Byte to write</param>
        /// <param name="repeat">Times to write the byte</param>
        public void WriteByte(byte @byte, int repeat = 1)
        {
            for (var i = 0; i < repeat; i++)
            {
                this.Writer.Write(@byte);
            }
        }

        /// <summary>
        ///     Writes a float.
        /// </summary>
        /// <param name="f">Float to write</param>
        public void WriteFloat(float f)
        {
            this.Writer.Write(f);
        }

        /// <summary>
        ///     Writes the hex string as bytes to the packet.
        /// </summary>
        /// <param name="hex">Hex string to write</param>
        public void WriteHexString(string hex)
        {
            hex = hex.Trim();
            this.Writer.Write(
                Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray());
        }

        /// <summary>
        ///     Writes an integer.
        /// </summary>
        /// <param name="i">Integer to write</param>
        public void WriteInteger(int i)
        {
            this.Writer.Write(i);
        }

        /// <summary>
        ///     Writes a short.
        /// </summary>
        /// <param name="s">Short to write</param>
        public void WriteShort(short s)
        {
            this.Writer.Write(s);
        }

        /// <summary>
        ///     Writes the string.
        /// </summary>
        /// <param name="str">String to write</param>
        public void WriteString(string str)
        {
            this.Writer.Write(Encoding.UTF8.GetBytes(str));
        }

        #endregion
    }

    /// <summary>
    ///     GamePacket Data.
    /// </summary>
    internal sealed class GamePacketData : IDisposable
    {
        #region Fields

        /// <summary>
        ///     Memory Stream.
        /// </summary>
        private MemoryStream ms;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacketData" /> class.
        ///     Constructor.
        /// </summary>
        public GamePacketData()
        {
            this.CreatePacket(null);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacketData" /> class.
        ///     Constructor.
        /// </summary>
        /// <param name="data">
        ///     Packet Data
        /// </param>
        public GamePacketData(byte[] data)
        {
            this.CreatePacket(data);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacketData" /> class.
        ///     Constructor.
        /// </summary>
        /// <param name="args">
        ///     GamePacket Event Arguments Container
        /// </param>
        public GamePacketData(GamePacketEventArgs args)
        {
            this.CreatePacket(args.PacketData);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePacketData" /> class.
        ///     Constructor.
        /// </summary>
        /// <param name="id">
        ///     Short Value
        /// </param>
        public GamePacketData(short id)
        {
            this.CreatePacket(BitConverter.GetBytes(id));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Packet Data.
        /// </summary>
        public byte[] PacketData
        {
            get
            {
                return this.ms.ToArray();
            }
        }

        /// <summary>
        ///     Gets the Binary Reader.
        /// </summary>
        public BinaryReader Reader { get; private set; }

        /// <summary>
        ///     Gets the Binary Writer.
        /// </summary>
        public BinaryWriter Writer { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     GamePacket Data Dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.ms != null)
            {
                this.ms.Dispose();
                this.ms = null;
            }

            if (this.Reader != null)
            {
                this.Reader.Dispose();
                this.Reader = null;
            }

            if (this.Writer != null)
            {
                this.Writer.Dispose();
                this.Writer = null;
            }
        }

        /// <summary>
        ///     Returns the packet identity.
        /// </summary>
        /// <returns>Short value of the packet ID</returns>
        public short GetPacketId()
        {
            return this.PacketData.GetPacketId();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Create a Packet.
        /// </summary>
        /// <param name="data">Packet Data</param>
        private void CreatePacket(byte[] data)
        {
            this.ms = data == null ? new MemoryStream() : new MemoryStream(data);
            this.Reader = new BinaryReader(this.ms);
            this.Writer = new BinaryWriter(this.ms);

            this.Reader.BaseStream.Position = 0;
            this.Writer.BaseStream.Position = 0;
        }

        #endregion
    }
}