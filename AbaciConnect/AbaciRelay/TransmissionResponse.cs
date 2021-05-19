using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AbaciConnect.Relay
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 11)]
    public struct TransmissionResponse
    {
        public byte StartByte;
        public byte LengthH;
        public byte LengthL;
        public byte FrameType;
        public byte FrameID;
        public byte AddressH;
        public byte AddressL;
        public byte RetryCount;
        public byte DeliveryStatus;
        public byte DiscoveryStatus;
        public byte Checksum;
    }
}
