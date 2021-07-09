using AbaciConnect.Relay.Common;
using AbaciConnect.Relay.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciConnect.Relay.TransmissionObjects
{
    public class TransmissionObjectFactory
    {
        private readonly ICompressor compressor = null;
        public TransmissionObjectFactory(ICompressor compressor)
        {
            this.compressor = compressor ?? new NonCompressor();
        }
        public TransmissionObject Create(byte[] data_in, uint object_id)
        {
            byte[] data_cmpr = this.compressor.Compress(data_in);
            List<TransmissionChunk> chunks = new List<TransmissionChunk>();
            // determine max number of raw data bytes per chunk
            uint max_chunk_size = CONSTANTS.MAX_FRAME_DATA - TransmissionChunk.OVERHEAD_SIZE;
            // determine number of chunks
            uint chunk_count = (uint)data_cmpr.Length/ max_chunk_size;
            if(data_cmpr.Length% max_chunk_size > 0)
                chunk_count++;
            // create chunks
            for(uint i =0; i< chunk_count; i++)
            {
                // determine offset and byte count
                uint current_offset = i* max_chunk_size;
                byte current_count = (byte)Math.Min(max_chunk_size, data_cmpr.Length - current_offset);
                // create chunk
                TransmissionChunkHeader chunk_header = new TransmissionChunkHeader(object_id, i, current_count);
                IEnumerable<byte> current_bytes = data_cmpr.Skip((int)current_offset).Take(current_count);
                TransmissionChunk packet = new TransmissionChunk(chunk_header, current_bytes);
                chunks.Add(packet);
            }
            // create transmission object
            TransmissionObjectHeader object_header = new TransmissionObjectHeader(object_id, chunk_count, 0);
            TransmissionObject transmission_object = new TransmissionObject(object_header, chunks);
            return transmission_object;
        }
        public byte[] GetData(TransmissionObject obj)
        {
            List<byte> bytes_cmpr = new List<byte>();
            foreach(TransmissionChunk packet in obj.Chunks.OrderBy(p => p.Header.ID))
            {
                bytes_cmpr.AddRange(packet.Data);
            }
            byte[] bytes = this.compressor.Decompress(bytes_cmpr);
            return bytes;
        }
    }
}
