using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Compression
{
    public class NonCompressor : ICompressor
    {
        public byte[] Compress(IEnumerable<byte> data)
        {
            return data.ToArray();
        }

        public byte[] Decompress(IEnumerable<byte> data)
        {
            return data.ToArray();
        }
    }
}
