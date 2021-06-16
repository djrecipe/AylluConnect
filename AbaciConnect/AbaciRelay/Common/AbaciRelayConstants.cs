using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


[assembly: InternalsVisibleTo("AbaciConnectTests")]
namespace AbaciConnect.Relay.Common
{
    internal class CONSTANTS
    {
        internal const int MAX_FRAME_DATA = 84;
        internal const int EMISSION_HEADER_SIZE = 4;
        internal const int PACKET_TYPE_OFFSET = 3;
        internal const byte FRAME_START_BYTE = 0x7e;
        internal const byte XM_HEADER_START_BYTE = 0xDD;
        internal const byte XM_PACKET_HEADER_START_BYTE = 0xBC;
        internal const byte FT_ATCMD = 0x08;
        internal const byte FT_ATCMDQ = 0x09;
        internal const byte FT_TRANSMIT = 0x10;
        internal const byte EM_XTRANSMIT_STATUS = 0x8B;
        internal const byte EM_TRANSMIT_STATUS = 0x89;
        internal const byte EM_RECEIVE_PACKET = 0x90;
        internal const string DISCOVER = "Discover";
    }
}
