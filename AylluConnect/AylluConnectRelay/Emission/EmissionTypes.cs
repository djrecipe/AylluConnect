using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Emission
{
    public enum EmissionTypes : byte
    {
        None = 0x00,
        ExtendedTransmitStatus = 0x8B,
        ReceivePacket = 0x90,
        ExplicitReceiveIndicator
    }
}
