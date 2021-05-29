using AbaciConnect.Relay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionChunk : IPackable, IUnpackable
    {
        public const int OVERHEAD_SIZE = TransmissionChunkHeader.SIZE + 1;
        public TransmissionChunkHeader Header { get; private set; } = new TransmissionChunkHeader();
        public List<byte> Data { get; private set; } = new List<byte>();
        public byte Checksum { get; private set; } = 0;
        public bool ValidChecksum { get; private set;} =false;
        public TransmissionChunk()
        {

        }
        public TransmissionChunk(TransmissionChunkHeader header, IEnumerable<byte> data)
        {
            this.Data = new List<byte>(data);
            this.Header = header;
            Checksum = Common.Checksum.Calculate(this.Data);
            this.ValidChecksum = true;
        }
        public List<byte> Pack()
        {
            List<byte> bytes = this.Header.Pack();
            bytes.AddRange(this.Data);
            bytes.Add(this.Checksum);
            return bytes;
        }

        public void Unpack(List<byte> bytes)
        {
            // unpack header
            this.Header.Unpack(bytes);
            // check total size
            if (bytes.Count < TransmissionChunk.OVERHEAD_SIZE + (int)this.Header.Length )
                throw new Exception($"Invalid byte count {bytes.Count} to unpack transmission chunk");
            this.Data = bytes.Skip(TransmissionChunkHeader.SIZE).Take((int)this.Header.Length).ToList();
            this.Checksum = bytes[bytes.Count - 1];
            byte expected_checksum = Common.Checksum.Calculate(this.Data);
            this.ValidChecksum = this.Checksum == expected_checksum;
        }
    }
}
