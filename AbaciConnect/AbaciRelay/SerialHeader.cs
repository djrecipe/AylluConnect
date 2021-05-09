using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay
{
    public enum DataTypes : byte
    {
        Bytes = 0,
        String = 1,
        File = 2
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SerialHeader
    {
        public byte Marker1;
        public byte Marker2;
        public DataTypes Type;
        public long Length;
    }
}
