#region

using System;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace LeagueSharp.CommonEx.Core.Network
{
    /// <summary>
    ///     This class makes easier to handle packets.
    /// </summary>
    public class GamePacket
    {
        /// <summary>
        ///     Packet Header
        /// </summary>
        private readonly short _header;

        /// <summary>
        ///     Packet Memory Stream
        /// </summary>
        private readonly MemoryStream _memoryStream;

        /// <summary>
        ///     Packet Binary Reader instance
        /// </summary>
        private readonly BinaryReader _reader;

        /// <summary>
        ///     Packet Binary Writer instance
        /// </summary>
        private readonly BinaryWriter _writer;

        /// <summary>
        ///     The channel this packet is sent on.
        /// </summary>
        public PacketChannel Channel = PacketChannel.C2S;

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
            Block = false;
            _memoryStream = new MemoryStream(data);
            _reader = new BinaryReader(_memoryStream);
            _writer = new BinaryWriter(_memoryStream);

            _reader.BaseStream.Position = 0;
            _writer.BaseStream.Position = 0;
            var rawPacket = data;
            _header = (short) ((rawPacket[1] << 8) | (rawPacket[0] << 0));
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with the event arguements for packets.
        /// </summary>
        /// <param name="args"></param>
        public GamePacket(GamePacketEventArgs args)
        {
            Block = false;
            _memoryStream = new MemoryStream(args.PacketData);
            _reader = new BinaryReader(_memoryStream);
            _writer = new BinaryWriter(_memoryStream);

            _reader.BaseStream.Position = 0;
            _writer.BaseStream.Position = 0;
            var rawPacket = args.PacketData;
            _header = (short) ((rawPacket[1] << 8) | (rawPacket[0] << 0));
            Channel = args.Channel;
            Flags = args.ProtocolFlag;
        }

        /// <summary>
        ///     Creates a <see cref="GamePacket" /> with a header, channel, and protocol.
        /// </summary>
        /// <param name="header">Header of the packet</param>
        /// <param name="channel">Channel</param>
        /// <param name="flags">Protocol to send packet</param>
        public GamePacket(short header,
            PacketChannel channel = PacketChannel.C2S,
            PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            Block = false;
            _memoryStream = new MemoryStream();
            _reader = new BinaryReader(_memoryStream);
            _writer = new BinaryWriter(_memoryStream);

            _reader.BaseStream.Position = 0;
            _writer.BaseStream.Position = 0;
            WriteShort(header);
            _header = header;
            Channel = channel;
            Flags = flags;
        }

        /// <summary>
        ///     Gets the Header of the packet(The first byte)
        /// </summary>
        public short Header
        {
            get { return _header; }
        }

        /// <summary>
        ///     Gets/Sets the position of the reader.
        /// </summary>
        public long Position
        {
            get { return _reader.BaseStream.Position; }
            set
            {
                if (value >= 0L)
                {
                    _reader.BaseStream.Position = value;
                }
            }
        }

        /// <summary>
        ///     Blocks sending and processing the packet.
        /// </summary>
        public bool Block { get; set; }

        /// <summary>
        ///     Returns the packet size.
        /// </summary>
        public long Size()
        {
            return _reader.BaseStream.Length;
        }

        /// <summary>
        ///     Reads a byte from the packet and increases the position by 1.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read byte</returns>
        public byte ReadByte(long position = -1)
        {
            Position = position;
            return _reader.ReadBytes(1)[0];
        }

        /// <summary>
        ///     Reads and returns a double byte.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read short</returns>
        public short ReadShort(long position = -1)
        {
            Position = position;
            return BitConverter.ToInt16(_reader.ReadBytes(2), 0);
        }

        /// <summary>
        ///     Reads and returns a float.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read float</returns>
        public float ReadFloat(long position = -1)
        {
            Position = position;
            return BitConverter.ToSingle(_reader.ReadBytes(4), 0);
        }

        /// <summary>
        ///     Reads and returns an integer.
        /// </summary>
        /// <param name="position">Position to read at</param>
        /// <returns>The read integer</returns>
        public int ReadInteger(long position = -1)
        {
            Position = position;
            return BitConverter.ToInt32(_reader.ReadBytes(4), 0);
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

            for (var i = Position; i < Size(); i++)
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
                _writer.Write(@byte);
            }
        }

        /// <summary>
        ///     Writes a short.
        /// </summary>
        /// <param name="s">Short to write</param>
        public void WriteShort(short s)
        {
            _writer.Write(s);
        }

        /// <summary>
        ///     Writes a float.
        /// </summary>
        /// <param name="f">Float to write</param>
        public void WriteFloat(float f)
        {
            _writer.Write(f);
        }

        /// <summary>
        ///     Writes an integer.
        /// </summary>
        /// <param name="i">Integer to write</param>
        public void WriteInteger(int i)
        {
            _writer.Write(i);
        }

        /// <summary>
        ///     Writes the hex string as bytes to the packet.
        /// </summary>
        /// <param name="hex">Hex string to write</param>
        public void WriteHexString(string hex)
        {
            hex = hex.Replace(" ", string.Empty);

            if ((hex.Length % 2) != 0)
            {
                hex = "0" + hex;
            }

            _writer.Write(
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
            _writer.Write(Encoding.UTF8.GetBytes(str));
        }

        /* public int[] SearchByte(byte num)
        {
            //return rawPacket.IndexOf(new byte[num]).ToArray();
            return rawPacket.IndexOf(BitConverter.GetBytes(num)).ToArray();
        }

        public int[] SearchShort(short num)
        {
            return rawPacket.IndexOf(BitConverter.GetBytes(num)).ToArray();
        }

        public int[] SearchFloat(float num)
        {
            return rawPacket.IndexOf(BitConverter.GetBytes(num)).ToArray();
        }

        public int[] SearchInteger(int num)
        {
            return rawPacket.IndexOf(BitConverter.GetBytes(num)).ToArray();
        }*/

        /*public int[] SearchString(string str)
        {
            return rawPacket.IndexOf(Utils.GetBytes(str)).ToArray();
        }*/

        /*public int[] SearchHexString(string hex)
        {
            hex = hex.Replace(" ", string.Empty);

            if ((hex.Length % 2) != 0)
            {
                hex = "0" + hex;
            }

            return
                rawPacket.IndexOf(
                    Enumerable.Range(0, hex.Length)
                        .Where(x => x % 2 == 0)
                        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                        .ToArray()).ToArray();
        }*/

        /// <summary>
        ///     Returns the raw packet bytes
        /// </summary>
        /// <returns>Bytes of the packet</returns>
        public byte[] GetRawPacket()
        {
            return _memoryStream.ToArray();
        }

        /// <summary>
        ///     Sends the packet
        /// </summary>
        /// <param name="channel">Channel to send the packet on</param>
        /// <param name="flags">Protocol to send to packet on</param>
        public void Send(PacketChannel channel = PacketChannel.C2S,
            PacketProtocolFlags flags = PacketProtocolFlags.Reliable)
        {
            if (!Block)
            {
                Game.SendPacket(
                    _memoryStream.ToArray(), Channel == PacketChannel.C2S ? channel : Channel,
                    Flags == PacketProtocolFlags.Reliable ? flags : Flags);
            }
        }

        /// <summary>
        ///     Receives the packet.
        /// </summary>
        /// <param name="channel">Channel to process the packet on</param>
        public void Process(PacketChannel channel = PacketChannel.S2C)
        {
            if (!Block)
            {
                Game.ProcessPacket(_memoryStream.ToArray(), channel);
            }
        }

        /// <summary>
        ///     Dumps the packet.
        /// </summary>
        /// <param name="additionalInfo">Prints the data, channel, and flags(if true)</param>
        public string Dump(bool additionalInfo = false)
        {
            var s = string.Concat(_memoryStream.ToArray().Select(b => b.ToString("X2") + " "));
            if (additionalInfo)
            {
                s = "Data: " + s + " Channel: " + Channel + " Flags: " + Flags;
            }
            return s;
        }

        /// <summary>
        ///     Saves the packet dump to a file
        /// </summary>
        /// <param name="filePath">Path to save the file to</param>
        public void SaveToFile(string filePath)
        {
            var w = File.AppendText(filePath);

            w.WriteLine(Dump(true));
            w.Close();
        }
    }
}