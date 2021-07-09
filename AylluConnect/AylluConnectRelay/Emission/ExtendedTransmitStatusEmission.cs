using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AbaciConnect.Relay.Common;

namespace AbaciConnect.Relay.Emission
{
    public class ExtendedTransmitStatusEmission : IUnpackable
    {
        public byte FrameID { get; private set; } = 0;
        public ushort Address { get; private set; } = 0;
        public byte RetryCount { get; private set; } = 0;
        public byte DeliveryStatus { get; private set; } = 0;
        public byte DiscoveryStatus { get; private set; } = 0;
        public byte Checksum { get; private set; } = 0;
        public void Unpack(List<byte> bytes)
        {
            if (bytes.Count < 7)
                throw new Exception($"Invalid byte count {bytes.Count} to unpack emission header");
            this.FrameID = bytes[0];
            this.Address = (ushort)(((ushort)bytes[1] << 8) | bytes[2]);
            this.RetryCount = bytes[3];
            this.DeliveryStatus = bytes[4];
            this.DiscoveryStatus = bytes[5];
            this.Checksum = bytes[6];
        }
    }
}
