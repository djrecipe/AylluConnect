using System;
using System.Collections.Generic;
using System.Text;

namespace AbaciConnect.Relay.Compression
{
    public interface ICompressor
    {
        byte[] Compress(IEnumerable<byte> data);
        byte[] Decompress(IEnumerable<byte> data);
    }
}
