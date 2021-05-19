using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    internal class CONSTANTS
    {
        internal static readonly byte START_BYTE = 0x7e;
        internal static readonly byte FT_ATCMD = 0x08;
        internal static readonly byte FT_ATCMDQ = 0x09;
        internal static readonly byte FT_TRANSMIT = 0x10;
    }
}
