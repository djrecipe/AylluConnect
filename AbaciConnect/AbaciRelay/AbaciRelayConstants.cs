using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


[assembly: InternalsVisibleTo("AbaciConnect.RelayTests")]
namespace AbaciConnect.Relay
{
    internal class CONSTANTS
    {
        internal const int EMISSION_HEADER_SIZE = 4;
        internal const int XTRANSMIT_RESPONSE_SIZE = 7;
        internal const int PACKET_TYPE_OFFSET = 3;
        internal const byte START_BYTE = 0x7e;
        internal const byte FT_ATCMD = 0x08;
        internal const byte FT_ATCMDQ = 0x09;
        internal const byte FT_TRANSMIT = 0x10;
        internal const byte EM_XTRANSMIT_STATUS = 0x8B;
        internal const byte EM_TRANSMIT_STATUS = 0x89;
        internal const byte EM_RECEIVE_PACKET = 0x90;
    }
}
