using AbaciConnect.Relay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionObjectFactory
    {
        public TransmissionObject Create(byte[] data, uint object_id)
        {
            List<TransmissionChunk> chunks = new List<TransmissionChunk>();
            // determine max number of raw data bytes per chunk
            uint max_chunk_size = CONSTANTS.MAX_FRAME_DATA - TransmissionChunk.OVERHEAD_SIZE;
            // determine number of chunks
            uint chunk_count = (uint)data.Length/ max_chunk_size;
            if(data.Length% max_chunk_size > 0)
                chunk_count++;
            // create chunks
            for(uint i =0; i< chunk_count; i++)
            {
                // determine offset and byte count
                uint current_offset = i* max_chunk_size;
                byte current_count = (byte)Math.Min(max_chunk_size, data.Length - current_offset);
                // create chunk
                TransmissionChunkHeader chunk_header = new TransmissionChunkHeader(object_id, i, current_count);
                IEnumerable<byte> current_bytes = data.Skip((int)current_offset).Take(current_count);
                TransmissionChunk packet = new TransmissionChunk(chunk_header, current_bytes);
                chunks.Add(packet);
            }
            // create transmission object
            TransmissionObjectHeader object_header = new TransmissionObjectHeader(object_id, chunk_count, 0);
            TransmissionObject transmission_object = new TransmissionObject(object_header, chunks);
            return transmission_object;
        }
        public List<byte> GetDataBytes(TransmissionObject obj)
        {
            List<byte> bytes = new List<byte>();
            foreach(TransmissionChunk packet in obj.Chunks.OrderBy(p => p.Header.ID))
            {
                bytes.AddRange(packet.Data);
            }
            return bytes;
        }
    }
}
