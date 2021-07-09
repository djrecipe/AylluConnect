using AbaciConnect.Relay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionObjectHeader : IPackable, IUnpackable
    {
        public const int SIZE = 13;
        public byte StartByte { get; private set;} = 0;
        public uint ID { get; private set;} = 0;
        public uint PacketCount { get; private set; } = 0;
        public uint Type { get; private set; } = 0;
        public TransmissionObjectHeader()
        {

        }
        public TransmissionObjectHeader(uint id, uint packet_count, uint type)
        {
            this.StartByte = CONSTANTS.XM_HEADER_START_BYTE;
            this.ID = id;
            this.PacketCount = packet_count;
            this.Type = type;
        }
        public List<byte> Pack()
        {
            byte[] bytes = new byte[TransmissionObjectHeader.SIZE];
            //
            bytes[0] = this.StartByte;
            //
            bytes[1] = (byte)(this.ID >> 24);
            bytes[2] = (byte)(this.ID >> 16);
            bytes[3] = (byte)(this.ID >> 8);
            bytes[4] = (byte)(this.ID);
            //
            bytes[5] = (byte)(this.PacketCount >> 24);
            bytes[6] = (byte)(this.PacketCount >> 16);
            bytes[7] = (byte)(this.PacketCount >> 8);
            bytes[8] = (byte)(this.PacketCount);
            //
            bytes[9] = (byte)(this.Type >> 24);
            bytes[10] = (byte)(this.Type >> 16);
            bytes[11] = (byte)(this.Type >> 8);
            bytes[12] = (byte)(this.Type);
            return bytes.ToList();
        }
        public void Unpack(List<byte> bytes)
        {
            if(bytes.Count < TransmissionObjectHeader.SIZE)
                throw new Exception($"Invalid byte count {bytes.Count} to unpack transmission data header");
            this.StartByte = bytes[0];
            if(this.StartByte != CONSTANTS.XM_HEADER_START_BYTE)
                throw new Exception($"Invalid start byte {this.StartByte} while unpacking transmission data header");
            //
            this.ID = (uint)((uint)bytes[1] << 24);
            this.ID |= (uint)((uint)bytes[2] << 16);
            this.ID |= (uint)((uint)bytes[3] << 8);
            this.ID |= (uint)((uint)bytes[4]);
            //
            this.PacketCount = (uint)((uint)bytes[5] << 24);
            this.PacketCount |= (uint)((uint)bytes[6] << 16);
            this.PacketCount |= (uint)((uint)bytes[7] << 8);
            this.PacketCount |= (uint)((uint)bytes[8]);
            //
            this.Type = (uint)((uint)bytes[9] << 24);
            this.Type |= (uint)((uint)bytes[10] << 16);
            this.Type |= (uint)((uint)bytes[11] << 8);
            this.Type |= (uint)((uint)bytes[12]);
        }
    }
}
