using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SerialHeader
    {
        public byte Marker1;
        public byte Marker2;
        public long Length;
    }
}
