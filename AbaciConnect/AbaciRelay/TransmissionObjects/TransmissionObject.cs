using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciConnect.Relay;
using AbaciConnect.Relay.Common;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionObject : IPackable
    {
        public TransmissionObjectHeader Header { get; private set; } = new TransmissionObjectHeader();
        public List<TransmissionChunk> Packets { get; private set;} = new List<TransmissionChunk>();
        public TransmissionObject(TransmissionObjectHeader header, IEnumerable<TransmissionChunk> data)
        {
            this.Header = header;
            this.Packets = new List<TransmissionChunk>(data);
        }
        public List<byte> Pack()
        {
            List<byte> bytes = this.Header.Pack();
            foreach(TransmissionChunk chunk in this.Packets.OrderBy(p => p.Header.ID))
            {
                bytes.AddRange(chunk.Pack());
            }
            return bytes;
        }
    }
}
