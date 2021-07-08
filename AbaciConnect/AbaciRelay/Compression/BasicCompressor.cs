using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.Compression
{
    public class BasicCompressor : ICompressor
    {
        public byte[] Compress(IEnumerable<byte> data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data.ToArray(), 0, data.Count());
            }
            return output.ToArray();
        }

        public byte[] Decompress(IEnumerable<byte> data)
        {
            MemoryStream input = new MemoryStream(data.ToArray());
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
