using System;
using System.Collections.Generic;
using System.Linq;
using AbaciConnect.Relay.Common;

namespace AbaciConnect.Relay.Emission
{
    public class ReceivePacketEmission : IUnpackable
    {
        public ulong LongAddress {get; private set;} = 0;
        public ushort ShortAddress { get; private set;} = 0;
        public byte ReceiveOptions { get; private set;} = 0;
        public List<byte> Data { get; private set;} = new List<byte>();
        public byte Checksum { get; private set; } = 0;
        public void Unpack(List<byte> bytes)
        {
            this.LongAddress = (ulong)((ulong)bytes[0] << 56);
            this.LongAddress |= (ulong)((ulong)bytes[1] << 48);
            this.LongAddress |= (ulong)((ulong)bytes[2] << 40);
            this.LongAddress |= (ulong)((ulong)bytes[3] << 32);
            this.LongAddress |= (ulong)((ulong)bytes[4] << 24);
            this.LongAddress |= (ulong)((ulong)bytes[5] << 16);
            this.LongAddress |= (ulong)((ulong)bytes[6] << 8);
            this.LongAddress |= (ulong)bytes[7];
            this.ShortAddress |= (ushort)((ushort)bytes[8] << 8);
            this.ShortAddress |= (ushort)bytes[9];
            this.ReceiveOptions = bytes[10];
            int data_offset = 11;
            int data_length = bytes.Count - (data_offset+1); // skip first 11 bytes and last byte
            this.Data = bytes.Skip(data_offset).Take(data_length).ToList();
            this.Checksum = bytes[bytes.Count - 1];

        }
    }
}
