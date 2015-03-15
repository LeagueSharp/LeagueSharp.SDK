#region LICENSE

/*
 Copyright 2014 - 2015 LeagueSharp
 GamePacket.cs is part of LeagueSharp.CommonEx.
 
 LeagueSharp.Common is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 LeagueSharp.Common is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with LeagueSharp.Common. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region

using System;
using System.IO;
using System.Linq;
using System.Text;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.Network
{
    /// <summary>
    ///     This class makes easier to handle packets.
    /// </summary>
    public class GamePacket
    {
        /// <summary>
        ///     Game Packet Data
        /// </summary>
        private readonly GamePacketData packetData;

        /// <summary>
        ///     The channel this packet is sent on.
        /// </summary>
        public PacketChannel Channel;

        /// <summary>
        ///     The protocol that packet is sent on.
        /// </summary>
        public PacketProtocolFlags Flags = PacketProtocolFlags.Reliable;

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with the packet data.
        /// </summary>
        /// <param name="data">Bytes of the packet</param>
        public GamePacket(byte[] data)
        {
            packetData = new GamePacketData(data);
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with the event arguements for packets.
        /// </summary>
        /// <param name="args"></param>
        public GamePacket(GamePacketEventArgs args)
        {
            packetData = new GamePacketData(args);
            Channel = args.Channel;
            Flags = args.ProtocolFlag;
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> from a packet.
        /// </summary>
        /// <param name="packet">Packet</param>
        public GamePacket(Packet packet)
        {
            packetData = new GamePacketData(packet.Id);
            Channel = packet.Channel;
            Flags = packet.Flags;
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with a header, channel, and protocol.
        /// </summary>
        /// <param name="id">Id of the packet</param>
        /// <param name="channel">Channel</param>
        /// <param name="flags">Protocol to send packet</param>
        public GamePacket(short id, PacketChannel channel, PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            packetData = new GamePacketData(id);
            Channel = channel;
            Flags = flags;
        }

        /// <summary>
        ///     Gets the Header of the packet(The first byte)
        /// </summary>
        public short PacketId
        {
            get { return packetData.GetPacketId(); }
        }

        /// <summary>
        ///     Gets/Sets the position of the reader.
        /// </summary>
        public long Position
        {
            get { return Reader.BaseStream.Position; }
            set
            {
                if (value >= 0L)
                {
                    Reader.BaseStream.Position = value;
                }
            }
        }

        private BinaryWriter Writer
        {
            get { return packetData.Writer; }
        }

        private BinaryReader Reader
        {
            get { return packetData.Reader; }
        }

        /// <summary>
        ///     Blocks sending and processing the packet.
        /// </summary>
        public bool Process { get; set; }

        /// <summary>
        ///     Returns the packet size.
        /// </summary>
        public long Size
        {
            get { return Reader.BaseStream.Length; }
        }

        /// <summary>
        ///     Returns the raw packet bytes
        /// </summary>
        /// <returns>Bytes of the packet</returns>
        public byte[] PacketData
        {
            get { return packetData.PacketData; }
        }

        /// <summary>
        ///     Reads a byte from the packet and increases the position by 1.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read byte</returns>
        public byte ReadByte(long position = -1)
        {
            Position = position;
            return Reader.ReadBytes(1)[0];
        }

        /// <summary>
        ///     Reads and returns a double byte.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read short</returns>
        public short ReadShort(long position = -1)
        {
            Position = position;
            return BitConverter.ToInt16(Reader.ReadBytes(2), 0);
        }

        /// <summary>
        ///     Reads and returns a float.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read float</returns>
        public float ReadFloat(long position = -1)
        {
            Position = position;
            return BitConverter.ToSingle(Reader.ReadBytes(4), 0);
        }

        /// <summary>
        ///     Reads and returns an integer.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read integer</returns>
        public int ReadInteger(long position = -1)
        {
            Position = position;
            return BitConverter.ToInt32(Reader.ReadBytes(4), 0);
        }

        /// <summary>
        ///     Reads and returns a string.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read string</returns>
        public string ReadString(long position = -1)
        {
            Position = position;
            var sb = new StringBuilder();

            for (var i = Position; i < Size; i++)
            {
                var num = ReadByte();

                if (num == 0)
                {
                    return sb.ToString();
                }
                sb.Append(Convert.ToChar(num));
            }

            return sb.ToString();
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
                Writer.Write(@byte);
            }
        }

        /// <summary>
        ///     Writes a short.
        /// </summary>
        /// <param name="s">Short to write</param>
        public void WriteShort(short s)
        {
            Writer.Write(s);
        }

        /// <summary>
        ///     Writes a float.
        /// </summary>
        /// <param name="f">Float to write</param>
        public void WriteFloat(float f)
        {
            Writer.Write(f);
        }

        /// <summary>
        ///     Writes an integer.
        /// </summary>
        /// <param name="i">Integer to write</param>
        public void WriteInteger(int i)
        {
            Writer.Write(i);
        }

        /// <summary>
        ///     Writes the hex string as bytes to the packet.
        /// </summary>
        /// <param name="hex">Hex string to write</param>
        public void WriteHexString(string hex)
        {
            hex = hex.Trim();
            Writer.Write(
                Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray());
        }

        /// <summary>
        ///     Writes the string.
        /// </summary>
        /// <param name="str">String to write</param>
        public void WriteString(string str)
        {
            Writer.Write(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        ///     Sends the packet
        /// </summary>
        /// <param name="channel">Channel to send the packet on</param>
        /// <param name="flags">Protocol to send to packet on</param>
        public void SendPacket(PacketChannel channel = PacketChannel.C2S,
            PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            if (Process)
            {
                Game.SendPacket(PacketData, channel, flags);
            }
        }

        /// <summary>
        ///     Receives the packet.
        /// </summary>
        /// <param name="channel">Channel to process the packet on</param>
        public void ProcessPacket(PacketChannel channel = PacketChannel.S2C)
        {
            if (Process)
            {
                Game.ProcessPacket(PacketData, channel);
            }
        }

        /// <summary>
        ///     Dumps the packet.
        /// </summary>
        /// <param name="simple">Dumps only the raw data if true.</param>
        public string DumpPacket(bool simple = false)
        {
            var bytes = string.Concat(PacketData.Select(b => b.ToString("X2") + " "));

            if (simple)
            {
                return bytes;
            }

            bytes = "Data: " + bytes;
            var channel = "Channel: " + Channel;
            var flags = "Flags: " + Flags;

            return bytes + "\n" + channel + "\n" + flags + "\n";
        }

        /// <summary>
        ///     Saves the packet dump to a file
        /// </summary>
        /// <param name="filePath">Path to save the file to</param>
        public void SaveToFile(string filePath)
        {
            File.AppendAllText(filePath, DumpPacket());
        }
    }

    internal class GamePacketData
    {
        private MemoryStream ms;

        public GamePacketData()
        {
            CreatePacket(null);
        }

        public GamePacketData(byte[] data)
        {
            CreatePacket(data);
        }

        public GamePacketData(GamePacketEventArgs args)
        {
            CreatePacket(args.PacketData);
        }

        public GamePacketData(short id)
        {
            CreatePacket(BitConverter.GetBytes(id));
        }

        public BinaryReader Reader { get; private set; }
        public BinaryWriter Writer { get; private set; }

        public byte[] PacketData
        {
            get { return ms.ToArray(); }
        }

        private void CreatePacket(byte[] data)
        {
            ms = data == null ? new MemoryStream() : new MemoryStream(data);
            Reader = new BinaryReader(ms);
            Writer = new BinaryWriter(ms);

            Reader.BaseStream.Position = 0;
            Writer.BaseStream.Position = 0;
        }

        public short GetPacketId()
        {
            return PacketData.GetPacketId();
        }
    }
}