using AbaciConnect.Relay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionChunkHeader : IPackable, IUnpackable
    {
        public const int SIZE = 10;
        public byte StartByte { get; private set; } = 0;
        public uint ObjectID { get; private set; } = 0;
        public uint ID { get; private set; } = 0;
        public byte Length { get; private set; } = 0;
        public TransmissionChunkHeader()
        {

        }
        public TransmissionChunkHeader(uint object_id, uint id, byte length)
        {
            this.StartByte = CONSTANTS.XM_PACKET_HEADER_START_BYTE;
            this.ObjectID = object_id;
            this.ID = id;
            this.Length = length;
        }
        public List<byte> Pack()
        {
            byte[] bytes = new byte[TransmissionChunkHeader.SIZE];
            //
            bytes[0] = this.StartByte;
            //
            bytes[1] = (byte)(this.ObjectID >> 24);
            bytes[2] = (byte)(this.ObjectID >> 16);
            bytes[3] = (byte)(this.ObjectID >> 8);
            bytes[4] = (byte)(this.ObjectID);
            //
            bytes[5] = (byte)(this.ID >> 24);
            bytes[6] = (byte)(this.ID >> 16);
            bytes[7] = (byte)(this.ID >> 8);
            bytes[8] = (byte)(this.ID);
            //
            bytes[9] = (byte)(this.Length);
            return bytes.ToList();
        }
        public void Unpack(List<byte> bytes)
        {
            if (bytes.Count < TransmissionChunkHeader.SIZE)
                throw new Exception($"Invalid byte count {bytes.Count} to unpack transmission chunk header");
            this.StartByte = bytes[0];
            if (this.StartByte != CONSTANTS.XM_PACKET_HEADER_START_BYTE)
                throw new Exception($"Invalid start byte {this.StartByte} while unpacking transmission chunk header");
            //
            this.ObjectID = (uint)((uint)bytes[1] << 24);
            this.ObjectID |= (uint)((uint)bytes[2] << 16);
            this.ObjectID |= (uint)((uint)bytes[3] << 8);
            this.ObjectID |= (uint)((uint)bytes[4]);
            //
            this.ID = (uint)((uint)bytes[5] << 24);
            this.ID |= (uint)((uint)bytes[6] << 16);
            this.ID |= (uint)((uint)bytes[7] << 8);
            this.ID |= (uint)((uint)bytes[8]);
            //
            this.Length = bytes[9];
        }
    }
}
